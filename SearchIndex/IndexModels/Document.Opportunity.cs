using Microsoft.Azure.Search;
using Newtonsoft.Json;

namespace IndexModels
{
    public partial class Document
    {
        public const string OpportunityEntityName = "opportunity";
        public const string OpportunityEntityIdName = "opportunityid";

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty(OpportunityEntityName + FieldNameDelimiter + "name")]
        [PrimaryField]
        public string OpportunityName { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty(OpportunityEntityName + FieldNameDelimiter + "stepname")]
        public string OpportunityPipelinePhase { get; set; }
    }
}
