// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.IndexAttributeConvention
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  /// <summary>
  /// A convention for discovering <see cref="T:System.ComponentModel.DataAnnotations.Schema.IndexAttribute" /> attributes on properties and generating
  /// <see cref="T:System.Data.Entity.Infrastructure.Annotations.IndexAnnotation" /> column annotations in the model.
  /// </summary>
  public class IndexAttributeConvention : AttributeToColumnAnnotationConvention<IndexAttribute, IndexAnnotation>
  {
    /// <summary>Constructs a new instance of the convention.</summary>
    public IndexAttributeConvention()
      : base("Index", (Func<PropertyInfo, IList<IndexAttribute>, IndexAnnotation>) ((p, a) => new IndexAnnotation(p, (IEnumerable<IndexAttribute>) a.OrderBy<IndexAttribute, string>((Func<IndexAttribute, string>) (i => i.ToString())))))
    {
    }
  }
}
