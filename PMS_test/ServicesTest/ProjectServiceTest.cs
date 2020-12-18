using Isopoh.Cryptography.Argon2;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using project_manage_system_backend.Dtos;
using project_manage_system_backend.Models;
using project_manage_system_backend.Services;
using project_manage_system_backend.Shares;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Xunit;

namespace PMS_test.ServicesTest
{
    [TestCaseOrderer("XUnit.Project.Orderers.AlphabeticalOrderer", "XUnit.Project")]
    public class ProjectServiceTest
    {
        private readonly PMSContext _dbContext;
        private readonly ProjectService _projectService;

        public ProjectServiceTest()
        {
            _dbContext = new PMSContext(new DbContextOptionsBuilder<PMSContext>()
                                            .UseSqlite(CreateInMemoryDatabase())
                                            .Options);
            _dbContext.Database.EnsureCreated();
            _projectService = new ProjectService(_dbContext);
            InitialDatabase();
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("DataSource=:memory:");

            connection.Open();

            return connection;
        }

        private void InitialDatabase()
        {
            User testUser1 = new User
            {
                Account = "github_testUser1",
                Name = "testUser1",
                Projects = new List<UserProject>(),
            };
            User testUser2 = new User
            {
                Account = "github_testUser2",
                Name = "testUser2",
            };
            User testUser3 = new User
            {
                Account = "github_testUser3",
                Name = "testUser3",
            };

            Project project1 = new Project
            {
                Name = "testProject1",
                Owner = testUser1,
            };
            Project project2 = new Project
            {
                Name = "testProject2",
                Owner = testUser1,
            };

            List<UserProject> testUser1Project = new List<UserProject>()
            {
                new UserProject
                {
                    User = testUser1,
                    Project = project1,
                },
                new UserProject
                {
                    User = testUser2,
                    Project = project1,
                },
            };

            List<UserProject> testUser2Project = new List<UserProject>()
            {
                new UserProject
                {
                    User = testUser1,
                    Project = project2,
                },
                new UserProject
                {
                    User = testUser2,
                    Project = project2,
                },
            };

            List<UserProject> testUser3Project = new List<UserProject>()
            {
                new UserProject
                {
                    User = testUser3,
                    Project = project1,
                },
            };

            project1.Users = testUser1Project;
            project2.Users = testUser2Project;
            //testUser3.Projects = testUser3Project;

            _dbContext.Projects.Add(project1);
            _dbContext.Projects.Add(project2);
            _dbContext.Users.Add(testUser1);
            _dbContext.Users.Add(testUser2);
            _dbContext.Users.Add(testUser3);
            _dbContext.SaveChanges();
        }

        [Fact]
        public void TestGetProjectMember()
        {
            List<UserInfoDto> projectMembers = _projectService.GetProjectMember(1);

            Assert.Equal(2, projectMembers.Count);
            Assert.Equal("testUser1", projectMembers[0].Name);
        }

        [Fact]
        public void TestGetProjectMemberByNotExistProjectId()
        {
            Assert.Throws<Exception>(() => _projectService.GetProjectMember(4));
        }
    }
}
