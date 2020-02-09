using IndexServer.Models;
using Microsoft.Azure.Search.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IndexServer.Services
{
    public interface ISearchProvider
    {
        Task<IReadOnlyCollection<TokenInfo>> AnalyzeAsync(string searchText, string analyzerName);

        Task<IReadOnlyCollection<MatchedTerm>> SearchAsync(string searchText);
    }
}
