using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CloudSolution
{
    public class FourSharedCloud : ICloud
    {
        private readonly HttpClient httpClient;
        private readonly string oauthToken;
        private readonly string oauthTokenSecret;

        public FourSharedCloud()
        {
            var handler = new HttpClientHandler()
            {
                CookieContainer = new CookieContainer(),
                Proxy = new WebProxy(),
                UseCookies = true,
                UseProxy = true,
                AllowAutoRedirect = false
            };
            httpClient = new HttpClient(handler);

            var oAuthBase = new OAuthBase();
            string nonce = oAuthBase.GenerateNonce();
            string timestamp = oAuthBase.GenerateTimeStamp();

            string normalizedUrl;
            string normalizedUrlRequest;

            var result = oAuthBase.GenerateSignature(new Uri("https://api.4shared.com/v1_2/oauth/initiate"),
                "",
                "", null, null, "POST", timestamp, nonce, out normalizedUrl,
                out normalizedUrlRequest);

            var response =
                httpClient.PostAsync(normalizedUrl + "?" + normalizedUrlRequest + "&oauth_signature=" + result
               , null).Result;
            var message = response.Content.ReadAsStringAsync().Result;

            oauthToken = message.Split('&')[0];
            oauthTokenSecret = message.Split('&')[1];
            
        }


        public Task<string> GetServiceToken()
        {
            throw new NotImplementedException();
        }

        public string GetUser()
        {
            string address = "https://api.4shared.com/v1_2/user";

            var oauthBase = new OAuthBase();
            string nonce = oauthBase.GenerateNonce();
            string timestamp = oauthBase.GenerateTimeStamp();

            string normalizedUrl;
            string normalizedUrlRequest;

            string signature = oauthBase.GenerateSignature(new Uri(address), "",
                "", oauthToken, oauthTokenSecret, "GET", timestamp, nonce, out normalizedUrl,
                out normalizedUrlRequest);

            var response =
                httpClient.GetAsync(normalizedUrl + "?" + normalizedUrlRequest + "&oauth_signature=" + signature + 
                "&" + oauthToken + "&" + oauthTokenSecret).Result;
            var message = response.Content.ReadAsStringAsync().Result;

            return message;
        }

        public string GetFolderList(string path)
        {
            throw new NotImplementedException();
        }

        public string GetFileList(string path)
        {
            throw new NotImplementedException();
        }

        public Stream DownloadFile(string sourceFile)
        {
            throw new NotImplementedException();
        }

        public string DownloadFile(string sourceFile, string destinationFile)
        {
            throw new NotImplementedException();
        }

        public string UploadFile(Stream sourceDataStream, string destFileName)
        {
            throw new NotImplementedException();
        }

        public string UploadFile(string sourceFile, string destinationFile)
        {
            throw new NotImplementedException();
        }

        public string CreateFolder(string path)
        {
            throw new NotImplementedException();
        }

        public string Delete(string path)
        {
            throw new NotImplementedException();
        }
    }
}
