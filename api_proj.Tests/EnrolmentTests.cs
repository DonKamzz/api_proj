using System.Net;
using System.Net.Http.Json;
using api_proj.DTOs;
using api_proj.Models;
using Xunit;

namespace api_proj.Tests;

public class EnrolmentTests : IClassFixture<ApiProjWebApplicationFactory>
{
    private readonly ApiProjWebApplicationFactory _factory;

    public EnrolmentTests(ApiProjWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Participant_CanEnrol_AndEnrolmentIsRecorded()
    {
        var (organiser, _) = await TestHelpers.CreateAuthenticatedClientAsync(_factory, UserRoles.Organiser);
        var eventId = await TestHelpers.CreateEventAsync(organiser, "Soweto Fun Walk");
        var categoryId = await TestHelpers.CreateCategoryAsync(organiser, eventId, "Senior");

        var (participant, participantDetails) = await TestHelpers.CreateAuthenticatedClientAsync(_factory, UserRoles.Participant);

        var enrolResponse = await participant.PostAsJsonAsync($"/api/events/{eventId}/enrolments", new EnrolRequest
        {
            CategoryId = categoryId
        });

        Assert.Equal(HttpStatusCode.Created, enrolResponse.StatusCode);
        var enrolment = await enrolResponse.Content.ReadFromJsonAsync<EnrolmentResponse>(TestHelpers.JsonOptions);
        Assert.Equal(eventId, enrolment!.EventId);
        Assert.Equal(categoryId, enrolment.CategoryId);
        Assert.Equal(EnrolmentStatuses.Confirmed, enrolment.EnrolmentStatus);

        // Confirm it is correctly recorded and retrievable from the Participant's own list.
        var myEnrolmentsResponse = await participant.GetAsync("/api/users/me/enrolments");
        Assert.Equal(HttpStatusCode.OK, myEnrolmentsResponse.StatusCode);

        var myEnrolments = await myEnrolmentsResponse.Content.ReadFromJsonAsync<List<EnrolmentResponse>>(TestHelpers.JsonOptions);
        Assert.Contains(myEnrolments!, e => e.EventId == eventId && e.CategoryId == categoryId);

        // Confirm the Organiser can see it too, for their own event.
        var organiserViewResponse = await organiser.GetAsync($"/api/events/{eventId}/enrolments");
        Assert.Equal(HttpStatusCode.OK, organiserViewResponse.StatusCode);
        var organiserView = await organiserViewResponse.Content.ReadFromJsonAsync<List<EnrolmentResponse>>(TestHelpers.JsonOptions);
        Assert.Single(organiserView!);
    }

    [Fact]
    public async Task Participant_CannotEnrolTwiceInTheSameEvent()
    {
        var (organiser, _) = await TestHelpers.CreateAuthenticatedClientAsync(_factory, UserRoles.Organiser);
        var eventId = await TestHelpers.CreateEventAsync(organiser);
        var categoryId = await TestHelpers.CreateCategoryAsync(organiser, eventId);

        var (participant, _) = await TestHelpers.CreateAuthenticatedClientAsync(_factory, UserRoles.Participant);

        var first = await participant.PostAsJsonAsync($"/api/events/{eventId}/enrolments", new EnrolRequest { CategoryId = categoryId });
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var second = await participant.PostAsJsonAsync($"/api/events/{eventId}/enrolments", new EnrolRequest { CategoryId = categoryId });
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task Enrol_InNonExistentEvent_ReturnsNotFound()
    {
        var (participant, _) = await TestHelpers.CreateAuthenticatedClientAsync(_factory, UserRoles.Participant);

        var response = await participant.PostAsJsonAsync("/api/events/999999/enrolments", new EnrolRequest { CategoryId = 1 });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UnauthenticatedUser_CannotEnrol()
    {
        var client = _factory.CreateClientWithCookies();

        var response = await client.PostAsJsonAsync("/api/events/1/enrolments", new EnrolRequest { CategoryId = 1 });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
