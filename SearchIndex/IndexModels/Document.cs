using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace IndexModels
{
    public partial class Document
    {
        #region Analyzers

        public const string DefaultAnalyzerName = AnalyzerName.AsString.StandardLucene;

        public const string BizQADefaultAnalyzerName = "bizqa-default-analyzer";

        public static readonly CustomAnalyzer BizQADefaultAnalyzer = new CustomAnalyzer()
        {
            Name = BizQADefaultAnalyzerName,
            Tokenizer = TokenizerName.Standard,
        };

        public const string BizQAUaxUrlEmailAnalyzerName = "bizqa-uax-url-email-analyzer";

        public static readonly CustomAnalyzer BizQAUaxUrlEmailAnalyzer = new CustomAnalyzer()
        {
            Name = BizQAUaxUrlEmailAnalyzerName,
            Tokenizer = TokenizerName.UaxUrlEmail,
            TokenFilters = new[] { TokenFilterName.Lowercase },
        };

        #endregion Analyzers

        #region Common fields

        public const string FieldNameDelimiter = "__";

        public const string EntityIdFieldName = "entity_id";
        public const string EntityNameFieldName = "entity_name";

        [Key]
        [IsFilterable]
        [JsonProperty(EntityIdFieldName)]
        public string EntityId { get; set; }

        [IsFilterable, IsFacetable]
        [JsonProperty(EntityNameFieldName)]
        public string EntityName { get; set; }

        #endregion Common fields

        #region Helpers

        public static readonly string[] SearchableFields = typeof(Document).GetProperties()
                                                                           .Where(p => p.GetCustomAttribute<IsSearchableAttribute>() != null)
                                                                           .Select(p => p.GetCustomAttribute<JsonPropertyAttribute>().PropertyName)
                                                                           .ToArray();

        public static bool TryResolveCdsAttributeName(string fieldName, string cdsEntityName, out string cdsAttributeName)
        {
            string cdsEntityNameWithDelimiter = cdsEntityName + FieldNameDelimiter;

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
                string fieldName = property.GetCustomAttribute<JsonPropertyAttribute>().PropertyName;

                if (TryResolveCdsAttributeName(fieldName, cdsEntityName, out string cdsAttributeName))
                {
                    result.Add((property, cdsAttributeName));
                }
            }

            return result;
        }

        #endregion Helpers
    }
}
