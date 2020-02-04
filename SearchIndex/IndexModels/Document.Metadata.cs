using Microsoft.Azure.Search;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IndexModels
{
    public partial class Document
    {
        public const string EntityIdFieldName = "entity_id";

        public const string EntityNameFieldName = "entity_name";

        public static readonly string[] SearchableFields = typeof(Document).GetProperties()
                                                                           .Where(p => p.GetCustomAttribute<IsSearchableAttribute>() != null)
                                                                           .Select(p => p.GetCustomAttribute<JsonPropertyAttribute>().PropertyName)
                                                                           .ToArray();

        public static bool TryResolveCdsAttributeName(string fieldName, string cdsEntityName, out string cdsAttributeName)
        {
            string cdsEntityNameWithDelimiter = $"{cdsEntityName}__";

            if (!fieldName.StartsWith(cdsEntityNameWithDelimiter, StringComparison.Ordinal))
            {
                cdsAttributeName = null;
                return false;
            }
            else
            {
                cdsAttributeName = fieldName.Substring(cdsEntityNameWithDelimiter.Length);
                return true;
            }
        }

        public static IReadOnlyList<(PropertyInfo PropertyInfo, string CdsAttributeName)> GetPropertiesToIndex(string cdsEntityName)
        {
            var result = new List<(PropertyInfo, string)>();

            var properties = typeof(Document).GetProperties().Where(p => p.GetCustomAttribute<IsSearchableAttribute>() != null);

            foreach (var property in properties)
            {
                if (TryResolveCdsAttributeName(property.GetCustomAttribute<JsonPropertyAttribute>().PropertyName, cdsEntityName, out string cdsAttributeName))
                {
                    result.Add((property, cdsAttributeName));
                }
            }

            return result;
        }
    }
}
