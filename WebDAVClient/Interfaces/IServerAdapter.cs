using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebDAVClient.Interfaces
{
    public interface IServerAdapter
    {
        Task<HttpResponseMessage> HttpRequest(Uri uri, HttpMethod method, IDictionary<string, string> headers = null, byte[] content = null);
        Task<HttpResponseMessage> HttpUploadRequest(Uri uri, HttpMethod method, Stream content, IDictionary<string, string> headers = null);
        Uri GetServerUrl(String path, Boolean appendTrailingSlash);
    }
}