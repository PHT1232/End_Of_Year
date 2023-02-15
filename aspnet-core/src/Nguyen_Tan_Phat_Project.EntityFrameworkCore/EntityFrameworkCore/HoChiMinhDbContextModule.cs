using Abp.EntityFrameworkCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Nguyen_Tan_Phat_Project.EntityFrameworkCore.Seed;

namespace Nguyen_Tan_Phat_Project.EntityFrameworkCore
{
    public class HoChiMinhDbContextModule : AbpModule
    {
        public bool SkipDbContextRegistration { get; set; }

        public bool SkipDbSeed { get; set; }

        public override void PreInitialize()
        {
            if (!SkipDbContextRegistration)
            {
                Configuration.Modules.AbpEfCore().AddDbContext<HoChiMinhDbContext>(options =>
                {
                    if (options.ExistingConnection != null)
                    {
                        HoChiMinhDbContextConfigurer.Configure(options.DbContextOptions, options.ExistingConnection);
                    } else
                    {
                        HoChiMinhDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
                    }
                });
            }
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(HoChiMinhDbContextModule).GetAssembly());
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
