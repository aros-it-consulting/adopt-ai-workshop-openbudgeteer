using System;
using System.Collections.Generic;
using System.Linq;
using OpenBudgeteer.Core.Data.Contracts.Repositories;
using OpenBudgeteer.Core.Data.Entities.Models;
using OpenBudgeteer.Core.Data.Repository;
using OpenBudgeteer.Core.Test.Common;
using OpenBudgeteer.Core.Test.Mocking;
using OpenBudgeteer.Core.Test.Mocking.Repository;
using Xunit;

namespace OpenBudgeteer.Core.Test.Tests.Database;

public class TransactionTagDatabaseTest : BaseDatabaseTest<TransactionTag>
{
    private Tag _testTag1 = new();
    private Tag _testTag2 = new();

    protected override void CompareEntities(TransactionTag expected, TransactionTag actual)
    {
        Assert.Equal(expected.TransactionId, actual.TransactionId);
        Assert.Equal(expected.TagId, actual.TagId);
    }

    public static IEnumerable<object[]> TestData_Repository
    {
        get
        {
            var mockDatabase = new MockDatabase();
            var databaseContext = GetInMemoryContext();
            return
            [
                [
                    new MockTransactionTagRepository(mockDatabase),
                    new MockTagRepository(mockDatabase),
                    new MockBankTransactionRepository(mockDatabase),
                    new MockAccountRepository(mockDatabase)
                ],
                [
                    new TransactionTagRepository(databaseContext),
                    new TagRepository(databaseContext),
                    new BankTransactionRepository(databaseContext),
                    new AccountRepository(databaseContext)
                ]
            ];
        }
    }

    private List<TransactionTag> SetupTestData(
        ITransactionTagRepository transactionTagRepository,
        ITagRepository tagRepository,
        IBankTransactionRepository bankTransactionRepository,
        IAccountRepository accountRepository)
    {
        DeleteAllExtension<ITransactionTagRepository, TransactionTag>.DeleteAll(transactionTagRepository);
        DeleteAllExtension<ITagRepository, Tag>.DeleteAll(tagRepository);
        DeleteAllExtension<IBankTransactionRepository, BankTransaction>.DeleteAll(bankTransactionRepository);
        DeleteAllExtension<IAccountRepository, Account>.DeleteAll(accountRepository);

        var account = TestDataGenerator.Current.GenerateAccount();
        accountRepository.Create(account);
        var transaction1 = TestDataGenerator.Current.GenerateBankTransaction(account);
        var transaction2 = TestDataGenerator.Current.GenerateBankTransaction(account);
        bankTransactionRepository.Create(transaction1);
        bankTransactionRepository.Create(transaction2);
        _testTag1 = TestDataGenerator.Current.GenerateTag();
        _testTag2 = TestDataGenerator.Current.GenerateTag();
        tagRepository.Create(_testTag1);
        tagRepository.Create(_testTag2);

        var transactionTags = new List<TransactionTag>
        {
            new() { TransactionId = transaction1.Id, TagId = _testTag1.Id },
            new() { TransactionId = transaction1.Id, TagId = _testTag2.Id },
            new() { TransactionId = transaction2.Id, TagId = _testTag1.Id }
        };
        foreach (var transactionTag in transactionTags)
        {
            var result = transactionTagRepository.Create(transactionTag);
            Assert.Equal(1, result);
            Assert.NotEqual(Guid.Empty, transactionTag.Id);
        }

        return transactionTags;
    }

    [Theory]
    [MemberData(nameof(TestData_Repository))]
    public void Create(
        ITransactionTagRepository transactionTagRepository,
        ITagRepository tagRepository,
        IBankTransactionRepository bankTransactionRepository,
        IAccountRepository accountRepository)
    {
        var transactionTags = SetupTestData(
            transactionTagRepository, tagRepository, bankTransactionRepository, accountRepository);
        RunChecks(transactionTagRepository, transactionTags);

        DeleteAllExtension<ITransactionTagRepository, TransactionTag>.DeleteAll(transactionTagRepository);
    }

    [Theory]
    [MemberData(nameof(TestData_Repository))]
    public void Update(
        ITransactionTagRepository transactionTagRepository,
        ITagRepository tagRepository,
        IBankTransactionRepository bankTransactionRepository,
        IAccountRepository accountRepository)
    {
        var transactionTags = SetupTestData(
            transactionTagRepository, tagRepository, bankTransactionRepository, accountRepository);

        var transactionTag = transactionTags.Last();
        var transactionTagId = transactionTag.Id;
        transactionTag.TagId = _testTag2.Id;
        var result = transactionTagRepository.Update(transactionTag);
        Assert.Equal(1, result);
        Assert.Equal(transactionTagId, transactionTag.Id); // Check if no new Guid has been generated (no CREATE)

        RunChecks(transactionTagRepository, transactionTags);

        DeleteAllExtension<ITransactionTagRepository, TransactionTag>.DeleteAll(transactionTagRepository);
    }

    [Theory]
    [MemberData(nameof(TestData_Repository))]
    public void Delete(
        ITransactionTagRepository transactionTagRepository,
        ITagRepository tagRepository,
        IBankTransactionRepository bankTransactionRepository,
        IAccountRepository accountRepository)
    {
        var transactionTags = SetupTestData(
            transactionTagRepository, tagRepository, bankTransactionRepository, accountRepository);

        var deleteResult1 = transactionTagRepository.Delete(transactionTags.First().Id);
        var deleteResult2 = transactionTagRepository.Delete(transactionTags.Last().Id);
        Assert.Equal(1, deleteResult1);
        Assert.Equal(1, deleteResult2);
        transactionTags.Remove(transactionTags.First());
        transactionTags.Remove(transactionTags.Last());

        RunChecks(transactionTagRepository, transactionTags);
        Assert.Equal(2, tagRepository.All().Count()); // Tags are kept

        DeleteAllExtension<ITransactionTagRepository, TransactionTag>.DeleteAll(transactionTagRepository);
    }
}
