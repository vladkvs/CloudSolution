using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;

namespace Cloud.Core.Amazon 
{
    public class AmazonCloud : ICloud
    {
        private readonly string _bucketName;
        private string _accessKey;
        private string _secretKey;
        private RegionEndpoint _endpoint;
        private AmazonS3Client _httpClient;

        public string AccessKey
        {
            get { return _accessKey; }
            set { _accessKey = value; }
        }

        public string SecretKey
        {
            get { return _secretKey; }
            set { _secretKey = value; }
        }

        public RegionEndpoint Endpoint
        {
            get { return _endpoint; }
            set { _endpoint = value; }
        }

        public AmazonCloud()
        {
            _accessKey = ConfigurationManager.AppSettings["AmazonAccessKey"];
            _secretKey = ConfigurationManager.AppSettings["AmazonSecretKey"];
            _endpoint = new AppConfigAWSRegion().Region;
            _bucketName = ConfigurationManager.AppSettings["Bucket"];
        }

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
                BucketName = _bucketName,
                Prefix = path
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
