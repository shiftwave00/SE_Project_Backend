﻿using project_manage_system_backend.Dtos;
using project_manage_system_backend.Models;
using project_manage_system_backend.Shares;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System;

namespace project_manage_system_backend.Services
{
    public class SonarqubeService : BaseService
    {
        private readonly HttpClient _httpClient;
        private const int PAGE_SIZE = 500;

        public SonarqubeService(PMSContext dbContext, HttpClient client = null) : base(dbContext)
        {
            _httpClient = client ?? new HttpClient();
        }

        public async Task<SonarqubeInfoDto> GetSonarqubeInfoAsync(int repoId)
        {
            Repo repo = _dbContext.Repositories.Find(repoId);
            string sonarqubeHostUrl = repo.SonarqubeUrl;
            string apiUrl = "api/measures/search?";
            string projectKey = repo.ProjectKey;
            string query = "&metricKeys=bugs,vulnerabilities,code_smells,duplicated_lines_density,coverage";
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "request");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {repo.AccountColonPw}");
            var response = await _httpClient.GetAsync($"{sonarqubeHostUrl}{apiUrl}projectKeys={projectKey}{query}");
            string content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SonarqubeInfoDto>(content);
            result.projectName = projectKey;
            return result;
        }

        public async Task<Dictionary<string, List<Issues>>> GetSonarqubeCodeSmellAsync(int repoId)
        {
            Repo repo = _dbContext.Repositories.Find(repoId);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "request");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {repo.AccountColonPw}");
            var result = await RequestCodeSmellData(repoId);
            int totalPages = (result.total - 1) / PAGE_SIZE + 1;
            
            for (int i = 2; i <= totalPages; i++)
            {
                var others = await RequestCodeSmellData(repoId, i);
                result.issues.AddRange(others.issues);
            }

            return MapIssueBy(result.issues);
        }

        public async Task<Dictionary<string, List<Issues>>> GetSonarqubeBugAsync(int repoId)
        {
            Repo repo = _dbContext.Repositories.Find(repoId);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "request");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {repo.AccountColonPw}");
            var result = await RequestBugData(repoId);
            int totalPages = (result.total - 1) / PAGE_SIZE + 1;
            Console.Write(result.total);
            Console.Write(result.issues);
            for (int i = 2; i <= totalPages; i++)
            {
                var others = await RequestBugData(repoId, i);
                result.issues.AddRange(others.issues);
            }

            return MapIssueBy(result.issues);
        }

        private Dictionary<string, List<Issues>> MapIssueBy(List<Issues> issues)
        {
            Dictionary<string, List<Issues>> info = new Dictionary<string, List<Issues>>();

            foreach (var item in issues)
            {
                List<Issues> theIssue;
                if (info.TryGetValue(item.component, out theIssue))
                {
                    theIssue.Add(item);
                }
                else
                {
                    info.Add(item.component, new List<Issues>() { item });
                }
            }

            return info;
        }

        private async Task<CodeSmellDataDto> RequestCodeSmellData(int repoId, int pageIndex = 1)
        {
            Repo repo = _dbContext.Repositories.Find(repoId);
            string sonarqubeHostUrl = repo.SonarqubeUrl;
            string apiUrl = "api/issues/search?";
            string projectKey = repo.ProjectKey;
            string query = $"componentKeys={projectKey}&s=FILE_LINE&resolved=false&ps={PAGE_SIZE}&organization=default-organization&facets=severities%2Ctypes&types=CODE_SMELL";
            var response = await _httpClient.GetAsync($"{sonarqubeHostUrl}{apiUrl}projectKeys={projectKey}&{query}&p={pageIndex}");
            string content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CodeSmellDataDto>(content);
        }

        private async Task<BugDataDto> RequestBugData(int repoId, int pageIndex = 1)
        {
            Repo repo = _dbContext.Repositories.Find(repoId);
            string sonarqubeHostUrl = repo.SonarqubeUrl;
            string apiUrl = "api/issues/search?";
            string projectKey = repo.ProjectKey;
            string query = $"componentKeys={projectKey}&s=FILE_LINE&resolved=false&ps={PAGE_SIZE}&organization=default-organization&facets=severities%2Ctypes&types=BUG";
            var response = await _httpClient.GetAsync($"{sonarqubeHostUrl}{apiUrl}projectKeys={projectKey}&{query}&p={pageIndex}");
            string content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<BugDataDto>(content);
        }

        public async Task<SonarSheetDto> GetCoverage(int repoId, int pageIndex = 1)
        {
            Repo repo = _dbContext.Repositories.Find(repoId);
            string sonarqubeHostUrl = repo.SonarqubeUrl;
            string apiUrl = "api/measures/component_tree?";
            string projectKey = repo.ProjectKey;
            string query = $"component={projectKey}&addittionalFields=metrics&ps={PAGE_SIZE}&metricKeys=complexity,coverage,uncovered_lines&qualifiers=FIL,TRK";
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "request");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {repo.AccountColonPw}");
            var response = await _httpClient.GetAsync($"{sonarqubeHostUrl}{apiUrl}projectKeys={projectKey}&{query}&p={pageIndex}");
            string content = await response.Content.ReadAsStringAsync(); 
            var result = JsonSerializer.Deserialize<SonarSheetDto>(content);
            Console.WriteLine(result);
            return result;
        }

        public async Task<SonarSheetDto> GetDuplication(int repoId, int pageIndex = 1)
        {
            Repo repo = _dbContext.Repositories.Find(repoId);
            string sonarqubeHostUrl = repo.SonarqubeUrl;
            string apiUrl = "api/measures/component_tree?";
            string projectKey = repo.ProjectKey;
            string query = $"component={projectKey}&addittionalFields=metrics&ps={PAGE_SIZE}&metricKeys=ncloc,duplicated_lines,duplicated_blocks&qualifiers=FIL,TRK";
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "request");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {repo.AccountColonPw}");
            var response = await _httpClient.GetAsync($"{sonarqubeHostUrl}{apiUrl}projectKeys={projectKey}&{query}&p={pageIndex}");
            string content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SonarSheetDto>(content);
            Console.WriteLine(result);
            return result;
        }

        public async Task<bool> IsHaveSonarqube(int repoId)
        {
            Repo repo = await _dbContext.Repositories.FindAsync(repoId);
            return repo.IsSonarqube;
        }
    }
}
