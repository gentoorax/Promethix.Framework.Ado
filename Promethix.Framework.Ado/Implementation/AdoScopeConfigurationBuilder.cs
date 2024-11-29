using Microsoft.Extensions.Configuration;
using System;

namespace Promethix.Framework.Ado.Implementation
{
    public class AdoScopeConfigurationBuilder
    {
        private AdoScopeOptionsBuilder OptionsBuilder { get; set; }

        public AdoScopeConfigurationBuilder ConfigureScope(Action<AdoScopeOptionsBuilder> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            OptionsBuilder = new AdoScopeOptionsBuilder();
            configure(OptionsBuilder);

            return this;
        }

        public AdoScopeOptionsBuilder Build()
        {
            return OptionsBuilder;
        }
    }
}
