using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Nguyen_Tan_Phat_Project.Configuration;
using Nguyen_Tan_Phat_Project.Web;

namespace Nguyen_Tan_Phat_Project.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class Nguyen_Tan_Phat_ProjectDbContextFactory : IDesignTimeDbContextFactory<Nguyen_Tan_Phat_ProjectDbContext>
    {
        public Nguyen_Tan_Phat_ProjectDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<Nguyen_Tan_Phat_ProjectDbContext>();
            
            /*
             You can provide an environmentName parameter to the AppConfigurations.Get method. 
             In this case, AppConfigurations will try to read appsettings.{environmentName}.json.
             Use Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") method or from string[] args to get environment if necessary.
             https://docs.microsoft.com/en-us/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli#args
             */
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            Nguyen_Tan_Phat_ProjectDbContextConfigurer.Configure(builder, configuration.GetConnectionString(Nguyen_Tan_Phat_ProjectConsts.ConnectionStringName));

            return new Nguyen_Tan_Phat_ProjectDbContext(builder.Options);
        }
    }
}
