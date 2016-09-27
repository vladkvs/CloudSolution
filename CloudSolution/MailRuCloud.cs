using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;

namespace CloudSolution
{
    public class MailRuCloud : ICloud
    {
        private string Domain = "mail.ru";
        private string CloudDomain = "https://cloud.mail.ru";
        private string AuthDomen = "https://auth.mail.ru";

        private HttpClient httpClient;
        private string token;

        public string Login { get; set; }
        public string Password { get; set; }

        public MailRuCloud(string login, string password)
        {
            Login = login;
            Password = password;

            var handler = new HttpClientHandler()
            {
                CookieContainer = new CookieContainer(),
                UseCookies = true
            };
            httpClient = new HttpClient(handler);
        }

        public string GetServiceToken()
        {

            string reqString = $"Login={Login}&Domain={Domain}&Password={this.Password}";
            string address = $"{AuthDomen}/cgi-bin/auth";

            var response = httpClient.PostAsync(address, new StringContent(reqString)).Result;
            if (response.IsSuccessStatusCode)
            {
                address = $"{AuthDomen}/sdc?from={CloudDomain}/home";
                response = httpClient.GetAsync(address).Result;
                if (response.IsSuccessStatusCode)
                {
                    address = $"{CloudDomain}/api/v2/tokens/csrf";
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.UserAgent.Clear();
                    httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Mozilla", "5.0"));
                    response = httpClient.GetAsync(address).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var result = response.Content.ReadAsStringAsync().Result;
                        dynamic jsonObj = JObject.Parse(result);
                        this.token = jsonObj.body.token;
                        return result;
                    }
                }
            }
            return null;
        }

        public string GetFolderList(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = "/";
            }

            var uri =
                new Uri($"{CloudDomain}/api/v2/folder?token={token}&home={HttpUtility.UrlEncode(path)}");
            httpClient.BaseAddress = uri;

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.UserAgent.Clear();
            httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Mozilla", "5.0"));
            var response = httpClient.GetAsync("").Result;
            if (response.IsSuccessStatusCode)
            {

            }
            var resul = response.Content.ReadAsStringAsync().Result;

            return null;
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
