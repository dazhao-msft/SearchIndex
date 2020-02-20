//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Microsoft.BizQA.NLU.QueryRewrite.Speller;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.BizQA.NLU.QueryRewrite.QueryRewrite
{
    internal sealed class QueryRewriteSpeller : IQueryRewriteStep
    {
        private readonly IList<ISpeller> _spellers;

        public QueryRewriteSpeller(QueryRewriteSpellerOptions options, ITokenizer tokenizer)
        {
            Enabled = options != null ? options.SpellerEnabled : false;
            if (Enabled)
            {
                _spellers = new List<ISpeller>();
                var types = options.SpellerType;
                foreach (var type in types.Split(',').Select(p => p.Trim()))
                {
                    switch (Enum.Parse(typeof(SpellerTypeEnum), type))
                    {
                        case SpellerTypeEnum.EntitySpeller:
                            _spellers.Add(new DictCorrectSpeller(options.SpellerEntityFilePath, tokenizer));
                            break;
                        case SpellerTypeEnum.GeneralSpeller:
                            _spellers.Add(new GeneralSpeller(options, tokenizer));
                            break;
                        default:
                            throw new NotSupportedException("The speller type isn't being supported");
                    }
                }
            }
        }

        public bool Enabled { get; }

        public QueryRewriteOperations RewriteOperation { get; } = QueryRewriteOperations.SpellCorrection;

        public QueryRewriteStepResult Rewrite(QueryRewriteStepResult prevStepResult)
        {
            if (!Enabled)
            {
                return prevStepResult;
            }

            var spellCorrectedTokens = prevStepResult.RewriteTokens;
            foreach (var speller in _spellers)
            {
                spellCorrectedTokens = speller.SpellCorrect(spellCorrectedTokens);
            }

            return new QueryRewriteStepResult(spellCorrectedTokens, prevStepResult.RewriteOperations | RewriteOperation);
        }
    }
}
