// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Structures.MemberPath
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

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
  internal sealed class MemberPath : InternalBase, IEquatable<MemberPath>
  {
    internal static readonly IEqualityComparer<MemberPath> EqualityComparer = (IEqualityComparer<MemberPath>) new MemberPath.Comparer();
    private readonly EntitySetBase m_extent;
    private readonly List<EdmMember> m_path;

    internal MemberPath(EntitySetBase extent, IEnumerable<EdmMember> path)
    {
      this.m_extent = extent;
      this.m_path = path.ToList<EdmMember>();
    }

    internal MemberPath(EntitySetBase extent)
      : this(extent, Enumerable.Empty<EdmMember>())
    {
    }

    internal MemberPath(EntitySetBase extent, EdmMember member)
      : this(extent, Enumerable.Repeat<EdmMember>(member, 1))
    {
    }

    internal MemberPath(MemberPath prefix, EdmMember last)
    {
      this.m_extent = prefix.m_extent;
      this.m_path = new List<EdmMember>((IEnumerable<EdmMember>) prefix.m_path);
      this.m_path.Add(last);
    }

    internal EdmMember RootEdmMember
    {
      get
      {
        if (this.m_path.Count <= 0)
          return (EdmMember) null;
        return this.m_path[0];
      }
    }

    internal EdmMember LeafEdmMember
    {
      get
      {
        if (this.m_path.Count <= 0)
          return (EdmMember) null;
        return this.m_path[this.m_path.Count - 1];
      }
    }

    internal string LeafName
    {
      get
      {
        if (this.m_path.Count == 0)
          return this.m_extent.Name;
        return this.LeafEdmMember.Name;
      }
    }

    internal bool IsComputed
    {
      get
      {
        if (this.m_path.Count == 0)
          return false;
        return this.RootEdmMember.IsStoreGeneratedComputed;
      }
    }

    internal object DefaultValue
    {
      get
      {
        if (this.m_path.Count == 0)
          return (object) null;
        Facet facet;
        if (this.LeafEdmMember.TypeUsage.Facets.TryGetValue(nameof (DefaultValue), false, out facet))
          return facet.Value;
        return (object) null;
      }
    }

    internal bool IsPartOfKey
    {
      get
      {
        if (this.m_path.Count == 0)
          return false;
        return MetadataHelper.IsPartOfEntityTypeKey(this.LeafEdmMember);
      }
    }

    internal bool IsNullable
    {
      get
      {
        if (this.m_path.Count == 0)
          return false;
        return MetadataHelper.IsMemberNullable(this.LeafEdmMember);
      }
    }

    internal EntitySet EntitySet
    {
      get
      {
        if (this.m_path.Count == 0)
          return this.m_extent as EntitySet;
        if (this.m_path.Count == 1)
        {
          AssociationEndMember rootEdmMember = this.RootEdmMember as AssociationEndMember;
          if (rootEdmMember != null)
            return MetadataHelper.GetEntitySetAtEnd((AssociationSet) this.m_extent, rootEdmMember);
        }
        return (EntitySet) null;
      }
    }

    internal EntitySetBase Extent
    {
      get
      {
        return this.m_extent;
      }
    }

    internal EdmType EdmType
    {
      get
      {
        if (this.m_path.Count > 0)
          return this.LeafEdmMember.TypeUsage.EdmType;
        return (EdmType) this.m_extent.ElementType;
      }
    }

    internal string CqlFieldAlias
    {
      get
      {
        string name = this.PathToString(new bool?(true));
        if (!name.Contains("_"))
          name = name.Replace('.', '_');
        StringBuilder builder = new StringBuilder();
        CqlWriter.AppendEscapedName(builder, name);
        return builder.ToString();
      }
    }

    internal bool IsAlwaysDefined(
      Dictionary<EntityType, Set<EntityType>> inheritanceGraph)
    {
      if (this.m_path.Count == 0)
        return true;
      EdmMember member = this.m_path.Last<EdmMember>();
      for (int index = 0; index < this.m_path.Count - 1; ++index)
      {
        if (MetadataHelper.IsMemberNullable(this.m_path[index]))
          return false;
      }
      if (this.m_path[0].DeclaringType is AssociationType)
        return true;
      EntityType elementType = this.m_extent.ElementType as EntityType;
      if (elementType == null)
        return true;
      EntityType declaringType = this.m_path[0].DeclaringType as EntityType;
      EntityType baseType = declaringType.BaseType as EntityType;
      if (elementType.EdmEquals((MetadataItem) declaringType) || MetadataHelper.IsParentOf(declaringType, elementType) || baseType == null)
        return true;
      if (!baseType.Abstract && !MetadataHelper.DoesMemberExist((StructuralType) baseType, member))
        return false;
      return !MemberPath.RecurseToFindMemberAbsentInConcreteType(baseType, declaringType, member, elementType, inheritanceGraph);
    }

    private static bool RecurseToFindMemberAbsentInConcreteType(
      EntityType current,
      EntityType avoidEdge,
      EdmMember member,
      EntityType entitySetType,
      Dictionary<EntityType, Set<EntityType>> inheritanceGraph)
    {
      foreach (EntityType current1 in inheritanceGraph[current].Where<EntityType>((Func<EntityType, bool>) (type => !type.EdmEquals((MetadataItem) avoidEdge))))
      {
        if ((entitySetType.BaseType == null || !entitySetType.BaseType.EdmEquals((MetadataItem) current1)) && (!current1.Abstract && !MetadataHelper.DoesMemberExist((StructuralType) current1, member) || MemberPath.RecurseToFindMemberAbsentInConcreteType(current1, current, member, entitySetType, inheritanceGraph)))
          return true;
      }
      return false;
    }

    internal void GetIdentifiers(CqlIdentifiers identifiers)
    {
      identifiers.AddIdentifier(this.m_extent.Name);
      identifiers.AddIdentifier(this.m_extent.ElementType.Name);
      foreach (EdmMember edmMember in this.m_path)
        identifiers.AddIdentifier(edmMember.Name);
    }

    internal static bool AreAllMembersNullable(IEnumerable<MemberPath> members)
    {
      foreach (MemberPath member in members)
      {
        if (member.m_path.Count == 0 || !member.IsNullable)
          return false;
      }
      return true;
    }

    internal static string PropertiesToUserString(IEnumerable<MemberPath> members, bool fullPath)
    {
      bool flag = true;
      StringBuilder stringBuilder = new StringBuilder();
      foreach (MemberPath member in members)
      {
        if (!flag)
          stringBuilder.Append(", ");
        flag = false;
        if (fullPath)
          stringBuilder.Append(member.PathToString(new bool?(false)));
        else
          stringBuilder.Append(member.LeafName);
      }
      return stringBuilder.ToString();
    }

    internal StringBuilder AsEsql(StringBuilder inputBuilder, string blockAlias)
    {
      StringBuilder builder = new StringBuilder();
      CqlWriter.AppendEscapedName(builder, blockAlias);
      this.AsCql((Action<string>) (memberName =>
      {
        builder.Append('.');
        CqlWriter.AppendEscapedName(builder, memberName);
      }), (Action) (() =>
      {
        builder.Insert(0, "Key(");
        builder.Append(")");
      }), (Action<StructuralType>) (treatAsType =>
      {
        builder.Insert(0, "TREAT(");
        builder.Append(" AS ");
        CqlWriter.AppendEscapedTypeName(builder, (EdmType) treatAsType);
        builder.Append(')');
      }));
      inputBuilder.Append((object) builder);
      return inputBuilder;
    }

    internal DbExpression AsCqt(DbExpression row)
    {
      this.AsCql((Action<string>) (memberName => row = (DbExpression) row.Property(memberName)), (Action) (() => row = (DbExpression) row.GetRefKey()), (Action<StructuralType>) (treatAsType => row = (DbExpression) row.TreatAs(TypeUsage.Create((EdmType) treatAsType))));
      return row;
    }

    internal void AsCql(Action<string> accessMember, Action getKey, Action<StructuralType> treatAs)
    {
      EdmType edmType = (EdmType) this.m_extent.ElementType;
      foreach (EdmMember member in this.m_path)
      {
        RefType refType;
        StructuralType type;
        if (Helper.IsRefType((GlobalItem) edmType))
        {
          refType = (RefType) edmType;
          type = (StructuralType) refType.ElementType;
        }
        else
        {
          refType = (RefType) null;
          type = (StructuralType) edmType;
        }
        bool flag = MetadataHelper.DoesMemberExist(type, member);
        if (refType != null)
          getKey();
        else if (!flag)
          treatAs(member.DeclaringType);
        accessMember(member.Name);
        edmType = member.TypeUsage.EdmType;
      }
    }

    public bool Equals(MemberPath right)
    {
      return MemberPath.EqualityComparer.Equals(this, right);
    }

    public override bool Equals(object obj)
    {
      MemberPath right = obj as MemberPath;
      if (obj == null)
        return false;
      return this.Equals(right);
    }

    public override int GetHashCode()
    {
      return MemberPath.EqualityComparer.GetHashCode(this);
    }

    internal bool IsScalarType()
    {
      if (this.EdmType.BuiltInTypeKind != BuiltInTypeKind.PrimitiveType)
        return this.EdmType.BuiltInTypeKind == BuiltInTypeKind.EnumType;
      return true;
    }

    internal static IEnumerable<MemberPath> GetKeyMembers(
      EntitySetBase extent,
      MemberDomainMap domainMap)
    {
      MemberPath memberPath = new MemberPath(extent);
      return (IEnumerable<MemberPath>) new List<MemberPath>(memberPath.GetMembers((EdmType) memberPath.Extent.ElementType, new bool?(), new bool?(), new bool?(true), domainMap));
    }

    internal IEnumerable<MemberPath> GetMembers(
      EdmType edmType,
      bool? isScalar,
      bool? isConditional,
      bool? isPartOfKey,
      MemberDomainMap domainMap)
    {
      MemberPath currentPath = this;
      StructuralType structuralType = (StructuralType) edmType;
      foreach (EdmMember member1 in structuralType.Members)
      {
        if (member1 is AssociationEndMember)
        {
          foreach (MemberPath member2 in new MemberPath(currentPath, member1).GetMembers((EdmType) ((RefType) member1.TypeUsage.EdmType).ElementType, isScalar, isConditional, new bool?(true), domainMap))
            yield return member2;
        }
        bool isActuallyScalar = MetadataHelper.IsNonRefSimpleMember(member1);
        if (isScalar.HasValue)
        {
          bool? nullable = isScalar;
          bool flag = isActuallyScalar;
          if ((nullable.GetValueOrDefault() != flag ? 0 : (nullable.HasValue ? 1 : 0)) == 0)
            continue;
        }
        EdmProperty childProperty = member1 as EdmProperty;
        if (childProperty != null)
        {
          bool isActuallyKey = MetadataHelper.IsPartOfEntityTypeKey((EdmMember) childProperty);
          if (isPartOfKey.HasValue)
          {
            bool? nullable = isPartOfKey;
            bool flag = isActuallyKey;
            if ((nullable.GetValueOrDefault() != flag ? 0 : (nullable.HasValue ? 1 : 0)) == 0)
              continue;
          }
          MemberPath childPath = new MemberPath(currentPath, (EdmMember) childProperty);
          bool isActuallyConditional = domainMap.IsConditionMember(childPath);
          if (isConditional.HasValue)
          {
            bool? nullable = isConditional;
            bool flag = isActuallyConditional;
            if ((nullable.GetValueOrDefault() != flag ? 0 : (nullable.HasValue ? 1 : 0)) == 0)
              continue;
          }
          yield return childPath;
        }
      }
    }

    internal bool IsEquivalentViaRefConstraint(MemberPath path1)
    {
      MemberPath assocPath0_1 = this;
      if (assocPath0_1.EdmType is EntityTypeBase || path1.EdmType is EntityTypeBase || (!MetadataHelper.IsNonRefSimpleMember(assocPath0_1.LeafEdmMember) || !MetadataHelper.IsNonRefSimpleMember(path1.LeafEdmMember)))
        return false;
      AssociationSet extent1 = assocPath0_1.Extent as AssociationSet;
      AssociationSet extent2 = path1.Extent as AssociationSet;
      EntitySet extent3 = assocPath0_1.Extent as EntitySet;
      EntitySet extent4 = path1.Extent as EntitySet;
      bool flag = false;
      if (extent1 != null && extent2 != null)
      {
        if (!extent1.Equals((object) extent2))
          return false;
        flag = MemberPath.AreAssocationEndPathsEquivalentViaRefConstraint(assocPath0_1, path1, extent1);
      }
      else if (extent3 != null && extent4 != null)
      {
        foreach (AssociationSet associationsForEntitySet in MetadataHelper.GetAssociationsForEntitySets(extent3, extent4))
        {
          if (MemberPath.AreAssocationEndPathsEquivalentViaRefConstraint(assocPath0_1.GetCorrespondingAssociationPath(associationsForEntitySet), path1.GetCorrespondingAssociationPath(associationsForEntitySet), associationsForEntitySet))
          {
            flag = true;
            break;
          }
        }
      }
      else
      {
        AssociationSet assocSet = extent1 ?? extent2;
        MemberPath assocPath0_2 = assocPath0_1.Extent is AssociationSet ? assocPath0_1 : path1;
        MemberPath correspondingAssociationPath = (assocPath0_1.Extent is EntitySet ? assocPath0_1 : path1).GetCorrespondingAssociationPath(assocSet);
        flag = correspondingAssociationPath != null && MemberPath.AreAssocationEndPathsEquivalentViaRefConstraint(assocPath0_2, correspondingAssociationPath, assocSet);
      }
      return flag;
    }

    private static bool AreAssocationEndPathsEquivalentViaRefConstraint(
      MemberPath assocPath0,
      MemberPath assocPath1,
      AssociationSet assocSet)
    {
      AssociationEndMember rootEdmMember1 = assocPath0.RootEdmMember as AssociationEndMember;
      AssociationEndMember rootEdmMember2 = assocPath1.RootEdmMember as AssociationEndMember;
      EdmProperty leafEdmMember1 = assocPath0.LeafEdmMember as EdmProperty;
      EdmProperty leafEdmMember2 = assocPath1.LeafEdmMember as EdmProperty;
      if (rootEdmMember1 == null || rootEdmMember2 == null || (leafEdmMember1 == null || leafEdmMember2 == null))
        return false;
      AssociationType elementType = assocSet.ElementType;
      bool flag1 = false;
      foreach (ReferentialConstraint referentialConstraint in elementType.ReferentialConstraints)
      {
        bool flag2 = rootEdmMember1.Name == referentialConstraint.FromRole.Name && rootEdmMember2.Name == referentialConstraint.ToRole.Name;
        bool flag3 = rootEdmMember2.Name == referentialConstraint.FromRole.Name && rootEdmMember1.Name == referentialConstraint.ToRole.Name;
        if (flag2 || flag3)
        {
          ReadOnlyMetadataCollection<EdmProperty> metadataCollection1 = flag2 ? referentialConstraint.FromProperties : referentialConstraint.ToProperties;
          ReadOnlyMetadataCollection<EdmProperty> metadataCollection2 = flag2 ? referentialConstraint.ToProperties : referentialConstraint.FromProperties;
          int num1 = metadataCollection1.IndexOf(leafEdmMember1);
          int num2 = metadataCollection2.IndexOf(leafEdmMember2);
          if (num1 == num2 && num1 != -1)
          {
            flag1 = true;
            break;
          }
        }
      }
      return flag1;
    }

    private MemberPath GetCorrespondingAssociationPath(AssociationSet assocSet)
    {
      AssociationEndMember someEndForEntitySet = MetadataHelper.GetSomeEndForEntitySet(assocSet, this.m_extent);
      if (someEndForEntitySet == null)
        return (MemberPath) null;
      List<EdmMember> edmMemberList = new List<EdmMember>();
      edmMemberList.Add((EdmMember) someEndForEntitySet);
      edmMemberList.AddRange((IEnumerable<EdmMember>) this.m_path);
      return new MemberPath((EntitySetBase) assocSet, (IEnumerable<EdmMember>) edmMemberList);
    }

    internal EntitySet GetScopeOfRelationEnd()
    {
      if (this.m_path.Count == 0)
        return (EntitySet) null;
      AssociationEndMember leafEdmMember = this.LeafEdmMember as AssociationEndMember;
      if (leafEdmMember == null)
        return (EntitySet) null;
      return MetadataHelper.GetEntitySetAtEnd((AssociationSet) this.m_extent, leafEdmMember);
    }

    internal string PathToString(bool? forAlias)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (forAlias.HasValue)
      {
        bool? nullable = forAlias;
        if ((!nullable.GetValueOrDefault() ? 0 : (nullable.HasValue ? 1 : 0)) != 0)
        {
          if (this.m_path.Count == 0)
            return this.m_extent.ElementType.Name;
          stringBuilder.Append(this.m_path[0].DeclaringType.Name);
        }
        else
          stringBuilder.Append(this.m_extent.Name);
      }
      for (int index = 0; index < this.m_path.Count; ++index)
      {
        stringBuilder.Append('.');
        stringBuilder.Append(this.m_path[index].Name);
      }
      return stringBuilder.ToString();
    }

    internal override void ToCompactString(StringBuilder builder)
    {
      builder.Append(this.PathToString(new bool?(false)));
    }

    internal void ToCompactString(StringBuilder builder, string instanceToken)
    {
      builder.Append(instanceToken + this.PathToString(new bool?()));
    }

    private sealed class Comparer : IEqualityComparer<MemberPath>
    {
      public bool Equals(MemberPath left, MemberPath right)
      {
        if (object.ReferenceEquals((object) left, (object) right))
          return true;
        if (left == null || right == null || (!left.m_extent.Equals((object) right.m_extent) || left.m_path.Count != right.m_path.Count))
          return false;
        for (int index = 0; index < left.m_path.Count; ++index)
        {
          if (!left.m_path[index].Equals((object) right.m_path[index]))
            return false;
        }
        return true;
      }

      public int GetHashCode(MemberPath key)
      {
        int hashCode = key.m_extent.GetHashCode();
        foreach (EdmMember edmMember in key.m_path)
          hashCode ^= edmMember.GetHashCode();
        return hashCode;
      }
    }
  }
}
