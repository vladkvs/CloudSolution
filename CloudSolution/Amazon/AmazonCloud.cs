using System;
using System.Collections.Generic;
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
using Cloud.Core.Models;
using log4net;

namespace Cloud.Core.Amazon 
{
    public class AmazonCloud : Logger, ICloud, IDisposable
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

        public IEnumerable<CloudFolder> GetFolderList(string path)
        {
            List<CloudFolder> listFolders = new List<CloudFolder>();
            var prefix = path == string.Empty ? path : path.EndsWith("/") ? path : path + "/";

            var request = new ListObjectsRequest()
            {
                BucketName = _bucketName,
                Prefix = prefix
            };

            try
            {
                do
                {
                    var response = _httpClient.ListObjects(request);
                    if (response.HttpStatusCode != HttpStatusCode.OK)
                    {
                        return null;
                    }

                    listFolders.AddRange(from folder in response.S3Objects
                        where
                            folder.Key != prefix && folder.Size == 0 && folder.Key.EndsWith("/")
                            && prefix.Count(x => x == '/') == folder.Key.Count(x => x == '/') - 1
                        select new CloudFolder()
                        {
                            Name = string.IsNullOrEmpty(path) ? folder.Key : folder.Key.Remove(0, prefix.Length),
                            Path = prefix
                        });

                    request.Marker = response.IsTruncated ? response.NextMarker : null;
                } while (request.Marker != null);
            }
            catch (Exception ex)
            {
                Log.Error("Can't get folder list", ex);
            }

            return listFolders;
        }

        public IEnumerable<CloudFile> GetFileList(string path)
        {
            List<CloudFile> listFiles = new List<CloudFile>();
            var prefix = path == string.Empty ? path : path.EndsWith("/") ? path : path + "/";

            var request = new ListObjectsRequest()
            {
                BucketName = _bucketName,
                Prefix = prefix
            };

            try
            {
                do
                {
                    var response = _httpClient.ListObjects(request);
                    if (response.HttpStatusCode != HttpStatusCode.OK)
                    {
                        return null;
                    }

                    listFiles.AddRange(from file in response.S3Objects
                        where
                            file.Size > 0 && !file.Key.EndsWith("/")
                            && file.Key.Count(x => x == '/') == prefix.Count(x => x == '/')
                        select new CloudFile()
                        {
                            Name = string.IsNullOrEmpty(path) ? file.Key : file.Key.Remove(0, prefix.Length),
                            Path = prefix,
                            Size = file.Size,
                            LastModified = file.LastModified
                        });

                    request.Marker = response.IsTruncated ? response.NextMarker : null;
                } while (request.Marker != null);
            }
            catch (Exception ex)
            {
                Log.Error("Can't get file list", ex);
            }

            return listFiles;
        }

        public Stream DownloadFile(string sourceFile)
        {
            var request = new GetObjectRequest()
            {
                BucketName = _bucketName,
                Key = sourceFile
            };
            try
            {
                var response = _httpClient.GetObject(request);

                if (response.HttpStatusCode == HttpStatusCode.OK)
                {
                    return response.ResponseStream;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Can't download file", ex);
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
                Log.Error("Can't create file", ex);
            }
            return null;
        }

        public string UploadFile(Stream sourceDataStream, string destFileName)
        {
            var request = new PutObjectRequest()
            {
                BucketName = _bucketName,
                InputStream = sourceDataStream,
                Key = destFileName
            };

            try
            {
                var response = _httpClient.PutObject(request);

                if (response.HttpStatusCode == HttpStatusCode.OK)
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Can't upload file", ex);
            }

            return null;
        }

        public string UploadFile(string sourceFile, string destinationFile)
        {
            var request = new PutObjectRequest()
            {
                BucketName = _bucketName,
                FilePath = sourceFile,
                Key = destinationFile
            };

            try
            {
                var response = _httpClient.PutObject(request);

                if (response.HttpStatusCode == HttpStatusCode.OK)
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Can't upload file", ex);
            }

            return null;
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

            try
            {
                var response = _httpClient.PutObject(putRequest);

                if (response.HttpStatusCode == HttpStatusCode.OK)
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Can't create folder", ex);
            }

            return null;
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
                Log.Error("Can't delete file", ex);
            }
            return null;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
