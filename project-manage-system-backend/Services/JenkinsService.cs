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

        public async Task<JenkinsInfoDto> GetJenkinsInfoAsync(string Jobname)
        {
            string jenkinsUrl = "https://localhost:8081/job/TestForProjectOfSoftwareEngineering01";
            string apiUrl = "/api/json?pretty=true";
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "request");
            //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {repo.AccountColonPw}");
            var response = await _httpClient.GetAsync($"{jenkinsUrl}{apiUrl}");
            string content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JenkinsInfoDto>(content);
            result.name = Jobname;
            return result;
        }
    }
}
