using System;
using System.Collections.Generic;
using System.Linq;
using OpenBudgeteer.Core.Data.Entities.Models;
using OpenBudgeteer.Core.Test.Mocking;
using OpenBudgeteer.Core.Test.Mocking.Services;
using OpenBudgeteer.Core.ViewModels.Helper;
using Xunit;

namespace OpenBudgeteer.Core.Test.Tests.PageViewModels;

public class TagListingViewModelTest
{
    private const string ItalyTag = "Italy";
    private const string WorkTag = "Work";
    private const string UnusedTag = "Unused";
    private const string HotelMemo = "Hotel";
    private const string TrainMemo = "Train";

    /// <summary>
    /// Creates a Hotel Transaction tagged Italy, a Train Transaction tagged Italy and Work, and an unused Tag
    /// </summary>
    private static MockServiceManager SetupTestData()
    {
        var serviceManager = new MockServiceManager(new MockDatabase());
        var account = new Account { IsActive = 1, Name = "Account" };
        serviceManager.AccountService.Create(account);
        serviceManager.TagService.Create(new Tag { Name = UnusedTag });
        CreateTaggedTransaction(serviceManager, account, HotelMemo, ItalyTag);
        CreateTaggedTransaction(serviceManager, account, TrainMemo, ItalyTag, WorkTag);
        return serviceManager;
    }

    private static void CreateTaggedTransaction(
        MockServiceManager serviceManager, Account account, string memo, params string[] tagNames)
    {
        var transactionTags = tagNames
            .Select(tagName => new TransactionTag { Tag = new Tag { Name = tagName } })
            .ToList();
        var transaction = new BankTransaction
        {
            AccountId = account.Id,
            TransactionDate = DateOnly.FromDateTime(DateTime.Today),
            Memo = memo,
            TransactionTags = transactionTags
        };
        serviceManager.BankTransactionService.Create(transaction);
    }

    private static IEnumerable<string> GetTagNamesOfTransaction(MockServiceManager serviceManager, string memo)
    {
        var transaction = serviceManager.BankTransactionService
            .GetAll(null, null)
            .Single(bankTransaction => bankTransaction.Memo == memo);
        return transaction.TransactionTags!
            .Select(transactionTag => transactionTag.Tag!.Name)
            .Order();
    }

    private static TagListingViewModel LoadTagListing(MockServiceManager serviceManager)
    {
        var viewModel = new TagListingViewModel(serviceManager);
        viewModel.LoadData();
        return viewModel;
    }

    [Fact]
    public void LoadData_ListsTagsByNameWithTheirTransactionCount()
    {
        var serviceManager = SetupTestData();

        var viewModel = new TagListingViewModel(serviceManager);
        var result = viewModel.LoadData();

        Assert.True(result.IsSuccessful);
        Assert.Equal(new[] { ItalyTag, UnusedTag, WorkTag }, viewModel.Tags.Select(tag => tag.Name));
        Assert.Equal(new[] { 2, 0, 1 }, viewModel.Tags.Select(tag => tag.TransactionCount));
    }

    [Fact]
    public void RenameTag_TransactionsShowTheNewName()
    {
        const string newName = "Italy 2026";
        var serviceManager = SetupTestData();
        var italy = LoadTagListing(serviceManager).Tags.Single(tag => tag.Name == ItalyTag);

        italy.Name = newName;
        var result = italy.RenameTag();

        Assert.True(result.IsSuccessful);
        Assert.True(result.ViewModelReloadRequired);
        Assert.Equal(new[] { newName }, GetTagNamesOfTransaction(serviceManager, HotelMemo));
        Assert.Equal(new[] { newName, WorkTag }, GetTagNamesOfTransaction(serviceManager, TrainMemo));
    }

    [Fact]
    public void RenameTag_AllowsChangingTheCaseOfTheName()
    {
        var serviceManager = SetupTestData();
        var italy = LoadTagListing(serviceManager).Tags.Single(tag => tag.Name == ItalyTag);

        italy.Name = ItalyTag.ToUpper();
        var result = italy.RenameTag();

        Assert.True(result.IsSuccessful);
        Assert.Equal(new[] { ItalyTag.ToUpper() }, GetTagNamesOfTransaction(serviceManager, HotelMemo));
    }

    [Theory]
    [InlineData(WorkTag)]
    [InlineData(" work ")]
    [InlineData("")]
    [InlineData("   ")]
    public void RenameTag_FailsAndKeepsTheNameIfItIsEmptyOrAlreadyUsed(string newName)
    {
        var serviceManager = SetupTestData();
        var italy = LoadTagListing(serviceManager).Tags.Single(tag => tag.Name == ItalyTag);

        italy.Name = newName;
        var result = italy.RenameTag();

        Assert.False(result.IsSuccessful);
        Assert.Equal(new[] { ItalyTag, UnusedTag, WorkTag }, LoadTagListing(serviceManager).Tags.Select(tag => tag.Name));
        Assert.Equal(new[] { ItalyTag, WorkTag }, GetTagNamesOfTransaction(serviceManager, TrainMemo));
    }

    [Fact]
    public void DeleteTag_RemovesTheTagFromItsTransactionsAndKeepsThem()
    {
        var serviceManager = SetupTestData();
        var italy = LoadTagListing(serviceManager).Tags.Single(tag => tag.Name == ItalyTag);

        var result = italy.DeleteTag();

        Assert.True(result.IsSuccessful);
        Assert.True(result.ViewModelReloadRequired);
        Assert.Equal(new[] { UnusedTag, WorkTag }, LoadTagListing(serviceManager).Tags.Select(tag => tag.Name));
        Assert.Empty(GetTagNamesOfTransaction(serviceManager, HotelMemo));
        Assert.Equal(new[] { WorkTag }, GetTagNamesOfTransaction(serviceManager, TrainMemo));
    }
}
