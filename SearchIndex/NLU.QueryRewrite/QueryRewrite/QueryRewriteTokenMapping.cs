//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Microsoft.BizQA.NLU.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.BizQA.NLU.QueryRewrite.QueryRewrite
{
    public sealed class QueryRewriteTokenMapping : IQueryRewriteStep
    {
        private const string PossessPostFix = @"'s";

        private readonly ITokenizer _tokenizer;
        private readonly IDictionary<string, string> _tokenMappings;

        public QueryRewriteTokenMapping(QueryRewriteTokenMappingOptions options, ITokenizer tokenizer)
        {
            Enabled = options != null
                && options.TokenMappingEnabled;

            if (Enabled)
            {
                _tokenizer = tokenizer;
                if (File.Exists(options.TokenMappingFilePath))
                {
                    _tokenMappings = CommonFileLoader.LoadFileAsDic(options.TokenMappingFilePath, true);
                }
            }
        }

        public bool Enabled { get; }

        public QueryRewriteOperations RewriteOperation { get; } = QueryRewriteOperations.TokenMapping;

        public QueryRewriteStepResult Rewrite(QueryRewriteStepResult prevStepResult)
        {
            if (!Enabled)
            {
                return prevStepResult;
            }

            var queryRewriteTokens = new List<QueryRewriteToken>();
            foreach (var token in prevStepResult.RewriteTokens)
            {
                if (!token.Tags.HasFlag(QueryRewriteTags.NoAlter) && _tokenMappings.TryGetValue(token.Text, out var tokenReplacement))
                {
                    var subTokens = _tokenizer.Tokenize(tokenReplacement);
                    foreach (var subToken in subTokens)
                    {
                        queryRewriteTokens.Add(new QueryRewriteToken(subToken, token.Start, token.Length));
                    }
                }
                else if (token.Text.EndsWith(PossessPostFix, StringComparison.OrdinalIgnoreCase) && token.Text.Length > PossessPostFix.Length)
                {
                    queryRewriteTokens.Add(new QueryRewriteToken(token.Text.Substring(0, token.Text.Length - PossessPostFix.Length), token.Start, token.Length - PossessPostFix.Length)
                    {
                        Tags = token.Tags,
                    });
                    queryRewriteTokens.Add(new QueryRewriteToken(token.Text.Substring(token.Text.Length - PossessPostFix.Length), token.Start + token.Length - PossessPostFix.Length, PossessPostFix.Length));
                }
                else
                {
                    queryRewriteTokens.Add(token);
                }
            }

            return new QueryRewriteStepResult(queryRewriteTokens, prevStepResult.RewriteOperations | RewriteOperation);
        }
    }
}
