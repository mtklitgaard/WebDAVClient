using System.Linq;
using FileUploader.Interfaces;
using WebDAVClient.Interfaces;

namespace FileUploader
{
    public class FileCreator : IFileCreator
    {
        private readonly IWebDAVOperator _webDavOperator;
        private readonly IDirectoryUtilityWrapper _directoryUtilityWrapper;

        public FileCreator(IWebDAVOperator webDavOperator, IDirectoryUtilityWrapper directoryUtilityWrapper)
        {
            _webDavOperator = webDavOperator;
            _directoryUtilityWrapper = directoryUtilityWrapper;
        }

        public void Upload(string directory, string uploadFolder)
        {
            var files = _directoryUtilityWrapper.GetAllFiles(directory);

            foreach (var file in files)
            {
                var fileStream = _directoryUtilityWrapper.GetFileStream(file);
                var fileName = file.Split('\\').Last();

                _webDavOperator.Upload(uploadFolder + "\\", fileStream, fileName);
            }
        }
    }
}