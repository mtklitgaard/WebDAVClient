using System.Collections.Generic;
using System.IO;

namespace FileUploader.Interfaces
{
    public interface IDirectoryWrapper
    {
        List<string> GetSubDirectoriesAndFiles(string directoryPath);
        List<string> GetTopDirectories(string directoryPath);
        List<string> GetAllFiles(string directoryPath);
        Stream GetFileStream(string fileLocation);
    }
}