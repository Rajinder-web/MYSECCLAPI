using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MYSECCLAPI.Dto;
using MYSECCLAPI.Services;

namespace MYSECCLAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController] 
    public class PortfolioController : ControllerBase
    {
   
        private readonly IPortfolioAggregationService _aggregationService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PortfolioController> _logger;

        public PortfolioController(IPortfolioAggregationService aggregationService, IConfiguration configuration, ILogger<PortfolioController> logger)
        {
            _aggregationService = aggregationService;
            _configuration = configuration;
            _logger = logger;
        }
        [HttpGet("aggregated-summary")]
        public async Task<ActionResult<AggregatedPortfolioDto>> GetAggregatedPortfolioSummary()
        {
            try
            {
                var portfolioIds = _configuration.GetSection("PortfolioSettings:ClientPortfolioIds").Get<List<string>>();
                if (portfolioIds == null || !portfolioIds.Any())
                {
                    _logger.LogWarning("No portfolio IDs configured in PortfolioSettings:ClientPortfolioIds.");
                    return BadRequest("Portfolio IDs not configured.");
                }

                var data = await _aggregationService.GetAggregatedDataAsync(portfolioIds);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting aggregated portfolio summary.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
    }
}
