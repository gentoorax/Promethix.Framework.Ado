/*
 * MIT License
 * Copyright (c) 2023 Christopher Law
 * https://chrislaw.me
 */
namespace Promethix.Framework.Ado.Enums
{
    /// <summary>
    /// Sets whether the AdoContext will be executed in a transaction or not.
    /// </summary>
    public enum AdoContextExecutionOption
    {
        /// <summary>
        /// Set your AdoContext to this if you want a Unit of Work pattern.
        /// Recommended setting for most use cases.
        /// </summary>
        Transactional,

        /// <summary>
        /// Set your AdoContext to this if you want to execute your AdoContext without a transaction.
        /// </summary>
        NonTransactional
    }
}
