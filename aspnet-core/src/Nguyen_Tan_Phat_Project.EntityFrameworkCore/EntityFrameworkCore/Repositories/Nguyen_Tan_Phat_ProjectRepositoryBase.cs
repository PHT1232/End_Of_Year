using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Repositories;

namespace Nguyen_Tan_Phat_Project.EntityFrameworkCore.Repositories
{
    /// <summary>
    /// Base class for custom repositories of the application.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key type of the entity</typeparam>
    public abstract class Nguyen_Tan_Phat_ProjectRepositoryBase<TEntity, TPrimaryKey> : EfCoreRepositoryBase<Nguyen_Tan_Phat_ProjectDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected Nguyen_Tan_Phat_ProjectRepositoryBase(IDbContextProvider<Nguyen_Tan_Phat_ProjectDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        // Add your common methods for all repositories
    }

    /// <summary>
    /// Base class for custom repositories of the application.
    /// This is a shortcut of <see cref="Nguyen_Tan_Phat_ProjectRepositoryBase{TEntity,TPrimaryKey}"/> for <see cref="int"/> primary key.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public abstract class Nguyen_Tan_Phat_ProjectRepositoryBase<TEntity> : Nguyen_Tan_Phat_ProjectRepositoryBase<TEntity, int>, IRepository<TEntity>
        where TEntity : class, IEntity<int>
    {
        protected Nguyen_Tan_Phat_ProjectRepositoryBase(IDbContextProvider<Nguyen_Tan_Phat_ProjectDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        // Do not add any method here, add to the class above (since this inherits it)!!!
    }
}
