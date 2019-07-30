// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.TypeAttributeConfigurationConvention`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  /// <summary>
  /// Base class for conventions that process CLR attributes found in the model.
  /// </summary>
  /// <typeparam name="TAttribute"> The type of the attribute to look for. </typeparam>
  [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
  public abstract class TypeAttributeConfigurationConvention<TAttribute> : Convention where TAttribute : Attribute
  {
    private readonly AttributeProvider _attributeProvider = DbConfiguration.DependencyResolver.GetService<AttributeProvider>();

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.ModelConfiguration.Conventions.TypeAttributeConfigurationConvention`1" /> class.
    /// </summary>
    protected TypeAttributeConfigurationConvention()
    {
      this.Types().Having<IEnumerable<TAttribute>>((Func<Type, IEnumerable<TAttribute>>) (t => this._attributeProvider.GetAttributes(t).OfType<TAttribute>())).Configure((Action<ConventionTypeConfiguration, IEnumerable<TAttribute>>) ((configuration, attributes) =>
      {
        foreach (TAttribute attribute in attributes)
          this.Apply(configuration, attribute);
      }));
    }

    /// <summary>
    /// Applies this convention to a class that has an attribute of type TAttribute applied.
    /// </summary>
    /// <param name="configuration">The configuration for the class that contains the property.</param>
    /// <param name="attribute">The attribute.</param>
    public abstract void Apply(ConventionTypeConfiguration configuration, TAttribute attribute);
  }
}
