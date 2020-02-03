using Microsoft.Azure.Search;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace IndexBuilder
{
    public class Document
    {
        [Key]
        [IsFilterable]
        public string Id { get; set; }

        [IsFilterable]
        public string OrganizationId { get; set; }

        public string RecordAsJson { get; set; }

        [IsFilterable, IsFacetable]
        [JsonProperty("account")]
        public string EntityName { get; set; }

        //
        // The following fields are indexed for NLU. They are hardcoded in this prototype.
        // In production, a better design would be to define a collection of generic index fields,
        // and use a metadata mapping table to dynamically map the selected fields to the index.
        //

        #region Account 

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("name")]
        public string AccountName { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("address1_city")]
        public string AccountAddress1City { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("address1_state")]
        public string AccountAddress1State { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("address1_country")]
        public string AccountAddress1Country { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("telephone1")]
        public string AccountTelephone1 { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("emailaddress1")]
        public string AccountEMailAddress1 { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("tickersymbol")]
        public string AccountTickerSymbol { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("websiteurl")]
        public string AccountWebSite { get; set; }

        #endregion
    }
}
