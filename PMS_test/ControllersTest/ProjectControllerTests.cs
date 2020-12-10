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
            _dbContext.Users.Add(new User
            {
                Account = "testAccount2",
                Authority = "User2",
                AvatarUrl = "none2",
                Name = "name2"
            });
            var user = _dbContext.Users.Find("testAccount");
            var project = new UserProject
            {
                Project = new Project
                {
                    Name = "testproject",
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
            ProjectDto dto;

            dto = new ProjectDto
            {
                ProjectName = "testProject",
            };

            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var requestTask = await _client.PostAsync("/project/add", content);

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
                ProjectName = "testProject"
            };

            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var requestTask = await _client.PostAsync("/project/add", content);

            var result = requestTask.Content.ReadAsStringAsync().Result;
            var responseDto = JsonSerializer.Deserialize<ResponseDto>(result);
            Assert.False(responseDto.success);
        }

        [Fact]
        public async Task TestAddDuplicateProject()
        {
            ProjectDto dto = new ProjectDto
            {
                ProjectName = "testProject"
            };

            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var requestTask = await _client.PostAsync("/project/add", content);

            Assert.True(requestTask.IsSuccessStatusCode);

            ProjectDto duplicateDto = new ProjectDto
            {
                ProjectName = "testProject"
            };

            var duplicateContent = new StringContent(JsonSerializer.Serialize(duplicateDto), Encoding.UTF8, "application/json");
            var duplicateRequestTask = await _client.PostAsync("/project/add", duplicateContent);

            var result = duplicateRequestTask.Content.ReadAsStringAsync().Result;
            var responseDto = JsonSerializer.Deserialize<ResponseDto>(result);
            Assert.False(responseDto.success);
        }

        [Fact]
        public async Task TestAddEmptyProjectName()
        {
            ProjectDto dto = new ProjectDto
            {
                ProjectName = "",
            };

            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var requestTask = await _client.PostAsync("/project/add", content);

            var result = requestTask.Content.ReadAsStringAsync().Result;
            var responseDto = JsonSerializer.Deserialize<ResponseDto>(result);
            Assert.False(responseDto.success);
        }

        [Fact]
        public async Task TestEditProjectName()
        {
            ProjectDto dto;

            dto = new ProjectDto
            {
                ProjectName = "testProject"
            };

            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var requestTask = await _client.PostAsync("/project/add", content);

            Assert.True(requestTask.IsSuccessStatusCode);

            dto = new ProjectDto
            {
                ProjectId = 2,
                ProjectName = "newEditProject",
            };

            content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            requestTask = await _client.PostAsync("/project/edit", content);

            Assert.True(requestTask.IsSuccessStatusCode);

            var autual = _dbContext.Projects.Find(2);
            string expectedName = "newEditProject";

            Assert.Equal(expectedName, autual.Name);
        }

        [Fact]
        public async Task TestEditProjectNameWithEmptyName()
        {
            ProjectDto dto;

            dto = new ProjectDto
            {
                ProjectName = "testProject"
            };

            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var requestTask = await _client.PostAsync("/project/add", content);

            Assert.True(requestTask.IsSuccessStatusCode);

            dto = new ProjectDto
            {
                ProjectId = 2,
                ProjectName = "",
            };

            content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            requestTask = await _client.PostAsync("/project/edit", content);


            var result = requestTask.Content.ReadAsStringAsync().Result;
            var responseDto = JsonSerializer.Deserialize<ResponseDto>(result);

            Assert.False(responseDto.success);
        }

        [Fact]
        public async Task TestEditProjectNameWithDuplicateName()
        {
            ProjectDto dto;

            dto = new ProjectDto
            {
                ProjectName = "testProject"
            };

            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            await _client.PostAsync("/project/add", content);

            dto = new ProjectDto
            {
                ProjectName = "testProject2"
            };

            content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            await _client.PostAsync("/project/add", content);


            dto = new ProjectDto
            {
                ProjectId = 4,
                ProjectName = "testProject2"
            };

            content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var requestTask = await _client.PostAsync("/project/edit", content);


            var result = requestTask.Content.ReadAsStringAsync().Result;
            var responseDto = JsonSerializer.Deserialize<ResponseDto>(result);

            Assert.False(responseDto.success);
        }

        [Fact]
        public async Task TestGetProject()
        {
            ProjectDto dto = new ProjectDto
            {
                ProjectName = "testProject"
            };

            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            await _client.PostAsync("/project/add", content);

            var requestTask = await _client.GetAsync("/project");

            string resultContent = await requestTask.Content.ReadAsStringAsync();
            var actual = JsonSerializer.Deserialize<List<ProjectResultDto>>(resultContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var expected = new List<ProjectResultDto>()
            {
                new ProjectResultDto
                {
                    Id=2,
                    Name="testProject",
                    OwnerId = "github_testuser",
                    OwnerName = "testuser"
                }
            };

            var expectedStr = JsonSerializer.Serialize(expected);
            var actualStr = JsonSerializer.Serialize(actual);
            Assert.Equal(expectedStr, actualStr);
        }
    }
}
