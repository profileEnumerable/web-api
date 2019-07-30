// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.DecimalPropertyConvention
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  /// <summary>
  /// Convention to set precision to 18 and scale to 2 for decimal properties.
  /// </summary>
  public class DecimalPropertyConvention : IConceptualModelConvention<EdmProperty>, IConvention
  {
    private readonly byte _precision;
    private readonly byte _scale;

    /// <summary>
    /// Initializes a new instance of <see cref="T:System.Data.Entity.ModelConfiguration.Conventions.DecimalPropertyConvention" /> with the default precision and scale.
    /// </summary>
    public DecimalPropertyConvention()
      : this((byte) 18, (byte) 2)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="T:System.Data.Entity.ModelConfiguration.Conventions.DecimalPropertyConvention" /> with the specified precision and scale.
    /// </summary>
    /// <param name="precision"> Precision </param>
    /// <param name="scale"> Scale </param>
    public DecimalPropertyConvention(byte precision, byte scale)
    {
      this._precision = precision;
      this._scale = scale;
    }

    /// <inheritdoc />
    public virtual void Apply(EdmProperty item, DbModel model)
    {
      Check.NotNull<EdmProperty>(item, nameof (item));
      Check.NotNull<DbModel>(model, nameof (model));
      if (item.PrimitiveType != PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.Decimal))
        return;
      byte? precision = item.Precision;
      if (!(precision.HasValue ? new int?((int) precision.GetValueOrDefault()) : new int?()).HasValue)
        item.Precision = new byte?(this._precision);
      byte? scale = item.Scale;
      if ((scale.HasValue ? new int?((int) scale.GetValueOrDefault()) : new int?()).HasValue)
        return;
      item.Scale = new byte?(this._scale);
    }
  }
}
