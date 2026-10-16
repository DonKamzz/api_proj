using System.Net.Http.Json;
using System.Text.Json;
using api_proj.DTOs;
using api_proj.Models;

namespace api_proj.Tests;

public static class TestHelpers
{
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static int _counter = 0;
    private static int Next() => Interlocked.Increment(ref _counter);

    /// <summary>Registers a new user with a unique email and returns the request used, for reuse in login.</summary>
    public static RegisterRequest BuildRegisterRequest(string role, string? emailPrefix = null)
    {
        var n = Next();
        var prefix = emailPrefix ?? role.ToLower();
        return new RegisterRequest
        {
            FirstName = "Test",
            LastName = $"User{n}",
            Email = $"{prefix}{n}@raceday-tests.co.za",
            Password = "P@ssword123!",
            PhoneNumber = "0821234567",
            Role = role
        };
    }

    /// <summary>Registers and logs in a user of the given role on a fresh, cookie-aware HttpClient.</summary>
    public static async Task<(HttpClient Client, RegisterRequest User)> CreateAuthenticatedClientAsync(
        ApiProjWebApplicationFactory factory, string role)
    {
        var client = factory.CreateClientWithCookies();
        var registerRequest = BuildRegisterRequest(role);

        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.EnsureSuccessStatusCode();

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Email = registerRequest.Email,
            Password = registerRequest.Password
        });
        loginResponse.EnsureSuccessStatusCode();

        return (client, registerRequest);
    }

    /// <summary>Creates an event as the given (already-authenticated Organiser) client and returns its ID.</summary>
    public static async Task<int> CreateEventAsync(HttpClient organiserClient, string name = "Test Event")
    {
        var request = new EventCreateRequest
        {
            Name = name,
            Description = "A test event.",
            EventDate = DateTime.UtcNow.AddDays(30),
            Location = "Johannesburg",
            DistanceKm = 10,
            EventType = EventTypes.Run
        };

        var response = await organiserClient.PostAsJsonAsync("/api/events", request);
        response.EnsureSuccessStatusCode();

        var ev = await response.Content.ReadFromJsonAsync<EventResponse>(JsonOptions);
        return ev!.EventId;
    }

    /// <summary>Creates a category on the given event as its owning Organiser client and returns its ID.</summary>
    public static async Task<int> CreateCategoryAsync(HttpClient organiserClient, int eventId, string name = "Open")
    {
        var request = new CategoryRequest { CategoryName = name, DistanceKm = 10 };
        var response = await organiserClient.PostAsJsonAsync($"/api/events/{eventId}/categories", request);
        response.EnsureSuccessStatusCode();

        var category = await response.Content.ReadFromJsonAsync<CategoryResponse>(JsonOptions);
        return category!.CategoryId;
    }
}
