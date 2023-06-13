using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.SumaryService.dtos
{
    public class DatasetDtoList
    {
        public DatasetDto items { get; set; }
    }
    public class DatasetDto
    {
        public List<string> Labels { get; set; }
        public List<DatasetClass> Datasets { get; set; }
    }

    public class DatasetClass
    {
        public List<int> Data { get; set; }
        public List<string> BackgroundColor { get; set; }
        public List<string> HoverBackgroundColor { get; set; }
    }
}
