// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.InterceptableDbCommand
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity.Internal
{
  internal sealed class InterceptableDbCommand : DbCommand
  {
    private readonly DbCommand _command;
    private readonly DbInterceptionContext _interceptionContext;
    private readonly DbDispatchers _dispatchers;

    public InterceptableDbCommand(
      DbCommand command,
      DbInterceptionContext context,
      DbDispatchers dispatchers = null)
    {
      GC.SuppressFinalize((object) this);
      this._command = command;
      this._interceptionContext = context;
      this._dispatchers = dispatchers ?? DbInterception.Dispatch;
    }

    public DbInterceptionContext InterceptionContext
    {
      get
      {
        return this._interceptionContext;
      }
    }

    public override void Prepare()
    {
      this._command.Prepare();
    }

    [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
    public override string CommandText
    {
      get
      {
        return this._command.CommandText;
      }
      set
      {
        this._command.CommandText = value;
      }
    }

    public override int CommandTimeout
    {
      get
      {
        return this._command.CommandTimeout;
      }
      set
      {
        this._command.CommandTimeout = value;
      }
    }

    public override CommandType CommandType
    {
      get
      {
        return this._command.CommandType;
      }
      set
      {
        this._command.CommandType = value;
      }
    }

    public override UpdateRowSource UpdatedRowSource
    {
      get
      {
        return this._command.UpdatedRowSource;
      }
      set
      {
        this._command.UpdatedRowSource = value;
      }
    }

    protected override DbConnection DbConnection
    {
      get
      {
        return this._command.Connection;
      }
      set
      {
        this._command.Connection = value;
      }
    }

    protected override DbParameterCollection DbParameterCollection
    {
      get
      {
        return this._command.Parameters;
      }
    }

    protected override DbTransaction DbTransaction
    {
      get
      {
        return this._command.Transaction;
      }
      set
      {
        this._command.Transaction = value;
      }
    }

    public override bool DesignTimeVisible
    {
      get
      {
        return this._command.DesignTimeVisible;
      }
      set
      {
        this._command.DesignTimeVisible = value;
      }
    }

    public override void Cancel()
    {
      this._command.Cancel();
    }

    protected override DbParameter CreateDbParameter()
    {
      return this._command.CreateParameter();
    }

    public override int ExecuteNonQuery()
    {
      if (!this._dispatchers.CancelableCommand.Executing(this._command, this._interceptionContext))
        return 1;
      return this._dispatchers.Command.NonQuery(this._command, new DbCommandInterceptionContext(this._interceptionContext));
    }

    public override object ExecuteScalar()
    {
      if (!this._dispatchers.CancelableCommand.Executing(this._command, this._interceptionContext))
        return (object) null;
      return this._dispatchers.Command.Scalar(this._command, new DbCommandInterceptionContext(this._interceptionContext));
    }

    protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
    {
      if (!this._dispatchers.CancelableCommand.Executing(this._command, this._interceptionContext))
        return (DbDataReader) new InterceptableDbCommand.NullDataReader();
      DbCommandInterceptionContext interceptionContext = new DbCommandInterceptionContext(this._interceptionContext);
      if (behavior != CommandBehavior.Default)
        interceptionContext = interceptionContext.WithCommandBehavior(behavior);
      return this._dispatchers.Command.Reader(this._command, interceptionContext);
    }

    public override Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      if (!this._dispatchers.CancelableCommand.Executing(this._command, this._interceptionContext))
        return new Task<int>((Func<int>) (() => 1));
      return this._dispatchers.Command.NonQueryAsync(this._command, new DbCommandInterceptionContext(this._interceptionContext), cancellationToken);
    }

    public override Task<object> ExecuteScalarAsync(CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      if (!this._dispatchers.CancelableCommand.Executing(this._command, this._interceptionContext))
        return new Task<object>((Func<object>) (() => (object) null));
      return this._dispatchers.Command.ScalarAsync(this._command, new DbCommandInterceptionContext(this._interceptionContext), cancellationToken);
    }

    protected override Task<DbDataReader> ExecuteDbDataReaderAsync(
      CommandBehavior behavior,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      if (!this._dispatchers.CancelableCommand.Executing(this._command, this._interceptionContext))
        return new Task<DbDataReader>((Func<DbDataReader>) (() => (DbDataReader) new InterceptableDbCommand.NullDataReader()));
      DbCommandInterceptionContext interceptionContext = new DbCommandInterceptionContext(this._interceptionContext);
      if (behavior != CommandBehavior.Default)
        interceptionContext = interceptionContext.WithCommandBehavior(behavior);
      return this._dispatchers.Command.ReaderAsync(this._command, interceptionContext, cancellationToken);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this._command != null)
        this._command.Dispose();
      base.Dispose(disposing);
    }

    private class NullDataReader : DbDataReader
    {
      private int _resultCount;
      private int _readCount;

      public override void Close()
      {
      }

      public override bool NextResult()
      {
        return this._resultCount++ == 0;
      }

      public override bool Read()
      {
        return this._readCount++ == 0;
      }

      public override bool IsClosed
      {
        get
        {
          return false;
        }
      }

      public override int FieldCount
      {
        get
        {
          return 0;
        }
      }

      public override int GetOrdinal(string name)
      {
        return -1;
      }

      public override object GetValue(int ordinal)
      {
        throw new NotImplementedException();
      }

      public override DataTable GetSchemaTable()
      {
        throw new NotImplementedException();
      }

      public override int Depth
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public override int RecordsAffected
      {
        get
        {
          return 0;
        }
      }

      public override bool GetBoolean(int ordinal)
      {
        throw new NotImplementedException();
      }

      public override byte GetByte(int ordinal)
      {
        throw new NotImplementedException();
      }

      public override long GetBytes(
        int ordinal,
        long dataOffset,
        byte[] buffer,
        int bufferOffset,
        int length)
      {
        throw new NotImplementedException();
      }

      public override char GetChar(int ordinal)
      {
        throw new NotImplementedException();
      }

      public override long GetChars(
        int ordinal,
        long dataOffset,
        char[] buffer,
        int bufferOffset,
        int length)
      {
        throw new NotImplementedException();
      }

      public override Guid GetGuid(int ordinal)
      {
        throw new NotImplementedException();
      }

      public override short GetInt16(int ordinal)
      {
        throw new NotImplementedException();
      }

      public override int GetInt32(int ordinal)
      {
        throw new NotImplementedException();
      }

      public override long GetInt64(int ordinal)
      {
        throw new NotImplementedException();
      }

      public override DateTime GetDateTime(int ordinal)
      {
        throw new NotImplementedException();
      }

      public override string GetString(int ordinal)
      {
        throw new NotImplementedException();
      }

      public override Decimal GetDecimal(int ordinal)
      {
        throw new NotImplementedException();
      }

      public override double GetDouble(int ordinal)
      {
        throw new NotImplementedException();
      }

      public override float GetFloat(int ordinal)
      {
        throw new NotImplementedException();
      }

      public override string GetName(int ordinal)
      {
        throw new NotImplementedException();
      }

      public override int GetValues(object[] values)
      {
        return 0;
      }

      public override bool IsDBNull(int ordinal)
      {
        return true;
      }

      public override object this[int ordinal]
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public override object this[string name]
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public override bool HasRows
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public override string GetDataTypeName(int ordinal)
      {
        throw new NotImplementedException();
      }

      public override Type GetFieldType(int ordinal)
      {
        throw new NotImplementedException();
      }

      public override IEnumerator GetEnumerator()
      {
        throw new NotImplementedException();
      }
    }
  }
}
