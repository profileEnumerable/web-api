// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.Mapping.TablePrimitiveOperations
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Linq;

namespace System.Data.Entity.ModelConfiguration.Configuration.Mapping
{
  internal static class TablePrimitiveOperations
  {
    public static void AddColumn(EntityType table, EdmProperty column)
    {
      if (table.Properties.Contains(column))
        return;
      System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration configuration = column.GetConfiguration() as System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration;
      if (configuration == null || string.IsNullOrWhiteSpace(configuration.ColumnName))
      {
        string name = column.GetPreferredName() ?? column.Name;
        column.SetUnpreferredUniqueName(column.Name);
        column.Name = ((IEnumerable<INamedDataModelItem>) table.Properties).UniquifyName(name);
      }
      table.AddMember((EdmMember) column);
    }

    public static EdmProperty RemoveColumn(EntityType table, EdmProperty column)
    {
      if (!column.IsPrimaryKeyColumn)
        table.RemoveMember((EdmMember) column);
      return column;
    }

    public static EdmProperty IncludeColumn(
      EntityType table,
      EdmProperty templateColumn,
      Func<EdmProperty, bool> isCompatible,
      bool useExisting)
    {
      EdmProperty edmProperty = table.Properties.FirstOrDefault<EdmProperty>(isCompatible);
      templateColumn = edmProperty != null ? (useExisting || edmProperty.IsPrimaryKeyColumn ? edmProperty : templateColumn.Clone()) : templateColumn.Clone();
      TablePrimitiveOperations.AddColumn(table, templateColumn);
      return templateColumn;
    }

    public static Func<EdmProperty, bool> GetNameMatcher(string name)
    {
      return (Func<EdmProperty, bool>) (c => string.Equals(c.Name, name, StringComparison.Ordinal));
    }
  }
}
