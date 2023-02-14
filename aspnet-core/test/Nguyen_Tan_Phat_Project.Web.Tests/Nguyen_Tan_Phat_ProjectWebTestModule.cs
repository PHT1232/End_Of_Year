using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Nguyen_Tan_Phat_Project.EntityFrameworkCore;
using Nguyen_Tan_Phat_Project.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Nguyen_Tan_Phat_Project.Web.Tests
{
    [DependsOn(
        typeof(Nguyen_Tan_Phat_ProjectWebMvcModule),
        typeof(AbpAspNetCoreTestBaseModule)
    )]
    public class Nguyen_Tan_Phat_ProjectWebTestModule : AbpModule
    {
        public Nguyen_Tan_Phat_ProjectWebTestModule(Nguyen_Tan_Phat_ProjectEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
        } 
        
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(Nguyen_Tan_Phat_ProjectWebTestModule).GetAssembly());
        }
        
        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(Nguyen_Tan_Phat_ProjectWebMvcModule).Assembly);
        }
    }
}