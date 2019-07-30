// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.PrimitiveColumnConfiguration
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.ModelConfiguration.Configuration
{
  /// <summary>Configures a primitive column from an entity type.</summary>
  public class PrimitiveColumnConfiguration
  {
    private readonly System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration _configuration;

    internal PrimitiveColumnConfiguration(System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration configuration)
    {
      this._configuration = configuration;
    }

    internal System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration Configuration
    {
      get
      {
        return this._configuration;
      }
    }

    /// <summary>Configures the primitive column to be optional.</summary>
    /// <returns>The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.PrimitiveColumnConfiguration" /> instance so that multiple calls can be chained.</returns>
    public PrimitiveColumnConfiguration IsOptional()
    {
      this.Configuration.IsNullable = new bool?(true);
      return this;
    }

    /// <summary>Configures the primitive column to be required.</summary>
    /// <returns>The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.PrimitiveColumnConfiguration" /> instance so that multiple calls can be chained.</returns>
    public PrimitiveColumnConfiguration IsRequired()
    {
      this.Configuration.IsNullable = new bool?(false);
      return this;
    }

    /// <summary>Configures the data type of the primitive column used to store the property.</summary>
    /// <returns>The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.PrimitiveColumnConfiguration" /> instance so that multiple calls can be chained.</returns>
    /// <param name="columnType">The name of the database provider specific data type.</param>
    public PrimitiveColumnConfiguration HasColumnType(string columnType)
    {
      this.Configuration.ColumnType = columnType;
      return this;
    }

    /// <summary>Configures the order of the primitive column used to store the property. This method is also used to specify key ordering when an entity type has a composite key.</summary>
    /// <returns>The same <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.PrimitiveColumnConfiguration" /> instance so that multiple calls can be chained.</returns>
    /// <param name="columnOrder">The order that this column should appear in the database table.</param>
    public PrimitiveColumnConfiguration HasColumnOrder(int? columnOrder)
    {
      if (columnOrder.HasValue && columnOrder.Value < 0)
        throw new ArgumentOutOfRangeException(nameof (columnOrder));
      this.Configuration.ColumnOrder = columnOrder;
      return this;
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override string ToString()
    {
      return base.ToString();
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj)
    {
      return base.Equals(obj);
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    /// <summary>
    /// Gets the <see cref="T:System.Type" /> of the current instance.
    /// </summary>
    /// <returns>The exact runtime type of the current instance.</returns>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new Type GetType()
    {
      return base.GetType();
    }
  }
}
