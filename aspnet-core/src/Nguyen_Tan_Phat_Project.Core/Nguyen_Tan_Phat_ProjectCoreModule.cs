using Abp.Localization;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Runtime.Security;
using Abp.Timing;
using Abp.Zero;
using Abp.Zero.Configuration;
using Nguyen_Tan_Phat_Project.Authorization.Roles;
using Nguyen_Tan_Phat_Project.Authorization.Users;
using Nguyen_Tan_Phat_Project.Configuration;
using Nguyen_Tan_Phat_Project.Localization;
using Nguyen_Tan_Phat_Project.MultiTenancy;
using Nguyen_Tan_Phat_Project.Timing;

namespace Nguyen_Tan_Phat_Project
{
    [DependsOn(typeof(AbpZeroCoreModule))]
    public class Nguyen_Tan_Phat_ProjectCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;

            // Declare entity types
            Configuration.Modules.Zero().EntityTypes.Tenant = typeof(Tenant);
            Configuration.Modules.Zero().EntityTypes.Role = typeof(Role);
            Configuration.Modules.Zero().EntityTypes.User = typeof(User);

            Nguyen_Tan_Phat_ProjectLocalizationConfigurer.Configure(Configuration.Localization);

            // Enable this line to create a multi-tenant application.
            Configuration.MultiTenancy.IsEnabled = Nguyen_Tan_Phat_ProjectConsts.MultiTenancyEnabled;

            // Configure roles
            AppRoleConfig.Configure(Configuration.Modules.Zero().RoleManagement);

            Configuration.Settings.Providers.Add<AppSettingProvider>();
            
            Configuration.Localization.Languages.Add(new LanguageInfo("vi", "Việt Nam", "famfamfam-flags vi", true));
            
            Configuration.Settings.SettingEncryptionConfiguration.DefaultPassPhrase = Nguyen_Tan_Phat_ProjectConsts.DefaultPassPhrase;
            SimpleStringCipher.DefaultPassPhrase = Nguyen_Tan_Phat_ProjectConsts.DefaultPassPhrase;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(Nguyen_Tan_Phat_ProjectCoreModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            IocManager.Resolve<AppTimes>().StartupTime = Clock.Now;
        }
    }
}
