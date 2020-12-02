﻿using Microsoft.AspNetCore.Mvc.Testing;
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

        private const string _owner = "WeedChen";
        private const string _name = "AutoPlaneCup";
        private const string _commitUrl = "https://api.github.com/repos/" + _owner + "/" + _name + "/stats/commit_activity";
        private const string _openIssueUrl = "https://api.github.com/repos/" + _owner + "/" + _name + "/issues?state=open&per_page=100&page=1&sort=created";
        private const string _closedIssueUrl = "https://api.github.com/repos/" + _owner + "/" + _name + "/issues?state=closed&per_page=100&page=1&sort=created";

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

            //issue

            RepoIssuesDto dto = new RepoIssuesDto();
            DateTime closed = Convert.ToDateTime("2020/12/03");
            DateTime created = Convert.ToDateTime("2020/12/01");
            dto.averageDealwithIssueTime = TimeSpan.FromSeconds((created - closed).TotalSeconds).ToString(@"dd\.hh\:mm\:\:ss\.\.");
            dto.openIssues = JsonConvert.DeserializeObject<List<ResponseGithubRepoIssuesDto>>(CreateFakeIssues("open"));
            dto.closeIssues = JsonConvert.DeserializeObject<List<ResponseGithubRepoIssuesDto>>(CreateFakeIssues("closed"));

            string json = JsonConvert.SerializeObject(dto);

            mockHttp.When(HttpMethod.Get, _openIssueUrl)
                    .Respond("application/json", CreateFakeIssues("open"));
            mockHttp.When(HttpMethod.Get, _closedIssueUrl)
                    .Respond("application/json", CreateFakeIssues("closed"));
            return mockHttp.ToHttpClient();
        }

        private string CreateFakeIssues(string status)
        {
            DateTime closed = Convert.ToDateTime("2020-12-03");
            DateTime created = Convert.ToDateTime("2020-12-01");

            List<ResponseGithubRepoIssuesDto> openList = new List<ResponseGithubRepoIssuesDto>()
            {
                new ResponseGithubRepoIssuesDto()
                { created_at=created.ToString("yyyy-MM-dd HH:mm:ss"),number =1,state="open",title="test",user = new user{html_url="",login="aaa" },closed_at="",html_url="" }
            };
            List<ResponseGithubRepoIssuesDto> closedList = new List<ResponseGithubRepoIssuesDto>()
            {
                new ResponseGithubRepoIssuesDto()
                { created_at=created.ToString("yyyy-MM-dd HH:mm:ss"),closed_at=closed.ToString("yyyy-MM-dd HH:mm:ss"),number=2,state="closed",title="test",user = new user{html_url="",login="aaa" },html_url=""}
            };
            string openjson = JsonConvert.SerializeObject(openList);
            string closedjson = JsonConvert.SerializeObject(closedList);
            return status == "open" ? openjson : closedjson;
        }

        private void InitialDatabase()
        {
            _dbContext.Repositories.Add(new Repo
            {
                Name = _name,
                Owner = _owner,
                Url = "https://github.com/" + _owner + "/" + _name + ""
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

        [Fact]
        public async Task TestRequestIssueInfo()
        {
            RepoIssuesDto dto = new RepoIssuesDto();
            DateTime closed = Convert.ToDateTime("2020-12-03");
            DateTime created = Convert.ToDateTime("2020-12-01");
            dto.averageDealwithIssueTime = TimeSpan.FromSeconds((created - closed).TotalSeconds).ToString(@"dd\.hh\:mm\:\:ss\.\.").Replace("..", "秒").Replace(".", "天").Replace("::", "分鐘").Replace(":", "小時"); ;
            dto.openIssues = JsonConvert.DeserializeObject<List<ResponseGithubRepoIssuesDto>>(CreateFakeIssues("open"));
            dto.closeIssues = JsonConvert.DeserializeObject<List<ResponseGithubRepoIssuesDto>>(CreateFakeIssues("closed"));
            var excepted = JsonConvert.SerializeObject(dto);

            var issues = await _repoInfoService.RequestIssueInfo(1, "KENFOnwogneorngIONefokwNGFIONROPGNro");
            string actual = JsonConvert.SerializeObject(issues);

            Assert.Equal(excepted, actual);
        }

    }
}