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

        private readonly AdoContextGroupExecutionOption adoContextGroupExecutionOption;

        private readonly IsolationLevel? isolationLevel;

        public AdoScopeFactory(
            IAdoContextGroupFactory adoContextGroupFactory,
            IsolationLevel? isolationLevel = null,
            AdoContextGroupExecutionOption adoContextGroupExecutionOption = AdoContextGroupExecutionOption.Standard) 
        {
            this.adoContextGroupFactory = adoContextGroupFactory;
            this.adoContextGroupExecutionOption = adoContextGroupExecutionOption;
            this.isolationLevel = isolationLevel;
        }

        public IAdoScope Create(AdoScopeOption adoScopeOption = AdoScopeOption.JoinExisting)
        {
            return new AdoScope(adoScopeOption, isolationLevel, adoContextGroupFactory, adoContextGroupExecutionOption);
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
