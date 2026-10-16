using System.Net;
using System.Net.Http.Json;
using api_proj.DTOs;
using api_proj.Models;
using Xunit;

namespace api_proj.Tests;

public class AuthTests : IClassFixture<ApiProjWebApplicationFactory>
{
    private readonly ApiProjWebApplicationFactory _factory;

    public AuthTests(ApiProjWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsCreatedUser()
    {
        var client = _factory.CreateClientWithCookies();
        var request = TestHelpers.BuildRegisterRequest(UserRoles.Participant);

        var response = await client.PostAsJsonAsync("/api/auth/register", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var user = await response.Content.ReadFromJsonAsync<UserResponse>(TestHelpers.JsonOptions);
        Assert.Equal(request.Email, user!.Email);
        Assert.Equal(UserRoles.Participant, user.Role);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsConflict()
    {
        var client = _factory.CreateClientWithCookies();
        var request = TestHelpers.BuildRegisterRequest(UserRoles.Organiser);

        var first = await client.PostAsJsonAsync("/api/auth/register", request);
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var second = await client.PostAsJsonAsync("/api/auth/register", request);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task Register_WithInvalidRole_ReturnsBadRequest()
    {
        var client = _factory.CreateClientWithCookies();
        var request = TestHelpers.BuildRegisterRequest("Administrator");

        var response = await client.PostAsJsonAsync("/api/auth/register", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithCorrectCredentials_ReturnsOkAndUser()
    {
        var client = _factory.CreateClientWithCookies();
        var registerRequest = TestHelpers.BuildRegisterRequest(UserRoles.Participant);
        await client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Email = registerRequest.Email,
            Password = registerRequest.Password
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var user = await response.Content.ReadFromJsonAsync<UserResponse>(TestHelpers.JsonOptions);
        Assert.Equal(registerRequest.Email, user!.Email);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        var client = _factory.CreateClientWithCookies();
        var registerRequest = TestHelpers.BuildRegisterRequest(UserRoles.Participant);
        await client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Email = registerRequest.Email,
            Password = "TheWrongPassword!"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithUnknownEmail_ReturnsUnauthorized()
    {
        var client = _factory.CreateClientWithCookies();

        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Email = "nobody-registered@raceday-tests.co.za",
            Password = "WhateverPassword1!"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetMyProfile_WithoutLoggingIn_ReturnsUnauthorized()
    {
        var client = _factory.CreateClientWithCookies();

        var response = await client.GetAsync("/api/users/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Logout_SignsOutSoProfileIsNoLongerAccessible()
    {
        var (client, _) = await TestHelpers.CreateAuthenticatedClientAsync(_factory, UserRoles.Participant);

        var beforeLogout = await client.GetAsync("/api/users/me");
        Assert.Equal(HttpStatusCode.OK, beforeLogout.StatusCode);

        var logoutResponse = await client.PostAsync("/api/auth/logout", null);
        Assert.Equal(HttpStatusCode.OK, logoutResponse.StatusCode);

        var afterLogout = await client.GetAsync("/api/users/me");
        Assert.Equal(HttpStatusCode.Unauthorized, afterLogout.StatusCode);
    }
}
