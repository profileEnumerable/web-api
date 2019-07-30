// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Structures.TypeConstant
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Mapping.ViewGeneration.CqlGeneration;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Structures
{
  internal sealed class TypeConstant : Constant
  {
    private readonly EdmType m_edmType;

    internal TypeConstant(EdmType type)
    {
      this.m_edmType = type;
    }

    internal EdmType EdmType
    {
      get
      {
        return this.m_edmType;
      }
    }

    internal override bool IsNull()
    {
      return false;
    }

    internal override bool IsNotNull()
    {
      return false;
    }

    internal override bool IsUndefined()
    {
      return false;
    }

    internal override bool HasNotNull()
    {
      return false;
    }

    protected override bool IsEqualTo(Constant right)
    {
      TypeConstant typeConstant = right as TypeConstant;
      if (typeConstant == null)
        return false;
      return this.m_edmType == typeConstant.m_edmType;
    }

    public override int GetHashCode()
    {
      if (this.m_edmType == null)
        return 0;
      return this.m_edmType.GetHashCode();
    }

    internal override StringBuilder AsEsql(
      StringBuilder builder,
      MemberPath outputMember,
      string blockAlias)
    {
      this.AsCql((Action<EntitySet, IList<MemberPath>>) ((refScopeEntitySet, keyMemberOutputPaths) =>
      {
        EntityType elementType = (EntityType) ((RefType) outputMember.EdmType).ElementType;
        builder.Append("CreateRef(");
        CqlWriter.AppendEscapedQualifiedName(builder, refScopeEntitySet.EntityContainer.Name, refScopeEntitySet.Name);
        builder.Append(", row(");
        for (int index = 0; index < keyMemberOutputPaths.Count; ++index)
        {
          if (index > 0)
            builder.Append(", ");
          builder.Append(CqlWriter.GetQualifiedName(blockAlias, keyMemberOutputPaths[index].CqlFieldAlias));
        }
        builder.Append("), ");
        CqlWriter.AppendEscapedTypeName(builder, (EdmType) elementType);
        builder.Append(')');
      }), (Action<IList<MemberPath>>) (membersOutputPaths =>
      {
        CqlWriter.AppendEscapedTypeName(builder, this.m_edmType);
        builder.Append('(');
        for (int index = 0; index < membersOutputPaths.Count; ++index)
        {
          if (index > 0)
            builder.Append(", ");
          builder.Append(CqlWriter.GetQualifiedName(blockAlias, membersOutputPaths[index].CqlFieldAlias));
        }
        builder.Append(')');
      }), outputMember);
      return builder;
    }

    internal override DbExpression AsCqt(DbExpression row, MemberPath outputMember)
    {
      DbExpression cqt = (DbExpression) null;
      this.AsCql((Action<EntitySet, IList<MemberPath>>) ((refScopeEntitySet, keyMemberOutputPaths) =>
      {
        EntityType elementType = (EntityType) ((RefType) outputMember.EdmType).ElementType;
        cqt = (DbExpression) refScopeEntitySet.CreateRef(elementType, (IEnumerable<DbExpression>) keyMemberOutputPaths.Select<MemberPath, DbPropertyExpression>((Func<MemberPath, DbPropertyExpression>) (km => row.Property(km.CqlFieldAlias))));
      }), (Action<IList<MemberPath>>) (membersOutputPaths => cqt = (DbExpression) TypeUsage.Create(this.m_edmType).New((IEnumerable<DbExpression>) membersOutputPaths.Select<MemberPath, DbPropertyExpression>((Func<MemberPath, DbPropertyExpression>) (m => row.Property(m.CqlFieldAlias))))), outputMember);
      return cqt;
    }

    private void AsCql(
      Action<EntitySet, IList<MemberPath>> createRef,
      Action<IList<MemberPath>> createType,
      MemberPath outputMember)
    {
      EntitySet scopeOfRelationEnd = outputMember.GetScopeOfRelationEnd();
      if (scopeOfRelationEnd != null)
      {
        List<MemberPath> memberPathList = new List<MemberPath>(scopeOfRelationEnd.ElementType.KeyMembers.Select<EdmMember, MemberPath>((Func<EdmMember, MemberPath>) (km => new MemberPath(outputMember, km))));
        createRef(scopeOfRelationEnd, (IList<MemberPath>) memberPathList);
      }
      else
      {
        List<MemberPath> memberPathList = new List<MemberPath>();
        foreach (EdmMember structuralMember in (IEnumerable) Helper.GetAllStructuralMembers(this.m_edmType))
          memberPathList.Add(new MemberPath(outputMember, structuralMember));
        createType((IList<MemberPath>) memberPathList);
      }
    }

    internal override string ToUserString()
    {
      StringBuilder builder = new StringBuilder();
      this.ToCompactString(builder);
      return builder.ToString();
    }

    internal override void ToCompactString(StringBuilder builder)
    {
      builder.Append(this.m_edmType.Name);
    }
  }
}
