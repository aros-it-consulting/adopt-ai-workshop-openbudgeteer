using OpenBudgeteer.Core.Data.Services.Generic;
using OpenBudgeteer.Core.Test.Mocking.Repository;

namespace OpenBudgeteer.Core.Test.Mocking.Services;

public class MockTagService : GenericTagService
{
    public MockTagService(MockDatabase mockDatabase) : base(new MockTagRepository(mockDatabase))
    {
    }
}
