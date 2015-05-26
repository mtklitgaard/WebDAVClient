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
        
        public List<string> GetDirectories(string directoryPath)
        {
            return Directory.GetDirectories(directoryPath, "*.*", SearchOption.AllDirectories).ToList();
        }
    }
}