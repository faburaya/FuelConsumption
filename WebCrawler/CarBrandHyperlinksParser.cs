using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <inheritdoc/>
        public IEnumerable<Uri> ParseHyperlinks(string hypertext, IEnumerable<string> keywords)
        {
            var brandsByName = new SortedDictionary<string, Uri>();

            keywords = from x in (keywords ?? new string[0]) select x.ToLower();

            MatchCollection matches = CarBrandHyperlinkRegex.Matches(hypertext);
            foreach (Match match in matches)
            {
                string carBrand = match.Groups[1].Value.ToLower();

                if (!keywords.Any() || keywords.Any(keyword => carBrand.Contains(keyword)))
                {
                    brandsByName[carBrand] = new Uri(match.Groups[0].Value);
                }
            }

            return brandsByName.Values;
        }
    }
}
