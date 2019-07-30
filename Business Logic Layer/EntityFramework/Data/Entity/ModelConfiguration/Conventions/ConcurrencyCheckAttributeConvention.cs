// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.ConcurrencyCheckAttributeConvention
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  /// <summary>
  /// Convention to process instances of <see cref="T:System.ComponentModel.DataAnnotations.ConcurrencyCheckAttribute" /> found on properties in the model.
  /// </summary>
  public class ConcurrencyCheckAttributeConvention : PrimitivePropertyAttributeConfigurationConvention<ConcurrencyCheckAttribute>
  {
    /// <inheritdoc />
    public override void Apply(
      ConventionPrimitivePropertyConfiguration configuration,
      ConcurrencyCheckAttribute attribute)
    {
      Check.NotNull<ConventionPrimitivePropertyConfiguration>(configuration, nameof (configuration));
      Check.NotNull<ConcurrencyCheckAttribute>(attribute, nameof (attribute));
      configuration.IsConcurrencyToken();
    }
  }
}
