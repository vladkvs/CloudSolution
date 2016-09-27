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
        private AmazonS3Client httpClient;

        public AmazonCloud(string accessKey, string secretKey, RegionEndpoint endpoint)
        {
            httpClient = new AmazonS3Client(new BasicAWSCredentials(accessKey, secretKey), endpoint);
        }

        public Task<string> GetServiceToken()
        {
            ListObjectsRequest request = new ListObjectsRequest()
            {
                BucketName = "master7246",
                MaxKeys = 2
            };

            var objs = httpClient.ListObjects("master7246");

            return null;
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
