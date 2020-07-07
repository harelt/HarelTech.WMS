using HarelTech.WMS.Common.Entities;
using HarelTech.WMS.Common.Models;
using ServiceStack;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
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

            _restClient = new JsonHttpClient(_apiUrl)
            {
                HttpMessageHandler = new HttpClientHandler
                {
                    UseCookies = true,
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                    ServerCertificateCustomValidationCallback = (req, cert, chain, errors) => true
                }
            }.WithCache();
            //_restClient = new JsonServiceClient(_apiUrl) 
            //{
            //    //HttpMessageHandler = new HttpClientHandler
            //    //{
            //    //    UseCookies = true,
            //    //    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            //    //    ServerCertificateCustomValidationCallback = (req, cert, chain, errors) => true
            //    //},
            //    OnAuthenticationRequired = ClientAuth, AlwaysSendBasicAuthHeader = true,
               
            //}.WithCache();
            


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

        public async Task<SystemUser> GetSystemUserAsync(UserLoginModel userLogin)
        {
            SystemUser user;
            var req = $"Users/appAuth";
            var res = await _restClient.PostAsync<string>(req, userLogin);
            
            user = res.FromJson<SystemUser>();
            if(user.Id == 0)
            {
                user.Response = res.FromJson<RequestResponseDto>();
            }
            
            return user;
        }

        public async Task<List<Warhouse>> GetUserWarhousesAsync(string company, long userId)
        {
            var req = $"Warhouses/{company}/{userId}";
            var res = await _restClient.GetAsync<List<Warhouse>>(req);
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

        public async Task<List<TaskLotSerial>> GetTransactionItems(TransactionItemsRequest request)
        {
            var req = $"Tasks/TransactionItems";
            var res = await _restClient.PostAsync<List<TaskLotSerial>>(req, request);
            return res;
        }

        public async Task<bool> AddTaskLots(AddTaskLotsRequest request)
        {
            var req = $"Tasks/AddTaskLots";
            var res = await _restClient.PostAsync<bool>(req, request);
            return res;
        }

        public async Task<long> AddTaskLot(AddTaskLotsRequest request)
        {
            var req = $"Tasks/AddTaskLot";
            var res = await _restClient.PostAsync<long>(req, request);
            return res;
        }

        public async Task<List<string>> GetBins(string company, long warhouseId)
        {
            var req = $"Tasks/Bins/{company}/{warhouseId}";
            var res = await _restClient.GetAsync<List<string>>(req);
            return res;
        }

        public async Task<int> DeleteTaskLots(string company, long taskId)
        {
            var req = $"Tasks/DeleteTaskLots/{company}/{taskId}";
            var res = await _restClient.DeleteAsync<int>(req);
            return res;
        }

        public  async Task<int> ActivateTask(ActivateTaskRequest request)
        {
            var req = $"Tasks/ActivateTask";
            var res = await _restClient.PostAsync<int>(req, request);
            return res;
        }

        public async Task<List<SerialModel>> GetSerials(SerialsRequest serialsRequest)
        {
            var req = $"Tasks/Serials";
            var res = await _restClient.PostAsync<List<SerialModel>>(req, serialsRequest);
            return res;
        }
        public async Task<List<SerialModel>> GetSelectedSerials(string company, long iTaslLot)
        {
            var req = $"Tasks/Serials/Selected/{company}/{iTaslLot}";
            var res = await _restClient.GetAsync<List<SerialModel>>(req);
            return res;
        }

        public async Task<List<ITaskLotModel>> GetOpenedTaskLots(string company, long TaskId)
        {
            var req = $"Tasks/OpenedTaskLots/{company}/{TaskId}";
            var res = await _restClient.GetAsync<List<ITaskLotModel>>(req);
            return res;
        }

        public async Task<bool> DeleteOpenedTaskLotSerials(string company, long taskLot)
        {
            var req = $"Tasks/DeleteOpenTaskSerials/{company}/{taskLot}";
            var res = await _restClient.DeleteAsync<bool>(req);
            return res;
        }
    }
}
