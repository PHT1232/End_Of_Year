using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Nguyen_Tan_Phat_Project.Configuration;
using Nguyen_Tan_Phat_Project.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.EntityFrameworkCore
{
    public class HoChiMinhDbContextFactory : IDesignTimeDbContextFactory<HoChiMinhDbContext>
    {
        public HoChiMinhDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<HoChiMinhDbContext>();

            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            HoChiMinhDbContextConfigurer.Configure(builder, configuration.GetConnectionString(Nguyen_Tan_Phat_ProjectConsts.externalClientConnection));

            return new HoChiMinhDbContext(builder.Options);
        }
    }
}
