// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.ForeignKeyIndexConvention
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.Utilities;
using System.Linq;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  /// <summary>Convention to introduce indexes for foreign keys.</summary>
  public class ForeignKeyIndexConvention : IStoreModelConvention<AssociationType>, IConvention
  {
    /// <inheritdoc />
    public virtual void Apply(AssociationType item, DbModel model)
    {
      Check.NotNull<AssociationType>(item, nameof (item));
      if (item.Constraint == null)
        return;
      IEnumerable<ConsolidatedIndex> source = ConsolidatedIndex.BuildIndexes(item.Name, item.Constraint.ToProperties.Select<EdmProperty, Tuple<string, EdmProperty>>((Func<EdmProperty, Tuple<string, EdmProperty>>) (p => Tuple.Create<string, EdmProperty>(p.Name, p))));
      IEnumerable<string> dependentColumnNames = item.Constraint.ToProperties.Select<EdmProperty, string>((Func<EdmProperty, string>) (p => p.Name));
      if (source.Any<ConsolidatedIndex>((Func<ConsolidatedIndex, bool>) (c => c.Columns.SequenceEqual<string>(dependentColumnNames))))
        return;
      string name = IndexOperation.BuildDefaultName(dependentColumnNames);
      int num = 0;
      foreach (EdmProperty toProperty in item.Constraint.ToProperties)
      {
        IndexAnnotation indexAnnotation = new IndexAnnotation(new IndexAttribute(name, num++));
        object annotation = toProperty.Annotations.GetAnnotation("http://schemas.microsoft.com/ado/2013/11/edm/customannotation:Index");
        if (annotation != null)
          indexAnnotation = (IndexAnnotation) ((IndexAnnotation) annotation).MergeWith((object) indexAnnotation);
        toProperty.AddAnnotation("http://schemas.microsoft.com/ado/2013/11/edm/customannotation:Index", (object) indexAnnotation);
      }
    }
  }
}
