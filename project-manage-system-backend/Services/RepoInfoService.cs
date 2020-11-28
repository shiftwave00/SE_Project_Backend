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
    public class RepoInfoService:BaseService
    {
        private readonly HttpClient _httpClient;
        public RepoInfoService(PMSContext dbContext, HttpClient client = null) :base(dbContext) 
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

        public async Task<List<ContributorsCommitActivityDto>> RequestContributorsActtivity(int repoId, string oauth_token)
        {
            Repo repo = _dbContext.Repositories.Find(repoId);
            string url = "https://api.github.com/repos/" + repo.Owner + "/" + repo.Name + "/stats/contributors";

            _httpClient.DefaultRequestHeaders.Add("User-Agent", "request");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", oauth_token);
            var response = await _httpClient.GetAsync(url);
            string content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<ContributorsCommitActivityDto>>(content);
            // sort by commit 
            result.Sort((r1, r2) => r2.total.CompareTo(r1.total));

            foreach (var item in result)
            {
                foreach (var week in item.weeks)
                {
                    week.ws = DateHandler.ConvertToDateString(week.w);
                    item.totalAdditions += week.a;
                    item.totalDeletions += week.d;
                }
            }
            return result;
        }
    }
}
