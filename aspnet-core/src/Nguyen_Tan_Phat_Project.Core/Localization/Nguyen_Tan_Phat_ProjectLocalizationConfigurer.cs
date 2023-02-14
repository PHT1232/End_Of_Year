using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace Nguyen_Tan_Phat_Project.Localization
{
    public static class Nguyen_Tan_Phat_ProjectLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(Nguyen_Tan_Phat_ProjectConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(Nguyen_Tan_Phat_ProjectLocalizationConfigurer).GetAssembly(),
                        "Nguyen_Tan_Phat_Project.Localization.SourceFiles"
                    )
                )
            );
        }
    }
}
