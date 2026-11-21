using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenBudgeteer.Core.Data.Contracts.Services;
using OpenBudgeteer.Core.Data.Entities;
using OpenBudgeteer.Core.Data.Entities.Models;
using OpenBudgeteer.Core.Data.Repository;
using OpenBudgeteer.Core.Data.Services.Generic;

namespace OpenBudgeteer.Core.Data.Services.EFCore;

public class EFCoreTagService : EFCoreBaseService<Tag>, ITagService
{
    public EFCoreTagService(
        IDbContextFactory<DatabaseContext> dbContextFactory,
        ILogger<EFCoreTagService> logger) : base(dbContextFactory, logger)
    {
    }

    protected override GenericTagService CreateBaseService(DatabaseContext dbContext)
    {
        return new GenericTagService(new TagRepository(dbContext));
    }
}
