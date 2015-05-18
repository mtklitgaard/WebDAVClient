using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebDAVClient.Helpers;
using WebDAVClient.Interfaces;
using WebDAVClient.Model;

namespace WebDAVClient
{
    public class WebDAVOperator
    {
        private readonly IServerAdapter _serverAdapter;

        private static readonly HttpMethod PropFind = new HttpMethod("PROPFIND");
        private static readonly HttpMethod MoveMethod = new HttpMethod("MOVE");

        private static readonly HttpMethod MkCol = new HttpMethod(WebRequestMethods.Http.MkCol);

        private const int HttpStatusCode_MultiStatus = 207;

        private const string PropFindRequestContent =
           "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" +
           "<propfind xmlns=\"DAV:\">" +
           "<allprop/>" +
           "</propfind>";

        public WebDAVOperator(IServerAdapter serverAdapter)
        {
            _serverAdapter = serverAdapter;
        }

        /// <summary>
        /// List all files present on the server.
        /// </summary>
        /// <param name="path">List only files in this path</param>
        /// <param name="depth">Recursion depth</param>
        /// <returns>A list of files (entries without a trailing slash) and directories (entries with a trailing slash)</returns>
        public async Task<IEnumerable<Item>> List(string path = "/", int? depth = 1)
        {
            Uri listUri = _serverAdapter.GetServerUrl(path, true);


            // Depth header: http://webdav.org/specs/rfc4918.html#rfc.section.9.1.4
            IDictionary<string, string> headers = new Dictionary<string, string>();
            if (depth != null)
            {
                headers.Add("Depth", depth.ToString());
            }


            HttpResponseMessage response = null;

            try
            {
                response = await _serverAdapter.HttpRequest(listUri, PropFind, headers, Encoding.UTF8.GetBytes(PropFindRequestContent)).ConfigureAwait(false);

                if (response.StatusCode != HttpStatusCode.OK &&
                    (int)response.StatusCode != HttpStatusCode_MultiStatus)
                {
                    throw new WebDAVException((int)response.StatusCode, "Failed retrieving items in folder.");
                }

                using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    var result = ResponseParser.ParseItems(path, stream);

                    if (result == null)
                    {
                        throw new WebDAVException("Failed deserializing data returned from server.");
                    }

                    var listUrl = listUri.ToString();
                    var listPath = listUri.PathAndQuery;
                    var listPathTrimmed = listPath.TrimEnd('/');
                    var listPathDecoded = HttpUtility.UrlDecode(listUri.PathAndQuery);
                    var listPathDecodedTrimmed = listPathDecoded.TrimEnd('/');

                    return result
                        .Where(r => !string.Equals(r.Href, listUrl, StringComparison.CurrentCultureIgnoreCase) &&
                                    !string.Equals(r.Href, listPath, StringComparison.CurrentCultureIgnoreCase) &&
                                    !string.Equals(r.Href, listPathTrimmed, StringComparison.CurrentCultureIgnoreCase) &&
                                    !string.Equals(r.Href, listPathDecoded, StringComparison.CurrentCultureIgnoreCase) &&
                                    !string.Equals(r.Href, listPathDecodedTrimmed, StringComparison.CurrentCultureIgnoreCase))
                        .ToList();
                }

            }
            finally
            {
                if (response != null)
                    response.Dispose();
            }
        }

        /// <summary>
        /// List all files present on the server.
        /// </summary>
        /// <returns>A list of files (entries without a trailing slash) and directories (entries with a trailing slash)</returns>
        public async Task<Item> GetFolder(string path = "/")
        {
            Uri listUri = _serverAdapter.GetServerUrl(path, false);
            return await Get(listUri, path);
        }

        /// <summary>
        /// List all files present on the server.
        /// </summary>
        /// <returns>A list of files (entries without a trailing slash) and directories (entries with a trailing slash)</returns>
        public async Task<Item> GetFile(string path = "/")
        {
            Uri listUri = _serverAdapter.GetServerUrl(path, false);
            return await Get(listUri, path);
        }


        /// <summary>
        /// List all files present on the server.
        /// </summary>
        /// <returns>A list of files (entries without a trailing slash) and directories (entries with a trailing slash)</returns>
        private async Task<Item> Get(Uri listUri, string path)
        {

            // Depth header: http://webdav.org/specs/rfc4918.html#rfc.section.9.1.4
            IDictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Depth", "0");


            HttpResponseMessage response = null;

            try
            {
                response = await _serverAdapter.HttpRequest(listUri, PropFind, headers, Encoding.UTF8.GetBytes(PropFindRequestContent)).ConfigureAwait(false);

                if (response.StatusCode != HttpStatusCode.OK &&
                    (int)response.StatusCode != HttpStatusCode_MultiStatus)
                {
                    throw new WebDAVException((int)response.StatusCode, string.Format("Failed retrieving item/folder (Status Code: {0})", response.StatusCode));
                }

                using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    var result = ResponseParser.ParseItem(path, stream);

                    if (result == null)
                    {
                        throw new WebDAVException("Failed deserializing data returned from server.");
                    }

                    return result;
                }
            }
            finally
            {
                if (response != null)
                    response.Dispose();
            }
        }

        /// <summary>
        /// Download a file from the server
        /// </summary>
        /// <param name="remoteFilePath">Source path and filename of the file on the server</param>
        public async Task<Stream> Download(string remoteFilePath)
        {
            // Should not have a trailing slash.
            Uri downloadUri = _serverAdapter.GetServerUrl(remoteFilePath, false);

            var response = await _serverAdapter.HttpRequest(downloadUri, HttpMethod.Get).ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new WebDAVException((int)response.StatusCode, "Failed retrieving file.");
            }
            return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Download a file from the server
        /// </summary>
        /// <param name="remoteFilePath">Source path and filename of the file on the server</param>
        /// <param name="content"></param>
        /// <param name="name"></param>
        public async Task<bool> Upload(string remoteFilePath, Stream content, string name)
        {
            // Should not have a trailing slash.
            Uri uploadUri = _serverAdapter.GetServerUrl(remoteFilePath + name, false);

            HttpResponseMessage response = null;

            try
            {
                response = await _serverAdapter.HttpUploadRequest(uploadUri, HttpMethod.Put, content).ConfigureAwait(false);

                if (response.StatusCode != HttpStatusCode.OK &&
                    response.StatusCode != HttpStatusCode.NoContent &&
                    response.StatusCode != HttpStatusCode.Created)
                {
                    throw new WebDAVException((int)response.StatusCode, "Failed uploading file.");
                }

                return response.IsSuccessStatusCode;
            }
            finally
            {
                if (response != null)
                    response.Dispose();
            }

        }


        /// <summary>
        /// Create a directory on the server
        /// </summary>
        /// <param name="remotePath">Destination path of the directory on the server</param>
        /// <param name="name"></param>
        public async Task<bool> CreateDir(string remotePath, string name)
        {
            // Should not have a trailing slash.
            Uri dirUri = _serverAdapter.GetServerUrl(remotePath + name, false);

            HttpResponseMessage response = null;

            try
            {
                response = await _serverAdapter.HttpRequest(dirUri, MkCol).ConfigureAwait(false);

                if (response.StatusCode == HttpStatusCode.Conflict)
                    throw new WebDAVConflictException((int)response.StatusCode, "Failed creating folder.");

                if (response.StatusCode != HttpStatusCode.OK &&
                    response.StatusCode != HttpStatusCode.NoContent &&
                    response.StatusCode != HttpStatusCode.Created)
                {
                    throw new WebDAVException((int)response.StatusCode, "Failed creating folder.");
                }

                return response.IsSuccessStatusCode;
            }
            finally
            {
                if (response != null)
                    response.Dispose();
            }
        }

        public async Task DeleteFolder(string href)
        {
            Uri listUri = _serverAdapter.GetServerUrl(href, true);
            await Delete(listUri).ConfigureAwait(false);
        }

        public async Task DeleteFile(string href)
        {
            Uri listUri = _serverAdapter.GetServerUrl(href, false);
            await Delete(listUri).ConfigureAwait(false);
        }


        private async Task Delete(Uri listUri)
        {
            var response = await _serverAdapter.HttpRequest(listUri, HttpMethod.Delete).ConfigureAwait(false);

            if (response.StatusCode != HttpStatusCode.OK &&
                response.StatusCode != HttpStatusCode.NoContent)
            {
                throw new WebDAVException((int)response.StatusCode, "Failed deleting item.");
            }
        }

        public async Task<bool> MoveFolder(string srcFolderPath, string dstFolderPath)
        {
            // Should have a trailing slash.
            Uri srcUri = _serverAdapter.GetServerUrl(srcFolderPath, true);
            Uri dstUri = _serverAdapter.GetServerUrl(dstFolderPath, true);

            return await Move(srcUri, dstUri).ConfigureAwait(false);

        }

        public async Task<bool> MoveFile(string srcFilePath, string dstFilePath)
        {
            // Should not have a trailing slash.
            Uri srcUri = _serverAdapter.GetServerUrl(srcFilePath, false);
            Uri dstUri = _serverAdapter.GetServerUrl(dstFilePath, false);

            return await Move(srcUri, dstUri).ConfigureAwait(false);
        }


        private async Task<bool> Move(Uri srcUri, Uri dstUri)
        {
            const string requestContent = "MOVE";

            IDictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Destination", dstUri.ToString());

            var response = await _serverAdapter.HttpRequest(srcUri, MoveMethod, headers, Encoding.UTF8.GetBytes(requestContent)).ConfigureAwait(false);

            if (response.StatusCode != HttpStatusCode.OK &&
                response.StatusCode != HttpStatusCode.Created)
            {
                throw new WebDAVException((int)response.StatusCode, "Failed moving file.");
            }

            return response.IsSuccessStatusCode;
        }
    }
}
