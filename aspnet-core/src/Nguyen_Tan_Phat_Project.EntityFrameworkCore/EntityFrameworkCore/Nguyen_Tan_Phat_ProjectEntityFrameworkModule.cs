using Abp.EntityFrameworkCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.EntityFrameworkCore;
using Nguyen_Tan_Phat_Project.EntityFrameworkCore.Seed;

namespace Nguyen_Tan_Phat_Project.EntityFrameworkCore
{
    [DependsOn(
        typeof(Nguyen_Tan_Phat_ProjectCoreModule), 
        typeof(AbpZeroCoreEntityFrameworkCoreModule))]
    public class Nguyen_Tan_Phat_ProjectEntityFrameworkModule : AbpModule
    {
        /* Used it tests to skip dbcontext registration, in order to use in-memory database of EF Core */
        public bool SkipDbContextRegistration { get; set; }

        public bool SkipDbSeed { get; set; }

        public override void PreInitialize()
        {
            if (!SkipDbContextRegistration)
            {
                Configuration.Modules.AbpEfCore().AddDbContext<Nguyen_Tan_Phat_ProjectDbContext>(options =>
                {
                    if (options.ExistingConnection != null)
                    {
                        Nguyen_Tan_Phat_ProjectDbContextConfigurer.Configure(options.DbContextOptions, options.ExistingConnection);
                    }
                    else
                    {
                        Nguyen_Tan_Phat_ProjectDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
                    }
                });
            }
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(Nguyen_Tan_Phat_ProjectEntityFrameworkModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            if (!SkipDbSeed)
            {
                SeedHelper.SeedHostDb(IocManager);
            }
        }
    }
}
