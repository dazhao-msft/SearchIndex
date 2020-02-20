//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Newtonsoft.Json;

namespace Microsoft.BizQA.NLU.Common
{
    public class ScoredItem<T>
    {
        public ScoredItem()
        {
        }

        public ScoredItem(T item, float score)
        {
            Item = item;
            Score = score;
        }

        public ScoredItem(T item)
            : this(item, 0f)
        {
        }

        [JsonProperty]
        public T Item { get; set; }

        [JsonProperty]
        public float Score { get; set; }
    }
}
