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

        private readonly AdoScopeOptionsBuilder adoScopeBuilderOptions;

        public AdoScopeFactory(
            IAdoContextGroupFactory adoContextGroupFactory,
            AdoScopeOptionsBuilder adoScopeBuilderOptions) 
        {
            this.adoContextGroupFactory = adoContextGroupFactory;
            this.adoScopeBuilderOptions = adoScopeBuilderOptions;
        }

        public IAdoScope Create(AdoScopeOption adoScopeOption = AdoScopeOption.JoinExisting)
        {
            return new AdoScope(adoScopeOption, adoScopeBuilderOptions.DefaultIsolationLevel, adoContextGroupFactory, adoScopeBuilderOptions.ScopeExecutionOption);
        }

        public IAdoScope CreateWithTransaction(IsolationLevel isolationLevel)
        {
            return new AdoScope(AdoScopeOption.ForceCreateNew, isolationLevel, adoContextGroupFactory);
        }

        public IAdoScope CreateWithDistributedTransaction(IsolationLevel? isolationLevel)
        {
            return new AdoScope(AdoScopeOption.ForceCreateNew, isolationLevel, adoContextGroupFactory, AdoContextGroupExecutionOption.Distributed);
        }
        
        public IDisposable SuppressAmbientContext()
        {
            return new AmbientContextSurpressor();
        }
    }
}
