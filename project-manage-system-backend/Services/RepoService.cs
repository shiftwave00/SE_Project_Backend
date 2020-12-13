using Microsoft.EntityFrameworkCore;
using project_manage_system_backend.Dtos;
using project_manage_system_backend.Models;
using project_manage_system_backend.Shares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace project_manage_system_backend.Services
{
    public class RepoService : BaseService
    {
        private readonly HttpClient _httpClient;
        public RepoService(PMSContext dbContext, HttpClient client = null) : base(dbContext)
        {
            _httpClient = client ?? new HttpClient();
        }

        public async Task<ResponseGithubRepoInfoDto> CheckRepoExist(string url)
        {
            string matchPatten = @"^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$";
            if (!Regex.IsMatch(url, matchPatten))
                return new ResponseGithubRepoInfoDto() { message = "Url Error" };

            url = url.Replace(".git", "");
            url = url.Replace("github.com", "api.github.com/repos");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "request");
            var result = await _httpClient.GetAsync(url);
            string content = await result.Content.ReadAsStringAsync();
            var msg = JsonSerializer.Deserialize<ResponseGithubRepoInfoDto>(content);
            msg.IsSucess = string.IsNullOrEmpty(msg.message);
            return msg;
        }
        public void CreateRepo(Repo model)
        {
            //get project by id
            var project = _dbContext.Projects.Include(r => r.Repositories).Where(p => p.ID == model.Project.ID).First();

            var repo = project.Repositories.Where(r => r.Url == model.Url);

            // check duplicate =>  add or throw exception
            if (!repo.Any())
                _dbContext.Add(model);
            else
                throw new Exception("Duplicate repo!");

            //save
            if (_dbContext.SaveChanges() == 0)
            {
                throw new Exception("DB can't save!");
            }
        }

        public List<Repo> GetRepositoryByProjectId(int id)
        {
            var project = _dbContext.Projects.Where(p => p.ID.Equals(id)).Include(p => p.Repositories).First();
            return project.Repositories;
        }

        public Project GetProjectByProjectId(int id)
        {
            var project = _dbContext.Projects.Single(p => p.ID == id);
            return project;
        }

        public bool DeleteRepo(int projectId, int repoId)
        {
            //var project = _dbContext.Projects.Include(r => r.Repositories).Where(p => p.ID == projectId).First();
            try
            {
                var repo = _dbContext.Repositories.Include(p => p.Project).First(r => r.ID == repoId && r.Project.ID == projectId);
                _dbContext.Repositories.Remove(repo);
                return !(_dbContext.SaveChanges() == 0);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
