// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.PrimitivePropertyAttributeConfigurationConvention`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Utilities;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  /// <summary>
  /// Base class for conventions that process CLR attributes found on primitive properties in the model.
  /// </summary>
  /// <typeparam name="TAttribute"> The type of the attribute to look for. </typeparam>
  public abstract class PrimitivePropertyAttributeConfigurationConvention<TAttribute> : Convention
    where TAttribute : Attribute
  {
    private readonly AttributeProvider _attributeProvider = DbConfiguration.DependencyResolver.GetService<AttributeProvider>();

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.ModelConfiguration.Conventions.PrimitivePropertyAttributeConfigurationConvention`1" /> class.
    /// </summary>
    protected PrimitivePropertyAttributeConfigurationConvention()
    {
      this.Properties().Having<IEnumerable<TAttribute>>((Func<PropertyInfo, IEnumerable<TAttribute>>) (pi => this._attributeProvider.GetAttributes(pi).OfType<TAttribute>())).Configure((Action<ConventionPrimitivePropertyConfiguration, IEnumerable<TAttribute>>) ((configuration, attributes) =>
      {
        foreach (TAttribute attribute in attributes)
          this.Apply(configuration, attribute);
      }));
    }

    /// <summary>
    /// Applies this convention to a property that has an attribute of type TAttribute applied.
    /// </summary>
    /// <param name="configuration">The configuration for the property that has the attribute.</param>
    /// <param name="attribute">The attribute.</param>
    public abstract void Apply(
      ConventionPrimitivePropertyConfiguration configuration,
      TAttribute attribute);
  }
}
