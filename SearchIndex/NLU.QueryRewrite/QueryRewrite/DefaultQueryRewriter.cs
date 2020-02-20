//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Microsoft.BizQA.NLU.Common;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.BizQA.NLU.QueryRewrite.QueryRewrite
{
    public sealed class DefaultQueryRewriter : IQueryRewriter
    {
        private readonly IList<IQueryRewriteStep> _rewriteSteps;

        private IQueryRewriteStep _postLowerCaseRewriter;

        private IQueryRewriteStep _tokenMapingRewriter;

        private IQueryRewriteStep _spellerRewriter;

        private ITokenizer _tokenizer;

        private IQueryRewriteStep _oralRewriter;

        private IQueryRewriteStep _stopwordsRemovalRewriter;

        private IQueryRewriteStep _stemmerRewriter;

        private bool _preLowerCaseEnabled;

        public DefaultQueryRewriter(QueryRewriteOptions options)
        {
            _tokenizer = QueryRewriteActivator.GetTokenizer(options);
            CreateRewriteSteps(options, _tokenizer);
            _preLowerCaseEnabled = options.PreLowerCaseEnabled;

            _rewriteSteps = new List<IQueryRewriteStep>
            {
                _tokenMapingRewriter,
                _spellerRewriter,
                _oralRewriter,
                _stopwordsRemovalRewriter,
                _stemmerRewriter,
                _postLowerCaseRewriter,
            }.Where(s => s.Enabled).ToList();
        }

        public QueryRewriteResult Rewrite(string query) => Rewrite(query, null, _rewriteSteps, true);

        public QueryRewriteResult Rewrite(string query, QueryRewriteOperations rewriteOperations)
            => Rewrite(query, null, _rewriteSteps.Where(s => rewriteOperations.HasFlag(s.RewriteOperation)), rewriteOperations.HasFlag(QueryRewriteOperations.PreLowerCaseNormalization));

        public QueryRewriteResult Rewrite(string query, IReadOnlyList<ISpan> noAlterSpans, QueryRewriteOperations rewriteOperations)
            => Rewrite(query, noAlterSpans, _rewriteSteps.Where(s => rewriteOperations.HasFlag(s.RewriteOperation)), rewriteOperations.HasFlag(QueryRewriteOperations.PreLowerCaseNormalization));

        public List<string> Tokenize(string text) => _tokenizer.Tokenize(text);

        public List<QueryRewriteToken> QueryRewriteTokenize(string text) => _tokenizer.QueryRewriteTokenize(text);

        private QueryRewriteResult Rewrite(
            string query,
            IReadOnlyList<ISpan> noAlterSpans,
            IEnumerable<IQueryRewriteStep> rewriteSteps,
            bool preLowerCaseNormalization)
        {
            var result = new QueryRewriteResult(query);
            if (preLowerCaseNormalization && _preLowerCaseEnabled)
            {
                var lowerCaseQuery = query.ToLowerInvariant();
                var lowerCaseQueryToken = new QueryRewriteToken(lowerCaseQuery, 0, lowerCaseQuery.Length);
                var preLowerCaseStepResult = new QueryRewriteStepResult(
                    new List<QueryRewriteToken>() { lowerCaseQueryToken },
                    result.LastStepResult.RewriteOperations | QueryRewriteOperations.PreLowerCaseNormalization);
                result.QueryRewriteStepResults.Add(preLowerCaseStepResult);
            }

            var rawTokens = _tokenizer.QueryRewriteTokenize(result.LastRewrittenQuery);
            SetNoAlterTag(rawTokens, noAlterSpans);

            var rewriteStepResult = new QueryRewriteStepResult(rawTokens, QueryRewriteOperations.Tokenization);
            result.QueryRewriteStepResults.Add(rewriteStepResult);

            foreach (var rewriteStep in rewriteSteps)
            {
                rewriteStepResult = rewriteStep.Rewrite(rewriteStepResult);
                result.QueryRewriteStepResults.Add(rewriteStepResult);
            }

            return result;
        }

        private void CreateRewriteSteps(QueryRewriteOptions options, ITokenizer tokenizer)
        {
            _postLowerCaseRewriter = new QueryRewriteLowerCase(options.PostLowerCaseEnabled, QueryRewriteOperations.PostLowerCaseNormalization);
            _tokenMapingRewriter = new QueryRewriteTokenMapping(options.TokenMappingOptions, tokenizer);
            _spellerRewriter = new QueryRewriteSpeller(options.SpellerOptions, tokenizer);
            _oralRewriter = new QueryRewriteOral(options.OralOptions, tokenizer);
            _stopwordsRemovalRewriter = new QueryRewriteStopwords(options.StopwordsOptions);
            _stemmerRewriter = new QueryRewriteStemmer(options.StemmerOptions);
        }

        private void SetNoAlterTag(IEnumerable<QueryRewriteToken> tokens, IEnumerable<ISpan> noAlterSpans)
        {
            if (noAlterSpans != null && noAlterSpans.Any())
            {
                foreach (var token in tokens)
                {
                    if (noAlterSpans.Any(s => SpanUtils.IsOverlap(s, token)))
                    {
                        token.Tags |= QueryRewriteTags.NoAlter;
                    }
                }
            }
        }
    }
}
