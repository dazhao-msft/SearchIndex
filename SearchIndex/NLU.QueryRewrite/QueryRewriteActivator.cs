//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Microsoft.BizQA.NLU.QueryRewrite.QueryRewrite;
using Microsoft.BizQA.NLU.QueryRewrite.Tokenizer;
using System;

namespace Microsoft.BizQA.NLU.QueryRewrite
{
    public static class QueryRewriteActivator
    {
        /// <summary>
        /// Note: query rewrite module doesn't do QueryRewriter object cache.
        /// It will generate a new object for each GetQueryRewrite call. So,
        /// for performance consideration, it would be better to reuse QueryRewrite
        /// object as much as possible as long as the QueryRewriteOptions is the same.
        /// Refer to class QueryRewriteE2ETests for more usage.
        /// </summary>
        public static IQueryRewriter GetQueryRewriter(QueryRewriteOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options), "The QueryRewriteOptions should not be null");
            }

            if (Enum.TryParse(options.QueryRewriteTypeName, out QueryRewriteTypeEnum queryRewriteType))
            {
                switch (queryRewriteType)
                {
                    case QueryRewriteTypeEnum.DefaultQueryRewriter:
                        return new DefaultQueryRewriter(options);
                    default:
                        throw new NotSupportedException("The query rewrite type isn't being supported");
                }
            }

            throw new NotSupportedException("The query rewrite type isn't being supported");
        }

        /// <summary>
        /// Note: query rewrite module doesn't do Tokenizer object cache.
        /// So, it will generate a new object for each GetTokenizer call.
        /// </summary>
        public static ITokenizer GetTokenizer(QueryRewriteOptions options)
        {
            if (options?.TokenizerOptions == null)
            {
                throw new ArgumentNullException(nameof(options), "The TokenizerOptions should not be null");
            }

            if (Enum.TryParse(options.TokenizerOptions.TokenizeTypeName, out TokenizeTypeEnum tokenizeType))
            {
                switch (tokenizeType)
                {
                    case TokenizeTypeEnum.DefaultTokenizer:
                        return new DefaultTokenizer(options?.TokenizerOptions);
                    case TokenizeTypeEnum.BertTokenizer:
                        return new BertTokenizer(options?.TokenizerOptions);
                    default:
                        throw new NotSupportedException("The query rewrite type isn't being supported");
                }
            }

            throw new NotSupportedException("The query rewrite type isn't being supported");
        }
    }
}
