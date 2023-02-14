using System.ComponentModel.DataAnnotations;

namespace Nguyen_Tan_Phat_Project.Users.Dto
{
    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}
