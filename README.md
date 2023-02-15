# **Run Migrations**

- Open Package Manager Console, select MultipleDbContextEfCoreDemo.EntityFrameworkCore as default project.
- **Add-Migration -Context HoChiMinhDbContext Initial_1**
- **Add-Migration -Context Nguyen_Tan_Phat_ProjectDbContext Initial_2**
- **Update-Database -Context HoChiMinhDbContext**
- **Update-Database -Context Nguyen_Tan_Phat_ProjectDbContext**
