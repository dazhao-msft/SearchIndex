//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.BizQA.NLU.Common
{
    public static class DictionaryExtensions
    {
        public static TVal GetValueOrDefault<TKey, TVal>(this IDictionary<TKey, TVal> dictionary, TKey key, TVal defaultValue = default)
        {
            if (dictionary != null && dictionary.TryGetValue(key, out var value))
            {
                return value;
            }

            return defaultValue;
        }
    }
}
