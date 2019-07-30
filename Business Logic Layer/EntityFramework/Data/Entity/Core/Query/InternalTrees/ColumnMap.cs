// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.ColumnMap
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal abstract class ColumnMap
  {
    internal const string DefaultColumnName = "Value";
    private TypeUsage _type;
    private string _name;

    internal ColumnMap(TypeUsage type, string name)
    {
      this._type = type;
      this._name = name;
    }

    internal TypeUsage Type
    {
      get
      {
        return this._type;
      }
      set
      {
        this._type = value;
      }
    }

    internal string Name
    {
      get
      {
        return this._name;
      }
      set
      {
        this._name = value;
      }
    }

    internal bool IsNamed
    {
      get
      {
        return this._name != null;
      }
    }

    [DebuggerNonUserCode]
    internal abstract void Accept<TArgType>(ColumnMapVisitor<TArgType> visitor, TArgType arg);

    [DebuggerNonUserCode]
    internal abstract TResultType Accept<TResultType, TArgType>(
      ColumnMapVisitorWithResults<TResultType, TArgType> visitor,
      TArgType arg);
  }
}
