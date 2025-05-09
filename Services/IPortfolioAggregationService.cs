using MYSECCLAPI.Dto;

namespace MYSECCLAPI.Services
{
    public interface IPortfolioAggregationService
    {
        Task<AggregatedPortfolioDto> GetAggregatedDataAsync(IEnumerable<string> portfolioIds);
    }
}
