using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileUploader.Interfaces
{
    public class DirectoryWrapper : IDirectoryWrapper
    {
        public List<string> GetSubDirectoriesAndFiles(string directoryPath)
        {
            return Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories).ToList();
        }  
        
        public List<string> GetTopDirectories(string directoryPath)
        {
            return Directory.GetDirectories(directoryPath, "*.*", SearchOption.TopDirectoryOnly).ToList();
        }

        public List<string> GetAllFiles(string directoryPath)
        {
            return Directory.GetFiles(directoryPath).ToList();
        }

        public Stream GetFileStream(string fileLocation)
        {
            return File.OpenRead(fileLocation);
        }
    }
}