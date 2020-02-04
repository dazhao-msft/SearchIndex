using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IndexServer.Models
{
    public class DataIndexSearchRequest
    {
        /// <summary>
        /// The natural language query to search index for.
        /// </summary>
        [Required]
        public string Query { get; set; }

        /// <summary>
        /// Options for the search.
        /// </summary>
        public SearchOption Option { get; set; }

        public string OrganizationId { get; set; }
    }

    public class SearchOption
    {
        /// <summary>
        /// How many results to retain.
        /// </summary>
        public int RetainedSize { get; set; } = 5;

        /// <summary>
        /// Is the query single term, i.e. not to be tokenized.
        /// </summary>
        public bool IsSingleTerm { get; set; } = false;

        /// <summary>
        /// Does the query require synonym match.
        /// </summary>
        public bool SynonymMatch { get; set; } = true;

        /// <summary>
        /// Which criteria to search against.
        /// </summary>
        public MatchCriterias MatchCriterias { get; set; } = MatchCriterias.ExactlyMatch;

        /// <summary>
        /// Which scope to search against.
        /// </summary>
        public IEnumerable<SearchScope> SearchScopes { get; set; }
    }

    [Flags]
    public enum MatchCriterias
    {
        None = 0,
        ExactlyMatch = 1 << 0,
        StartsWith = 1 << 1,
        Contains = 1 << 2,
        EndsWith = 1 << 3,
    }

    public class SearchScope : IEquatable<SearchScope>
    {
        /// <summary>
        /// Database table to look for.
        /// </summary>
        public string Table { get; set; }

        /// <summary>
        /// Database table's column to look for.
        /// </summary>
        public string Column { get; set; }

        public bool Equals(SearchScope other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (string.IsNullOrEmpty(Table) && !string.IsNullOrEmpty(other.Table))
            {
                return false;
            }

            if (string.IsNullOrEmpty(Table) || !Table.Equals(other.Table, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (string.IsNullOrEmpty(Column) && !string.IsNullOrEmpty(other.Column))
            {
                return false;
            }

            if (string.IsNullOrEmpty(Column) || !Table.Equals(other.Column, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SearchScope);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = (hash * 23) + (string.IsNullOrEmpty(Table) ? 0 : StringComparer.OrdinalIgnoreCase.GetHashCode(Table));
                hash = (hash * 23) + (string.IsNullOrEmpty(Column) ? 0 : StringComparer.OrdinalIgnoreCase.GetHashCode(Column));

                return hash;
            }
        }

        public override string ToString()
        {
            return $"{Table ?? string.Empty}_{Column ?? string.Empty}";
        }
    }
}
