using Abp.Zero.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Nguyen_Tan_Phat_Project.Authorization.Roles;
using Nguyen_Tan_Phat_Project.Authorization.Users;
using Nguyen_Tan_Phat_Project.MultiTenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.EntityFrameworkCore
{
    public class HoChiMinhDbContext : AbpZeroDbContext<Tenant, Role, User, HoChiMinhDbContext>
    {
        public HoChiMinhDbContext(DbContextOptions<HoChiMinhDbContext> options) : base(options) { }
    }
}
