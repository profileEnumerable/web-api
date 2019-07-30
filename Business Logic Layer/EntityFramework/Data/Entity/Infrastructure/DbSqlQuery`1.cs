// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DbSqlQuery`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel;
using System.Data.Entity.Internal;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// Represents a SQL query for entities that is created from a <see cref="T:System.Data.Entity.DbContext" />
  /// and is executed using the connection from that context.
  /// Instances of this class are obtained from the <see cref="T:System.Data.Entity.DbSet`1" /> instance for the
  /// entity type. The query is not executed when this object is created; it is executed
  /// each time it is enumerated, for example by using foreach.
  /// SQL queries for non-entities are created using <see cref="M:System.Data.Entity.Database.SqlQuery``1(System.String,System.Object[])" />.
  /// See <see cref="T:System.Data.Entity.Infrastructure.DbSqlQuery" /> for a non-generic version of this class.
  /// </summary>
  /// <typeparam name="TEntity">The type of entities returned by the query.</typeparam>
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
  public class DbSqlQuery<TEntity> : DbRawSqlQuery<TEntity> where TEntity : class
  {
    internal DbSqlQuery(InternalSqlQuery internalQuery)
      : base(internalQuery)
    {
    }

    /// <summary>
    /// Creates an instance of a <see cref="T:System.Data.Entity.Infrastructure.DbSqlQuery`1" /> when called from the constructor of a derived
    /// type that will be used as a test double for <see cref="M:System.Data.Entity.DbSet`1.SqlQuery(System.String,System.Object[])" />. Methods and properties
    /// that will be used by the test double must be implemented by the test double except AsNoTracking and
    /// AsStreaming where the default implementation is a no-op.
    /// </summary>
    protected DbSqlQuery()
      : this((InternalSqlQuery) null)
    {
    }

    /// <summary>
    /// Returns a new query where the entities returned will not be cached in the <see cref="T:System.Data.Entity.DbContext" />.
    /// </summary>
    /// <returns> A new query with NoTracking applied. </returns>
    public virtual DbSqlQuery<TEntity> AsNoTracking()
    {
      if (this.InternalQuery != null)
        return new DbSqlQuery<TEntity>(this.InternalQuery.AsNoTracking());
      return this;
    }

    /// <summary>
    /// Returns a new query that will stream the results instead of buffering.
    /// </summary>
    /// <returns> A new query with AsStreaming applied. </returns>
    [Obsolete("Queries are now streaming by default unless a retrying ExecutionStrategy is used. Calling this method will have no effect.")]
    public virtual DbSqlQuery<TEntity> AsStreaming()
    {
      if (this.InternalQuery != null)
        return new DbSqlQuery<TEntity>(this.InternalQuery.AsStreaming());
      return this;
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override string ToString()
    {
      return base.ToString();
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj)
    {
      return base.Equals(obj);
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public new Type GetType()
    {
      return base.GetType();
    }
  }
}
