using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text.RegularExpressions;

using HtmlAgilityPack;

namespace FuelConsumption.WebCrawler
{
    /// <summary>
    /// Zergliedert den Inhalt aus https://www.ultimatespecs.com/de,
    /// um die Spezifikationen eines bestimmten Automodells zu sammeln.
    /// </summary>
    public class CarSpecsHypertextContentParser : Reusable.WebAccess.IHypertextContentParser<CarSpecifications>
    {
        public IEnumerable<CarSpecifications> ParseContent(string hypertext)
        {
            var specs = new CarSpecifications();
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(hypertext);

            IEnumerable<CallbackTryParseRow> parsers = allParsers;

            IEnumerable<HtmlNode> allRowElements =
                from node in htmlDocument.DocumentNode.SelectNodes("//tr")
                where node.NodeType == HtmlNodeType.Element
                select node;

            foreach (HtmlNode node in allRowElements)
            {
                // versucht alle Zerglieder, die nocht nicht erfolgreich sind:
                foreach (CallbackTryParseRow callbackTryParse in parsers)
                {
                    if (callbackTryParse(node, specs))
                    {
                        parsers = (from x in parsers where x != callbackTryParse select x);
                        break;
                    }
                }

                if (!parsers.Any())
                {
                    return new[] { specs };
                }
            }

            // meldet alle Zerglieder, die nicht erfolgreich waren
            throw new AggregateException("Zergliederung der Spezifikationen is gescheitert!",
                from parser in parsers select new Exception($"{parser.Method.Name} ohne Erfolg"));
        }

        private delegate bool CallbackTryParseRow(HtmlNode rowElement, CarSpecifications specs);

        private static readonly IList<CallbackTryParseRow> allParsers;

        private static readonly CultureInfo culture;

        static CarSpecsHypertextContentParser()
        {
            culture = new CultureInfo("de-DE", false);
            culture.NumberFormat.NumberDecimalSeparator = ".";

            allParsers = new CallbackTryParseRow[] {
                TryParseCylinderCapacity,
                TryParseCompressionRatio,
                TryParseFuel,
                TryParseFuelSystem,
                TryParseFuelConsumptionCity,
                TryParseFuelConsumptionHighway,
                TryParsePowerAndRotation,
                TryParseTorque,
                TryParseTransmissionAndSpeeds,
                TryParseWeight,
            };
        }   

        #region Parsers

        private static bool TryParseFuel(HtmlNode element, CarSpecifications specs)
        {
            return TryParseValueFrom(element, "Kraftstoffart",
                text => { specs.Fuel = ParseFuel(text); });
        }

        private static FuelType ParseFuel(string valueStamp)
        {
            string fuelName = valueStamp.ToLower();
            if (fuelName == "benzin")
            {
                return FuelType.Petrol;
            }
            else if (fuelName == "diesel")
            {
                return FuelType.Diesel;
            }
            else if (fuelName.Contains("cng"))
            {
                return FuelType.CNG;
            }

            throw new ArgumentException($"'{valueStamp}' ist kein anerkannter Kraftstoff!");
        }

        private static bool TryParseFuelSystem(HtmlNode element, CarSpecifications specs)
        {
            return TryParseValueFrom(element, "Fuel System",
                text => { specs.FuelSystem = ParseFuelSystem(text); });
        }

        private static FuelSystemType ParseFuelSystem(string valueStamp)
        {
            string fuelSystemName = valueStamp.ToLower();
            if (fuelSystemName.Contains("direct injection"))
            {
                return FuelSystemType.DirectInjection;
            }
            return FuelSystemType.Other;
        }

        private static bool TryParseCompressionRatio(HtmlNode element, CarSpecifications specs)
        {
            return TryParseValueFrom(element, "Verdichtung",
                text => { specs.CompressionRatio = ParseCompressionRate(text); });
        }

        private static float ParseCompressionRate(string valueStamp)
        {
            if (TryParseFloat(valueStamp, out float value))
            {
                return value;
            }

            throw new ArgumentException(
                $"Verdichtung kann nicht zergliedert werden: '{valueStamp}'");
        }

        private static bool TryParseTransmissionAndSpeeds(HtmlNode element, CarSpecifications specs)
        {
            return TryParseValueFrom(element, "Getriebe",
                text => {
                    var (gearSpeedsCount, transmission) = ParseTransmissionAndSpeeds(text);
                    specs.Transmission = transmission;
                    specs.TransmissionSpeeds = gearSpeedsCount;
                });
        }

        private static readonly Regex transmissionRegex =
            new Regex(@"(\d+) Geschwindigkeit (\w+)", RegexOptions.Compiled);

        private static (ushort, TransmissionType) ParseTransmissionAndSpeeds(string valueStamp)
        {
            Match match = transmissionRegex.Match(valueStamp);
            if (match.Success
                && ushort.TryParse(match.Groups[1].Value, out ushort gearSpeedsCount))
            {
                TransmissionType transmission;
                switch (match.Groups[2].Value.ToLower())
                {
                    case "automatic":
                        transmission = TransmissionType.Automatic;
                        break;
                    case "manual":
                        transmission = TransmissionType.Manual;
                        break;
                    default:
                        throw new ArgumentException($"Das Getriebe '{match.Groups[0].Value}' kann nicht zergliedert werden!");
                }

                return (gearSpeedsCount, transmission);
            }

            throw new ArgumentException($"Zergliederung des Getriebes ist gescheitert: {valueStamp}");
        }

        private static bool TryParsePowerAndRotation(HtmlNode element, CarSpecifications specs)
        {
            return TryParseValueFrom(element, "Motorleistung",
                text => {
                    var (maxPower, rotationAtMaxPower) = ParsePowerAndRotation(text);
                    specs.MaxPower = maxPower;
                    specs.RotationAtMaxPower = rotationAtMaxPower;
                });
        }

        private static readonly Regex enginePowerRegex =
            new Regex(@"(?<power>\d+) PS(?: or \d+ \w+)+ @ (?<rpm>\d+) rpm");

        private static (ushort, ushort) ParsePowerAndRotation(string valueStamp)
        {
            Match match = enginePowerRegex.Match(valueStamp);
            if (match.Success
                && ushort.TryParse(match.Groups["power"].Value, out ushort maxPower)
                && ushort.TryParse(match.Groups["rpm"].Value, out ushort rotationAtMaxPower))
            {
                return (maxPower, rotationAtMaxPower);
            }

            throw new ArgumentException($"Zergliederung der Motorleistung ist gescheitert: {valueStamp}");
        }

        private static readonly Regex cylinderCapacityRegex = new Regex(@"(\d+) cm3", RegexOptions.Compiled);

        private static bool TryParseCylinderCapacity(HtmlNode element, CarSpecifications specs)
        {
            return TryParseValueFrom(element, "Hubraum", text => { specs.CylinderCapacity = ParseSingleValue<ushort>("Hubraum", cylinderCapacityRegex, ushort.TryParse, text); });
        }

        private static readonly Regex weightRegex = new Regex(@"(\d+) kg", RegexOptions.Compiled);

        private static bool TryParseWeight(HtmlNode element, CarSpecifications specs)
        {
            return TryParseValueFrom(element, "Leergewicht", text => { specs.Weight = ParseSingleValue<ushort>("Leergewicht", weightRegex, ushort.TryParse, text); });
        }

        private static readonly Regex engineTorqueRegex = new Regex(@"(\d+) Nm", RegexOptions.Compiled);

        private static bool TryParseTorque(HtmlNode element, CarSpecifications specs)
        {
            return TryParseValueFrom(element, "Drehmoment", text => { specs.MaxTorque = ParseSingleValue<ushort>("Drehmoment", engineTorqueRegex, ushort.TryParse, text); });
        }

        private static readonly Regex fuelConsumptionRegex =
            new Regex(@"(\d+(\.\d+)?) L/100km", RegexOptions.Compiled);

        private static bool TryParseFuelConsumptionCity(HtmlNode element, CarSpecifications specs)
        {
            return TryParseValueFrom(element, "Verbrauch - Innerorts", text => {
                specs.FuelConsumptionCity = ParseSingleValue<float>("Verbrauch innerorts", fuelConsumptionRegex, TryParseFloat, text);
            });
        }

        private static bool TryParseFuelConsumptionHighway(HtmlNode element, CarSpecifications specs)
        {
            return TryParseValueFrom(element, "Verbrauch - Außerorts", text => {
                specs.FuelConsumptionHighway = ParseSingleValue<float>("Verbrauch außerorts", fuelConsumptionRegex, TryParseFloat, text);
            });
        }

        private static bool TryParseFloat(string text, out float value)
        {
            return float.TryParse(text, NumberStyles.Float, culture, out value);
        }

        private delegate bool CallbackTryParseValue<ValueType>(string text, out ValueType value);

        /// <summary>
        /// Macht einen regulären Ausdruck zunutze, um einen einzelnen Wert zu zergliedern.
        /// </summary>
        /// <typeparam name="ValueType">Der Typ zu erfassen.</typeparam>
        /// <param name="label">Eine Bezeichnung für den Wert (zum Protokollierung).</param>
        /// <param name="regex">Der regulärer Ausdruck.</param>
        /// <param name="callbackTryParse">Wandelt den Wert aus der Textform in den gegebenen Typ um.</param>
        /// <param name="valueStamp">Der Wert in Textform.</param>
        /// <returns>Der erfassten Wert.</returns>
        private static ValueType ParseSingleValue<ValueType>(
            string label,
            Regex regex,
            CallbackTryParseValue<ValueType> callbackTryParse,
            string valueStamp) where ValueType : struct
        {
            Match match = regex.Match(valueStamp);
            if (match.Success
                && callbackTryParse(match.Groups[1].Value, out ValueType value))
            {
                return value;
            }

            throw new ArgumentException($"{label} kann nicht zergliedert werden: '{valueStamp}'");
        }

        /// <summary>
        /// Versucht die Zergliederung eines Wertes aus einer Reihe der Tabellen,
        /// die in der Webseite alle Spezifikationen aufstellen.
        /// </summary>
        /// <param name="rowElement">Das HTML-Element, das einen Reihe in einer Tabelle dartellt.</param>
        /// <param name="label">Die Bezeichnung des zu zergliedernden Wertes (linke Spalte).</param>
        /// <param name="tryParseFromText">Zergliedert den Wert aus der Textform.</param>
        /// <returns>Ob die Zergliederung gelungen ist.</returns>
        private static bool TryParseValueFrom(HtmlNode rowElement,
                                              string label,
                                              Action<string> parseFromText)
        {
            if (rowElement.Name != "tr" || rowElement.NodeType != HtmlNodeType.Element)
            {
                throw new ArgumentException(
                    $"Unerwartetes HTML-Node {rowElement.Name} des Typs {rowElement.NodeType}");
            }

            HtmlNode[] columns =
                (from node in rowElement.Elements("td")
                 where node.NodeType == HtmlNodeType.Element
                 select node).ToArray();

            if (columns.Count() != 2)
            {
                return false;
            }

            int idxColumnValue = -1;
            for (int colIdx = 0; colIdx < 2; ++colIdx)
            {
                if (columns[colIdx].InnerText.Contains(label))
                {
                    // nimmt das andere Index:
                    idxColumnValue = (colIdx + 1) % 2;
                    break;
                }
            }

            if (idxColumnValue < 0)
            {
                return false;
            }

            string value = columns[idxColumnValue].InnerText.TrimStart().TrimEnd();
            parseFromText(value);
            return true;
        }

        #endregion
    }
}
