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

        private readonly AdoScopeOptions adoScopeOptions;

        public AdoScopeFactory(
            IAdoContextGroupFactory adoContextGroupFactory,
            AdoScopeOptionsBuilder adoScopeBuilderOptions) 
        {
            this.adoContextGroupFactory = adoContextGroupFactory;
            adoScopeOptions = adoScopeBuilderOptions?.adoScopeOptions;
        }

        public IAdoScope Create(AdoScopeOption adoScopeOption = AdoScopeOption.JoinExisting)
        {
            var overrides = Overrides().WithJoinOption(adoScopeOption);

            return new AdoScope(overrides, adoContextGroupFactory);
        }

        public IAdoScope CreateWithTransaction(IsolationLevel isolationLevel)
        {
            var overrides = Overrides()
                                .WithJoinOption(AdoScopeOption.ForceCreateNew)
                                .WithIsolationLevel(isolationLevel);

            return new AdoScope(overrides, adoContextGroupFactory);
        }

        public IAdoScope CreateWithDistributedTransaction(IsolationLevel? isolationLevel)
        {
            var overrides = Overrides()
                                .WithJoinOption(AdoScopeOption.ForceCreateNew)
                                .WithIsolationLevel(isolationLevel)
                                .WithAdoScopeExecutionOption(AdoContextGroupExecutionOption.Distributed);

            return new AdoScope(overrides, adoContextGroupFactory);
        }

        public IDisposable SuppressAmbientContext()
        {
            return new AmbientContextSurpressor();
        }

        private AdoScopeOverrideOptionsBuilder Overrides()
        {
            return new AdoScopeOverrideOptionsBuilder(adoScopeOptions);
        }
    }
}
