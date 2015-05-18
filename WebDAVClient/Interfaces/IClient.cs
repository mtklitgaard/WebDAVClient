using System;
using System.Net.Http;

namespace WebDAVClient.Interfaces
{
    public interface IClient
    {
        HttpClient UploadClient { get; }
        HttpClient NonUploadClient { get; }
        String Server { get; set; }
        String BasePath { get; set; }
        int? Port { get; set; }
    }
}