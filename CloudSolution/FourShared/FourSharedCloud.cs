using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CloudSolution.FourShared;

namespace CloudSolution
{
    public class FourSharedCloud : ICloud
    {
        private readonly string _consumerKey;
        private readonly string _consumerSecretKey;
        private readonly FourSharedConsumer _consumer;
        private string _accessToken;

        public FourSharedCloud(string consumerKey, string consumerSecretKey)
        {
            _consumerKey = consumerKey;
            _consumerSecretKey = consumerSecretKey;
            _consumer = new FourSharedConsumer(consumerKey, consumerSecretKey);
        }


        public Task<string> GetServiceToken()
        {
            return Task.Run(() =>
            {
                var url = _consumer.BeginAuth();

                var process = Process.Start(url.AbsoluteUri);
                process?.WaitForExit();

                string verifier = _consumer.TokenManager.GetTokenSecret(url.Query.Split('=')[1]);
                _accessToken = _consumer.CompleteAuth(verifier);

                return _accessToken;
            });
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
