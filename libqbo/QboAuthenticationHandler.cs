using Intuit.Ipp.Diagnostics;
using Intuit.Ipp.OAuth2PlatformClient;
using libqbo.Configuration;
using libqbo.Models.Auth;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Web;

namespace libqbo;

/// <summary>
/// Handles Quickbooks Online authentication.
/// </summary>
public class QboAuthenticationHandler
{
    private QboAuthToken? Token { get; set; } = null;
    private OAuth2Client Client { get; set; }
    private QboConfiguration _configuration;
    private readonly IQboTokenStorageProvider _tokenStorageProvider;
    private readonly ILogger<QboAuthenticationHandler> _logger;

    public QboAuthenticationHandler(IOptions<QboConfiguration> configuration, IQboTokenStorageProvider tokenStorageProvider, ILogger<QboAuthenticationHandler> logger)
    {
        _configuration = configuration.Value;
        Client = new OAuth2Client(_configuration.ClientId, _configuration.ClientSecret, _configuration.RedirectUrl, _configuration.UseSandbox ? "sandbox" : "production");
        _tokenStorageProvider = tokenStorageProvider;
        _logger = logger;
    }

    /// <summary>
    /// Tries to get a valid token. Will throw <see cref="QboAuthenticationRequiredException"/> if the stored token is invalid or missing.
    /// </summary>
    internal async Task<QboAuthToken> GetValidToken()
    {
        if (Token == null)
        {
            _logger?.LogInformation("Fetching stored token.");
            await FetchStoredToken().ConfigureAwait(false);
        }

        if (Token == null || string.IsNullOrEmpty(Token.AccessToken) || string.IsNullOrEmpty(Token.RefreshToken) || string.IsNullOrEmpty(Token.RealmId))
        {
            _logger?.LogInformation("Stored token did not exist.");
            throw new QboAuthenticationRequiredException();
        }

        if (Token.AccessExpiry <= DateTimeOffset.UtcNow.AddMinutes(1) || Token.RefreshExpiry <= DateTimeOffset.UtcNow.AddMinutes(1))
        {
            _logger.LogInformation("Access token is about to expire. Refreshing token...");
            TokenResponse response = await Client.RefreshTokenAsync(Token.RefreshToken!);

            if (response.IsError)
            {
                _logger?.LogError("Could not refresh access token.");
                throw new QboAuthenticationRequiredException();
            }

            _logger?.LogInformation("Refresh access token succeeded.");

            Token.AccessToken = response.AccessToken;
            Token.RefreshToken = response.RefreshToken;
            Token.AccessExpiry = DateTimeOffset.UtcNow.AddSeconds(response.AccessTokenExpiresIn);
            Token.RefreshExpiry = DateTimeOffset.UtcNow.AddSeconds(response.RefreshTokenExpiresIn);
            await _tokenStorageProvider.SaveToken(Token);
        }
        else
        {
            _logger?.LogInformation("Token is valid until {date}", Token.AccessExpiry);
        }

        return Token;
    }

    [MemberNotNull(nameof(Token))]
    private async Task FetchStoredToken()
    {
        Token ??= (await _tokenStorageProvider.GetToken() ?? new());
    }

    /// <summary>
    /// Creates a new authorization URL using the OAuth2 class.
    /// </summary>
    public string GetAuthorizationURL(params OidcScopes[] scopes)
    {
        // Get the authorization url based
        // on the passed scopes.
        return Client.GetAuthorizationURL([.. scopes]);
    }

    internal async Task<bool> CheckQueryParamsAndSet(string queryString, bool suppressErrors = true)
    {
        // Parse the query string into a
        // NameValueCollection for easy access
        // to each parameter.
        NameValueCollection query = HttpUtility.ParseQueryString(queryString);

        // Make sure the required query
        // parameters exist.
        if (query["code"] != null && query["realmId"] != null)
        {
            TokenResponse response = await Client.GetBearerTokenAsync(query["code"]);

            if (Token == null)
            {
                await FetchStoredToken();
            }

            if (response.IsError)
            {
                if (suppressErrors)
                {
                    return false;
                }
                else
                {
                    throw new InvalidDataException($"Error retrieving bearer token: {response.ErrorDescription}");
                }
            }

            // Set the token values with the client
            // responce and query parameters.
            Token.AccessToken = response.AccessToken;
            Token.RefreshToken = response.RefreshToken;
            Token.RealmId = query["realmId"];
            Token.AccessExpiry = DateTimeOffset.UtcNow.AddSeconds(response.AccessTokenExpiresIn);
            Token.RefreshExpiry = DateTimeOffset.UtcNow.AddSeconds(response.RefreshTokenExpiresIn);

            await _tokenStorageProvider.SaveToken(Token);

            return true;
        }
        else
        {
            if (suppressErrors)
            {
                return false;
            }
            else
            {
                throw new InvalidDataException($"The 'code' or 'realmId' was not present in the query parameters '{query}'.");
            }
        }
    }
}
