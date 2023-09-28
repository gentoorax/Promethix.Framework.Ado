using Promethix.Framework.Ado.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethix.Framework.Ado.Implementation
{
    public class AdoContextOptionsRegistry : IAdoContextOptionsRegistry
    {
        private Dictionary<Type, AdoContextOptionsBuilder> adoContextOptions = new();

        public void AddContextOptions<TAdoContext>(AdoContextOptionsBuilder optionsBuilder)
        {
            adoContextOptions.Add(typeof(TAdoContext), optionsBuilder);
        }

        public bool TryGetContextOptions<TAdoContext>(out AdoContextOptionsBuilder optionsBuilder)
        {
            return adoContextOptions.TryGetValue(typeof(TAdoContext), out optionsBuilder);
        }
    }
}
