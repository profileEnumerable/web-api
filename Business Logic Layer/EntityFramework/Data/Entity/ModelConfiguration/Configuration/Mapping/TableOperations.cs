// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.Mapping.TableOperations
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.ModelConfiguration.Configuration.Mapping
{
  internal static class TableOperations
  {
    public static EdmProperty CopyColumnAndAnyConstraints(
      EdmModel database,
      EntityType fromTable,
      EntityType toTable,
      EdmProperty column,
      Func<EdmProperty, bool> isCompatible,
      bool useExisting)
    {
      EdmProperty movedColumn = column;
      if (fromTable != toTable)
      {
        movedColumn = TablePrimitiveOperations.IncludeColumn(toTable, column, isCompatible, useExisting);
        if (!movedColumn.IsPrimaryKeyColumn)
          ForeignKeyPrimitiveOperations.CopyAllForeignKeyConstraintsForColumn(database, fromTable, toTable, column, movedColumn);
      }
      return movedColumn;
    }

    public static EdmProperty MoveColumnAndAnyConstraints(
      EntityType fromTable,
      EntityType toTable,
      EdmProperty column,
      bool useExisting)
    {
      EdmProperty edmProperty = column;
      if (fromTable != toTable)
      {
        edmProperty = TablePrimitiveOperations.IncludeColumn(toTable, column, TablePrimitiveOperations.GetNameMatcher(column.Name), useExisting);
        TablePrimitiveOperations.RemoveColumn(fromTable, column);
        ForeignKeyPrimitiveOperations.MoveAllForeignKeyConstraintsForColumn(fromTable, toTable, column);
      }
      return edmProperty;
    }
  }
}
