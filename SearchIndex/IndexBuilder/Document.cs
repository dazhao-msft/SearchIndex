using Microsoft.Azure.Search;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IndexBuilder
{
    public class Document
    {
        [Key]
        [IsFilterable]
        public string Id { get; set; }

        [IsFilterable]
        public string OrganizationId { get; set; }

        [IsFilterable]
        [IsFacetable]
        public string EntityName { get; set; }

        public string RecordAsJson { get; set; }

        #region

        [IsSearchable]
        [IsFacetable]
        [IsFilterable]
        public string AccountName { get; set; }

        [IsSearchable]
        [IsFacetable]
        [IsFilterable]
        public string AccountCity { get; set; }

        #endregion
    }
}
