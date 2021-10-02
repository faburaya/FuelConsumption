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
            new Regex(@"https://www.ultimatespecs.com/de/car-specs/(?<brand>[\w-]+)/\d+/\k<brand>-(?<version>[\w-]+).html", RegexOptions.Compiled);

        /// <inheritdoc/>
        public IEnumerable<Uri> ParseHyperlinks(string hypertext, IEnumerable<string> keywords)
        {
            var trimLevels = new SortedSet<string>();

            keywords = from x in (keywords ?? new string[0]) select x.ToLower();

            MatchCollection matches = TrimLevelHyperlinkRegex.Matches(hypertext);
            foreach (Match match in matches)
            {
                string url = match.Groups[0].Value.ToLower();

                if (!keywords.Any() || keywords.Any(keyword => url.Contains(keyword)))
                {
                    trimLevels.Add(url);
                }
            }

            return from address in trimLevels select new Uri(address);
        }
    }
}
