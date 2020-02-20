//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Microsoft.BizQA.NLU.Common
{
    public static class CommonFileLoader
    {
        private const string columnSplit = "\t";

        private const string valueSplit = ", ";

        /// <summary>
        /// Load the file and return hash set result. It saves each line to result.
        /// If duplicate line is existing in the file, it will throw out exception.
        /// </summary>
        public static HashSet<string> LoadFileAsHashSet(string filePath)
        {
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Trim();
                    result.Add(line);
                }
            }

            return result;
        }

        /// <summary>
        /// Load the file and return list result.
        /// </summary>
        public static IList<string> LoadFileAsList(string filePath)
        {
            var result = new List<string>();

            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    result.Add(line);
                }
            }

            return result;
        }

        /// <summary>
        /// Load the file and return dictionary result. It splits each line with \t.
        /// It saves the first column as the key while the second as the value to result.
        /// If duplicate key is existing in the file, it will throw out exception.
        /// </summary>
        public static IDictionary<string, string> LoadFileAsDic(string filePath, bool convertValueToLower = false)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var cols = line.Split(columnSplit.ToCharArray());
                    result[cols[0].Trim()] = convertValueToLower ? cols[1].Trim().ToLowerInvariant() : cols[1].Trim();
                }
            }

            return result;
        }

        /// <summary>
        /// Load the file and return dictionary result. It splits each line with \t.
        /// It saves the first column as the key while the second as the value of template type to result.
        /// If duplicate key is existing in the file, it will throw out exception.
        /// </summary>
        public static IDictionary<string, T> LoadFileAsDic<T>(string filePath) where T : IConvertible
        {
            var result = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
            var converter = TypeDescriptor.GetConverter(typeof(T));
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var cols = line.Split(columnSplit.ToCharArray());
                    result.Add(cols[0].Trim(), (T)converter.ConvertFromString(cols[1].Trim()));
                }
            }

            return result;
        }

        /// <summary>
        /// Load the file and return dictionary result. It splits each line with \t.
        /// It saves the first column as the key while the second as list of string to result.
        /// If duplicate key is existing in the file, it will throw out exception.
        /// </summary>
        public static IDictionary<string, IList<string>> LoadFileAsDicList(string filePath, bool convertValueToLower = false)
        {
            var result = new Dictionary<string, IList<string>>(StringComparer.OrdinalIgnoreCase);

            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var cols = line.Split(columnSplit.ToCharArray());
                    var values = convertValueToLower ? cols[1].Trim().ToLowerInvariant() : cols[1].Trim();
                    result.Add(cols[0].Trim(), values.Split(valueSplit.ToCharArray()));
                }
            }

            return result;
        }
    }
}
