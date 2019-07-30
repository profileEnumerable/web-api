// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.PropertyAttributeConfigurationConvention`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Utilities;
using System.Data.Entity.Utilities;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  /// <summary>
  /// Base class for conventions that process CLR attributes found on properties of types in the model.
  /// </summary>
  /// <remarks>
  /// Note that the derived convention will be applied for any non-static property on the mapped type that has
  /// the specified attribute, even if it wasn't included in the model.
  /// </remarks>
  /// <typeparam name="TAttribute"> The type of the attribute to look for. </typeparam>
  public abstract class PropertyAttributeConfigurationConvention<TAttribute> : Convention where TAttribute : Attribute
  {
    private readonly AttributeProvider _attributeProvider = DbConfiguration.DependencyResolver.GetService<AttributeProvider>();

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.ModelConfiguration.Conventions.PropertyAttributeConfigurationConvention`1" /> class.
    /// </summary>
    protected PropertyAttributeConfigurationConvention()
    {
      this.Types().Configure((Action<ConventionTypeConfiguration>) (ec =>
      {
        foreach (PropertyInfo instanceProperty in ec.ClrType.GetInstanceProperties())
        {
          IList<Attribute> attributes = (IList<Attribute>) this._attributeProvider.GetAttributes(instanceProperty);
          for (int index = 0; index < attributes.Count; ++index)
          {
            TAttribute attribute = attributes[index] as TAttribute;
            if ((object) attribute != null)
              this.Apply(instanceProperty, ec, attribute);
          }
        }
      }));
    }

    /// <summary>
    /// Applies this convention to a property that has an attribute of type TAttribute applied.
    /// </summary>
    /// <param name="memberInfo">The member info for the property that has the attribute.</param>
    /// <param name="configuration">The configuration for the class that contains the property.</param>
    /// <param name="attribute">The attribute.</param>
    public abstract void Apply(
      PropertyInfo memberInfo,
      ConventionTypeConfiguration configuration,
      TAttribute attribute);
  }
}
