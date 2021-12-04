using System;
using System.Collections.Generic;

using Reusable.Utils;
using Reusable.WebAccess;

namespace FuelConsumption.WebCrawler
{
    /// <summary>
    /// 
    /// </summary>
    public class CarSpecsScraper
    {
        private readonly DatenSauger<CarSpecifications> _scraper;

        public CarSpecsScraper(IHypertextFetcher hypertextFetcher)
        {
            _scraper = new DatenSauger<CarSpecifications>(hypertextFetcher, new SimpleConsoleLogger());
        }

        public void ScrapeCarSpecs()
        {
            static bool doParsePage(string hypertext) => true;
            static bool doAcceptLink(string name) => true;

            var carBrandHyperlinksParser = new CarBrandHyperlinksParser(doParsePage, doAcceptLink);
            var carModelHyperlinksParser = new CarModelHyperlinksParser(doParsePage, doAcceptLink);
            var modelVersionHyperlinksParser = new ModelVersionHyperlinksParser(doParsePage, doAcceptLink);
            var trimLevelHyperlinksParser = new TrimLevelHyperlinksParser(doParsePage, doAcceptLink);
            var carSpecificationsParser = new CarSpecsHypertextContentParser();

            IEnumerable<CarSpecifications> allCarSpecifications =
                _scraper.CollectData(new Uri("https://www.ultimatespecs.com/de"),
                                     new IHyperlinksParser[] {
                                         carBrandHyperlinksParser,
                                         carModelHyperlinksParser,
                                         modelVersionHyperlinksParser,
                                         trimLevelHyperlinksParser
                                     },
                                     carSpecificationsParser);

            foreach (CarSpecifications carSpecifications in allCarSpecifications)
            {
                Console.WriteLine(carSpecifications);
            }
        }
    }
}
