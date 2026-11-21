using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using OpenBudgeteer.Core.Data.Entities.Models;
using OpenBudgeteer.Core.Test.Mocking;
using OpenBudgeteer.Core.Test.Mocking.Services;
using OpenBudgeteer.Core.ViewModels.EntityViewModels;
using OpenBudgeteer.Core.ViewModels.Helper;
using OpenBudgeteer.Core.ViewModels.PageViewModels;
using Xunit;

namespace OpenBudgeteer.Core.Test.Tests.PageViewModels;

public class TransactionPageViewModelTest
{
    public static IEnumerable<object[]> TestData_ProposeBuckets_CheckOrder =>
    [
        [

        ]
    ];
    
    public static IEnumerable<object[]> TestData_ProposeBuckets_CheckValidBucket =>
    [
        [

        ]
    ];

    public async Task ProposeBuckets_CheckOrder()
    {
        var serviceManager = new MockServiceManager(new MockDatabase());
        var testAccount = new Account() { IsActive = 1, Name = "Account" };
        
        serviceManager.AccountService.Create(testAccount);
        //TODO: Create Test Cases for Propose Buckets
    }
    
    public static IEnumerable<object[]> TestData_AddRecurringTransactionsAsync_CheckRecurrance =>
    [
        // Every week, starting 1.1.2010
        [
            new DateOnly(2010,1,1), 1, 1, "Every week",
            new[]
            {
                new Tuple<int, int, DateOnly[]>(2010, 1, [
                    new DateOnly(2010,1,1),
                    new DateOnly(2010,1,8),
                    new DateOnly(2010,1,15),
                    new DateOnly(2010,1,22),
                    new DateOnly(2010,1,29)
                ]),
                new Tuple<int, int, DateOnly[]>(2010, 2, [
                    new DateOnly(2010,2,5),
                    new DateOnly(2010,2,12),
                    new DateOnly(2010,2,19),
                    new DateOnly(2010,2,26)
                ]),
                new Tuple<int, int, DateOnly[]>(2010, 3, [
                    new DateOnly(2010,3,5),
                    new DateOnly(2010,3,12),
                    new DateOnly(2010,3,19),
                    new DateOnly(2010,3,26)
                ])
            }
        ],
        // Every 2 week, starting 1.1.2010
        [
            new DateOnly(2010,1,1), 1, 2, "Every 2 weeks",
            new[]
            {
                new Tuple<int, int, DateOnly[]>(2010, 1, [
                    new DateOnly(2010,1,1),
                    new DateOnly(2010,1,15),
                    new DateOnly(2010,1,29)
                ]),
                new Tuple<int, int, DateOnly[]>(2010, 2, [
                    new DateOnly(2010,2,12),
                    new DateOnly(2010,2,26)
                ]),
                new Tuple<int, int, DateOnly[]>(2010, 3, [
                    new DateOnly(2010,3,12),
                    new DateOnly(2010,3,26)
                ])
            }
        ],
        // Every 5 week, starting 1.1.2010
        [
            new DateOnly(2010,1,1), 1, 5, "Every 5 weeks",
            new[]
            {
                new Tuple<int, int, DateOnly[]>(2010, 1, [
                    new DateOnly(2010,1,1)
                ]),
                new Tuple<int, int, DateOnly[]>(2010, 2, [
                    new DateOnly(2010,2,5)
                ]),
                new Tuple<int, int, DateOnly[]>(2010, 3, [
                    new DateOnly(2010,3,12)
                ])
            }
        ],
        // Every month, starting 15.1.2010
        [
            new DateOnly(2010,1,15), 2, 1, "Every month",
            new[]
            {
                new Tuple<int, int, DateOnly[]>(2010, 1, [new DateOnly(2010,1,15)]),
                new Tuple<int, int, DateOnly[]>(2010, 2, [new DateOnly(2010,2,15)]),
                new Tuple<int, int, DateOnly[]>(2010, 3, [new DateOnly(2010,3,15)]),
                new Tuple<int, int, DateOnly[]>(2010, 4, [new DateOnly(2010,4,15)]),
                new Tuple<int, int, DateOnly[]>(2010, 5, [new DateOnly(2010,5,15)]),
                new Tuple<int, int, DateOnly[]>(2010, 6, [new DateOnly(2010,6,15)])
            }
        ],
        // Every 2 month, starting 15.1.2010
        [
            new DateOnly(2010,1,15), 2, 2, "Every 2 month",
            new[]
            {
                new Tuple<int, int, DateOnly[]>(2010, 1, [new DateOnly(2010,1,15)]),
                new Tuple<int, int, DateOnly[]>(2010, 2, []),
                new Tuple<int, int, DateOnly[]>(2010, 3, [new DateOnly(2010,3,15)]),
                new Tuple<int, int, DateOnly[]>(2010, 4, []),
                new Tuple<int, int, DateOnly[]>(2010, 5, [new DateOnly(2010,5,15)]),
                new Tuple<int, int, DateOnly[]>(2010, 6, [])
            }
        ],
        // Every 5 month, starting 15.1.2010
        [
            new DateOnly(2010,1,15), 2, 5, "Every 5 month",
            new[]
            {
                new Tuple<int, int, DateOnly[]>(2010, 1, [new DateOnly(2010,1,15)]),
                new Tuple<int, int, DateOnly[]>(2010, 2, []),
                new Tuple<int, int, DateOnly[]>(2010, 3, []),
                new Tuple<int, int, DateOnly[]>(2010, 4, []),
                new Tuple<int, int, DateOnly[]>(2010, 5, []),
                new Tuple<int, int, DateOnly[]>(2010, 6, [new DateOnly(2010,6,15)])
            }
        ],
        // Every quarter, starting 15.1.2010
        [
            new DateOnly(2010,1,15), 3, 1, "Every quarter",
            new[]
            {
                new Tuple<int, int, DateOnly[]>(2010, 1, [new DateOnly(2010,1,15)]),
                new Tuple<int, int, DateOnly[]>(2010, 2, []),
                new Tuple<int, int, DateOnly[]>(2010, 3, []),
                new Tuple<int, int, DateOnly[]>(2010, 4, [new DateOnly(2010,4,15)]),
                new Tuple<int, int, DateOnly[]>(2010, 5, []),
                new Tuple<int, int, DateOnly[]>(2010, 6, [])
            }
        ],
        // Every 2 quarter, starting 15.1.2010
        [
            new DateOnly(2010,1,15), 3, 2, "Every 2 quarter",
            new[]
            {
                new Tuple<int, int, DateOnly[]>(2010, 1, [new DateOnly(2010,1,15)]),
                new Tuple<int, int, DateOnly[]>(2010, 2, []),
                new Tuple<int, int, DateOnly[]>(2010, 3, []),
                new Tuple<int, int, DateOnly[]>(2010, 4, []),
                new Tuple<int, int, DateOnly[]>(2010, 5, []),
                new Tuple<int, int, DateOnly[]>(2010, 6, []),
                new Tuple<int, int, DateOnly[]>(2010, 7, [new DateOnly(2010,7,15)]),
                new Tuple<int, int, DateOnly[]>(2010, 8, []),
                new Tuple<int, int, DateOnly[]>(2010, 9, []),
                new Tuple<int, int, DateOnly[]>(2010, 10, []),
                new Tuple<int, int, DateOnly[]>(2010, 11, []),
                new Tuple<int, int, DateOnly[]>(2010, 12, []),
                new Tuple<int, int, DateOnly[]>(2011, 1, [new DateOnly(2011,1,15)]),
                new Tuple<int, int, DateOnly[]>(2011, 2, []),
                new Tuple<int, int, DateOnly[]>(2011, 3, []),
                new Tuple<int, int, DateOnly[]>(2011, 4, []),
                new Tuple<int, int, DateOnly[]>(2011, 5, []),
                new Tuple<int, int, DateOnly[]>(2011, 6, []),
                new Tuple<int, int, DateOnly[]>(2011, 7, [new DateOnly(2011,7,15)]),
                new Tuple<int, int, DateOnly[]>(2011, 8, []),
                new Tuple<int, int, DateOnly[]>(2011, 9, []),
                new Tuple<int, int, DateOnly[]>(2011, 10, []),
                new Tuple<int, int, DateOnly[]>(2011, 11, []),
                new Tuple<int, int, DateOnly[]>(2011, 12, [])
            }
        ],
        // Every 5 quarter, starting 15.1.2010
        [
            new DateOnly(2010,1,15), 3, 5, "Every 5 quarter",
            new[]
            {
                new Tuple<int, int, DateOnly[]>(2010, 1, [new DateOnly(2010,1,15)]),
                new Tuple<int, int, DateOnly[]>(2010, 2, []),
                new Tuple<int, int, DateOnly[]>(2010, 3, []),
                new Tuple<int, int, DateOnly[]>(2010, 4, []),
                new Tuple<int, int, DateOnly[]>(2010, 5, []),
                new Tuple<int, int, DateOnly[]>(2010, 6, []),
                new Tuple<int, int, DateOnly[]>(2010, 7, []),
                new Tuple<int, int, DateOnly[]>(2010, 8, []),
                new Tuple<int, int, DateOnly[]>(2010, 9, []),
                new Tuple<int, int, DateOnly[]>(2010, 10, []),
                new Tuple<int, int, DateOnly[]>(2010, 11, []),
                new Tuple<int, int, DateOnly[]>(2010, 12, []),
                new Tuple<int, int, DateOnly[]>(2011, 1, []),
                new Tuple<int, int, DateOnly[]>(2011, 2, []),
                new Tuple<int, int, DateOnly[]>(2011, 3, []),
                new Tuple<int, int, DateOnly[]>(2011, 4, [new DateOnly(2011,4,15)]),
                new Tuple<int, int, DateOnly[]>(2011, 5, []),
                new Tuple<int, int, DateOnly[]>(2011, 6, []),
                new Tuple<int, int, DateOnly[]>(2011, 7, []),
                new Tuple<int, int, DateOnly[]>(2011, 8, []),
                new Tuple<int, int, DateOnly[]>(2011, 9, []),
                new Tuple<int, int, DateOnly[]>(2011, 10, []),
                new Tuple<int, int, DateOnly[]>(2011, 11, []),
                new Tuple<int, int, DateOnly[]>(2011, 12, [])
            }
        ],
        // Every year, starting 15.1.2010
        [
            new DateOnly(2010,1,15), 4, 1, "Every year",
            new[]
            {
                new Tuple<int, int, DateOnly[]>(2010, 1, [new DateOnly(2010,1,15)]),
                new Tuple<int, int, DateOnly[]>(2010, 2, []),
                new Tuple<int, int, DateOnly[]>(2010, 3, []),
                new Tuple<int, int, DateOnly[]>(2010, 4, []),
                new Tuple<int, int, DateOnly[]>(2010, 5, []),
                new Tuple<int, int, DateOnly[]>(2010, 6, []),
                new Tuple<int, int, DateOnly[]>(2010, 7, []),
                new Tuple<int, int, DateOnly[]>(2010, 8, []),
                new Tuple<int, int, DateOnly[]>(2010, 9, []),
                new Tuple<int, int, DateOnly[]>(2010, 10, []),
                new Tuple<int, int, DateOnly[]>(2010, 11, []),
                new Tuple<int, int, DateOnly[]>(2010, 12, []),
                new Tuple<int, int, DateOnly[]>(2011, 1, [new DateOnly(2011,1,15)]),
                new Tuple<int, int, DateOnly[]>(2011, 2, []),
                new Tuple<int, int, DateOnly[]>(2011, 3, []),
                new Tuple<int, int, DateOnly[]>(2011, 4, []),
                new Tuple<int, int, DateOnly[]>(2011, 5, []),
                new Tuple<int, int, DateOnly[]>(2011, 6, []),
                new Tuple<int, int, DateOnly[]>(2011, 7, []),
                new Tuple<int, int, DateOnly[]>(2011, 8, []),
                new Tuple<int, int, DateOnly[]>(2011, 9, []),
                new Tuple<int, int, DateOnly[]>(2011, 10, []),
                new Tuple<int, int, DateOnly[]>(2011, 11, []),
                new Tuple<int, int, DateOnly[]>(2011, 12, [])
            }
        ],
        // Every 2 year, starting 15.1.2010
        [
            new DateOnly(2010,1,15), 4, 2, "Every 2 year",
            new[]
            {
                new Tuple<int, int, DateOnly[]>(2010, 1, [new DateOnly(2010,1,15)]),
                new Tuple<int, int, DateOnly[]>(2010, 2, []),
                new Tuple<int, int, DateOnly[]>(2010, 3, []),
                new Tuple<int, int, DateOnly[]>(2010, 4, []),
                new Tuple<int, int, DateOnly[]>(2010, 5, []),
                new Tuple<int, int, DateOnly[]>(2010, 6, []),
                new Tuple<int, int, DateOnly[]>(2010, 7, []),
                new Tuple<int, int, DateOnly[]>(2010, 8, []),
                new Tuple<int, int, DateOnly[]>(2010, 9, []),
                new Tuple<int, int, DateOnly[]>(2010, 10, []),
                new Tuple<int, int, DateOnly[]>(2010, 11, []),
                new Tuple<int, int, DateOnly[]>(2010, 12, []),
                new Tuple<int, int, DateOnly[]>(2011, 1, []),
                new Tuple<int, int, DateOnly[]>(2012, 1, [new DateOnly(2012,1,15)]),
                new Tuple<int, int, DateOnly[]>(2013, 1, []),
                new Tuple<int, int, DateOnly[]>(2014, 1, [new DateOnly(2014,1,15)])
            }
        ],
        // Every 5 year, starting 15.1.2010
        [
            new DateOnly(2010,1,15), 4, 5, "Every 5 year",
            new[]
            {
                new Tuple<int, int, DateOnly[]>(2010, 1, [new DateOnly(2010,1,15)]),
                new Tuple<int, int, DateOnly[]>(2010, 2, []),
                new Tuple<int, int, DateOnly[]>(2010, 3, []),
                new Tuple<int, int, DateOnly[]>(2010, 4, []),
                new Tuple<int, int, DateOnly[]>(2010, 5, []),
                new Tuple<int, int, DateOnly[]>(2010, 6, []),
                new Tuple<int, int, DateOnly[]>(2010, 7, []),
                new Tuple<int, int, DateOnly[]>(2010, 8, []),
                new Tuple<int, int, DateOnly[]>(2010, 9, []),
                new Tuple<int, int, DateOnly[]>(2010, 10, []),
                new Tuple<int, int, DateOnly[]>(2010, 11, []),
                new Tuple<int, int, DateOnly[]>(2010, 12, []),
                new Tuple<int, int, DateOnly[]>(2011, 1, []),
                new Tuple<int, int, DateOnly[]>(2012, 1, []),
                new Tuple<int, int, DateOnly[]>(2013, 1, []),
                new Tuple<int, int, DateOnly[]>(2014, 1, []),
                new Tuple<int, int, DateOnly[]>(2015, 1, [new DateOnly(2015,1,15)]),
                new Tuple<int, int, DateOnly[]>(2016, 1, []),
                new Tuple<int, int, DateOnly[]>(2017, 1, []),
                new Tuple<int, int, DateOnly[]>(2018, 1, []),
                new Tuple<int, int, DateOnly[]>(2019, 1, []),
                new Tuple<int, int, DateOnly[]>(2020, 1, [new DateOnly(2020,1,15)])
            }
        ]
    ];

    [Theory]
    [MemberData(nameof(TestData_AddRecurringTransactionsAsync_CheckRecurrance))]
    public async Task AddRecurringTransactionsAsync_CheckRecurrence(
        DateOnly firstOccurrence,
        int recurrenceType,
        int recurrenceAmount,
        string memo,
        Tuple<int, int, DateOnly[]>[] expectedCreationDatesPerMonth
        )
    {
        var serviceManager = new MockServiceManager(new MockDatabase());
        var testAccount = new Account() { IsActive = 1, Name = "Account" };

        serviceManager.AccountService.Create(testAccount);
        serviceManager.RecurringBankTransactionService.Create(new RecurringBankTransaction
        {
            FirstOccurrenceDate = firstOccurrence,
            AccountId = testAccount.Id,
            RecurrenceType = recurrenceType,
            RecurrenceAmount = recurrenceAmount,
            Memo = memo
        });

        foreach (var monthData in expectedCreationDatesPerMonth.Select(i => new 
                     { Year = i.Item1, Month = i.Item2, Dates = i.Item3 }))
        {
            var monthSelectorViewModel = new YearMonthSelectorViewModel(serviceManager)
            {
                SelectedYear = monthData.Year,
                SelectedMonth = monthData.Month
            };
            var viewModel = new TransactionPageViewModel(serviceManager, monthSelectorViewModel);
            await viewModel.LoadDataAsync();
            await viewModel.AddRecurringTransactionsAsync();
            await viewModel.LoadDataAsync();

            Assert.Equal(monthData.Dates.Count(), viewModel.Transactions.Count);
            foreach (var item in monthData.Dates)
            {
                Assert.Contains(item, viewModel.Transactions.Select(i => i.TransactionDate));
            }
        }
    }

    #region Tags

    private static readonly DateOnly TagTestDate = new(2026, 3, 15);
    private const string ItalyTag = "Italy";
    private const string WorkTag = "Work";
    private const string RefundTag = "Refund";
    private const string HotelMemo = "Hotel";
    private const string TrainMemo = "Train";

    private static Account CreateTestAccount(MockServiceManager serviceManager)
    {
        var account = new Account { IsActive = 1, Name = "Account" };
        serviceManager.AccountService.Create(account);
        return account;
    }

    private static TransactionViewModel CreateTaggedTransaction(
        MockServiceManager serviceManager, Account account, string memo, params string[] tagNames)
    {
        var transaction = TransactionViewModel.CreateEmpty(serviceManager);
        transaction.TransactionDate = TagTestDate;
        transaction.SelectedAccount = transaction.AvailableAccounts.First(item => item.AccountId == account.Id);
        transaction.Memo = memo;
        foreach (var tagName in tagNames)
        {
            transaction.AddTag(tagName);
        }
        return transaction;
    }

    private static async Task<TransactionPageViewModel> LoadTransactionPageAsync(MockServiceManager serviceManager)
    {
        var monthSelectorViewModel = new YearMonthSelectorViewModel(serviceManager)
        {
            SelectedYear = TagTestDate.Year,
            SelectedMonth = TagTestDate.Month
        };
        var viewModel = new TransactionPageViewModel(serviceManager, monthSelectorViewModel);
        await viewModel.LoadDataAsync();
        return viewModel;
    }

    [Fact]
    public async Task CreateItem_SavesTagsAndReusesExistingTagIgnoringCase()
    {
        var serviceManager = new MockServiceManager(new MockDatabase());
        var account = CreateTestAccount(serviceManager);
        serviceManager.TagService.Create(new Tag { Name = ItalyTag });
        var viewModel = await LoadTransactionPageAsync(serviceManager);
        var transaction = CreateTaggedTransaction(serviceManager, account, HotelMemo, ItalyTag.ToLower(), WorkTag);

        var result = viewModel.CreateItem(transaction);
        await viewModel.LoadDataAsync();

        Assert.True(result.IsSuccessful);
        Assert.Equal(new[] { ItalyTag, WorkTag }, viewModel.Transactions.Single().Tags);
        Assert.Equal(new[] { ItalyTag, WorkTag }, serviceManager.TagService.GetAll().Select(tag => tag.Name));
    }

    [Fact]
    public async Task SaveAllTransaction_ReplacesTagsOfModifiedTransactionOnly()
    {
        var serviceManager = new MockServiceManager(new MockDatabase());
        var account = CreateTestAccount(serviceManager);
        var viewModel = await LoadTransactionPageAsync(serviceManager);
        viewModel.CreateItem(CreateTaggedTransaction(serviceManager, account, HotelMemo, ItalyTag));
        viewModel.CreateItem(CreateTaggedTransaction(serviceManager, account, TrainMemo, ItalyTag, WorkTag));
        await viewModel.LoadDataAsync();
        var hotel = viewModel.Transactions.Single(transaction => transaction.Memo == HotelMemo);

        hotel.StartModification();
        hotel.RemoveTag(ItalyTag);
        hotel.AddTag(RefundTag);
        var result = viewModel.SaveAllTransaction();
        await viewModel.LoadDataAsync();

        Assert.True(result.IsSuccessful);
        Assert.Equal(new[] { RefundTag }, viewModel.Transactions.Single(transaction => transaction.Memo == HotelMemo).Tags);
        Assert.Equal(new[] { ItalyTag, WorkTag }, viewModel.Transactions.Single(transaction => transaction.Memo == TrainMemo).Tags);
    }

    [Fact]
    public async Task SaveAllTransaction_RequestsReloadToShowTheStoredTagNames()
    {
        var serviceManager = new MockServiceManager(new MockDatabase());
        var account = CreateTestAccount(serviceManager);
        var viewModel = await LoadTransactionPageAsync(serviceManager);
        viewModel.CreateItem(CreateTaggedTransaction(serviceManager, account, HotelMemo));
        viewModel.CreateItem(CreateTaggedTransaction(serviceManager, account, TrainMemo));
        await viewModel.LoadDataAsync();
        viewModel.EditAllTransaction();

        viewModel.Transactions.Single(transaction => transaction.Memo == HotelMemo).AddTag(ItalyTag);
        viewModel.Transactions.Single(transaction => transaction.Memo == TrainMemo).AddTag(ItalyTag.ToUpper());
        var result = viewModel.SaveAllTransaction();
        await viewModel.LoadDataAsync();

        var storedTagName = serviceManager.TagService.GetAll().Single().Name;
        Assert.True(result.IsSuccessful);
        Assert.True(result.ViewModelReloadRequired);
        Assert.All(viewModel.Transactions, transaction => Assert.Equal(new[] { storedTagName }, transaction.Tags));
    }

    [Fact]
    public async Task CreateItem_FailsWithoutCreatingAnythingIfATagNameIsTooLong()
    {
        var serviceManager = new MockServiceManager(new MockDatabase());
        var account = CreateTestAccount(serviceManager);
        var viewModel = await LoadTransactionPageAsync(serviceManager);
        var tooLongTagName = new string('x', Tag.NameMaxLength + 1);
        var transaction = CreateTaggedTransaction(serviceManager, account, HotelMemo, ItalyTag, tooLongTagName);

        var result = viewModel.CreateItem(transaction);

        Assert.False(result.IsSuccessful);
        Assert.Empty(serviceManager.BankTransactionService.GetAll());
        Assert.Empty(serviceManager.TagService.GetAll());
    }

    [Fact]
    public void AddTag_IgnoresEmptyAndAlreadyAssignedNames()
    {
        var serviceManager = new MockServiceManager(new MockDatabase());
        var transaction = TransactionViewModel.CreateEmpty(serviceManager);

        transaction.AddTag($"  {ItalyTag} ");
        transaction.AddTag(ItalyTag.ToUpper());
        transaction.AddTag("   ");
        transaction.AddTag(null);

        Assert.Equal(new[] { ItalyTag }, transaction.Tags);
    }

    [Fact]
    public void AddTag_IgnoresAlreadyAssignedNameWithATurkishCulture()
    {
        var serviceManager = new MockServiceManager(new MockDatabase());
        var transaction = TransactionViewModel.CreateEmpty(serviceManager);
        var currentCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("tr-TR");

        try
        {
            transaction.AddTag(ItalyTag);
            transaction.AddTag(ItalyTag.ToLowerInvariant());
        }
        finally
        {
            CultureInfo.CurrentCulture = currentCulture;
        }

        Assert.Equal(new[] { ItalyTag }, transaction.Tags);
    }

    [Fact]
    public void CancelModification_KeepsTheTransactionInASelection()
    {
        var serviceManager = new MockServiceManager(new MockDatabase());
        var transaction = TransactionViewModel.CreateEmpty(serviceManager);
        transaction.AddTag(ItalyTag);
        var selectedTransactions = new HashSet<TransactionViewModel> { transaction };

        transaction.StartModification();
        transaction.AddTag(WorkTag);
        transaction.CancelModification();

        Assert.True(selectedTransactions.Remove(transaction));
    }

    [Fact]
    public void CancelModification_RestoresTags()
    {
        var serviceManager = new MockServiceManager(new MockDatabase());
        var transaction = TransactionViewModel.CreateEmpty(serviceManager);
        transaction.AddTag(ItalyTag);

        transaction.StartModification();
        transaction.RemoveTag(ItalyTag);
        transaction.AddTag(WorkTag);
        transaction.CancelModification();

        Assert.Equal(new[] { ItalyTag }, transaction.Tags);
    }

    public static IEnumerable<object[]> TestData_HasAnyTag =>
    [
        [new[] { ItalyTag, WorkTag }, new[] { WorkTag }, true],
        [new[] { ItalyTag }, new[] { WorkTag, RefundTag }, false],
        [new[] { ItalyTag }, new[] { TransactionViewModel.NoTag }, false],
        [Array.Empty<string>(), new[] { TransactionViewModel.NoTag }, true],
        [Array.Empty<string>(), new[] { ItalyTag }, false]
    ];

    [Theory]
    [MemberData(nameof(TestData_HasAnyTag))]
    public void HasAnyTag_MatchesTransactionsWithOneOfTheSelectedTags(
        string[] transactionTags,
        string[] selectedTags,
        bool expectedResult)
    {
        var serviceManager = new MockServiceManager(new MockDatabase());
        var transaction = TransactionViewModel.CreateEmpty(serviceManager);
        foreach (var tagName in transactionTags)
        {
            transaction.AddTag(tagName);
        }

        Assert.Equal(expectedResult, transaction.HasAnyTag(selectedTags));
    }

    [Fact]
    public async Task LoadAvailableTags_ListsAllTagNames()
    {
        var serviceManager = new MockServiceManager(new MockDatabase());
        serviceManager.TagService.Create(new Tag { Name = WorkTag });
        serviceManager.TagService.Create(new Tag { Name = ItalyTag });
        var viewModel = await LoadTransactionPageAsync(serviceManager);

        var result = viewModel.LoadAvailableTags();

        Assert.True(result.IsSuccessful);
        Assert.Equal(new[] { ItalyTag, WorkTag }, viewModel.AvailableTags);
    }

    #endregion
}