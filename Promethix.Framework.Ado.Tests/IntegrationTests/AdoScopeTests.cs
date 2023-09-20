using Promethix.Framework.Ado.Implementation;
using Promethix.Framework.Ado.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethix.Framework.Ado.Tests.IntegrationTests
{
    [TestClass]
    public class AdoScopeTests
    {
        [TestMethod, TestCategory("IntegrationTests")]
        public void SqlLiteAdoScopeTest()
        {
            IAdoScopeFactory adoScopeFactory = new AdoScopeFactory();

            using IAdoScope adoScope = adoScopeFactory.Create();
        }
    }
}
