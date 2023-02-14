using Abp.Application.Services.Dto;

namespace Nguyen_Tan_Phat_Project.MultiTenancy.Dto
{
    public class PagedTenantResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public bool? IsActive { get; set; }
    }
}

