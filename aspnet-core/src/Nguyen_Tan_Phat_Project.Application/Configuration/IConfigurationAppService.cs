using System.Threading.Tasks;
using Nguyen_Tan_Phat_Project.Configuration.Dto;

namespace Nguyen_Tan_Phat_Project.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
