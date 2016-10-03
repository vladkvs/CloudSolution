using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Cloud.Core.Models;

namespace Cloud.Core
{
    public interface ICloud
    {
        Task<string> GetServiceToken();
        IEnumerable<CloudFolder> GetFolderList(string path);
        IEnumerable<CloudFile> GetFileList(string path);
        Stream DownloadFile(string sourceFile);
        string DownloadFile(string sourceFile, string destinationFile);
        string UploadFile(Stream sourceDataStream, string destFileName);
        string UploadFile(string sourceFile, string destinationFile);
        string CreateFolder(string path);
        string Delete(string path);
    }
}
