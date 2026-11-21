using System;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using OpenBudgeteer.Core.Common;
using OpenBudgeteer.Core.Data.Contracts.Services;
using OpenBudgeteer.Core.Data.Services.Exceptions;
using OpenBudgeteer.Core.ViewModels.EntityViewModels;

namespace OpenBudgeteer.Core.ViewModels.Helper;

public class TagListingViewModel : ViewModelBase
{
    /// <summary>
    /// Collection of loaded Tags, ordered by name
    /// </summary>
    public ObservableCollection<TagViewModel> Tags { get; }

    /// <summary>
    /// Basic Constructor
    /// </summary>
    /// <param name="serviceManager">Reference to API based services</param>
    public TagListingViewModel(IServiceManager serviceManager)
        : base(serviceManager, serviceManager.CreateLogger(typeof(TagListingViewModel)))
    {
        Tags = new ObservableCollection<TagViewModel>();
    }

    /// <summary>
    /// Initialize ViewModel and load data from database
    /// </summary>
    /// <returns>Object which contains information and results of this method</returns>
    public ViewModelOperationResult LoadData()
    {
        try
        {
            Tags.Clear();
            foreach (var tag in ServiceManager.TagService.GetAllWithEntities())
            {
                Tags.Add(TagViewModel.CreateFromTag(ServiceManager, tag));
            }
            return new ViewModelOperationResult(true);
        }
        catch (ServiceException e)
        {
            return new ViewModelOperationResult(false, e.Message);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "An unexpected error occurred.");
            return new ViewModelOperationResult(false, "An unexpected error occurred. Please check the logs for more details.");
        }
    }
}
