using Promethix.Framework.Ado.Tests.TestSupport.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethix.Framework.Ado.Tests.TestSupport.DataAccess
{
    public interface IMultiTestRepository
    {
        void Add(TestEntity entity);

        void CreateDatabase();
    }
}
