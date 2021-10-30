using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

namespace FuelConsumption.WebCrawler.Tests
{
    [Collection("ParserTests")]
    public class CarModelHyperlinksParserTest
    {
        private ParserTestsFixture Fixture { get; }

        public CarModelHyperlinksParserTest(ParserTestsFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void ParseHyperlinks_DoNotParse_GiveNone()
        {
            var parser = new CarModelHyperlinksParser(
                (string hypertext) => false, // zergliedere die Webseite NICHT
                (string carModel) => true // gib alle Automodelle zurück
            );

            IEnumerable<Uri> hyperlinks =
                parser.ParseHyperlinks(Fixture.CarModelsWebPageContentSample);

            Assert.NotNull(hyperlinks);
            Assert.Empty(hyperlinks);
        }

        [Fact]
        public void ParseHyperlinks_DoParse_DoNotFilter_GiveAll()
        {
            var parser = new CarModelHyperlinksParser(
                (string hypertext) => true, // zergliedere die Webseite
                (string carModel) => true // gib alle Automodelle zurück
            );

            IEnumerable<Uri> hyperlinks = parser.ParseHyperlinks(Fixture.CarModelsWebPageContentSample);

            Assert.NotNull(hyperlinks);
            IEnumerable<string> urls = from address in hyperlinks select address.ToString().ToLower();

            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/volkswagen-models/volkswagen-jetta", urls);
            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/volkswagen-models/volkswagen-t-roc", urls);
            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/volkswagen/m10919/id3", urls);
            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/volkswagen/m9410/amarok-2017", urls);
        }

        [Fact]
        public void ParseHyperlinks_DoParse_DoFilter_GiveOnesContainingKeyword()
        {
            var keywords = new string[] { "JETTA", "ID3" };

            var parser = new CarModelHyperlinksParser(
                (string hypertext) => true, // zergliedere die Webseite
                (string carModel) => { // suche die Automodelle aus:
                    carModel = carModel.ToUpper();
                    return keywords.Any(keyword => carModel.Contains(keyword));
                }
            );

            IEnumerable<Uri> hyperlinks = parser.ParseHyperlinks(Fixture.CarModelsWebPageContentSample);

            Assert.NotNull(hyperlinks);
            IEnumerable<string> urls = from address in hyperlinks select address.ToString().ToLower();

            Assert.Equal(2, urls.Count());
            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/volkswagen-models/volkswagen-jetta", urls);
            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/volkswagen/m10919/id3", urls);
        }
    }
}