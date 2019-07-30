// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.TransactionalBehavior
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity
{
  /// <summary>
  /// Controls the transaction creation behavior while executing a database command or query.
  /// </summary>
  public enum TransactionalBehavior
  {
    /// <summary>
    /// If no transaction is present then a new transaction will be used for the operation.
    /// </summary>
    EnsureTransaction,
    /// <summary>
    /// If an existing transaction is present then use it, otherwise execute the command or query without a transaction.
    /// </summary>
    DoNotEnsureTransaction,
  }
}
