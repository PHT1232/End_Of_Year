using Nguyen_Tan_Phat_Project.Debugging;

namespace Nguyen_Tan_Phat_Project
{
    public class Nguyen_Tan_Phat_ProjectConsts
    {
        public const string LocalizationSourceName = "Nguyen_Tan_Phat_Project";

        public const string ConnectionStringName = "Default";

        public const bool MultiTenancyEnabled = true;


        /// <summary>
        /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
        /// </summary>
        public static readonly string DefaultPassPhrase =
            DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "71b4e8ef1d1548f4a485ac2a057833ca";
    }
}
