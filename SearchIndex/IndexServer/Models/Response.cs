using System;
using System.Collections.Generic;
using System.Linq;

namespace IndexServer.Models
{
    public class MatchedTerm : IEquatable<MatchedTerm>
    {
        /// <summary>
        /// Matched text from the original query.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Location index of the first character of the term.
        /// </summary>
        public int StartIndex { get; set; }

        /// <summary>
        /// Length of the term.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// List of TermBindings which contain the term.
        /// </summary>
        public ISet<TermBinding> TermBindings { get; set; }

        public bool Equals(MatchedTerm other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (string.IsNullOrEmpty(Text) && !string.IsNullOrEmpty(other.Text))
            {
                return false;
            }

            if (string.IsNullOrEmpty(Text) || !Text.Equals(other.Text, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (StartIndex != other.StartIndex || Length != other.Length)
            {
                return false;
            }

            if (TermBindings == null || other.TermBindings == null || TermBindings.Count != other.TermBindings.Count)
            {
                return false;
            }

            if (TermBindings.Except(other.TermBindings).Any() || other.TermBindings.Except(TermBindings).Any())
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MatchedTerm);
        }

        public override string ToString()
        {
            string stringValue = string.IsNullOrEmpty(Text) ? string.Empty : Text + StartIndex + Length;
            foreach (TermBinding termBinding in TermBindings)
            {
                stringValue += termBinding.ToString();
            }

            return stringValue;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = (hash * 23) + (string.IsNullOrEmpty(Text) ? 0 : StringComparer.OrdinalIgnoreCase.GetHashCode(Text));
                hash = (hash * 23) + StartIndex.GetHashCode();
                hash = (hash * 23) + Length.GetHashCode();

                if (TermBindings != null)
                {
                    foreach (var termBinding in TermBindings)
                    {
                        hash = (hash * 23) + termBinding.GetHashCode();
                    }
                }

                return hash;
            }
        }
    }

    public enum BindingType
    {
        Table,
        Column,
        InstanceValue,
    }

    public class TermBinding : IEquatable<TermBinding>
    {
        /// <summary>
        /// The type of this binding.
        /// Can be table name, column name, or instance value.
        /// </summary>
        public BindingType BindingType { get; set; }

        /// <summary>
        /// Database table and column in which the term exist.
        /// </summary>
        public SearchScope SearchScope { get; set; }

        /// <summary>
        /// Indicate a synonym match.
        /// </summary>
        public bool IsSynonymMatch { get; set; }

        /// <summary>
        /// Indicate a value return is exactly match.
        /// </summary>
        public bool IsExactlyMatch { get; set; } = true;

        /// <summary>
        /// Database cell value.
        /// </summary>
        public string Value { get; set; }

        public bool Equals(TermBinding other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if ((Value == null && other.Value != null) && !Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if ((SearchScope.Table == null && other.SearchScope.Table != null) && !SearchScope.Table.Equals(other.SearchScope.Table, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if ((SearchScope.Column == null && other.SearchScope.Column != null) && !SearchScope.Column.Equals(other.SearchScope.Column, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return BindingType == other.BindingType;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TermBinding);
        }

        public override string ToString()
        {
            return BindingType.ToString() + string.IsNullOrEmpty(Value) ?? string.Empty + string.IsNullOrEmpty(SearchScope.Table) ?? string.Empty + string.IsNullOrEmpty(SearchScope.Column) ?? string.Empty;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = (hash * 23) + BindingType.GetHashCode();
                hash = (hash * 23) + (SearchScope != null ? SearchScope.GetHashCode() : 0);
                hash = (hash * 23) + IsSynonymMatch.GetHashCode();
                hash = (hash * 23) + (string.IsNullOrEmpty(Value) ? 0 : StringComparer.OrdinalIgnoreCase.GetHashCode(Value));
                return hash;
            }
        }
    }
}
