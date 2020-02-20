//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Microsoft.Extensions.ObjectPool;
using System.Collections.Generic;

namespace Microsoft.BizQA.NLU.QueryRewrite.Speller
{
    internal static class OfficeSpellerWrapper
    {
        private const int SpellerInstanceNumber = 200;

        private static readonly DefaultObjectPool<TorontoSpeller> s_instance =
            new DefaultObjectPool<TorontoSpeller>(new DefaultPooledObjectPolicy<TorontoSpeller>(), SpellerInstanceNumber);

        public static IList<FlaggedToken> Correct(string sentence)
        {
            // For performance consideration, we used object pool to reuse the speller.
            TorontoSpeller speller = null;
            try
            {
                speller = s_instance.Get();
                return speller.Check(sentence);
            }
            finally
            {
                s_instance.Return(speller);
            }
        }
    }
}
