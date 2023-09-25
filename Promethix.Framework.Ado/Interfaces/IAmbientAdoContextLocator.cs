/*
 * MIT License
 * Copyright (c) 2023 Christopher Law
 * https://chrislaw.me
 */
using Promethix.Framework.Ado.Implementation;

namespace Promethix.Framework.Ado.Interfaces
{
    public interface IAmbientAdoContextLocator
    {
        TAdoContext GetContext<TAdoContext>() where TAdoContext : AdoContext;
    }
}
