using System;
using System.Net;
using System.Net.Http;
using WebDAVClient.Interfaces;

namespace WebDAVClient
{
    public class Client : IClient
    {
        #region WebDAV connection parameters
        private String _server;
        public readonly HttpClient _client;
        public readonly HttpClient _uploadClient;

        public HttpClient UploadClient
        {
            get { return _uploadClient; }
        }
        
        public HttpClient NonUploadClient
        {
            get { return _client; }
        }

        public String Server
        {
            get { return _server; }
            set
            {
                value = value.TrimEnd('/');
                _server = value;
            }
        }
        private String _basePath = "/";

        public String BasePath
        {
            get { return _basePath; }
            set
            {
                value = value.Trim('/');
                if (string.IsNullOrEmpty(value))
                    _basePath = "/";
                else
                    _basePath = "/" + value + "/";
            }
        }
        
        public int? Port { get; set; }
        #endregion

        public Client(NetworkCredential credential, TimeSpan? uploadTimeout = null)
        {
            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
                handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            if (credential != null)
            {
                handler.Credentials = credential;
                handler.PreAuthenticate = true;
            }

            _client = new HttpClient(handler);
            _client.DefaultRequestHeaders.ExpectContinue = false;

            if (uploadTimeout != null)
            {
                _uploadClient = new HttpClient(handler);
                _uploadClient.DefaultRequestHeaders.ExpectContinue = false;
                _uploadClient.Timeout = uploadTimeout.Value;
            }
        }
      
    }
}
