// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Structures.QualifiedCellIdBoolean
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Mapping.ViewGeneration.CqlGeneration;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Structures
{
  internal sealed class QualifiedCellIdBoolean : CellIdBoolean
  {
    private readonly CqlBlock m_block;

    internal QualifiedCellIdBoolean(
      CqlBlock block,
      CqlIdentifiers identifiers,
      int originalCellNum)
      : base(identifiers, originalCellNum)
    {
      this.m_block = block;
    }

    internal override StringBuilder AsEsql(
      StringBuilder builder,
      string blockAlias,
      bool skipIsNotNull)
    {
      return base.AsEsql(builder, this.m_block.CqlAlias, skipIsNotNull);
    }

    internal override DbExpression AsCqt(DbExpression row, bool skipIsNotNull)
    {
      return base.AsCqt(this.m_block.GetInput(row), skipIsNotNull);
    }
  }
}
