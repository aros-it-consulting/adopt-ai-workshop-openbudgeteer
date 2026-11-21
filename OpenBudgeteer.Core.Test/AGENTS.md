# Tests of the core

xUnit. Run them with `dotnet test OpenBudgeteer.Core.Test`, from the repository root.

The tests run on MariaDB, as in CI: start a MariaDB on port 3306 and export `CONNECTION_PROVIDER=mariadb`,
`CONNECTION_SERVER`, `CONNECTION_PORT`, `CONNECTION_USER`, `CONNECTION_PASSWORD` and `CONNECTION_DATABASE`
before `dotnet test`. Never run them on another database.

- A view model test builds its services on `new MockServiceManager(new MockDatabase())`, creates its data
  through these services, then asserts on the view model.
- A test of `Tests/Database` runs each repository twice, through its `TestData_Repository`: once on the mock
  repository, once on the real repository over `GetInMemoryContext()`.
- A test is named `<Method>_Check<WhatIsChecked>`: `LoadData_CheckNameAndLoadOnlyActiveAccounts`.
- The cases of a `[Theory]` live in a static `TestData_<TestName>` property, passed with
  `[MemberData(nameof(TestData_<TestName>))]`.
- A new entity needs, in `Mocking/`: its dictionary in `MockDatabase` and in `MockDatabase.Cleanup()`, a
  `Mock<Entity>Repository`, a `Mock<Entity>Service` when it has a service, and that service in
  `MockServiceManager`. Then a `<Entity>DatabaseTest` that inherits `BaseDatabaseTest<Entity>`.
- Random entities come from `TestDataGenerator.Current`.
- `DistributeBudget_CheckDistributedMoney` is skipped on purpose: a run reports one skipped test.
- The import tests read their files from `Resources/`. The expected values of `TestImportFile5.txt` are in
  `Tests/PageViewModels/ImportPageViewModelTest.cs`.
- Do not write a test like this one, which asserts nothing:

  ```c#
  [Fact]
  public void LoadData_Works()
  {
      var viewModel = new AccountPageViewModel(new MockServiceManager(new MockDatabase()));
      viewModel.LoadData();
  }
  ```
