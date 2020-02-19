using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;

namespace IndexModels
{
    public partial class Document
    {
        public const string BusinessUnitEntityName = "businessunit";
        public const string BusinessUnitEntityIdName = "businessunitid";

        [IsSearchable, IsFilterable, IsFacetable, Analyzer(DefaultAnalyzerName)]
        [JsonProperty(BusinessUnitEntityName + FieldNameDelimiter + "name")]
        [PrimaryField]
        public string BusinessUnitName { get; set; }
    }
}
