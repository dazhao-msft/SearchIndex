using IndexModels;
using IndexServer.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
                    if (highlight.Key == Document.MetadataEntityEntityFieldName)
                    {
                        string cdsEntityName = searchResult.Document[Document.MetadataEntityEntityFieldName].ToString();

                        int startIndex = context.SearchText.IndexOf(cdsEntityName, StringComparison.OrdinalIgnoreCase);
                        if (startIndex >= 0)
                        {
                            var matchedTerm = new MatchedTerm
                            {
                                Text = cdsEntityName,
                                StartIndex = startIndex,
                                Length = cdsEntityName.Length,
                                TermBindings = new HashSet<TermBinding>(),
                            };

                            matchedTerm.TermBindings.Add(new TermBinding()
                            {
                                BindingType = BindingType.Table,
                                SearchScope = new SearchScope()
                                {
                                    Table = cdsEntityName,
                                    Column = null,
                                },
                                Value = cdsEntityName,
                                IsExactlyMatch = true,
                                IsSynonymMatch = false,
                            });

                            context.MatchedTerms.Add(matchedTerm);
                        }
                    }
                    else if (highlight.Key == Document.MetadataEntityAttributeFieldName)
                    {
                        string cdsEntityName = searchResult.Document[Document.MetadataEntityEntityFieldName].ToString();
                        string cdsAttributeName = searchResult.Document[Document.MetadataEntityAttributeFieldName].ToString();

                        int startIndex = context.SearchText.IndexOf(cdsAttributeName, StringComparison.OrdinalIgnoreCase);
                        if (startIndex >= 0)
                        {
                            var matchedTerm = new MatchedTerm
                            {
                                Text = cdsAttributeName,
                                StartIndex = startIndex,
                                Length = cdsAttributeName.Length,
                                TermBindings = new HashSet<TermBinding>(),
                            };

                            matchedTerm.TermBindings.Add(new TermBinding()
                            {
                                BindingType = BindingType.Column,
                                SearchScope = new SearchScope()
                                {
                                    Table = cdsEntityName,
                                    Column = cdsAttributeName,
                                },
                                Value = cdsAttributeName,
                                IsExactlyMatch = true,
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
