﻿/*
 * MIT License
 * Copyright (c) 2023 Christopher Law
 * https://chrislaw.me
 */
using Promethix.Framework.Ado.Interfaces;

namespace Promethix.Framework.Ado.Implementation
{
    public class AmbientAdoContextLocator : IAmbientAdoContextLocator
    {
        public TAdoContext GetContext<TAdoContext>() where TAdoContext : AdoContext
        {
            AdoScope ambientAdoScope = AdoScope.GetAmbientScope();
            return ambientAdoScope?.AdoContexts.GetContext<TAdoContext>();
        }
    }
}
