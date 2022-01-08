using Microsoft.EntityFrameworkCore;
using project_manage_system_backend.Dtos;
using project_manage_system_backend.Factory;
using project_manage_system_backend.Models;
using project_manage_system_backend.Repository;
using project_manage_system_backend.Shares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace project_manage_system_backend.Services
{
    public class RepoService : BaseService
    {
        private readonly HttpClient _httpClient;
        private readonly RepoFactory _repoFactory;
        public RepoService(PMSContext dbContext, HttpClient client = null) : base(dbContext)
        {
            _httpClient = client ?? new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(3);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "request");
            _repoFactory = new RepoFactory();
        }

        private async Task<ResponseRepoInfoDto> GetRepositoryInformation(string url)
        {
            IRepo repo = _repoFactory.CreateRepoBy(url, _httpClient, null);
            return await repo.GetRepositoryInformation(url);
        }

        private async Task<ResponseDto> CheckSonarqubeAliveAndProjectExisted(AddRepoDto addRepoDto)
        {
            ResponseDto responseDto = new ResponseDto() { success = false, message = "Sonarqube Error " };
            try
            {
                var sonarqubeUrl = addRepoDto.sonarqubeUrl + $"api/project_analyses/search?project={addRepoDto.projectKey}";
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {addRepoDto.accountColonPassword}");
                var result = await _httpClient.GetAsync(sonarqubeUrl);
                responseDto.success = result.IsSuccessStatusCode;
                responseDto.message = result.IsSuccessStatusCode ? "Sonarqube online" : "Sonarqube Project doesn't exist";
                return responseDto;
            }
            catch (Exception ex)
            {
                responseDto.message.Insert(0, ex.Message);
                return responseDto;
            }
        }

        private async Task<ResponseDto> CheckJenkinsAliveAndProjectExisted(AddRepoDto addRepoDto)
        //�n�ʨ�Ӧh�a��A�����m
        {
            ResponseDto responseDto = new ResponseDto() { success = false, message = "Jenkins Error " };
            try
            {
                //Console.WriteLine(addRepoDto.jenkinsUrl);
                var jenkinsUrl = addRepoDto.jenkinsUrl + $"job/{addRepoDto.jobName}/api/json";
                //Console.WriteLine(jenkinsUrl);
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {addRepoDto.accountColonPwJenkins}");
                var result = await _httpClient.GetAsync(jenkinsUrl);
                responseDto.success = true;
                responseDto.message = result.IsSuccessStatusCode ? "Jenkins online" : "Jenkins Job doesn't exist";
                //Console.WriteLine(responseDto.message);
                //Console.WriteLine(responseDto.success);
                return responseDto;
            }
            catch (Exception ex)
            {
                responseDto.message.Insert(0, ex.Message);
                return responseDto;
            }
        }

        public async Task<ResponseDto> AddRepo(AddRepoDto addRepoDto)
        {
            //Console.WriteLine(addRepoDto.isJenkins);
            //Console.WriteLine(addRepoDto.isSonarqube);
            //Console.WriteLine(addRepoDto.jenkinsUrl);
            //Console.WriteLine(addRepoDto.sonarqubeUrl);
            //Console.WriteLine(addRepoDto.jobName);
            //Console.WriteLine(addRepoDto.projectKey);
            try
            {
                var githubResponse = await GetRepositoryInformation(addRepoDto.url);
                var sonarqubeResponse = await CheckSonarqubeAliveAndProjectExisted(addRepoDto);
                //Console.WriteLine(sonarqubeResponse.message);
                //Console.WriteLine(sonarqubeResponse.success);
                var jenkinsResponse = await CheckJenkinsAliveAndProjectExisted(addRepoDto);
                //Console.WriteLine(jenkinsResponse.message);
                //Console.WriteLine(jenkinsResponse.success);
                ResponseDto result = new ResponseDto() { success = githubResponse.success, message = githubResponse.message };
                if (githubResponse.success)
                {// github repo�s�b
                    if ((!addRepoDto.isSonarqube && !addRepoDto.isJenkins) || (sonarqubeResponse.success && jenkinsResponse.success))
                    {// �S��sonarqube & jenkins    ��    ��git �� sonarqube & jenkins�s�b
                        Repo model = MakeRepoModel(githubResponse, addRepoDto);
                        CreateRepo(model);
                        result.message = "Add Success";
                        return result;
                    }
                    else if(addRepoDto.isSonarqube && !addRepoDto.isJenkins)
                    {// ��sonarqube�Sjenkins
                        if(sonarqubeResponse.success){
                            Repo model = MakeRepoModel(githubResponse, addRepoDto);
                            CreateRepo(model);
                            result.message = "Add Success";
                            return result;
                        }
                        else
                        {
                            result.success = sonarqubeResponse.success;
                            result.message = sonarqubeResponse.message;
                        }
                        
                    }
                    else if (!addRepoDto.isSonarqube && addRepoDto.isJenkins)
                    {// ��jenkins�Ssonarqube
                        if (jenkinsResponse.success)
                        {
                            Repo model = MakeRepoModel(githubResponse, addRepoDto);
                            CreateRepo(model);
                            result.message = "Add Success";
                            return result;
                        }
                        else
                        {
                            result.success = jenkinsResponse.success;
                            result.message = jenkinsResponse.message;
                        }
                    }
                    else
                    {//��ӳ��� ��ӳ������D
                        result.success = false;
                        result.message = "Add Failed, Check the input data";
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return new ResponseDto() { message = ex.Message, success = false };
            }
        }

        private Repo MakeRepoModel(ResponseRepoInfoDto githubResponse, AddRepoDto addRepoDto)
        {
            var project = GetProjectByProjectId(addRepoDto.projectId);
            string owner = string.Empty;
            if (githubResponse.owner != null)
            {
                owner = githubResponse.owner.login ?? githubResponse.owner.name;
            }
            return new Repo()
            {
                Name = githubResponse.name,
                Owner = owner,
                Url = githubResponse.html_url ?? githubResponse.web_url,
                Project = project,
                RepoId = githubResponse.id.ToString(),
                IsSonarqube = addRepoDto.isSonarqube,
                SonarqubeUrl = addRepoDto.sonarqubeUrl,
                AccountColonPw = addRepoDto.accountColonPassword,
                ProjectKey = addRepoDto.projectKey,
                JobName = addRepoDto.jobName,
                IsJenkins = addRepoDto.isJenkins,
                JenkinsUrl = addRepoDto.jenkinsUrl,
                AccountColonPwJenkins = addRepoDto.accountColonPwJenkins
            };
        }

        private void CreateRepo(Repo model)
        {
            //get project by id
            var repo = model.Project.Repositories.Where(r => r.Url == model.Url);
            // check duplicate =>  add or throw exception
            if (!repo.Any())
                _dbContext.Add(model);
            else
                throw new Exception("Duplicate repo!");
            //save
            if (_dbContext.SaveChanges() == 0)
                throw new Exception("DB can't save!");
        }

        public List<Repo> GetRepositoryByProjectId(int id)
        {
            var project = _dbContext.Projects.Where(p => p.ID.Equals(id)).Include(p => p.Repositories).First();
            return project.Repositories;
        }

        public Project GetProjectByProjectId(int id)
        {
            var project = _dbContext.Projects.Include(r => r.Repositories).Where(p => p.ID == id).First();
            return project;
        }

        public bool DeleteRepo(int projectId, int repoId)
        {
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
