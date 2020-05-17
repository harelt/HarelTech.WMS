using HarelTech.WMS.Common.Entities;
using HarelTech.WMS.Common.Models;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarelTech.WMS.RestClient
{
    public class WmsClient : IWmsClient
    {
        private static IServiceClient _restClient;
        private static string _apiUrl;
        private static string _userName;
        private static string _password;
        public WmsClient(string apiUrl, string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(apiUrl) || string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Check API connection parameters.");

            _apiUrl = apiUrl;
            _userName = userName;
            _password = password;

            _restClient = new JsonServiceClient(_apiUrl) { OnAuthenticationRequired = ClientAuth, AlwaysSendBasicAuthHeader = true }.WithCache();


        }

        private static void ClientAuth()
        {
            var token = $"{_apiUrl}/users/apiAuth";

            //var response = token.PostToUrl($"username={_user}&password={_password}");
            var response = _restClient.Post<AuthUser>(token, new AuthenticateModel { UserName = _userName, Password = _password });


            if (response == null)
                throw new Exception("Rest client not authenticated with web api");
            _restClient.BearerToken = response.Token;
            //_restClient.AddHeader("Authorization", $"Bearer {response.FromJson<string>()}");
        }

        public async Task<SystemUser> GetSystemUserAsync(long userId)
        {
            var req = $"Users/appAuth";
            var res = await _restClient.PostAsync<SystemUser>(req, userId);
            return res;
        }

        public async Task<List<UserWarhouse>> GetUserWarhousesAsync(string company, long userId)
        {
            var req = $"Warhouses/{company}/{userId}";
            var res = await _restClient.GetAsync<List<UserWarhouse>>(req);
            return res;
        }

        public async Task<TasksSummerize> GetTasksSummerizeAsync(long userId, long warhouseId, string company)
        {
            var req = $"Tasks/Dashboard/{userId}/{warhouseId}/{company}";
            var res = await _restClient.GetAsync<TasksSummerize>(req);
            return res;
        }

        public async Task<List<TaskType>> GetTaskTypesAsync(string company)
        {
            var req = $"Tasks/TaskTypes/{company}";
            var res = await _restClient.GetAsync<List<TaskType>>(req);
            return res;
        }

        public async Task<List<CompleteTasksByGroup>> GetCompleteTasksByGroups(CompleteTasksByGroupRequest request)
        {
            try
            {
                var req = $"Tasks/CompleteTasksByGroup";
                var res = await _restClient.PostAsync<List<CompleteTasksByGroup>>(req, request);
                return res;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        public async Task<List<CompleteTaskItem>> GetCompleteTaskItems(CompleteTaskItemsRequest request)
        {
            var req = $"Tasks/CompleteTaskItems";
            var res = await _restClient.PostAsync<List<CompleteTaskItem>>(req, request);
            return res;
        }
    }
}
