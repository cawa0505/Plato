﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Net.Abstractions;

namespace Plato.Internal.Net
{
    
    public class HttpClient : IHttpClient
    {

        private readonly ILogger<HttpClient> _logger;

        public int Timeout { get; set; } = 30;
        
        public HttpClient(ILogger<HttpClient> logger)
        {
            _logger = logger;
        }

        #region "Implementation"

        public async Task<HttpClientResponse> GetAsync(Uri url)
        {
            return await GetAsync(url, null);
        }

        public async Task<HttpClientResponse> GetAsync(Uri url, IDictionary<string, string> parameters)
        {
            return await RequestAsync(HttpMethod.Get, url, parameters);
        }

        public async Task<HttpClientResponse> PostAsync(Uri url)
        {
            return await PostAsync(url, null);
        }

        public async Task<HttpClientResponse> PostAsync(Uri url, IDictionary<string, string> parameters)
        {
            return await RequestAsync(HttpMethod.Post, url, parameters);
        }

        public async Task<HttpClientResponse> RequestAsync(
            HttpMethod method,
            Uri url,
            IDictionary<string, string> parameters)
        {
            return await RequestAsync(method, url, parameters, "application/x-www-form-urlencoded");
        }

        public async Task<HttpClientResponse> RequestAsync(
            HttpMethod method, 
            Uri url, 
            IDictionary<string, string> parameters,
            string contentType)
        {

            var result = new HttpClientResponse();

            var encoding = new UTF8Encoding();
            var data = string.Empty;
            if (parameters != null)
            {
                data = contentType.Equals("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase) ?
                    BuildParameterString(parameters) :
                    BuildParameterJsonString(parameters);
            }

            if (method == HttpMethod.Get)
            {
                url = new Uri(url.ToString() + "?" + data);
            }

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method.ToString().ToUpper();
            request.Timeout = Timeout * 1000;
            request.UserAgent = "Mozilla/5.0 (Windows NT x.y; rv:10.0) Gecko/20100101 Firefox/10.0";

            if (method == HttpMethod.Post)
            {

                request.ContentType = contentType;
                
                var byteData = encoding.GetBytes(data);
                request.ContentLength = byteData.Length;
                var stream = await request.GetRequestStreamAsync();
                stream.Write(byteData, 0, byteData.Length);
                stream.Close();
              
        
            }

            WebResponse response = null;
            StreamReader responseStream = null;
   
            try
            {

                response = await request.GetResponseAsync();
                var readStream = response.GetResponseStream();
                if (readStream != null)
                {
                    responseStream = new StreamReader(readStream, Encoding.UTF8);
                    result.Response = responseStream.ReadToEnd();
                }

                result.Succeeded = true;
                
            }
            catch (Exception e)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError(e, e.Message);
                }

                result.Succeeded = false;
                result.Error = e.Message;
            }
            finally
            {
                response?.Close();
                responseStream?.Close();
            }

            return result;

        }

        #endregion

        #region "Private Methods"

        string BuildParameterJsonString(IDictionary<string, string> parameters)
        {
            return parameters.Serialize();
        }

        string BuildParameterString(IDictionary<string, string> parameters)
        {
            var output = string.Empty;
            foreach (var pair in parameters)
            {
                output += BuildSet(pair.Key, pair.Value);
            }

            return (string.IsNullOrEmpty(output) ? "" : output.Substring(0, output.Length - 1));
        }

        string BuildSet(string key, string value)
        {
            return $"{key}={HttpUtility.UrlEncode(value)}&";
        }

        #endregion

    }

}
