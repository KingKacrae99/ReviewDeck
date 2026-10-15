using Microsoft.AspNetCore.Antiforgery;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using ReviewDeck.Models;
using ReviewDeck.Services;

namespace ReviewDeck.Routes;

public static class LoginRoutes
{
    public static void MapLoginRoutes(this IEndpointRouteBuilder app)
    {
        app.MapGet("/account/antiforgery", (
            HttpContext context,
            IAntiforgery antiforgery) =>
        {
            var tokens = antiforgery.GetAndStoreTokens(context);

            return Results.Ok(new
            {
                token = tokens.RequestToken
            });
        });

        app.MapPost("/account/login", HandleLogin);
    }


    private static async Task<IResult> HandleLogin(
        HttpContext httpContext,
        IUserService userService,
        IAntiforgery antiforgery)
    {
        try
        {
            await antiforgery.ValidateRequestAsync(httpContext);
        }
        catch (AntiforgeryValidationException)
        {
            return Results.BadRequest("Invalid antiforgery token.");
        }

        var form = await httpContext.Request.ReadFormAsync();

        var request = new LoginRequest
        {
            Email = form["Email"].ToString(),
            Password = form["Password"].ToString()
        };

        var user = await userService.LoginUserAsync(request);

        if (user is null)
        {
            return Results.Redirect("/login?error=invalid");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Email),
            new(ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal);

        return Results.Redirect("/");
    }
}
