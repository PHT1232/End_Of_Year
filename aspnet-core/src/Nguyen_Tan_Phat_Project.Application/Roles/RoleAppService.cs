using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Linq.Extensions;
using Nguyen_Tan_Phat_Project.Authorization;
using Nguyen_Tan_Phat_Project.Authorization.Roles;
using Nguyen_Tan_Phat_Project.Authorization.Users;
using Nguyen_Tan_Phat_Project.Roles.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Abp.Authorization.Users;
using Nguyen_Tan_Phat_Project.Global;

namespace Nguyen_Tan_Phat_Project.Roles
{
    [AbpAuthorize(PermissionNames.Pages_System)]
    public class RoleAppService : AsyncCrudAppService<Role, RoleDto, int, PagedRoleResultRequestDto, CreateRoleDto, RoleDto>, IRoleAppService
    {
        private readonly RoleManager _roleManager;
        private readonly UserManager _userManager;
        private readonly IRepository<UserRole, long> _userRoleRepository;

        public RoleAppService(IRepository<Role> repository
            , RoleManager roleManager
            , UserManager userManager
            , IRepository<UserRole, long> userRoleRepository)
            : base(repository)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            this._userRoleRepository = userRoleRepository;
        }

        public override async Task<RoleDto> CreateAsync(CreateRoleDto input)
        {
            input.Name = GlobalFunction.RegexFormat(input.Name);
            input.DisplayName = GlobalFunction.RegexFormat(input.DisplayName);
            input.Description = GlobalFunction.RegexFormat(input.Description);
            input.GrantedPermissions = input.GrantedPermissions.Distinct().ToList();

            CheckCreatePermission();

            var role = ObjectMapper.Map<Role>(input);
            role.SetNormalizedName();

            CheckErrors(await _roleManager.CreateAsync(role));

            var grantedPermissions = PermissionManager
                .GetAllPermissions()
                .Where(p => input.GrantedPermissions.Contains(p.Name))
                .ToList();

            await _roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);

            return MapToEntityDto(role);
        }

        public async Task<ListResultDto<RoleListDto>> GetRolesAsync(GetRolesInput input)
        {
            var roles = await _roleManager
                .Roles
                .WhereIf(
                    !input.Permission.IsNullOrWhiteSpace(),
                    r => r.Permissions.Any(rp => rp.Name == input.Permission && rp.IsGranted)
                )
                .ToListAsync();

            return new ListResultDto<RoleListDto>(ObjectMapper.Map<List<RoleListDto>>(roles));
        }

        public override async Task<RoleDto> UpdateAsync(RoleDto input)
        {
            CheckUpdatePermission();

            var role = await _roleManager.GetRoleByIdAsync(input.Id);

            ObjectMapper.Map(input, role);

            CheckErrors(await _roleManager.UpdateAsync(role));

            var grantedPermissions = PermissionManager
                .GetAllPermissions()
                .Where(p => input.GrantedPermissions.Contains(p.Name))
                .ToList();

            await _roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);

            return MapToEntityDto(role);
        }

        public override async Task DeleteAsync(EntityDto<int> input)
        {
            CheckDeletePermission();

            var role = await _roleManager.FindByIdAsync(input.Id.ToString());
            var users = await _userManager.GetUsersInRoleAsync(role.NormalizedName);

            foreach (var user in users)
            {
                CheckErrors(await _userManager.RemoveFromRoleAsync(user, role.NormalizedName));
            }

            CheckErrors(await _roleManager.DeleteAsync(role));
        }

        public Task<ListResultDto<PermissionDto>> GetAllPermissions()
        {
            var permissions = PermissionManager.GetAllPermissions();

            return Task.FromResult(new ListResultDto<PermissionDto>(
                ObjectMapper.Map<List<PermissionDto>>(permissions).OrderBy(p => p.DisplayName).ToList()
            ));
        }

        protected override IQueryable<Role> CreateFilteredQuery(PagedRoleResultRequestDto input)
        {
            return Repository.GetAllIncluding(x => x.Permissions)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Name.Contains(input.Keyword)
                || x.DisplayName.Contains(input.Keyword)
                || x.Description.Contains(input.Keyword));
        }

        protected override async Task<Role> GetEntityByIdAsync(int id)
        {
            return await Repository.GetAllIncluding(x => x.Permissions).FirstOrDefaultAsync(x => x.Id == id);
        }

        protected override IQueryable<Role> ApplySorting(IQueryable<Role> query, PagedRoleResultRequestDto input)
        {
            return query.OrderBy(r => r.DisplayName);
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        public async Task<GetRoleForEditOutput> GetRoleForEdit(EntityDto input)
        {
            var permissions = PermissionManager.GetAllPermissions();
            var role = await _roleManager.GetRoleByIdAsync(input.Id);
            var grantedPermissions = (await _roleManager.GetGrantedPermissionsAsync(role)).ToArray();
            var roleEditDto = ObjectMapper.Map<RoleEditDto>(role);

            return new GetRoleForEditOutput
            {
                Role = roleEditDto,
                Permissions = ObjectMapper.Map<List<FlatPermissionDto>>(permissions).OrderBy(p => p.DisplayName).ToList(),
                GrantedPermissionNames = grantedPermissions.Select(p => p.Name).ToList()
            };
        }

        public async Task<string> MultiDeleteAsync(List<int> ids)
        {
            this.AssertNull(ids == null, "ids is null");

            var query = await this._roleManager.Roles.Where(e => ids.Contains(e.Id))
            .Select(e => new
            {
                id = this._userRoleRepository.GetAll().Count(w => w.RoleId == e.Id) == 0 ? e.Id : 0,
            }).ToArrayAsync();

            // var count = await this.userRoleRepository.GetAll().CountAsync(e => ids.Contains(e.RoleId));
            // this.Assert(count > 0, "Roles have used so can't delete");

            List<EntityDto<int>> listEntity = new List<EntityDto<int>>();
            foreach (var e in query)
            {
                if (e.id != 0)
                {
                    EntityDto<int> entity = new EntityDto<int>();
                    entity.Id = e.id;
                    listEntity.Add(entity);
                }
            }

            foreach (var e in listEntity)
            {
                await DeleteAsync(e);
            }

            string result = string.Empty;
            if (listEntity.Count == 0)
            {
                result = "Roles have used so can\'t delete";
            }
            else if (listEntity.Count == ids.Count)
            {
                result = "Delete Successfully";
            }
            else
            {
                result = $"Delete Successfully {ids.Count - listEntity.Count}/{ids.Count} Roles";
            }

            return result;
        }
    }
}

