// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.RequiredNavigationPropertyAttributeConvention
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.ModelConfiguration.Configuration.Properties;
using System.Data.Entity.ModelConfiguration.Configuration.Properties.Navigation;
using System.Data.Entity.ModelConfiguration.Utilities;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  /// <summary>
  /// Convention to process instances of <see cref="T:System.ComponentModel.DataAnnotations.RequiredAttribute" /> found on navigation properties in the model.
  /// </summary>
  public class RequiredNavigationPropertyAttributeConvention : Convention
  {
    private readonly AttributeProvider _attributeProvider = DbConfiguration.DependencyResolver.GetService<AttributeProvider>();

    internal override void ApplyPropertyConfiguration(
      PropertyInfo propertyInfo,
      Func<PropertyConfiguration> propertyConfiguration,
      System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration modelConfiguration)
    {
      if (!propertyInfo.IsValidEdmNavigationProperty() || propertyInfo.PropertyType.IsCollection() || !this._attributeProvider.GetAttributes(propertyInfo).OfType<RequiredAttribute>().Any<RequiredAttribute>())
        return;
      NavigationPropertyConfiguration propertyConfiguration1 = (NavigationPropertyConfiguration) propertyConfiguration();
      if (propertyConfiguration1.RelationshipMultiplicity.HasValue)
        return;
      propertyConfiguration1.RelationshipMultiplicity = new RelationshipMultiplicity?(RelationshipMultiplicity.One);
    }
  }
}
