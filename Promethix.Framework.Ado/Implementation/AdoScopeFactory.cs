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

        public AdoScopeFactory(IAdoContextGroupFactory adoContextGroupFactory) 
        {
            this.adoContextGroupFactory = adoContextGroupFactory;
        }

        public IAdoScope Create(AdoScopeOption adoScopeOption = AdoScopeOption.JoinExisting)
        {
            return new AdoScope(adoScopeOption, adoContextGroupFactory);
        }

        public IAdoScope CreateWithTransaction(IsolationLevel isolationLevel)
        {
            return new AdoScope(AdoScopeOption.ForceCreateNew, isolationLevel, adoContextGroupFactory);
        }

        public IDisposable SuppressAmbientContext()
        {
            return new AmbientContextSurpressor();
        }
    }
}
