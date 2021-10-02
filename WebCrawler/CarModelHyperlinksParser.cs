using System;
using System.Collections.Generic;
using System.Linq;
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
            new Regex(@"https://www.ultimatespecs.com/de/car-specs/(?<brand>[\w-]+)-models/\k<brand>-(?<model>[\w-]+)",           RegexOptions.Compiled),
            new Regex(@"https://www.ultimatespecs.com/de/car-specs/[\w-]+/M\d+/(?<model>[\w-]+)",
                      RegexOptions.Compiled),
        };

        /// <inheritdoc/>
        public IEnumerable<Uri> ParseHyperlinks(string hypertext, IEnumerable<string> keywords)
        {
            var modelsByName = new SortedDictionary<string, Uri>();

            keywords = from x in (keywords ?? new string[0]) select x.ToLower();

            foreach (Regex regex in CarModelHyperlinkRegexes)
            {
                MatchCollection matches = regex.Matches(hypertext);
                foreach (Match match in matches)
                {
                    string carModel = match.Groups["model"].Value.ToLower();

                    if (!keywords.Any() || keywords.Any(keyword => carModel.Contains(keyword)))
                    {
                        modelsByName[carModel] = new Uri(match.Groups[0].Value);
                    }
                }
            }

            return modelsByName.Values;
        }
    }
}
