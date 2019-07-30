// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.InversePropertyAttributeConvention
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Mappers;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  /// <summary>
  /// Convention to process instances of <see cref="T:System.ComponentModel.DataAnnotations.Schema.InversePropertyAttribute" /> found on properties in the model.
  /// </summary>
  public class InversePropertyAttributeConvention : PropertyAttributeConfigurationConvention<InversePropertyAttribute>
  {
    /// <inheritdoc />
    public override void Apply(
      PropertyInfo memberInfo,
      ConventionTypeConfiguration configuration,
      InversePropertyAttribute attribute)
    {
      Check.NotNull<PropertyInfo>(memberInfo, nameof (memberInfo));
      Check.NotNull<ConventionTypeConfiguration>(configuration, nameof (configuration));
      Check.NotNull<InversePropertyAttribute>(attribute, nameof (attribute));
      if (!memberInfo.IsValidEdmNavigationProperty())
        return;
      Type targetType = memberInfo.PropertyType.GetTargetType();
      PropertyInfo inverseNavigationProperty = new PropertyFilter(DbModelBuilderVersion.Latest).GetProperties(targetType, false, (IEnumerable<PropertyInfo>) null, (IEnumerable<Type>) null, false).SingleOrDefault<PropertyInfo>((Func<PropertyInfo, bool>) (p => string.Equals(p.Name, attribute.Property, StringComparison.OrdinalIgnoreCase)));
      if (inverseNavigationProperty == (PropertyInfo) null)
        throw Error.InversePropertyAttributeConvention_PropertyNotFound((object) attribute.Property, (object) targetType, (object) memberInfo.Name, (object) memberInfo.ReflectedType);
      if (memberInfo == inverseNavigationProperty)
        throw Error.InversePropertyAttributeConvention_SelfInverseDetected((object) memberInfo.Name, (object) memberInfo.ReflectedType);
      configuration.NavigationProperty(memberInfo).HasInverseNavigationProperty((Func<PropertyInfo, PropertyInfo>) (p => inverseNavigationProperty));
    }
  }
}
