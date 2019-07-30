// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.ViewGenerator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting;
using System.Data.Entity.Core.Mapping.ViewGeneration.Structures;
using System.Data.Entity.Core.Mapping.ViewGeneration.Validation;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration
{
  internal class ViewGenerator : InternalBase
  {
    private readonly Set<Cell> m_cellGroup;
    private readonly ConfigViewGenerator m_config;
    private readonly MemberDomainMap m_queryDomainMap;
    private readonly MemberDomainMap m_updateDomainMap;
    private readonly Dictionary<EntitySetBase, QueryRewriter> m_queryRewriterCache;
    private readonly List<ForeignConstraint> m_foreignKeyConstraints;
    private readonly EntityContainerMapping m_entityContainerMapping;

    internal ViewGenerator(
      Set<Cell> cellGroup,
      ConfigViewGenerator config,
      List<ForeignConstraint> foreignKeyConstraints,
      EntityContainerMapping entityContainerMapping)
    {
      this.m_cellGroup = cellGroup;
      this.m_config = config;
      this.m_queryRewriterCache = new Dictionary<EntitySetBase, QueryRewriter>();
      this.m_foreignKeyConstraints = foreignKeyConstraints;
      this.m_entityContainerMapping = entityContainerMapping;
      Dictionary<EntityType, Set<EntityType>> inheritanceGraph = MetadataHelper.BuildUndirectedGraphOfTypes(entityContainerMapping.StorageMappingItemCollection.EdmItemCollection);
      this.SetConfiguration(entityContainerMapping);
      this.m_queryDomainMap = new MemberDomainMap(ViewTarget.QueryView, this.m_config.IsValidationEnabled, (IEnumerable<Cell>) cellGroup, entityContainerMapping.StorageMappingItemCollection.EdmItemCollection, this.m_config, inheritanceGraph);
      this.m_updateDomainMap = new MemberDomainMap(ViewTarget.UpdateView, this.m_config.IsValidationEnabled, (IEnumerable<Cell>) cellGroup, entityContainerMapping.StorageMappingItemCollection.EdmItemCollection, this.m_config, inheritanceGraph);
      MemberDomainMap.PropagateUpdateDomainToQueryDomain((IEnumerable<Cell>) cellGroup, this.m_queryDomainMap, this.m_updateDomainMap);
      ViewGenerator.UpdateWhereClauseForEachCell((IEnumerable<Cell>) cellGroup, this.m_queryDomainMap, this.m_updateDomainMap, this.m_config);
      MemberDomainMap openDomain1 = this.m_queryDomainMap.GetOpenDomain();
      MemberDomainMap openDomain2 = this.m_updateDomainMap.GetOpenDomain();
      foreach (Cell cell in cellGroup)
      {
        cell.CQuery.WhereClause.FixDomainMap(openDomain1);
        cell.SQuery.WhereClause.FixDomainMap(openDomain2);
        cell.CQuery.WhereClause.ExpensiveSimplify();
        cell.SQuery.WhereClause.ExpensiveSimplify();
        cell.CQuery.WhereClause.FixDomainMap(this.m_queryDomainMap);
        cell.SQuery.WhereClause.FixDomainMap(this.m_updateDomainMap);
      }
    }

    private void SetConfiguration(EntityContainerMapping entityContainerMapping)
    {
      this.m_config.IsValidationEnabled = entityContainerMapping.Validate;
      this.m_config.GenerateUpdateViews = entityContainerMapping.GenerateUpdateViews;
    }

    internal ErrorLog GenerateAllBidirectionalViews(
      KeyToListMap<EntitySetBase, GeneratedView> views,
      CqlIdentifiers identifiers)
    {
      if (this.m_config.IsNormalTracing)
      {
        StringBuilder builder = new StringBuilder();
        Cell.CellsToBuilder(builder, (IEnumerable<Cell>) this.m_cellGroup);
        Helpers.StringTraceLine(builder.ToString());
      }
      this.m_config.SetTimeForFinishedActivity(PerfType.CellCreation);
      ErrorLog errorLog = new CellGroupValidator((IEnumerable<Cell>) this.m_cellGroup, this.m_config).Validate();
      if (errorLog.Count > 0)
      {
        errorLog.PrintTrace();
        return errorLog;
      }
      this.m_config.SetTimeForFinishedActivity(PerfType.KeyConstraint);
      if (this.m_config.GenerateUpdateViews)
      {
        errorLog = this.GenerateDirectionalViews(ViewTarget.UpdateView, identifiers, views);
        if (errorLog.Count > 0)
          return errorLog;
      }
      if (this.m_config.IsValidationEnabled)
        this.CheckForeignKeyConstraints(errorLog);
      this.m_config.SetTimeForFinishedActivity(PerfType.ForeignConstraint);
      if (errorLog.Count > 0)
      {
        errorLog.PrintTrace();
        return errorLog;
      }
      this.m_updateDomainMap.ExpandDomainsToIncludeAllPossibleValues();
      return this.GenerateDirectionalViews(ViewTarget.QueryView, identifiers, views);
    }

    internal ErrorLog GenerateQueryViewForSingleExtent(
      KeyToListMap<EntitySetBase, GeneratedView> views,
      CqlIdentifiers identifiers,
      EntitySetBase entity,
      EntityTypeBase type,
      ViewGenMode mode)
    {
      if (this.m_config.IsNormalTracing)
      {
        StringBuilder builder = new StringBuilder();
        Cell.CellsToBuilder(builder, (IEnumerable<Cell>) this.m_cellGroup);
        Helpers.StringTraceLine(builder.ToString());
      }
      ErrorLog errorLog = new CellGroupValidator((IEnumerable<Cell>) this.m_cellGroup, this.m_config).Validate();
      if (errorLog.Count > 0)
      {
        errorLog.PrintTrace();
        return errorLog;
      }
      if (this.m_config.IsValidationEnabled)
        this.CheckForeignKeyConstraints(errorLog);
      if (errorLog.Count > 0)
      {
        errorLog.PrintTrace();
        return errorLog;
      }
      this.m_updateDomainMap.ExpandDomainsToIncludeAllPossibleValues();
      foreach (Cell cell in this.m_cellGroup)
        cell.SQuery.WhereClause.FixDomainMap(this.m_updateDomainMap);
      return this.GenerateQueryViewForExtentAndType(identifiers, views, entity, type, mode);
    }

    private static void UpdateWhereClauseForEachCell(
      IEnumerable<Cell> extentCells,
      MemberDomainMap queryDomainMap,
      MemberDomainMap updateDomainMap,
      ConfigViewGenerator config)
    {
      foreach (Cell extentCell in extentCells)
      {
        extentCell.CQuery.UpdateWhereClause(queryDomainMap);
        extentCell.SQuery.UpdateWhereClause(updateDomainMap);
      }
      queryDomainMap.ReduceEnumerableDomainToEnumeratedValues(config);
      updateDomainMap.ReduceEnumerableDomainToEnumeratedValues(config);
    }

    private ErrorLog GenerateQueryViewForExtentAndType(
      CqlIdentifiers identifiers,
      KeyToListMap<EntitySetBase, GeneratedView> views,
      EntitySetBase entity,
      EntityTypeBase type,
      ViewGenMode mode)
    {
      ErrorLog errorLog = new ErrorLog();
      if (this.m_config.IsViewTracing)
      {
        Helpers.StringTraceLine(string.Empty);
        Helpers.StringTraceLine(string.Empty);
        Helpers.FormatTraceLine("================= Generating {0} Query View for: {1} ===========================", mode == ViewGenMode.OfTypeViews ? (object) "OfType" : (object) "OfTypeOnly", (object) entity.Name);
        Helpers.StringTraceLine(string.Empty);
        Helpers.StringTraceLine(string.Empty);
      }
      try
      {
        ViewgenContext viewgenContext = this.CreateViewgenContext(entity, ViewTarget.QueryView, identifiers);
        this.GenerateViewsForExtentAndType((EdmType) type, viewgenContext, identifiers, views, mode);
      }
      catch (InternalMappingException ex)
      {
        errorLog.Merge(ex.ErrorLog);
      }
      return errorLog;
    }

    private ErrorLog GenerateDirectionalViews(
      ViewTarget viewTarget,
      CqlIdentifiers identifiers,
      KeyToListMap<EntitySetBase, GeneratedView> views)
    {
      bool flag = viewTarget == ViewTarget.QueryView;
      KeyToListMap<EntitySetBase, Cell> keyToListMap = ViewGenerator.GroupCellsByExtent((IEnumerable<Cell>) this.m_cellGroup, viewTarget);
      ErrorLog errorLog = new ErrorLog();
      foreach (EntitySetBase key in keyToListMap.Keys)
      {
        if (this.m_config.IsViewTracing)
        {
          Helpers.StringTraceLine(string.Empty);
          Helpers.StringTraceLine(string.Empty);
          Helpers.FormatTraceLine("================= Generating {0} View for: {1} ===========================", flag ? (object) "Query" : (object) "Update", (object) key.Name);
          Helpers.StringTraceLine(string.Empty);
          Helpers.StringTraceLine(string.Empty);
        }
        try
        {
          QueryRewriter directionalViewsForExtent = this.GenerateDirectionalViewsForExtent(viewTarget, key, identifiers, views);
          if (viewTarget == ViewTarget.UpdateView)
          {
            if (this.m_config.IsValidationEnabled)
            {
              if (this.m_config.IsViewTracing)
              {
                Helpers.StringTraceLine(string.Empty);
                Helpers.StringTraceLine(string.Empty);
                Helpers.FormatTraceLine("----------------- Validation for generated update view for: {0} -----------------", (object) key.Name);
                Helpers.StringTraceLine(string.Empty);
                Helpers.StringTraceLine(string.Empty);
              }
              new RewritingValidator(directionalViewsForExtent.ViewgenContext, directionalViewsForExtent.BasicView).Validate();
            }
          }
        }
        catch (InternalMappingException ex)
        {
          errorLog.Merge(ex.ErrorLog);
        }
      }
      return errorLog;
    }

    private QueryRewriter GenerateDirectionalViewsForExtent(
      ViewTarget viewTarget,
      EntitySetBase extent,
      CqlIdentifiers identifiers,
      KeyToListMap<EntitySetBase, GeneratedView> views)
    {
      ViewgenContext viewgenContext = this.CreateViewgenContext(extent, viewTarget, identifiers);
      QueryRewriter queryRewriter = (QueryRewriter) null;
      if (this.m_config.GenerateViewsForEachType)
      {
        foreach (EdmType generatedType in MetadataHelper.GetTypeAndSubtypesOf((EdmType) extent.ElementType, (ItemCollection) this.m_entityContainerMapping.StorageMappingItemCollection.EdmItemCollection, false))
        {
          if (this.m_config.IsViewTracing && !generatedType.Equals((object) extent.ElementType))
            Helpers.FormatTraceLine("CQL View for {0} and type {1}", (object) extent.Name, (object) generatedType.Name);
          queryRewriter = this.GenerateViewsForExtentAndType(generatedType, viewgenContext, identifiers, views, ViewGenMode.OfTypeViews);
        }
      }
      else
        queryRewriter = this.GenerateViewsForExtentAndType((EdmType) extent.ElementType, viewgenContext, identifiers, views, ViewGenMode.OfTypeViews);
      if (viewTarget == ViewTarget.QueryView)
        this.m_config.SetTimeForFinishedActivity(PerfType.QueryViews);
      else
        this.m_config.SetTimeForFinishedActivity(PerfType.UpdateViews);
      this.m_queryRewriterCache[extent] = queryRewriter;
      return queryRewriter;
    }

    private ViewgenContext CreateViewgenContext(
      EntitySetBase extent,
      ViewTarget viewTarget,
      CqlIdentifiers identifiers)
    {
      QueryRewriter queryRewriter;
      if (this.m_queryRewriterCache.TryGetValue(extent, out queryRewriter))
        return queryRewriter.ViewgenContext;
      List<Cell> list = this.m_cellGroup.Where<Cell>((Func<Cell, bool>) (c => c.GetLeftQuery(viewTarget).Extent == extent)).ToList<Cell>();
      return new ViewgenContext(viewTarget, extent, (IList<Cell>) list, identifiers, this.m_config, this.m_queryDomainMap, this.m_updateDomainMap, this.m_entityContainerMapping);
    }

    private QueryRewriter GenerateViewsForExtentAndType(
      EdmType generatedType,
      ViewgenContext context,
      CqlIdentifiers identifiers,
      KeyToListMap<EntitySetBase, GeneratedView> views,
      ViewGenMode mode)
    {
      QueryRewriter queryRewriter = new QueryRewriter(generatedType, context, mode);
      queryRewriter.GenerateViewComponents();
      CellTreeNode basicView = queryRewriter.BasicView;
      if (this.m_config.IsNormalTracing)
      {
        Helpers.StringTrace("Basic View: ");
        Helpers.StringTraceLine(basicView.ToString());
      }
      CellTreeNode simplifiedView = ViewGenerator.GenerateSimplifiedView(basicView, queryRewriter.UsedCells);
      if (this.m_config.IsNormalTracing)
      {
        Helpers.StringTraceLine(string.Empty);
        Helpers.StringTrace("Simplified View: ");
        Helpers.StringTraceLine(simplifiedView.ToString());
      }
      CqlGenerator cqlGenerator = new CqlGenerator(simplifiedView, queryRewriter.CaseStatements, identifiers, context.MemberMaps.ProjectedSlotMap, queryRewriter.UsedCells.Count, queryRewriter.TopLevelWhereClause, this.m_entityContainerMapping.StorageMappingItemCollection);
      string eSQL;
      DbQueryCommandTree commandTree;
      if (this.m_config.GenerateEsql)
      {
        eSQL = cqlGenerator.GenerateEsql();
        commandTree = (DbQueryCommandTree) null;
      }
      else
      {
        eSQL = (string) null;
        commandTree = cqlGenerator.GenerateCqt();
      }
      GeneratedView generatedView = GeneratedView.CreateGeneratedView(context.Extent, generatedType, commandTree, eSQL, this.m_entityContainerMapping.StorageMappingItemCollection, this.m_config);
      views.Add(context.Extent, generatedView);
      return queryRewriter;
    }

    private static CellTreeNode GenerateSimplifiedView(
      CellTreeNode basicView,
      List<LeftCellWrapper> usedCells)
    {
      int count = usedCells.Count;
      for (int cellNum = 0; cellNum < count; ++cellNum)
        usedCells[cellNum].RightCellQuery.InitializeBoolExpressions(count, cellNum);
      return CellTreeSimplifier.MergeNodes(basicView);
    }

    private void CheckForeignKeyConstraints(ErrorLog errorLog)
    {
      foreach (ForeignConstraint foreignKeyConstraint in this.m_foreignKeyConstraints)
      {
        QueryRewriter childRewriter = (QueryRewriter) null;
        QueryRewriter parentRewriter = (QueryRewriter) null;
        this.m_queryRewriterCache.TryGetValue((EntitySetBase) foreignKeyConstraint.ChildTable, out childRewriter);
        this.m_queryRewriterCache.TryGetValue((EntitySetBase) foreignKeyConstraint.ParentTable, out parentRewriter);
        foreignKeyConstraint.CheckConstraint(this.m_cellGroup, childRewriter, parentRewriter, errorLog, this.m_config);
      }
    }

    private static KeyToListMap<EntitySetBase, Cell> GroupCellsByExtent(
      IEnumerable<Cell> cells,
      ViewTarget viewTarget)
    {
      KeyToListMap<EntitySetBase, Cell> keyToListMap = new KeyToListMap<EntitySetBase, Cell>((IEqualityComparer<EntitySetBase>) EqualityComparer<EntitySetBase>.Default);
      foreach (Cell cell in cells)
      {
        CellQuery leftQuery = cell.GetLeftQuery(viewTarget);
        keyToListMap.Add(leftQuery.Extent, cell);
      }
      return keyToListMap;
    }

    internal override void ToCompactString(StringBuilder builder)
    {
      Cell.CellsToBuilder(builder, (IEnumerable<Cell>) this.m_cellGroup);
    }
  }
}
