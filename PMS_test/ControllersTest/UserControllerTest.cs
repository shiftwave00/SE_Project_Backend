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
    public class UserControllerTests : BaseControllerTests
    {
        public UserControllerTests() : base()
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
        public async Task EditUserNameTest()
        {
            UserInfoDto dto = new UserInfoDto
            {
                Name = "testEditUser",
            };

            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var requestTask = await _client.PostAsync("/user/edit", content);

            Assert.True(requestTask.IsSuccessStatusCode);

            var actual = _dbContext.Users.Find("github_testuser");

            actual.Name = "testEditUser";

            _dbContext.Update(actual);

            _dbContext.SaveChanges();

            actual = _dbContext.Users.Find("github_testuser");

            string expectedName = "testEditUser";

            Assert.Equal(expectedName, actual.Name);
        }

    }
}

