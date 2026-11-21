# Learnings

The way the team works in this repository. One entry per learning: the date it was written, where it was met,
the learning and its reason.

- 2025-12-04 · `OpenBudgeteer.Core.Test` · Run `dotnet test OpenBudgeteer.Core.Test` before pushing: CI takes
  four minutes to say the same thing.
- 2025-12-11 · `Makefile` · `make test` is faster than `dotnet test`: it skips the restore.
- 2025-12-18 · `OpenBudgeteer.Core.Test/Tests/PageViewModels/AccountPageViewModelTest.cs` · The tests are named
  after the method and what they check: `LoadData_CheckNameAndLoadOnlyActiveAccounts`. The reviewer asked for
  it twice.
- Be careful with the import.
- 2026-01-08 · `payments-gateway/config/sandbox.yaml` · The sandbox key of the payment provider expires every
  ninety days. When the integration tests of the gateway fail with a 401, ask the platform team for a new key.
- 2026-01-22 · `OpenBudgeteer.Blazor/Program.cs` · The `.env` is looked up from the working directory upwards:
  a `.env` left in a parent folder is picked up without a word. Run the front end from the repository.
- 2026-01-29 · Run the tests of the core before any push (`dotnet test OpenBudgeteer.Core.Test`). I forgot
  again and CI went red.
- 2026-02-10 · `OpenBudgeteer.Blazor/Common/Services/DatabaseMigratorService.cs` · The front end does not
  start without its database: the migrations run at startup and stop it with "Target database is not online".
  Check the database before reading the stack trace.
- 2026-02-18 · `OpenBudgeteer.Core.Data.MySql.Migrations/20250301100128_DateTimeToDateOnly.cs` · When a
  migration is wrong, edit it by hand, in place, and fix the snapshot with it: it is much faster than adding a
  migration on top, and nobody has to squash anything later.
- 2026-03-05 · `.github/workflows/test.yml` · CI starts a MariaDB and passes its `CONNECTION_*` variables to
  the core tests, which never read them: a red CI run is never a database problem.
- 2026-03-12 · A test name says the method, then `_Check`, then what is verified. Same as the existing tests.
- 2026-03-26 · `scripts/new-migration.sh` · Generate the migrations with `scripts/new-migration.sh <Name>`: it
  runs `dotnet ef migrations add` for the three providers in one go.
- 2026-04-02 · Every repository of the company pins the version of each NuGet package: no floating version,
  no `*`. The security team refuses a pull request that floats one.
- 2026-04-16 · `OpenBudgeteer.Blazor/Pages/Report.razor.cs` · When the view model has no method for a read,
  call the repository from the page: it is quicker than adding a method to the service, its interface and its
  wrapper for one query.
- 2026-04-23 · `OpenBudgeteer.Core.Test/Mocking/Repository/MockAccountRepository.cs` · A mock repository sets
  a new `Guid` in `Create`, as the database does: the database tests assert that the id is no longer empty.
- 2026-05-07 · I read a diff more easily in a split terminal, the tests on the right. Open the test file
  first.
- 2026-05-21 · `OpenBudgeteer.Core.Test/Tests/PageViewModels/BucketPageViewModelTest.cs` · Third time: the core
  tests are run before the push, not after. `dotnet test OpenBudgeteer.Core.Test` takes ten seconds.
- 2026-05-28 · The code is indented with four spaces, the namespaces are file-scoped and a line stops at 120
  characters. The reviewer does not want to see a formatting comment again.
- 2026-06-04 · The pull requests go to `pre-release`, never to `main`.
- 2026-06-11 · `OpenBudgeteer.Core.Test/Tests/PageViewModels/TransactionPageViewModelTest.cs` · New tests follow
  `<Method>_Check<WhatIsChecked>`. A test named `Test1` was sent back in review.
- 2026-07-01 · `OpenBudgeteer.Core.Data/README.md` · A migration is generated once per provider, with the two
  commands of that README. Reviewers refuse a pull request that carries one provider only: the other database
  would start on a schema the model no longer matches.
- 2026-07-16 · To release: move the changelog entries under the version, update `SECURITY.md`, merge
  `pre-release` into `main`, tag, wait for `.github/workflows/docker-image-release.yml`, publish the release.
- 2026-08-06 · `OpenBudgeteer.Blazor/Pages/Transaction.razor.cs` · Every page handles the result of a view
  model in its own `HandleResult`. A page that shows the error itself, without it, was sent back in review.
- 2026-08-13 · `OpenBudgeteer.Blazor/Shared/Dialog/EditAccountDialog.razor` · The pages and the dialogs use
  the MudBlazor components only, and a dialog is opened with `IDialogService`.
- Ask before touching the report.
- 2026-08-27 · Do not forget the tests.
- 2026-09-17 · `OpenBudgeteer.Core.Data.Services/EFCore/EFCoreBaseService.cs` · A new service method is written
  three times: in the interface, as the rule in the `Generic` service, as a wrapper in the `EFCore` one. The
  view models only ever see the interface.
- 2026-10-01 · `OpenBudgeteer.Core.Test` · Before pushing, run the core tests. Written here because the three
  entries above did not stop it from happening.
- 2026-10-08 · `OpenBudgeteer.Core.Data.Postgres.Migrations/20250331173217_BucketIsHiddenFromSummaries.cs` ·
  The Postgres migration was forgotten once more. Both providers, every time: see the entry of July.
- 2026-10-15 · `OpenBudgeteer.Core.Data.Services/Generic/GenericBucketService.cs` · The system buckets are
  Income and Transfer, known by their ids ending in 1 and 2.
- 2026-11-05 · The old wiki page on the import profiles is wrong, do not read it.
