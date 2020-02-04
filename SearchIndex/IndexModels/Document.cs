using Microsoft.Azure.Search;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace IndexModels
{
    public partial class Document
    {
        [Key]
        [IsFilterable]
        [JsonProperty(EntityIdFieldName)]
        public string EntityId { get; set; }

        [IsFilterable, IsFacetable]
        [JsonProperty(EntityNameFieldName)]
        public string EntityName { get; set; }

        //
        // The following fields are indexed for NLU. They are hardcoded in the prototype.
        // In production, a better design would be to define a collection of generic index fields,
        // and use a metadata mapping table to dynamically map the selected fields to the index.
        //
        // CDS entity names plus attribute names are defined as the Azure Search field names:
        //
        // `[JsonProperty("{CDSEntityName}__{CDSAttributeName}")]`
        //

        #region Account

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("account__name")]
        [PrimaryField]
        public string AccountName { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("account__address1_city")]
        public string AccountAddress1City { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("account__address1_stateorprovince")]
        public string AccountAddress1StateOrProvince { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("account__address1_country")]
        public string AccountAddress1Country { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("account__telephone1")]
        public string AccountMainPhone { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("account__emailaddress1")]
        public string AccountEmail { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("account__tickersymbol")]
        public string AccountTickerSymbol { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("account__websiteurl")]
        public string AccountWebSite { get; set; }

        #endregion Account

        #region Opportunity

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("opportunity__name")]
        [PrimaryField]
        public string OpportunityName { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("opportunity__stepname")]
        public string OpportunityPipelinePhase { get; set; }

        #endregion Opportunity

        #region Lead

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("lead__fullname")]
        [PrimaryField]
        public string LeadFullName { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("lead__subject")]
        public string LeadSubject { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("lead__companyname")]
        public string LeadCompanyName { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("lead__address1_city")]
        public string LeadAddress1City { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("lead__address1_stateorprovince")]
        public string LeadAddress1StateOrProvince { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("lead__address1_country")]
        public string LeadAddress1Country { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("lead__telephone1")]
        public string LeadBusinessPhone { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("lead__jobtitle")]
        public string LeadJobTitle { get; set; }

        #endregion Lead

        #region Contact

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("contact__fullname")]
        [PrimaryField]
        public string ContactFullName { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("contact__jobtitle")]
        public string ContactJobTitle { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("contact__telephone1")]
        public string ContactBusinessPhone { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("contact__mobilephone")]
        public string ContactMobilePhone { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("contact__emailaddress1")]
        public string ContactEmail { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("contact__address1_city")]
        public string ContactAddress1City { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("contact__address1_stateorprovince")]
        public string ContactAddress1StateOrProvince { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("contact__address1_country")]
        public string ContactAddress1Country { get; set; }

        #endregion Contact

        #region User

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("user__fullname")]
        [PrimaryField]
        public string UserFullName { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("systemuser__internalemailaddress")]
        public string UserPrimaryEmail { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("systemuser__address1_telephone1")]
        public string UserMainPhone { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        [JsonProperty("systemuser__jobtitle")]
        public string UserJobTitle { get; set; }

        #endregion User
    }
}
