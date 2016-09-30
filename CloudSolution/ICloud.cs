using System.IO;
using System.Threading.Tasks;

namespace Cloud.Core
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
