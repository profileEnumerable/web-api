// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.ColumnVar
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal sealed class ColumnVar : Var
  {
    private readonly ColumnMD m_columnMetadata;
    private readonly Table m_table;

    internal ColumnVar(int id, Table table, ColumnMD columnMetadata)
      : base(id, VarType.Column, columnMetadata.Type)
    {
      this.m_table = table;
      this.m_columnMetadata = columnMetadata;
    }

    internal Table Table
    {
      get
      {
        return this.m_table;
      }
    }

    internal ColumnMD ColumnMetadata
    {
      get
      {
        return this.m_columnMetadata;
      }
    }

    internal override bool TryGetName(out string name)
    {
      name = this.m_columnMetadata.Name;
      return true;
    }
  }
}
