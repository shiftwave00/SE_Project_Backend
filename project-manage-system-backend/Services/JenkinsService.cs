using project_manage_system_backend.Dtos.Jenkins;
using project_manage_system_backend.Models;
using project_manage_system_backend.Shares;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System;

namespace project_manage_system_backend.Services
{
    public class JenkinsService : BaseService
    {
        private readonly HttpClient _httpClient;

        public JenkinsService(PMSContext dbContext, HttpClient client = null) : base(dbContext)
        {
            _httpClient = client ?? new HttpClient();
        }

        public async Task<JenkinsInfoDto> GetJenkinsInfoAsync(int repoId)
        {
            Repo repo = _dbContext.Repositories.Find(repoId);
            string jenkinsUrl = repo.JenkinsUrl;
            string apiUrl = "/api/json?pretty=true";
            string jobName = repo.JobName;
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "request");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {repo.AccountColonPwJenkins}");
            var response = await _httpClient.GetAsync($"{jenkinsUrl}job/{jobName}{apiUrl}");
            string content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JenkinsInfoDto>(content);
            //Console.WriteLine(result.healthReport);
            return result;
        }

        public async Task<JenkinsJobInfoDto> GetJenkinsJobIssue (int repoId)
        {
            Repo repo = _dbContext.Repositories.Find(repoId);
            string jenkinsUrl = repo.JenkinsUrl;
            string apiUrl = "/api/json?pretty=true&tree=builds[*]";
            string jobName = repo.JobName;
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "request");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {repo.AccountColonPwJenkins}");
            var response = await _httpClient.GetAsync($"{jenkinsUrl}job/{jobName}{apiUrl}");
            string content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JenkinsJobInfoDto>(content);
            //Console.WriteLine(result.healthReport);
            return result;
        }

        public async Task<bool> IsHaveJenkins(int repoId)
        {
            Repo repo = await _dbContext.Repositories.FindAsync(repoId);
            return repo.IsJenkins;
        }

    }
}
