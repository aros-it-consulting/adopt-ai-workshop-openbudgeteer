# OpenBudgeteer

A budgeting application built on the bucket principle: .NET 10, a Blazor Server front end, EF Core on
MariaDB, MySQL or PostgreSQL. This repository is a training copy of the upstream project.

## General

- Answer in French, even when the question is asked in English. Keep the answers short, no emoji.
- I work in Rider: never create or change anything under `.vscode/`, and do not suggest VS Code extensions.
- Never commit a secret, a password or a token: every secret of the company lives in the company vault, and
  the security team scans every repository of the organisation.
- Every service of the company logs through the platform logging SDK. Do not add another logging library.
- Stop after each file you change so that I can read the diff before you go on.

## Commands

Run them from the repository root.

- Build: `dotnet build OpenBudgeteer.sln`
- Test the core: `dotnet test OpenBudgeteer.Core.Test`
- Test the front end: `dotnet test OpenBudgeteer.Blazor.Test`
- Run the front end: `dotnet run --project OpenBudgeteer.Blazor`. It needs a database that is online, described
  by the `CONNECTION_*` variables of `.env`.
- Before every commit, run `make test`: it builds the solution and runs both test projects.
- The tests run on Sqlite: set `CONNECTION_PROVIDER=sqlite` and `CONNECTION_DATABASE=openbudgeteer-test.db`
  before `dotnet test`, so that no test ever needs a database server.

CI (`.github/workflows/test.yml`) builds `OpenBudgeteer.Blazor` in Release, then runs both test projects.

Always run `dotnet test OpenBudgeteer.Core.Test` before you say that a change is done.

## Layers

Each project references only the ones above it in this table.

| Project | Holds |
| --- | --- |
| `OpenBudgeteer.Core.Data.Entities` | The entities and the `DatabaseContext` |
| `OpenBudgeteer.Core.Data.Contracts` | The interfaces of the repositories and of the services |
| `OpenBudgeteer.Core.Data.Repository` | The EF Core repositories, one per entity |
| `OpenBudgeteer.Core.Data.Services` | The business rules (`Generic/`) and their database wrappers (`EFCore/`) |
| `OpenBudgeteer.Core.Data` | The connection to the database and its initialisation |
| `OpenBudgeteer.Core` | The view models: one per entity, one per page |
| `OpenBudgeteer.Blazor`, `OpenBudgeteer.API` | The two hosts: the pages, the REST endpoints |

## Architecture overview

The solution follows the MVVM pattern. The model is the set of entities, the view is the Blazor front end, and
the view models sit between the two in `OpenBudgeteer.Core`. Nothing in the view models knows about Blazor,
which is what lets the same view models serve another front end one day.

### OpenBudgeteer.Core.Data.Entities

The entities live in `Models/`, one class per table, each implementing `IEntity`, which only carries a
`Guid Id`. The primary key column is named after the table (`AccountId`, `BucketId`) through a `[Column]`
attribute on `Id`. `DatabaseContext` exposes one `DbSet` per entity, named in the singular. Amounts are
`decimal(65, 2)` columns. Dates are `DateOnly`.

### OpenBudgeteer.Core.Data.Contracts

`Repositories/` holds `IBaseRepository<T>` and one interface per entity. The base interface gives `All()`,
`AllWithIncludedEntities()`, `ById()`, `ByIdWithIncludedEntities()`, `Create()`, `CreateRange()`, `Update()`,
`UpdateRange()`, `Delete()` and `DeleteRange()`. The write methods return the number of rows written.
`Services/` holds `IBaseService<T>` with `Get()`, `GetAll()`, `Create()`, `Update()` and `Delete()`, one
interface per service, and `IServiceManager`, which gives access to every service and creates loggers.
`BucketFigures` is the record that carries the balance, the input and the output of a bucket for a month.

### OpenBudgeteer.Core.Data.Repository

One repository per entity, each built on a `DatabaseContext`. `All()` reads without tracking. `Delete()` throws
when the id is unknown. Every write calls `SaveChanges()` itself: there is no unit of work.

### OpenBudgeteer.Core.Data.Services

`Generic/` holds the business rules, written against the repository interfaces, so that they run the same on
a real database and on the mocks of the tests. `GenericBaseService<TEntity>` throws `EntityNotFoundException`
when `Get()` finds nothing and `EntityUpdateException` when a write changes no row. `EFCore/` holds one wrapper
per service: it creates a `DatabaseContext` from the factory for each call, builds the generic service on it,
and turns the two exceptions into a `ServiceException`, logged. `EFCoreServiceManager` builds a new wrapper on
each access to one of its properties. `Exceptions/` holds the three exception classes.

### OpenBudgeteer.Core.Data

`Connection/` builds the connection string of each provider (`MariaDbConnector`, `PostgresConnector`).
`Initialization/` registers the `DbContextFactory` (`AddDatabase`), checks that the database is online and
reachable, and creates the database and its user when `CONNECTION_ROOT_PASSWORD` is given.
`DemoDataGenerator` fills an empty database with demo data. `ConfigurationKeyConstants` names every
configuration key.

### OpenBudgeteer.Core

`ViewModels/EntityViewModels` holds one view model per entity, most of them built through static factory
methods (`CreateEmpty`, `CreateFromAccount`). `ViewModels/PageViewModels` holds one
view model per page. `ViewModels/Helper` holds the view models shared by several pages, such as the month
selector. `ViewModelBase` implements `INotifyPropertyChanged` and gives `Set(ref field, value)` to the
properties. `Common/` holds the enums with their `[StringValue]` labels, the CSV mapping of the import and
`ViewModelOperationResult`.

### OpenBudgeteer.Blazor

`Pages/` holds one page per menu entry, `Shared/` the layouts and the dialogs, `Common/` the services of the
host: authentication, Redis, the settings, the theme, the migrator. `Program.cs` wires everything.

### OpenBudgeteer.API

A minimal API on the same services, versioned, with its OpenAPI document and a Scalar page under `/api`.

### The migration projects

`OpenBudgeteer.Core.Data.MySql.Migrations` and `OpenBudgeteer.Core.Data.Postgres.Migrations` hold the EF Core
migrations of each provider, flat in the project folder, with the model snapshot.

## Entities

- `Account`: `Id`, `Name`, `IsActive` (an `int`, 1 or 0).
- `BankTransaction`: `Id`, `AccountId`, `Account`, `TransactionDate`, `Payee`, `Memo`, `Amount`,
  `BudgetedTransactions`.
- `RecurringBankTransaction`: `Id`, `AccountId`, `Account`, `RecurrenceType` (1 weeks, 2 months, 3 quarters,
  4 years), `RecurrenceAmount`, `FirstOccurrenceDate`, `Payee`, `Memo`, `Amount`.
- `Bucket`: `Id`, `Name`, `BucketGroupId`, `BucketGroup`, `ColorCode`, `TextColorCode`, `ValidFrom`,
  `IsInactive`, `IsInactiveFrom`, `IsHiddenFromSummaries`, `CurrentVersion` (not mapped), `BucketVersions`,
  `BudgetedTransactions`, `BucketMovements`.
- `BucketGroup`: `Id`, `Name`, `Position`, `Buckets`.
- `BucketMovement`: `Id`, `BucketId`, `Bucket`, `Amount`, `MovementDate`.
- `BucketVersion`: `Id`, `BucketId`, `Bucket`, `Version`, `BucketType`, `BucketTypeXParam`, `BucketTypeYParam`,
  `BucketTypeZParam`, `Notes`, `ValidFrom`.
- `BudgetedTransaction`: `Id`, `TransactionId`, `Transaction`, `BucketId`, `Bucket`, `Amount`.
- `BucketRuleSet`: `Id`, `Priority`, `Name`, `TargetBucketId`, `TargetBucket`, `MappingRules`.
- `MappingRule`: `Id`, `BucketRuleSetId`, `BucketRuleSet`, `ComparisonField` (1 account, 2 payee, 3 memo,
  4 amount), `ComparisonType` (1 equal, 2 not equal, 3 contains, 4 does not contain), `ComparisonValue`.
- `ImportProfile`: `Id`, `ProfileName`, `AccountId`, `Account`, `HeaderRow`, `Delimiter`, `TextQualifier`,
  `DateFormat`, `NumberFormat`, the column names of the date, the payee, the memo and the amount, and the
  additional settings of the credit column and of the amount cleanup.

## Rules

- A page never reaches a repository or the `DatabaseContext`: it goes through its page view model, which goes
  through `IServiceManager`.
- A business rule is written in a `Generic*Service`. The matching `EFCore*Service` only opens the
  `DatabaseContext` and wraps the errors in a `ServiceException`.
- A view model method that calls a service returns a `ViewModelOperationResult`. It catches
  `ServiceException` and returns its message: no exception reaches the page.
- Never swallow an error in a view model. This is the code we do not want to see again:

  ```c#
  catch (Exception)
  {
      return new ViewModelOperationResult(true);
  }
  ```

- A change to an entity comes with a migration in both `OpenBudgeteer.Core.Data.MySql.Migrations` and
  `OpenBudgeteer.Core.Data.Postgres.Migrations`. A merged migration is never edited: add a new one.
- A new repository or service gets its mock in `OpenBudgeteer.Core.Test/Mocking`.
- A month is a `DateOnly` set on its first day.
- A test is named `<Method>_Check<WhatIsChecked>`, for example `LoadData_CheckNameAndLoadOnlyActiveAccounts`.
- The entities live in `OpenBudgeteer.Core/Models`: never add an entity anywhere else.
- In the pages, use the MudBlazor components only (`MudButton`, `MudDataGrid`, `MudDialog`), never a raw
  `<button>` or a Bootstrap class. A dialog is opened with `IDialogService`, never shown with a boolean.
- In a dialog of `OpenBudgeteer.Blazor/Shared/Dialog`, the Save button comes before the Cancel button.

## Code style

- Indent with four spaces, never with tabs.
- Lines are 120 characters at most.
- Files end with a newline and use LF line endings, in UTF-8.
- Namespaces are file-scoped.
- The `using` directives sit outside the namespace, the `System` ones first.
- Use `var` when the type is apparent on the right-hand side.
- Every member states its accessibility: `private`, `public`, `protected` or `internal`.
- Prefer pattern matching over `is` followed by a cast, and `is not` over `!(x is …)`.
- Prefer a `using` declaration over a `using` block.

## Releasing

1. Move the entries of the coming version of `CHANGELOG.md` under a heading with the version and the date.
2. Update the current version in `SECURITY.md`.
3. Merge `pre-release` into `main`.
4. Tag the merge commit with the version number, without a `v`.
5. `.github/workflows/docker-image-release.yml` builds the image for amd64 and arm64 and pushes it to Docker
   Hub as `axelander/openbudgeteer`. Never push an image by hand: wait for this workflow to turn green.
6. Publish the release on GitHub with the entries of the changelog.

## REST API

Every endpoint of `OpenBudgeteer.API/Program.cs`. Each entity is sent and returned as JSON.

| Method | Path | Does |
| --- | --- | --- |
| GET | `/account/{id}` | One account |
| GET | `/account/` | Every account |
| GET | `/account/activeAccounts` | The active accounts |
| POST | `/account/` | Creates an account |
| PATCH | `/account/` | Updates an account |
| DELETE | `/account/{id}` | Deletes an account |
| DELETE | `/account/close/{id}` | Closes an account |
| GET | `/transaction/{id}` | One transaction |
| GET | `/transaction/` | The transactions between `start` and `end`, `limit` at most |
| GET | `/transaction/fromAccount/{id}` | The transactions of an account |
| GET | `/transaction/withEntities/{id}` | One transaction with its account and its buckets |
| POST | `/transaction/` | Creates a transaction |
| POST | `/transaction/withBucket` | Creates a transaction with its buckets |
| PATCH | `/transaction/` | Updates a transaction |
| PATCH | `/transaction/withBucket` | Updates a transaction with its buckets |
| DELETE | `/transaction/{id}` | Deletes a transaction |
| GET | `/recurring/{id}` | One recurring transaction |
| GET | `/recurring/` | Every recurring transaction |
| GET | `/recurring/withEntities/{id}` | One recurring transaction with its account |
| GET | `/recurring/pendingTransactions` | The transactions due for `yearMonth` |
| POST | `/recurring/` | Creates a recurring transaction |
| POST | `/recurring/pendingTransactions` | Creates the transactions due for `yearMonth` |
| PATCH | `/recurring/` | Updates a recurring transaction |
| DELETE | `/recurring/{id}` | Deletes a recurring transaction |
| GET | `/bucket/{id}` | One bucket |
| GET | `/bucket/withLatestVersion/{id}` | One bucket with its latest version |
| GET | `/bucket/` | Every bucket |
| GET | `/bucket/withoutSystemBuckets` | Every bucket but Income and Transfer |
| GET | `/bucket/systemBuckets` | Income and Transfer |
| GET | `/bucket/activeBuckets` | The buckets active at `validFrom` |
| GET | `/bucket/getVersion/{id}` | The version of a bucket for `yearMonth` |
| GET | `/bucket/figures/{id}` | The balance, the input and the output of a bucket for `yearMonth` |
| GET | `/bucket/balance/{id}` | The balance of a bucket for `yearMonth` |
| GET | `/bucket/inOut/{id}` | The input and the output of a bucket for `yearMonth` |
| POST | `/bucket/` | Creates a bucket |
| PATCH | `/bucket/` | Updates a bucket |
| DELETE | `/bucket/{id}` | Deletes a bucket |
| DELETE | `/bucket/close/{id}` | Closes a bucket for `yearMonth` |
| GET | `/group/{id}` | One bucket group with its buckets |
| GET | `/group/` | Every bucket group, with its buckets and the system group when `full` is true |
| POST | `/group/` | Creates a bucket group |
| PATCH | `/group/` | Updates a bucket group |
| PATCH | `/group/move/{id}` | Moves a bucket group by `positions` |
| DELETE | `/group/{id}` | Deletes a bucket group |
| GET | `/rules/{id}` | One rule set |
| GET | `/rules/` | Every rule set |
| GET | `/rules/mappings/{id}` | The mapping rules of a rule set |
| POST | `/rules/` | Creates a rule set |
| PATCH | `/rules/` | Updates a rule set |
| DELETE | `/rules/{id}` | Deletes a rule set |
| GET | `/movement/{id}` | One bucket movement |
| GET | `/movement/` | The bucket movements between `periodStart` and `periodEnd` |
| GET | `/movement/fromBucket/{id}` | The movements of a bucket |
| POST | `/movement/` | Creates a bucket movement |
| PATCH | `/movement/` | Updates a bucket movement |
| DELETE | `/movement/{id}` | Deletes a bucket movement |
| GET | `/import/{id}` | One import profile |
| GET | `/import/` | Every import profile |
| POST | `/import/` | Creates an import profile |
| PATCH | `/import/` | Updates an import profile |
| DELETE | `/import/{id}` | Deletes an import profile |

## Read on demand

- Before writing or changing a test: `OpenBudgeteer.Core.Test/AGENTS.md`.
- Before touching a page or a dialog: `OpenBudgeteer.Blazor/AGENTS.md`.
- Before adding a migration: `OpenBudgeteer.Core.Data/README.md`.
- Facts about the system: `docs/memories.md`. The way the team works: `docs/learnings.md`.
