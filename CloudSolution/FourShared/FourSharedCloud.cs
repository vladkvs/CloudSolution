using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Cloud.Core.Models;
using DotNetOpenAuth.Messaging;
using Newtonsoft.Json.Linq;

namespace Cloud.Core.FourShared
{
    public class FourSharedCloud : Logger, ICloud
    {
        private readonly string _consumerKey;
        private readonly string _consumerSecretKey;
        private readonly FourSharedConsumer _consumer;
        private string _accessToken;

        public FourSharedCloud()
        {
            _consumerKey = ConfigurationManager.AppSettings["4SharedConsumerKey"];
            _consumerSecretKey = ConfigurationManager.AppSettings["4SharedConsumerSecretKey"];
            _consumer = new FourSharedConsumer(_consumerKey, _consumerSecretKey);
        }

        public FourSharedCloud(string consumerKey, string consumerSecretKey)
        {
            _consumerKey = consumerKey;
            _consumerSecretKey = consumerSecretKey;
            _consumer = new FourSharedConsumer(consumerKey, consumerSecretKey);
        }


        public Task<string> GetServiceToken()
        {
            Uri url = null;
            var firstTask = new Task(() =>
            {
                url = _consumer.BeginAuth();

                var process = Process.Start(url.AbsoluteUri);
                process?.WaitForExit();
            });
            var secondTask = firstTask.ContinueWith((t) =>
            {
                string verifier = _consumer.TokenManager.GetTokenSecret(url.Query.Split('=')[1]);
                _accessToken = _consumer.CompleteAuth(verifier);
                return _accessToken;
            });
            firstTask.Start();

            return secondTask;
        }

        public string GetUserInfo()
        {
            var request =
                _consumer.PrepareAuthorizedRequest(
                    new MessageReceivingEndpoint(
                        "https://api.4shared.com/v1_2/user",
                        HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest),
                    _accessToken, new List<MultipartPostPart>());

            var response = (HttpWebResponse)request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());

            return reader.ReadToEnd();
        }

        public IEnumerable<CloudFolder> GetFolderList(string path)
        {
            dynamic user = JObject.Parse(GetUserInfo());
            string rootFoolderId = user.rootFolderId;
            try
            {
                var request =
                    _consumer.PrepareAuthorizedRequest(
                        new MessageReceivingEndpoint(
                            "https://api.4shared.com/v1_2/folders/" + rootFoolderId,
                            HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest),
                        _accessToken,
                        new List<MultipartPostPart>());
                var response = (HttpWebResponse) request.GetResponse();
                var reader = new StreamReader(response.GetResponseStream());
                string objText = reader.ReadToEnd();
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
