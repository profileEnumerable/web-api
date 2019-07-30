// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.ForeignKeyPrimitivePropertyAttributeConvention
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Configuration.Properties.Navigation;
using System.Data.Entity.ModelConfiguration.Mappers;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  /// <summary>
  /// Convention to process instances of <see cref="T:System.ComponentModel.DataAnnotations.Schema.ForeignKeyAttribute" /> found on foreign key properties in the model.
  /// </summary>
  public class ForeignKeyPrimitivePropertyAttributeConvention : PropertyAttributeConfigurationConvention<ForeignKeyAttribute>
  {
    /// <inheritdoc />
    public override void Apply(
      PropertyInfo memberInfo,
      ConventionTypeConfiguration configuration,
      ForeignKeyAttribute attribute)
    {
      Check.NotNull<PropertyInfo>(memberInfo, nameof (memberInfo));
      Check.NotNull<ConventionTypeConfiguration>(configuration, nameof (configuration));
      Check.NotNull<ForeignKeyAttribute>(attribute, nameof (attribute));
      if (!memberInfo.IsValidEdmScalarProperty())
        return;
      PropertyInfo propertyInfo = new PropertyFilter(DbModelBuilderVersion.Latest).GetProperties(configuration.ClrType, false, (IEnumerable<PropertyInfo>) null, (IEnumerable<Type>) null, false).Where<PropertyInfo>((Func<PropertyInfo, bool>) (pi => pi.Name.Equals(attribute.Name, StringComparison.Ordinal))).SingleOrDefault<PropertyInfo>();
      if (propertyInfo == (PropertyInfo) null)
        throw Error.ForeignKeyAttributeConvention_InvalidNavigationProperty((object) memberInfo.Name, (object) configuration.ClrType, (object) attribute.Name);
      configuration.NavigationProperty(propertyInfo).HasConstraint<ForeignKeyConstraintConfiguration>((Action<ForeignKeyConstraintConfiguration>) (fk => fk.AddColumn(memberInfo)));
    }
  }
}
