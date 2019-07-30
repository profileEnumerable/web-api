// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.SqlGen.JoinSymbol
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.SqlServer.SqlGen
{
  internal sealed class JoinSymbol : Symbol
  {
    private List<Symbol> columnList;
    private readonly List<Symbol> extentList;
    private List<Symbol> flattenedExtentList;
    private readonly Dictionary<string, Symbol> nameToExtent;

    internal List<Symbol> ColumnList
    {
      get
      {
        if (this.columnList == null)
          this.columnList = new List<Symbol>();
        return this.columnList;
      }
      set
      {
        this.columnList = value;
      }
    }

    internal List<Symbol> ExtentList
    {
      get
      {
        return this.extentList;
      }
    }

    internal List<Symbol> FlattenedExtentList
    {
      get
      {
        if (this.flattenedExtentList == null)
          this.flattenedExtentList = new List<Symbol>();
        return this.flattenedExtentList;
      }
      set
      {
        this.flattenedExtentList = value;
      }
    }

    internal Dictionary<string, Symbol> NameToExtent
    {
      get
      {
        return this.nameToExtent;
      }
    }

    internal bool IsNestedJoin { get; set; }

    public JoinSymbol(string name, TypeUsage type, List<Symbol> extents)
      : base(name, type)
    {
      this.extentList = new List<Symbol>(extents.Count);
      this.nameToExtent = new Dictionary<string, Symbol>(extents.Count, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (Symbol extent in extents)
      {
        this.nameToExtent[extent.Name] = extent;
        this.ExtentList.Add(extent);
      }
    }
  }
}
