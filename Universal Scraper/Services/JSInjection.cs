using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal_Scraper.Services
{
    public class JSInjection
    {
        /// <summary>
        /// Inject JS functionality to get xPath of active elements.
        /// </summary>
        public static async void InjectXPathReader(WebView view, bool ignoreID) {
            Color highlightColor = (Color)Application.Current.Resources["HighlightColor"];
            string cssClass = ".hoverEffect { border: 2px solid " + highlightColor.ToHex() + "; }";

            // Indsæt CSS-klasse i <style> tag
            string jsCode = "var style = document.createElement('style'); style.innerHTML = '" + cssClass + "'; document.head.appendChild(style);";

            // Hent Get-XPath bibliotek
            jsCode += @"
                var scriptElement = document.createElement('script');
                scriptElement.src = 'https://unpkg.com/get-xpath';
                document.head.appendChild(scriptElement);
            ";

            // Definér JavaScript-kode, der injiceres i WebView'en
            jsCode += @"
                var xpath;

                document.addEventListener('mouseover', function(event) {
                    event.target.classList.add('hoverEffect');

                    xpath = getXPath(event.target, { ignoreId: " + ignoreID.ToString().ToLower() + @" });
                    window.location.href = 'http://poc.JSHandler?arg0=' + xpath + '&arg1=false';
                });
                
                document.addEventListener('mouseout', function() {
                    event.target.classList.remove('hoverEffect');
                });

                document.addEventListener('click', function(event) {
                    if(xpath != null) window.location.href = 'http://poc.JSHandler?arg0=' + xpath + '&arg1=true';
                });
            ";

            jsCode = jsCode.Replace("\n", "").Replace("\r", "").Trim();

            // Indlæs JavaScript-kode ved hjælp af EvaluateJavaScriptAsync-metoden
            await view.EvaluateJavaScriptAsync(jsCode);
        }
    }
}
