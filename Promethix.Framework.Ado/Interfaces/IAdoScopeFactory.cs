/*
 * MIT License
 * Copyright (c) 2023 Christopher Law
 * https://chrislaw.me
 */
using Promethix.Framework.Ado.Enums;
using System.Data;

namespace Promethix.Framework.Ado.Interfaces
{
    public interface IAdoScopeFactory
    {
        /// <summary>
        /// Creates a new AdoScope.
        /// 
        /// By default, the new scope will join any existing ambient scope. This is the recommended option.
        /// This ensures the same ADO connection and/or transaction is used by all service methods within the scope
        /// of a business transaction.
        /// 
        /// NOTE: Whether the ADO connection is transactional is determined by the AdoContext configuration. Unlike
        /// DbContextScope and EF, if transactional then the transaction will be opened for the duration of the scope.
        /// So be careful not to create a scope that is too long-lived if ExecutionsOption is set to 'Transactional'!
        /// 
        /// Set 'joiningOption' to 'ForceCreateNew' if you want to ignore the ambient scope
        /// and force the creation of new AdoContext instances within that scope. Using 'ForceCreateNew'
        /// is an advanced feature that should be used with great care and only if you fully understand the
        /// implications of doing this.
        /// </summary>
        /// <param name="adoScopeOption"></param>
        /// <returns></returns>
        IAdoScope Create(AdoScopeOption adoScopeOption = AdoScopeOption.JoinExisting);

        /// <summary>
        /// Creates a new AdoScope with an explicit database transaction and separate ADO connection.
        /// 
        /// Forces the creation of a new ambient AdoScope (i.e. does not
        /// join the ambient scope if there is one) and wraps all AdoContext instances
        /// created within that scope in an explicit database transaction with 
        /// the provided isolation level. 
        /// 
        /// WARNING: the database transaction will remain open for the whole 
        /// duration of the scope! So keep the scope as short-lived as possible.
        /// Don't make any remote API calls or perform any long running computation 
        /// within that scope.
        /// 
        /// This is an advanced feature that you should use very carefully
        /// and only if you fully understand the implications of doing this.
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        IAdoScope CreateWithTransaction(IsolationLevel isolationLevel);

        // TODO: Implement ambient scope surpression for certain parallel execution scenarios
    }
}
