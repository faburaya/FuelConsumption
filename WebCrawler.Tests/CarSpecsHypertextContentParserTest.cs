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
            Assert.Equal(TransmissionType.Automatic, carSpecs.Transmission);
            Assert.Equal(7, carSpecs.TransmissionSpeeds);
            Assert.Equal(1984, carSpecs.CylinderCapacity);
            Assert.Equal(1450, carSpecs.Weight);
            Assert.Equal(270, carSpecs.MaxTorque);
            Assert.Equal(150, carSpecs.MaxPower);
            Assert.Equal(3900, carSpecs.RotationAtMaxPower);

            float p = 1e-6F;
            Assert.InRange(carSpecs.CompressionRatio, 10.5 - p, 10.5 + p);
            Assert.InRange(carSpecs.FuelConsumptionHighway, 4.6 - p, 4.6 + p);
            Assert.InRange(carSpecs.FuelConsumptionCity, 7.2 - p, 7.2 + p);
        }
    }
}