using System;
using System.Collections.Generic;
using System.IO;

namespace IndexBuilder
{
    public class SynonymMap
    {
        public static readonly SynonymMap CitySynonymMap = new SynonymMap(@"Synonyms\City.csv");
        public static readonly SynonymMap StateOrProvinceSynonymMap = new SynonymMap(@"Synonyms\State.csv");
        public static readonly SynonymMap CountrySynonymMap = new SynonymMap(@"Synonyms\Country.csv");
        public static readonly SynonymMap OrganizationSynonymMap = new SynonymMap(@"Synonyms\Organization.csv");

        private readonly Dictionary<string, string> _synonymMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public SynonymMap(string synonymMapFile)
        {
            foreach (string line in File.ReadAllLines(synonymMapFile))
            {
                foreach (string synonym in line.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    _synonymMap.TryAdd(synonym.Trim(), line);
                }
            }
        }

        public bool TryGetSynonyms(string value, out string synonyms) => _synonymMap.TryGetValue(value, out synonyms);
    }
}
