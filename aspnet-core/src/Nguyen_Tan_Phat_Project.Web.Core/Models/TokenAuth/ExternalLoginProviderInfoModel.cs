using Abp.AutoMapper;
using Nguyen_Tan_Phat_Project.Authentication.External;

namespace Nguyen_Tan_Phat_Project.Models.TokenAuth
{
    [AutoMapFrom(typeof(ExternalLoginProviderInfo))]
    public class ExternalLoginProviderInfoModel
    {
        public string Name { get; set; }

        public string ClientId { get; set; }
    }
}
