// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.SqlCePropertyMaxLengthConvention
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  /// <summary>
  /// Convention to set a default maximum length of 4000 for properties whose type supports length facets when SqlCe is the provider.
  /// </summary>
  public class SqlCePropertyMaxLengthConvention : IConceptualModelConvention<EntityType>, IConceptualModelConvention<ComplexType>, IConvention
  {
    private const int DefaultLength = 4000;
    private readonly int _length;

    /// <summary>
    /// Initializes a new instance of <see cref="T:System.Data.Entity.ModelConfiguration.Conventions.SqlCePropertyMaxLengthConvention" /> with the default length.
    /// </summary>
    public SqlCePropertyMaxLengthConvention()
      : this(4000)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="T:System.Data.Entity.ModelConfiguration.Conventions.SqlCePropertyMaxLengthConvention" /> with the specified length.
    /// </summary>
    /// <param name="length">The default maximum length for properties.</param>
    public SqlCePropertyMaxLengthConvention(int length)
    {
      if (length <= 0)
        throw new ArgumentOutOfRangeException(nameof (length), Strings.InvalidMaxLengthSize);
      this._length = length;
    }

    /// <inheritdoc />
    public virtual void Apply(EntityType item, DbModel model)
    {
      Check.NotNull<EntityType>(item, nameof (item));
      Check.NotNull<DbModel>(model, nameof (model));
      DbProviderInfo providerInfo = model.ProviderInfo;
      if (providerInfo == null || !providerInfo.IsSqlCe())
        return;
      this.SetLength((IEnumerable<EdmProperty>) item.DeclaredProperties);
    }

    /// <inheritdoc />
    public virtual void Apply(ComplexType item, DbModel model)
    {
      Check.NotNull<ComplexType>(item, nameof (item));
      Check.NotNull<DbModel>(model, nameof (model));
      DbProviderInfo providerInfo = model.ProviderInfo;
      if (providerInfo == null || !providerInfo.IsSqlCe())
        return;
      this.SetLength((IEnumerable<EdmProperty>) item.Properties);
    }

    private void SetLength(IEnumerable<EdmProperty> properties)
    {
      foreach (EdmProperty property in properties)
      {
        if (property.IsPrimitiveType && (property.PrimitiveType == PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.String) || property.PrimitiveType == PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.Binary)))
          this.SetDefaults(property);
      }
    }

    private void SetDefaults(EdmProperty property)
    {
      if (property.MaxLength.HasValue || property.IsMaxLength)
        return;
      property.MaxLength = new int?(this._length);
    }
  }
}
