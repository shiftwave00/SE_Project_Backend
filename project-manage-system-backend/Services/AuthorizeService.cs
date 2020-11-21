using Microsoft.Extensions.Configuration;
using project_manage_system_backend.Dtos;
using project_manage_system_backend.Models;
using project_manage_system_backend.Shares;
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
        private readonly JwtHelper _jwtHelper;
        private readonly UserService _userService;
        private readonly HttpClient _httpClient;
        public AuthorizeService(IConfiguration configuration, JwtHelper jwt, HttpClient client = null, PMSContext context = null)
        {
            _configuration = configuration;
            _jwtHelper = jwt;

            //for testing
            if(client == null)
            {
                _httpClient = new HttpClient();
            }
            else
            {
                _httpClient = client;
            }

            if(context == null)
            {
                _userService = new UserService();
            }
            else
            {
                _userService = new UserService(context);
            }
        }

        public async Task<string> AuthenticateGithub(RequestGithubLoginDto dto)
        {
            string accessToken = await RequestGithubAccessToken(dto.Code);

            if (!string.IsNullOrEmpty(accessToken))
            {
                UserModel userModel = await RequestGithubUserInfo(accessToken);

                if (!_userService.CheckUserExist(userModel.Account))
                {
                    _userService.CreateUser(userModel);
                }

                return _jwtHelper.GenerateToken(userModel.Account);
            }
            else
            {
                throw new Exception("error code");
            }
        }

        public async Task<string> RequestGithubAccessToken(string code)
        {
            const string url = "https://github.com/login/oauth/access_token";
            string clientId = _configuration.GetValue<string>("githubConfig:client_id");
            string clientSecret = _configuration.GetValue<string>("githubConfig:client_secret");

            RequestOauthDto requestData = new RequestOauthDto
            {
                Client_id = clientId,
                Client_secret = clientSecret,
                Code = code
            };

            var stringContent = new StringContent(JsonSerializer.Serialize(requestData).ToLower(), Encoding.UTF8, "application/json");
            var responseTask = await _httpClient.PostAsync(url, stringContent);

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

        public async Task<UserModel> RequestGithubUserInfo(string accessToken)
        {
            const string url = "https://api.github.com/user";

            UserModel result = null;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "request");
            var responseTask = await _httpClient.GetAsync(url);

            string resultContent = await responseTask.Content.ReadAsStringAsync();
            var userInfo = JsonSerializer.Deserialize<ResponseGuthubUserInfoDto>(resultContent);//反序列化
            result = new UserModel
            {
                Account = "github_" + userInfo.login,
                Name = userInfo.login,
                AvatarUrl = userInfo.avatar_url,
                Authority = "User"
            };
            return result;
        }
    }
}
