/*
 * MIT License
 * Copyright (c) 2023 Christopher Law
 * https://chrislaw.me
 */
using Promethix.Framework.Ado.Enums;
using Promethix.Framework.Ado.Interfaces;
using System;
using System.Data;

namespace Promethix.Framework.Ado.Implementation
{
    public class AdoScopeFactory : IAdoScopeFactory
    {
        private readonly IAdoContextGroupFactory adoContextGroupFactory;

        private readonly AdoScopeOptionsBuilder adoScopeOptions;

        public AdoScopeFactory(
            IAdoContextGroupFactory adoContextGroupFactory,
            AdoScopeOptionsBuilder adoScopeOptions) 
        {
            this.adoContextGroupFactory = adoContextGroupFactory;
            this.adoScopeOptions = adoScopeOptions;
        }

        public IAdoScope Create(AdoScopeOption adoScopeOption = AdoScopeOption.JoinExisting)
        {
            return new AdoScope(adoScopeOption, adoScopeOptions.OverrideDefaultIsolationLevel, adoContextGroupFactory, adoScopeOptions.ScopeExecutionOption);
        }

        public IAdoScope CreateWithTransaction(IsolationLevel isolationLevel)
        {
            return new AdoScope(AdoScopeOption.ForceCreateNew, isolationLevel, adoContextGroupFactory);
        }

        public IAdoScope CreateWithDistributedTransaction(IsolationLevel? isolationLevel)
        {
            return new AdoScope(AdoScopeOption.ForceCreateNew, isolationLevel, adoContextGroupFactory, AdoContextGroupExecutionOption.ExplicitDistributed);
        }
        
        public IDisposable SuppressAmbientContext()
        {
            return new AmbientContextSurpressor();
        }
    }
}
