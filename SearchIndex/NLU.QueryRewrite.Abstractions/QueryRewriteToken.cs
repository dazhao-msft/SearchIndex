//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Microsoft.BizQA.NLU.Common;
using Newtonsoft.Json;
using System;

namespace Microsoft.BizQA.NLU.QueryRewrite
{
    /// <summary>
    /// Query rewrite token
    /// </summary>
    [Serializable]
    public class QueryRewriteToken : ISpan
    {
        /// <summary>
        /// For json deserialization, do not remove.
        /// </summary>
        public QueryRewriteToken()
        {
            Tags = QueryRewriteTags.None;
        }

        public QueryRewriteToken(string text, int start, int length, QueryRewriteTags tags = QueryRewriteTags.None)
        {
            Text = text;
            Start = start;
            Length = length;
            Tags = tags;
        }

        /// <summary>
        /// char level start position of this token in raw query
        /// </summary>
        [JsonProperty]
        public int Start { get; set; }

        /// <summary>
        /// char level length of this token in raw query
        /// </summary>
        [JsonProperty]
        public int Length { get; set; }

        /// <summary>
        /// char level end position (exclusive) of this token in raw query
        /// </summary>
        [JsonIgnore]
        public int End
        {
            get
            {
                return Start + Length;
            }
        }

        /// <summary>
        /// text of this token
        /// </summary>
        [JsonProperty]
        public string Text { get; set; }

        [JsonProperty]
        public QueryRewriteTags Tags { get; set; }

        [JsonIgnore]
        public bool IsStopword => Tags.HasFlag(QueryRewriteTags.Stopword);

        [JsonIgnore]
        public bool IsPreposition => Tags.HasFlag(QueryRewriteTags.Preposition);

        public QueryRewriteToken Clone()
        {
            return new QueryRewriteToken()
            {
                Start = Start,
                Length = Length,
                Text = Text,
                Tags = Tags,
            };
        }
    }
}
