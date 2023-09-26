namespace WeatherMvc.Services
{
    // this model maps information that needs to be used when calling Identity Server
    // the values for this model are set in appsettings.json
    public class IdentityServerSettings
    {
        public string DiscoveryUrl { get; set; }
        public string ClientName { get; set; }
        public string ClientPassword { get; set; }
        public bool UseHttps { get; set; }
    }
}
