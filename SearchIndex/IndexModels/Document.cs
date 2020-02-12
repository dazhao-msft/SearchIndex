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

        public const string DefaultAnalyzerName = AnalyzerName.AsString.EnMicrosoft;

        #endregion Analyzers

        #region Scoring profiles

        public const string PrimaryFieldFavoredScoringProfile = nameof(PrimaryFieldFavoredScoringProfile);

        #endregion Scoring profiles

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

        public static ScoringProfile CreatePrimaryFieldFavoredScoringProfile()
        {
            var textWeights = new Dictionary<string, double>();

            var properties = typeof(Document).GetProperties().Where(p => p.GetCustomAttribute<IsSearchableAttribute>() != null);

            foreach (var property in properties)
            {
                string fieldName = property.GetCustomAttribute<JsonPropertyAttribute>().PropertyName;

                bool isPrimaryField = property.GetCustomAttribute<PrimaryFieldAttribute>() != null;

                textWeights.Add(fieldName, isPrimaryField ? 10.0 : 1.0);
            }

            return new ScoringProfile() { Name = PrimaryFieldFavoredScoringProfile, TextWeights = new TextWeights(textWeights) };
        }

        #endregion Helpers
    }
}
