using System.Collections.Generic;

namespace FileUploader.Interfaces
{
    public interface IDirectoryWrapper
    {
        List<string> GetSubDirectoriesAndFiles(string directoryPath);
        List<string> GetDirectories(string directoryPath);
    }
}