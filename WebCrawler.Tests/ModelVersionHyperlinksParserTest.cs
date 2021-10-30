using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

namespace FuelConsumption.WebCrawler.Tests
{
    [Collection("ParserTests")]
    public class ModelVersionHyperlinksParserTest
    {
        private ParserTestsFixture Fixture { get; }

        public ModelVersionHyperlinksParserTest(ParserTestsFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void ParseHyperlinks_DoNotParse_GiveNone()
        {
            var parser = new ModelVersionHyperlinksParser(
                (string hypertext) => false, // zergliedere die Weite NICHT
                (string modelVersion) => true // gib alle Versionen zurück
            );

            IEnumerable<Uri> hyperlinks = parser.ParseHyperlinks(Fixture.ModelVersionsWebPageContentSample);

            Assert.NotNull(hyperlinks);
            Assert.Empty(hyperlinks);
        }

        [Fact]
        public void ParseHyperlinks_DoParse_DoNotFilter_GiveAll()
        {
            var parser = new ModelVersionHyperlinksParser(
                (string hypertext) => true, // zergliedere die Weite
                (string modelVersion) => true // gib alle Versionen zurück
            );

            IEnumerable<Uri> hyperlinks = parser.ParseHyperlinks(Fixture.ModelVersionsWebPageContentSample);

            Assert.NotNull(hyperlinks);
            IEnumerable<string> urls = from address in hyperlinks select address.ToString().ToLower();

            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/volkswagen/m1511/jetta-6", urls);
            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/volkswagen/m329/jetta-5", urls);
        }

        [Fact]
        public void ParseHyperlinks_DoParse_DoFilter_GiveOnesContainingKeyword()
        {
            var keywords = new[] { "FACELIFT", "JETTA-6" };

            var parser = new ModelVersionHyperlinksParser(
                (string hypertext) => true, // zergliedere die Weite
                (string modelVersion) => { // suche die Versionen aus:
                    modelVersion = modelVersion.ToUpper();
                    return keywords.Any(keyword => modelVersion.Contains(keyword));
                }
            );

            IEnumerable<Uri> hyperlinks = parser.ParseHyperlinks(Fixture.ModelVersionsWebPageContentSample);

            Assert.NotNull(hyperlinks);
            IEnumerable<string> urls = from address in hyperlinks select address.ToString().ToLower();

            Assert.Equal(2, urls.Count());
            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/volkswagen/m1511/jetta-6", urls);
            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/volkswagen/m8878/jetta-6-facelift", urls);
        }
    }
}