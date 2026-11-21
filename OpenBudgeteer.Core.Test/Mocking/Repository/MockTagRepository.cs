using System;
using System.Collections.Generic;
using System.Linq;
using OpenBudgeteer.Core.Data.Contracts.Repositories;
using OpenBudgeteer.Core.Data.Entities.Models;

namespace OpenBudgeteer.Core.Test.Mocking.Repository;

public class MockTagRepository : ITagRepository
{
    private readonly MockDatabase _mockDatabase;

    public MockTagRepository(MockDatabase mockDatabase)
    {
        _mockDatabase = mockDatabase;
    }

    public IQueryable<Tag> All()
    {
        return _mockDatabase.Tags.Values.AsQueryable();
    }

    public IQueryable<Tag> AllWithIncludedEntities()
    {
        var tags = All().ToList();
        foreach (var tag in tags)
        {
            IncludeTransactionTags(tag);
        }

        return tags.AsQueryable();
    }

    public Tag? ById(Guid id)
    {
        _mockDatabase.Tags.TryGetValue(id, out var result);
        return result;
    }

    public Tag? ByIdWithIncludedEntities(Guid id)
    {
        var tag = ById(id);
        if (tag is not null) IncludeTransactionTags(tag);
        return tag;
    }

    public int Create(Tag entity)
    {
        entity.Id = Guid.NewGuid();
        _mockDatabase.Tags[entity.Id] = entity;
        return 1;
    }

    public int CreateRange(IEnumerable<Tag> entities)
    {
        return entities.Sum(Create);
    }

    public int Update(Tag entity)
    {
        _mockDatabase.Tags[entity.Id] = entity;
        return 1;
    }

    public int UpdateRange(IEnumerable<Tag> entities)
    {
        return entities.Sum(Update);
    }

    public int Delete(Guid id)
    {
        // Like the ON DELETE CASCADE of the database: the Tag is removed from its Transactions
        var mockTransactionTagRepository = new MockTransactionTagRepository(_mockDatabase);
        var transactionTagIds = _mockDatabase.TransactionTags.Values
            .Where(transactionTag => transactionTag.TagId == id)
            .Select(transactionTag => transactionTag.Id)
            .ToList();
        mockTransactionTagRepository.DeleteRange(transactionTagIds);
        return _mockDatabase.Tags.Remove(id) ? 1 : 0;
    }

    public int DeleteRange(IEnumerable<Guid> ids)
    {
        return ids.Sum(Delete);
    }

    private void IncludeTransactionTags(Tag tag)
    {
        tag.TransactionTags = _mockDatabase.TransactionTags.Values
            .Where(transactionTag => transactionTag.TagId == tag.Id)
            .ToList();
    }
}
