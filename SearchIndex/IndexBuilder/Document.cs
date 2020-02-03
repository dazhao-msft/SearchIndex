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
        [JsonProperty("account_address1_stateorprovince")]
        public string AccountAddress1StateOrProvince { get; set; }

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

        #region Lead

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("lead_subject")]
        public string LeadSubject { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("lead_companyname")]
        public string LeadCompanyName { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("lead_address1_city")]
        public string LeadAddress1City { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("lead_address1_stateorprovince")]
        public string LeadAddress1StateOrProvince { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("lead_address1_country")]
        public string LeadAddress1Country { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("lead_telephone1")]
        public string LeadTelephone1 { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("lead_jobtitle")]
        public string LeadJobTitle { get; set; }

        #endregion Lead

        #region Contact

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("contact_jobtitle")]
        public string ContactJobTitle { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("contact_telephone1")]
        public string ContactBusinessPhone { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("contact_mobilephone")]
        public string ContactMobilePhone { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("contact_emailaddress1")]
        public string ContactEmail { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("contact_address1_city")]
        public string ContactAddress1City { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("contact_address1_stateorprovince")]
        public string ContactAddress1StateOrProvince { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("contact_address1_country")]
        public string ContactAddress1Country { get; set; }

        #endregion Contact

        #region User

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("systemuser_internalemailaddress")]
        public string UserPrimaryEmail { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("systemuser_address1_telephone1")]
        public string UserMainPhone { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("systemuser_jobtitle")]
        public string UserJobTitle { get; set; }

        #endregion User
    }
}
