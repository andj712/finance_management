using finance_management.AutoCategorize;
using finance_management.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq.Dynamic.Core;


namespace finance_management.Commands.Auto_Categorize
{
    public class AutoCategorizeTransactionsHandler
        : IRequestHandler<AutoCategorizeTransactionCommand, int>
    {
        private readonly ITransactionRepository _repo;
        private readonly AutoCategorizationOptions _opts;

        public AutoCategorizeTransactionsHandler(
            ITransactionRepository repo,
            IOptions<AutoCategorizationOptions> opts)
        {
            _repo = repo;
            _opts = opts.Value;
        }

        public async Task<int> Handle(
            AutoCategorizeTransactionCommand request,
            CancellationToken ct)
        {
            //pocinjem od nekategorizovanih transakcija
            var baseQuery = _repo.Query()
                                 .Where(t => t.CatCode == null);

            var total = 0;
            //iteracija kroz svako pravilo
            foreach (var rule in _opts.Rules)
            {
                // filtriranje transakcija koje pogadja taj predikat dinamic LINQ 
                var hits = await baseQuery
                    .Where(rule.Predicate)
                    .ToListAsync(ct);    // async execution

                //za svaku kategoriju koja odgovara, update kategoriju
                foreach (var tx in hits)
                {
                    await _repo.UpdateCategoryAsync(tx.Id, rule.CatCode);
                    total++;
                }
            }

            return total;
        }
    }
}

