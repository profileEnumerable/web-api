// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting.FragmentQueryKB
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Common.Utils.Boolean;
using System.Data.Entity.Core.Mapping.ViewGeneration.Structures;
using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting
{
  internal class FragmentQueryKB : KnowledgeBase<DomainConstraint<BoolLiteral, Constant>>
  {
    private BoolExpr<DomainConstraint<BoolLiteral, Constant>> _kbExpression = (BoolExpr<DomainConstraint<BoolLiteral, Constant>>) TrueExpr<DomainConstraint<BoolLiteral, Constant>>.Value;

    internal override void AddFact(
      BoolExpr<DomainConstraint<BoolLiteral, Constant>> fact)
    {
      base.AddFact(fact);
      this._kbExpression = (BoolExpr<DomainConstraint<BoolLiteral, Constant>>) new AndExpr<DomainConstraint<BoolLiteral, Constant>>(new BoolExpr<DomainConstraint<BoolLiteral, Constant>>[2]
      {
        this._kbExpression,
        fact
      });
    }

    internal BoolExpr<DomainConstraint<BoolLiteral, Constant>> KbExpression
    {
      get
      {
        return this._kbExpression;
      }
    }

    internal void CreateVariableConstraints(
      EntitySetBase extent,
      MemberDomainMap domainMap,
      EdmItemCollection edmItemCollection)
    {
      this.CreateVariableConstraintsRecursion((EdmType) extent.ElementType, new MemberPath(extent), domainMap, edmItemCollection);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    internal void CreateAssociationConstraints(
      EntitySetBase extent,
      MemberDomainMap domainMap,
      EdmItemCollection edmItemCollection)
    {
      AssociationSet associationSet = extent as AssociationSet;
      if (associationSet == null)
        return;
      BoolExpression literal1 = BoolExpression.CreateLiteral((BoolLiteral) new RoleBoolean((EntitySetBase) associationSet), domainMap);
      HashSet<Pair<EdmMember, EntityType>> associationkeys = new HashSet<Pair<EdmMember, EntityType>>();
      foreach (AssociationEndMember associationEndMember in associationSet.ElementType.AssociationEndMembers)
      {
        EntityType type = (EntityType) ((RefType) associationEndMember.TypeUsage.EdmType).ElementType;
        type.KeyMembers.All<EdmMember>((Func<EdmMember, bool>) (member =>
        {
          associationkeys.Add(new Pair<EdmMember, EntityType>(member, type));
          return true;
        }));
      }
      foreach (AssociationSetEnd associationSetEnd in associationSet.AssociationSetEnds)
      {
        HashSet<EdmType> edmTypeSet = new HashSet<EdmType>();
        edmTypeSet.UnionWith(MetadataHelper.GetTypeAndSubtypesOf(associationSetEnd.CorrespondingAssociationEndMember.TypeUsage.EdmType, (ItemCollection) edmItemCollection, false));
        BoolExpression isOfTypeCondition = FragmentQueryKB.CreateIsOfTypeCondition(new MemberPath((EntitySetBase) associationSetEnd.EntitySet), (IEnumerable<EdmType>) edmTypeSet, domainMap);
        BoolExpression literal2 = BoolExpression.CreateLiteral((BoolLiteral) new RoleBoolean(associationSetEnd), domainMap);
        BoolExpression and = BoolExpression.CreateAnd(BoolExpression.CreateLiteral((BoolLiteral) new RoleBoolean((EntitySetBase) associationSetEnd.EntitySet), domainMap), isOfTypeCondition);
        this.AddImplication(literal2.Tree, and.Tree);
        if (MetadataHelper.IsEveryOtherEndAtLeastOne(associationSet, associationSetEnd.CorrespondingAssociationEndMember))
          this.AddImplication(and.Tree, literal2.Tree);
        if (MetadataHelper.DoesEndKeySubsumeAssociationSetKey(associationSet, associationSetEnd.CorrespondingAssociationEndMember, associationkeys))
          this.AddEquivalence(literal2.Tree, literal1.Tree);
      }
      foreach (ReferentialConstraint referentialConstraint in associationSet.ElementType.ReferentialConstraints)
      {
        AssociationEndMember toRole = (AssociationEndMember) referentialConstraint.ToRole;
        EntitySet entitySetAtEnd = MetadataHelper.GetEntitySetAtEnd(associationSet, toRole);
        if (Helpers.IsSetEqual<EdmMember>(Helpers.AsSuperTypeList<EdmProperty, EdmMember>((IEnumerable<EdmProperty>) referentialConstraint.ToProperties), (IEnumerable<EdmMember>) entitySetAtEnd.ElementType.KeyMembers, (IEqualityComparer<EdmMember>) EqualityComparer<EdmMember>.Default) && referentialConstraint.FromRole.RelationshipMultiplicity.Equals((object) RelationshipMultiplicity.One))
          this.AddEquivalence(BoolExpression.CreateLiteral((BoolLiteral) new RoleBoolean(associationSet.AssociationSetEnds[0]), domainMap).Tree, BoolExpression.CreateLiteral((BoolLiteral) new RoleBoolean(associationSet.AssociationSetEnds[1]), domainMap).Tree);
      }
    }

    internal void CreateEquivalenceConstraintForOneToOneForeignKeyAssociation(
      AssociationSet assocSet,
      MemberDomainMap domainMap)
    {
      foreach (ReferentialConstraint referentialConstraint in assocSet.ElementType.ReferentialConstraints)
      {
        AssociationEndMember toRole = (AssociationEndMember) referentialConstraint.ToRole;
        AssociationEndMember fromRole = (AssociationEndMember) referentialConstraint.FromRole;
        EntitySet entitySetAtEnd1 = MetadataHelper.GetEntitySetAtEnd(assocSet, toRole);
        EntitySet entitySetAtEnd2 = MetadataHelper.GetEntitySetAtEnd(assocSet, fromRole);
        if (Helpers.IsSetEqual<EdmMember>(Helpers.AsSuperTypeList<EdmProperty, EdmMember>((IEnumerable<EdmProperty>) referentialConstraint.ToProperties), (IEnumerable<EdmMember>) entitySetAtEnd1.ElementType.KeyMembers, (IEqualityComparer<EdmMember>) EqualityComparer<EdmMember>.Default))
          this.AddEquivalence(BoolExpression.CreateLiteral((BoolLiteral) new RoleBoolean((EntitySetBase) entitySetAtEnd2), domainMap).Tree, BoolExpression.CreateLiteral((BoolLiteral) new RoleBoolean((EntitySetBase) entitySetAtEnd1), domainMap).Tree);
      }
    }

    private void CreateVariableConstraintsRecursion(
      EdmType edmType,
      MemberPath currentPath,
      MemberDomainMap domainMap,
      EdmItemCollection edmItemCollection)
    {
      HashSet<EdmType> edmTypeSet1 = new HashSet<EdmType>();
      edmTypeSet1.UnionWith(MetadataHelper.GetTypeAndSubtypesOf(edmType, (ItemCollection) edmItemCollection, true));
      foreach (EdmType type in edmTypeSet1)
      {
        HashSet<EdmType> edmTypeSet2 = new HashSet<EdmType>();
        edmTypeSet2.UnionWith(MetadataHelper.GetTypeAndSubtypesOf(type, (ItemCollection) edmItemCollection, false));
        if (edmTypeSet2.Count != 0)
        {
          BoolExpression not = BoolExpression.CreateNot(FragmentQueryKB.CreateIsOfTypeCondition(currentPath, (IEnumerable<EdmType>) edmTypeSet2, domainMap));
          if (not.IsSatisfiable())
          {
            foreach (EdmProperty declaredOnlyMember in ((StructuralType) type).GetDeclaredOnlyMembers<EdmProperty>())
            {
              MemberPath memberPath = new MemberPath(currentPath, (EdmMember) declaredOnlyMember);
              bool flag = MetadataHelper.IsNonRefSimpleMember((EdmMember) declaredOnlyMember);
              if (domainMap.IsConditionMember(memberPath) || domainMap.IsProjectedConditionMember(memberPath))
              {
                List<Constant> constantList = new List<Constant>(domainMap.GetDomain(memberPath));
                BoolExpression boolExpression = !flag ? BoolExpression.CreateLiteral((BoolLiteral) new TypeRestriction(new MemberProjectedSlot(memberPath), new Domain(Constant.Undefined, (IEnumerable<Constant>) constantList)), domainMap) : BoolExpression.CreateLiteral((BoolLiteral) new ScalarRestriction(new MemberProjectedSlot(memberPath), new Domain(Constant.Undefined, (IEnumerable<Constant>) constantList)), domainMap);
                this.AddEquivalence(not.Tree, boolExpression.Tree);
              }
              if (!flag)
                this.CreateVariableConstraintsRecursion(memberPath.EdmType, memberPath, domainMap, edmItemCollection);
            }
          }
        }
      }
    }

    private static BoolExpression CreateIsOfTypeCondition(
      MemberPath currentPath,
      IEnumerable<EdmType> derivedTypes,
      MemberDomainMap domainMap)
    {
      Domain domain = new Domain(derivedTypes.Select<EdmType, Constant>((Func<EdmType, Constant>) (derivedType => (Constant) new TypeConstant(derivedType))), domainMap.GetDomain(currentPath));
      return BoolExpression.CreateLiteral((BoolLiteral) new TypeRestriction(new MemberProjectedSlot(currentPath), domain), domainMap);
    }
  }
}
