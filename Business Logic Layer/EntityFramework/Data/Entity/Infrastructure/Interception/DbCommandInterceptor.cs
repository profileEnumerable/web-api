// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.Interception.DbCommandInterceptor
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Common;

namespace System.Data.Entity.Infrastructure.Interception
{
  /// <summary>
  /// Base class that implements <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbCommandInterceptor" />. This class is a convenience for
  /// use when only one or two methods of the interface actually need to have any implementation.
  /// </summary>
  public class DbCommandInterceptor : IDbCommandInterceptor, IDbInterceptor
  {
    /// <inheritdoc />
    public virtual void NonQueryExecuting(
      DbCommand command,
      DbCommandInterceptionContext<int> interceptionContext)
    {
    }

    /// <inheritdoc />
    public virtual void NonQueryExecuted(
      DbCommand command,
      DbCommandInterceptionContext<int> interceptionContext)
    {
    }

    /// <inheritdoc />
    public virtual void ReaderExecuting(
      DbCommand command,
      DbCommandInterceptionContext<DbDataReader> interceptionContext)
    {
    }

    /// <inheritdoc />
    public virtual void ReaderExecuted(
      DbCommand command,
      DbCommandInterceptionContext<DbDataReader> interceptionContext)
    {
    }

    /// <inheritdoc />
    public virtual void ScalarExecuting(
      DbCommand command,
      DbCommandInterceptionContext<object> interceptionContext)
    {
    }

    /// <inheritdoc />
    public virtual void ScalarExecuted(
      DbCommand command,
      DbCommandInterceptionContext<object> interceptionContext)
    {
    }
  }
}
