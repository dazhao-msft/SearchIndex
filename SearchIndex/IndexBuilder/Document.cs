using Microsoft.Azure.Search;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace IndexBuilder
{
    public class Document
    {
        [Key]
        [IsFilterable]
        [JsonProperty("id")]
        public string EntityId { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("primary_field")]
        public string EntityPrimaryField { get; set; }

        [IsFilterable, IsFacetable]
        [JsonProperty("type")]
        public string EntityType { get; set; }

        [JsonProperty("entity")]
        public string EntityAsJson { get; set; }

        //
        // The following fields are indexed for NLU. They are hardcoded in the prototype.
        // In production, a better design would be to define a collection of generic index fields,
        // and use a metadata mapping table to dynamically map the selected fields to the index.
        //
        // CDS entity names plus attribute names are defined as the property name in the following properties:
        // `[JsonProperty("{CDSEntityName}_{CDSAttributeName}")]`
        //

        #region Account 

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("account_address1_city")]
        public string AccountAddress1City { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("account_address1_state")]
        public string AccountAddress1State { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("account_address1_country")]
        public string AccountAddress1Country { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("account_telephone1")]
        public string AccountTelephone1 { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("account_emailaddress1")]
        public string AccountEmailAddress1 { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("account_tickersymbol")]
        public string AccountTickerSymbol { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("account_websiteurl")]
        public string AccountWebSite { get; set; }

        #endregion Account

        #region Opportunity 

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("opportunity_stepname")]
        public string OpportunityStepName { get; set; }

        #endregion Opportunity
    }
}
