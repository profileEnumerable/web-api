// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.SqlGen.SqlSelectStatement
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Collections.Generic;
using System.Globalization;

namespace System.Data.Entity.SqlServer.SqlGen
{
  internal sealed class SqlSelectStatement : ISqlFragment
  {
    private readonly SqlBuilder from = new SqlBuilder();
    private List<Symbol> fromExtents;
    private Dictionary<Symbol, bool> outerExtents;
    private readonly SqlSelectClauseBuilder select;
    private SqlBuilder where;
    private SqlBuilder groupBy;
    private SqlBuilder orderBy;

    internal bool OutputColumnsRenamed { get; set; }

    internal Dictionary<string, Symbol> OutputColumns { get; set; }

    internal List<Symbol> AllJoinExtents { get; set; }

    internal List<Symbol> FromExtents
    {
      get
      {
        if (this.fromExtents == null)
          this.fromExtents = new List<Symbol>();
        return this.fromExtents;
      }
    }

    internal Dictionary<Symbol, bool> OuterExtents
    {
      get
      {
        if (this.outerExtents == null)
          this.outerExtents = new Dictionary<Symbol, bool>();
        return this.outerExtents;
      }
    }

    internal SqlSelectClauseBuilder Select
    {
      get
      {
        return this.select;
      }
    }

    internal SqlBuilder From
    {
      get
      {
        return this.from;
      }
    }

    internal SqlBuilder Where
    {
      get
      {
        if (this.where == null)
          this.where = new SqlBuilder();
        return this.where;
      }
    }

    internal SqlBuilder GroupBy
    {
      get
      {
        if (this.groupBy == null)
          this.groupBy = new SqlBuilder();
        return this.groupBy;
      }
    }

    public SqlBuilder OrderBy
    {
      get
      {
        if (this.orderBy == null)
          this.orderBy = new SqlBuilder();
        return this.orderBy;
      }
    }

    internal bool IsTopMost { get; set; }

    internal SqlSelectStatement()
    {
      this.select = new SqlSelectClauseBuilder((Func<bool>) (() => this.IsTopMost));
    }

    public void WriteSql(SqlWriter writer, SqlGenerator sqlGenerator)
    {
      List<string> stringList = (List<string>) null;
      if (this.outerExtents != null && 0 < this.outerExtents.Count)
      {
        foreach (Symbol key in this.outerExtents.Keys)
        {
          JoinSymbol joinSymbol = key as JoinSymbol;
          if (joinSymbol != null)
          {
            foreach (Symbol flattenedExtent in joinSymbol.FlattenedExtentList)
            {
              if (stringList == null)
                stringList = new List<string>();
              stringList.Add(flattenedExtent.NewName);
            }
          }
          else
          {
            if (stringList == null)
              stringList = new List<string>();
            stringList.Add(key.NewName);
          }
        }
      }
      List<Symbol> symbolList = this.AllJoinExtents ?? this.fromExtents;
      if (symbolList != null)
      {
        foreach (Symbol symbol in symbolList)
        {
          if (stringList != null && stringList.Contains(symbol.Name))
          {
            int allExtentName = sqlGenerator.AllExtentNames[symbol.Name];
            string key;
            do
            {
              ++allExtentName;
              key = symbol.Name + allExtentName.ToString((IFormatProvider) CultureInfo.InvariantCulture);
            }
            while (sqlGenerator.AllExtentNames.ContainsKey(key));
            sqlGenerator.AllExtentNames[symbol.Name] = allExtentName;
            symbol.NewName = key;
            sqlGenerator.AllExtentNames[key] = 0;
          }
          if (stringList == null)
            stringList = new List<string>();
          stringList.Add(symbol.NewName);
        }
      }
      ++writer.Indent;
      this.select.WriteSql(writer, sqlGenerator);
      writer.WriteLine();
      writer.Write("FROM ");
      this.From.WriteSql(writer, sqlGenerator);
      if (this.where != null && !this.Where.IsEmpty)
      {
        writer.WriteLine();
        writer.Write("WHERE ");
        this.Where.WriteSql(writer, sqlGenerator);
      }
      if (this.groupBy != null && !this.GroupBy.IsEmpty)
      {
        writer.WriteLine();
        writer.Write("GROUP BY ");
        this.GroupBy.WriteSql(writer, sqlGenerator);
      }
      if (this.orderBy != null && !this.OrderBy.IsEmpty && (this.IsTopMost || this.Select.Top != null || this.Select.Skip != null))
      {
        writer.WriteLine();
        writer.Write("ORDER BY ");
        this.OrderBy.WriteSql(writer, sqlGenerator);
      }
      if (this.Select.Skip != null)
      {
        writer.WriteLine();
        SqlSelectStatement.WriteOffsetFetch(writer, this.Select.Top, this.Select.Skip, sqlGenerator);
      }
      --writer.Indent;
    }

    private static void WriteOffsetFetch(
      SqlWriter writer,
      TopClause top,
      SkipClause skip,
      SqlGenerator sqlGenerator)
    {
      skip.WriteSql(writer, sqlGenerator);
      if (top == null)
        return;
      writer.Write("FETCH NEXT ");
      top.TopCount.WriteSql(writer, sqlGenerator);
      writer.Write(" ROWS ONLY ");
    }
  }
}
