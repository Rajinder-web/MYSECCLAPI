using MySECCLAppAPI.Dto;

namespace MySECCLAppAPI.Services
{
    public interface IPortfolioAggregationService
    {
        Task<AggregatedPortfolioDto> GetAggregatedDataAsync(IEnumerable<string> portfolioIds);
    }
}
