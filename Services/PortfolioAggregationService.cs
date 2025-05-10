using MYSECCLAPI.Dto;
using MYSECCLAPI.Models;

namespace MYSECCLAPI.Services
{
    public class PortfolioAggregationService : IPortfolioAggregationService
    {
        private readonly ISecclApiService _secclApiService;
        private readonly ILogger<PortfolioAggregationService> _logger;

        public PortfolioAggregationService(ISecclApiService secclApiService, ILogger<PortfolioAggregationService> logger)
        {
            _secclApiService = secclApiService;
            _logger = logger;
        }

        public async Task<AggregatedPortfolioDto> GetAggregatedDataAsync(IEnumerable<string> portfolioIds)
        {
            var accessToken = await _secclApiService.GetAccessTokenAsync();
            if (string.IsNullOrEmpty(accessToken))
            {
                return new AggregatedPortfolioDto
                {
                    PortfolioTotalValue = 0,
                    TotalAccounts = 0,
                    PortfolioAllAccountsTotal = 0,
                    AccountType = string.Empty,
                    FetchedPortfolioIds = new List<string>()
                };
            }
            AggregatedPortfolioDto aggregatedDto = new AggregatedPortfolioDto();
            
            var portfolioTasks = portfolioIds.Select(async id =>
            {
                try
                {
                    var valuation = await _secclApiService.GetPortfolioValuationAsync(id, accessToken);
                    return new { Id = id, Valuation = valuation };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to fetch valuation for portfolio {PortfolioId}", id);
                    return new { Id = id, Valuation = (Root?)null };
                }
            }).ToList();

            var results = await Task.WhenAll(portfolioTasks);
            double totalCurrentValue = 0;
            foreach (var result in results)
            {
                if (result.Valuation != null)
                {
                    aggregatedDto.FetchedPortfolioIds.Add(result.Id);
                    aggregatedDto.TotalAccounts += result.Valuation.data.accounts.Count;
                    aggregatedDto.PortfolioTotalValue += (decimal)result.Valuation.data.currentValue;
                   
                    // If API provides total value directly for the portfolio
                    totalCurrentValue += result.Valuation.data.accounts.Sum(account => account.currentValue);

                    // If we need to Sum total value

                    aggregatedDto.PortfolioAllAccountsTotal += totalCurrentValue;


                    var totalsByAccountType = result.Valuation.data.accounts
            .GroupBy(account => account.accountType)
            .Select(group => new
            {
                AccountType = group.Key,
                TotalCurrentValue = group.Sum(account => account.currentValue)
            });

                    foreach (var item in totalsByAccountType)
                    {
                        if (!string.IsNullOrEmpty(item.AccountType))
                        {
                            aggregatedDto.AccountType = item.AccountType;

                            aggregatedDto.PortfolioAllAccountsTotal = item.TotalCurrentValue;
                        }

                    }

                }
            }

            return aggregatedDto;
        }

    }
}
