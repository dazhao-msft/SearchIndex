using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;

namespace IndexModels
{
    public partial class Document
    {
        public const string TerritoryEntityName = "territory";
        public const string TerritoryEntityIdName = "territoryid";

        [IsSearchable, IsFilterable, IsFacetable, Analyzer(DefaultAnalyzerName)]
        [JsonProperty(TerritoryEntityName + FieldNameDelimiter + "name")]
        [PrimaryField]
        public string TerritoryName { get; set; }
    }
}
