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
            _carBrandsWebPageContentSample ??= File.ReadAllText(
                Path.Combine("samples", "test_web_page_brands.html"));

        private string _carModelsWebPageContentSample;

        public string CarModelsWebPageContentSample =>
            _carModelsWebPageContentSample ??= File.ReadAllText(
                Path.Combine("samples", "test_web_page_models.html"));

        private string _modelVersionsWebPageContentSample;

        public string ModelVersionsWebPageContentSample =>
            _modelVersionsWebPageContentSample ??= File.ReadAllText(
                Path.Combine("samples", "test_web_page_versions.html"));

        private string _trimLevelsWebPageContentSample;

        public string TrimLevelsWebPageContentSample =>
            _trimLevelsWebPageContentSample ??= File.ReadAllText(
                Path.Combine("samples", "test_web_page_trim_levels.html"));

        private string _carSpecsWebPageContentSample;

        public string CarSpecsWebPageContentSample =>
            _carSpecsWebPageContentSample ??= File.ReadAllText(
                Path.Combine("samples", "test_web_page_specs.html"));
    }

}// end of namespace FuelConsumption.WebCrawler.Tests
