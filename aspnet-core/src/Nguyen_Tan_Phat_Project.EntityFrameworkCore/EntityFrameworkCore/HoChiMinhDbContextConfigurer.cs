using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.EntityFrameworkCore
{
    public class HoChiMinhDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<HoChiMinhDbContext> builder, string externalClientConnection)
        {
            builder.UseSqlServer(externalClientConnection);
        }

        public static void Configure(DbContextOptionsBuilder<HoChiMinhDbContext> builder, DbConnection externalClientConnection)
        {
            builder.UseSqlServer(externalClientConnection);
        }
    }
}
