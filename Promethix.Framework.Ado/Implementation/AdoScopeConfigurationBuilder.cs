using System;

namespace Promethix.Framework.Ado.Implementation
{
    public class AdoScopeConfigurationBuilder
    {
        public AdoContextConfigurationBuilder AdoContextConfiguration { get; }

        public AdoScopeConfigurationBuilder() 
        {
            AdoContextConfiguration = new AdoContextConfigurationBuilder();
        }

        public AdoScopeConfigurationBuilder ConfigureScope(Action<AdoScopeOptionsBuilder> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var optionsBuilder = new AdoScopeOptionsBuilder();
            configure(optionsBuilder);

            return this;
        }
    }
}
