// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.PropertyMappingConfiguration
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;

namespace System.Data.Entity.ModelConfiguration.Configuration
{
  /// <summary>
  /// Used to configure a property in a mapping fragment.
  /// This configuration functionality is available via the Code First Fluent API, see <see cref="T:System.Data.Entity.DbModelBuilder" />.
  /// </summary>
  public class PropertyMappingConfiguration
  {
    private readonly System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration _configuration;

    internal PropertyMappingConfiguration(System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration configuration)
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

    /// <summary>
    /// Configures the name of the database column used to store the property, in a mapping fragment.
    /// </summary>
    /// <param name="columnName"> The name of the column. </param>
    /// <returns> The same PropertyMappingConfiguration instance so that multiple calls can be chained. </returns>
    public PropertyMappingConfiguration HasColumnName(string columnName)
    {
      this.Configuration.ColumnName = columnName;
      return this;
    }

    /// <summary>
    /// Sets an annotation in the model for the database column used to store the property. The annotation
    /// value can later be used when processing the column such as when creating migrations.
    /// </summary>
    /// <remarks>
    /// It will likely be necessary to register a <see cref="T:System.Data.Entity.Infrastructure.IMetadataAnnotationSerializer" /> if the type of
    /// the annotation value is anything other than a string. Passing a null value clears any annotation with
    /// the given name on the column that had been previously set.
    /// </remarks>
    /// <param name="name">The annotation name, which must be a valid C#/EDM identifier.</param>
    /// <param name="value">The annotation value, which may be a string or some other type that
    /// can be serialized with an <see cref="T:System.Data.Entity.Infrastructure.IMetadataAnnotationSerializer" /></param>
    /// .
    ///             <returns>The same PropertyMappingConfiguration instance so that multiple calls can be chained.</returns>
    public PropertyMappingConfiguration HasColumnAnnotation(
      string name,
      object value)
    {
      Check.NotEmpty(name, nameof (name));
      this.Configuration.SetAnnotation(name, value);
      return this;
    }
  }
}
