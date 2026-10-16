using System.Net;
using System.Net.Http.Json;
using api_proj.DTOs;
using api_proj.Models;
using Xunit;

namespace api_proj.Tests;

public class RoleEnforcementTests : IClassFixture<ApiProjWebApplicationFactory>
{
    private readonly ApiProjWebApplicationFactory _factory;

    public RoleEnforcementTests(ApiProjWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Participant_CannotCreateEvent()
    {
        var (participant, _) = await TestHelpers.CreateAuthenticatedClientAsync(_factory, UserRoles.Participant);

        var response = await participant.PostAsJsonAsync("/api/events", new EventCreateRequest
        {
            Name = "Should Not Be Created",
            EventDate = DateTime.UtcNow.AddMonths(1),
            Location = "Soweto",
            DistanceKm = 5,
            EventType = EventTypes.Walk
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UnauthenticatedUser_CannotCreateEvent()
    {
        var client = _factory.CreateClientWithCookies();

        var response = await client.PostAsJsonAsync("/api/events", new EventCreateRequest
        {
            Name = "Should Not Be Created",
            EventDate = DateTime.UtcNow.AddMonths(1),
            Location = "Soweto",
            DistanceKm = 5,
            EventType = EventTypes.Walk
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Organiser_CannotEnrolInAnEvent()
    {
        var (organiser, _) = await TestHelpers.CreateAuthenticatedClientAsync(_factory, UserRoles.Organiser);
        var eventId = await TestHelpers.CreateEventAsync(organiser);
        var categoryId = await TestHelpers.CreateCategoryAsync(organiser, eventId);

        // The Organiser role is explicitly excluded from the Participant-only enrolment endpoint.
        var response = await organiser.PostAsJsonAsync($"/api/events/{eventId}/enrolments", new EnrolRequest
        {
            CategoryId = categoryId
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Participant_CannotViewEnrolmentsForAnEventTheyDoNotOwn()
    {
        var (organiser, _) = await TestHelpers.CreateAuthenticatedClientAsync(_factory, UserRoles.Organiser);
        var (participant, _) = await TestHelpers.CreateAuthenticatedClientAsync(_factory, UserRoles.Participant);
        var eventId = await TestHelpers.CreateEventAsync(organiser);

        var response = await participant.GetAsync($"/api/events/{eventId}/enrolments");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Participant_CannotAddCategoryToEvent()
    {
        var (organiser, _) = await TestHelpers.CreateAuthenticatedClientAsync(_factory, UserRoles.Organiser);
        var (participant, _) = await TestHelpers.CreateAuthenticatedClientAsync(_factory, UserRoles.Participant);
        var eventId = await TestHelpers.CreateEventAsync(organiser);

        var response = await participant.PostAsJsonAsync($"/api/events/{eventId}/categories", new CategoryRequest
        {
            CategoryName = "Should Not Be Added"
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Participant_CannotCaptureResults()
    {
        var (organiser, _) = await TestHelpers.CreateAuthenticatedClientAsync(_factory, UserRoles.Organiser);
        var (participant, _) = await TestHelpers.CreateAuthenticatedClientAsync(_factory, UserRoles.Participant);
        var eventId = await TestHelpers.CreateEventAsync(organiser);

        var response = await participant.PostAsJsonAsync($"/api/events/{eventId}/results", new ResultCreateRequest
        {
            EnrolmentId = 1,
            FinishTimeSeconds = 3600,
            FinishPosition = 1,
            TotalFinishers = 100
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
