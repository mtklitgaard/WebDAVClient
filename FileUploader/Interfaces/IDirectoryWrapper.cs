using System.Collections.Generic;

namespace FileUploader.Interfaces
{
    public interface IDirectoryWrapper
    {
        List<string> GetSubDirectoriesAndFiles(string directoryPath);
    }
}