using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;

namespace IndexModels
{
    public partial class Document
    {
        public const string UserEntityName = "systemuser";
        public const string UserEntityIdName = "systemuserid";

        [IsSearchable, IsFilterable, IsFacetable, Analyzer(AnalyzerName.AsString.EnMicrosoft)]
        [JsonProperty(UserEntityName + FieldNameDelimiter + "fullname")]
        [PrimaryField]
        public string UserFullName { get; set; }

        [IsSearchable, IsFilterable, IsFacetable, Analyzer(AnalyzerName.AsString.EnMicrosoft)]
        [JsonProperty(UserEntityName + FieldNameDelimiter + "internalemailaddress")]
        public string UserPrimaryEmail { get; set; }

        [IsSearchable, IsFilterable, IsFacetable, Analyzer(AnalyzerName.AsString.EnMicrosoft)]
        [JsonProperty(UserEntityName + FieldNameDelimiter + "address1_telephone1")]
        public string UserMainPhone { get; set; }

        [IsSearchable, IsFilterable, IsFacetable, Analyzer(AnalyzerName.AsString.EnMicrosoft)]
        [JsonProperty(UserEntityName + FieldNameDelimiter + "jobtitle")]
        public string UserJobTitle { get; set; }
    }
}
