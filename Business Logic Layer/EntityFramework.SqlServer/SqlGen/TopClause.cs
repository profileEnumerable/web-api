// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.SqlGen.TopClause
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Globalization;

namespace System.Data.Entity.SqlServer.SqlGen
{
  internal class TopClause : ISqlFragment
  {
    private readonly ISqlFragment topCount;
    private readonly bool withTies;

    internal bool WithTies
    {
      get
      {
        return this.withTies;
      }
    }

    internal ISqlFragment TopCount
    {
      get
      {
        return this.topCount;
      }
    }

    internal TopClause(ISqlFragment topCount, bool withTies)
    {
      this.topCount = topCount;
      this.withTies = withTies;
    }

    internal TopClause(int topCount, bool withTies)
    {
      SqlBuilder sqlBuilder = new SqlBuilder();
      sqlBuilder.Append((object) topCount.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      this.topCount = (ISqlFragment) sqlBuilder;
      this.withTies = withTies;
    }

    public void WriteSql(SqlWriter writer, SqlGenerator sqlGenerator)
    {
      writer.Write("TOP ");
      if (sqlGenerator.SqlVersion != SqlVersion.Sql8)
        writer.Write("(");
      this.TopCount.WriteSql(writer, sqlGenerator);
      if (sqlGenerator.SqlVersion != SqlVersion.Sql8)
        writer.Write(")");
      writer.Write(" ");
      if (!this.WithTies)
        return;
      writer.Write("WITH TIES ");
    }
  }
}
