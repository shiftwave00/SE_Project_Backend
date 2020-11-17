using Microsoft.Extensions.Configuration;
using project_manage_system_backend.Dtos;
using project_manage_system_backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace project_manage_system_backend.Services
{
    public class AuthorizeService
    {
        private readonly IConfiguration _configuration;
        public AuthorizeService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> RequestGithubAccessToken(string code)
        {
            const string url = "https://github.com/login/oauth/access_token";
            string clientId = _configuration.GetValue<string>("githubConfig:client_id");
            string clientSecret = _configuration.GetValue<string>("githubConfig:client_secret");

            using (var client = new HttpClient())
            {
                RequestOauthDto requestData = new RequestOauthDto
                {
                    Client_id = clientId,
                    Client_secret = clientSecret,
                    Code = code
                };

                var stringContent = new StringContent(JsonSerializer.Serialize(requestData).ToLower(), Encoding.UTF8, "application/json");
                var responseTask = await client.PostAsync(url, stringContent);

                string resultContent = await responseTask.Content.ReadAsStringAsync();

                string result = null;
                resultContent.Split('&').ToList().ForEach(x =>
                {
                    if (x.Contains("access_token"))
                    {
                        result = x.Split('=').ToList()[1];
                    }
                });
                return result;
            }
        }

        public async Task<UserModel> RequestGithubUserInfo(string accessToken)
        {
            const string url = "https://api.github.com/user";

            UserModel result = null;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
                client.DefaultRequestHeaders.Add("User-Agent", "request");
                var responseTask = await client.GetAsync(url);

                string resultContent = await responseTask.Content.ReadAsStringAsync();
                var userInfo = JsonSerializer.Deserialize<ResponseGuthubUserInfoDto>(resultContent);//反序列化
                result = new UserModel
                {
                    Account = "github_" + userInfo.login,
                    Name =  userInfo.login,
                    AvatarUrl = userInfo.avatar_url,
                    Authority = "User"
                };
            }
            return result;
        }
    }
}
