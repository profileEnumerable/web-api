// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.Interception.DbConnectionPropertyInterceptionContext`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Infrastructure.Interception
{
  /// <summary>
  /// Represents contextual information associated with calls to property setters of type <typeparamref name="TValue" /> on a <see cref="T:System.Data.Common.DbConnection" />.
  /// </summary>
  /// <typeparam name="TValue">The type of the target property.</typeparam>
  public class DbConnectionPropertyInterceptionContext<TValue> : PropertyInterceptionContext<TValue>
  {
    /// <summary>
    /// Constructs a new <see cref="T:System.Data.Entity.Infrastructure.Interception.DbConnectionPropertyInterceptionContext`1" /> with no state.
    /// </summary>
    public DbConnectionPropertyInterceptionContext()
    {
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Infrastructure.Interception.DbConnectionPropertyInterceptionContext`1" /> by copying immutable state from the given
    /// interception context. Also see <see cref="M:System.Data.Entity.Infrastructure.Interception.DbConnectionPropertyInterceptionContext`1.Clone" />
    /// </summary>
    /// <param name="copyFrom">The context from which to copy state.</param>
    public DbConnectionPropertyInterceptionContext(DbInterceptionContext copyFrom)
      : base(copyFrom)
    {
      Check.NotNull<DbInterceptionContext>(copyFrom, nameof (copyFrom));
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Infrastructure.Interception.DbConnectionPropertyInterceptionContext`1" /> that contains all the contextual information in this
    /// interception context together with the given property value.
    /// </summary>
    /// <param name="value">The value that will be assigned to the target property.</param>
    /// <returns>A new interception context associated with the given property value.</returns>
    public DbConnectionPropertyInterceptionContext<TValue> WithValue(
      TValue value)
    {
      return (DbConnectionPropertyInterceptionContext<TValue>) base.WithValue(value);
    }

    /// <inheritdoc />
    protected override DbInterceptionContext Clone()
    {
      return (DbInterceptionContext) new DbConnectionPropertyInterceptionContext<TValue>((DbInterceptionContext) this);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Infrastructure.Interception.DbConnectionPropertyInterceptionContext`1" /> that contains all the contextual information in this
    /// interception context together with the <see cref="P:System.Data.Entity.Infrastructure.Interception.DbInterceptionContext.IsAsync" /> flag set to true.
    /// </summary>
    /// <returns>A new interception context associated with the async flag set.</returns>
    public DbConnectionPropertyInterceptionContext<TValue> AsAsync()
    {
      return (DbConnectionPropertyInterceptionContext<TValue>) base.AsAsync();
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Infrastructure.Interception.DbConnectionPropertyInterceptionContext`1" /> that contains all the contextual information in this
    /// interception context with the addition of the given <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />.
    /// </summary>
    /// <param name="context">The context to associate.</param>
    /// <returns>A new interception context associated with the given context.</returns>
    public DbConnectionPropertyInterceptionContext<TValue> WithDbContext(
      DbContext context)
    {
      Check.NotNull<DbContext>(context, nameof (context));
      return (DbConnectionPropertyInterceptionContext<TValue>) base.WithDbContext(context);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Infrastructure.Interception.DbConnectionPropertyInterceptionContext`1" /> that contains all the contextual information in this
    /// interception context with the addition of the given <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />.
    /// </summary>
    /// <param name="context">The context to associate.</param>
    /// <returns>A new interception context associated with the given context.</returns>
    public DbConnectionPropertyInterceptionContext<TValue> WithObjectContext(
      ObjectContext context)
    {
      Check.NotNull<ObjectContext>(context, nameof (context));
      return (DbConnectionPropertyInterceptionContext<TValue>) base.WithObjectContext(context);
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
