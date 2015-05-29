using System.Net;
using FileUploader.Interfaces;
using WebDAVClient;

namespace FileUploader
{
    public class CreateClientAndUpload
    {
        public void Upload(string server, string basePath, string userName, string password, string directory)
        {
            var client = new Client(new NetworkCredential { UserName = userName, Password = password });
            client.Server = server;
            client.BasePath = basePath;

            var serverAdapter = new ServerAdapter(client);
            var webDAVOperator = new WebDAVOperator(serverAdapter);

            var directoryUtilityWrapper = new DirectoryUtilityWrapper();
            var fileCreator = new FileCreator(webDAVOperator, directoryUtilityWrapper);
            var fileUploader = new UploadFiles(directoryUtilityWrapper, webDAVOperator, fileCreator);

            fileUploader.Upload(directory);
        }
    }
}
