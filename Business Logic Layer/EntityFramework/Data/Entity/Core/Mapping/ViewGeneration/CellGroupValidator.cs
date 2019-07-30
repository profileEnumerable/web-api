// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.CellGroupValidator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.Structures;
using System.Data.Entity.Core.Mapping.ViewGeneration.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.Validation;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Diagnostics;
using System.Linq;

namespace System.Data.Entity.Core.Mapping.ViewGeneration
{
  internal class CellGroupValidator
  {
    private readonly IEnumerable<Cell> m_cells;
    private readonly ConfigViewGenerator m_config;
    private readonly ErrorLog m_errorLog;
    private SchemaConstraints<ViewKeyConstraint> m_cViewConstraints;
    private SchemaConstraints<ViewKeyConstraint> m_sViewConstraints;

    internal CellGroupValidator(IEnumerable<Cell> cells, ConfigViewGenerator config)
    {
      this.m_cells = cells;
      this.m_config = config;
      this.m_errorLog = new ErrorLog();
    }

    internal ErrorLog Validate()
    {
      if (this.m_config.IsValidationEnabled)
      {
        if (!this.PerformSingleCellChecks())
          return this.m_errorLog;
      }
      else if (!this.CheckCellsWithDistinctFlag())
        return this.m_errorLog;
      SchemaConstraints<BasicKeyConstraint> schemaConstraints1 = new SchemaConstraints<BasicKeyConstraint>();
      SchemaConstraints<BasicKeyConstraint> schemaConstraints2 = new SchemaConstraints<BasicKeyConstraint>();
      this.ConstructCellRelationsWithConstraints(schemaConstraints1, schemaConstraints2);
      if (this.m_config.IsVerboseTracing)
      {
        Trace.WriteLine(string.Empty);
        Trace.WriteLine("C-Level Basic Constraints");
        Trace.WriteLine((object) schemaConstraints1);
        Trace.WriteLine("S-Level Basic Constraints");
        Trace.WriteLine((object) schemaConstraints2);
      }
      this.m_cViewConstraints = CellGroupValidator.PropagateConstraints(schemaConstraints1);
      this.m_sViewConstraints = CellGroupValidator.PropagateConstraints(schemaConstraints2);
      if (this.m_config.IsVerboseTracing)
      {
        Trace.WriteLine(string.Empty);
        Trace.WriteLine("C-Level View Constraints");
        Trace.WriteLine((object) this.m_cViewConstraints);
        Trace.WriteLine("S-Level View Constraints");
        Trace.WriteLine((object) this.m_sViewConstraints);
      }
      if (this.m_config.IsValidationEnabled)
        this.CheckImplication(this.m_cViewConstraints, this.m_sViewConstraints);
      return this.m_errorLog;
    }

    private void ConstructCellRelationsWithConstraints(
      SchemaConstraints<BasicKeyConstraint> cConstraints,
      SchemaConstraints<BasicKeyConstraint> sConstraints)
    {
      int cellNumber = 0;
      foreach (Cell cell in this.m_cells)
      {
        cell.CreateViewCellRelation(cellNumber);
        BasicCellRelation basicCellRelation1 = cell.CQuery.BasicCellRelation;
        BasicCellRelation basicCellRelation2 = cell.SQuery.BasicCellRelation;
        CellGroupValidator.PopulateBaseConstraints(basicCellRelation1, cConstraints);
        CellGroupValidator.PopulateBaseConstraints(basicCellRelation2, sConstraints);
        ++cellNumber;
      }
      foreach (Cell cell1 in this.m_cells)
      {
        foreach (Cell cell2 in this.m_cells)
          object.ReferenceEquals((object) cell1, (object) cell2);
      }
    }

    private static void PopulateBaseConstraints(
      BasicCellRelation baseRelation,
      SchemaConstraints<BasicKeyConstraint> constraints)
    {
      baseRelation.PopulateKeyConstraints(constraints);
    }

    private static SchemaConstraints<ViewKeyConstraint> PropagateConstraints(
      SchemaConstraints<BasicKeyConstraint> baseConstraints)
    {
      SchemaConstraints<ViewKeyConstraint> schemaConstraints = new SchemaConstraints<ViewKeyConstraint>();
      foreach (BasicKeyConstraint keyConstraint in baseConstraints.KeyConstraints)
      {
        ViewKeyConstraint constraint = keyConstraint.Propagate();
        if (constraint != null)
          schemaConstraints.Add(constraint);
      }
      return schemaConstraints;
    }

    private void CheckImplication(
      SchemaConstraints<ViewKeyConstraint> cViewConstraints,
      SchemaConstraints<ViewKeyConstraint> sViewConstraints)
    {
      this.CheckImplicationKeyConstraints(cViewConstraints, sViewConstraints);
      KeyToListMap<CellGroupValidator.ExtentPair, ViewKeyConstraint> keyToListMap = new KeyToListMap<CellGroupValidator.ExtentPair, ViewKeyConstraint>((IEqualityComparer<CellGroupValidator.ExtentPair>) EqualityComparer<CellGroupValidator.ExtentPair>.Default);
      foreach (ViewKeyConstraint keyConstraint in cViewConstraints.KeyConstraints)
      {
        CellGroupValidator.ExtentPair key = new CellGroupValidator.ExtentPair(keyConstraint.Cell.CQuery.Extent, keyConstraint.Cell.SQuery.Extent);
        keyToListMap.Add(key, keyConstraint);
      }
      foreach (CellGroupValidator.ExtentPair key in keyToListMap.Keys)
      {
        ReadOnlyCollection<ViewKeyConstraint> readOnlyCollection = keyToListMap.ListForKey(key);
        bool flag = false;
        foreach (ViewKeyConstraint second in readOnlyCollection)
        {
          foreach (ViewKeyConstraint keyConstraint in sViewConstraints.KeyConstraints)
          {
            if (keyConstraint.Implies(second))
            {
              flag = true;
              break;
            }
          }
        }
        if (!flag)
          this.m_errorLog.AddEntry(ViewKeyConstraint.GetErrorRecord((IEnumerable<ViewKeyConstraint>) readOnlyCollection));
      }
    }

    private void CheckImplicationKeyConstraints(
      SchemaConstraints<ViewKeyConstraint> leftViewConstraints,
      SchemaConstraints<ViewKeyConstraint> rightViewConstraints)
    {
      foreach (ViewKeyConstraint keyConstraint1 in rightViewConstraints.KeyConstraints)
      {
        bool flag = false;
        foreach (ViewKeyConstraint keyConstraint2 in leftViewConstraints.KeyConstraints)
        {
          if (keyConstraint2.Implies(keyConstraint1))
          {
            flag = true;
            break;
          }
        }
        if (!flag)
          this.m_errorLog.AddEntry(ViewKeyConstraint.GetErrorRecord(keyConstraint1));
      }
    }

    private bool CheckCellsWithDistinctFlag()
    {
      int count = this.m_errorLog.Count;
      foreach (Cell cell1 in this.m_cells)
      {
        Cell cell = cell1;
        if (cell.SQuery.SelectDistinctFlag == CellQuery.SelectDistinct.Yes)
        {
          EntitySetBase cExtent = cell.CQuery.Extent;
          EntitySetBase sExtent = cell.SQuery.Extent;
          IEnumerable<Cell> cells = this.m_cells.Where<Cell>((Func<Cell, bool>) (otherCell => otherCell != cell)).Where<Cell>((Func<Cell, bool>) (otherCell =>
          {
            if (otherCell.CQuery.Extent == cExtent)
              return otherCell.SQuery.Extent == sExtent;
            return false;
          }));
          if (cells.Any<Cell>())
          {
            IEnumerable<Cell> sourceCells = Enumerable.Repeat<Cell>(cell, 1).Union<Cell>(cells);
            this.m_errorLog.AddEntry(new ErrorLog.Record(ViewGenErrorCode.MultipleFragmentsBetweenCandSExtentWithDistinct, Strings.Viewgen_MultipleFragmentsBetweenCandSExtentWithDistinct((object) cExtent.Name, (object) sExtent.Name), sourceCells, string.Empty));
          }
        }
      }
      return this.m_errorLog.Count == count;
    }

    private bool PerformSingleCellChecks()
    {
      int count = this.m_errorLog.Count;
      foreach (Cell cell in this.m_cells)
      {
        ErrorLog.Record record1 = cell.SQuery.CheckForDuplicateFields(cell.CQuery, cell);
        if (record1 != null)
          this.m_errorLog.AddEntry(record1);
        ErrorLog.Record record2 = cell.CQuery.VerifyKeysPresent(cell, new Func<object, object, string>(Strings.ViewGen_EntitySetKey_Missing), new Func<object, object, object, string>(Strings.ViewGen_AssociationSetKey_Missing), ViewGenErrorCode.KeyNotMappedForCSideExtent);
        if (record2 != null)
          this.m_errorLog.AddEntry(record2);
        ErrorLog.Record record3 = cell.SQuery.VerifyKeysPresent(cell, new Func<object, object, string>(Strings.ViewGen_TableKey_Missing), (Func<object, object, object, string>) null, ViewGenErrorCode.KeyNotMappedForTable);
        if (record3 != null)
          this.m_errorLog.AddEntry(record3);
        ErrorLog.Record record4 = cell.CQuery.CheckForProjectedNotNullSlots(cell, this.m_cells.Where<Cell>((Func<Cell, bool>) (c => c.SQuery.Extent is AssociationSet)));
        if (record4 != null)
          this.m_errorLog.AddEntry(record4);
        ErrorLog.Record record5 = cell.SQuery.CheckForProjectedNotNullSlots(cell, this.m_cells.Where<Cell>((Func<Cell, bool>) (c => c.CQuery.Extent is AssociationSet)));
        if (record5 != null)
          this.m_errorLog.AddEntry(record5);
      }
      return this.m_errorLog.Count == count;
    }

    [Conditional("DEBUG")]
    private static void CheckConstraintSanity(
      SchemaConstraints<BasicKeyConstraint> cConstraints,
      SchemaConstraints<BasicKeyConstraint> sConstraints,
      SchemaConstraints<ViewKeyConstraint> cViewConstraints,
      SchemaConstraints<ViewKeyConstraint> sViewConstraints)
    {
    }

    private class ExtentPair
    {
      internal readonly EntitySetBase cExtent;
      internal readonly EntitySetBase sExtent;

      internal ExtentPair(EntitySetBase acExtent, EntitySetBase asExtent)
      {
        this.cExtent = acExtent;
        this.sExtent = asExtent;
      }

      public override bool Equals(object obj)
      {
        if (object.ReferenceEquals((object) this, obj))
          return true;
        CellGroupValidator.ExtentPair extentPair = obj as CellGroupValidator.ExtentPair;
        if (extentPair == null || !extentPair.cExtent.Equals((object) this.cExtent))
          return false;
        return extentPair.sExtent.Equals((object) this.sExtent);
      }

      public override int GetHashCode()
      {
        return this.cExtent.GetHashCode() ^ this.sExtent.GetHashCode();
      }
    }
  }
}
