using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

namespace FuelConsumption.WebCrawler.Tests
{
    [Collection("ParserTests")]
    public class CarBrandHyperlinksParserTest
    {
        private ParserTestsFixture Fixture { get; }

        public CarBrandHyperlinksParserTest(ParserTestsFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void ParseHyperlinks_DoNotParse_GiveNone()
        {
            var parser = new CarBrandHyperlinksParser(
                (string hypertext) => false, // zergliedere die Webseite nicht
                (string carBrand) => true // gib alle Marken zurück
            );

            IEnumerable<Uri> hyperlinks =
                parser.ParseHyperlinks(Fixture.CarBrandsWebPageContentSample);

            Assert.NotNull(hyperlinks);
            Assert.Empty(hyperlinks);
        }

        [Fact]
        public void ParseHyperlinks_DoParse_DoNotFilter_GiveAll()
        {
            var parser = new CarBrandHyperlinksParser(
                (string hypertext) => true, // zergliedere die Webseite
                (string carBrand) => true // gib alle Marken zurück
            );

            IEnumerable<Uri> hyperlinks =
                parser.ParseHyperlinks(Fixture.CarBrandsWebPageContentSample);

            Assert.NotNull(hyperlinks);
            IEnumerable<string> urls = (from url in hyperlinks select url.ToString().ToLower());

            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/hyundai-models", urls);
            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/toyota-models", urls);
            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/renault-models", urls);
            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/volkswagen-models", urls);
        }

        [Fact]
        public void ParseHyperlinks_DoParse_DoFilter_GiveOnesContainingKeyword()
        {
            var keywords = new string[] { "HYUNDAI", "TOYOTA" };

            var parser = new CarBrandHyperlinksParser(
                (string hypertext) => true, // zergliedere die Webseite
                (string carBrand) => { // suche die Marken aus:
                    carBrand = carBrand.ToUpper();
                    return keywords.Any(keyword => carBrand.Contains(keyword));
                }
            );

            IEnumerable<Uri> hyperlinks = parser.ParseHyperlinks(Fixture.CarBrandsWebPageContentSample);

            Assert.NotNull(hyperlinks);
            IEnumerable<string> urls = (from address in hyperlinks select address.ToString().ToLower());

            Assert.Equal(2, urls.Count());
            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/hyundai-models", urls);
            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/toyota-models", urls);
        }
    }
}