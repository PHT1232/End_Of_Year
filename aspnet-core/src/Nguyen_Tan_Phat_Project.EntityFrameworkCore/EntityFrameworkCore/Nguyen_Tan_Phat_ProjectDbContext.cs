using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using Nguyen_Tan_Phat_Project.Authorization.Roles;
using Nguyen_Tan_Phat_Project.Authorization.Users;
using Nguyen_Tan_Phat_Project.MultiTenancy;

namespace Nguyen_Tan_Phat_Project.EntityFrameworkCore
{
    public class Nguyen_Tan_Phat_ProjectDbContext : AbpZeroDbContext<Tenant, Role, User, Nguyen_Tan_Phat_ProjectDbContext>
    {
        /* Define a DbSet for each entity of the application */
        
        public Nguyen_Tan_Phat_ProjectDbContext(DbContextOptions<Nguyen_Tan_Phat_ProjectDbContext> options)
            : base(options)
        {
        }
    }
}
