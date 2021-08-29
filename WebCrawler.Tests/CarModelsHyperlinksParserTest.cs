using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

namespace FuelConsumption.WebCrawler.Tests
{
    [Collection("ParserTests")]
    public class CarModelsHyperlinksParserTest
    {
        private ParserTestsFixture Fixture { get; }

        private CarModelsHyperlinksParser Parser { get; }

        public CarModelsHyperlinksParserTest(ParserTestsFixture fixture)
        {
            Fixture = fixture;
            Parser = new CarModelsHyperlinksParser();
        }

        [Fact]
        public void ParseHyperlinks_NoKeywords_GiveAll()
        {
            IEnumerable<Uri> hyperlinks =
                Parser.ParseHyperlinks(Fixture.CarModelsWebPageContentSample, null);

            Assert.NotNull(hyperlinks);
            IEnumerable<string> urls = from address in hyperlinks select address.ToString().ToLower();

            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/volkswagen-models/volkswagen-jetta", urls);
            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/volkswagen-models/volkswagen-t-roc", urls);
            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/volkswagen/m10919/id3", urls);
            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/volkswagen/m9410/amarok-2017", urls);
        }

        [Fact]
        public void ParseHyperlinks_HasKeywords_GiveOnesContainingKeyword()
        {
            IEnumerable<Uri> hyperlinks =
                Parser.ParseHyperlinks(Fixture.CarModelsWebPageContentSample, new[] { "JETTA", "ID3" });

            Assert.NotNull(hyperlinks);
            IEnumerable<string> urls = from address in hyperlinks select address.ToString().ToLower();

            Assert.Equal(2, urls.Count());
            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/volkswagen-models/volkswagen-jetta", urls);
            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/volkswagen/m10919/id3", urls);
        }
    }
}