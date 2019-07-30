// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DbUpdateConcurrencyException
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core;
using System.Data.Entity.Internal;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// Exception thrown by <see cref="T:System.Data.Entity.DbContext" /> when it was expected that SaveChanges for an entity would
  /// result in a database update but in fact no rows in the database were affected.  This usually indicates
  /// that the database has been concurrently updated such that a concurrency token that was expected to match
  /// did not actually match.
  /// Note that state entries referenced by this exception are not serialized due to security and accesses to
  /// the state entries after serialization will return null.
  /// </summary>
  [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "SerializeObjectState used instead")]
  [Serializable]
  public class DbUpdateConcurrencyException : DbUpdateException
  {
    internal DbUpdateConcurrencyException(
      InternalContext context,
      OptimisticConcurrencyException innerException)
      : base(context, (UpdateException) innerException, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Infrastructure.DbUpdateException" /> class.
    /// </summary>
    public DbUpdateConcurrencyException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Infrastructure.DbUpdateException" /> class.
    /// </summary>
    /// <param name="message"> The message. </param>
    public DbUpdateConcurrencyException(string message)
      : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Infrastructure.DbUpdateException" /> class.
    /// </summary>
    /// <param name="message"> The message. </param>
    /// <param name="innerException"> The inner exception. </param>
    public DbUpdateConcurrencyException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
