// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.StringColumnConfiguration
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.ModelConfiguration.Configuration
{
  /// <summary>
  /// Configures a database column used to store a string values.
  /// This configuration functionality is available via the Code First Fluent API, see <see cref="T:System.Data.Entity.DbModelBuilder" />.
  /// </summary>
  public class StringColumnConfiguration : LengthColumnConfiguration
  {
    internal StringColumnConfiguration(System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.StringPropertyConfiguration configuration)
      : base((System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.LengthPropertyConfiguration) configuration)
    {
    }

    internal System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.StringPropertyConfiguration Configuration
    {
      get
      {
        return (System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.StringPropertyConfiguration) base.Configuration;
      }
    }

    /// <summary>
    /// Configures the column to allow the maximum length supported by the database provider.
    /// </summary>
    /// <returns> The same StringColumnConfiguration instance so that multiple calls can be chained. </returns>
    public StringColumnConfiguration IsMaxLength()
    {
      base.IsMaxLength();
      return this;
    }

    /// <summary>
    /// Configures the property to have the specified maximum length.
    /// </summary>
    /// <param name="value">
    /// The maximum length for the property. Setting 'null' will result in a default length being used for the column.
    /// </param>
    /// <returns> The same StringColumnConfiguration instance so that multiple calls can be chained. </returns>
    public StringColumnConfiguration HasMaxLength(int? value)
    {
      base.HasMaxLength(value);
      return this;
    }

    /// <summary>
    /// Configures the column to be fixed length.
    /// Use HasMaxLength to set the length that the property is fixed to.
    /// </summary>
    /// <returns> The same StringColumnConfiguration instance so that multiple calls can be chained. </returns>
    public StringColumnConfiguration IsFixedLength()
    {
      base.IsFixedLength();
      return this;
    }

    /// <summary>
    /// Configures the column to be variable length.
    /// Columns are variable length by default.
    /// </summary>
    /// <returns> The same StringColumnConfiguration instance so that multiple calls can be chained. </returns>
    public StringColumnConfiguration IsVariableLength()
    {
      base.IsVariableLength();
      return this;
    }

    /// <summary>Configures the column to be optional.</summary>
    /// <returns> The same StringColumnConfiguration instance so that multiple calls can be chained. </returns>
    public StringColumnConfiguration IsOptional()
    {
      base.IsOptional();
      return this;
    }

    /// <summary>Configures the column to be required.</summary>
    /// <returns> The same StringColumnConfiguration instance so that multiple calls can be chained. </returns>
    public StringColumnConfiguration IsRequired()
    {
      base.IsRequired();
      return this;
    }

    /// <summary>Configures the data type of the database column.</summary>
    /// <param name="columnType"> Name of the database provider specific data type. </param>
    /// <returns> The same StringColumnConfiguration instance so that multiple calls can be chained. </returns>
    public StringColumnConfiguration HasColumnType(string columnType)
    {
      base.HasColumnType(columnType);
      return this;
    }

    /// <summary>Configures the order of the database column.</summary>
    /// <param name="columnOrder"> The order that this column should appear in the database table. </param>
    /// <returns> The same StringColumnConfiguration instance so that multiple calls can be chained. </returns>
    public StringColumnConfiguration HasColumnOrder(int? columnOrder)
    {
      base.HasColumnOrder(columnOrder);
      return this;
    }

    /// <summary>
    /// Configures the column to support Unicode string content.
    /// </summary>
    /// <returns> The same StringColumnConfiguration instance so that multiple calls can be chained. </returns>
    public StringColumnConfiguration IsUnicode()
    {
      this.IsUnicode(new bool?(true));
      return this;
    }

    /// <summary>
    /// Configures whether or not the column supports Unicode string content.
    /// </summary>
    /// <param name="unicode"> Value indicating if the column supports Unicode string content or not. Specifying 'null' will remove the Unicode facet from the column. Specifying 'null' will cause the same runtime behavior as specifying 'false'. </param>
    /// <returns> The same StringColumnConfiguration instance so that multiple calls can be chained. </returns>
    public StringColumnConfiguration IsUnicode(bool? unicode)
    {
      this.Configuration.IsUnicode = unicode;
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

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public new Type GetType()
    {
      return base.GetType();
    }
  }
}
