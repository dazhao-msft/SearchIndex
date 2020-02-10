using IndexModels;
using IndexServer.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IndexServer.Services
{
    public class InstanceSearchResultHandler : ISearchResultHandler
    {
        private readonly ILogger<InstanceSearchResultHandler> _logger;

        public InstanceSearchResultHandler(ILogger<InstanceSearchResultHandler> logger)
        {
            _logger = logger;
        }

        public async Task ProcessAsync(SearchResultHandlerContext context)
        {
            await Task.Yield();

            foreach (var searchResult in context.SearchResults)
            {
                string entityName = searchResult.Document[Document.EntityNameFieldName].ToString();

                if (entityName == Document.MetadataEntityName)
                {
                    continue;
                }

                string cdsEntityName = entityName;

                foreach (var highlight in searchResult.Highlights)
                {
                    if (Document.TryResolveCdsAttributeName(highlight.Key, cdsEntityName, out string cdsAttributeName))
                    {
                        foreach (string fragment in highlight.Value)
                        {
                            foreach (var tokenFromFragment in TokenHelper.GetTokensFromText(fragment, context.SearchParameters.HighlightPreTag, context.SearchParameters.HighlightPostTag))
                            {
                                //
                                // Question: what if the same word shows in multiple positions?
                                //
                                var token = context.QueryTokens.FirstOrDefault(p => StringComparer.OrdinalIgnoreCase.Equals(p.Token, tokenFromFragment.Token));

                                if (token == null)
                                {
                                    _logger.LogWarning($"Token value '{tokenFromFragment.Token}' isn't matched.");
                                    continue;
                                }

                                //
                                // Question: why offset is nullable?
                                //
                                string matchedText = context.SearchText[(int)token.StartOffset..(int)token.EndOffset];
                                string fieldValue = searchResult.Document[highlight.Key].ToString();

                                var matchedTerm = new MatchedTerm
                                {
                                    Text = matchedText,
                                    StartIndex = (int)token.StartOffset,
                                    Length = matchedText.Length,
                                    TermBindings = new HashSet<TermBinding>(),
                                };

                                matchedTerm.TermBindings.Add(new TermBinding()
                                {
                                    BindingType = BindingType.InstanceValue,
                                    SearchScope = new SearchScope()
                                    {
                                        Table = cdsEntityName,
                                        Column = cdsAttributeName,
                                    },
                                    Value = fieldValue,
                                    IsExactlyMatch = StringComparer.OrdinalIgnoreCase.Equals(matchedText, fieldValue),
                                    IsSynonymMatch = false,
                                });

                                context.MatchedTerms.Add(matchedTerm);
                            }
                        }
                    }
                }
            }
        }
    }
}
