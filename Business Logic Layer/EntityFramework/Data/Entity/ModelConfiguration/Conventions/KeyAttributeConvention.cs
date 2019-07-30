// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.KeyAttributeConvention
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.ModelConfiguration.Configuration.Types;
using System.Data.Entity.ModelConfiguration.Utilities;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  /// <summary>
  /// Convention to process instances of <see cref="T:System.ComponentModel.DataAnnotations.KeyAttribute" /> found on properties in the model.
  /// </summary>
  public class KeyAttributeConvention : Convention
  {
    private readonly AttributeProvider _attributeProvider = DbConfiguration.DependencyResolver.GetService<AttributeProvider>();

    internal override void ApplyPropertyTypeConfiguration<TStructuralTypeConfiguration>(
      PropertyInfo propertyInfo,
      Func<TStructuralTypeConfiguration> structuralTypeConfiguration,
      System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration modelConfiguration)
    {
      if (!(typeof (TStructuralTypeConfiguration) == typeof (EntityTypeConfiguration)) || !this._attributeProvider.GetAttributes(propertyInfo).OfType<KeyAttribute>().Any<KeyAttribute>())
        return;
      EntityTypeConfiguration typeConfiguration = (EntityTypeConfiguration) (object) structuralTypeConfiguration();
      if (!propertyInfo.IsValidEdmScalarProperty())
        return;
      typeConfiguration.Key(propertyInfo);
    }
  }
}
