//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.BizQA.NLU.Common
{
    public static class StringExtensions
    {
        private static readonly Regex s_metadataPattern = new Regex(@"\{\{(?<param>[\w\._\d]+)\}\}");

        /// <summary>
        /// Compares two strings using the ordinal ignore case comparison.
        /// This is a utility method to reduce the code repetition.
        /// </summary>
        /// <param name="s1">First string to compare.</param>
        /// <param name="s2">Second string to compare.</param>
        /// <returns>True if two strings are equal.</returns>
        public static bool OrdinalEquals(this string s1, string s2)
        {
            return string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns the number of placeholders in a template strings.
        /// </summary>
        public static int GetNumberOfTokens(this string template)
        {
            return s_metadataPattern.Matches(template).Count;
        }

        /// <summary>
        /// Check if two strings are equal if not considering special chars.
        /// </summary>
        /// <param name="s1">First string to compare.</param>
        /// <param name="s2">Second string to compare.</param>
        /// <returns>Check result.</returns>
        public static bool EqualsIgnoreSpecialChars(this string s1, string s2)
        {
            if (object.ReferenceEquals(s1, s2))
            {
                return true;
            }

            if (s1 == null || s2 == null)
            {
                return false;
            }

            int i = 0, j = 0;
            int len1 = s1.Length, len2 = s2.Length;
            while (i < len1 && j < len2)
            {
                if (IsSpecialChars(s1[i]))
                {
                    i++;
                }
                else if (IsSpecialChars(s2[j]))
                {
                    j++;
                }
                else
                {
                    if (!char.ToLower(s1[i], CultureInfo.CurrentCulture).Equals(char.ToLower(s2[j], CultureInfo.CurrentCulture)))
                    {
                        return false;
                    }

                    i++;
                    j++;
                }
            }

            while (i < len1)
            {
                if (!IsSpecialChars(s1[i]))
                {
                    break;
                }

                i++;
            }

            while (j < len2)
            {
                if (!IsSpecialChars(s2[j]))
                {
                    break;
                }

                j++;
            }

            return i == len1 && j == len2;
        }

        /// <summary>
        /// Check if one string contains another string if ignore special characters.
        /// Maybe improve by DP or KMP.
        /// </summary>
        /// <param name="s1">First string to compare.</param>
        /// <param name="s2">Second string to compare.</param>
        /// <returns>True if the first string contains the second string if ignore special characters.</returns>
        public static bool ContainsIgnoreSpecialChars(this string s1, string s2)
        {
            if (object.ReferenceEquals(s1, s2))
            {
                return true;
            }

            if (s1 == null)
            {
                return false;
            }

            if (s2 == null)
            {
                return true;
            }

            int len1 = s1.Length, len2 = s2.Length;
            for (int k = 0; k < len1; k++)
            {
                if (IsSpecialChars(s1[k]))
                {
                    continue;
                }

                int i = k, j = 0;
                bool isEqual = true;
                while (i < len1 && j < len2)
                {
                    if (IsSpecialChars(s1[i]))
                    {
                        i++;
                    }
                    else if (IsSpecialChars(s2[j]))
                    {
                        j++;
                    }
                    else
                    {
                        if (!char.ToLower(s1[i], CultureInfo.CurrentCulture).Equals(char.ToLower(s2[j], CultureInfo.CurrentCulture)))
                        {
                            isEqual = false;
                            break;
                        }

                        i++;
                        j++;
                    }
                }

                if (!isEqual)
                {
                    continue;
                }

                while (j < len2)
                {
                    if (!IsSpecialChars(s2[j]))
                    {
                        break;
                    }

                    j++;
                }

                return j == len2;
            }

            return false;
        }

        private static bool IsSpecialChars(char c)
        {
            return char.IsPunctuation(c) || char.IsWhiteSpace(c);
        }
    }
}
