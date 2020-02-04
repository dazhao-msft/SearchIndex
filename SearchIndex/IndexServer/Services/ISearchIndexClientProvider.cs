using Microsoft.Azure.Search;

namespace IndexServer.Services
{
    public interface ISearchIndexClientProvider
    {
        ISearchIndexClient CreateSearchIndexClient();
    }
}