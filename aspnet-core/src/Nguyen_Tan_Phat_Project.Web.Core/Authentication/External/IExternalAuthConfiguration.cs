using System.Collections.Generic;

namespace Nguyen_Tan_Phat_Project.Authentication.External
{
    public interface IExternalAuthConfiguration
    {
        List<ExternalLoginProviderInfo> Providers { get; }
    }
}
