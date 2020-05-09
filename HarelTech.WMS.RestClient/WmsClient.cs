using HarelTech.WMS.Common.Models;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Text;

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
            var token = $"{_apiUrl}/users/authenticate";

            //var response = token.PostToUrl($"username={_user}&password={_password}");
            var response = _restClient.Post<AuthUser>(token, new AuthenticateModel { UserName = _userName, Password = _password });


            if (response == null)
                throw new Exception("Rest client not authenticated with web api");
            _restClient.BearerToken = response.Token;
            //_restClient.AddHeader("Authorization", $"Bearer {response.FromJson<string>()}");
        }

        //public async Task<>
    }
}
