using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using project_manage_system_backend;
using project_manage_system_backend.Dtos;
using project_manage_system_backend.Services;
using project_manage_system_backend.Shares;
using RichardSzalay.MockHttp;
using System;
using System.Data.Common;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace PMS_test
{
    public class AuthorizeServiceTests
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;
        private readonly AuthorizeService _authorizeService;
        private readonly WebApplicationFactory<Startup> factory = new WebApplicationFactory<Startup>();

        public AuthorizeServiceTests()
        {
            var dbContext = new PMSContext(new DbContextOptionsBuilder<PMSContext>()
                .UseSqlite(CreateInMemoryDatabase())
                .Options);

            var mockHttp = new MockHttpMessageHandler();

            var _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            string clientId = _configuration.GetValue<string>("githubConfig:client_id");
            string clientSecret = _configuration.GetValue<string>("githubConfig:client_secret");

            string content = "{\"client_id\":\"" + clientId + "\",\"client_secret\":\"" + clientSecret + "\",\"code\":\"testcode\"}";
            mockHttp.When(HttpMethod.Post, "https://github.com/login/oauth/access_token")
                    .WithContent(content)
                    .Respond("application/text", 
                    "access_token=f1aee02d3e3a2921bdce88dceb7aa6f08558bbe3&scope=&token_type=bearer");

            mockHttp.When(HttpMethod.Get, "https://api.github.com/user")
                    .Respond("application/json",
                    "{\"login\":\"testuser\",\"avatar_url\":\"test\"}");

            _client = mockHttp.ToHttpClient();
            _authorizeService = new AuthorizeService(_configuration, new JwtHelper(_configuration), _client);
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }

        [Fact]
        public async Task TestAuthenticateGithub()
        {
            string token = await _authorizeService.AuthenticateGithub(new RequestGithubLoginDto { Code = "testcode" });

        }
    }
}
