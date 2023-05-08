using System;
using System.ComponentModel.Design;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ISVOD
{
    public class Request
    {
        private HttpClient _client;


        public Request()
        {
            _client = new HttpClient();
        }

        public void ClearCookies()
        {
            _client.DefaultRequestHeaders.Remove("Cookie");
        }

        public void SetCookies(string iscreds, string issession)
        {
            ClearCookies();
            _client.DefaultRequestHeaders.Add("Cookie", $"iscreds={iscreds}; issession={issession}");
        }


        private HttpRequestMessage GenerateRequest(HttpMethod method, string url, Dictionary<string, string>? headers)
        {
            var request = new HttpRequestMessage(method, url);
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            return request;
        }

        private async Task<HttpContent> SendAsync(HttpRequestMessage request)
        {
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return response.Content;
        }

        public Task<HttpContent> GetAsync(string url, Dictionary<string, string>? headers = null)
        {
            var request = GenerateRequest(HttpMethod.Get, url, headers);
            return SendAsync(request);
        }

        public Task<HttpContent> PostAsync(string url, HttpContent requestBody, Dictionary<string, string>? headers = null)
        {
            var request = GenerateRequest(HttpMethod.Post, url, headers);
            request.Content = requestBody;
        
            return SendAsync(request);
        }
    }
}