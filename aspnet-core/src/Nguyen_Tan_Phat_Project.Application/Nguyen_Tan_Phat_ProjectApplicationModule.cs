using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Nguyen_Tan_Phat_Project.Authorization;

namespace Nguyen_Tan_Phat_Project
{
    [DependsOn(
        typeof(Nguyen_Tan_Phat_ProjectCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class Nguyen_Tan_Phat_ProjectApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<Nguyen_Tan_Phat_ProjectAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(Nguyen_Tan_Phat_ProjectApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
