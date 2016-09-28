using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.IO;
using Amazon.S3.Model;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using Microsoft.SqlServer.Server;

namespace CloudSolution 
{
    public class AmazonCloud : ICloud
    {
        private readonly string _accessKey;
        private readonly string _secretKey;
        private readonly string _bucketName;
        private readonly RegionEndpoint _endpoint;
        private AmazonS3Client _httpClient;

        public AmazonCloud(string accessKey, string secretKey, RegionEndpoint endpoint)
        {
            _accessKey = accessKey;
            _secretKey = secretKey;
            _endpoint = endpoint;
            _bucketName = ConfigurationManager.AppSettings["Bucket"];
        }

        private Task<string> DoLogin()
        {
            return Task.Run(() =>
            {
                var credentials = new BasicAWSCredentials(_accessKey, _secretKey);
                _httpClient = new AmazonS3Client(new BasicAWSCredentials(_accessKey, _secretKey), _endpoint);
                return credentials.GetCredentials().Token;
            });
        } 

        public async Task<string> GetServiceToken()
        {
            return await DoLogin();
        }

        public string GetFolderList(string path)
        {
            var request = new ListObjectsV2Request()
            {
                BucketName = _bucketName,
                Prefix = path
            };

            var response = _httpClient.ListObjectsV2(request);

            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                var folders = response.S3Objects.Where(x => x.Size == 0 && x.Key.EndsWith(@"/")).ToList();
                string folderString = string.Empty;
                folders.ForEach(f => folderString += f.Key + "\n");
                return folderString;
            }

            return string.Empty;
        }

        public string GetFileList(string path)
        {
            var response = _httpClient.ListObjectsV2(new ListObjectsV2Request()
            {
                BucketName = _bucketName
            });

            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                var files  = response.S3Objects.Where(x => x.Size > 0 && !x.Key.Contains(@"/")).ToList();
                string filesString = string.Empty;
                files.ForEach(f => filesString += f.Key + "\r" + f.Size + "\n");
                return filesString;
            }

            return string.Empty;
        }

        public Stream DownloadFile(string sourceFile)
        {
            var request = new GetObjectRequest()
            {
                BucketName = _bucketName,
                Key = sourceFile
            };
            var response = _httpClient.GetObject(request);

            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                return response.ResponseStream;
            }

            return null;
        }

        public string DownloadFile(string sourceFile, string destinationFile)
        {
            var stream = this.DownloadFile(sourceFile);
            if (stream == null)
            {
                return null;
            }

            try
            {
                var fileStream = File.Create(destinationFile);
                stream.CopyTo(fileStream);
                fileStream.Close();

                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string UploadFile(Stream sourceDataStream, string destFileName)
        {
            var response = _httpClient.PutObject(new PutObjectRequest()
            {
                BucketName = _bucketName,
                InputStream = sourceDataStream,
                Key = destFileName
            });

            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                return string.Empty;
            }

            return response.HttpStatusCode.ToString();
        }

        public string UploadFile(string sourceFile, string destinationFile)
        {
            var response = _httpClient.PutObject(new PutObjectRequest()
            {
                BucketName = _bucketName,
                FilePath = sourceFile,
                Key = destinationFile
            });

            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                return string.Empty;
            }

            return response.HttpStatusCode.ToString();
        }

        public string CreateFolder(string path)
        {
            var putRequest = new PutObjectRequest()
            {
                BucketName = _bucketName,
                StorageClass = S3StorageClass.Standard,
                ServerSideEncryptionMethod = ServerSideEncryptionMethod.None,
                Key = path + "/",
                ContentBody = string.Empty
            };
            var response = _httpClient.PutObject(putRequest);

            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                return string.Empty;
            }

            return response.HttpStatusCode.ToString();
        }

        public string Delete(string path)
        {
            var deleteRequest = new DeleteObjectRequest()
            {
                BucketName = _bucketName,
                Key = path
            };

            try
            {
                var response = _httpClient.DeleteObject(deleteRequest);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
