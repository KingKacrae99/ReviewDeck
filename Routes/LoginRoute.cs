using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
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
        })
        .AllowAnonymous();

        app.MapPost("/account/login", HandleLogin)
            .AllowAnonymous();

        app.MapPost("/account/logout", HandleLogout)
            .RequireAuthorization();
    }

    private static async Task<IResult> HandleLogin(
        HttpContext httpContext,
        IUserService userService,
        IAntiforgery antiforgery,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("ReviewDeck.Login");

        try
        {
            await antiforgery.ValidateRequestAsync(httpContext);
        }
        catch (AntiforgeryValidationException)
        {
            logger.LogWarning("Login rejected because antiforgery validation failed.");
            return Results.BadRequest(
                "Your session has expired. Refresh the page and try again.");
        }

        var form = await httpContext.Request.ReadFormAsync();

        var request = new LoginRequest
        {
            Email = form["Email"].ToString().Trim(),
            Password = form["Password"].ToString()
        };

        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return Results.Redirect("/login?error=invalid");
        }

        // Unexpected service exceptions are handled by ASP.NET Core's
        // centralized exception handler.
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

        httpContext.Response.Headers.CacheControl = "no-store";

        return Results.Redirect("/");
    }

    private static async Task<IResult> HandleLogout(
        HttpContext httpContext,
        IAntiforgery antiforgery,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("ReviewDeck.Logout");

        try
        {
            await antiforgery.ValidateRequestAsync(httpContext);
        }
        catch (AntiforgeryValidationException)
        {
            logger.LogWarning(
                "Logout rejected because antiforgery validation failed.");

            return Results.BadRequest(
                "Your session has expired. Refresh the page and try again.");
        }

        await httpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        httpContext.Response.Headers.CacheControl = "no-store";

        return Results.Redirect("/login");
    }
}
