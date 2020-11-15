using Microsoft.Extensions.Configuration;
using project_manage_system_backend.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

        public string RequestGithubAccessToken(string code)
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
                    Code = "8a5fa42530d850095c75"
                };

                var stringContent = new StringContent(JsonSerializer.Serialize(requestData).ToLower(), Encoding.UTF8, "application/json");
                var responseTask = client.PostAsync(url, stringContent);
                responseTask.Wait();

                var result = responseTask.Result;
                var ReadAsStringAsync = result.Content.ReadAsStringAsync();

                return "test";
            }
        }
    }
}
