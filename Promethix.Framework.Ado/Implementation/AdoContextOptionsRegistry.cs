using Promethix.Framework.Ado.Interfaces;
using System;
using System.Collections.Generic;

namespace Promethix.Framework.Ado.Implementation
{
    public class AdoContextOptionsRegistry : IAdoContextOptionsRegistry
    {
        private readonly Dictionary<Type, AdoContextOptionsBuilder> adoContextOptions = [];

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
