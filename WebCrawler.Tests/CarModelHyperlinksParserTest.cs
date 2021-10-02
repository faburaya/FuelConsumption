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

        private CarModelHyperlinksParser Parser { get; }

        public CarModelHyperlinksParserTest(ParserTestsFixture fixture)
        {
            Fixture = fixture;
            Parser = new CarModelHyperlinksParser();
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