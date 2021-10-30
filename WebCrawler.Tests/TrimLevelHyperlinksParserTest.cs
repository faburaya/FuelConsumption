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

        public TrimLevelHyperlinksParserTest(ParserTestsFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void ParseHyperlinks_DoNotParse_GiveNone()
        {
            var parser = new TrimLevelHyperlinksParser(
                (string hypertext) => false, // zergliedere die Webseite NICHT
                (string trimLevel) => true // gib alle "Trim Level" zurück
            );

            IEnumerable<Uri> hyperlinks = parser.ParseHyperlinks(Fixture.TrimLevelsWebPageContentSample);

            Assert.NotNull(hyperlinks);
            Assert.Empty(hyperlinks);
        }

        [Fact]
        public void ParseHyperlinks_DoParse_DoNotFilter_GiveAll()
        {
            var parser = new TrimLevelHyperlinksParser(
                (string hypertext) => true, // zergliedere die Webseite
                (string trimLevel) => true // gib alle "Trim Level" zurück
            );

            IEnumerable<Uri> hyperlinks = parser.ParseHyperlinks(Fixture.TrimLevelsWebPageContentSample);

            Assert.NotNull(hyperlinks);
            IEnumerable<string> urls = from address in hyperlinks select address.ToString().ToLower();

            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/volkswagen/111948/volkswagen-polo-6-10-65hp.html", urls);
            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/volkswagen/111216/volkswagen-polo-6-16-tdi-95hp.html", urls);
        }

        [Fact]
        public void ParseHyperlinks_DoParse_DoFilter_GiveOnesContainingKeyword()
        {
            var keywords = new string[] { "GTI", "TGI" };

            var parser = new TrimLevelHyperlinksParser(
                (string hypertext) => true, // zergliedere die Webseite
                (string trimLevel) => { // suche die "Trim Levels" aus:
                    trimLevel = trimLevel.ToUpper();
                    return keywords.Any(keyword => trimLevel.Contains(keyword));
                }
            );

            IEnumerable<Uri> hyperlinks = parser.ParseHyperlinks(Fixture.TrimLevelsWebPageContentSample);

            Assert.NotNull(hyperlinks);
            IEnumerable<string> urls = from address in hyperlinks select address.ToString().ToLower();

            Assert.Equal(2, urls.Count());
            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/volkswagen/111956/volkswagen-polo-6-20-tsi-gti.html", urls);
            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/volkswagen/111212/volkswagen-polo-6-10-tgi-90hp.html", urls);
        }
    }
}