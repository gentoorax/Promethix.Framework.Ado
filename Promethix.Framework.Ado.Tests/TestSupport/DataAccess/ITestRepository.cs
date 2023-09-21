using Promethix.Framework.Ado.Tests.TestSupport.Entities;

namespace Promethix.Framework.Ado.Tests.TestSupport.DataAccess
{
    public interface ITestRepository
    {
        void Add(TestEntity entity);
        TestEntity GetEntityByName(string v);
    }
}