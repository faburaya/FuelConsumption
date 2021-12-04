using System;

namespace FuelConsumption.DataCollector
{
    class Program
    {
        static void Main()
        {
            var hypertextFetcher = new Reusable.WebAccess.HypertextFetcher();
            var scraper = new WebCrawler.CarSpecsScraper(hypertextFetcher);
            scraper.ScrapeCarSpecs();
        }
    }
}
