//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace Microsoft.BizQA.NLU.QueryRewrite
{
    public class QueryRewriteTokenizerOptions
    {
        /// <summary>
        /// Whether to do the tokenization for the original text.
        /// </summary>
        public bool TokenizationEnabled { get; set; }

        /// <summary>
        /// The TokenizeTypeName is the Tokenizer class Name. For now,
        /// we only expose class DefaultTokenizer outside the module,
        /// so it only supports class name of DefaultTokenizer.
        /// </summary>
        public string TokenizeTypeName { get; set; }

        /// <summary>
        /// The tokenize whitelist file path.
        /// </summary>
        public string TokenizeWhiteListFilePath { get; set; }

        /// <summary>
        /// Regular expression tokens
        /// </summary>
        public string RegexTokensFilePath { get; set; }

        /// <summary>
        /// The tokenize resource directory path.
        /// </summary>
        public string TokenizeResourcePath { get; set; }

        /// <summary>
        /// Wordpiece vocab file path
        /// </summary>
        public string WordpieceVocabFilePath { get; set; }

        /// <summary>
        /// Unknown vocab
        /// </summary>
        public string UnknownVocab { get; set; }

        /// <summary>
        /// Maximum number of chars per word
        /// </summary>
        public int MaxCharsPerWord { get; set; }
    }
}
