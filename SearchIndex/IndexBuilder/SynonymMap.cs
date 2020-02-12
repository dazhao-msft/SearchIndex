using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IndexBuilder
{
    public class SynonymMap
    {
        public static readonly SynonymMap CitySynonymMap = new SynonymMap(@"Synonyms\City.csv");
        public static readonly SynonymMap StateOrProvinceSynonymMap = new SynonymMap(@"Synonyms\State.csv");
        public static readonly SynonymMap CountrySynonymMap = new SynonymMap(@"Synonyms\Country.csv");
        public static readonly SynonymMap OrganizationSynonymMap = new SynonymMap(@"Synonyms\Organization.csv");

        private readonly Dictionary<string, HashSet<string>> _synonymMap = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

        public SynonymMap(string synonymMapFile)
        {
            var synonymSetList = new List<HashSet<string>>();

            foreach (string line in File.ReadAllLines(synonymMapFile))
            {
                synonymSetList.Add(new HashSet<string>(line.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()), StringComparer.OrdinalIgnoreCase));
            }

            foreach (var synonymSet in synonymSetList)
            {
                foreach (string synonym in synonymSet)
                {
                    if (!_synonymMap.ContainsKey(synonym))
                    {
                        _synonymMap[synonym] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    }

                    _synonymMap[synonym].UnionWith(synonymSet);
                }
            }

            foreach (var kvp in _synonymMap)
            {
                kvp.Value.Remove(kvp.Key);
            }
        }

        public bool TryGetSynonyms(string value, out string synonyms)
        {
            synonyms = null;

            if (_synonymMap.TryGetValue(value, out var synonymSet))
            {
                synonyms = string.Join(',', synonymSet);
                return true;
            }

            return false;
        }
    }
}
