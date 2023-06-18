using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Nguyen_Tan_Phat_Project.Entities;
using Nguyen_Tan_Phat_Project.Module.StorageAppService.ExportImportManagement.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.StorageAppService.ExportImportManagement
{
    public interface IExportImportAppService
    {
        public Task AddNewAsync(ExportImportInput input);
        public Task UpdateOrderAsync(ExportImportInput input);
        public Task<ContentResult> UpdateOrderQRAsync(ExportImportInput input);
        public Task UpdateAsync(ExportImportInput input);
        public Task DeleteAsync(string id);
        public Task<PagedResultDto<ExportImportGetAllDto>> GetAllAsync(ExportImportPagedResultInput input);
        public Task<ExportImportOutput> GetAsync(string id);
    }
}
