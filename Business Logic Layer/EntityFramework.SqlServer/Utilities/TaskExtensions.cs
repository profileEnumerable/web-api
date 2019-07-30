// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.Utilities.TaskExtensions
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity.SqlServer.Utilities
{
  /// <summary>
  /// Contains extension methods for the <see cref="T:System.Threading.Tasks.Task" /> class.
  /// </summary>
  public static class TaskExtensions
  {
    /// <summary>
    /// Configures an awaiter used to await this <see cref="T:System.Threading.Tasks.Task`1" /> to avoid
    /// marshalling the continuation
    /// back to the original context, but preserve the current culture and UI culture.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the result produced by the associated <see cref="T:System.Threading.Tasks.Task`1" />.
    /// </typeparam>
    /// <param name="task">The task to be awaited on.</param>
    /// <returns>An object used to await this task.</returns>
    public static TaskExtensions.CultureAwaiter<T> WithCurrentCulture<T>(
      this Task<T> task)
    {
      return new TaskExtensions.CultureAwaiter<T>(task);
    }

    /// <summary>
    /// Configures an awaiter used to await this <see cref="T:System.Threading.Tasks.Task" /> to avoid
    /// marshalling the continuation
    /// back to the original context, but preserve the current culture and UI culture.
    /// </summary>
    /// <param name="task">The task to be awaited on.</param>
    /// <returns>An object used to await this task.</returns>
    public static TaskExtensions.CultureAwaiter WithCurrentCulture(this Task task)
    {
      return new TaskExtensions.CultureAwaiter(task);
    }

    /// <summary>
    /// Provides an awaitable object that allows for awaits on <see cref="T:System.Threading.Tasks.Task`1" /> that
    /// preserve the culture.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the result produced by the associated <see cref="T:System.Threading.Tasks.Task`1" />.
    /// </typeparam>
    /// <remarks>This type is intended for compiler use only.</remarks>
    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Awaiter")]
    [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
    public struct CultureAwaiter<T> : ICriticalNotifyCompletion, INotifyCompletion
    {
      private readonly Task<T> _task;

      /// <summary>
      /// Constructs a new instance of the <see cref="T:System.Data.Entity.SqlServer.Utilities.TaskExtensions.CultureAwaiter`1" /> class.
      /// </summary>
      /// <param name="task">The task to be awaited on.</param>
      public CultureAwaiter(Task<T> task)
      {
        this._task = task;
      }

      /// <summary>Gets an awaiter used to await this <see cref="T:System.Threading.Tasks.Task`1" />.</summary>
      /// <returns>An awaiter instance.</returns>
      /// <remarks>This method is intended for compiler user rather than use directly in code.</remarks>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Awaiter")]
      [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
      public TaskExtensions.CultureAwaiter<T> GetAwaiter()
      {
        return this;
      }

      /// <summary>
      /// Gets whether this <see cref="T:System.Threading.Tasks.Task">Task</see> has completed.
      /// </summary>
      /// <remarks>
      /// <see cref="P:System.Data.Entity.SqlServer.Utilities.TaskExtensions.CultureAwaiter`1.IsCompleted" /> will return true when the Task is in one of the three
      /// final states: <see cref="F:System.Threading.Tasks.TaskStatus.RanToCompletion">RanToCompletion</see>,
      /// <see cref="F:System.Threading.Tasks.TaskStatus.Faulted">Faulted</see>, or
      /// <see cref="F:System.Threading.Tasks.TaskStatus.Canceled">Canceled</see>.
      /// </remarks>
      public bool IsCompleted
      {
        get
        {
          return this._task.IsCompleted;
        }
      }

      /// <summary>Ends the await on the completed <see cref="T:System.Threading.Tasks.Task`1" />.</summary>
      /// <returns>The result of the completed <see cref="T:System.Threading.Tasks.Task`1" />.</returns>
      /// <exception cref="T:System.NullReferenceException">The awaiter was not properly initialized.</exception>
      /// <exception cref="T:System.Threading.Tasks.TaskCanceledException">The task was canceled.</exception>
      /// <exception cref="T:System.Exception">The task completed in a Faulted state.</exception>
      [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
      public T GetResult()
      {
        return this._task.GetAwaiter().GetResult();
      }

      /// <summary>This method is not implemented and should not be called.</summary>
      /// <param name="continuation">The action to invoke when the await operation completes.</param>
      public void OnCompleted(Action continuation)
      {
        throw new NotImplementedException();
      }

      /// <summary>
      /// Schedules the continuation onto the <see cref="T:System.Threading.Tasks.Task`1" /> associated with this
      /// <see cref="T:System.Runtime.CompilerServices.TaskAwaiter`1" />.
      /// </summary>
      /// <param name="continuation">The action to invoke when the await operation completes.</param>
      /// <exception cref="T:System.ArgumentNullException">
      /// The <paramref name="continuation" /> argument is null
      /// (Nothing in Visual Basic).
      /// </exception>
      /// <exception cref="T:System.InvalidOperationException">The awaiter was not properly initialized.</exception>
      /// <remarks>This method is intended for compiler user rather than use directly in code.</remarks>
      public void UnsafeOnCompleted(Action continuation)
      {
        CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
        CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
        this._task.ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted((Action) (() =>
        {
          CultureInfo currentCulture1 = Thread.CurrentThread.CurrentCulture;
          CultureInfo currentUiCulture = Thread.CurrentThread.CurrentUICulture;
          Thread.CurrentThread.CurrentCulture = currentCulture;
          Thread.CurrentThread.CurrentUICulture = currentUICulture;
          try
          {
            continuation();
          }
          finally
          {
            Thread.CurrentThread.CurrentCulture = currentCulture1;
            Thread.CurrentThread.CurrentUICulture = currentUiCulture;
          }
        }));
      }
    }

    /// <summary>
    /// Provides an awaitable object that allows for awaits on <see cref="T:System.Threading.Tasks.Task" /> that
    /// preserve the culture.
    /// </summary>
    /// <remarks>This type is intended for compiler use only.</remarks>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Awaiter")]
    [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
    public struct CultureAwaiter : ICriticalNotifyCompletion, INotifyCompletion
    {
      private readonly Task _task;

      /// <summary>
      /// Constructs a new instance of the <see cref="T:System.Data.Entity.SqlServer.Utilities.TaskExtensions.CultureAwaiter" /> class.
      /// </summary>
      /// <param name="task">The task to be awaited on.</param>
      public CultureAwaiter(Task task)
      {
        this._task = task;
      }

      /// <summary>Gets an awaiter used to await this <see cref="T:System.Threading.Tasks.Task" />.</summary>
      /// <returns>An awaiter instance.</returns>
      /// <remarks>This method is intended for compiler user rather than use directly in code.</remarks>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Awaiter")]
      [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
      public TaskExtensions.CultureAwaiter GetAwaiter()
      {
        return this;
      }

      /// <summary>
      /// Gets whether this <see cref="T:System.Threading.Tasks.Task">Task</see> has completed.
      /// </summary>
      /// <remarks>
      /// <see cref="P:System.Data.Entity.SqlServer.Utilities.TaskExtensions.CultureAwaiter.IsCompleted" /> will return true when the Task is in one of the three
      /// final states: <see cref="F:System.Threading.Tasks.TaskStatus.RanToCompletion">RanToCompletion</see>,
      /// <see cref="F:System.Threading.Tasks.TaskStatus.Faulted">Faulted</see>, or
      /// <see cref="F:System.Threading.Tasks.TaskStatus.Canceled">Canceled</see>.
      /// </remarks>
      public bool IsCompleted
      {
        get
        {
          return this._task.IsCompleted;
        }
      }

      /// <summary>Ends the await on the completed <see cref="T:System.Threading.Tasks.Task" />.</summary>
      /// <exception cref="T:System.NullReferenceException">The awaiter was not properly initialized.</exception>
      /// <exception cref="T:System.Threading.Tasks.TaskCanceledException">The task was canceled.</exception>
      /// <exception cref="T:System.Exception">The task completed in a Faulted state.</exception>
      public void GetResult()
      {
        this._task.GetAwaiter().GetResult();
      }

      /// <summary>This method is not implemented and should not be called.</summary>
      /// <param name="continuation">The action to invoke when the await operation completes.</param>
      public void OnCompleted(Action continuation)
      {
        throw new NotImplementedException();
      }

      /// <summary>
      /// Schedules the continuation onto the <see cref="T:System.Threading.Tasks.Task" /> associated with this
      /// <see cref="T:System.Runtime.CompilerServices.TaskAwaiter" />.
      /// </summary>
      /// <param name="continuation">The action to invoke when the await operation completes.</param>
      /// <exception cref="T:System.ArgumentNullException">
      /// The <paramref name="continuation" /> argument is null
      /// (Nothing in Visual Basic).
      /// </exception>
      /// <exception cref="T:System.InvalidOperationException">The awaiter was not properly initialized.</exception>
      /// <remarks>This method is intended for compiler user rather than use directly in code.</remarks>
      public void UnsafeOnCompleted(Action continuation)
      {
        CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
        CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
        this._task.ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted((Action) (() =>
        {
          CultureInfo currentCulture1 = Thread.CurrentThread.CurrentCulture;
          CultureInfo currentUiCulture = Thread.CurrentThread.CurrentUICulture;
          Thread.CurrentThread.CurrentCulture = currentCulture;
          Thread.CurrentThread.CurrentUICulture = currentUICulture;
          try
          {
            continuation();
          }
          finally
          {
            Thread.CurrentThread.CurrentCulture = currentCulture1;
            Thread.CurrentThread.CurrentUICulture = currentUiCulture;
          }
        }));
      }
    }
  }
}
