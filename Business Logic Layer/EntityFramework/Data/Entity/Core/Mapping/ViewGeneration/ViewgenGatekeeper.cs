// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.ViewgenGatekeeper
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.Structures;
using System.Data.Entity.Core.Mapping.ViewGeneration.Validation;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Linq;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration
{
  internal abstract class ViewgenGatekeeper : InternalBase
  {
    internal static ViewGenResults GenerateViewsFromMapping(
      EntityContainerMapping containerMapping,
      ConfigViewGenerator config)
    {
      CellCreator cellCreator = new CellCreator(containerMapping);
      List<Cell> cells = cellCreator.GenerateCells();
      CqlIdentifiers identifiers = cellCreator.Identifiers;
      return ViewgenGatekeeper.GenerateViewsFromCells(cells, config, identifiers, containerMapping);
    }

    internal static ViewGenResults GenerateTypeSpecificQueryView(
      EntityContainerMapping containerMapping,
      ConfigViewGenerator config,
      EntitySetBase entity,
      EntityTypeBase type,
      bool includeSubtypes,
      out bool success)
    {
      if (config.IsNormalTracing)
      {
        Helpers.StringTraceLine("");
        Helpers.StringTraceLine("<<<<<<<< Generating Query View for Entity [" + entity.Name + "] OfType" + (includeSubtypes ? "" : "Only") + "(" + type.Name + ") >>>>>>>");
      }
      if (containerMapping.GetEntitySetMapping(entity.Name).QueryView != null)
      {
        success = false;
        return (ViewGenResults) null;
      }
      InputForComputingCellGroups args = new InputForComputingCellGroups(containerMapping, config);
      OutputFromComputeCellGroups cellgroups = containerMapping.GetCellgroups(args);
      success = cellgroups.Success;
      if (!success)
        return (ViewGenResults) null;
      List<ForeignConstraint> foreignKeyConstraints = cellgroups.ForeignKeyConstraints;
      List<Set<Cell>> list = cellgroups.CellGroups.Select<Set<Cell>, Set<Cell>>((Func<Set<Cell>, Set<Cell>>) (setOfcells => new Set<Cell>(setOfcells.Select<Cell, Cell>((Func<Cell, Cell>) (cell => new Cell(cell)))))).ToList<Set<Cell>>();
      List<Cell> cells = cellgroups.Cells;
      CqlIdentifiers identifiers = cellgroups.Identifiers;
      ViewGenResults viewGenResults = new ViewGenResults();
      ErrorLog errorLog1 = ViewgenGatekeeper.EnsureAllCSpaceContainerSetsAreMapped((IEnumerable<Cell>) cells, containerMapping);
      if (errorLog1.Count > 0)
      {
        viewGenResults.AddErrors(errorLog1);
        Helpers.StringTraceLine(viewGenResults.ErrorsToString());
        success = true;
        return viewGenResults;
      }
      foreach (Set<Cell> set in list)
      {
        if (ViewgenGatekeeper.DoesCellGroupContainEntitySet(set, entity))
        {
          ViewGenerator viewGenerator = (ViewGenerator) null;
          ErrorLog errorLog2 = new ErrorLog();
          try
          {
            viewGenerator = new ViewGenerator(set, config, foreignKeyConstraints, containerMapping);
          }
          catch (InternalMappingException ex)
          {
            errorLog2 = ex.ErrorLog;
          }
          if (errorLog2.Count <= 0)
          {
            ViewGenMode mode = includeSubtypes ? ViewGenMode.OfTypeViews : ViewGenMode.OfTypeOnlyViews;
            ErrorLog viewForSingleExtent = viewGenerator.GenerateQueryViewForSingleExtent(viewGenResults.Views, identifiers, entity, type, mode);
            if (viewForSingleExtent.Count != 0)
              viewGenResults.AddErrors(viewForSingleExtent);
          }
          else
            break;
        }
      }
      success = true;
      return viewGenResults;
    }

    private static ViewGenResults GenerateViewsFromCells(
      List<Cell> cells,
      ConfigViewGenerator config,
      CqlIdentifiers identifiers,
      EntityContainerMapping containerMapping)
    {
      EntityContainer storageEntityContainer = containerMapping.StorageEntityContainer;
      ViewGenResults viewGenResults = new ViewGenResults();
      ErrorLog errorLog1 = ViewgenGatekeeper.EnsureAllCSpaceContainerSetsAreMapped((IEnumerable<Cell>) cells, containerMapping);
      if (errorLog1.Count > 0)
      {
        viewGenResults.AddErrors(errorLog1);
        Helpers.StringTraceLine(viewGenResults.ErrorsToString());
        return viewGenResults;
      }
      List<ForeignConstraint> foreignConstraints = ForeignConstraint.GetForeignConstraints(storageEntityContainer);
      foreach (Set<Cell> groupRelatedCell in new CellPartitioner((IEnumerable<Cell>) cells, (IEnumerable<ForeignConstraint>) foreignConstraints).GroupRelatedCells())
      {
        ViewGenerator viewGenerator = (ViewGenerator) null;
        ErrorLog errorLog2 = new ErrorLog();
        try
        {
          viewGenerator = new ViewGenerator(groupRelatedCell, config, foreignConstraints, containerMapping);
        }
        catch (InternalMappingException ex)
        {
          errorLog2 = ex.ErrorLog;
        }
        if (errorLog2.Count == 0)
          errorLog2 = viewGenerator.GenerateAllBidirectionalViews(viewGenResults.Views, identifiers);
        if (errorLog2.Count != 0)
          viewGenResults.AddErrors(errorLog2);
      }
      return viewGenResults;
    }

    private static ErrorLog EnsureAllCSpaceContainerSetsAreMapped(
      IEnumerable<Cell> cells,
      EntityContainerMapping containerMapping)
    {
      Set<EntitySetBase> set = new Set<EntitySetBase>();
      EntityContainer entityContainer = (EntityContainer) null;
      foreach (Cell cell in cells)
      {
        set.Add(cell.CQuery.Extent);
        string sourceLocation = cell.CellLabel.SourceLocation;
        entityContainer = cell.CQuery.Extent.EntityContainer;
      }
      List<EntitySetBase> entitySetBaseList = new List<EntitySetBase>();
      foreach (EntitySetBase baseEntitySet in entityContainer.BaseEntitySets)
      {
        if (!set.Contains(baseEntitySet) && !containerMapping.HasQueryViewForSetMap(baseEntitySet.Name))
        {
          AssociationSet associationSet = baseEntitySet as AssociationSet;
          if (associationSet == null || !associationSet.ElementType.IsForeignKey)
            entitySetBaseList.Add(baseEntitySet);
        }
      }
      ErrorLog errorLog = new ErrorLog();
      if (entitySetBaseList.Count > 0)
      {
        StringBuilder stringBuilder = new StringBuilder();
        bool flag = true;
        foreach (EntitySetBase entitySetBase in entitySetBaseList)
        {
          if (!flag)
            stringBuilder.Append(", ");
          flag = false;
          stringBuilder.Append(entitySetBase.Name);
        }
        string message = Strings.ViewGen_Missing_Set_Mapping((object) stringBuilder);
        int num = -1;
        foreach (Cell cell in cells)
        {
          if (num == -1 || cell.CellLabel.StartLineNumber < num)
            num = cell.CellLabel.StartLineNumber;
        }
        ErrorLog.Record record = new ErrorLog.Record(new EdmSchemaError(message, 3027, EdmSchemaErrorSeverity.Error, containerMapping.SourceLocation, containerMapping.StartLineNumber, containerMapping.StartLinePosition, (Exception) null));
        errorLog.AddEntry(record);
      }
      return errorLog;
    }

    private static bool DoesCellGroupContainEntitySet(Set<Cell> group, EntitySetBase entity)
    {
      foreach (Cell cell in group)
      {
        if (cell.GetLeftQuery(ViewTarget.QueryView).Extent.Equals((object) entity))
          return true;
      }
      return false;
    }

    internal override void ToCompactString(StringBuilder builder)
    {
    }
  }
}
