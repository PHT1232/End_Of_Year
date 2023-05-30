using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project
{
    public class AppFolders : IAppFolders, ISingletonDependency
    {
        public string DemoUploadFolder { get; set; }
        public string ExcelTemplateFolder { get; set; }
        public string ProductUploadFolder { get; set; }
    }
}
