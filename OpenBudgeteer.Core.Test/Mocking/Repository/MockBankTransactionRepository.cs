using System;
using System.Collections.Generic;
using System.Linq;
using OpenBudgeteer.Core.Data.Contracts.Repositories;
using OpenBudgeteer.Core.Data.Entities.Models;

namespace OpenBudgeteer.Core.Test.Mocking.Repository;

public class MockBankTransactionRepository : IBankTransactionRepository
{
    private readonly MockDatabase _mockDatabase;

    public MockBankTransactionRepository(MockDatabase mockDatabase)
    {
        _mockDatabase = mockDatabase;
    }

    public IQueryable<BankTransaction> All()
    {
        return _mockDatabase.BankTransactions.Values.AsQueryable();
    }

    public IQueryable<BankTransaction> AllWithIncludedEntities()
    {
        var mockAccountRepository = new MockAccountRepository(_mockDatabase);
        var mockBudgetedTransactionRepository = new MockBudgetedTransactionRepository(_mockDatabase);
        var transactions = All().ToList();
        foreach (var transaction in transactions)
        {
            transaction.Account = mockAccountRepository.ById(transaction.AccountId) 
                                  ?? throw new Exception("Account doesn't exist");
            transaction.BudgetedTransactions = new List<BudgetedTransaction>();
            foreach (var budgetedTransaction in mockBudgetedTransactionRepository
                         .All()
                         .Where(i => i.TransactionId == transaction.Id))
            {
                transaction.BudgetedTransactions.Add(budgetedTransaction); 
            }
            IncludeTransactionTags(transaction);
        }

        return transactions.AsQueryable();
    }

    public BankTransaction? ById(Guid id)
    {
        _mockDatabase.BankTransactions.TryGetValue(id, out var result);
        return result;
    }

    public BankTransaction? ByIdWithIncludedEntities(Guid id)
    {
        var mockAccountRepository = new MockAccountRepository(_mockDatabase);
        var mockBudgetedTransactionRepository = new MockBudgetedTransactionRepository(_mockDatabase);
        var transaction = ById(id);
        if (transaction is null) return transaction;
        transaction.Account = mockAccountRepository.ById(transaction.AccountId) 
                              ?? throw new Exception("Account doesn't exist");
        transaction.BudgetedTransactions = new List<BudgetedTransaction>();
        foreach (var budgetedTransaction in mockBudgetedTransactionRepository
                     .All()
                     .Where(i => i.TransactionId == transaction.Id))
        {
            transaction.BudgetedTransactions.Add(budgetedTransaction); 
        }
        IncludeTransactionTags(transaction);
        return transaction;
    }

    public int Create(BankTransaction entity)
    {
        entity.Id = Guid.NewGuid();
        _mockDatabase.BankTransactions[entity.Id] = entity;
        SaveTransactionTags(entity);
        return 1;
    }

    public int CreateRange(IEnumerable<BankTransaction> entities)
    {
        return entities.Sum(Create);
    }

    public int Update(BankTransaction entity)
    {
        try
        {
            _mockDatabase.BankTransactions[entity.Id] = entity;
            SaveTransactionTags(entity);
            return 1;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return 0;
        }
    }

    public int UpdateRange(IEnumerable<BankTransaction> entities)
    {
        return entities.Sum(Update);
    }

    public int Delete(Guid id)
    {
        try
        {
            return _mockDatabase.BankTransactions.Remove(id) ? 1 : 0;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return 0;
        }
    }

    public int DeleteRange(IEnumerable<Guid> ids)
    {
        return ids.Sum(Delete);
    }

    private void IncludeTransactionTags(BankTransaction transaction)
    {
        var mockTagRepository = new MockTagRepository(_mockDatabase);
        transaction.TransactionTags = _mockDatabase.TransactionTags.Values
            .Where(transactionTag => transactionTag.TransactionId == transaction.Id)
            .ToList();
        foreach (var transactionTag in transaction.TransactionTags)
        {
            transactionTag.Tag = mockTagRepository.ById(transactionTag.TagId)
                                 ?? throw new Exception("Tag doesn't exist");
        }
    }

    // Like EF Core, which stores the Tag assignments passed along with the Transaction
    private void SaveTransactionTags(BankTransaction transaction)
    {
        var mockTransactionTagRepository = new MockTransactionTagRepository(_mockDatabase);
        foreach (var transactionTag in transaction.TransactionTags ?? [])
        {
            transactionTag.TransactionId = transaction.Id;
            if (transactionTag.Id == Guid.Empty) transactionTag.Id = Guid.NewGuid();
            mockTransactionTagRepository.Update(transactionTag);
        }
    }
}