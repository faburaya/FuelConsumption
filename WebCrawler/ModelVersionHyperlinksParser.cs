using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FuelConsumption.WebCrawler
{
    /// <summary>
    /// Zergliedert den Inhalt aus https://www.ultimatespecs.com/de/car-specs/
    /// bezüglich auf die Versionen eines bestimmten Automodells.
    /// </summary>
    public class ModelVersionHyperlinksParser : Reusable.WebAccess.IHyperlinksParser
    {
        private Regex ModelVersionHyperlinkRegex { get; }

        public ModelVersionHyperlinksParser()
        {
            ModelVersionHyperlinkRegex = new Regex(
                @"https://www.ultimatespecs.com/de/car-specs/[\w-]+/M\d+/([\w-]+)",
                RegexOptions.Compiled);
        }

        /// <inheritdoc/>
        public IEnumerable<Uri> ParseHyperlinks(string hypertext, IEnumerable<string> keywords)
        {
            var versionsByLabel = new SortedDictionary<string, Uri>();

            keywords = from x in (keywords ?? new string[0]) select x.ToLower();

            MatchCollection matches = ModelVersionHyperlinkRegex.Matches(hypertext);
            foreach (Match match in matches)
            {
                string versionLabel = match.Groups[1].Value.ToLower();

                if (!keywords.Any() || keywords.Any(keyword => versionLabel.Contains(keyword)))
                {
                    versionsByLabel[versionLabel] = new Uri(match.Groups[0].Value);
                }
            }

            return versionsByLabel.Values;
        }
    }
}
