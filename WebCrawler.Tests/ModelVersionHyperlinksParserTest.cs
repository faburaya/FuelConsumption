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

        private ModelVersionHyperlinksParser Parser { get; }

        public ModelVersionHyperlinksParserTest(ParserTestsFixture fixture)
        {
            Fixture = fixture;
            Parser = new ModelVersionHyperlinksParser();
        }

        [Fact]
        public void ParseHyperlinks_NoKeywords_GiveAll()
        {
            IEnumerable<Uri> hyperlinks =
                Parser.ParseHyperlinks(Fixture.ModelVersionsWebPageContentSample, null);

            Assert.NotNull(hyperlinks);
            IEnumerable<string> urls = from address in hyperlinks select address.ToString().ToLower();

            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/volkswagen/m1511/jetta-6", urls);
            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/volkswagen/m329/jetta-5", urls);
        }

        [Fact]
        public void ParseHyperlinks_HasKeywords_GiveOnesContainingKeyword()
        {
            IEnumerable<Uri> hyperlinks =
                Parser.ParseHyperlinks(Fixture.ModelVersionsWebPageContentSample, new[] { "FACELIFT", "JETTA-6" });

            Assert.NotNull(hyperlinks);
            IEnumerable<string> urls = from address in hyperlinks select address.ToString().ToLower();

            Assert.Equal(2, urls.Count());
            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/volkswagen/m1511/jetta-6", urls);
            Assert.Contains("https://www.ultimatespecs.com/de/car-specs/volkswagen/m8878/jetta-6-facelift", urls);
        }
    }
}