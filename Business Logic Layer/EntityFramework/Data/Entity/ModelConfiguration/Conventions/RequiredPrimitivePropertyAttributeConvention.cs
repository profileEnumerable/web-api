// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.RequiredPrimitivePropertyAttributeConvention
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  /// <summary>
  /// Convention to process instances of <see cref="T:System.ComponentModel.DataAnnotations.RequiredAttribute" /> found on primitive properties in the model.
  /// </summary>
  public class RequiredPrimitivePropertyAttributeConvention : PrimitivePropertyAttributeConfigurationConvention<RequiredAttribute>
  {
    /// <inheritdoc />
    public override void Apply(
      ConventionPrimitivePropertyConfiguration configuration,
      RequiredAttribute attribute)
    {
      Check.NotNull<ConventionPrimitivePropertyConfiguration>(configuration, nameof (configuration));
      Check.NotNull<RequiredAttribute>(attribute, nameof (attribute));
      configuration.IsRequired();
    }
  }
}
