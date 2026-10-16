using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using api_proj.Auth;
using api_proj.Data;

var builder = WebApplication.CreateBuilder(args);

// ---------- Database (EF Core, Code-First) ----------
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ---------- Cookie Authentication ----------
// This is the "session" the Build Guide's Part W describes: a cookie the
// browser (or Swagger, or a test HttpClient with cookies enabled) holds
// onto, which the server reads on every request to know who's logged in
// and what role they have. Claims are set at sign-in in AuthController.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "RaceDayAuth";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;

        // AddCookie() defaults to redirecting (302) to a login page on
        // failure, which is the right behaviour for an MVC app but wrong
        // for a Web API - Swagger, a test HttpClient, and the Part 3 MVC
        // client all need real 401/403 status codes back, not a redirect.
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();

// ---------- Auth helpers ----------
builder.Services.AddSingleton<PasswordHasher>();

// ---------- Controllers ----------
builder.Services.AddControllers();

// ---------- Swagger ----------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RaceDay API",
        Version = "v1",
        Description = "RaceDay event management platform - RESTful API (PROG6212/w Part 2)."
    });

    // Include XML doc comments (endpoint summaries) in Swagger UI, if generated.
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// ---------- CORS (MVC front end in Part 3 will call this API) ----------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMvcClient", policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>())
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// ---------- Middleware pipeline ----------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "RaceDay API v1");
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowMvcClient");

// Authentication must run before authorization, so the app knows who's
// making the request before it checks what they're allowed to do.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Exposed for WebApplicationFactory-based integration tests.
public partial class Program { }
