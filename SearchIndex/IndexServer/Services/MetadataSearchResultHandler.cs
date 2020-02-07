using IndexModels;
using Microsoft.Extensions.Logging;
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
            }
        }
    }
}
