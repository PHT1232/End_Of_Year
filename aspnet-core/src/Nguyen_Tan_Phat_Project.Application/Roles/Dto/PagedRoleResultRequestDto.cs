using Abp.Application.Services.Dto;

namespace Nguyen_Tan_Phat_Project.Roles.Dto
{
    public class PagedRoleResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}

