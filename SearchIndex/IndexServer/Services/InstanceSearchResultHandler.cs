using IndexModels;
using IndexServer.Models;
using IndexServer.Tokens;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IndexServer.Services
{
    public class InstanceSearchResultHandler : ISearchResultHandler
    {
        private readonly ITokenizer _tokenizer;
        private readonly ILogger<InstanceSearchResultHandler> _logger;

        public InstanceSearchResultHandler(ITokenizer tokenizer, ILogger<InstanceSearchResultHandler> logger)
        {
            _tokenizer = tokenizer;
            _logger = logger;
        }

        public async Task ProcessAsync(SearchResultHandlerContext context)
        {
            await Task.Yield();

            var queryTokenSequence = new TokenSequence(context.SearchText, _tokenizer);

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
                        string fieldValue = searchResult.Document[highlight.Key].ToString();

                        foreach (string fragment in highlight.Value)
                        {
                            var fragmentTokenSequence = new TokenSequence(fragment, _tokenizer);

                            string matchedText = queryTokenSequence.FindLcs(fragmentTokenSequence, StringComparer.OrdinalIgnoreCase);

                            if (string.IsNullOrEmpty(matchedText))
                            {
                                continue;
                            }

                            var matchedTerm = new MatchedTerm
                            {
                                Text = matchedText,
                                StartIndex = context.SearchText.IndexOf(matchedText, StringComparison.OrdinalIgnoreCase),
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
                                IsExactlyMatch = StringComparer.OrdinalIgnoreCase.Equals(fieldValue, matchedText),
                                IsSynonymMatch = false,
                            });

                            if (matchedTerm.StartIndex >= 0)
                            {
                                context.MatchedTerms.Add(matchedTerm);
                            }
                            else
                            {
                                _logger.LogWarning($"start index of matched term is incorrect: {matchedTerm}");
                            }
                        }
                    }
                }
            }
        }
    }
}
