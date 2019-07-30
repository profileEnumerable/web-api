// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.ColumnAttributeConvention
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  /// <summary>
  /// Convention to process instances of <see cref="T:System.ComponentModel.DataAnnotations.Schema.ColumnAttribute" /> found on properties in the model
  /// </summary>
  public class ColumnAttributeConvention : PrimitivePropertyAttributeConfigurationConvention<ColumnAttribute>
  {
    /// <inheritdoc />
    public override void Apply(
      ConventionPrimitivePropertyConfiguration configuration,
      ColumnAttribute attribute)
    {
      Check.NotNull<ConventionPrimitivePropertyConfiguration>(configuration, nameof (configuration));
      Check.NotNull<ColumnAttribute>(attribute, nameof (attribute));
      if (!string.IsNullOrWhiteSpace(attribute.Name))
        configuration.HasColumnName(attribute.Name);
      if (!string.IsNullOrWhiteSpace(attribute.TypeName))
        configuration.HasColumnType(attribute.TypeName);
      if (attribute.Order < 0)
        return;
      configuration.HasColumnOrder(attribute.Order);
    }
  }
}
