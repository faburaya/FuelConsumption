using System;
using System.Collections.Generic;
using System.Linq;

namespace FuelConsumption.TestSpecsParser
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (string path in args)
            {
                try
                {
                    var hypertextFetcher = new Reusable.WebAccess.HypertextFetcher();
                    Console.WriteLine($"Ladet '{path}' herunter...");
                    string hypertext = hypertextFetcher.DownloadFrom(new Uri(path)).Result;

                    var scraper = new WebCrawler.CarSpecsHypertextContentParser();
                    IEnumerable<WebCrawler.CarSpecifications> parsedSpecs = scraper.ParseContent(hypertext);
                    Console.WriteLine(
                        $"Zergliederung hat die Spezifikationen von {parsedSpecs.Count()} Autos ergeben.");

                    foreach (WebCrawler.CarSpecifications specs in parsedSpecs)
                    {
                        Console.WriteLine(specs.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler bei der Zergliederung von '{path}' - {ex}");
                }
            }
        }
    }
}
