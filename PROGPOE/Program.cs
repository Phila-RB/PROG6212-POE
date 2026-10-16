using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using PROGPOE.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) //cookie based auth
    .AddCookie(options =>
    {
        options.Cookie.Name = "YourAppSessionCookie";
        options.Cookie.HttpOnly = true; // Prevents client-side script access
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Requires HTTPS
        options.Cookie.SameSite = SameSiteMode.Strict; // Mitigates CSRF
        options.ExpireTimeSpan = TimeSpan.FromMinutes(1); // Session duration
        options.SlidingExpiration = true; // Renews session if user is active

        // Suppress automatic redirects to Login pages since this is an API
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
    });

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Map the native OpenAPI endpoint (/openapi/v1.json)
    app.MapOpenApi();

    // Enable the Swashbuckle Swagger UI to read the native document
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run(); //runs app
