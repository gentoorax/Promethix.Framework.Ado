using Promethix.Framework.Ado.Tests.TestSupport.Entities;

namespace Promethix.Framework.Ado.Tests.TestSupport.DataAccess
{
    public interface ISimpleTestRepository
    {
        void Add(TestEntity entity);

        TestEntity GetEntityByName(string name);
    }
}