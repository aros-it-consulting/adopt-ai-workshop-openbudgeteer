using OpenBudgeteer.Core.Data.Entities.Models;

namespace OpenBudgeteer.Core.Data.Contracts.Services;

public interface ITagService : IBaseService<Tag>
{
    public IEnumerable<Tag> GetAllWithEntities();
}
