using LogicPOS.Domain.Entities.Common;
using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;

using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Repositories.Common;

public abstract partial class Repository
{
    public abstract class WithDesignation<TEntity> : Repository<TEntity>, IRepository.IWithDesignation where TEntity : Entity.WithCode.AndOrder.AndDesignation
    {
        protected WithDesignation(LogicPOSDbContext database) : base(database)
        {
        
        }

        public async Task<bool> DesignationExistsAsync(string designation, CancellationToken cancellationToken = default)
        {
            return await Database.Set<TEntity>().AnyAsync(
                x => x.Designation == designation,
                cancellationToken);
        }
    }
}

