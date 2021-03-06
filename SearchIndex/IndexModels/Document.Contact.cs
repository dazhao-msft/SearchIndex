﻿using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;

namespace IndexModels
{
    public partial class Document
    {
        public const string ContactEntityName = "contact";
        public const string ContactEntityIdName = "contactid";

        [IsSearchable, IsFilterable, IsFacetable, Analyzer(DefaultAnalyzerName)]
        [JsonProperty(ContactEntityName + FieldNameDelimiter + "fullname")]
        [PrimaryField]
        public string ContactFullName { get; set; }

        [IsSearchable, IsFilterable, IsFacetable, Analyzer(DefaultAnalyzerName)]
        [JsonProperty(ContactEntityName + FieldNameDelimiter + "address1_city")]
        public string ContactAddress1City { get; set; }

        [IsSearchable, IsFilterable, IsFacetable, Analyzer(DefaultAnalyzerName)]
        [JsonProperty(ContactEntityName + FieldNameDelimiter + "address1_stateorprovince")]
        public string ContactAddress1StateOrProvince { get; set; }

        [IsSearchable, IsFilterable, IsFacetable, Analyzer(DefaultAnalyzerName)]
        [JsonProperty(ContactEntityName + FieldNameDelimiter + "address1_country")]
        public string ContactAddress1Country { get; set; }

        [IsSearchable, IsFilterable, IsFacetable, Analyzer(DefaultAnalyzerName)]
        [JsonProperty(ContactEntityName + FieldNameDelimiter + "department")]
        public string ContactDepartment { get; set; }

        [IsSearchable, IsFilterable, IsFacetable, Analyzer(DefaultAnalyzerName)]
        [JsonProperty(ContactEntityName + FieldNameDelimiter + "jobtitle")]
        public string ContactJobTitle { get; set; }
    }
}
