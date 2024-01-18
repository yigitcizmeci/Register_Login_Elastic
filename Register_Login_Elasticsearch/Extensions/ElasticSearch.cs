using Elasticsearch.Net;
using Nest;
using Register_Login_Elasticsearch.Models;

namespace Register_Login_Elasticsearch.Extensions
{
    public static class ElasticSearch
    {
        public static void AddElastic(this IServiceCollection services, IConfiguration configuration)
        {
            var pool = new SingleNodeConnectionPool(new Uri(configuration.GetSection("Elastic")["Uri"]!));
            var settings = new ConnectionSettings(pool)
                .DefaultIndex("register_login_elasticsearch-development-01-2024")
                .DefaultMappingFor<Users>(m => m.IndexName("register_login_elasticsearch-development-01-2024"));
            var client = new ElasticClient(settings);
            services.AddSingleton(client);
        }
    }
}
