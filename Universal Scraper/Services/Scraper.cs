using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Universal_Scraper.Models;

namespace Universal_Scraper.Services
{
    public class Scraper
    {
        public static async Task<List<Selector>> ScrapeTarget(ProjectData projectData)
        {
            List<Selector> selectors = new();

            switch (projectData.TargetType)
            {
                case (TargetType.Single_Page):
                    selectors = await ScrapeSinglePage(projectData.TargetUrl, projectData.Selectors);

                    break;
                case (TargetType.Multi_Page):

                    break;
                case (TargetType.Pages_From_File):

                    break;
            }

            return selectors;
        }

        private static async Task<List<Selector>> ScrapeSinglePage(string url, List<Selector> selectors)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36");
            client.DefaultRequestHeaders.Add("x-restli-protocol-version", "2.0.0");

            try
            {
                using (client)
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (!response.IsSuccessStatusCode) {
                        Debug.WriteLine($"Fejl: {response.StatusCode} - {response.ReasonPhrase}");
                        return null;
                    }

                    string html = await response.Content.ReadAsStringAsync();
                    html = WebUtility.HtmlDecode(html);

                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(html);

                    foreach (Selector selector in selectors)
                    {
                        HtmlNode scrapeResult = doc.DocumentNode.SelectSingleNode(selector.XPath);

                        if(scrapeResult == null)
                        {
                            Debug.WriteLine("Scrape Error!");
                            return null;
                        }

                        switch (selector.ElementType)
                        {
                            case (ElementType.Text):
                                selector.Result = Regex.Replace(scrapeResult.InnerText, @"[\r\n\t]", "");
                                break;
                            case (ElementType.Link):
                                string link = scrapeResult.Attributes["href"].Value;

                                if (link.Contains("http"))
                                {
                                    selector.Result = scrapeResult.Attributes["href"].Value;
                                    break;
                                }

                                selector.Result = GetBaseUrl(url) + link;
                                break;
                        }
                    }
                }
            } catch (System.Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return selectors;
        }

        static string GetBaseUrl(string urlString)
        {
            Uri uri = new Uri(urlString);
            string baseUrl = uri.GetLeftPart(UriPartial.Authority);
            return baseUrl;
        }
    }
}
