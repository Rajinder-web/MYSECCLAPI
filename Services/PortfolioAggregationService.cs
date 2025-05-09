using MYSECCLAPI.Dto;
using MYSECCLAPI.Models;

namespace MYSECCLAPI.Services
{
    public class PortfolioAggregationService: IPortfolioAggregationService
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
            if(string.IsNullOrEmpty(accessToken))
            {
                return new AggregatedPortfolioDto
                {
                    LastUpdated = DateTime.UtcNow,
                    PortfolioTotalValue = 0,
                    TotalAccounts=0,
                    PortfolioAllAccountTotal = 0,
                    //ValueByAccountType = new Dictionary<string, decimal>(),
                    FetchedPortfolioIds = new List<string>()
                };
            }
            var aggregatedDto = new AggregatedPortfolioDto
            {
                LastUpdated = DateTime.UtcNow
            };

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
            double totalcurrentvalue = 0;
            foreach (var result in results)
            {
                if (result.Valuation != null)
                {
                    aggregatedDto.FetchedPortfolioIds.Add(result.Id);
                    aggregatedDto.TotalAccounts = result.Valuation.data.accounts.Count;
                    aggregatedDto.PortfolioTotalValue = (decimal)result.Valuation.data.currentValue;
                    foreach (var account in result.Valuation.data.accounts)
                    {
                        totalcurrentvalue += account.currentValue;
                    }
                    // If API provides total value directly for the portfolio
                    // aggregatedDto.TotalCombinedValue += result.Valuation.TotalValue;

                    // If we need to sum holdings
                    //foreach (var holding in result.Valuation.Holdings)
                    //{
                    //    aggregatedDto.TotalCombinedValue += holding.Value; // Sum total value

                    //    if (!string.IsNullOrEmpty(holding.AccountType))
                    //    {
                    //        if (aggregatedDto.ValueByAccountType.ContainsKey(holding.AccountType))
                    //        {
                    //            aggregatedDto.ValueByAccountType[holding.AccountType] += holding.Value;
                    //        }
                    //        else
                    //        {
                    //            aggregatedDto.ValueByAccountType[holding.AccountType] = holding.Value;
                    //        }
                    //    }
                    //}
                }
            }
            aggregatedDto.PortfolioAllAccountTotal = (decimal)totalcurrentvalue;
            
            
            return aggregatedDto;
        }
    }
}
