// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.SqlGen.SqlBuilder
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Collections.Generic;

namespace System.Data.Entity.SqlServer.SqlGen
{
  internal class SqlBuilder : ISqlFragment
  {
    private List<object> _sqlFragments;

    private List<object> sqlFragments
    {
      get
      {
        if (this._sqlFragments == null)
          this._sqlFragments = new List<object>();
        return this._sqlFragments;
      }
    }

    public void Append(object s)
    {
      this.sqlFragments.Add(s);
    }

    public void AppendLine()
    {
      this.sqlFragments.Add((object) "\r\n");
    }

    public virtual bool IsEmpty
    {
      get
      {
        if (this._sqlFragments != null)
          return 0 == this._sqlFragments.Count;
        return true;
      }
    }

    public virtual void WriteSql(SqlWriter writer, SqlGenerator sqlGenerator)
    {
      if (this._sqlFragments == null)
        return;
      foreach (object sqlFragment1 in this._sqlFragments)
      {
        string str = sqlFragment1 as string;
        if (str != null)
        {
          writer.Write(str);
        }
        else
        {
          ISqlFragment sqlFragment2 = sqlFragment1 as ISqlFragment;
          if (sqlFragment2 == null)
            throw new InvalidOperationException();
          sqlFragment2.WriteSql(writer, sqlGenerator);
        }
      }
    }
  }
}
