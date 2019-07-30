// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext
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
  /// Represents contextual information associated with calls into <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbCommandInterceptor" />
  /// implementations.
  /// </summary>
  /// <remarks>
  /// An instance of this class is passed to the dispatch methods of <see cref="T:System.Data.Entity.Infrastructure.Interception.DbCommandDispatcher" />
  /// and does not contain mutable information such as the result of the operation. This mutable information
  /// is obtained from the <see cref="T:System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext`1" /> that is passed to the interceptors.
  /// Instances of this class are publicly immutable. To add contextual information use one of the
  /// With... or As... methods to create a new interception context containing the new information.
  /// </remarks>
  public class DbCommandInterceptionContext : DbInterceptionContext
  {
    private CommandBehavior _commandBehavior;

    /// <summary>
    /// Constructs a new <see cref="T:System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext" /> with no state.
    /// </summary>
    public DbCommandInterceptionContext()
    {
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext" /> by copying state from the given
    /// interception context. Also see <see cref="M:System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext.Clone" />
    /// </summary>
    /// <param name="copyFrom">The context from which to copy state.</param>
    public DbCommandInterceptionContext(DbInterceptionContext copyFrom)
      : base(copyFrom)
    {
      Check.NotNull<DbInterceptionContext>(copyFrom, nameof (copyFrom));
      DbCommandInterceptionContext interceptionContext = copyFrom as DbCommandInterceptionContext;
      if (interceptionContext == null)
        return;
      this._commandBehavior = interceptionContext._commandBehavior;
    }

    /// <summary>
    /// The <see cref="P:System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext.CommandBehavior" /> that will be used or has been used to execute the command with a
    /// <see cref="T:System.Data.Common.DbDataReader" />. This property is only used for <see cref="M:System.Data.Common.DbCommand.ExecuteReader(System.Data.CommandBehavior)" />
    /// and its async counterparts.
    /// </summary>
    public CommandBehavior CommandBehavior
    {
      get
      {
        return this._commandBehavior;
      }
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext" /> that contains all the contextual information in this
    /// interception context together with the given <see cref="P:System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext.CommandBehavior" />.
    /// </summary>
    /// <param name="commandBehavior">The command behavior to associate.</param>
    /// <returns>A new interception context associated with the given command behavior.</returns>
    public DbCommandInterceptionContext WithCommandBehavior(
      CommandBehavior commandBehavior)
    {
      DbCommandInterceptionContext interceptionContext = this.TypedClone();
      interceptionContext._commandBehavior = commandBehavior;
      return interceptionContext;
    }

    private DbCommandInterceptionContext TypedClone()
    {
      return (DbCommandInterceptionContext) this.Clone();
    }

    /// <inheritdoc />
    protected override DbInterceptionContext Clone()
    {
      return (DbInterceptionContext) new DbCommandInterceptionContext((DbInterceptionContext) this);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext" /> that contains all the contextual information in this
    /// interception context with the addition of the given <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />.
    /// </summary>
    /// <param name="context">The context to associate.</param>
    /// <returns>A new interception context associated with the given context.</returns>
    public DbCommandInterceptionContext WithDbContext(DbContext context)
    {
      Check.NotNull<DbContext>(context, nameof (context));
      return (DbCommandInterceptionContext) base.WithDbContext(context);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext" /> that contains all the contextual information in this
    /// interception context with the addition of the given <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />.
    /// </summary>
    /// <param name="context">The context to associate.</param>
    /// <returns>A new interception context associated with the given context.</returns>
    public DbCommandInterceptionContext WithObjectContext(
      ObjectContext context)
    {
      Check.NotNull<ObjectContext>(context, nameof (context));
      return (DbCommandInterceptionContext) base.WithObjectContext(context);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext" /> that contains all the contextual information in this
    /// interception context the <see cref="P:System.Data.Entity.Infrastructure.Interception.DbInterceptionContext.IsAsync" /> flag set to true.
    /// </summary>
    /// <returns>A new interception context associated with the async flag set.</returns>
    public DbCommandInterceptionContext AsAsync()
    {
      return (DbCommandInterceptionContext) base.AsAsync();
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
