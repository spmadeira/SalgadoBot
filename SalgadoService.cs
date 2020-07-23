using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Newtonsoft.Json;

namespace SalgadoBot
{
    public class SalgadoService
    {
        private static SalgadoService _instance;
        public static async Task<SalgadoService> GetInstance() => _instance ??= await Create();
        
        private readonly HttpClient _httpClient;
        private readonly CookieContainer _cookieContainer;

        private static async Task<SalgadoService> Create()
        {
            var service = new SalgadoService();
            await service.GetAuthorizationCookie();
            return service;
        }
        
        public async Task<List<UserInfo>> GetUsers()
        {
            var result = await _httpClient.GetAsync("/bot/users");

            if (result.IsSuccessStatusCode)
            {
                var users = JsonConvert.DeserializeObject<List<UserInfo>>(
                    await result.Content.ReadAsStringAsync());
                return users;
            } else return new List<UserInfo>();
        }

        public async Task<List<UserInfo>> GetUsersByQuery(string query)
        {
            var result = await _httpClient.GetAsync($"/bot/user?name={query}");

            if (result.IsSuccessStatusCode)
            {
                var users = JsonConvert.DeserializeObject<List<UserInfo>>(
                    await result.Content.ReadAsStringAsync());
                return users;
            } else return new List<UserInfo>();
        }

        public async Task<UserInfo> EditPoints(EditPointsInfo editPointsInfo)
        {
            var content = new StringContent(JsonConvert.SerializeObject(editPointsInfo), Encoding.UTF8, "application/json");
            var result = await _httpClient.PutAsync("/bot/user", content);

            if (result.IsSuccessStatusCode)
            {
                var user = JsonConvert.DeserializeObject<UserInfo>(
                    await result.Content.ReadAsStringAsync());
                return user;
            }
            else return null;
        }
        
        private async Task<bool> GetAuthorizationCookie()
        {
            var info = new AuthKeyInfo {Key = Environment.GetEnvironmentVariable("SS_Authkey")!};
            var content = new StringContent(JsonConvert.SerializeObject(info), Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync("/bot/login", content);
    
            if (result.IsSuccessStatusCode)
            {
                await Logger.Instance.Log(new LogMessage(LogSeverity.Info, "Sal. Serv", "Connected to Salgado Service"));
                return true;
            }
            else
            {
                await Logger.Instance.Log(new LogMessage(LogSeverity.Warning, "Sal. Serv", "Could not connect to Salgado Service"));
                return false;
            }
        }
    
        private SalgadoService()
        {
            _cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler {CookieContainer = _cookieContainer};
            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(Environment.GetEnvironmentVariable("SS_Address")!)
            };
        }

        public class UserInfo
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public int Pontos { get; set; }

            public EditPointsInfo ChangePoints(int points) => new EditPointsInfo {Id = Id, Pontos = points};
        }

        public class EditPointsInfo
        {
            public string Id { get; set; }
            public int Pontos { get; set; }
        }
        
        class AuthKeyInfo
        {
            public string Key { get; set; }
        }
    }
}