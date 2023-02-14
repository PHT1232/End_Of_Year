using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Nguyen_Tan_Phat_Project.EntityFrameworkCore
{
    public static class Nguyen_Tan_Phat_ProjectDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<Nguyen_Tan_Phat_ProjectDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<Nguyen_Tan_Phat_ProjectDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
