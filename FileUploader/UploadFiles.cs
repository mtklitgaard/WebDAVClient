using System;
using System.Collections.Generic;
using System.Linq;
using FileUploader.Interfaces;
using WebDAVClient.Interfaces;

namespace FileUploader
{
    public class UploadFiles
    {
        private readonly IDirectoryUtilityWrapper _directoryUtilityWrapper;
        private readonly IWebDAVOperator _webDavOperator;
        private readonly IFileCreator _fileCreator;

        public UploadFiles(IDirectoryUtilityWrapper directoryUtilityWrapper, IWebDAVOperator webDavOperator, IFileCreator fileCreator)
        {
            _directoryUtilityWrapper = directoryUtilityWrapper;
            _webDavOperator = webDavOperator;
            _fileCreator = fileCreator;
        }


        public async void Upload(string pathToRootDirectory)
        {
            var uploadRootDirectory = FindRootDirectory(pathToRootDirectory);
            var directories = _directoryUtilityWrapper.GetTopDirectories(pathToRootDirectory);
            var rootPath = "/";
            
            await _webDavOperator.CreateDir(rootPath, uploadRootDirectory);

            CreateDirectories(directories);
        }

        private async void CreateDirectories(List<string> directories)
        {
            foreach (var directory in directories)
            {
                var flattenedDirectories = FlattenDirectoryStructure(directory);
                var folderLocation = flattenedDirectories.Take(flattenedDirectories.Count - 1).ToList();
                var directoryLocation = string.Join("\\", folderLocation);

                await _webDavOperator.CreateDir(directoryLocation + "\\", flattenedDirectories.Last());

                var subDirectories = _directoryUtilityWrapper.GetTopDirectories(directory);
                if (subDirectories != null && subDirectories.Count > 0)
                {
                    CreateDirectories(subDirectories);
                }

                _fileCreator.Upload(directory, directoryLocation + "\\" + flattenedDirectories.Last());
            }
        }

        private List<string> FlattenDirectoryStructure(string directory)
        {
            var directories = directory.Split('\\').ToList();

            return directories.Where(x => !x.Contains(':')).ToList();
        }

        private string FindRootDirectory(string pathToRootDirectory)
        {
            var path = pathToRootDirectory.Split('\\').ToList();

            return path.First(x => !x.Contains(':'));
        }
    }
}
