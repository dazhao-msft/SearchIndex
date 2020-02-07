using IndexServer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IndexServer.Services
{
    public interface ISearchProvider
    {
        Task<IReadOnlyCollection<MatchedTerm>> SearchAsync(string searchText);
    }
}
