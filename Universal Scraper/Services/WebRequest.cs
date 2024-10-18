using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Universal_Scraper.Services
{
    public static class WebRequest
    {
        /// <summary>
        /// Check if provided url is valid.
        /// Returns url of valid site. 
        /// </summary>
        public static async Task<string> IsValid(string url)
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36");
            client.DefaultRequestHeaders.Add("x-restli-protocol-version", "2.0.0");

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return response.RequestMessage.RequestUri.ToString();
                } else
                {
                    Debug.WriteLine($"Fejl: {response.StatusCode} - {response.ReasonPhrase}");
                }
            } catch (HttpRequestException ex)
            {
                Debug.WriteLine($"Fejl: {ex.Message}");
            }
                
            return null;
        }
    }
}
