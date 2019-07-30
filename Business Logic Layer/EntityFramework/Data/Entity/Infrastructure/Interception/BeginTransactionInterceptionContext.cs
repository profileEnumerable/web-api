// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.Interception.BeginTransactionInterceptionContext
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel;
using System.Data.Common;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Infrastructure.Interception
{
  /// <summary>
  /// Represents contextual information associated with calls to <see cref="M:System.Data.Common.DbConnection.BeginTransaction(System.Data.IsolationLevel)" />
  /// implementations.
  /// </summary>
  /// <remarks>
  /// Instances of this class are publicly immutable for contextual information. To add
  /// contextual information use one of the With... or As... methods to create a new
  /// interception context containing the new information.
  /// </remarks>
  public class BeginTransactionInterceptionContext : DbConnectionInterceptionContext<DbTransaction>
  {
    private IsolationLevel _isolationLevel = IsolationLevel.Unspecified;

    /// <summary>
    /// Constructs a new <see cref="T:System.Data.Entity.Infrastructure.Interception.BeginTransactionInterceptionContext" /> with no state.
    /// </summary>
    public BeginTransactionInterceptionContext()
    {
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Infrastructure.Interception.BeginTransactionInterceptionContext" /> by copying immutable state from the given
    /// interception context. Also see <see cref="M:System.Data.Entity.Infrastructure.Interception.BeginTransactionInterceptionContext.Clone" />
    /// </summary>
    /// <param name="copyFrom">The context from which to copy state.</param>
    public BeginTransactionInterceptionContext(DbInterceptionContext copyFrom)
      : base(copyFrom)
    {
      Check.NotNull<DbInterceptionContext>(copyFrom, nameof (copyFrom));
      BeginTransactionInterceptionContext interceptionContext = copyFrom as BeginTransactionInterceptionContext;
      if (interceptionContext == null)
        return;
      this._isolationLevel = interceptionContext._isolationLevel;
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Infrastructure.Interception.BeginTransactionInterceptionContext" /> that contains all the contextual information in this
    /// interception context together with the <see cref="P:System.Data.Entity.Infrastructure.Interception.DbInterceptionContext.IsAsync" /> flag set to true.
    /// </summary>
    /// <returns>A new interception context associated with the async flag set.</returns>
    public BeginTransactionInterceptionContext AsAsync()
    {
      return (BeginTransactionInterceptionContext) base.AsAsync();
    }

    /// <summary>
    /// The <see cref="P:System.Data.Entity.Infrastructure.Interception.BeginTransactionInterceptionContext.IsolationLevel" /> that will be used or has been used to start a transaction.
    /// </summary>
    public IsolationLevel IsolationLevel
    {
      get
      {
        return this._isolationLevel;
      }
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Infrastructure.Interception.BeginTransactionInterceptionContext" /> that contains all the contextual information in this
    /// interception context together with the given <see cref="P:System.Data.Entity.Infrastructure.Interception.BeginTransactionInterceptionContext.IsolationLevel" />.
    /// </summary>
    /// <param name="isolationLevel">The isolation level to associate.</param>
    /// <returns>A new interception context associated with the given isolation level.</returns>
    public BeginTransactionInterceptionContext WithIsolationLevel(
      IsolationLevel isolationLevel)
    {
      BeginTransactionInterceptionContext interceptionContext = this.TypedClone();
      interceptionContext._isolationLevel = isolationLevel;
      return interceptionContext;
    }

    private BeginTransactionInterceptionContext TypedClone()
    {
      return (BeginTransactionInterceptionContext) this.Clone();
    }

    /// <inheritdoc />
    protected override DbInterceptionContext Clone()
    {
      return (DbInterceptionContext) new BeginTransactionInterceptionContext((DbInterceptionContext) this);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Infrastructure.Interception.BeginTransactionInterceptionContext" /> that contains all the contextual information in this
    /// interception context with the addition of the given <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />.
    /// </summary>
    /// <param name="context">The context to associate.</param>
    /// <returns>A new interception context associated with the given context.</returns>
    public BeginTransactionInterceptionContext WithDbContext(
      DbContext context)
    {
      Check.NotNull<DbContext>(context, nameof (context));
      return (BeginTransactionInterceptionContext) base.WithDbContext(context);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Infrastructure.Interception.BeginTransactionInterceptionContext" /> that contains all the contextual information in this
    /// interception context with the addition of the given <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />.
    /// </summary>
    /// <param name="context">The context to associate.</param>
    /// <returns>A new interception context associated with the given context.</returns>
    public BeginTransactionInterceptionContext WithObjectContext(
      ObjectContext context)
    {
      Check.NotNull<ObjectContext>(context, nameof (context));
      return (BeginTransactionInterceptionContext) base.WithObjectContext(context);
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
