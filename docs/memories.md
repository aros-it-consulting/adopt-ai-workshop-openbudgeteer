# Memories

Durable facts about the system. One entry per fact: the date it was written, the file that proves it, the fact.

- 2025-12-03 · `OpenBudgeteer.Core/Models/Bucket.cs` · A bucket belongs to exactly one bucket group, and a
  bucket group cannot be deleted while it still holds a bucket.
- 2025-12-03 · The application has three kinds of users: the administrator, the editor and the reader.
- 2025-12-10 · `OpenBudgeteer.Core.Test/Tests/Database/BaseDatabaseTest.cs` · The database tests connect to the
  MariaDB of the office, `192.168.178.153`, with the user `openbudgeteer_unit_test`.
- 2025-12-17 · `.github/workflows/docker-image-pre-release.yml` · Every push to `pre-release` builds the Docker
  image and publishes it under the `pre-release` tag.
- Careful with the rounding.
- 2026-01-14 · `OpenBudgeteer.Core.Data.Services/Generic/GenericBucketService.cs` · The two system buckets are
  Income (`00000000-0000-0000-0000-000000000001`) and Transfer (`00000000-0000-0000-0000-000000000002`). The
  services tell them apart by id, never by name.
- 2026-01-14 · `OpenBudgeteer.Core.Data.Services/Generic/GenericBucketService.cs` · The balance of a bucket for
  a month is the sum of its budgeted transactions and of its bucket movements up to the end of that month.
- 2026-01-20 · Session on the Account page. Looked for the reason why the balance of a closed account was still
  shown. Read `AccountPageViewModel`, then `GenericAccountService`, then the repository. The page only loads the
  active accounts, so the bug was somewhere else: the browser had kept the old page. Nothing to fix in the end.
  Took most of the afternoon.
- 2026-01-28 · `billing-api/src/invoices/vat.ts` · The invoicing service rounds the VAT line by line, never on
  the total of the invoice, and the accountants reconcile on the rounded lines.
- 2026-02-03 · `OpenBudgeteer.Core.Data.Services/Generic/GenericBucketService.cs` · A bucket is closed only at a
  balance of 0. With transactions or movements it becomes inactive from the next month, without any it is
  deleted.
- 2026-02-03 · `OpenBudgeteer.Core.Data.Services/Generic/GenericBucketService.cs` · Changing a bucket in the
  month of its latest version overwrites that version, changing it in another month adds a version.
- 2026-02-12 · `OpenBudgeteer.Core.Data.Services/Generic/GenericBudgetedTransactionService.cs` · Income is the
  bucket `00000000-0000-0000-0000-000000000001` and Transfer the bucket `00000000-0000-0000-0000-000000000002`.
  They are the system buckets, and the code recognises them by their id.
- The report page is slow sometimes.
- 2026-02-26 · `OpenBudgeteer.Core.Data.Services/Generic/GenericBucketService.cs` · A bucket can be closed
  whatever its balance: what is left goes back to the budget of the month.
- 2026-03-04 · Always run the tests of the core before pushing, and name a new test
  `<Method>_Check<WhatIsChecked>`.
- 2026-03-11 · `OpenBudgeteer.Core.Data.Entities/Models/Bucket.cs` · A bucket holds its `Name`, its
  `BucketGroupId`, its `ColorCode`, its `TextColorCode`, its `ValidFrom`, its `IsInactive` flag with
  `IsInactiveFrom`, and `IsHiddenFromSummaries`. A bucket version holds its `Version`, its `BucketType`, the
  three parameters `BucketTypeXParam`, `BucketTypeYParam` and `BucketTypeZParam`, its `Notes` and its
  `ValidFrom`.
- 2026-03-19 · `OpenBudgeteer.Core/ViewModels/EntityViewModels/BucketViewModel.cs` · The Want of a bucket depends
  on its type: none for a standard bucket, the monthly amount minus what came in for a monthly expense, the
  target spread over the remaining months for the two types with a target date.
- 2026-03-25 · `OpenBudgeteer.Core.Data.Entities/Models/BankTransaction.cs` · A bank transaction holds its
  `AccountId`, its `TransactionDate`, its `Payee`, its `Memo`, its `Amount` and its `BudgetedTransactions`. A
  budgeted transaction holds its `TransactionId`, its `BucketId` and its `Amount`.
- 2026-04-08 · `OpenBudgeteer.Core/ViewModels/PageViewModels/BucketPageViewModel.cs` · Distribute Budget gives
  each bucket its Want, then brings every negative balance back to 0. It does not look at the budget left.
- 2026-04-15 · `mobile/app/src/cache/BucketCache.kt` · The mobile application keeps the list of buckets for
  fifteen minutes: a bucket renamed on the web shows its old name there until the cache expires.
- Somebody said the import breaks on some banks.
- 2026-04-29 · Session on the Import page. Imported the four files of `OpenBudgeteer.Core.Test/Resources` by
  hand to see what the page does with them. The second one has a semicolon as delimiter, the third one has
  the credit and the debit in two columns. Wanted to add a fifth file with a header on the third row, did not
  have the time. The page keeps the last profile selected, which is handy.
- The numbers of the home page are wrong when the month changes.
- 2026-05-06 · `OpenBudgeteer.Core.Data.Sqlite.Migrations/OpenBudgeteer.Core.Data.Sqlite.Migrations.csproj` ·
  Sqlite is the default provider: without `CONNECTION_PROVIDER`, the application creates `openbudgeteer.db` in
  `/app/database`.
- 2026-05-27 · `OpenBudgeteer.Core/ViewModels/EntityViewModels/TransactionViewModel.cs` · A transaction is saved
  only when the amounts of its buckets add up to its own amount.
- 2026-05-27 · `OpenBudgeteer.Core/ViewModels/EntityViewModels/TransactionViewModel.cs` · Rule sets are tried by
  ascending priority. The first one whose mapping rules all apply gives its bucket, compared in lower case.
- 2026-06-02 · `OpenBudgeteer.Core.Data.Services/Generic/GenericBucketService.cs` · Closing a bucket needs a
  balance of 0. A bucket that has transactions or movements is set inactive from the following month, a bucket
  that has none is deleted.
- 2026-06-16 · `OpenBudgeteer.Core/Common/Extensions/DuplicateMatchComparer.cs` · An imported transaction is a
  potential duplicate when the date and the amount match, and the payee or the memo.
- 2026-06-30 · Session on the Rules page. The rule sets are listed by priority. Tried to sort them by name,
  gave up: the priority is what the users want. Then renamed two variables in `RulesPageViewModel` and reverted
  it, the diff was noise.
- 2026-07-02 · `OpenBudgeteer.Core/ViewModels/ItemViewModels/BucketViewModelItem.cs` · The view model of a
  bucket computes its Want when it is built, for the month it is given.
- 2026-07-08 · `OpenBudgeteer.Blazor/Pages/Report.razor` · The charts of the Report page are drawn with
  Chart.js, and their colours come from `wwwroot/css/chart-theme.css`.
- 2026-07-22 · `OpenBudgeteer.Core/ViewModels/EntityViewModels/TransactionViewModel.cs` · Saving a transaction
  is refused while the sum of its bucket assignments differs from its amount.
- 2026-08-19 · To add an entity: create the class in `Models/`, add its `DbSet`, write the repository and its
  interface, write the generic service and its EF Core wrapper, add the service to both service managers,
  generate the two migrations, write the mocks, write the database test.
- 2026-08-26 · `OpenBudgeteer.Core/Common/Extensions/DuplicateMatchComparer.cs` · The import flags a
  transaction as a possible duplicate when another one has the same date and the same amount, and either the
  same payee or the same memo.
- 2026-09-02 · `OpenBudgeteer.Blazor/Common/Services/DatabaseMigratorService.cs` · The migrations run when the
  front end starts. `APPSETTINGS_DEMO_DATA=true` then fills the database with demo data.
- 2026-09-09 · The export job still runs on the old server.
- 2026-10-07 · `crm-sync/README.md` · The synchronisation with the CRM runs every night at two o'clock and
  locks the customer table for about ten minutes.
- 2026-10-21 · `OpenBudgeteer.Core.Data/Initialization/DatabaseInitializationExtensions.cs` · The providers are
  `MARIADB`, `MYSQL`, `POSTGRES` and `POSTGRESQL`. Sqlite was dropped in 1.8.3:
  `OpenBudgeteer.Core.Data.Sqlite.Migrations` is a leftover outside the solution.
- 2026-11-04 · `OpenBudgeteer.API/Program.cs` · The API exposes `GET /bucket/figures/{id}`,
  `GET /bucket/balance/{id}` and `GET /bucket/inOut/{id}`, each with an optional `yearMonth`.
