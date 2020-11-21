using Microsoft.EntityFrameworkCore;
using project_manage_system_backend.Dtos;
using project_manage_system_backend.Models;
using project_manage_system_backend.Shares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace project_manage_system_backend.Services
{
    public class RepoService
    {
        public async Task<ResponseGithubRepoInfoDto> CheckRepoExist(string url)
        {
            url = url.Replace("github.com", "api.github.com/repos");
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "request");
                var result = await client.GetAsync(url);
                string content = await result.Content.ReadAsStringAsync();
                var msg = JsonSerializer.Deserialize<ResponseGithubRepoInfoDto>(content);
                msg.IsSucess = string.IsNullOrEmpty(msg.message);
                return msg;
            }
        }
        public void CreateRepo(RepoModel model)
        {
            using (var dbContext = new PMSContext())
            {
                //get repo by url
                var repo = dbContext.Repositories.Where(m => m.Url.Contains(model.Url)).ToList();

                // check duplicate =>  add or throw exception
                if (repo.Count == 0)
                    dbContext.Add(model);
                else
                    throw new Exception("Duplicate repo!");

                //save
                if (dbContext.SaveChanges() == 0)
                {
                    throw new Exception("DB can't save!");
                }
            }
        }

        public List<RepoModel> GetRepositoryByProjectId(int id)
        {
            using (var dbContent = new PMSContext())
            {
                var project = dbContent.Projects.Where(p => p.ID.Equals(id)).Include(p => p.Repositories).First();
                return project.Repositories;
            }
        }
    }
}
