using project_manage_system_backend.Dtos;
using project_manage_system_backend.Models;
using project_manage_system_backend.Shares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace project_manage_system_backend.Services
{
    public class RepoInfoService : BaseService
    {
        private readonly HttpClient _httpClient;
        public RepoInfoService(PMSContext dbContext, HttpClient client = null) : base(dbContext)
        {
            _httpClient = client ?? new HttpClient();
        }

        public async Task<CommitInfoDto> RequestCommitInfo(int repoId)
        {
            Repo repo = _dbContext.Repositories.Find(repoId);
            string url = "https://api.github.com/repos/" + repo.Owner + "/" + repo.Name + "/stats/commit_activity";

            _httpClient.DefaultRequestHeaders.Add("User-Agent", "request");

            var response = await _httpClient.GetAsync(url);
            string content = await response.Content.ReadAsStringAsync();
            var commitInfos = JsonSerializer.Deserialize<List<ResponseCommitInfoDto>>(content);

            List<WeekTotalData> weekChartDatas = new List<WeekTotalData>();
            List<DayOfWeekData> detailChartDatas = new List<DayOfWeekData>();

            foreach (var commitInfo in commitInfos)
            {
                weekChartDatas.Add(ConvertToWeekChartData(commitInfo));
                detailChartDatas.Add(ConvertToDetailChartData(commitInfo));
            }

            return new CommitInfoDto
            {
                WeekTotalData = weekChartDatas,
                DayOfWeekData = detailChartDatas
            };
        }

        public async Task<GithubRepoIssuesDto> RequestIssueInfo(int repoId)
        {
            GithubRepoIssuesDto result = new GithubRepoIssuesDto();
            Repo repo = _dbContext.Repositories.Find(repoId);
            List<double> closedTime = new List<double>();
            string url = repo.Url.Replace("github.com/", "api.github.com/repos/");

            result.closeIssues = await GetRepoIssues("closed", url,100);
            result.openIssues = await GetRepoIssues("open", url,100);

            foreach (var item in result.closeIssues)
                closedTime.Add((item.closed_at.Value - item.created_at).TotalSeconds);  
            result.averageDealwithIssueTime = TimeSpan.FromSeconds(closedTime.Average());
            return result;
        }

        private async Task<List<ResponseGithubRepoIssuesDto>> GetRepoIssues(string state, string url,int perPage)
        {
            List<ResponseGithubRepoIssuesDto> result = new List<ResponseGithubRepoIssuesDto>();
           
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "request");
            int page = 0;
            while (true)
            {
                page++;
                var response = await _httpClient.GetAsync(url + $"/issues?state={state}&per_page=100&page={page}&sort=created");
                string content = await response.Content.ReadAsStringAsync();
                var tempList = JsonSerializer.Deserialize<List<ResponseGithubRepoIssuesDto>>(content);
                result.AddRange(tempList);
                if (tempList.Count != perPage)
                    break;
            }
            return result;
        }

        private WeekTotalData ConvertToWeekChartData(ResponseCommitInfoDto commitInfo)
        {
            return new WeekTotalData
            {
                Total = commitInfo.total,
                Week = DateHandler.ConvertToDateString(commitInfo.week)
            };
        }

        private DayOfWeekData ConvertToDetailChartData(ResponseCommitInfoDto commitInfo)
        {
            List<DayCommit> detailDatas = new List<DayCommit>();

            int dayOfWeekCount = 0;
            foreach (var day in commitInfo.days)
            {
                detailDatas.Add(new DayCommit
                {
                    Day = DateHandler.ConvertToDayOfWeek(dayOfWeekCount++),
                    Commit = day
                });
            }

            return new DayOfWeekData
            {
                Week = DateHandler.ConvertToDateString(commitInfo.week),
                DetailDatas = detailDatas
            };
        }


    }
}
