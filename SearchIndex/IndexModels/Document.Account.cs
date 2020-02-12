using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;

namespace IndexModels
{
    public partial class Document
    {
        public const string AccountEntityName = "account";
        public const string AccountEntityIdName = "accountid";

        [IsSearchable, IsFilterable, IsFacetable, Analyzer(DefaultAnalyzerName)]
        [JsonProperty(AccountEntityName + FieldNameDelimiter + "name")]
        [PrimaryField]
        public string AccountName { get; set; }

        [IsSearchable, IsFilterable, IsFacetable, Analyzer(DefaultAnalyzerName)]
        [JsonProperty(AccountEntityName + FieldNameDelimiter + "address1_city")]
        public string AccountAddress1City { get; set; }

        [IsSearchable, IsFilterable, IsFacetable, Analyzer(DefaultAnalyzerName)]
        [JsonProperty(AccountEntityName + FieldNameDelimiter + "address1_stateorprovince")]
        public string AccountAddress1StateOrProvince { get; set; }

        [IsSearchable, IsFilterable, IsFacetable, Analyzer(DefaultAnalyzerName)]
        [JsonProperty(AccountEntityName + FieldNameDelimiter + "address1_country")]
        public string AccountAddress1Country { get; set; }

        //[IsSearchable, IsFilterable, IsFacetable, Analyzer(DefaultAnalyzerName)]
        //[JsonProperty(AccountEntityName + FieldNameDelimiter + "telephone1")]
        //public string AccountMainPhone { get; set; }

        //[IsSearchable, IsFilterable, IsFacetable, Analyzer(DefaultAnalyzerName)]
        //[JsonProperty(AccountEntityName + FieldNameDelimiter + "emailaddress1")]
        //public string AccountEmail { get; set; }

        //[IsSearchable, IsFilterable, IsFacetable, Analyzer(DefaultAnalyzerName)]
        //[JsonProperty(AccountEntityName + FieldNameDelimiter + "tickersymbol")]
        //public string AccountTickerSymbol { get; set; }

        //[IsSearchable, IsFilterable, IsFacetable, Analyzer(DefaultAnalyzerName)]
        //[JsonProperty(AccountEntityName + FieldNameDelimiter + "websiteurl")]
        //public string AccountWebSite { get; set; }
    }
}
