// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.CommandTracer
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Internal
{
  internal sealed class CommandTracer : ICancelableDbCommandInterceptor, IDbCommandTreeInterceptor, ICancelableEntityConnectionInterceptor, IDbInterceptor, IDisposable
  {
    private readonly List<DbCommand> _commands = new List<DbCommand>();
    private readonly List<DbCommandTree> _commandTrees = new List<DbCommandTree>();
    private readonly DbContext _context;
    private readonly DbDispatchers _dispatchers;

    public CommandTracer(DbContext context)
      : this(context, DbInterception.Dispatch)
    {
    }

    internal CommandTracer(DbContext context, DbDispatchers dispatchers)
    {
      this._context = context;
      this._dispatchers = dispatchers;
      this._dispatchers.AddInterceptor((IDbInterceptor) this);
    }

    public IEnumerable<DbCommand> DbCommands
    {
      get
      {
        return (IEnumerable<DbCommand>) this._commands;
      }
    }

    public IEnumerable<DbCommandTree> CommandTrees
    {
      get
      {
        return (IEnumerable<DbCommandTree>) this._commandTrees;
      }
    }

    public bool CommandExecuting(DbCommand command, DbInterceptionContext interceptionContext)
    {
      if (!interceptionContext.DbContexts.Contains<DbContext>(this._context, new Func<DbContext, DbContext, bool>(object.ReferenceEquals)))
        return true;
      this._commands.Add(command);
      return false;
    }

    public void TreeCreated(
      DbCommandTreeInterceptionContext interceptionContext)
    {
      if (!interceptionContext.DbContexts.Contains<DbContext>(this._context, new Func<DbContext, DbContext, bool>(object.ReferenceEquals)))
        return;
      this._commandTrees.Add(interceptionContext.Result);
    }

    public bool ConnectionOpening(
      EntityConnection connection,
      DbInterceptionContext interceptionContext)
    {
      return !interceptionContext.DbContexts.Contains<DbContext>(this._context, new Func<DbContext, DbContext, bool>(object.ReferenceEquals));
    }

    void IDisposable.Dispose()
    {
      this._dispatchers.RemoveInterceptor((IDbInterceptor) this);
    }
  }
}
