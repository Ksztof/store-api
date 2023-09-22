using Duende.IdentityServer.Models;

namespace PerfumeShop.Serv
{
  public class Config
  {
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResource("role", new List<string> { "role" })
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("PerfumeStoreAPI.read"),
            new ApiScope("PerfumeStoreAPI.write")
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new ApiResource[]
        {
            new ApiResource("PerfumeStoreAPI", new List<string> { "role" })
            {
                Scopes = new List<string> { "PerfumeStoreAPI.read", "PerfumeStoreAPI.write" },
                ApiSecrets = new List<Secret> { new Secret("ScopeSecret".Sha256()) }
            }
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            new Client
            {
                ClientId = "m2m.client",
                ClientName = "Client Credentials Client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("ClientSecret1".Sha256()) },
                AllowedScopes = { "PerfumeStoreAPI.read", "PerfumeStoreAPI.write" }
            },
            new Client
            {
                ClientId = "interactive",
                ClientSecrets = { new Secret("ClientSecret1".Sha256()) },
                AllowedGrantTypes = GrantTypes.Code,
                RedirectUris = { "https://localhost:5444/signin-oidc" },
                FrontChannelLogoutUri = "https://localhost:5444/signout-oidc",
                PostLogoutRedirectUris = { "https://localhost:5444/signout-callback-oidc" },
                AllowOfflineAccess = true,
                AllowedScopes = { "openid", "profile", "PerfumeStoreAPI.read" },
                RequirePkce = true,
                RequireConsent = true,
                AllowPlainTextPkce = false
            }
        };
  }
}