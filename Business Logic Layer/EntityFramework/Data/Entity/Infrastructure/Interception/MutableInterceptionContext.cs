// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.Interception.MutableInterceptionContext
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace System.Data.Entity.Infrastructure.Interception
{
  /// <summary>
  /// Represents contextual information associated with calls that don't return any results.
  /// </summary>
  public abstract class MutableInterceptionContext : DbInterceptionContext, IDbMutableInterceptionContext
  {
    private readonly InterceptionContextMutableData _mutableData = new InterceptionContextMutableData();

    /// <summary>
    /// Constructs a new <see cref="T:System.Data.Entity.Infrastructure.Interception.MutableInterceptionContext" /> with no state.
    /// </summary>
    protected MutableInterceptionContext()
    {
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Infrastructure.Interception.MutableInterceptionContext" /> by copying immutable state from the given
    /// interception context. Also see <see cref="M:System.Data.Entity.Infrastructure.Interception.DbInterceptionContext.Clone" />
    /// </summary>
    /// <param name="copyFrom">The context from which to copy state.</param>
    protected MutableInterceptionContext(DbInterceptionContext copyFrom)
      : base(copyFrom)
    {
      Check.NotNull<DbInterceptionContext>(copyFrom, nameof (copyFrom));
    }

    InterceptionContextMutableData IDbMutableInterceptionContext.MutableData
    {
      get
      {
        return this._mutableData;
      }
    }

    internal InterceptionContextMutableData MutableData
    {
      get
      {
        return this._mutableData;
      }
    }

    /// <summary>
    /// When true, this flag indicates that that execution of the operation has been suppressed by
    /// one of the interceptors. This can be done before the operation has executed by calling
    /// <see cref="M:System.Data.Entity.Infrastructure.Interception.MutableInterceptionContext.SuppressExecution" /> or by setting an <see cref="P:System.Data.Entity.Infrastructure.Interception.MutableInterceptionContext.Exception" /> to be thrown
    /// </summary>
    public bool IsExecutionSuppressed
    {
      get
      {
        return this._mutableData.IsExecutionSuppressed;
      }
    }

    /// <summary>
    /// Prevents the operation from being executed if called before the operation has executed.
    /// </summary>
    /// <exception cref="T:System.InvalidOperationException">
    /// Thrown if this method is called after the operation has already executed.
    /// </exception>
    public void SuppressExecution()
    {
      this._mutableData.SuppressExecution();
    }

    /// <summary>
    /// If execution of the operation fails, then this property will contain the exception that was
    /// thrown. If the operation was suppressed or did not fail, then this property will always be null.
    /// </summary>
    /// <remarks>
    /// When an operation fails both this property and the <see cref="P:System.Data.Entity.Infrastructure.Interception.MutableInterceptionContext.Exception" /> property are set
    /// to the exception that was thrown. However, the <see cref="P:System.Data.Entity.Infrastructure.Interception.MutableInterceptionContext.Exception" /> property can be set or
    /// changed by interceptors, while this property will always represent the original exception thrown.
    /// </remarks>
    public Exception OriginalException
    {
      get
      {
        return this._mutableData.OriginalException;
      }
    }

    /// <summary>
    /// If this property is set before the operation has executed, then execution of the operation will
    /// be suppressed and the set exception will be thrown instead. Otherwise, if the operation fails, then
    /// this property will be set to the exception that was thrown. In either case, interceptors that run
    /// after the operation can change this property to change the exception that will be thrown, or set this
    /// property to null to cause no exception to be thrown at all.
    /// </summary>
    /// <remarks>
    /// When an operation fails both this property and the <see cref="P:System.Data.Entity.Infrastructure.Interception.MutableInterceptionContext.OriginalException" /> property are set
    /// to the exception that was thrown. However, the this property can be set or changed by
    /// interceptors, while the <see cref="P:System.Data.Entity.Infrastructure.Interception.MutableInterceptionContext.OriginalException" /> property will always represent
    /// the original exception thrown.
    /// </remarks>
    public Exception Exception
    {
      get
      {
        return this._mutableData.Exception;
      }
      set
      {
        this._mutableData.Exception = value;
      }
    }

    /// <summary>
    /// Set to the status of the <see cref="T:System.Threading.Tasks.Task" /> after an async operation has finished. Not used for
    /// synchronous operations.
    /// </summary>
    public TaskStatus TaskStatus
    {
      get
      {
        return this._mutableData.TaskStatus;
      }
    }

    /// <summary>
    /// Gets or sets a value containing arbitrary user-specified state information associated with the operation.
    /// </summary>
    public object UserState
    {
      get
      {
        return this._mutableData.UserState;
      }
      set
      {
        this._mutableData.UserState = value;
      }
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Infrastructure.Interception.MutableInterceptionContext" /> that contains all the contextual information in this
    /// interception context together with the <see cref="P:System.Data.Entity.Infrastructure.Interception.DbInterceptionContext.IsAsync" /> flag set to true.
    /// </summary>
    /// <returns>A new interception context associated with the async flag set.</returns>
    public MutableInterceptionContext AsAsync()
    {
      return (MutableInterceptionContext) base.AsAsync();
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Infrastructure.Interception.MutableInterceptionContext" /> that contains all the contextual information in this
    /// interception context with the addition of the given <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />.
    /// </summary>
    /// <param name="context">The context to associate.</param>
    /// <returns>A new interception context associated with the given context.</returns>
    public MutableInterceptionContext WithDbContext(DbContext context)
    {
      Check.NotNull<DbContext>(context, nameof (context));
      return (MutableInterceptionContext) base.WithDbContext(context);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Infrastructure.Interception.MutableInterceptionContext" /> that contains all the contextual information in this
    /// interception context with the addition of the given <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />.
    /// </summary>
    /// <param name="context">The context to associate.</param>
    /// <returns>A new interception context associated with the given context.</returns>
    public MutableInterceptionContext WithObjectContext(
      ObjectContext context)
    {
      Check.NotNull<ObjectContext>(context, nameof (context));
      return (MutableInterceptionContext) base.WithObjectContext(context);
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
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new Type GetType()
    {
      return base.GetType();
    }
  }
}
