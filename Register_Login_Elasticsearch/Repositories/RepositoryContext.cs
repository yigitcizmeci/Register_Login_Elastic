using Microsoft.EntityFrameworkCore;
using Register_Login_Elasticsearch.Config;
using Register_Login_Elasticsearch.DTOs;
using Register_Login_Elasticsearch.Models;
using Register_Login_Elasticsearch.Security;

namespace Register_Login_Elasticsearch.Repositories
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions options) : base(options) 
        {

        }
        public DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfig());
        }
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddSingleton<TokenHandler>();
        }

    }
}
