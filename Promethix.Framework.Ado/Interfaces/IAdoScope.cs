/*
 * MIT License
 * Copyright (c) 2023 Christopher Law
 * https://chrislaw.me
 */
using System;

namespace Promethix.Framework.Ado.Interfaces
{
    public interface IAdoScope : IDisposable
    {
        void Complete();
    }
}
