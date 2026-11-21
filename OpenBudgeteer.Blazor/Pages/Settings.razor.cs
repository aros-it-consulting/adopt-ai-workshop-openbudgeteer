using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using OpenBudgeteer.Blazor.Common.Services;
using OpenBudgeteer.Blazor.Shared.Dialog;
using OpenBudgeteer.Blazor.ViewModels;
using OpenBudgeteer.Core.Common;
using OpenBudgeteer.Core.Data.Contracts.Services;
using OpenBudgeteer.Core.ViewModels.EntityViewModels;
using OpenBudgeteer.Core.ViewModels.Helper;

namespace OpenBudgeteer.Blazor.Pages;

public partial class Settings : ComponentBase
{
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private IServiceManager ServiceManager { get; set; } = null!;
    [Inject] private MudThemeService MudThemeService { get; set; } = null!;
    [Inject] private AppSettingService AppSettingService { get; set; } = null!;
    
    private SettingsPageViewModel _dataContext = null!;
    private TagListingViewModel _tagListingDataContext = null!;
    private bool _showAll;

    protected override async Task OnInitializedAsync()
    {
        _dataContext = new SettingsPageViewModel(ServiceManager, MudThemeService, AppSettingService);
        _tagListingDataContext = new TagListingViewModel(ServiceManager);
        await RestoreThemeAsync();
        await LoadTagsAsync();
    }
    
    private async Task HandleResult(ViewModelOperationResult result)
    {
        if (!result.IsSuccessful)
        {
            var parameters = new DialogParameters<ErrorMessageDialog>
            {
                { x => x.Title, "Settings" },
                { x => x.Message, result.Message }
            };
            await DialogService.ShowAsync<ErrorMessageDialog>("Settings", parameters);
        }
        if (result.ViewModelReloadRequired)
        {
            await _dataContext.LoadDataAsync();
            StateHasChanged();
        }
    }

    private async Task ApplyThemeAsync()
    {
        await HandleResult(await _dataContext.ApplyThemeAsync());
        StateHasChanged();
    }

    private async Task RestoreThemeAsync()
    {
        await HandleResult(await _dataContext.LoadDataAsync());
        StateHasChanged();
    }
    
    private async Task RestoreDefaultThemeAsync()
    {
        await _dataContext.RestoreDefaultThemeAsync();
        StateHasChanged();
    }

    private async Task ApplySettingsAsync()
    {
        await HandleResult(await _dataContext.ApplySettingsAsync());
        StateHasChanged();
    }
    
    private async Task RestoreSettingsAsync()
    {
        await HandleResult(await _dataContext.LoadDataAsync());
        StateHasChanged();
    }

    private async Task RenameTagAsync(TagViewModel tag)
    {
        await HandleTagResultAsync(tag.RenameTag());
    }

    private async Task DeleteTagAsync(TagViewModel tag)
    {
        var parameters = new DialogParameters<DeleteConfirmationDialog>
        {
            { dialog => dialog.Title, "Delete Tag" },
            { dialog => dialog.Message, $"Do you really want to delete the Tag \"{tag.Name}\"? " +
                                        $"It will be removed from {tag.TransactionCount} Transaction(s), which are kept." }
        };
        var dialog = await DialogService.ShowAsync<DeleteConfirmationDialog>("Delete Tag", parameters);
        var confirmation = await dialog.Result;
        if (confirmation is { Canceled: false }) await HandleTagResultAsync(tag.DeleteTag());
    }

    private async Task HandleTagResultAsync(ViewModelOperationResult result)
    {
        // Reload in any case: to show the new state, or to restore the name of a failed rename
        if (!result.IsSuccessful) await ShowTagErrorAsync(result.Message);
        await LoadTagsAsync();
    }

    private async Task LoadTagsAsync()
    {
        var result = _tagListingDataContext.LoadData();
        if (!result.IsSuccessful) await ShowTagErrorAsync(result.Message);
    }

    private async Task ShowTagErrorAsync(string message)
    {
        var parameters = new DialogParameters<ErrorMessageDialog>
        {
            { dialog => dialog.Title, "Tags" },
            { dialog => dialog.Message, message }
        };
        await DialogService.ShowAsync<ErrorMessageDialog>("Tags", parameters);
    }
}