using AutoMapper;
using finance_management.AutoCategorize;
using finance_management.Database;
using finance_management.DTOs.GetTransactions;
using finance_management.Interfaces;
using finance_management.Models;
using finance_management.Models.Enums;
using finance_management.Queries.GetTransactions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace finance_management.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly PfmDbContext _context;
        private readonly IMapper _mapper;

        public TransactionRepository(PfmDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<Transaction>> GetAllAsync()
        {
            return await _context.Transactions.ToListAsync();
        }

        public async Task<Transaction?> GetByIdAsync(string id)
        {
            return await _context.Transactions
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id);
        }
        public async Task<List<Transaction>> GetAllWithSplitsAsync()
        {
            return await _context.Transactions
                .Include(t => t.Category)
                .Include(t => t.Splits)
                    .ThenInclude(s => s.Category)
                .ToListAsync();
        }
        public async Task<Transaction> CreateAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<List<Transaction>> CreateBulkAsync(List<Transaction> transactions)
        {
            _context.Transactions.AddRange(transactions);
            await _context.SaveChangesAsync();
            return transactions;
        }

        public async Task<Transaction> UpdateAsync(Transaction transaction)
        {
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task DeleteAsync(string id)
        {
            var transaction = await GetByIdAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.Transactions.AnyAsync(t => t.Id == id);
        }
        public async Task AddAsync(Transaction tx)
        {
            await _context.Transactions.AddAsync(tx);
        }

        public IQueryable<Transaction> Query()
        {
            return _context.Transactions.AsQueryable();
        }
        public async Task UpdateCategoryAsync(string transactionId, string catCode)
        {
            var transaction = await _context.Transactions.FindAsync(transactionId);
            if (transaction != null)
            {
                transaction.CatCode = catCode;
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<string>> GetAllIdsAsync(CancellationToken cancellationToken)
        {
            return await _context.Transactions
                .AsNoTracking()//da bi brzi bio upit ne treba mi da pratim promene jer ne menjam 
                .Select(t => t.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<TransactionPagedList> GetTransactionsAsync(GetTransactionsQuery request, CancellationToken cancellationToken)
        {
            //osnovni IQueryable upit nad transactions tabelom
            var query = _context.Transactions
                .Include(t => t.Splits)
                .Include(t => t.Category)
                .AsQueryable();//omogucava kasnije dodavanje filtera i sortiranje nad ovim upitom

            // poziv metode koja na osnovnu prosledjenog request objekta dodaje where uslove
            query = ApplyFilters(query, request);

            // ukupan br rezultata pre paginacije koji zadovoljavaju uslove, treba za ukupan br strana zbog paginacije
            var totalCount = await query.CountAsync(cancellationToken);

            // dodaj sortiranje
            query = ApplySorting(query, request.SortBy, request.SortOrder);

            // primeni paginaciju
            //racuna broj rezultata koji treba preskociti
            var skip = (request.Page - 1) * request.PageSize;
            //izvrsava upit sa paginacijom
            var transactions = await query
                .Skip(skip)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            // mapiranje transactions u transactionwithsplits using automapper
            var mappedItems = _mapper.Map<List<TransactionWithSplits>>(transactions);

            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            return new TransactionPagedList
            {
                TotalCount = totalCount,
                PageSize = request.PageSize,
                Page = request.Page,
                TotalPages = totalPages,
                SortOrder = request.SortOrder,
                SortBy = request.SortBy ?? "date",
                Items = mappedItems
            };
        }

        private IQueryable<Transaction> ApplyFilters(IQueryable<Transaction> query, GetTransactionsQuery request)
        {
            //provera da li je poslao ispravne vrednosti za kind
            if (!string.IsNullOrWhiteSpace(request.TransactionKind))
            {
                var kinds = request.TransactionKind
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(k => Enum.Parse<TransactionKindEnum>(k, ignoreCase: true))
                    .ToList();

                if (kinds.Any())
                {
                    query = query.Where(t => kinds.Contains(t.Kind));
                }
            }



            // provera da li je poslao vrednosti za date
            if (request.StartDate.HasValue)
            {
                query = query.Where(t => t.Date >= request.StartDate.Value.ToUniversalTime());
            }

            if (request.EndDate.HasValue)
            {
                query = query.Where(t => t.Date <= request.EndDate.Value.ToUniversalTime());
            }

            return query;
        }

        private IQueryable<Transaction> ApplySorting(IQueryable<Transaction> query, string? sortBy, SortOrderEnum sortOrder)
        {
            // Ako nije prosledjen sortBy, koristi default sortiranje po datumu
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return sortOrder == SortOrderEnum.Desc
                    ? query.OrderByDescending(t => t.Date)
                    : query.OrderBy(t => t.Date);
            }
            //moguce je vise polja za sortiranje koja su podeljena zarezom
            var sortFields = sortBy.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(f => f.ToLower().Trim())
                .ToArray();
           
            IOrderedQueryable<Transaction>? orderedQuery = null;

            for (int i = 0; i < sortFields.Length; i++)
            {
                var field = sortFields[i];
                var isDescending = sortOrder == SortOrderEnum.Desc;

                if (i == 0)
                {
                    //orderBy za prvo polje sortiranja
                    orderedQuery = field switch
                    {
                        "id" => isDescending ? query.OrderByDescending(t => t.Id) : query.OrderBy(t => t.Id),
                        "date" => isDescending ? query.OrderByDescending(t => t.Date) : query.OrderBy(t => t.Date),
                        "amount" => isDescending ? query.OrderByDescending(t => t.Amount) : query.OrderBy(t => t.Amount),
                        "beneficiary-name" => isDescending ? query.OrderByDescending(t => t.BeneficiaryName) : query.OrderBy(t => t.BeneficiaryName),
                        "description" => isDescending ? query.OrderByDescending(t => t.Description) : query.OrderBy(t => t.Description),
                        "currency" => isDescending ? query.OrderByDescending(t => t.Currency) : query.OrderBy(t => t.Currency),
                        "mcc" => isDescending ? query.OrderByDescending(t => t.MccCode) : query.OrderBy(t => t.MccCode),
                        "kind" => isDescending ? query.OrderByDescending(t => t.Kind) : query.OrderBy(t => t.Kind),
                        "catcode" => isDescending ? query.OrderByDescending(t => t.CatCode) : query.OrderBy(t => t.CatCode),
                        "direction" => isDescending ? query.OrderByDescending(t => t.Direction) : query.OrderBy(t => t.Direction),
                        _ => isDescending ? query.OrderByDescending(t => t.Date) : query.OrderBy(t => t.Date)
                    };
                }
                else
                {
                    // ThenBy za ostala polja sortiranja
                    orderedQuery = field switch
                    {
                        "id" => isDescending ? orderedQuery!.ThenByDescending(t => t.Id) : orderedQuery!.ThenBy(t => t.Id),
                        "date" => isDescending ? orderedQuery!.ThenByDescending(t => t.Date) : orderedQuery!.ThenBy(t => t.Date),
                        "amount" => isDescending ? orderedQuery!.ThenByDescending(t => t.Amount) : orderedQuery!.ThenBy(t => t.Amount),
                        "beneficiary-name" => isDescending ? orderedQuery!.ThenByDescending(t => t.BeneficiaryName) : orderedQuery!.ThenBy(t => t.BeneficiaryName),
                        "description" => isDescending ? orderedQuery!.ThenByDescending(t => t.Description) : orderedQuery!.ThenBy(t => t.Description),
                        "currency" => isDescending ? orderedQuery!.ThenByDescending(t => t.Currency) : orderedQuery!.ThenBy(t => t.Currency),
                        "mcc" => isDescending ? orderedQuery!.ThenByDescending(t => t.MccCode) : orderedQuery!.ThenBy(t => t.MccCode),
                        "kind" => isDescending ? orderedQuery!.ThenByDescending(t => t.Kind) : orderedQuery!.ThenBy(t => t.Kind),
                        "catcode" => isDescending ? orderedQuery!.ThenByDescending(t => t.CatCode) : orderedQuery!.ThenBy(t => t.CatCode),
                        "direction" => isDescending ? orderedQuery!.ThenByDescending(t => t.Direction) : orderedQuery!.ThenBy(t => t.Direction),
                        _ => orderedQuery // ignorisi nepoznate vrednosti
                    };
                }
            }

            return orderedQuery ?? query.OrderBy(t => t.Date);
        }

        public async Task<int> AutoCategorizeAsync(RulesList rulesList)
        {
            int totalUpdated = 0;

            foreach (var rule in rulesList.Rules)
            {
                var sql = $"SELECT * FROM \"Transactions\" WHERE (" + rule.Predicate + ")::boolean";
                //izvrsavam upit nad bazom koristeci FromSqlRaw
                var transactions = await _context.Transactions
                    .FromSqlRaw(sql)
                    .ToListAsync();

                //proveravam da li ima transakcija koje odgovaraju ovom rule i update
                if (transactions.Any())
                {
                    transactions.ForEach(t => t.CatCode = rule.CatCode);
                    _context.UpdateRange(transactions);
                    totalUpdated += transactions.Count;
                }
            }

            await _context.SaveChangesAsync();
            return totalUpdated;
        }

        
    }
}
