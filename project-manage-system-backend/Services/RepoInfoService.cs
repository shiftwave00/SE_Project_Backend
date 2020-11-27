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
            _httpClient = client == null ? new HttpClient() : client;
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

        public async Task<List<CodebaseDto>> RequestCodebase(int repoId)
        {
            Repo repo = _dbContext.Repositories.Find(repoId);
            string repoURL = "https://api.github.com/repos/" + repo.Owner + "/" + repo.Name + "/stats/code_frequency";

            _httpClient.DefaultRequestHeaders.Add("User-Agent", "request");

            var response = await _httpClient.GetAsync(repoURL);
            var contents = await response.Content.ReadAsStringAsync();
            var codebases = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int[]>>(contents);

            List<CodebaseDto> codebaseSet = new List<CodebaseDto>();

            foreach (var codebase in codebases)
            {
                CodebaseDto codebaseDto = new CodebaseDto() {
                    date = DateHandler.ConvertToDateString(codebase[0]),
                    numberOfRowsAdded = Convert.ToInt32(codebase[1]),
                    numberOfRowsDeleted = Convert.ToInt32(codebase[2]),
                    numberOfRows = Convert.ToInt32(codebase[1]) + Convert.ToInt32(codebase[2])
                };

                codebaseSet.Add(codebaseDto);
            }

            int thisWeekRows = 0;

            foreach (var codebase in codebaseSet)
            {
                codebase.numberOfRows += thisWeekRows;
                thisWeekRows = codebase.numberOfRows;
            }

            return codebaseSet;
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
            List<Dtos.DayCommit> detailDatas = new List<Dtos.DayCommit>();

            int dayOfWeekCount = 0;
            foreach (var day in commitInfo.days)
            {
                detailDatas.Add(new Dtos.DayCommit
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
