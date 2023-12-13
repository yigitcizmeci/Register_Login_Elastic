using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Register_Login_Elasticsearch.Models;

namespace Register_Login_Elasticsearch.Config
{
    public class UserConfig : IEntityTypeConfiguration<Users>
    {
        public void Configure(EntityTypeBuilder<Users> builder)
        {
            builder.HasData(
                new Users { Id = 1, Name = "", Surname = "", Email = "", UserName = "", Password = "" }
                );
        }
    }
}
