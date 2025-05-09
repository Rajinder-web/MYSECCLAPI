namespace MySECCLAppAPI.Models
{
    public class SecclAuthResponseData
    {
        public string token { get; set; }
        public string userName { get; set; }
        public string userType { get; set; }
        public List<ScopeSecclAuthResponse> scopes { get; set; }
        public List<string> services { get; set; }
    }

    public class RootSecclAuthResponse
    {
        public SecclAuthResponseData data { get; set; }
    }

    public class ScopeSecclAuthResponse
    {
        public string scope { get; set; }
        public List<string> ranges { get; set; }
    }



}
