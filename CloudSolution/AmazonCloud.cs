using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace CloudSolution 
{
    public class AmazonCloud : ICloud
    {
        private AmazonS3Client httpClient;

        public AmazonCloud(string accessKey, string secretKey, RegionEndpoint endpoint)
        {
            httpClient = new AmazonS3Client(new BasicAWSCredentials(accessKey, secretKey), endpoint);
        }

        public string GetServiceToken()
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
