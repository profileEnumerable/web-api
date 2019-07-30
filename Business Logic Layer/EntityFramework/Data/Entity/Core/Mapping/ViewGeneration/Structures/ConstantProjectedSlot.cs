// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Structures.ConstantProjectedSlot
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Mapping.ViewGeneration.CqlGeneration;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Structures
{
  internal sealed class ConstantProjectedSlot : ProjectedSlot
  {
    private readonly Constant m_constant;

    internal ConstantProjectedSlot(Constant value)
    {
      this.m_constant = value;
    }

    internal Constant CellConstant
    {
      get
      {
        return this.m_constant;
      }
    }

    internal override ProjectedSlot DeepQualify(CqlBlock block)
    {
      return (ProjectedSlot) this;
    }

    internal override StringBuilder AsEsql(
      StringBuilder builder,
      MemberPath outputMember,
      string blockAlias,
      int indentLevel)
    {
      return this.m_constant.AsEsql(builder, outputMember, blockAlias);
    }

    internal override DbExpression AsCqt(DbExpression row, MemberPath outputMember)
    {
      return this.m_constant.AsCqt(row, outputMember);
    }

    protected override bool IsEqualTo(ProjectedSlot right)
    {
      ConstantProjectedSlot constantProjectedSlot = right as ConstantProjectedSlot;
      if (constantProjectedSlot == null)
        return false;
      return Constant.EqualityComparer.Equals(this.m_constant, constantProjectedSlot.m_constant);
    }

    protected override int GetHash()
    {
      return Constant.EqualityComparer.GetHashCode(this.m_constant);
    }

    internal override void ToCompactString(StringBuilder builder)
    {
      this.m_constant.ToCompactString(builder);
    }
  }
}
