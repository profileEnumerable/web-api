// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.ExtendedNodeInfo
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal class ExtendedNodeInfo : NodeInfo
  {
    private readonly VarVec m_localDefinitions;
    private readonly VarVec m_definitions;
    private readonly KeyVec m_keys;
    private readonly VarVec m_nonNullableDefinitions;
    private readonly VarVec m_nonNullableVisibleDefinitions;
    private RowCount m_minRows;
    private RowCount m_maxRows;

    internal ExtendedNodeInfo(Command cmd)
      : base(cmd)
    {
      this.m_localDefinitions = cmd.CreateVarVec();
      this.m_definitions = cmd.CreateVarVec();
      this.m_nonNullableDefinitions = cmd.CreateVarVec();
      this.m_nonNullableVisibleDefinitions = cmd.CreateVarVec();
      this.m_keys = new KeyVec(cmd);
      this.m_minRows = RowCount.Zero;
      this.m_maxRows = RowCount.Unbounded;
    }

    internal override void Clear()
    {
      base.Clear();
      this.m_definitions.Clear();
      this.m_localDefinitions.Clear();
      this.m_nonNullableDefinitions.Clear();
      this.m_nonNullableVisibleDefinitions.Clear();
      this.m_keys.Clear();
      this.m_minRows = RowCount.Zero;
      this.m_maxRows = RowCount.Unbounded;
    }

    internal override void ComputeHashValue(Command cmd, Node n)
    {
      base.ComputeHashValue(cmd, n);
      this.m_hashValue = this.m_hashValue << 4 ^ NodeInfo.GetHashValue(this.Definitions);
      this.m_hashValue = this.m_hashValue << 4 ^ NodeInfo.GetHashValue(this.Keys.KeyVars);
    }

    internal VarVec LocalDefinitions
    {
      get
      {
        return this.m_localDefinitions;
      }
    }

    internal VarVec Definitions
    {
      get
      {
        return this.m_definitions;
      }
    }

    internal KeyVec Keys
    {
      get
      {
        return this.m_keys;
      }
    }

    internal VarVec NonNullableDefinitions
    {
      get
      {
        return this.m_nonNullableDefinitions;
      }
    }

    internal VarVec NonNullableVisibleDefinitions
    {
      get
      {
        return this.m_nonNullableVisibleDefinitions;
      }
    }

    internal RowCount MinRows
    {
      get
      {
        return this.m_minRows;
      }
      set
      {
        this.m_minRows = value;
      }
    }

    internal RowCount MaxRows
    {
      get
      {
        return this.m_maxRows;
      }
      set
      {
        this.m_maxRows = value;
      }
    }

    internal void SetRowCount(RowCount minRows, RowCount maxRows)
    {
      this.m_minRows = minRows;
      this.m_maxRows = maxRows;
    }

    internal void InitRowCountFrom(ExtendedNodeInfo source)
    {
      this.m_minRows = source.m_minRows;
      this.m_maxRows = source.m_maxRows;
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [Conditional("DEBUG")]
    private void ValidateRowCount()
    {
    }
  }
}
