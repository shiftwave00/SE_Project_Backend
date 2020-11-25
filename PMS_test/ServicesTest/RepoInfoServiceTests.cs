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
    public class RepoInfoServiceTests
    {
        private readonly PMSContext _dbContext;
        private readonly HttpClient _client;
        private readonly RepoInfoService _repoInfoService;

        private const string _owner = "";
        private const string _name = "";
        private const string _commitUrl = "https://api.github.com/repos/" + _owner + "/" + _name + "/stats/commit_activity";

        public RepoInfoServiceTests()
        {
            _dbContext = new PMSContext(new DbContextOptionsBuilder<PMSContext>()
               .UseSqlite(CreateInMemoryDatabase())
               .Options);
            _dbContext.Database.EnsureCreated();
            _client = CreateMockClient();
            _repoInfoService = new RepoInfoService(_dbContext, _client);
            InitialDatabase();
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("DataSource=:memory:");

            connection.Open();

            return connection;
        }

        private HttpClient CreateMockClient()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When(HttpMethod.Get, _commitUrl)
                    .Respond("application/json",
                    "[{\"total\":2,\"week\":1575158400,\"days\":[2,0,0,0,0,0,0]},{\"total\":7,\"week\":1575763200,\"days\":[0,2,0,2,3,0,0]}]");

            return mockHttp.ToHttpClient();
        }

        private void InitialDatabase()
        {
            _dbContext.Repositories.Add(new Repo
            {
                Name = _name,
                Owner = _owner,
                Url = "https://github.com/" + _owner + "" + _name + ""
            });
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task TestRequestCommitInfo()
        {
            var commitInfo = await _repoInfoService.RequestCommitInfo(1);

            List<WeekTotalData> weekTotalDatas = new List<WeekTotalData>()
            {
                new WeekTotalData(){Total=2,Week="12/01"},
                new WeekTotalData(){Total=7,Week="12/08"}
            };

            List<DayCommit> dayCommitsFirst = new List<DayCommit>()
            {
                new DayCommit(){Day=DateHandler.ConvertToDayOfWeek(0),Commit=2},
                new DayCommit(){Day=DateHandler.ConvertToDayOfWeek(1),Commit=0},
                new DayCommit(){Day=DateHandler.ConvertToDayOfWeek(2),Commit=0},
                new DayCommit(){Day=DateHandler.ConvertToDayOfWeek(3),Commit=0},
                new DayCommit(){Day=DateHandler.ConvertToDayOfWeek(4),Commit=0},
                new DayCommit(){Day=DateHandler.ConvertToDayOfWeek(5),Commit=0},
                new DayCommit(){Day=DateHandler.ConvertToDayOfWeek(6),Commit=0}
            };

            List<DayCommit> dayCommitsSecond = new List<DayCommit>()
            {
                new DayCommit(){Day=DateHandler.ConvertToDayOfWeek(0),Commit=0},
                new DayCommit(){Day=DateHandler.ConvertToDayOfWeek(1),Commit=2},
                new DayCommit(){Day=DateHandler.ConvertToDayOfWeek(2),Commit=0},
                new DayCommit(){Day=DateHandler.ConvertToDayOfWeek(3),Commit=2},
                new DayCommit(){Day=DateHandler.ConvertToDayOfWeek(4),Commit=3},
                new DayCommit(){Day=DateHandler.ConvertToDayOfWeek(5),Commit=0},
                new DayCommit(){Day=DateHandler.ConvertToDayOfWeek(6),Commit=0}
            };

            List<DayOfWeekData> dayOfWeekDatas = new List<DayOfWeekData>()
            {
                new DayOfWeekData(){DetailDatas=dayCommitsFirst,Week="12/01"},
                new DayOfWeekData(){DetailDatas=dayCommitsSecond,Week="12/08"}
            };

            var expected = new CommitInfoDto
            {
                DayOfWeekData = dayOfWeekDatas,
                WeekTotalData = weekTotalDatas
            };

            var expectedStr = JsonConvert.SerializeObject(expected);
            var commitInfoStr = JsonConvert.SerializeObject(commitInfo);
            Assert.Equal(expectedStr, commitInfoStr);
        }
    }
}
