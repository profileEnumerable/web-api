// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.ViewgenContext
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting;
using System.Data.Entity.Core.Mapping.ViewGeneration.Structures;
using System.Data.Entity.Core.Mapping.ViewGeneration.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Linq;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration
{
  internal class ViewgenContext : InternalBase
  {
    private readonly ConfigViewGenerator m_config;
    private readonly ViewTarget m_viewTarget;
    private readonly EntitySetBase m_extent;
    private readonly MemberMaps m_memberMaps;
    private readonly EdmItemCollection m_edmItemCollection;
    private readonly EntityContainerMapping m_entityContainerMapping;
    private List<LeftCellWrapper> m_cellWrappers;
    private readonly FragmentQueryProcessor m_leftFragmentQP;
    private readonly FragmentQueryProcessor m_rightFragmentQP;
    private readonly CqlIdentifiers m_identifiers;
    private readonly Dictionary<FragmentQuery, Tile<FragmentQuery>> m_rewritingCache;

    internal ViewgenContext(
      ViewTarget viewTarget,
      EntitySetBase extent,
      IList<Cell> extentCells,
      CqlIdentifiers identifiers,
      ConfigViewGenerator config,
      MemberDomainMap queryDomainMap,
      MemberDomainMap updateDomainMap,
      EntityContainerMapping entityContainerMapping)
    {
      foreach (Cell extentCell in (IEnumerable<Cell>) extentCells)
        ;
      this.m_extent = extent;
      this.m_viewTarget = viewTarget;
      this.m_config = config;
      this.m_edmItemCollection = entityContainerMapping.StorageMappingItemCollection.EdmItemCollection;
      this.m_entityContainerMapping = entityContainerMapping;
      this.m_identifiers = identifiers;
      updateDomainMap = updateDomainMap.MakeCopy();
      MemberDomainMap domainMap = viewTarget == ViewTarget.QueryView ? queryDomainMap : updateDomainMap;
      this.m_memberMaps = new MemberMaps(viewTarget, MemberProjectionIndex.Create(extent, this.m_edmItemCollection), queryDomainMap, updateDomainMap);
      FragmentQueryKBChaseSupport kb1 = new FragmentQueryKBChaseSupport();
      kb1.CreateVariableConstraints(extent, domainMap, this.m_edmItemCollection);
      this.m_leftFragmentQP = new FragmentQueryProcessor(kb1);
      this.m_rewritingCache = new Dictionary<FragmentQuery, Tile<FragmentQuery>>(FragmentQuery.GetEqualityComparer(this.m_leftFragmentQP));
      if (!this.CreateLeftCellWrappers(extentCells, viewTarget))
        return;
      FragmentQueryKBChaseSupport kb2 = new FragmentQueryKBChaseSupport();
      MemberDomainMap memberDomainMap = viewTarget == ViewTarget.QueryView ? updateDomainMap : queryDomainMap;
      foreach (LeftCellWrapper cellWrapper in this.m_cellWrappers)
      {
        EntitySetBase rightExtent = cellWrapper.RightExtent;
        kb2.CreateVariableConstraints(rightExtent, memberDomainMap, this.m_edmItemCollection);
        kb2.CreateAssociationConstraints(rightExtent, memberDomainMap, this.m_edmItemCollection);
      }
      if (this.m_viewTarget == ViewTarget.UpdateView)
        this.CreateConstraintsForForeignKeyAssociationsAffectingThisWrapper((FragmentQueryKB) kb2, memberDomainMap);
      this.m_rightFragmentQP = new FragmentQueryProcessor(kb2);
      if (this.m_viewTarget == ViewTarget.QueryView)
        this.CheckConcurrencyControlTokens();
      this.m_cellWrappers.Sort(LeftCellWrapper.Comparer);
    }

    private void CreateConstraintsForForeignKeyAssociationsAffectingThisWrapper(
      FragmentQueryKB rightKB,
      MemberDomainMap rightDomainMap)
    {
      foreach (AssociationSet assocSet in new ViewgenContext.OneToOneFkAssociationsForEntitiesFilter().Filter((IList<EntityType>) this.m_cellWrappers.Select<LeftCellWrapper, EntitySetBase>((Func<LeftCellWrapper, EntitySetBase>) (it => it.RightExtent)).OfType<EntitySet>().Select<EntitySet, EntityType>((Func<EntitySet, EntityType>) (it => it.ElementType)).ToList<EntityType>(), this.m_entityContainerMapping.EdmEntityContainer.BaseEntitySets.OfType<AssociationSet>()))
        rightKB.CreateEquivalenceConstraintForOneToOneForeignKeyAssociation(assocSet, rightDomainMap);
    }

    internal ViewTarget ViewTarget
    {
      get
      {
        return this.m_viewTarget;
      }
    }

    internal MemberMaps MemberMaps
    {
      get
      {
        return this.m_memberMaps;
      }
    }

    internal EntitySetBase Extent
    {
      get
      {
        return this.m_extent;
      }
    }

    internal ConfigViewGenerator Config
    {
      get
      {
        return this.m_config;
      }
    }

    internal CqlIdentifiers CqlIdentifiers
    {
      get
      {
        return this.m_identifiers;
      }
    }

    internal EdmItemCollection EdmItemCollection
    {
      get
      {
        return this.m_edmItemCollection;
      }
    }

    internal FragmentQueryProcessor LeftFragmentQP
    {
      get
      {
        return this.m_leftFragmentQP;
      }
    }

    internal FragmentQueryProcessor RightFragmentQP
    {
      get
      {
        return this.m_rightFragmentQP;
      }
    }

    internal List<LeftCellWrapper> AllWrappersForExtent
    {
      get
      {
        return this.m_cellWrappers;
      }
    }

    internal EntityContainerMapping EntityContainerMapping
    {
      get
      {
        return this.m_entityContainerMapping;
      }
    }

    internal bool TryGetCachedRewriting(FragmentQuery query, out Tile<FragmentQuery> rewriting)
    {
      return this.m_rewritingCache.TryGetValue(query, out rewriting);
    }

    internal void SetCachedRewriting(FragmentQuery query, Tile<FragmentQuery> rewriting)
    {
      this.m_rewritingCache[query] = rewriting;
    }

    private void CheckConcurrencyControlTokens()
    {
      EntityTypeBase elementType = this.m_extent.ElementType;
      Set<EdmMember> forTypeHierarchy = MetadataHelper.GetConcurrencyMembersForTypeHierarchy(elementType, this.m_edmItemCollection);
      Set<MemberPath> other = new Set<MemberPath>(MemberPath.EqualityComparer);
      foreach (EdmMember member in forTypeHierarchy)
      {
        if (!member.DeclaringType.IsAssignableFrom((EdmType) elementType))
          ExceptionHelpers.ThrowMappingException(new ErrorLog.Record(ViewGenErrorCode.ConcurrencyDerivedClass, Strings.ViewGen_Concurrency_Derived_Class((object) member.Name, (object) member.DeclaringType.Name, (object) this.m_extent), (IEnumerable<LeftCellWrapper>) this.m_cellWrappers, string.Empty), this.m_config);
        other.Add(new MemberPath(this.m_extent, member));
      }
      if (forTypeHierarchy.Count <= 0)
        return;
      foreach (LeftCellWrapper cellWrapper in this.m_cellWrappers)
      {
        Set<MemberPath> set = new Set<MemberPath>(cellWrapper.OnlyInputCell.CQuery.WhereClause.MemberRestrictions.Select<MemberRestriction, MemberPath>((Func<MemberRestriction, MemberPath>) (oneOf => oneOf.RestrictedMemberSlot.MemberPath)), MemberPath.EqualityComparer);
        set.Intersect(other);
        if (set.Count > 0)
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.AppendLine(Strings.ViewGen_Concurrency_Invalid_Condition((object) MemberPath.PropertiesToUserString((IEnumerable<MemberPath>) set, false), (object) this.m_extent.Name));
          ExceptionHelpers.ThrowMappingException(new ErrorLog.Record(ViewGenErrorCode.ConcurrencyTokenHasCondition, stringBuilder.ToString(), (IEnumerable<LeftCellWrapper>) new LeftCellWrapper[1]
          {
            cellWrapper
          }, string.Empty), this.m_config);
        }
      }
    }

    private bool CreateLeftCellWrappers(IList<Cell> extentCells, ViewTarget viewTarget)
    {
      List<Cell> cellList = ViewgenContext.AlignFields((IEnumerable<Cell>) extentCells, this.m_memberMaps.ProjectedSlotMap, viewTarget);
      this.m_cellWrappers = new List<LeftCellWrapper>();
      for (int index = 0; index < cellList.Count; ++index)
      {
        Cell cell = cellList[index];
        CellQuery leftQuery = cell.GetLeftQuery(viewTarget);
        CellQuery rightQuery = cell.GetRightQuery(viewTarget);
        Set<MemberPath> nonNullSlots = leftQuery.GetNonNullSlots();
        FragmentQuery fragmentQuery = FragmentQuery.Create(BoolExpression.CreateLiteral((BoolLiteral) new CellIdBoolean(this.m_identifiers, extentCells[index].CellNumber), this.m_memberMaps.LeftDomainMap), leftQuery);
        if (viewTarget == ViewTarget.UpdateView)
          fragmentQuery = this.m_leftFragmentQP.CreateDerivedViewBySelectingConstantAttributes(fragmentQuery) ?? fragmentQuery;
        this.m_cellWrappers.Add(new LeftCellWrapper(this.m_viewTarget, nonNullSlots, fragmentQuery, leftQuery, rightQuery, this.m_memberMaps, extentCells[index]));
      }
      return true;
    }

    private static List<Cell> AlignFields(
      IEnumerable<Cell> cells,
      MemberProjectionIndex projectedSlotMap,
      ViewTarget viewTarget)
    {
      List<Cell> cellList = new List<Cell>();
      foreach (Cell cell1 in cells)
      {
        CellQuery newMainQuery;
        CellQuery newOtherQuery;
        cell1.GetLeftQuery(viewTarget).CreateFieldAlignedCellQueries(cell1.GetRightQuery(viewTarget), projectedSlotMap, out newMainQuery, out newOtherQuery);
        Cell cell2 = viewTarget == ViewTarget.QueryView ? Cell.CreateCS(newMainQuery, newOtherQuery, cell1.CellLabel, cell1.CellNumber) : Cell.CreateCS(newOtherQuery, newMainQuery, cell1.CellLabel, cell1.CellNumber);
        cellList.Add(cell2);
      }
      return cellList;
    }

    internal override void ToCompactString(StringBuilder builder)
    {
      LeftCellWrapper.WrappersToStringBuilder(builder, this.m_cellWrappers, "Left Celll Wrappers");
    }

    internal class OneToOneFkAssociationsForEntitiesFilter
    {
      public virtual IEnumerable<AssociationSet> Filter(
        IList<EntityType> entityTypes,
        IEnumerable<AssociationSet> associationSets)
      {
        return associationSets.Where<AssociationSet>((Func<AssociationSet, bool>) (a =>
        {
          if (a.ElementType.IsForeignKey)
            return a.ElementType.AssociationEndMembers.All<AssociationEndMember>((Func<AssociationEndMember, bool>) (aem =>
            {
              if (aem.RelationshipMultiplicity == RelationshipMultiplicity.One)
                return entityTypes.Contains(aem.GetEntityType());
              return false;
            }));
          return false;
        }));
      }
    }
  }
}
