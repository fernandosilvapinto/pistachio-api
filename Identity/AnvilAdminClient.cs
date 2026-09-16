using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Pistachio.Api.Identity;

/// <summary>
/// Creates customer accounts in the identity provider on the application's
/// behalf, using a service account. The application never sees a password:
/// the provider emails the invitation and collects the password itself.
/// </summary>
public sealed class AnvilAdminClient
{
    private readonly HttpClient _http;
    private readonly AnvilAdminOptions _options;
    private readonly ILogger<AnvilAdminClient> _logger;
    private readonly SemaphoreSlim _tokenLock = new(1, 1);

    private string? _accessToken;
    private DateTimeOffset _accessTokenExpiresAt = DateTimeOffset.MinValue;

    public AnvilAdminClient(
        HttpClient http,
        IOptions<AnvilAdminOptions> options,
        ILogger<AnvilAdminClient> logger)
    {
        _http = http;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<CustomerInvitationResult> InviteCustomerAsync(
        string email,
        string fullName,
        CancellationToken ct)
    {
        if (!_options.Enabled || string.IsNullOrWhiteSpace(_options.BaseUrl))
        {
            return CustomerInvitationResult.NotInvited;
        }

        try
        {
            var token = await GetAccessTokenAsync(ct);

            var existingId = await FindUserIdByEmailAsync(email, token, ct);

            if (existingId is not null)
            {
                return CustomerInvitationResult.AlreadyRegistered;
            }

            var userId = await CreateUserAsync(email, fullName, token, ct);

            await AssignDefaultRoleAsync(userId, token, ct);
            await SendInvitationAsync(userId, token, ct);

            return CustomerInvitationResult.Invited;
        }
        catch (Exception ex)
        {
            // A booking must never fail because provisioning did.
            _logger.LogError(ex, "Could not invite {Email} to the identity provider.", email);
            return CustomerInvitationResult.NotInvited;
        }
    }

    private string AdminBase => $"{_options.BaseUrl.TrimEnd('/')}/admin/realms/{_options.Realm}";

    private async Task<string> GetAccessTokenAsync(CancellationToken ct)
    {
        if (_accessToken is not null && DateTimeOffset.UtcNow < _accessTokenExpiresAt)
        {
            return _accessToken;
        }

        await _tokenLock.WaitAsync(ct);

        try
        {
            if (_accessToken is not null && DateTimeOffset.UtcNow < _accessTokenExpiresAt)
            {
                return _accessToken;
            }

            var url = $"{_options.BaseUrl.TrimEnd('/')}/realms/{_options.Realm}/protocol/openid-connect/token";

            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["grant_type"] = "client_credentials",
                    ["client_id"] = _options.ClientId,
                    ["client_secret"] = _options.ClientSecret
                })
            };

            using var response = await _http.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();

            using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync(ct));

            _accessToken = document.RootElement.GetProperty("access_token").GetString()
                ?? throw new InvalidOperationException("Token response has no access_token.");

            var expiresIn = document.RootElement.TryGetProperty("expires_in", out var value)
                ? value.GetInt32()
                : 60;

            _accessTokenExpiresAt = DateTimeOffset.UtcNow.AddSeconds(Math.Max(expiresIn - 30, 10));

            return _accessToken;
        }
        finally
        {
            _tokenLock.Release();
        }
    }

    private async Task<string?> FindUserIdByEmailAsync(string email, string token, CancellationToken ct)
    {
        using var request = Authorized(HttpMethod.Get, $"{AdminBase}/users?email={Uri.EscapeDataString(email)}&exact=true", token);
        using var response = await _http.SendAsync(request, ct);

        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync(ct));

        foreach (var element in document.RootElement.EnumerateArray())
        {
            return element.GetProperty("id").GetString();
        }

        return null;
    }

    private async Task<string> CreateUserAsync(string email, string fullName, string token, CancellationToken ct)
    {
        var separator = fullName.IndexOf(' ');
        var firstName = separator > 0 ? fullName[..separator] : fullName;
        var lastName = separator > 0 ? fullName[(separator + 1)..] : string.Empty;

        using var request = Authorized(HttpMethod.Post, $"{AdminBase}/users", token);

        request.Content = JsonContent.Create(new
        {
            username = email,
            email,
            firstName,
            lastName,
            enabled = true,
            emailVerified = false
        });

        using var response = await _http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        var location = response.Headers.Location?.ToString();

        if (!string.IsNullOrEmpty(location))
        {
            return location[(location.LastIndexOf('/') + 1)..];
        }

        return await FindUserIdByEmailAsync(email, token, ct)
            ?? throw new InvalidOperationException("Created user could not be found.");
    }

    private async Task AssignDefaultRoleAsync(string userId, string token, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_options.DefaultRole))
        {
            return;
        }

        using var roleRequest = Authorized(HttpMethod.Get, $"{AdminBase}/roles/{Uri.EscapeDataString(_options.DefaultRole)}", token);
        using var roleResponse = await _http.SendAsync(roleRequest, ct);

        roleResponse.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await roleResponse.Content.ReadAsStringAsync(ct));

        var representation = new
        {
            id = document.RootElement.GetProperty("id").GetString(),
            name = document.RootElement.GetProperty("name").GetString()
        };

        using var assignRequest = Authorized(HttpMethod.Post, $"{AdminBase}/users/{userId}/role-mappings/realm", token);
        assignRequest.Content = JsonContent.Create(new[] { representation });

        using var assignResponse = await _http.SendAsync(assignRequest, ct);
        assignResponse.EnsureSuccessStatusCode();
    }

    private async Task SendInvitationAsync(string userId, string token, CancellationToken ct)
    {
        var url =
            $"{AdminBase}/users/{userId}/execute-actions-email" +
            $"?client_id={Uri.EscapeDataString(_options.InviteClientId)}" +
            $"&redirect_uri={Uri.EscapeDataString(_options.InviteRedirectUri)}" +
            $"&lifespan={_options.InviteLifespanSeconds}";

        using var request = Authorized(HttpMethod.Put, url, token);
        request.Content = JsonContent.Create(new[] { "UPDATE_PASSWORD", "VERIFY_EMAIL" });

        using var response = await _http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
    }

    private static HttpRequestMessage Authorized(HttpMethod method, string url, string token)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
    }
}
