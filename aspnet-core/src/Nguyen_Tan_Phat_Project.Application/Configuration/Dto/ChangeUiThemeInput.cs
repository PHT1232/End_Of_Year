using System.ComponentModel.DataAnnotations;

namespace Nguyen_Tan_Phat_Project.Configuration.Dto
{
    public class ChangeUiThemeInput
    {
        [Required]
        [StringLength(32)]
        public string Theme { get; set; }
    }
}
