using System.IO;

using Xunit;

namespace FuelConsumption.WebCrawler.Tests
{
    [CollectionDefinition("ParserTests")]
    public class ParserTestFixturesCollection
        : ICollectionFixture<ParserTestsFixture>
    {
    }

    /// <summary>
    /// Fixture für das Testen aller Zerglieder.
    /// </summary>
    /// <remarks>Die Implementierung verzögert die Ladung einer HTML-Datei bis zum letzten Moment.</remarks>
    public class ParserTestsFixture
    {
        private string _carBrandsWebPageContentSample;

        public string CarBrandsWebPageContentSample =>
            _carBrandsWebPageContentSample ??= File.ReadAllText("test_web_page_brands.html");

        private string _carModelsWebPageContentSample;

        public string CarModelsWebPageContentSample =>
            _carModelsWebPageContentSample ??= File.ReadAllText("test_web_page_models.html");
    }
}
