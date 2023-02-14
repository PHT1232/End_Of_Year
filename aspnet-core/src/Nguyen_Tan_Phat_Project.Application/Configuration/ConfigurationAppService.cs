using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using Nguyen_Tan_Phat_Project.Configuration.Dto;

namespace Nguyen_Tan_Phat_Project.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : Nguyen_Tan_Phat_ProjectAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
