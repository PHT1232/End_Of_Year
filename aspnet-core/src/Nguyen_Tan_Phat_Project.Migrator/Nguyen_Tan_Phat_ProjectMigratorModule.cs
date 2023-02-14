using Microsoft.Extensions.Configuration;
using Castle.MicroKernel.Registration;
using Abp.Events.Bus;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Nguyen_Tan_Phat_Project.Configuration;
using Nguyen_Tan_Phat_Project.EntityFrameworkCore;
using Nguyen_Tan_Phat_Project.Migrator.DependencyInjection;

namespace Nguyen_Tan_Phat_Project.Migrator
{
    [DependsOn(typeof(Nguyen_Tan_Phat_ProjectEntityFrameworkModule))]
    public class Nguyen_Tan_Phat_ProjectMigratorModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public Nguyen_Tan_Phat_ProjectMigratorModule(Nguyen_Tan_Phat_ProjectEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbSeed = true;

            _appConfiguration = AppConfigurations.Get(
                typeof(Nguyen_Tan_Phat_ProjectMigratorModule).GetAssembly().GetDirectoryPathOrNull()
            );
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                Nguyen_Tan_Phat_ProjectConsts.ConnectionStringName
            );

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            Configuration.ReplaceService(
                typeof(IEventBus), 
                () => IocManager.IocContainer.Register(
                    Component.For<IEventBus>().Instance(NullEventBus.Instance)
                )
            );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(Nguyen_Tan_Phat_ProjectMigratorModule).GetAssembly());
            ServiceCollectionRegistrar.Register(IocManager);
        }
    }
}
