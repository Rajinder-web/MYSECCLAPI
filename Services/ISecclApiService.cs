using MYSECCLAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MYSECCLAPI.Services
{
    public interface ISecclApiService
    {
        Task<string> GetAccessTokenAsync();
        Task<Root?> GetPortfolioValuationAsync(string portfolioId, string accessToken);

    }
}
