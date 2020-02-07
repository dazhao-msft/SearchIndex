using Microsoft.Azure.Search;

namespace IndexServer.Services
{
    public interface ISearchClientProvider
    {
        ISearchServiceClient CreateSearchServiceClient();

        ISearchIndexClient CreateSearchIndexClient();
    }
}