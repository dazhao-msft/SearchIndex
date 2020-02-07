using System.Threading.Tasks;

namespace IndexServer.Services
{
    public interface ISearchResultHandler
    {
        Task ProcessAsync(SearchResultHandlerContext context);
    }
}
