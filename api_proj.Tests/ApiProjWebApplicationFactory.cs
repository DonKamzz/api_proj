using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using api_proj.Data;

namespace api_proj.Tests;

/// <summary>
/// Boots the real api_proj pipeline (routing, cookie authentication, the
/// [Authorize] checks, controllers) against an isolated in-memory database,
/// so tests exercise actual HTTP requests rather than calling controller
/// methods directly.
/// </summary>
public class ApiProjWebApplicationFactory : WebApplicationFactory<Program>
{
    public string DatabaseName { get; } = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDBContext>));
            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<ApplicationDBContext>(options =>
                options.UseInMemoryDatabase(DatabaseName));
        });
    }

    public HttpClient CreateClientWithCookies()
    {
        return CreateClient(new WebApplicationFactoryClientOptions
        {
            HandleCookies = true,
            BaseAddress = new Uri("http://localhost")
        });
    }
}
