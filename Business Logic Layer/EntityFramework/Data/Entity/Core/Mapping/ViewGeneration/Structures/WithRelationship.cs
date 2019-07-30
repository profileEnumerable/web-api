// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Structures.WithRelationship
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.CqlGeneration;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Structures
{
  internal sealed class WithRelationship : InternalBase
  {
    private readonly AssociationSet m_associationSet;
    private readonly RelationshipEndMember m_fromEnd;
    private readonly EntityType m_fromEndEntityType;
    private readonly RelationshipEndMember m_toEnd;
    private readonly EntityType m_toEndEntityType;
    private readonly EntitySet m_toEndEntitySet;
    private readonly IEnumerable<MemberPath> m_toEndEntityKeyMemberPaths;

    internal WithRelationship(
      AssociationSet associationSet,
      AssociationEndMember fromEnd,
      EntityType fromEndEntityType,
      AssociationEndMember toEnd,
      EntityType toEndEntityType,
      IEnumerable<MemberPath> toEndEntityKeyMemberPaths)
    {
      this.m_associationSet = associationSet;
      this.m_fromEnd = (RelationshipEndMember) fromEnd;
      this.m_fromEndEntityType = fromEndEntityType;
      this.m_toEnd = (RelationshipEndMember) toEnd;
      this.m_toEndEntityType = toEndEntityType;
      this.m_toEndEntitySet = MetadataHelper.GetEntitySetAtEnd(associationSet, toEnd);
      this.m_toEndEntityKeyMemberPaths = toEndEntityKeyMemberPaths;
    }

    internal EntityType FromEndEntityType
    {
      get
      {
        return this.m_fromEndEntityType;
      }
    }

    internal StringBuilder AsEsql(
      StringBuilder builder,
      string blockAlias,
      int indentLevel)
    {
      StringUtil.IndentNewLine(builder, indentLevel + 1);
      builder.Append("RELATIONSHIP(");
      List<string> stringList = new List<string>();
      builder.Append("CREATEREF(");
      CqlWriter.AppendEscapedQualifiedName(builder, this.m_toEndEntitySet.EntityContainer.Name, this.m_toEndEntitySet.Name);
      builder.Append(", ROW(");
      foreach (MemberPath entityKeyMemberPath in this.m_toEndEntityKeyMemberPaths)
      {
        string qualifiedName = CqlWriter.GetQualifiedName(blockAlias, entityKeyMemberPath.CqlFieldAlias);
        stringList.Add(qualifiedName);
      }
      StringUtil.ToSeparatedString(builder, (IEnumerable) stringList, ", ", (string) null);
      builder.Append(')');
      builder.Append(",");
      CqlWriter.AppendEscapedTypeName(builder, (EdmType) this.m_toEndEntityType);
      builder.Append(')');
      builder.Append(',');
      CqlWriter.AppendEscapedTypeName(builder, (EdmType) this.m_associationSet.ElementType);
      builder.Append(',');
      CqlWriter.AppendEscapedName(builder, this.m_fromEnd.Name);
      builder.Append(',');
      CqlWriter.AppendEscapedName(builder, this.m_toEnd.Name);
      builder.Append(')');
      builder.Append(' ');
      return builder;
    }

    internal DbRelatedEntityRef AsCqt(DbExpression row)
    {
      return DbExpressionBuilder.CreateRelatedEntityRef(this.m_fromEnd, this.m_toEnd, (DbExpression) this.m_toEndEntitySet.CreateRef(this.m_toEndEntityType, (IEnumerable<DbExpression>) this.m_toEndEntityKeyMemberPaths.Select<MemberPath, DbPropertyExpression>((Func<MemberPath, DbPropertyExpression>) (keyMember => row.Property(keyMember.CqlFieldAlias)))));
    }

    internal override void ToCompactString(StringBuilder builder)
    {
    }
  }
}
