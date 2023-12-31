﻿/*
 * MIT License
 * Copyright (c) 2023 Christopher Law
 * https://chrislaw.me
 */
using Promethix.Framework.Ado.Implementation;
using System;

namespace Promethix.Framework.Ado.Interfaces
{
    public interface IAdoContextGroup : IDisposable
    {
        TAdoContext GetContext<TAdoContext>() where TAdoContext : AdoContext;

        void Commit();

        void Rollback();
    }
}
