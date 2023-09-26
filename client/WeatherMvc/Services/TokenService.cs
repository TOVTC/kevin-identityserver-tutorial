using IdentityModel.Client;
using Microsoft.Extensions.Options;

namespace WeatherMvc.Services
{
    public class TokenService : ITokenService
    {
        private readonly ILogger<TokenService> _logger;
        private readonly IOptions<IdentityServerSettings> _identityServerSettings;
        private readonly DiscoveryDocumentResponse _discoveryDocument;

        public TokenService(ILogger<TokenService> logger, IOptions<IdentityServerSettings> identityServerSettings)
        {
            _logger = logger;
            _identityServerSettings = identityServerSettings;

            using var httpClient = new HttpClient();
            // this retrieves the DiscoveryUrl from the settings
            _discoveryDocument = httpClient.GetDiscoveryDocumentAsync(identityServerSettings.Value.DiscoveryUrl).Result;
            if (_discoveryDocument.IsError)
            {
                logger.LogError($"Unable to get discovery document. Error is: {_discoveryDocument.Error}");
                throw new Exception("Unable to get discovery document", _discoveryDocument.Exception);
            }
        }

        // this method works the same way the curl command examples worked
        public async Task<TokenResponse> GetToken(string scope)
        {
            // send a request to the token endpoint, providing the ClientId, ClientSecret, and Scope
            using var client = new HttpClient();
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                // retrieved from DiscoveryDocument - and to get the DiscoveryDocument, use the .GetDiscoveryDocumentAsync() method in the constructor above
                Address = _discoveryDocument.TokenEndpoint,

                // taken from appSettings
                ClientId = _identityServerSettings.Value.ClientName,
                ClientSecret = _identityServerSettings.Value.ClientPassword,
                Scope = scope
            });

            if (tokenResponse.IsError)
            {
                _logger.LogError($"Unable to get token. Error is: {tokenResponse.Error}");
                throw new Exception("Unable to get token", tokenResponse.Exception);
            }
            // the token is then returned and can be used as a Bearer token when the API is called
            return tokenResponse;
        }
    }
}
