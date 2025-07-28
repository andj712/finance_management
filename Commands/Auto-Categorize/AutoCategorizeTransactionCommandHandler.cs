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
        private readonly IRulesProvider _rulesProvider;


        public AutoCategorizeTransactionsHandler(
            ITransactionRepository repo,
             IRulesProvider rulesProvider)
        {
            _repo = repo;
            _rulesProvider = rulesProvider;
        }

        public async Task<int> Handle(
            AutoCategorizeTransactionCommand request,
            CancellationToken ct)
        {
            var rules = _rulesProvider.GetRules();
            return await _repo.AutoCategorizeAsync(rules);
        }
    }
}

