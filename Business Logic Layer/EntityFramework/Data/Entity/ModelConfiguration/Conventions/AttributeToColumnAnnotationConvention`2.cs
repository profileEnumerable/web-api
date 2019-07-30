// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.AttributeToColumnAnnotationConvention`2
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Utilities;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  /// <summary>
  /// A general purpose class for Code First conventions that read attributes from .NET properties
  /// and generate column annotations based on those attributes.
  /// </summary>
  /// <typeparam name="TAttribute">The type of attribute to discover.</typeparam>
  /// <typeparam name="TAnnotation">The type of annotation that will be created.</typeparam>
  public class AttributeToColumnAnnotationConvention<TAttribute, TAnnotation> : Convention where TAttribute : Attribute
  {
    /// <summary>
    /// Constructs a convention that will create column annotations with the given name and
    /// using the given factory delegate.
    /// </summary>
    /// <param name="annotationName">The name of the annotations to create.</param>
    /// <param name="annotationFactory">A factory for creating the annotation on each column.</param>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public AttributeToColumnAnnotationConvention(
      string annotationName,
      Func<PropertyInfo, IList<TAttribute>, TAnnotation> annotationFactory)
    {
      Check.NotEmpty(annotationName, nameof (annotationName));
      Check.NotNull<Func<PropertyInfo, IList<TAttribute>, TAnnotation>>(annotationFactory, nameof (annotationFactory));
      AttributeProvider attributeProvider = DbConfiguration.DependencyResolver.GetService<AttributeProvider>();
      this.Properties().Having<List<TAttribute>>((Func<PropertyInfo, List<TAttribute>>) (pi => attributeProvider.GetAttributes(pi).OfType<TAttribute>().ToList<TAttribute>())).Configure((Action<ConventionPrimitivePropertyConfiguration, List<TAttribute>>) ((c, a) =>
      {
        if (!a.Any<TAttribute>())
          return;
        c.HasColumnAnnotation(annotationName, (object) annotationFactory(c.ClrPropertyInfo, (IList<TAttribute>) a));
      }));
    }
  }
}
