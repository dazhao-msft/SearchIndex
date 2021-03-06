﻿using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;

namespace IndexModels
{
    public partial class Document
    {
        public const string UserEntityName = "systemuser";
        public const string UserEntityIdName = "systemuserid";

        [IsSearchable, IsFilterable, IsFacetable, Analyzer(DefaultAnalyzerName)]
        [JsonProperty(UserEntityName + FieldNameDelimiter + "fullname")]
        [PrimaryField]
        public string UserFullName { get; set; }
    }
}
