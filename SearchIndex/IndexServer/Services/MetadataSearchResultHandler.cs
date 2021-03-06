﻿using IndexModels;
using IndexServer.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IndexServer.Services
{
    public class MetadataSearchResultHandler : ISearchResultHandler
    {
        private readonly ILogger<MetadataSearchResultHandler> _logger;

        public MetadataSearchResultHandler(ILogger<MetadataSearchResultHandler> logger)
        {
            _logger = logger;
        }

        public async Task ProcessAsync(SearchResultHandlerContext context)
        {
            await Task.Yield();

            foreach (var searchResult in context.SearchResults)
            {
                string entityName = searchResult.Document[Document.EntityNameFieldName].ToString();

                if (entityName != Document.MetadataEntityName)
                {
                    continue;
                }

                foreach (var highlight in searchResult.Highlights)
                {
                    foreach (string fragment in highlight.Value)
                    {
                        foreach (var fragmentToken in FragmentHelper.GetTokensFromFragment(fragment, context.SearchParameters.HighlightPreTag, context.SearchParameters.HighlightPostTag))
                        {
                            //
                            // Question: what if the same word shows in multiple positions?
                            //
                            var searchToken = context.SearchTokens.FirstOrDefault(p => StringComparer.OrdinalIgnoreCase.Equals(p.Token, fragmentToken.Token));

                            if (searchToken == null)
                            {
                                _logger.LogWarning($"Token value '{fragmentToken.Token}' isn't matched.");
                                continue;
                            }

                            //
                            // Question: why offset is nullable?
                            //
                            string matchedText = context.SearchText[(int)searchToken.StartOffset..(int)searchToken.EndOffset];
                            string fieldValue = searchResult.Document[highlight.Key].ToString();

                            var matchedTerm = new MatchedTerm
                            {
                                Text = matchedText,
                                StartIndex = (int)searchToken.StartOffset,
                                Length = matchedText.Length,
                                TermBindings = new HashSet<TermBinding>(),
                            };

                            bool isEntityNameMatched = highlight.Key == Document.MetadataEntityEntityFieldName;

                            matchedTerm.TermBindings.Add(new TermBinding()
                            {
                                BindingType = isEntityNameMatched ? BindingType.Table : BindingType.Column,
                                SearchScope = new SearchScope()
                                {
                                    Table = searchResult.Document[Document.MetadataEntityEntityFieldName].ToString(),
                                    Column = isEntityNameMatched ? null : searchResult.Document[Document.MetadataEntityAttributeFieldName].ToString(),
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
