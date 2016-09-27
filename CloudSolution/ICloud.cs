using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSolution
{
    public interface ICloud
    {
        Task<string> GetServiceToken();
        string GetFolderList(string path);
        string GetFileList(string path);
        Stream DownloadFile(string sourceFile);
        string DownloadFile(string sourceFile, string destinationFile);
        string UploadFile(Stream sourceDataStream, string destFileName);
        string UploadFile(string sourceFile, string destinationFile);
        string CreateFolder(string path);
        string Delete(string path);
    }
}
