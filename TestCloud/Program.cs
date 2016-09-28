using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using CloudSolution;
using Microsoft.SqlServer.Server;

namespace TestCloud
{
    class Program
    {
        static void Main(string[] args)
        {
            //MailRuCloud mailRu = new MailRuCloud("", "");
            //string token = mailRu.GetServiceToken().Result;
            //string createFolder = mailRu.CreateFolder("NewFolder/NewFolder2/NewFolder3");
            //string folderList = mailRu.GetFolderList("");
            //string uploadFile = mailRu.UploadFile("", "");

            //ICloud amazon = new AmazonCloud("", "", RegionEndpoint.EUCentral1);
            //string token = amazon.GetServiceToken().Result;
            //string result = amazon.CreateFolder("newfolder10/newfolder11/newfolder12");
            //string result =
            //    amazon.UploadFile(File.Open("D:/Downloads/1cd375df-55a7-4ba6-b604-a6820123f9a0.jpg", FileMode.Open),
            //        "newfolder10/newfolder11/test12.jpg");
            //result = amazon.Delete("newfolder10/newfolder11/");

            //var downloadFile = amazon.DownloadFile("newfolder10/newfolder11/test12.jpg", "D:/Downloads/test.jpg");
            //string result = amazon.GetFileList("");

            FourSharedCloud fourShared = new FourSharedCloud();
            var user = fourShared.GetUser();
        }
    }
}
