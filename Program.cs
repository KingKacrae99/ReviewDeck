using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using ReviewDeck.Components;
using ReviewDeck.Models;
using ReviewDeck.Data;
using ReviewDeck.Services;
using ReviewDeck.Routes;
using Microsoft.AspNetCore.DataProtection;
using System.IO;



DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

// Retrieve Connection String from .env
var connectionString = builder.Configuration["CONNECTION_STRING"];

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("The CONNECTION_STRING environment variable is not configured.");
}

// Register Database Context
builder.Services.AddDbContext<ReviewDeckContext>(options =>
    options.UseNpgsql(connectionString));


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddScoped<CurrentUserService>();
builder.Services.AddScoped<ICurrentUserService>(provider =>
    provider.GetRequiredService<CurrentUserService>());
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<IPasswordHasher<User>,PasswordHasher<User>>();
builder.Services.AddScoped<IJobApplicationService, JobApplicationService>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
       options.LoginPath ="/login";
       options.AccessDeniedPath = "/access-denied";
       options.Cookie.Name = "ReviewDeck.Auth";
       options.Cookie.HttpOnly = true;
       options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
       options.Cookie.SameSite = SameSiteMode.Lax;
       options.ExpireTimeSpan = TimeSpan.FromHours(8);
       options.SlidingExpiration= true; 
    });
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorization();

var keyDirectory = Path.Combine(
    builder.Environment.ContentRootPath,
    "DataProtection-Keys"
);

builder.Services
    .AddDataProtection()
    .PersistKeysToDbContext<ReviewDeckContext>()
    .SetApplicationName("ReviewDeck");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapLoginRoutes();

app.Run();
