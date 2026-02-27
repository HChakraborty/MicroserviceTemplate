using FluentAssertions;
using SampleAuthService.Domain.Enums;
using SampleAuthService.IntegrationTests.Fixtures;
using SampleAuthService.IntegrationTests.Helpers;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SampleAuthService.IntegrationTests.Api;

public class UserAuthorizationIntegrationTests
    : IClassFixture<ContainersFixture>, IAsyncLifetime
{
    private readonly AuthApiFactory _factory;
    private readonly HttpClient _client;

    public UserAuthorizationIntegrationTests(
        ContainersFixture containers)
    {
        _factory = new AuthApiFactory(containers);
        _client = _factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await _factory.InitializeDatabaseAsync();
        await _factory.ResetDatabaseAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private async Task SeedUserAsync(
        string email,
        UserRole role)
    {
        var response = await _client.PostAsJsonAsync(
            "/api/v1/users",
            new
            {
                Email = email,
                Password = "password",
                Role = role
            });

        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(HttpStatusCode.OK, "User seeding must succeed");
    }

    [Fact]
    public async Task Admin_Should_Access_Any_User()
    {
        await SeedUserAsync("admin@example.com", UserRole.Admin);
        await SeedUserAsync("other@example.com", UserRole.ReadUser);

        var token = TestJwtHelper.GenerateToken(
            "admin@example.com",
            UserRole.Admin);

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response =
            await _client.GetAsync("/api/v1/users/other@example.com");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task WriteUser_Should_Access_ReadUser()
    {
        await SeedUserAsync("writer@example.com", UserRole.WriteUser);
        await SeedUserAsync("read@example.com", UserRole.ReadUser);

        var token = TestJwtHelper.GenerateToken(
            "writer@example.com",
            UserRole.WriteUser);

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response =
            await _client.GetAsync("/api/v1/users/read@example.com");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task WriteUser_Should_Not_Access_Admin()
    {
        await SeedUserAsync("writer@example.com", UserRole.WriteUser);
        await SeedUserAsync("admin@example.com", UserRole.Admin);

        var token = TestJwtHelper.GenerateToken(
            "writer@example.com",
            UserRole.WriteUser);

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response =
            await _client.GetAsync("/api/v1/users/admin@example.com");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ReadUser_Should_Access_Self()
    {
        await SeedUserAsync("reader@example.com", UserRole.ReadUser);

        var token = TestJwtHelper.GenerateToken(
            "reader@example.com",
            UserRole.ReadUser);

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response =
            await _client.GetAsync("/api/v1/users/reader@example.com");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ReadUser_Should_Not_Access_Others()
    {
        await SeedUserAsync("reader@example.com", UserRole.ReadUser);
        await SeedUserAsync("other@example.com", UserRole.ReadUser);

        var token = TestJwtHelper.GenerateToken(
            "reader@example.com",   // 🔥 FIXED domain mismatch
            UserRole.ReadUser);

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response =
            await _client.GetAsync("/api/v1/users/other@example.com");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task NoToken_Should_Return_Unauthorized()
    {
        await SeedUserAsync("any@example.com", UserRole.ReadUser);

        var response =
            await _client.GetAsync("/api/v1/users/any@example.com");

        response.StatusCode.Should()
            .Be(HttpStatusCode.Unauthorized);
    }
}