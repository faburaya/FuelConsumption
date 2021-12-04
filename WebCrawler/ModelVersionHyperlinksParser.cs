using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FuelConsumption.WebCrawler
{
    /// <summary>
    /// Zergliedert den Inhalt aus https://www.ultimatespecs.com/de/car-specs/
    /// bezüglich auf die Versionen eines bestimmten Automodells.
    /// </summary>
    public class ModelVersionHyperlinksParser : Reusable.WebAccess.IHyperlinksParser
    {
        private static Regex ModelVersionHyperlinkRegex { get; } =
            new Regex(@"/de/car-specs/[\w-]+/M\d+/([\w-]+)", RegexOptions.Compiled);

        private readonly Func<string, bool> _shouldParsePage;

        private readonly Func<string, bool> _modelVersionFilter;

        /// <summary>
        /// Erstellet eine neue Instanz von <see cref="CarModelHyperlinksParser"/>.
        /// </summary>
        /// <param name="shouldParsePage">Diese Rückrufaktion wird nur einmal aufgerufen mit dem gesamten Inhalt des Hypertexts und entscheidet, ob der Hypertext überhaupt zergliedert wird.</param>
        /// <param name="modelVersionFilter">Diese Rückrufaktion sucht die Versionen eines Automodells aus, deren Hyperlinks zurückgegeben werden.</param>
        public ModelVersionHyperlinksParser(Func<string, bool> shouldParsePage,
                                            Func<string, bool> modelVersionFilter)
        {
            _shouldParsePage = shouldParsePage;
            _modelVersionFilter = modelVersionFilter;
        }

        /// <inheritdoc/>
        public IEnumerable<Uri> ParseHyperlinks(string hypertext)
        {
            if (!_shouldParsePage(hypertext))
            {
                return new Uri[] { };
            }

            var versionsByLabel = new SortedDictionary<string, Uri>();

            MatchCollection matches = ModelVersionHyperlinkRegex.Matches(hypertext);
            foreach (Match match in matches)
            {
                string versionLabel = match.Groups[1].Value.ToLower();

                if (_modelVersionFilter(versionLabel))
                {
                    string href = match.Groups[0].Value;
                    if (!href.StartsWith("http"))
                    {
                        versionsByLabel[versionLabel] = new Uri("https://www.ultimatespecs.com" + href);
                    }
                    else
                    {
                        versionsByLabel[versionLabel] = new Uri(href);
                    }
                }
            }

            return versionsByLabel.Values;
        }
    }
}
