//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace Microsoft.BizQA.NLU.QueryRewrite
{
    public class QueryRewriteStemmerOptions
    {
        /// <summary>
        /// The legacy stemmer mappings.
        /// </summary>
        public const string DefaultRules = @"misc";

        public bool StemmerEnabled { get; set; }

        /// <summary>
        /// The tsv file containning root words and the corresponding different types of forms derived from the root, the first column contains the root words, and each followed column contains the derived words of a type of forming rule.
        /// </summary>
        public string StemmerFilePath { get; set; }

        /// <summary>
        /// The tsv file for tokens we don't apply stemming on
        /// </summary>
        public string NoStemFilePath { get; set; }

        /// <summary>
        /// Valid rule names delimited by ','. Each name is the header of a column in the stemmer tsv file, denoting a type of morphological rule (word to word mapping) of generating a derived form from a root word. Stemmer will pick up the corresponding columns and merge them together to a reverse mapping from a derived form to its root word.
        /// Components decide what rules to enable.
        ///
        /// All valid rules: "misc,adj-to-adv,adj-to-comparative,adj-to-noun,noun-to-plural,verb-to-gerund,verb-to-past-and-participle,verb-to-third-person".
        /// Components decide what rules to enable.
        ///
        /// misc: the default one, compatible with legacy stemmer file, refers to the column 'misc' containing all the legacy root-to-forms mapping. Notice that the legacy mapping mixed different types of rules together, e.g., -ing, -ed, etc. After a period of transition to the new stemmer file, if no regression observed, will merge the misc to other corresponding specific columns.
        /// adj-to-adv: refers to the column 'adj-to-adv' containing the adverb words derived from the root adjective words, e.g., clear -> clearly
        /// adj-to-comparative: refers to the column 'adj-to-comparative' containing the comprative/superlative forms of the root adjective words, e.g., good -> better,best. WARNING: this rule changes the meaning. If you need the very precise semantic meaning, do not enable this one.
        /// adj-to-noun: refers to the column 'adj-to-noun' containing the noun words derived from the root adjective words, e.g., abundant - > abundance, agile -> agility.
        /// noun-to-plural: refers to the column 'noun-to-plural' containing the plural forms of the root noun words, e.g., apple -> apples
        /// verb-to-gerund: refers to the column 'verb-to-gerund' containing the gerund (present continuous) forms of the root verb words, e.g., run -> running
        /// verb-to-past-and-participle: refers to the column 'verb-to-past-and-participle' containing the simple past and past participle forms of the root verb words, e.g., win -> won, go -> went, gone, be -> was,were
        /// verb-to-third-person: refers to the column 'verb-to-third-person' containing the third-person forms of the root verb words, e.g., be -> am,is,are
        ///
        /// Default rules: @"misc", i.e., the legacy stemmer mappings
        /// Rules suggested rules for intent triggering: "misc,adj-to-adv,adj-to-noun,noun-to-plural,verb-to-gerund,verb-to-past-and-participle,verb-to-third-person", i.e., only disable "adj-to-comparative".
        /// </summary>
        public string EnabledRules { get; set; } = DefaultRules;
    }
}
