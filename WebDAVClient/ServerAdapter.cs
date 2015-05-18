using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebDAVClient.Interfaces;

namespace WebDAVClient
{
    public class ServerAdapter : IServerAdapter
    {
        private readonly IClient _client;

        public ServerAdapter(IClient client)
        {
            _client = client;
        }

        public async Task<HttpResponseMessage> HttpRequest(Uri uri, HttpMethod method, IDictionary<string, string> headers = null, byte[] content = null)
        {
            using (var request = new HttpRequestMessage(method, uri))
            {
                if (headers != null)
                {
                    foreach (string key in headers.Keys)
                    {
                        request.Headers.Add(key, headers[key]);
                    }
                }

                if (content != null)
                {
                    request.Content = new ByteArrayContent(content);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");
                }

                return await _client.NonUploadClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            }
        }

     
        public async Task<HttpResponseMessage> HttpUploadRequest(Uri uri, HttpMethod method, Stream content, IDictionary<string, string> headers = null)
        {
            using (var request = new HttpRequestMessage(method, uri))
            {

                if (headers != null)
                {
                    foreach (string key in headers.Keys)
                    {
                        request.Headers.Add(key, headers[key]);
                    }
                }

                // Need to send along content?
                if (content != null)
                {
                    request.Content = new StreamContent(content);
                }

                var client = _client.UploadClient ?? _client.NonUploadClient;
                return await client.SendAsync(request).ConfigureAwait(false);
            }
        }

        public Uri GetServerUrl(String path, Boolean appendTrailingSlash)
        {
            string completePath = "";

            if (path != null)
            {
                if (!path.StartsWith(_client.BasePath))
                    completePath += _client.BasePath;
                if (!path.StartsWith(_client.Server, StringComparison.InvariantCultureIgnoreCase))
                {
                    completePath += path.Trim('/');
                }
                else
                {
                    completePath += path.Substring(_client.Server.Length + 1).Trim('/');
                }
            }
            else
            {
                completePath += _client.BasePath;
            }

            if (completePath.StartsWith("/") == false) { completePath = '/' + completePath; }
            if (appendTrailingSlash && completePath.EndsWith("/") == false) { completePath += '/'; }

            if (_client.Port.HasValue)
                return new Uri(_client.Server + ":" + _client.Port + completePath);

            return new Uri(_client.Server + completePath);
        }
    }
}
