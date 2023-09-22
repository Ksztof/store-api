using Duende.IdentityServer.Models;
using System.Collections.Generic;

namespace PerfumeStore.DatabaseMigration
{
  public class Config
  {
    public static IEnumerable<IdentityResource> IdentityResources =>
        new[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResource
            {
                Name = "role",
                UserClaims = new List<string> { "role" }
            }
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new[]
        {
            new ApiScope("PerfumeStoreAPI.read"),
            new ApiScope("PerfumeStoreAPI.write"),
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new[]
        {
            new ApiResource("PerfumeStoreAPI")
            {
                Scopes = { "PerfumeStoreAPI.read", "PerfumeStoreAPI.write" }, // This remains for compatibility, but it's recommended to use ApiScopes directly.
                ApiSecrets = { new Secret("ScopeSecret".Sha256()) },
                UserClaims = { "role" }
            }
        };

    public static IEnumerable<Client> Clients =>
        new[]
        {
            // m2m client credentials flow client
            new Client
            {
                ClientId = "m2m.client",
                ClientName = "Client Credentials Client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("ClientSecret1".Sha256()) },
                AllowedScopes = { "PerfumeStoreAPI.read", "PerfumeStoreAPI.write" }
            },
            // interactive client using code flow + pkce
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
            },
        };
  }

}
