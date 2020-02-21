using Microsoft.BizQA.NLU.QueryRewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace LanguageModelBuilder
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json");

            var configuration = configurationBuilder.Build();

            var services = new ServiceCollection();

            services.Configure<QueryRewriteOptions>(configuration.GetSection(nameof(QueryRewriteOptions)));

            var serviceProvider = services.BuildServiceProvider(true);

            var tokenizer = QueryRewriteActivator.GetTokenizer(serviceProvider.GetRequiredService<IOptions<QueryRewriteOptions>>().Value);

            var result = tokenizer.Tokenize("hello world");

            await Task.Yield();
        }
    }
}
