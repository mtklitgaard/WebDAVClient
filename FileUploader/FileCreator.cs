using System.Linq;
using FileUploader.Interfaces;
using WebDAVClient.Interfaces;

namespace FileUploader
{
    public class FileCreator : IFileCreator
    {
        private readonly IWebDAVOperator _webDavOperator;
        private readonly IDirectoryWrapper _directoryWrapper;

        public FileCreator(IWebDAVOperator webDavOperator, IDirectoryWrapper directoryWrapper)
        {
            _webDavOperator = webDavOperator;
            _directoryWrapper = directoryWrapper;
        }

        public void Upload(string directory, string uploadFolder)
        {
            var files = _directoryWrapper.GetAllFiles(directory);

            foreach (var file in files)
            {
                var fileStream = _directoryWrapper.GetFileStream(file);
                var fileName = file.Split('\\').Last();

                _webDavOperator.Upload(directory, fileStream, fileName);
            }
        }
    }
}