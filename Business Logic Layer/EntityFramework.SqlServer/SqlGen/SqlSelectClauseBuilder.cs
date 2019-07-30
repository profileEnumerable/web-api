// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.SqlGen.SqlSelectClauseBuilder
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Collections.Generic;

namespace System.Data.Entity.SqlServer.SqlGen
{
  internal class SqlSelectClauseBuilder : SqlBuilder
  {
    private List<OptionalColumn> m_optionalColumns;
    private TopClause m_top;
    private SkipClause m_skip;
    private readonly Func<bool> m_isPartOfTopMostStatement;

    internal void AddOptionalColumn(OptionalColumn column)
    {
      if (this.m_optionalColumns == null)
        this.m_optionalColumns = new List<OptionalColumn>();
      this.m_optionalColumns.Add(column);
    }

    internal TopClause Top
    {
      get
      {
        return this.m_top;
      }
      set
      {
        this.m_top = value;
      }
    }

    internal SkipClause Skip
    {
      get
      {
        return this.m_skip;
      }
      set
      {
        this.m_skip = value;
      }
    }

    internal bool IsDistinct { get; set; }

    public override bool IsEmpty
    {
      get
      {
        if (!base.IsEmpty)
          return false;
        if (this.m_optionalColumns != null)
          return this.m_optionalColumns.Count == 0;
        return true;
      }
    }

    internal SqlSelectClauseBuilder(Func<bool> isPartOfTopMostStatement)
    {
      this.m_isPartOfTopMostStatement = isPartOfTopMostStatement;
    }

    public override void WriteSql(SqlWriter writer, SqlGenerator sqlGenerator)
    {
      writer.Write("SELECT ");
      if (this.IsDistinct)
        writer.Write("DISTINCT ");
      if (this.Top != null && this.Skip == null)
        this.Top.WriteSql(writer, sqlGenerator);
      if (this.IsEmpty)
      {
        writer.Write("*");
      }
      else
      {
        bool flag = this.WriteOptionalColumns(writer, sqlGenerator);
        if (!base.IsEmpty)
        {
          if (flag)
            writer.Write(", ");
          base.WriteSql(writer, sqlGenerator);
        }
        else
        {
          if (flag)
            return;
          this.m_optionalColumns[0].MarkAsUsed();
          this.m_optionalColumns[0].WriteSqlIfUsed(writer, sqlGenerator, "");
        }
      }
    }

    private bool WriteOptionalColumns(SqlWriter writer, SqlGenerator sqlGenerator)
    {
      if (this.m_optionalColumns == null)
        return false;
      if (this.m_isPartOfTopMostStatement() || this.IsDistinct)
      {
        foreach (OptionalColumn optionalColumn in this.m_optionalColumns)
          optionalColumn.MarkAsUsed();
      }
      string separator = "";
      bool flag = false;
      foreach (OptionalColumn optionalColumn in this.m_optionalColumns)
      {
        if (optionalColumn.WriteSqlIfUsed(writer, sqlGenerator, separator))
        {
          flag = true;
          separator = ", ";
        }
      }
      return flag;
    }
  }
}
