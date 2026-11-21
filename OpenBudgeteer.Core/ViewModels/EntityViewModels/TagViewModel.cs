using System;
using Microsoft.Extensions.Logging;
using OpenBudgeteer.Core.Common;
using OpenBudgeteer.Core.Data.Contracts.Services;
using OpenBudgeteer.Core.Data.Entities.Models;
using OpenBudgeteer.Core.Data.Services.Exceptions;

namespace OpenBudgeteer.Core.ViewModels.EntityViewModels;

public class TagViewModel : BaseEntityViewModel<Tag>
{
    #region Properties & Fields

    /// <summary>
    /// Database Id of the Tag
    /// </summary>
    public readonly Guid TagId;

    private string _name;
    /// <summary>
    /// Name of the Tag
    /// </summary>
    public string Name
    {
        get => _name;
        set => Set(ref _name, value);
    }

    /// <summary>
    /// Number of Transactions which have this Tag
    /// </summary>
    public int TransactionCount { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initialize ViewModel based on an existing <see cref="Tag"/> object
    /// </summary>
    /// <param name="serviceManager">Reference to API based services</param>
    /// <param name="tag">Tag instance, with its assignments to count its Transactions</param>
    protected TagViewModel(IServiceManager serviceManager, Tag tag)
        : base(serviceManager, serviceManager.CreateLogger(typeof(TagViewModel)))
    {
        TagId = tag.Id;
        _name = tag.Name;
        TransactionCount = tag.TransactionTags?.Count ?? 0;
    }

    /// <summary>
    /// Initialize a copy of the passed ViewModel
    /// </summary>
    /// <param name="viewModel">Current ViewModel instance</param>
    protected TagViewModel(TagViewModel viewModel) : base(viewModel.ServiceManager, viewModel.Logger)
    {
        TagId = viewModel.TagId;
        _name = viewModel.Name;
        TransactionCount = viewModel.TransactionCount;
    }

    /// <summary>
    /// Initialize ViewModel based on an existing <see cref="Tag"/> object
    /// </summary>
    /// <param name="serviceManager">Reference to API based services</param>
    /// <param name="tag">Tag instance, with its assignments to count its Transactions</param>
    /// <returns>New ViewModel instance</returns>
    public static TagViewModel CreateFromTag(IServiceManager serviceManager, Tag tag)
    {
        return new TagViewModel(serviceManager, tag);
    }

    /// <summary>
    /// Return a deep copy of the ViewModel
    /// </summary>
    public override object Clone()
    {
        return new TagViewModel(this);
    }

    #endregion

    #region Modification Handler

    internal override Tag ConvertToDto()
    {
        return new Tag
        {
            Id = TagId,
            Name = Name
        };
    }

    /// <summary>
    /// Stores the current <see cref="Name"/> in the database. All Transactions with this Tag show the new name.
    /// </summary>
    /// <remarks>Triggers <see cref="ViewModelOperationResult.ViewModelReloadRequired"/></remarks>
    /// <returns>Object which contains information and results of this method</returns>
    public ViewModelOperationResult RenameTag()
    {
        try
        {
            ServiceManager.TagService.Update(ConvertToDto());
            return new ViewModelOperationResult(true, true);
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

    /// <summary>
    /// Removes the Tag from the database and from all Transactions which have it. The Transactions are kept.
    /// </summary>
    /// <remarks>Triggers <see cref="ViewModelOperationResult.ViewModelReloadRequired"/></remarks>
    /// <returns>Object which contains information and results of this method</returns>
    public ViewModelOperationResult DeleteTag()
    {
        try
        {
            ServiceManager.TagService.Delete(TagId);
            return new ViewModelOperationResult(true, true);
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

    #endregion

    public override string ToString() => Name;
}
