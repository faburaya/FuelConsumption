using System.Text.Json;

using Reusable.DataModels;

namespace FuelConsumption.WebCrawler
{
    public enum FuelType { Petrol, Diesel, CNG }

    public enum FuelSystemType { DirectInjection, Other }

    public enum TransmissionType { Automatic, Manual }

    /// <summary>
    /// Hält die Spezifikationen eines bestimmten Automodells.
    /// </summary>
    public class CarSpecifications
    {
        [RdbTablePrimaryKey]
        public string ModelFullName { get; set; }

        public FuelType Fuel { get; set; }

        public FuelSystemType FuelSystem { get; set; }

        public TransmissionType Transmission { get; set; }

        public ushort TransmissionSpeeds { get; set; }

        public float CompressionRatio { get; set; }

        /// <summary>
        /// Hubraum in cm3
        /// </summary>
        public ushort CylinderCapacity { get; set; }

        /// <summary>
        /// Leergewicht in kg
        /// </summary>
        public ushort Weight { get; set; }

        /// <summary>
        /// Drehmoment in N.m
        /// </summary>
        public ushort MaxTorque { get; set; }

        /// <summary>
        /// Maximale Motorleistung in PS
        /// </summary>
        public ushort MaxPower { get; set; }

        /// <summary>
        /// Drehung in rpm bei maximaler Leistung
        /// </summary>
        public ushort RotationAtMaxPower { get; set; }

        /// <summary>
        /// Kraftstoffverbrauch innerorts in L/100km
        /// </summary>
        public float FuelConsumptionCity { get; set; }

        /// <summary>
        /// Kraftstoffverbrauch außerorts in L/100km
        /// </summary>
        public float FuelConsumptionHighway { get; set; }

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}
