using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using Nguyen_Tan_Phat_Project.Authorization.Roles;
using Nguyen_Tan_Phat_Project.Authorization.Users;
using Nguyen_Tan_Phat_Project.MultiTenancy;
using Nguyen_Tan_Phat_Project.Entities;

namespace Nguyen_Tan_Phat_Project.EntityFrameworkCore
{
    public class Nguyen_Tan_Phat_ProjectDbContext : AbpZeroDbContext<Tenant, Role, User, Nguyen_Tan_Phat_ProjectDbContext>
    {
        /* Define a DbSet for each entity of the application */
        public DbSet<Test> tests { get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<ExportImport> exportImport { get; set; }
        public DbSet<ExportImportProduct> exportImportProduct { get; set; }
        public DbSet<Product> products { get; set; }
        public DbSet<ProductStorage> productsStorage { get; set; }
        public DbSet<Storage> storage { get; set; }
        public DbSet<SubCategory> subCategories { get; set; }
        public DbSet<Customer> customers { get; set; }
        public DbSet<Employee> employees { get; set; }
        public DbSet<Structure> structure { get; set; }
        public DbSet<BankAccount> bankAccounts { get; set; }
        public DbSet<CMND> chungMinhND { get; set; }
        public DbSet<ExportImportCustomer> exportImportCustomer { get; set; }
        public DbSet<Expenses> expenses { get; set; }
        public DbSet<ProductExpenses> productExpenses { get; set; }
        public DbSet<Retail> retails { get; set; }
        public DbSet<RetailCustomer> retailsCustomer { get; set; }
        public DbSet<RetailProduct> retailProducts { get; set; }
        
        public Nguyen_Tan_Phat_ProjectDbContext(DbContextOptions<Nguyen_Tan_Phat_ProjectDbContext> options)
            : base(options)
        {
        }
    }
}
