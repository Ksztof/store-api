﻿using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PerfumeStore.Core.Configuration;
using PerfumeStore.Core.DTOs.Request;

namespace PerfumeStore.Core.Services
{
  public class TokenService : ITokenService
  {
    private readonly ILogger<TokenService> _logger;
    public readonly IOptions<IdentityServerSettings> _identityServerSettings;
    public readonly DiscoveryDocumentResponse _discoveryDocument;
    private readonly HttpClient _httpClient;

    public TokenService(IOptions<IdentityServerSettings> identityServerSettings, ILogger<TokenService> logger, HttpClient httpClient)
    {
      _logger = logger;
      _identityServerSettings = identityServerSettings;
      _httpClient = httpClient;
      _discoveryDocument = httpClient.GetDiscoveryDocumentAsync(identityServerSettings.Value.DiscoveryUrl).Result;

      if (_discoveryDocument.IsError)
      {        logger.LogError($"Unable to get discovery document. Error is: {_discoveryDocument.Error}");

        throw new Exception("Unable to get discovery document", _discoveryDocument.Exception);
      }
    }

    public async Task<TokenResponse> GetToken(string scope, UserForAuthenticationDto userForAuthentication)
    {
      var tokenResponse = await _httpClient.RequestPasswordTokenAsync(new d
      {
        Address = _discoveryDocument.TokenEndpoint,
        ClientId = _identityServerSettings.Value.ClientName,
        ClientSecret = _identityServerSettings.Value.ClientPassword,
        UserName = userForAuthentication.Email,
        Password = userForAuthentication.Password,
        Scope = scope // Dodaj odpowiednie zakresy
      }); ;
      
      if (tokenResponse.IsError)
      {
        _logger.LogError($"Unable to get token. Error is: {tokenResponse.Error}");
        throw new Exception("Unable to get token", tokenResponse.Exception);
      }

      return tokenResponse;
    }
  }
}