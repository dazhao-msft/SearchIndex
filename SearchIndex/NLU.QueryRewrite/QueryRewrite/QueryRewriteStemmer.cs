//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Microsoft.BizQA.NLU.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.BizQA.NLU.QueryRewrite.QueryRewrite
{
    public sealed class QueryRewriteStemmer : IQueryRewriteStep
    {
        private readonly IDictionary<string, string> _stemmers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public QueryRewriteStemmer(QueryRewriteStemmerOptions options)
        {
            Enabled = options != null ? options.StemmerEnabled : false;
            if (Enabled)
            {
                LoadStemmer(options.StemmerFilePath, options.EnabledRules, options.NoStemFilePath);
            }
        }

        public bool Enabled { get; } = false;

        public QueryRewriteOperations RewriteOperation { get; } = QueryRewriteOperations.Stemming;

        public QueryRewriteStepResult Rewrite(QueryRewriteStepResult prevStepResult)
        {
            if (!Enabled)
            {
                return prevStepResult;
            }

            var stemmedTokens = new List<QueryRewriteToken>();
            foreach (var token in prevStepResult.RewriteTokens)
            {
                if (!token.Tags.HasFlag(QueryRewriteTags.NoAlter) && _stemmers.ContainsKey(token.Text))
                {
                    stemmedTokens.Add(new QueryRewriteToken(_stemmers[token.Text], token.Start, token.Length)
                    {
                        Tags = token.Tags,
                    });
                }
                else
                {
                    stemmedTokens.Add(token);
                }
            }

            return new QueryRewriteStepResult(stemmedTokens, prevStepResult.RewriteOperations | RewriteOperation);
        }

        private void LoadStemmer(string stemmerFilePath, string enabledRules, string noStemFilePath)
        {
            var noStemTokens = File.Exists(noStemFilePath) ? CommonFileLoader.LoadFileAsHashSet(noStemFilePath) : new HashSet<string>();

            using (var reader = new StreamReader(stemmerFilePath))
            {
                var headerColumns = reader.ReadLine().Split('\t').Select(column => column.Trim()).ToList();
                if (headerColumns?.Any() != true)
                {
                    throw new FormatException($"The stemmer file {stemmerFilePath} is of invalid format: header is empty!");
                }

                if (headerColumns.Count < 2)
                {
                    throw new FormatException($"The stemmer file {stemmerFilePath} is of invalid format: only {headerColumns.Count} columns found!");
                }

                var enabledColumns = enabledRules.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
                var invalidColumns = enabledColumns.Where(col => !headerColumns.Contains(col, StringComparer.OrdinalIgnoreCase)).ToList();
                if (invalidColumns.Any())
                {
                    throw new ArgumentOutOfRangeException(nameof(enabledRules), $"The following rules are invalid: {string.Join(",", invalidColumns)}, not found in the stemmer file!");
                }

                var columnNameToIdx = Enumerable.Range(0, headerColumns.Count).ToDictionary(idx => headerColumns[idx], idx => idx);
                var derivedWordsDelimiter = new char[] { ',' };
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    var cols = line.Split('\t').Select(col => col.Trim()).ToList();
                    if (cols.Count != headerColumns.Count)
                    {
                        throw new FormatException($"The stemmer file {stemmerFilePath} is of invalid format: number of columns in the current row is different from that of the header. File line: {line}!");
                    }

                    var rootWord = cols[0].ToLowerInvariant();
                    var mergedDerivedWords = new HashSet<string>();
                    foreach (var enabledColumn in enabledColumns)
                    {
                        var derivedWordsStr = cols[columnNameToIdx[enabledColumn]];
                        if (!string.IsNullOrEmpty(derivedWordsStr))
                        {
                            var derivedWords = derivedWordsStr.Split(derivedWordsDelimiter, StringSplitOptions.RemoveEmptyEntries).Select(word => word.Trim()).Where(word => !string.IsNullOrEmpty(word)).ToList();
                            foreach (var word in derivedWords)
                            {
                                if (!noStemTokens.Contains(word))
                                {
                                    mergedDerivedWords.Add(word);
                                }
                            }
                        }
                    }

                    foreach (var word in mergedDerivedWords)
                    {
                        if (_stemmers.ContainsKey(word))
                        {
                            throw new InvalidDataException($"The stemmer file {stemmerFilePath} is of invalid data: there are two root words [{_stemmers[word]}] and [{rootWord}] found for form word [{word}]. Line: {line}!");
                        }
                        else
                        {
                            _stemmers.Add(word, rootWord);
                        }
                    }
                }
            }
        }
    }
}
