using System.ComponentModel.DataAnnotations;

namespace Nguyen_Tan_Phat_Project.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}