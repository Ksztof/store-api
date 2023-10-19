using Duende.IdentityServer.Models;


namespace DuendeIs
{
  public static class Config
  {
    public static IEnumerable<IdentityResource> IdentityResources =>
      new IdentityResource[]
      {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
      };

    public static IEnumerable<ApiScope> ApiScopes =>
      new[]
      {
        new ApiScope("PerfumeStore.read"),
        new ApiScope("PerfumeStore.write"),
        new ApiScope("PerfumeStore")
      };

    public static IEnumerable<ApiResource> ApiResources => new[]
    {
      new ApiResource("PerfumeStore")
      {
        Scopes = new List<string> { "PerfumeStore.read", "PerfumeStore.write"},
        ApiSecrets = new List<Secret> {new Secret("ScopeSecret".Sha256())},
        UserClaims = new List<string> {"role"}
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
          ClientSecrets = {new Secret("SuperSecretPassword".Sha256())},

          AllowedScopes = { "PerfumeStore.read", "PerfumeStore.write" }
        },

        // interactive client using code flow + pkce
        new Client
        {
          ClientId = "interactive",
          ClientSecrets = {new Secret("SuperSecretPassword".Sha256())},

          AllowedGrantTypes = GrantTypes.Code,

          AllowOfflineAccess = true,
          AllowedScopes = {"openid", "profile", "PerfumeStore.read", "PerfumeStore"},
          RequireConsent = true,
        },
      };
  }
}
