// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.SqlGen.SkipClause
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Globalization;

namespace System.Data.Entity.SqlServer.SqlGen
{
  internal class SkipClause : ISqlFragment
  {
    private readonly ISqlFragment skipCount;

    internal ISqlFragment SkipCount
    {
      get
      {
        return this.skipCount;
      }
    }

    internal SkipClause(ISqlFragment skipCount)
    {
      this.skipCount = skipCount;
    }

    internal SkipClause(int skipCount)
    {
      SqlBuilder sqlBuilder = new SqlBuilder();
      sqlBuilder.Append((object) skipCount.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      this.skipCount = (ISqlFragment) sqlBuilder;
    }

    public void WriteSql(SqlWriter writer, SqlGenerator sqlGenerator)
    {
      writer.Write("OFFSET ");
      this.SkipCount.WriteSql(writer, sqlGenerator);
      writer.Write(" ROWS ");
    }
  }
}
