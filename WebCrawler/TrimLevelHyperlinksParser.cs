using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FuelConsumption.WebCrawler
{
    /// <summary>
    /// Zergliedert den Inhalt aus https://www.ultimatespecs.com/de/car-specs/
    /// bezüglich auf die Trimmstuffe einer bestimmten Version eines Automodells.
    /// </summary>
    public class TrimLevelHyperlinksParser : Reusable.WebAccess.IHyperlinksParser
    {
        private static Regex TrimLevelHyperlinkRegex { get; } =
            new Regex(@"/de/car-specs/(?<brand>[\w-]+)/\d+/\k<brand>-(?<level>[\w-]+).html", RegexOptions.Compiled);

        private readonly Func<string, bool> _shouldParsePage;

        private readonly Func<string, bool> _trimLevelFilter;

        /// <summary>
        /// Erstellet eine neue Instanz von <see cref="CarModelHyperlinksParser"/>.
        /// </summary>
        /// <param name="shouldParsePage">Diese Rückrufaktion wird nur einmal aufgerufen mit dem gesamten Inhalt des Hypertexts und entscheidet, ob der Hypertext überhaupt zergliedert wird.</param>
        /// <param name="trimLevelFilter">Diese Rückrufaktion sucht die Niveaus einer Version eines Automodells aus, deren Hyperlinks zurückgegeben werden.</param>
        public TrimLevelHyperlinksParser(Func<string, bool> shouldParsePage,
                                         Func<string, bool> trimLevelFilter)
        {
            _shouldParsePage = shouldParsePage;
            _trimLevelFilter = trimLevelFilter;
        }

        /// <inheritdoc/>
        public IEnumerable<Uri> ParseHyperlinks(string hypertext)
        {
            if (!_shouldParsePage(hypertext))
            {
                return new Uri[] { };
            }

            var trimLevels = new SortedSet<string>();

            MatchCollection matches = TrimLevelHyperlinkRegex.Matches(hypertext);
            foreach (Match match in matches)
            {
                string trimLevel = match.Groups["level"].Value.ToLower();

                if (_trimLevelFilter(trimLevel))
                {
                    string href = match.Groups[0].Value;
                    string url =
                        (href.StartsWith("http") ? href : "https://www.ultimatespecs.com" + href);

                    trimLevels.Add(url);
                }
            }

            return from address in trimLevels select new Uri(address);
        }
    }
}
