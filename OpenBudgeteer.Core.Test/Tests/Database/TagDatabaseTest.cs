using System;
using System.Collections.Generic;
using System.Linq;
using OpenBudgeteer.Core.Data.Contracts.Repositories;
using OpenBudgeteer.Core.Data.Entities.Models;
using OpenBudgeteer.Core.Data.Repository;
using OpenBudgeteer.Core.Data.Services.Generic;
using OpenBudgeteer.Core.Test.Common;
using OpenBudgeteer.Core.Test.Mocking;
using OpenBudgeteer.Core.Test.Mocking.Repository;
using Xunit;

namespace OpenBudgeteer.Core.Test.Tests.Database;

public class TagDatabaseTest : BaseDatabaseTest<Tag>
{
    private const int TagCount = 4;

    protected override void CompareEntities(Tag expected, Tag actual)
    {
        Assert.Equal(expected.Name, actual.Name);
    }

    public static IEnumerable<object[]> TestData_Repository =>
    [
        [new MockTagRepository(new MockDatabase())],
        [new TagRepository(GetInMemoryContext())]
    ];

    public static IEnumerable<object[]> TestData_RepositoryWithTransactions
    {
        get
        {
            var mockDatabase = new MockDatabase();
            var databaseContext = GetInMemoryContext();
            return
            [
                [
                    new MockTagRepository(mockDatabase),
                    new MockTransactionTagRepository(mockDatabase),
                    new MockBankTransactionRepository(mockDatabase),
                    new MockAccountRepository(mockDatabase)
                ],
                [
                    new TagRepository(databaseContext),
                    new TransactionTagRepository(databaseContext),
                    new BankTransactionRepository(databaseContext),
                    new AccountRepository(databaseContext)
                ]
            ];
        }
    }

    private List<Tag> SetupTestData(ITagRepository tagRepository)
    {
        DeleteAllExtension<ITagRepository, Tag>.DeleteAll(tagRepository);

        var result = new List<Tag>();
        for (var index = 1; index <= TagCount; index++)
        {
            var tag = TestDataGenerator.Current.GenerateTag();
            result.Add(tag);
            var repositoryResult = tagRepository.Create(tag);
            Assert.Equal(1, repositoryResult);
            Assert.NotEqual(Guid.Empty, tag.Id);
        }

        return result;
    }

    [Theory]
    [MemberData(nameof(TestData_Repository))]
    public void Create(ITagRepository tagRepository)
    {
        var tags = SetupTestData(tagRepository);
        RunChecks(tagRepository, tags);

        DeleteAllExtension<ITagRepository, Tag>.DeleteAll(tagRepository);
    }

    [Theory]
    [MemberData(nameof(TestData_Repository))]
    public void Update(ITagRepository tagRepository)
    {
        var tags = SetupTestData(tagRepository);

        foreach (var tag in tags)
        {
            var tagId = tag.Id;
            tag.Name += "Update";
            var result = tagRepository.Update(tag);
            Assert.Equal(1, result);
            Assert.Equal(tagId, tag.Id); // Check if no new Guid has been generated (no CREATE)
        }

        RunChecks(tagRepository, tags);

        DeleteAllExtension<ITagRepository, Tag>.DeleteAll(tagRepository);
    }

    [Theory]
    [MemberData(nameof(TestData_Repository))]
    public void Delete(ITagRepository tagRepository)
    {
        var tags = SetupTestData(tagRepository);

        var deleteResult1 = tagRepository.Delete(tags.First().Id);
        var deleteResult2 = tagRepository.Delete(tags.Last().Id);
        Assert.Equal(1, deleteResult1);
        Assert.Equal(1, deleteResult2);
        tags.Remove(tags.First());
        tags.Remove(tags.Last());

        RunChecks(tagRepository, tags);
        Assert.Equal(tags.Count, tagRepository.All().Count());

        DeleteAllExtension<ITagRepository, Tag>.DeleteAll(tagRepository);
    }

    [Theory]
    [MemberData(nameof(TestData_RepositoryWithTransactions))]
    public void Delete_RemovesTagFromItsTransactionsAndKeepsThem(
        ITagRepository tagRepository,
        ITransactionTagRepository transactionTagRepository,
        IBankTransactionRepository bankTransactionRepository,
        IAccountRepository accountRepository)
    {
        var account = TestDataGenerator.Current.GenerateAccount();
        accountRepository.Create(account);
        var transaction1 = TestDataGenerator.Current.GenerateBankTransaction(account);
        var transaction2 = TestDataGenerator.Current.GenerateBankTransaction(account);
        bankTransactionRepository.Create(transaction1);
        bankTransactionRepository.Create(transaction2);
        var deletedTag = TestDataGenerator.Current.GenerateTag();
        var keptTag = TestDataGenerator.Current.GenerateTag();
        tagRepository.Create(deletedTag);
        tagRepository.Create(keptTag);
        var keptTransactionTag = new TransactionTag { TransactionId = transaction1.Id, TagId = keptTag.Id };
        transactionTagRepository.Create(new TransactionTag { TransactionId = transaction1.Id, TagId = deletedTag.Id });
        transactionTagRepository.Create(new TransactionTag { TransactionId = transaction2.Id, TagId = deletedTag.Id });
        transactionTagRepository.Create(keptTransactionTag);

        var result = tagRepository.Delete(deletedTag.Id);

        Assert.True(result > 0);
        Assert.Equal([keptTag.Id], tagRepository.All().Select(tag => tag.Id));
        Assert.Equal([keptTransactionTag.Id], transactionTagRepository.All().Select(transactionTag => transactionTag.Id));
        Assert.Equal(2, bankTransactionRepository.All().Count());
    }

    [Fact]
    public void GetAll_LeavesOutTheAssignmentsLoadedByGetAllWithEntities()
    {
        var databaseContext = GetInMemoryContext();
        var tagRepository = new TagRepository(databaseContext);
        var account = TestDataGenerator.Current.GenerateAccount();
        new AccountRepository(databaseContext).Create(account);
        var transaction = TestDataGenerator.Current.GenerateBankTransaction(account);
        new BankTransactionRepository(databaseContext).Create(transaction);
        var tag = TestDataGenerator.Current.GenerateTag();
        tagRepository.Create(tag);
        var transactionTag = new TransactionTag { TransactionId = transaction.Id, TagId = tag.Id };
        new TransactionTagRepository(databaseContext).Create(transactionTag);
        var tagService = new GenericTagService(tagRepository);

        Assert.Null(tagService.GetAll().Single().TransactionTags);
        Assert.Single(tagService.GetAllWithEntities().Single().TransactionTags!);
    }
}
