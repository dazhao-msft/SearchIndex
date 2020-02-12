using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;

namespace IndexModels
{
    public partial class Document
    {
        public const string LeadEntityName = "lead";
        public const string LeadEntityIdName = "leadid";

        [IsSearchable, IsFilterable, IsFacetable, Analyzer(DefaultAnalyzerName)]
        [JsonProperty(LeadEntityName + FieldNameDelimiter + "fullname")]
        [PrimaryField]
        public string LeadFullName { get; set; }

        [IsSearchable, IsFilterable, IsFacetable, Analyzer(DefaultAnalyzerName)]
        [JsonProperty(LeadEntityName + FieldNameDelimiter + "subject")]
        public string LeadSubject { get; set; }

        [IsSearchable, IsFilterable, IsFacetable, Analyzer(DefaultAnalyzerName)]
        [JsonProperty(LeadEntityName + FieldNameDelimiter + "companyname")]
        public string LeadCompanyName { get; set; }

        [IsSearchable, IsFilterable, IsFacetable, Analyzer(DefaultAnalyzerName)]
        [JsonProperty(LeadEntityName + FieldNameDelimiter + "address1_city")]
        public string LeadAddress1City { get; set; }

        [IsSearchable, IsFilterable, IsFacetable, Analyzer(DefaultAnalyzerName)]
        [JsonProperty(LeadEntityName + FieldNameDelimiter + "address1_stateorprovince")]
        public string LeadAddress1StateOrProvince { get; set; }

        [IsSearchable, IsFilterable, IsFacetable, Analyzer(DefaultAnalyzerName)]
        [JsonProperty(LeadEntityName + FieldNameDelimiter + "address1_country")]
        public string LeadAddress1Country { get; set; }

        //[IsSearchable, IsFilterable, IsFacetable, Analyzer(DefaultAnalyzerName)]
        //[JsonProperty(LeadEntityName + FieldNameDelimiter + "telephone1")]
        //public string LeadBusinessPhone { get; set; }

        //[IsSearchable, IsFilterable, IsFacetable, Analyzer(DefaultAnalyzerName)]
        //[JsonProperty(LeadEntityName + FieldNameDelimiter + "jobtitle")]
        //public string LeadJobTitle { get; set; }
    }
}
