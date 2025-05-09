namespace MYSECCLAPI.Dto
{
    public class AggregatedPortfolioDto
    {
        public decimal PortfolioAllAccountTotal { get; set; }
        public int TotalAccounts { get; set; }
        public decimal PortfolioTotalValue { get; set; }
        public List<string> FetchedPortfolioIds { get; set; } = new List<string>();
        public DateTime LastUpdated { get; set; }
    }
}
