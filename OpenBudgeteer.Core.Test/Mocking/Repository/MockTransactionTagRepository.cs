using System;
using System.Collections.Generic;
using System.Linq;
using OpenBudgeteer.Core.Data.Contracts.Repositories;
using OpenBudgeteer.Core.Data.Entities.Models;

namespace OpenBudgeteer.Core.Test.Mocking.Repository;

public class MockTransactionTagRepository : ITransactionTagRepository
{
    private readonly MockDatabase _mockDatabase;

    public MockTransactionTagRepository(MockDatabase mockDatabase)
    {
        _mockDatabase = mockDatabase;
    }

    public IQueryable<TransactionTag> All()
    {
        return _mockDatabase.TransactionTags.Values.AsQueryable();
    }

    public IQueryable<TransactionTag> AllWithIncludedEntities()
    {
        var transactionTags = All().ToList();
        foreach (var transactionTag in transactionTags)
        {
            IncludeEntities(transactionTag);
        }

        return transactionTags.AsQueryable();
    }

    public TransactionTag? ById(Guid id)
    {
        _mockDatabase.TransactionTags.TryGetValue(id, out var result);
        return result;
    }

    public TransactionTag? ByIdWithIncludedEntities(Guid id)
    {
        var transactionTag = ById(id);
        if (transactionTag is not null) IncludeEntities(transactionTag);
        return transactionTag;
    }

    public int Create(TransactionTag entity)
    {
        entity.Id = Guid.NewGuid();
        _mockDatabase.TransactionTags[entity.Id] = entity;
        return 1;
    }

    public int CreateRange(IEnumerable<TransactionTag> entities)
    {
        return entities.Sum(Create);
    }

    public int Update(TransactionTag entity)
    {
        _mockDatabase.TransactionTags[entity.Id] = entity;
        return 1;
    }

    public int UpdateRange(IEnumerable<TransactionTag> entities)
    {
        return entities.Sum(Update);
    }

    public int Delete(Guid id)
    {
        return _mockDatabase.TransactionTags.Remove(id) ? 1 : 0;
    }

    public int DeleteRange(IEnumerable<Guid> ids)
    {
        return ids.Sum(Delete);
    }

    private void IncludeEntities(TransactionTag transactionTag)
    {
        var mockTagRepository = new MockTagRepository(_mockDatabase);
        var mockBankTransactionRepository = new MockBankTransactionRepository(_mockDatabase);
        transactionTag.Tag = mockTagRepository.ById(transactionTag.TagId)
                             ?? throw new Exception("Tag doesn't exist");
        transactionTag.Transaction = mockBankTransactionRepository.ById(transactionTag.TransactionId)
                                     ?? throw new Exception("BankTransaction doesn't exist");
    }
}
