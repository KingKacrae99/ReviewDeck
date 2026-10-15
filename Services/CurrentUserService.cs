
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace ReviewDeck.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public CurrentUserService(
        AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<int?> GetUserIdAsync()
    {
        var authState = await _authenticationStateProvider
            .GetAuthenticationStateAsync();

        var user = authState.User;

        if (user.Identity?.IsAuthenticated != true)
            return null;

        var userIdValue = user
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (int.TryParse(userIdValue, out var userId) && userId > 0)
            return userId;

        return null;
    }
}
