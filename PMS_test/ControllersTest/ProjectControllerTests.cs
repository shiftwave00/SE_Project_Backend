using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using project_manage_system_backend;
using project_manage_system_backend.Dtos;
using project_manage_system_backend.Services;
using project_manage_system_backend.Shares;
using RichardSzalay.MockHttp;
using System.Net.Http;
using System.Data.Common;
using System.Threading.Tasks;
using project_manage_system_backend.Controllers;
using project_manage_system_backend.Models;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;

namespace PMS_test.ControllersTest
{
    [TestCaseOrderer("XUnit.Project.Orderers.AlphabeticalOrderer", "XUnit.Project")]
    public class ProjectControllerTests:BaseControllerTests
    {
        public ProjectControllerTests():base()
        {
            InitialDatabase();
        }

        internal void InitialDatabase()
        {
            _dbContext.Users.Add(new User
            {
                Account = "testAccount",
                Authority = "User",
                AvatarUrl = "none",
                Name = "name"
            });
            var user = _dbContext.Users.Find("github_testuser");
            var project = new UserProject
            {
                Project = new Project
                {
                    Name = "project",
                    Owner = user,
                    Repositories = null
                },
                User = user
            };
            user.Projects.Add(project);
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task TestAddProjectSuccess()
        {
            ProjectDto dto = new ProjectDto
            {
                ProjectName = "testProject",
                UserId = "testAccount"
            };

            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var requestTask = await _client.PostAsync("/project", content);


            Assert.True(requestTask.IsSuccessStatusCode);

            var autual = _dbContext.Projects.Find(2);

            string expectedName = "testProject";

            Assert.Equal(expectedName, autual.Name);
        }

        [Fact]
        public async Task TestAddProjectFail()
        {
            ProjectDto dto = new ProjectDto
            {
                ProjectName = "testProject",
                UserId = "notExistUser"
            };

            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var requestTask = await _client.PostAsync("/project", content);

            Assert.True(requestTask.StatusCode == System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task TestGetProject()
        {
            var requestTask = await _client.GetAsync("/project");

            string resultContent = await requestTask.Content.ReadAsStringAsync();
            var actual = JsonSerializer.Deserialize<List<ProjectResultDto>>(resultContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var expected = new List<ProjectResultDto>()
            {
                new ProjectResultDto
                {
                    Id=1,
                    Name="project"
                }
            };

            var expectedStr = JsonSerializer.Serialize(expected);
            var actualStr = JsonSerializer.Serialize(actual);
            Assert.Equal(expectedStr, actualStr);
        }
    }
}
