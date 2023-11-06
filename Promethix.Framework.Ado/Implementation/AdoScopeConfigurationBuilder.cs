using Microsoft.Extensions.Configuration;
using System;

namespace Promethix.Framework.Ado.Implementation
{
    public class AdoScopeConfigurationBuilder
    {
        private AdoScopeOptionsBuilder optionsBuilder { get; set; }

        public AdoScopeConfigurationBuilder ConfigureScope(Action<AdoScopeOptionsBuilder> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            optionsBuilder = new AdoScopeOptionsBuilder();
            configure(optionsBuilder);

            return this;
        }

        public AdoScopeOptionsBuilder Build()
        {
            return optionsBuilder;
        }
    }
}
