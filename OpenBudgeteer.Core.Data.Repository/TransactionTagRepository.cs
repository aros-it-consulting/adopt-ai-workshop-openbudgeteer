using Microsoft.EntityFrameworkCore;
using OpenBudgeteer.Core.Data.Contracts.Repositories;
using OpenBudgeteer.Core.Data.Entities;
using OpenBudgeteer.Core.Data.Entities.Models;

namespace OpenBudgeteer.Core.Data.Repository;

public class TransactionTagRepository : ITransactionTagRepository
{
    private DatabaseContext DatabaseContext { get; }

    public TransactionTagRepository(DatabaseContext databaseContext)
    {
        DatabaseContext = databaseContext;
    }

    public IQueryable<TransactionTag> All() => DatabaseContext.TransactionTag
        .AsNoTracking();

    public IQueryable<TransactionTag> AllWithIncludedEntities() => DatabaseContext.TransactionTag
        .Include(transactionTag => transactionTag.Tag)
        .Include(transactionTag => transactionTag.Transaction)
        .AsNoTracking();

    public TransactionTag? ById(Guid id) => DatabaseContext.TransactionTag
        .FirstOrDefault(transactionTag => transactionTag.Id == id);

    public TransactionTag? ByIdWithIncludedEntities(Guid id) => DatabaseContext.TransactionTag
        .Include(transactionTag => transactionTag.Tag)
        .Include(transactionTag => transactionTag.Transaction)
        .FirstOrDefault(transactionTag => transactionTag.Id == id);

    public int Create(TransactionTag entity)
    {
        DatabaseContext.TransactionTag.Add(entity);
        return DatabaseContext.SaveChanges();
    }

    public int CreateRange(IEnumerable<TransactionTag> entities)
    {
        DatabaseContext.TransactionTag.AddRange(entities);
        return DatabaseContext.SaveChanges();
    }

    public int Update(TransactionTag entity)
    {
        DatabaseContext.TransactionTag.Update(entity);
        return DatabaseContext.SaveChanges();
    }

    public int UpdateRange(IEnumerable<TransactionTag> entities)
    {
        DatabaseContext.TransactionTag.UpdateRange(entities);
        return DatabaseContext.SaveChanges();
    }

    public int Delete(Guid id)
    {
        var entity = DatabaseContext.TransactionTag.FirstOrDefault(transactionTag => transactionTag.Id == id);
        if (entity is null) throw new Exception($"TransactionTag with id {id} not found.");

        DatabaseContext.TransactionTag.Remove(entity);
        return DatabaseContext.SaveChanges();
    }

    public int DeleteRange(IEnumerable<Guid> ids)
    {
        var entities = DatabaseContext.TransactionTag.Where(transactionTag => ids.Contains(transactionTag.Id));
        if (!entities.Any()) throw new Exception($"No TransactionTags found with passed IDs.");

        DatabaseContext.TransactionTag.RemoveRange(entities);
        return DatabaseContext.SaveChanges();
    }
}
