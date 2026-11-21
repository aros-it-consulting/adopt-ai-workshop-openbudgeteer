using Microsoft.EntityFrameworkCore;
using OpenBudgeteer.Core.Data.Contracts.Repositories;
using OpenBudgeteer.Core.Data.Entities;
using OpenBudgeteer.Core.Data.Entities.Models;

namespace OpenBudgeteer.Core.Data.Repository;

public class TagRepository : ITagRepository
{
    private DatabaseContext DatabaseContext { get; }

    public TagRepository(DatabaseContext databaseContext)
    {
        DatabaseContext = databaseContext;
    }

    public IQueryable<Tag> All() => DatabaseContext.Tag
        .AsNoTracking();

    public IQueryable<Tag> AllWithIncludedEntities() => DatabaseContext.Tag
        .Include(tag => tag.TransactionTags)
        .AsNoTracking();

    public Tag? ById(Guid id) => DatabaseContext.Tag
        .FirstOrDefault(tag => tag.Id == id);

    public Tag? ByIdWithIncludedEntities(Guid id) => DatabaseContext.Tag
        .Include(tag => tag.TransactionTags)
        .FirstOrDefault(tag => tag.Id == id);

    public int Create(Tag entity)
    {
        DatabaseContext.Tag.Add(entity);
        return DatabaseContext.SaveChanges();
    }

    public int CreateRange(IEnumerable<Tag> entities)
    {
        DatabaseContext.Tag.AddRange(entities);
        return DatabaseContext.SaveChanges();
    }

    public int Update(Tag entity)
    {
        DatabaseContext.Tag.Update(entity);
        return DatabaseContext.SaveChanges();
    }

    public int UpdateRange(IEnumerable<Tag> entities)
    {
        DatabaseContext.Tag.UpdateRange(entities);
        return DatabaseContext.SaveChanges();
    }

    public int Delete(Guid id)
    {
        // Assignments are loaded so that their removal is tracked too, not only left to the database cascade
        var entity = DatabaseContext.Tag
            .Include(tag => tag.TransactionTags)
            .FirstOrDefault(tag => tag.Id == id);
        if (entity is null) throw new Exception($"Tag with id {id} not found.");

        DatabaseContext.Tag.Remove(entity);
        return DatabaseContext.SaveChanges();
    }

    public int DeleteRange(IEnumerable<Guid> ids)
    {
        var entities = DatabaseContext.Tag
            .Include(tag => tag.TransactionTags)
            .Where(tag => ids.Contains(tag.Id));
        if (!entities.Any()) throw new Exception($"No Tags found with passed IDs.");

        DatabaseContext.Tag.RemoveRange(entities);
        return DatabaseContext.SaveChanges();
    }
}
