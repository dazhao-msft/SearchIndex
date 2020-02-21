using System;
using System.Collections.Generic;

namespace LanguageModelBuilder
{
    public class LanguageModel
    {
        private readonly Dictionary<string, Dictionary<(string, string), HashSet<Guid>>> _model = new Dictionary<string, Dictionary<(string, string), HashSet<Guid>>>();

        public void Add(string term, string entityName, string attributeName, Guid recordId)
        {
            if (!_model.TryGetValue(term, out var bindings))
            {
                bindings = new Dictionary<(string, string), HashSet<Guid>>();
                _model.Add(term, bindings);
            }

            if (!bindings.TryGetValue((entityName, attributeName), out var recordIds))
            {
                recordIds = new HashSet<Guid>();
                bindings.Add((entityName, attributeName), recordIds);
            }

            recordIds.Add(recordId);
        }

        public IReadOnlyCollection<(string, string)> GetTermBindings(string term)
        {
            if (_model.TryGetValue(term, out var bindings))
            {
                return bindings.Keys;
            }

            return Array.Empty<(string, string)>();
        }

        public IReadOnlyCollection<Guid> GetRecordIds(string term, string entityName, string attributeName)
        {
            if (_model.TryGetValue(term, out var bindings))
            {
                if (bindings.TryGetValue((entityName, attributeName), out var recordIds))
                {
                    return recordIds;
                }
            }

            return Array.Empty<Guid>();
        }
    }
}
