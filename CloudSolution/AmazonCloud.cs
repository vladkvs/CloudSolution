using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.IO;
using Amazon.S3.Model;
using Microsoft.SqlServer.Server;

namespace CloudSolution 
{
    public class AmazonCloud : ICloud
    {
        private readonly string _accessKey;
        private readonly string _secretKey;
        private readonly RegionEndpoint _endpoint;
        private AmazonS3Client httpClient;

        public AmazonCloud(string accessKey, string secretKey, RegionEndpoint endpoint)
        {
            _accessKey = accessKey;
            _secretKey = secretKey;
            _endpoint = endpoint;
        }

        private Task DoLogin()
        {
            return Task.Run(() =>
            {
                httpClient = new AmazonS3Client(new BasicAWSCredentials(_accessKey, _secretKey),
                    _endpoint);
            });
        } 

        public async Task<string> GetServiceToken()
        {
            await DoLogin();

            return string.Empty;
        }

        public string GetFolderList(string path)
        {

            return null;
        }

        public string GetFileList(string path)
        {
            var response = httpClient.ListObjectsV2(new ListObjectsV2Request()
            {
                BucketName = "master7246"
            });

            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                var files  = response.S3Objects.Where(x => x.Size > 0 && !x.Key.Contains(@"/")).ToList();
                string filesString = string.Empty;
                files.ForEach(f => filesString += f.Key + "\r" + f.Size + "\n");
                return filesString;
            }

            return null;
        }

        public Stream DownloadFile(string sourceFile)
        {
            var request = new GetObjectRequest()
            {
                BucketName = "master7246",
                Key = sourceFile
            };
            var response = httpClient.GetObject(request);

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
            var response = httpClient.PutObject(new PutObjectRequest()
            {
                BucketName = "master7246",
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
            var response = httpClient.PutObject(new PutObjectRequest()
            {
                BucketName = "master7246",
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
                BucketName = "master7246",
                StorageClass = S3StorageClass.Standard,
                ServerSideEncryptionMethod = ServerSideEncryptionMethod.None,
                Key = path + "/",
                ContentBody = string.Empty
            };
            var response = httpClient.PutObject(putRequest);

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
                BucketName = "master7246",
                Key = path
            };

            try
            {
                var response = httpClient.DeleteObject(deleteRequest);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
