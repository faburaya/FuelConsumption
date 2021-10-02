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

        private CarBrandHyperlinksParser Parser { get; }

        public CarBrandHyperlinksParserTest(ParserTestsFixture fixture)
        {
            Fixture = fixture;
            Parser = new CarBrandHyperlinksParser();
        }

        [Fact]
        public void ParseHyperlinks_NoKeywords_GiveAll()
        {
            IEnumerable<Uri> hyperlinks =
                Parser.ParseHyperlinks(Fixture.CarBrandsWebPageContentSample, null);

            Assert.NotNull(hyperlinks);
            IEnumerable<string> urls = (from url in hyperlinks select url.ToString().ToLower());

            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/hyundai-models", urls);
            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/toyota-models", urls);
            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/renault-models", urls);
            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/volkswagen-models", urls);
        }

        [Fact]
        public void ParseHyperlinks_HasKeywords_GiveOnesContainingKeyword()
        {
            IEnumerable<Uri> hyperlinks =
                Parser.ParseHyperlinks(Fixture.CarBrandsWebPageContentSample,
                                       new string[] { "HYUNDAI", "TOYOTA" });

            Assert.NotNull(hyperlinks);
            IEnumerable<string> urls = (from address in hyperlinks select address.ToString().ToLower());

            Assert.Equal(2, urls.Count());
            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/hyundai-models", urls);
            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/toyota-models", urls);
        }
    }
}