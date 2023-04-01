using Microsoft.Extensions.Configuration;
using System.Net;

namespace PriceDropCheck
{
    internal class HttpRequest
    {
        private readonly string Site = "https://www.ruroc.com";
        private readonly string FailureMessage = "Something's wrong, check site ";
        private readonly IEmailService _emailService;

        public HttpRequest(IEmailService emailService)
        {
            _emailService= emailService;
        }

        public void Create()
        {
            var message = FailureMessage + Site + " Error: ";

            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            var client = new HttpClient(handler);

            client.BaseAddress = new Uri(Site);
            HttpResponseMessage response = client.GetAsync("en_eu/atlas-4-nebula-carbon.html").Result;
            
            try
            {
                response.EnsureSuccessStatusCode();
                string html = response.Content.ReadAsStringAsync().Result;

                var priceProp = "<meta property=\"product:price:amount\" content=";

                if (html.Contains(priceProp))
                {
                    var price = html.Substring(html.IndexOf(priceProp) + priceProp.Length + 1, 3);
                    Console.WriteLine(price);

                    if (int.TryParse(price, out int priceInt))
                    {
                        if (priceInt < 350)
                        {
                            message = $"Price Dropped! New price: {priceInt}.";
                        }

                        return;
                    }
                    else
                    {
                        message += "Problem finding price.";
                    }
                }
            }
            catch
            {
                message += "Problem pinging site.";
            }

            _emailService.SendEmail(message);
        }
    }
}
