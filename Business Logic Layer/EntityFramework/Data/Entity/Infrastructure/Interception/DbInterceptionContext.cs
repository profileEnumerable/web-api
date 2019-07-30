// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.Interception.DbInterceptionContext
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.Infrastructure.Interception
{
  /// <summary>
  /// Represents contextual information associated with calls into <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbInterceptor" />
  /// implementations.
  /// </summary>
  /// <remarks>
  /// Note that specific types/operations that can be intercepted may use a more specific
  /// interception context derived from this class. For example, if SQL is being executed by
  /// a <see cref="T:System.Data.Entity.DbContext" />, then the DbContext will be contained in the
  /// <see cref="T:System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext`1" /> instance that is passed to the methods
  /// of <see cref="T:System.Data.Entity.Infrastructure.Interception.IDbCommandInterceptor" />.
  /// Instances of this class are publicly immutable for contextual information. To add
  /// contextual information use one of the With... or As... methods to create a new
  /// interception context containing the new information.
  /// </remarks>
  public class DbInterceptionContext
  {
    private readonly IList<DbContext> _dbContexts;
    private readonly IList<ObjectContext> _objectContexts;
    private bool _isAsync;

    /// <summary>
    /// Constructs a new <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterceptionContext" /> with no state.
    /// </summary>
    public DbInterceptionContext()
    {
      this._dbContexts = (IList<DbContext>) new List<DbContext>();
      this._objectContexts = (IList<ObjectContext>) new List<ObjectContext>();
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterceptionContext" /> by copying state from the given
    /// interception context. See <see cref="M:System.Data.Entity.Infrastructure.Interception.DbInterceptionContext.Clone" />
    /// </summary>
    /// <param name="copyFrom">The context from which to copy state.</param>
    protected DbInterceptionContext(DbInterceptionContext copyFrom)
    {
      Check.NotNull<DbInterceptionContext>(copyFrom, nameof (copyFrom));
      this._dbContexts = (IList<DbContext>) copyFrom.DbContexts.Where<DbContext>((Func<DbContext, bool>) (c =>
      {
        if (c.InternalContext != null)
          return !c.InternalContext.IsDisposed;
        return true;
      })).ToList<DbContext>();
      this._objectContexts = (IList<ObjectContext>) copyFrom.ObjectContexts.Where<ObjectContext>((Func<ObjectContext, bool>) (c => !c.IsDisposed)).ToList<ObjectContext>();
      this._isAsync = copyFrom._isAsync;
    }

    private DbInterceptionContext(IEnumerable<DbInterceptionContext> copyFrom)
    {
      this._dbContexts = (IList<DbContext>) copyFrom.SelectMany<DbInterceptionContext, DbContext>((Func<DbInterceptionContext, IEnumerable<DbContext>>) (c => c.DbContexts)).Distinct<DbContext>().Where<DbContext>((Func<DbContext, bool>) (c => !c.InternalContext.IsDisposed)).ToList<DbContext>();
      this._objectContexts = (IList<ObjectContext>) copyFrom.SelectMany<DbInterceptionContext, ObjectContext>((Func<DbInterceptionContext, IEnumerable<ObjectContext>>) (c => c.ObjectContexts)).Distinct<ObjectContext>().Where<ObjectContext>((Func<ObjectContext, bool>) (c => !c.IsDisposed)).ToList<ObjectContext>();
      this._isAsync = copyFrom.Any<DbInterceptionContext>((Func<DbInterceptionContext, bool>) (c => c.IsAsync));
    }

    /// <summary>
    /// Gets all the <see cref="T:System.Data.Entity.DbContext" /> instances associated with this interception context.
    /// </summary>
    /// <remarks>
    /// This list usually contains zero or one items. However, it can contain more than one item if
    /// a single <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" /> has been used to construct multiple <see cref="T:System.Data.Entity.DbContext" />
    /// instances.
    /// </remarks>
    public IEnumerable<DbContext> DbContexts
    {
      get
      {
        return (IEnumerable<DbContext>) this._dbContexts;
      }
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterceptionContext" /> that contains all the contextual information in this
    /// interception context with the addition of the given <see cref="T:System.Data.Entity.DbContext" />.
    /// </summary>
    /// <param name="context">The context to associate.</param>
    /// <returns>A new interception context associated with the given context.</returns>
    public DbInterceptionContext WithDbContext(DbContext context)
    {
      Check.NotNull<DbContext>(context, nameof (context));
      DbInterceptionContext interceptionContext = this.Clone();
      if (!((IEnumerable<object>) interceptionContext._dbContexts).Contains<object>((object) context, (IEqualityComparer<object>) ObjectReferenceEqualityComparer.Default))
        interceptionContext._dbContexts.Add(context);
      return interceptionContext;
    }

    /// <summary>
    /// Gets all the <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" /> instances associated with this interception context.
    /// </summary>
    /// <remarks>
    /// This list usually contains zero or one items. However, it can contain more than one item when
    /// EF has created a new <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" /> for use in database creation and initialization, or
    /// if a single <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnection" /> is used with multiple <see cref="P:System.Data.Entity.Infrastructure.Interception.DbInterceptionContext.ObjectContexts" />.
    /// </remarks>
    public IEnumerable<ObjectContext> ObjectContexts
    {
      get
      {
        return (IEnumerable<ObjectContext>) this._objectContexts;
      }
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterceptionContext" /> that contains all the contextual information in this
    /// interception context with the addition of the given <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />.
    /// </summary>
    /// <param name="context">The context to associate.</param>
    /// <returns>A new interception context associated with the given context.</returns>
    public DbInterceptionContext WithObjectContext(ObjectContext context)
    {
      Check.NotNull<ObjectContext>(context, nameof (context));
      DbInterceptionContext interceptionContext = this.Clone();
      if (!((IEnumerable<object>) interceptionContext._objectContexts).Contains<object>((object) context, (IEqualityComparer<object>) ObjectReferenceEqualityComparer.Default))
        interceptionContext._objectContexts.Add(context);
      return interceptionContext;
    }

    /// <summary>
    /// True if the operation is being executed asynchronously, otherwise false.
    /// </summary>
    public bool IsAsync
    {
      get
      {
        return this._isAsync;
      }
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterceptionContext" /> that contains all the contextual information in this
    /// interception context the <see cref="P:System.Data.Entity.Infrastructure.Interception.DbInterceptionContext.IsAsync" /> flag set to true.
    /// </summary>
    /// <returns>A new interception context associated with the async flag set.</returns>
    public DbInterceptionContext AsAsync()
    {
      DbInterceptionContext interceptionContext = this.Clone();
      interceptionContext._isAsync = true;
      return interceptionContext;
    }

    /// <summary>
    /// Call this method when creating a copy of an interception context in order to add new state
    /// to it. Using this method instead of calling the constructor directly ensures virtual dispatch
    /// so that the new type will have the same type (and any specialized state) as the context that
    /// is being cloned.
    /// </summary>
    /// <returns>A new context with all state copied.</returns>
    protected virtual DbInterceptionContext Clone()
    {
      return new DbInterceptionContext(this);
    }

    internal static DbInterceptionContext Combine(
      IEnumerable<DbInterceptionContext> interceptionContexts)
    {
      return new DbInterceptionContext(interceptionContexts);
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

    /// <summary>
    /// Gets the <see cref="T:System.Type" /> of the current instance.
    /// </summary>
    /// <returns>The exact runtime type of the current instance.</returns>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new Type GetType()
    {
      return base.GetType();
    }
  }
}
