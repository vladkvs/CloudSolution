using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Cloud.Core.Models;
using Newtonsoft.Json.Linq;

namespace Cloud.Core.MailRu
{
    public class MailRuCloud : Logger, ICloud
    {
        private string Domain = "mail.ru";
        private string CloudDomain = "https://cloud.mail.ru";
        private string AuthDomen = "https://auth.mail.ru";

        private HttpClient _httpClient;
        private string _token;

        public string Login { get; set; }
        public string Password { get; set; }

        public MailRuCloud()
        {
            Login = ConfigurationManager.AppSettings["MailRuLogin"];
            Password = ConfigurationManager.AppSettings["MailRuPassword"];
        }

        public MailRuCloud(string login, string password)
        {
            Login = login;
            Password = password;
        }

        private async Task<string> DoLogin()
        {
            if (_httpClient == null)
            {
                var handler = new HttpClientHandler()
                {
                    CookieContainer = new CookieContainer(),
                    UseCookies = true
                };
                _httpClient = new HttpClient(handler);
                _httpClient.DefaultRequestHeaders.UserAgent.Clear();
                _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Mozilla", "5.0"));
            }

            string reqString = $"Login={Login}&Domain={Domain}&Password={this.Password}";
            string address = $"{AuthDomen}/cgi-bin/auth";

            var response = await _httpClient.PostAsync(address, new StringContent(reqString));
            if (response.IsSuccessStatusCode)
            {
                address = $"{AuthDomen}/sdc?from={CloudDomain}/home";
                response = await _httpClient.GetAsync(address);
                if (response.IsSuccessStatusCode)
                {
                    await CheckAuth();
                    return _token;
                }
            }

            return string.Empty;
        }

        private async Task CheckAuth()
        {
            string address = $"{CloudDomain}/api/v2/tokens/csrf";
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await _httpClient.GetAsync(address);
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                dynamic jsonObj = JObject.Parse(result);
                this._token = jsonObj.body.token;
            }
        } 

        public async Task<string> GetServiceToken()
        {
            return await DoLogin();
        }

        public IEnumerable<CloudFolder> GetFolderList(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = "/";
            }

            var uri =
                new Uri($"{CloudDomain}/api/v2/folder?token={_token}&home={HttpUtility.UrlEncode(path)}");

            try
            {
                var response = _httpClient.GetAsync(uri).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Can't get folder list", ex);
            }

            return null;
        }

        public IEnumerable<CloudFile> GetFileList(string path)
        {
            return null;
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
            string shardUrl = GetUrlForAction(ShardType.Upload).Result;

            string url = $"{shardUrl}?cloud_domain=2&{Login}";
            try
            {
                FileStream file = File.Open(sourceFile, FileMode.Open);
                var content = new MultipartFormDataContent {new StreamContent(file)};

                var response = _httpClient.PostAsync(url, content).Result;
                var result = response.Content.ReadAsStringAsync().Result;

                return string.Empty;
            }
            catch (Exception ex)
            {
                Log.Error("Can't upload file", ex);
                return null;
            }
        }

        public string CreateFolder(string path)
        {
            string reqString =$"home={HttpUtility.UrlEncode(path)}&conflict=rename&api={2}&token={_token}";
            
            var url = new Uri($"{CloudDomain}/api/v2/folder/add");
            try
            {
                var response = _httpClient.PostAsync(url, new StringContent(reqString)).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    return string.Empty;
                }

                return response.StatusCode.ToString();
            }
            catch (Exception ex)
            {
                Log.Error("Can't create folder", ex);
                return null;
            }
        }

        public string Delete(string path)
        {
            throw new NotImplementedException();
        }

        private async Task<string> GetUrlForAction(ShardType shardType)
        {
            await this.CheckAuth();

            string uri = $"{CloudDomain}/api/v2/dispatcher?token={_token}";

            var response = await _httpClient.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                dynamic jObj = JObject.Parse(result);
                var memberInfo = typeof(ShardType).GetMember(shardType.ToString());
                var attributes = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                var description = ((DescriptionAttribute)attributes[0]).Description;

                foreach (var item in jObj.body)
                {
                    var jProperty = item as JProperty;
                    if (jProperty != null && jProperty.Name == description)
                    {
                        return jProperty.Value.Last.Value<string>("url");
                    }
                }
            }

            return null;
        }
    }
}
