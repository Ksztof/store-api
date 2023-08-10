using IdentityModel.Client;
using Microsoft.Extensions.Options;

namespace PerfumeShop.Client.Services
{
    public class TokenService : ITokenService
    {
        public readonly IOptions<IdentityServerSettings> identityServerSettigns;
        public readonly DiscoveryDocumentResponse discoveryDocument;
        public readonly HttpClient httpClient;

        public TokenService(IOptions<IdentityServerSettings> identityServerSettigns, DiscoveryDocumentResponse discoveryDocumentResponse, HttpClient httpClient)
        {
            this.identityServerSettigns = identityServerSettigns;
            this.httpClient = new HttpClient();
            discoveryDocument = httpClient.GetDiscoveryDocumentAsync(this.identityServerSettigns.Value.DiscoveryUrl).Result;

            if (discoveryDocumentResponse.IsError)
            {
                throw new Exception("Unable to get discovery document", discoveryDocument.Exception);
            }
        }

        public async Task<TokenResponse> GetToken(string scope)
        {
            var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = identityServerSettigns.Value.ClientName,
                ClientSecret = identityServerSettigns.Value.ClientPassword,
                Scope = scope
            });

            if (tokenResponse.IsError)
            {
                throw new Exception("Unable to get token", tokenResponse.Exception);
            }

            return tokenResponse;
        }
    }
}
