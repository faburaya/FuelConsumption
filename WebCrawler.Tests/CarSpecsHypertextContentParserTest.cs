using System.Linq;

using Xunit;

namespace FuelConsumption.WebCrawler.Tests
{
    [Collection("ParserTests")]
    public class CarSpecsHypertextContentParserTest
    {
        private ParserTestsFixture Fixture { get; }

        private CarSpecsHypertextContentParser Parser { get; }

        public CarSpecsHypertextContentParserTest(ParserTestsFixture fixture)
        {
            Fixture = fixture;
            Parser = new CarSpecsHypertextContentParser();
        }

        [Fact]
        public void ParseContent_CaptureAllSpecs()
        {
            CarSpecifications carSpecs =
                Parser.ParseContent(Fixture.CarSpecsWebPageContentSample).FirstOrDefault();

            Assert.NotNull(carSpecs);
            Assert.Equal(FuelType.Petrol, carSpecs.Fuel);
            Assert.Equal(FuelSystemType.DirectInjection, carSpecs.FuelSystem);
            Assert.Equal(TransmissionType.Manual, carSpecs.Transmission);
            Assert.Equal(6, carSpecs.TransmissionSpeeds);
            Assert.Equal(1984, carSpecs.CylinderCapacity);
            Assert.Equal(1241, carSpecs.Weight);
            Assert.Equal(320, carSpecs.MaxTorque);
            Assert.Equal(200, carSpecs.MaxPower);
            Assert.Equal(4700, carSpecs.RotationAtMaxPower);

            float p = 1e-6F;
            Assert.InRange(carSpecs.CompressionRatio, 10.3 - p, 10.3 + p);
            Assert.InRange(carSpecs.FuelConsumptionHighway, 4.9 - p, 4.9 + p);
            Assert.InRange(carSpecs.FuelConsumptionCity, 7.7 - p, 7.7 + p);
        }
    }
}