using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

namespace FuelConsumption.WebCrawler.Tests
{
    [Collection("ParserTests")]
    public class TrimLevelHyperlinksParserTest
    {
        private ParserTestsFixture Fixture { get; }

        private TrimLevelHyperlinksParser Parser { get; }

        public TrimLevelHyperlinksParserTest(ParserTestsFixture fixture)
        {
            Fixture = fixture;
            Parser = new TrimLevelHyperlinksParser();
        }

        [Fact]
        public void ParseHyperlinks_NoKeywords_GiveAll()
        {
            IEnumerable<Uri> hyperlinks =
                Parser.ParseHyperlinks(Fixture.TrimLevelsWebPageContentSample, null);

            Assert.NotNull(hyperlinks);
            IEnumerable<string> urls = from address in hyperlinks select address.ToString().ToLower();

            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/volkswagen/111948/volkswagen-polo-6-10-65hp.html", urls);
            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/volkswagen/111216/volkswagen-polo-6-16-tdi-95hp.html", urls);
        }

        [Fact]
        public void ParseHyperlinks_HasKeywords_GiveOnesContainingKeyword()
        {
            IEnumerable<Uri> hyperlinks =
                Parser.ParseHyperlinks(Fixture.TrimLevelsWebPageContentSample, new[] { "GTI", "TGI" });

            Assert.NotNull(hyperlinks);
            IEnumerable<string> urls = from address in hyperlinks select address.ToString().ToLower();

            Assert.Equal(2, urls.Count());
            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/volkswagen/111956/volkswagen-polo-6-20-tsi-gti.html", urls);
            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/volkswagen/111212/volkswagen-polo-6-10-tgi-90hp.html", urls);
        }
    }
}