using System.Net;
using FileUploader;
using FileUploader.Interfaces;
using NUnit.Framework;
using WebDAVClient;

namespace FileUploaderTests
{
    [TestFixture]
    public class FileUploadIntegrationTests
    {
        [Test, Category("Integration")]
        public async void UploadFiles()
        {
            var client = new Client(new NetworkCredential { UserName = "yourusername", Password = "yourpassword" });
            client.Server = "yourwebsite";
            client.BasePath = "basepathforwebdav";

            var serverAdapter = new ServerAdapter(client);
            var webDAVOperator = new WebDAVOperator(serverAdapter);
            
            var directoryUtilityWrapper = new DirectoryUtilityWrapper();
            var fileCreator = new FileCreator(webDAVOperator, directoryUtilityWrapper);
            var fileUploader = new UploadFiles(directoryUtilityWrapper, webDAVOperator, fileCreator);

            fileUploader.Upload("C:\\release-1.1.40");
        }
    }
}
