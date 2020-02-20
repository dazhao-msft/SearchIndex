using System.Threading.Tasks;

namespace LanguageModelBuilder
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            await Task.Yield();
        }
    }
}
