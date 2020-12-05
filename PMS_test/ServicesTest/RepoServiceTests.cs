using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using project_manage_system_backend;
using project_manage_system_backend.Services;
using project_manage_system_backend.Shares;
using project_manage_system_backend.Models;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using project_manage_system_backend.Dtos;
using Newtonsoft.Json;

namespace PMS_test.ControllersTest
{
    [TestCaseOrderer("XUnit.Project.Orderers.AlphabeticalOrderer", "XUnit.Project")]
    public class RepoServiceTests
    {
        private readonly PMSContext _dbContext;
        private readonly RepoService _repoService;

        private const string _owner = "shark";
        private const string _name = "a";

        public RepoServiceTests()
        {
            _dbContext = new PMSContext(new DbContextOptionsBuilder<PMSContext>()
               .UseSqlite(CreateInMemoryDatabase())
               .Options);
            _dbContext.Database.EnsureCreated();
            _repoService = new RepoService(_dbContext);
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
            var repo = new Repo
            {
                Name = _name,
                Owner = _owner,
                Url = "https://github.com/" + _owner + "/" + _name + ""
            };
            var project = new Project
            {
                Name = "AAAA",
                Repositories = new List<Repo>() { repo },
            };
            _dbContext.Repositories.Add(repo);
            _dbContext.Projects.Add(project);
            _dbContext.SaveChanges();
        }

        [Fact]
        public void TestGetRepositoryByProjectId()
        {
            var repo = _repoService.GetRepositoryByProjectId(1);
            Assert.Single(repo);
            Assert.Equal(_name, repo[0].Name);
            Assert.Equal(_owner, repo[0].Owner);
        }
    }
}
