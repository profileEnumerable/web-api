// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.FunctionImportReturnTypeStructuralTypeColumnRenameMapping
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Xml;

namespace System.Data.Entity.Core.Mapping
{
  internal class FunctionImportReturnTypeStructuralTypeColumnRenameMapping
  {
    private readonly Collection<FunctionImportReturnTypeStructuralTypeColumn> _columnListForType;
    private readonly Collection<FunctionImportReturnTypeStructuralTypeColumn> _columnListForIsTypeOfType;
    private readonly string _defaultMemberName;
    private readonly Memoizer<StructuralType, FunctionImportReturnTypeStructuralTypeColumn> _renameCache;

    internal FunctionImportReturnTypeStructuralTypeColumnRenameMapping(string defaultMemberName)
    {
      this._defaultMemberName = defaultMemberName;
      this._columnListForType = new Collection<FunctionImportReturnTypeStructuralTypeColumn>();
      this._columnListForIsTypeOfType = new Collection<FunctionImportReturnTypeStructuralTypeColumn>();
      this._renameCache = new Memoizer<StructuralType, FunctionImportReturnTypeStructuralTypeColumn>(new Func<StructuralType, FunctionImportReturnTypeStructuralTypeColumn>(this.GetRename), (IEqualityComparer<StructuralType>) EqualityComparer<StructuralType>.Default);
    }

    internal string GetRename(EdmType type)
    {
      IXmlLineInfo lineInfo;
      return this.GetRename(type, out lineInfo);
    }

    internal string GetRename(EdmType type, out IXmlLineInfo lineInfo)
    {
      FunctionImportReturnTypeStructuralTypeColumn structuralTypeColumn = this._renameCache.Evaluate(type as StructuralType);
      lineInfo = (IXmlLineInfo) structuralTypeColumn.LineInfo;
      return structuralTypeColumn.ColumnName;
    }

    private FunctionImportReturnTypeStructuralTypeColumn GetRename(
      StructuralType typeForRename)
    {
      FunctionImportReturnTypeStructuralTypeColumn structuralTypeColumn1 = this._columnListForType.FirstOrDefault<FunctionImportReturnTypeStructuralTypeColumn>((Func<FunctionImportReturnTypeStructuralTypeColumn, bool>) (t => t.Type == typeForRename));
      if (structuralTypeColumn1 != null)
        return structuralTypeColumn1;
      FunctionImportReturnTypeStructuralTypeColumn structuralTypeColumn2 = this._columnListForIsTypeOfType.Where<FunctionImportReturnTypeStructuralTypeColumn>((Func<FunctionImportReturnTypeStructuralTypeColumn, bool>) (t => t.Type == typeForRename)).LastOrDefault<FunctionImportReturnTypeStructuralTypeColumn>();
      if (structuralTypeColumn2 != null)
        return structuralTypeColumn2;
      IEnumerable<FunctionImportReturnTypeStructuralTypeColumn> structuralTypeColumns = this._columnListForIsTypeOfType.Where<FunctionImportReturnTypeStructuralTypeColumn>((Func<FunctionImportReturnTypeStructuralTypeColumn, bool>) (t => t.Type.IsAssignableFrom((EdmType) typeForRename)));
      if (structuralTypeColumns.Count<FunctionImportReturnTypeStructuralTypeColumn>() == 0)
        return new FunctionImportReturnTypeStructuralTypeColumn(this._defaultMemberName, typeForRename, false, (LineInfo) null);
      return FunctionImportReturnTypeStructuralTypeColumnRenameMapping.GetLowestParentInHierarchy(structuralTypeColumns);
    }

    private static FunctionImportReturnTypeStructuralTypeColumn GetLowestParentInHierarchy(
      IEnumerable<FunctionImportReturnTypeStructuralTypeColumn> nodesInHierarchy)
    {
      FunctionImportReturnTypeStructuralTypeColumn structuralTypeColumn1 = (FunctionImportReturnTypeStructuralTypeColumn) null;
      foreach (FunctionImportReturnTypeStructuralTypeColumn structuralTypeColumn2 in nodesInHierarchy)
      {
        if (structuralTypeColumn1 == null)
          structuralTypeColumn1 = structuralTypeColumn2;
        else if (structuralTypeColumn1.Type.IsAssignableFrom((EdmType) structuralTypeColumn2.Type))
          structuralTypeColumn1 = structuralTypeColumn2;
      }
      return structuralTypeColumn1;
    }

    internal void AddRename(
      FunctionImportReturnTypeStructuralTypeColumn renamedColumn)
    {
      if (!renamedColumn.IsTypeOf)
        this._columnListForType.Add(renamedColumn);
      else
        this._columnListForIsTypeOfType.Add(renamedColumn);
    }
  }
}
