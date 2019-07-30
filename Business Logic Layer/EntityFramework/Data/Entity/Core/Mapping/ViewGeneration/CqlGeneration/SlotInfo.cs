// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.CqlGeneration.SlotInfo
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.Structures;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.CqlGeneration
{
  internal sealed class SlotInfo : InternalBase
  {
    private bool m_isRequiredByParent;
    private readonly bool m_isProjected;
    private readonly ProjectedSlot m_slotValue;
    private readonly MemberPath m_outputMember;
    private readonly bool m_enforceNotNull;

    internal SlotInfo(
      bool isRequiredByParent,
      bool isProjected,
      ProjectedSlot slotValue,
      MemberPath outputMember)
      : this(isRequiredByParent, isProjected, slotValue, outputMember, false)
    {
    }

    internal SlotInfo(
      bool isRequiredByParent,
      bool isProjected,
      ProjectedSlot slotValue,
      MemberPath outputMember,
      bool enforceNotNull)
    {
      this.m_isRequiredByParent = isRequiredByParent;
      this.m_isProjected = isProjected;
      this.m_slotValue = slotValue;
      this.m_outputMember = outputMember;
      this.m_enforceNotNull = enforceNotNull;
    }

    internal bool IsRequiredByParent
    {
      get
      {
        return this.m_isRequiredByParent;
      }
    }

    internal bool IsProjected
    {
      get
      {
        return this.m_isProjected;
      }
    }

    internal MemberPath OutputMember
    {
      get
      {
        return this.m_outputMember;
      }
    }

    internal ProjectedSlot SlotValue
    {
      get
      {
        return this.m_slotValue;
      }
    }

    internal string CqlFieldAlias
    {
      get
      {
        if (this.m_slotValue == null)
          return (string) null;
        return this.m_slotValue.GetCqlFieldAlias(this.m_outputMember);
      }
    }

    internal bool IsEnforcedNotNull
    {
      get
      {
        return this.m_enforceNotNull;
      }
    }

    internal void ResetIsRequiredByParent()
    {
      this.m_isRequiredByParent = false;
    }

    internal StringBuilder AsEsql(
      StringBuilder builder,
      string blockAlias,
      int indentLevel)
    {
      if (this.m_enforceNotNull)
      {
        builder.Append('(');
        this.m_slotValue.AsEsql(builder, this.m_outputMember, blockAlias, indentLevel);
        builder.Append(" AND ");
        this.m_slotValue.AsEsql(builder, this.m_outputMember, blockAlias, indentLevel);
        builder.Append(" IS NOT NULL)");
      }
      else
        this.m_slotValue.AsEsql(builder, this.m_outputMember, blockAlias, indentLevel);
      return builder;
    }

    internal DbExpression AsCqt(DbExpression row)
    {
      DbExpression left = this.m_slotValue.AsCqt(row, this.m_outputMember);
      if (this.m_enforceNotNull)
        left = (DbExpression) left.And((DbExpression) left.IsNull().Not());
      return left;
    }

    internal override void ToCompactString(StringBuilder builder)
    {
      if (this.m_slotValue == null)
        return;
      builder.Append(this.CqlFieldAlias);
    }
  }
}
