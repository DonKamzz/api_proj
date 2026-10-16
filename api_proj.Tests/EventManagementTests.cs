using System.Net;
using System.Net.Http.Json;
using api_proj.DTOs;
using api_proj.Models;
using Xunit;

namespace api_proj.Tests;

public class EventManagementTests : IClassFixture<ApiProjWebApplicationFactory>
{
    private readonly ApiProjWebApplicationFactory _factory;

    public EventManagementTests(ApiProjWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetEvents_IsPublic_NoLoginRequired()
    {
        var client = _factory.CreateClientWithCookies();

        var response = await client.GetAsync("/api/events");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Organiser_CanCreateEvent()
    {
        var (organiser, _) = await TestHelpers.CreateAuthenticatedClientAsync(_factory, UserRoles.Organiser);

        var response = await organiser.PostAsJsonAsync("/api/events", new EventCreateRequest
        {
            Name = "JHB City Marathon",
            Description = "An iconic road race.",
            EventDate = DateTime.UtcNow.AddMonths(2),
            Location = "Johannesburg",
            DistanceKm = 42.2m,
            EventType = EventTypes.Run
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var ev = await response.Content.ReadFromJsonAsync<EventResponse>(TestHelpers.JsonOptions);
        Assert.Equal("JHB City Marathon", ev!.Name);
    }

    [Fact]
    public async Task Organiser_CanUpdateOwnEvent()
    {
        var (organiser, _) = await TestHelpers.CreateAuthenticatedClientAsync(_factory, UserRoles.Organiser);
        var eventId = await TestHelpers.CreateEventAsync(organiser, "Original Name");

        var response = await organiser.PutAsJsonAsync($"/api/events/{eventId}", new EventUpdateRequest
        {
            Name = "Updated Name",
            Description = "Updated description.",
            EventDate = DateTime.UtcNow.AddMonths(3),
            Location = "Pretoria",
            DistanceKm = 21.1m,
            EventType = EventTypes.Run
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var ev = await response.Content.ReadFromJsonAsync<EventResponse>(TestHelpers.JsonOptions);
        Assert.Equal("Updated Name", ev!.Name);
    }

    [Fact]
    public async Task Organiser_CannotUpdateAnotherOrganisersEvent()
    {
        var (organiserA, _) = await TestHelpers.CreateAuthenticatedClientAsync(_factory, UserRoles.Organiser);
        var (organiserB, _) = await TestHelpers.CreateAuthenticatedClientAsync(_factory, UserRoles.Organiser);
        var eventId = await TestHelpers.CreateEventAsync(organiserA);

        var response = await organiserB.PutAsJsonAsync($"/api/events/{eventId}", new EventUpdateRequest
        {
            Name = "Hijacked Name",
            EventDate = DateTime.UtcNow.AddMonths(1),
            Location = "Cape Town",
            DistanceKm = 5,
            EventType = EventTypes.Walk
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Organiser_CanDeleteOwnEvent()
    {
        var (organiser, _) = await TestHelpers.CreateAuthenticatedClientAsync(_factory, UserRoles.Organiser);
        var eventId = await TestHelpers.CreateEventAsync(organiser);

        var response = await organiser.DeleteAsync($"/api/events/{eventId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await organiser.GetAsync($"/api/events/{eventId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task CreateEvent_WithInvalidEventType_ReturnsBadRequest()
    {
        var (organiser, _) = await TestHelpers.CreateAuthenticatedClientAsync(_factory, UserRoles.Organiser);

        var response = await organiser.PostAsJsonAsync("/api/events", new EventCreateRequest
        {
            Name = "Bad Type Event",
            EventDate = DateTime.UtcNow.AddMonths(1),
            Location = "Durban",
            DistanceKm = 10,
            EventType = "Swim" // not one of Run/Walk/Cycle
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetEvent_ThatDoesNotExist_ReturnsNotFound()
    {
        var client = _factory.CreateClientWithCookies();

        var response = await client.GetAsync("/api/events/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
