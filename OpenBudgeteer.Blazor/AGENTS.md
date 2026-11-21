# Blazor front end

- A page is `Pages/<Name>.razor` and its code-behind `Pages/<Name>.razor.cs`, a partial class.
- The code-behind injects `IServiceManager`, builds the page view model in `OnInitializedAsync` and keeps it
  in `_dataContext`.
- Every `ViewModelOperationResult` goes through the page's `HandleResult`: it opens `ErrorMessageDialog` on a
  failure and reloads the view model when `ViewModelReloadRequired` is set.
- Components are MudBlazor's (`Mud*`). A dialog lives in `Shared/Dialog` and is opened with `IDialogService`.
- Every page carries `@attribute [Authorize]`, except `Login` and `Error`.
- A page view model lives in `OpenBudgeteer.Core/ViewModels/PageViewModels`. `ViewModels/` here holds only the
  ones that need MudBlazor, ApexCharts or Redis types.
- The settings of the Settings page are stored in Redis, through `AppSettingService` and `IRedisService`.
- A page never reaches a repository or the `DatabaseContext`: it goes through its page view model.
- The charts of the Report page are drawn with Chart.js, loaded from `wwwroot/js/chart.min.js`.

## Adding a page

1. Create `Pages/<Name>.razor` with its `@page` route and `@attribute [Authorize]`.
2. Create `Pages/<Name>.razor.cs`, a partial class that inherits `ComponentBase`.
3. Create the page view model in `OpenBudgeteer.Core/ViewModels/PageViewModels`, with a `LoadData` method that
   returns a `ViewModelOperationResult`.
4. Inject `IServiceManager` and `IDialogService` in the code-behind, build the view model in
   `OnInitializedAsync`.
5. Copy `HandleResult` from `Pages/Account.razor.cs` and change the title of the dialog.
6. Add the entry to `Shared/NavMenu.razor`.
7. Add a test class for the view model in `OpenBudgeteer.Core.Test/Tests/PageViewModels`.
8. Run both test projects.
