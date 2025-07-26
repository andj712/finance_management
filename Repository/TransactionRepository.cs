using AutoMapper;
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

        public async Task<TransactionPagedList> GetTransactionsAsync(GetTransactionsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Transactions
                                .Include(t => t.Splits)
                                .Include(t => t.Category)
                                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.TransactionKind))
            {
                var kinds = request.TransactionKind
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(s => Enum.Parse<TransactionKindEnum>(s, ignoreCase: true))
                    .ToList();

                if (kinds.Any())
                    query = query.Where(t => kinds.Contains(t.Kind));
            }

            if (request.StartDate.HasValue)
                query = query.Where(t => t.Date >= request.StartDate.Value.ToUniversalTime());
            if (request.EndDate.HasValue)
                query = query.Where(t => t.Date <= request.EndDate.Value.ToUniversalTime());

            var totalCount = await query.CountAsync(cancellationToken);

            var sortFields = (request.SortBy ?? "date")
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(f => f.ToLower())
                .ToArray();

            IQueryable<Transaction> sorted = query;
            string order = request.SortOrder == SortOrderEnum.Desc ? "desc" : "asc";
            for (int i = 0; i < sortFields.Length; i++)
            {
                var field = sortFields[i];
                if (i == 0)
                {
                    switch (field)
                    {
                        case "id":
                            sorted = order == "desc" ? sorted.OrderByDescending(t => t.Id) : sorted.OrderBy(t => t.Id);
                            break;
                        case "date":
                            sorted = order == "desc" ? sorted.OrderByDescending(t => t.Date) : sorted.OrderBy(t => t.Date);
                            break;
                        case "amount":
                            sorted = order == "desc" ? sorted.OrderByDescending(t => t.Amount) : sorted.OrderBy(t => t.Amount);
                            break;
                        case "beneficiary-name":
                            sorted = order == "desc" ? sorted.OrderByDescending(t => t.BeneficiaryName) : sorted.OrderBy(t => t.BeneficiaryName);
                            break;
                        case "description":
                            sorted = order == "desc" ? sorted.OrderByDescending(t => t.Description) : sorted.OrderBy(t => t.Description);
                            break;
                        default:
                            sorted = order == "desc" ? sorted.OrderByDescending(t => t.Date) : sorted.OrderBy(t => t.Date);
                            break;
                    }
                }
                else
                {
                    var orderedQuery = (IOrderedQueryable<Transaction>)sorted;
                    switch (field)
                    {
                        case "id":
                            sorted = order == "desc" ? orderedQuery.ThenByDescending(t => t.Id) : orderedQuery.ThenBy(t => t.Id);
                            break;
                        case "date":
                            sorted = order == "desc" ? orderedQuery.ThenByDescending(t => t.Date) : orderedQuery.ThenBy(t => t.Date);
                            break;
                        case "amount":
                            sorted = order == "desc" ? orderedQuery.ThenByDescending(t => t.Amount) : orderedQuery.ThenBy(t => t.Amount);
                            break;
                        case "beneficiary-name":
                            sorted = order == "desc" ? orderedQuery.ThenByDescending(t => t.BeneficiaryName) : orderedQuery.ThenBy(t => t.BeneficiaryName);
                            break;
                        case "description":
                            sorted = order == "desc" ? orderedQuery.ThenByDescending(t => t.Description) : orderedQuery.ThenBy(t => t.Description);
                            break;
                    }
                }
            }
            query = sorted;


            // Pagination
            var skip = (request.Page - 1) * request.PageSize;
            var pageItems = await query
                .Skip(skip)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            // Mapiraj u TransactionsWithSplits
            var mappedItems = _mapper.Map<List<TransactionWithSplits>>(pageItems);
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

     
    }
}
