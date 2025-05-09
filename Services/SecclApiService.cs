using MySECCLAppAPI.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace MySECCLAppAPI.Services
{
    public class SecclApiService : ISecclApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public SecclApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["SecclApi:BaseUrl"] ??
                                              throw new InvalidOperationException("SecclApi:BaseUrl not configured."));
        }
               
        public async Task<string> GetAccessTokenAsync()
        {

            var authRequest = new SecclAuthRequest
            {
                firmId = _configuration["SecclApi:FirmId"],// From Config Secrets
                id = _configuration["SecclApi:Id"],       // From Config Secrets
                password = _configuration["SecclApi:Password"] // From Config Secrets

            };

            // According to SECCL docs: "Getting started > Access and authentication > Generate an access token [Deprecated]"
            // The endpoint is likely /authenticate or similar - VERIFY THIS FROM POSTMAN DOCS
            var requestContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(authRequest), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("authenticate", requestContent); // VERIFY THIS ENDPOINT
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                RootSecclAuthResponse myDeserializedClass = JsonConvert.DeserializeObject<RootSecclAuthResponse>(json);

                var authResponse = myDeserializedClass?.data?.token;
                if (authResponse == null || string.IsNullOrEmpty(authResponse))
                {
                    throw new ApplicationException("Failed to retrieve access token from SECCL API.");
                }

                return authResponse;
            }
            return string.Empty;
        }
        public async Task<Root?> GetPortfolioValuationAsync(string portfolioId,string accessToken)
        {
            var firmId = _configuration["SecclApi:FirmId"];// From Config Secrets
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}portfolio/summary/{firmId}/{portfolioId}");
            request.Headers.Add("api-token", accessToken);
            var content = new StringContent(string.Empty);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            var data = await response.Content.ReadAsStringAsync();
            Root? myDeserializedClass = JsonConvert.DeserializeObject<Root?>(data);
            return myDeserializedClass;
                
        }
    }
}

