using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using project_manage_system_backend;
using project_manage_system_backend.Dtos;
using project_manage_system_backend.Services;
using project_manage_system_backend.Shares;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PMS_test.ControllersTest
{
    [TestCaseOrderer("XUnit.Project.Orderers.AlphabeticalOrderer", "XUnit.Project")]
    public class AuthorizeControllerTests
    {
        private readonly PMSContext _dbContext;
        private readonly HttpClient _client;
        private readonly AuthorizeService _authorizeService;
        private readonly WebApplicationFactory<Startup> factory = new WebApplicationFactory<Startup>();

        public AuthorizeControllerTests()
        {
            _dbContext = new PMSContext(new DbContextOptionsBuilder<PMSContext>()
                .UseSqlite(CreateInMemoryDatabase())
                .Options);
            _dbContext.Database.EnsureCreated();

            var mockHttp = new MockHttpMessageHandler();

            var _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            string clientId = _configuration.GetValue<string>("githubConfig:client_id");
            string clientSecret = _configuration.GetValue<string>("githubConfig:client_secret");

            string content = "{\"client_id\":\"" + clientId + "\",\"client_secret\":\"" + clientSecret + "\",\"code\":\"testcode\"}";
            mockHttp.When(HttpMethod.Post, "https://github.com/login/oauth/access_token")
                    .WithContent(content)
                    .Respond("application/text",
                    "access_token=token&scope=&token_type=bearer");

            mockHttp.When(HttpMethod.Get, "https://api.github.com/user")
                    .Respond("application/json",
                    "{\"login\":\"testuser\",\"avatar_url\":\"test\"}");

            _client = mockHttp.ToHttpClient();
            _authorizeService = new AuthorizeService(_dbContext, _configuration, new JwtHelper(_configuration), _client);
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("DataSource=:memory:");

            connection.Open();

            return connection;
        }

        [Fact]
        public async Task TestCheckAuthentucate()
        {
            AuthorizeDto token = await _authorizeService.AuthenticateGithub(new RequestGithubLoginDto { Code = "testcode" });
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.Token);

            var response = client.GetAsync("/authorize");

            Assert.True(response.Result.IsSuccessStatusCode);
        }
    }
}
