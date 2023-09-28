using Promethix.Framework.Ado.Interfaces;
using System;

namespace Promethix.Framework.Ado.Implementation
{
    public class AdoContextConfigurationBuilder
    {
        private IAdoContextOptionsRegistry adoContextOptionsRegistry = new AdoContextOptionsRegistry();

        public AdoContextConfigurationBuilder AddAdoContext<TAdoContext>(Action<AdoContextOptionsBuilder> configure)
            where TAdoContext : AdoContext
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }   

            var optionsBuilder = new AdoContextOptionsBuilder();
            configure(optionsBuilder);

            // Register the options for the context type in the dictionary
            adoContextOptionsRegistry.AddContextOptions<TAdoContext>(optionsBuilder);

            return this;
        }

        public IAdoContextOptionsRegistry Build()
        {
            return adoContextOptionsRegistry;
        }
    }



}
