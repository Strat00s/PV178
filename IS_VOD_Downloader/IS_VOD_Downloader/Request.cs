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


        public Request(Dictionary<string, string>? cookies = null)
        {
            _client = new HttpClient();
            if (cookies != null)
                SetCookies(cookies);
        }


        private static string CombineCookies(Dictionary<string, string> cookies)
        {
            return string.Join("; ", cookies.Select(cookie => $"{cookie.Key}={cookie.Value}"));
        }

        //public void AddCookie(string name, string value)
        //{
        //    if (_client.DefaultRequestHeaders.TryGetValues("Cookies", out IEnumerable<string>? cookiesRaw))
        //    {
        //        cookiesRaw += $"; {name}={value}";
        //    }
        //    else
        //    {
        //
        //    }
        //}

        public void ClearCookies()
        {
            _client.DefaultRequestHeaders.Remove("Cookie");
        }

        public void SetCookies(Dictionary<string, string> cookies)
        {
            ClearCookies();
            _client.DefaultRequestHeaders.Add("Cookie", CombineCookies(cookies));
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


        public async Task<HttpContent> SendRequestAsync(HttpMethod method, string url, HttpContent? requestBody = null, Dictionary<string, string>? headers = null)
        {
            using var request = new HttpRequestMessage(method, url);

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            if (requestBody != null)
            {
                request.Content = requestBody;
            }

            using var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStreamAsync();
            return new StreamContent(content);
        }
    }
}