# AdoScope (Pre-Release Alpha)

A simple and flexible way to manage your ADO.NET connections and transactions. 

Based on the impressive contributions to DbContextScope by Mehdime El Gueddari, I have great admiration for their brilliant work which inspired me to create AdoScope.

AdoScope will work with any ADO.NET provider however, I created it for Dapper. Having worked a great deal with Entity Framework and DbContextScope I wanted to create something similar for Dapper.

Dapper of course don't have a DbContext, but there are significant issues managing DbConnection and DbTransaction, so I created an AdoContext which is a wrapper around a DbConnection and DbTransaction.