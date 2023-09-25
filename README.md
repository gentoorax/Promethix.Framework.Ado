# AdoScope (Pre-Release Alpha)

AdoScope offers a simple and flexible solution for managing your ADO.NET connections and transactions. It draws inspiration
from the remarkable work in DbContextScope by Mehdime El Gueddari, whose DbContextScope library has been a source of
great inspiration for the creation of AdoScope.

While AdoScope is compatible with any ADO.NET provider, it was specifically designed with Dapper in mind. Having extensive
experience with Entity Framework and DbContextScope, the goal was to provide a similar solution tailored to the requirements
of Dapper.

Unlike Entity Framework, Dapper lacks a DbContext, which can lead to challenges in managing DbConnection and DbTransaction.
To address this, AdoScope introduces the concept of an AdoContext—a wrapper around DbConnection and DbTransaction, simplifying their management.

If you are seeking a Unit of Work pattern for Dapper with minimal coding overhead, AdoScope provides an elegant solution.