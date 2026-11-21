using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenBudgeteer.Core.Data.Contracts.Services;
using OpenBudgeteer.Core.Data.Entities;
using OpenBudgeteer.Core.Data.Entities.Models;
using OpenBudgeteer.Core.Data.Repository;
using OpenBudgeteer.Core.Data.Services.Exceptions;
using OpenBudgeteer.Core.Data.Services.Generic;

namespace OpenBudgeteer.Core.Data.Services.EFCore;

public class EFCoreTagService : EFCoreBaseService<Tag>, ITagService
{
    private readonly IDbContextFactory<DatabaseContext> _dbContextFactory;
    private readonly ILogger<EFCoreTagService> _logger;

    public EFCoreTagService(
        IDbContextFactory<DatabaseContext> dbContextFactory,
        ILogger<EFCoreTagService> logger) : base(dbContextFactory, logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    protected override GenericTagService CreateBaseService(DatabaseContext dbContext)
    {
        return new GenericTagService(new TagRepository(dbContext));
    }

    public IEnumerable<Tag> GetAllWithEntities()
    {
        try
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var genericTagService = CreateBaseService(dbContext);
            return genericTagService.GetAllWithEntities();
        }
        catch (EntityNotFoundException e)
        {
            throw new ServiceException($"Error on querying database: {e.Message}", _logger);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error on querying database.");
            throw;
        }
    }
}
