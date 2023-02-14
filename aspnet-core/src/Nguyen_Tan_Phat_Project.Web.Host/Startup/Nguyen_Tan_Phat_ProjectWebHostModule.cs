using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Nguyen_Tan_Phat_Project.Configuration;

namespace Nguyen_Tan_Phat_Project.Web.Host.Startup
{
    [DependsOn(
       typeof(Nguyen_Tan_Phat_ProjectWebCoreModule))]
    public class Nguyen_Tan_Phat_ProjectWebHostModule: AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public Nguyen_Tan_Phat_ProjectWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(Nguyen_Tan_Phat_ProjectWebHostModule).GetAssembly());
        }
    }
}
