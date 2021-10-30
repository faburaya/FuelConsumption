using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FuelConsumption.WebCrawler
{
    /// <summary>
    /// Zergliedert den Inhalt aus https://www.ultimatespecs.com/de,
    /// um die Hyperlinks zu den Automarken zu sammeln.
    /// </summary>
    public class CarBrandHyperlinksParser : Reusable.WebAccess.IHyperlinksParser
    {
        private static Regex CarBrandHyperlinkRegex { get; } =
            new Regex(@"https://www.ultimatespecs.com/de/car-specs/([\w-]+)-models", RegexOptions.Compiled);

        private readonly Func<string, bool> _shouldParsePage;

        private readonly Func<string, bool> _carBrandFilter;

        /// <summary>
        /// Erstellt eine neue Instanz von <see cref="CarBrandHyperlinksParser"/>.
        /// </summary>
        /// <param name="shouldParsePage">Diese Rückrufaktion wird nur einmal aufgerufen mit dem gesamten Inhalt des Hypertexts und entscheidet, ob der Hypertext überhaupt zergliedert wird.</param>
        /// <param name="carBrandFilter">Diese Rückrufaktion sucht die Namen der Autohersteller aus, deren Hyperlinks zurückgegeben werden.</param>
        public CarBrandHyperlinksParser(Func<string, bool> shouldParsePage,
                                        Func<string, bool> carBrandFilter)
        {
            _shouldParsePage = shouldParsePage;
            _carBrandFilter = carBrandFilter;
        }

        /// <inheritdoc/>
        public IEnumerable<Uri> ParseHyperlinks(string hypertext)
        {
            if (!_shouldParsePage(hypertext))
            {
                return new Uri[]{ };
            }

            var brandsByName = new SortedDictionary<string, Uri>();

            MatchCollection matches = CarBrandHyperlinkRegex.Matches(hypertext);
            foreach (Match match in matches)
            {
                string carBrand = match.Groups[1].Value.ToLower();

                if (_carBrandFilter(carBrand))
                {
                    brandsByName[carBrand] = new Uri(match.Groups[0].Value);
                }
            }

            return brandsByName.Values;
        }
    }
}
