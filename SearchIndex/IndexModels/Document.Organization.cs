using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;

namespace IndexModels
{
    public partial class Document
    {
        public const string OrganizationEntityName = "organization";
        public const string OrganizationEntityIdName = "organizationid";

        [IsSearchable, IsFilterable, IsFacetable, Analyzer(DefaultAnalyzerName)]
        [JsonProperty(OrganizationEntityName + FieldNameDelimiter + "name")]
        [PrimaryField]
        public string OrganizationName { get; set; }
    }
}
