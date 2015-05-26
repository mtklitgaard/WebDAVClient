using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WebDAVClient.Model;

namespace WebDAVClient.Interfaces
{
    public interface IWebDAVOperator
    {
        /// <summary>
        /// List all files present on the server.
        /// </summary>
        /// <param name="path">List only files in this path</param>
        /// <param name="depth">Recursion depth</param>
        /// <returns>A list of files (entries without a trailing slash) and directories (entries with a trailing slash)</returns>
        Task<IEnumerable<Item>> List(string path = "/", int? depth = 1);

        /// <summary>
        /// List all files present on the server.
        /// </summary>
        /// <returns>A list of files (entries without a trailing slash) and directories (entries with a trailing slash)</returns>
        Task<Item> GetFolder(string path = "/");

        /// <summary>
        /// List all files present on the server.
        /// </summary>
        /// <returns>A list of files (entries without a trailing slash) and directories (entries with a trailing slash)</returns>
        Task<Item> GetFile(string path = "/");

        /// <summary>
        /// Download a file from the server
        /// </summary>
        /// <param name="remoteFilePath">Source path and filename of the file on the server</param>
        Task<Stream> Download(string remoteFilePath);

        /// <summary>
        /// Download a file from the server
        /// </summary>
        /// <param name="remoteFilePath">Source path and filename of the file on the server</param>
        /// <param name="content"></param>
        /// <param name="name"></param>
        Task<bool> Upload(string remoteFilePath, Stream content, string name);

        /// <summary>
        /// Create a directory on the server
        /// </summary>
        /// <param name="remotePath">Destination path of the directory on the server</param>
        /// <param name="name"></param>
        Task<bool> CreateDir(string remotePath, string name);

        Task DeleteFolder(string href);
        Task DeleteFile(string href);
        Task<bool> MoveFolder(string srcFolderPath, string dstFolderPath);
        Task<bool> MoveFile(string srcFilePath, string dstFilePath);
    }
}