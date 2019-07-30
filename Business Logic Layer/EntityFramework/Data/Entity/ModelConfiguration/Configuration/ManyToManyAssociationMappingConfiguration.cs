// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.ManyToManyAssociationMappingConfiguration
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Configuration
{
  /// <summary>
  /// Configures the table and column mapping of a many:many relationship.
  /// This configuration functionality is available via the Code First Fluent API, see <see cref="T:System.Data.Entity.DbModelBuilder" />.
  /// </summary>
  public sealed class ManyToManyAssociationMappingConfiguration : AssociationMappingConfiguration
  {
    private readonly List<string> _leftKeyColumnNames = new List<string>();
    private readonly List<string> _rightKeyColumnNames = new List<string>();
    private readonly IDictionary<string, object> _annotations = (IDictionary<string, object>) new Dictionary<string, object>();
    private DatabaseName _tableName;

    internal ManyToManyAssociationMappingConfiguration()
    {
    }

    private ManyToManyAssociationMappingConfiguration(
      ManyToManyAssociationMappingConfiguration source)
    {
      this._leftKeyColumnNames.AddRange((IEnumerable<string>) source._leftKeyColumnNames);
      this._rightKeyColumnNames.AddRange((IEnumerable<string>) source._rightKeyColumnNames);
      this._tableName = source._tableName;
      foreach (KeyValuePair<string, object> annotation in (IEnumerable<KeyValuePair<string, object>>) source._annotations)
        this._annotations.Add(annotation);
    }

    internal override AssociationMappingConfiguration Clone()
    {
      return (AssociationMappingConfiguration) new ManyToManyAssociationMappingConfiguration(this);
    }

    /// <summary>Configures the join table name for the relationship.</summary>
    /// <param name="tableName"> Name of the table. </param>
    /// <returns> The same ManyToManyAssociationMappingConfiguration instance so that multiple calls can be chained. </returns>
    public ManyToManyAssociationMappingConfiguration ToTable(
      string tableName)
    {
      Check.NotEmpty(tableName, nameof (tableName));
      return this.ToTable(tableName, (string) null);
    }

    /// <summary>
    /// Configures the join table name and schema for the relationship.
    /// </summary>
    /// <param name="tableName"> Name of the table. </param>
    /// <param name="schemaName"> Schema of the table. </param>
    /// <returns> The same ManyToManyAssociationMappingConfiguration instance so that multiple calls can be chained. </returns>
    public ManyToManyAssociationMappingConfiguration ToTable(
      string tableName,
      string schemaName)
    {
      Check.NotEmpty(tableName, nameof (tableName));
      this._tableName = new DatabaseName(tableName, schemaName);
      return this;
    }

    /// <summary>
    /// Sets an annotation in the model for the join table. The annotation value can later be used when
    /// processing the table such as when creating migrations.
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
    ///             <returns>The same configuration instance so that multiple calls can be chained.</returns>
    public ManyToManyAssociationMappingConfiguration HasTableAnnotation(
      string name,
      object value)
    {
      Check.NotEmpty(name, nameof (name));
      if (!name.IsValidUndottedName())
        throw new ArgumentException(Strings.BadAnnotationName((object) name));
      this._annotations[name] = value;
      return this;
    }

    /// <summary>
    /// Configures the name of the column(s) for the left foreign key.
    /// The left foreign key points to the parent entity of the navigation property specified in the HasMany call.
    /// </summary>
    /// <param name="keyColumnNames"> The foreign key column names. When using multiple foreign key properties, the properties must be specified in the same order that the the primary key properties were configured for the target entity type. </param>
    /// <returns> The same ManyToManyAssociationMappingConfiguration instance so that multiple calls can be chained. </returns>
    public ManyToManyAssociationMappingConfiguration MapLeftKey(
      params string[] keyColumnNames)
    {
      Check.NotNull<string[]>(keyColumnNames, nameof (keyColumnNames));
      this._leftKeyColumnNames.Clear();
      this._leftKeyColumnNames.AddRange((IEnumerable<string>) keyColumnNames);
      return this;
    }

    /// <summary>
    /// Configures the name of the column(s) for the right foreign key.
    /// The right foreign key points to the parent entity of the the navigation property specified in the WithMany call.
    /// </summary>
    /// <param name="keyColumnNames"> The foreign key column names. When using multiple foreign key properties, the properties must be specified in the same order that the the primary key properties were configured for the target entity type. </param>
    /// <returns> The same ManyToManyAssociationMappingConfiguration instance so that multiple calls can be chained. </returns>
    public ManyToManyAssociationMappingConfiguration MapRightKey(
      params string[] keyColumnNames)
    {
      Check.NotNull<string[]>(keyColumnNames, nameof (keyColumnNames));
      this._rightKeyColumnNames.Clear();
      this._rightKeyColumnNames.AddRange((IEnumerable<string>) keyColumnNames);
      return this;
    }

    internal override void Configure(
      AssociationSetMapping associationSetMapping,
      EdmModel database,
      PropertyInfo navigationProperty)
    {
      EntityType table = associationSetMapping.Table;
      if (this._tableName != null)
      {
        table.SetTableName(this._tableName);
        table.SetConfiguration((object) this);
      }
      bool flag = navigationProperty.IsSameAs(associationSetMapping.SourceEndMapping.AssociationEnd.GetClrPropertyInfo());
      ManyToManyAssociationMappingConfiguration.ConfigureColumnNames(flag ? (ICollection<string>) this._leftKeyColumnNames : (ICollection<string>) this._rightKeyColumnNames, (IList<ScalarPropertyMapping>) associationSetMapping.SourceEndMapping.PropertyMappings.ToList<ScalarPropertyMapping>());
      ManyToManyAssociationMappingConfiguration.ConfigureColumnNames(flag ? (ICollection<string>) this._rightKeyColumnNames : (ICollection<string>) this._leftKeyColumnNames, (IList<ScalarPropertyMapping>) associationSetMapping.TargetEndMapping.PropertyMappings.ToList<ScalarPropertyMapping>());
      foreach (KeyValuePair<string, object> annotation in (IEnumerable<KeyValuePair<string, object>>) this._annotations)
        table.AddAnnotation("http://schemas.microsoft.com/ado/2013/11/edm/customannotation:" + annotation.Key, annotation.Value);
    }

    private static void ConfigureColumnNames(
      ICollection<string> keyColumnNames,
      IList<ScalarPropertyMapping> propertyMappings)
    {
      if (keyColumnNames.Count > 0 && keyColumnNames.Count != propertyMappings.Count)
        throw Error.IncorrectColumnCount((object) string.Join(", ", (IEnumerable<string>) keyColumnNames));
      keyColumnNames.Each<string>((Action<string, int>) ((n, i) => propertyMappings[i].Column.Name = n));
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override string ToString()
    {
      return base.ToString();
    }

    /// <summary>Determines whether the specified object is equal to the current object.</summary>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    /// <param name="other">The object to compare with the current object.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool Equals(ManyToManyAssociationMappingConfiguration other)
    {
      if (object.ReferenceEquals((object) null, (object) other))
        return false;
      if (object.ReferenceEquals((object) this, (object) other))
        return true;
      if (!object.Equals((object) other._tableName, (object) this._tableName) || !object.Equals((object) other._tableName, (object) this._tableName) || (!this._leftKeyColumnNames.SequenceEqual<string>((IEnumerable<string>) other._leftKeyColumnNames) || !this._rightKeyColumnNames.SequenceEqual<string>((IEnumerable<string>) other._rightKeyColumnNames)) && (!this._leftKeyColumnNames.SequenceEqual<string>((IEnumerable<string>) other._rightKeyColumnNames) || !this._rightKeyColumnNames.SequenceEqual<string>((IEnumerable<string>) other._leftKeyColumnNames)))
        return false;
      return this._annotations.OrderBy<KeyValuePair<string, object>, string>((Func<KeyValuePair<string, object>, string>) (a => a.Key)).SequenceEqual<KeyValuePair<string, object>>((IEnumerable<KeyValuePair<string, object>>) other._annotations.OrderBy<KeyValuePair<string, object>, string>((Func<KeyValuePair<string, object>, string>) (a => a.Key)));
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj)
    {
      if (object.ReferenceEquals((object) null, obj))
        return false;
      if (object.ReferenceEquals((object) this, obj))
        return true;
      if (obj.GetType() != typeof (ManyToManyAssociationMappingConfiguration))
        return false;
      return this.Equals((ManyToManyAssociationMappingConfiguration) obj);
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode()
    {
      return this._annotations.OrderBy<KeyValuePair<string, object>, string>((Func<KeyValuePair<string, object>, string>) (a => a.Key)).Aggregate<KeyValuePair<string, object>, int>(this._rightKeyColumnNames.Aggregate<string, int>(this._leftKeyColumnNames.Aggregate<string, int>((this._tableName != null ? this._tableName.GetHashCode() : 0) * 397, (Func<int, string, int>) ((h, v) => h * 397 ^ v.GetHashCode())), (Func<int, string, int>) ((h, v) => h * 397 ^ v.GetHashCode())), (Func<int, KeyValuePair<string, object>, int>) ((h, v) => h * 397 ^ v.GetHashCode()));
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
