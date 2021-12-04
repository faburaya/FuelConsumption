using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FuelConsumption.WebCrawler
{
    /// <summary>
    /// Zeglidert den Inhalt aus https://www.ultimatespecs.com/de/car-specs/*-models,
    /// um die Hyperlinks zu den Automodellen zu sammeln.
    /// </summary>
    public class CarModelHyperlinksParser : Reusable.WebAccess.IHyperlinksParser
    {
        private static Regex[] CarModelHyperlinkRegexes { get; } = new[] {
            new Regex(@"/de/car-specs/(?<brand>[\w-]+)-models/\k<brand>-(?<model>[\w-]+)", RegexOptions.Compiled),
            new Regex(@"/de/car-specs/[\w-]+/M\d+/(?<model>[\w-]+)", RegexOptions.Compiled),
        };

        private readonly Func<string, bool> _shouldParsePage;
        
        private readonly Func<string, bool> _carModelFilter;

        /// <summary>
        /// Erstellet eine neue Instanz von <see cref="CarModelHyperlinksParser"/>.
        /// </summary>
        /// <param name="shouldParsePage">Diese Rückrufaktion wird nur einmal aufgerufen mit dem gesamten Inhalt des Hypertexts und entscheidet, ob der Hypertext überhaupt zergliedert wird.</param>
        /// <param name="carModelFilter">Diese Rückrufaktion sucht die Namen der Automodelle aus, deren Hyperlinks zurückgegeben werden.</param>
        public CarModelHyperlinksParser(Func<string, bool> shouldParsePage,
                                        Func<string, bool> carModelFilter)
        {
            _shouldParsePage = shouldParsePage;
            _carModelFilter = carModelFilter;
        }

        /// <inheritdoc/>
        public IEnumerable<Uri> ParseHyperlinks(string hypertext)
        {
            if (!_shouldParsePage(hypertext))
            {
                return new Uri[] { };
            }

            var modelsByName = new SortedDictionary<string, Uri>();

            foreach (Regex regex in CarModelHyperlinkRegexes)
            {
                MatchCollection matches = regex.Matches(hypertext);
                foreach (Match match in matches)
                {
                    string carModel = match.Groups["model"].Value.ToLower();

                    if (_carModelFilter(carModel))
                    {
                        Uri url;
                        string href = match.Groups[0].Value;
                        if (!href.StartsWith("http"))
                        {
                            url = new Uri("https://www.ultimatespecs.com" + href);
                        }
                        else
                        {
                            url = new Uri(href);
                        }

                        modelsByName[carModel] = url;
                    }
                }
            }

            return modelsByName.Values;
        }
    }
}
