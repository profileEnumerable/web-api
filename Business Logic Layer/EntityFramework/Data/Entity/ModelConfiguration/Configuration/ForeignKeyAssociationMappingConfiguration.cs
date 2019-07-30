// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.ForeignKeyAssociationMappingConfiguration
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
  /// Configures the table and column mapping of a relationship that does not expose foreign key properties in the object model.
  /// This configuration functionality is available via the Code First Fluent API, see <see cref="T:System.Data.Entity.DbModelBuilder" />.
  /// </summary>
  public sealed class ForeignKeyAssociationMappingConfiguration : AssociationMappingConfiguration
  {
    private readonly List<string> _keyColumnNames = new List<string>();
    private readonly IDictionary<Tuple<string, string>, object> _annotations = (IDictionary<Tuple<string, string>, object>) new Dictionary<Tuple<string, string>, object>();
    private DatabaseName _tableName;

    internal ForeignKeyAssociationMappingConfiguration()
    {
    }

    private ForeignKeyAssociationMappingConfiguration(
      ForeignKeyAssociationMappingConfiguration source)
    {
      this._keyColumnNames.AddRange((IEnumerable<string>) source._keyColumnNames);
      this._tableName = source._tableName;
      foreach (KeyValuePair<Tuple<string, string>, object> annotation in (IEnumerable<KeyValuePair<Tuple<string, string>, object>>) source._annotations)
        this._annotations.Add(annotation);
    }

    internal override AssociationMappingConfiguration Clone()
    {
      return (AssociationMappingConfiguration) new ForeignKeyAssociationMappingConfiguration(this);
    }

    /// <summary>
    /// Configures the name of the column(s) for the foreign key.
    /// </summary>
    /// <param name="keyColumnNames"> The foreign key column names. When using multiple foreign key properties, the properties must be specified in the same order that the the primary key properties were configured for the target entity type. </param>
    /// <returns> The same ForeignKeyAssociationMappingConfiguration instance so that multiple calls can be chained. </returns>
    public ForeignKeyAssociationMappingConfiguration MapKey(
      params string[] keyColumnNames)
    {
      Check.NotNull<string[]>(keyColumnNames, nameof (keyColumnNames));
      this._keyColumnNames.Clear();
      this._keyColumnNames.AddRange((IEnumerable<string>) keyColumnNames);
      return this;
    }

    /// <summary>
    /// Sets an annotation in the model for a database column that has been configured with <see cref="M:System.Data.Entity.ModelConfiguration.Configuration.ForeignKeyAssociationMappingConfiguration.MapKey(System.String[])" />.
    /// The annotation value can later be used when processing the column such as when creating migrations.
    /// </summary>
    /// <remarks>
    /// It will likely be necessary to register a <see cref="T:System.Data.Entity.Infrastructure.IMetadataAnnotationSerializer" /> if the type of
    /// the annotation value is anything other than a string. Passing a null value clears any annotation with
    /// the given name on the column that had been previously set.
    /// </remarks>
    /// <param name="keyColumnName">The name of the column that was configured with the HasKey method.</param>
    /// <param name="annotationName">The annotation name, which must be a valid C#/EDM identifier.</param>
    /// <param name="value">The annotation value, which may be a string or some other type that
    /// can be serialized with an <see cref="T:System.Data.Entity.Infrastructure.IMetadataAnnotationSerializer" /></param>
    /// .
    ///             <returns>The same ForeignKeyAssociationMappingConfiguration instance so that multiple calls can be chained.</returns>
    public ForeignKeyAssociationMappingConfiguration HasColumnAnnotation(
      string keyColumnName,
      string annotationName,
      object value)
    {
      Check.NotEmpty(keyColumnName, nameof (keyColumnName));
      Check.NotEmpty(annotationName, nameof (annotationName));
      this._annotations[Tuple.Create<string, string>(keyColumnName, annotationName)] = value;
      return this;
    }

    /// <summary>
    /// Configures the table name that the foreign key column(s) reside in.
    /// The table that is specified must already be mapped for the entity type.
    /// If you want the foreign key(s) to reside in their own table then use the Map method
    /// on <see cref="T:System.Data.Entity.ModelConfiguration.EntityTypeConfiguration" /> to perform
    /// entity splitting to create the table with just the primary key property. Foreign keys can
    /// then be added to the table via this method.
    /// </summary>
    /// <param name="tableName"> Name of the table. </param>
    /// <returns> The same ForeignKeyAssociationMappingConfiguration instance so that multiple calls can be chained. </returns>
    public ForeignKeyAssociationMappingConfiguration ToTable(
      string tableName)
    {
      Check.NotEmpty(tableName, nameof (tableName));
      return this.ToTable(tableName, (string) null);
    }

    /// <summary>
    /// Configures the table name and schema that the foreign key column(s) reside in.
    /// The table that is specified must already be mapped for the entity type.
    /// If you want the foreign key(s) to reside in their own table then use the Map method
    /// on <see cref="T:System.Data.Entity.ModelConfiguration.EntityTypeConfiguration" /> to perform
    /// entity splitting to create the table with just the primary key property. Foreign keys can
    /// then be added to the table via this method.
    /// </summary>
    /// <param name="tableName"> Name of the table. </param>
    /// <param name="schemaName"> Schema of the table. </param>
    /// <returns> The same ForeignKeyAssociationMappingConfiguration instance so that multiple calls can be chained. </returns>
    public ForeignKeyAssociationMappingConfiguration ToTable(
      string tableName,
      string schemaName)
    {
      Check.NotEmpty(tableName, nameof (tableName));
      this._tableName = new DatabaseName(tableName, schemaName);
      return this;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    internal override void Configure(
      AssociationSetMapping associationSetMapping,
      EdmModel database,
      PropertyInfo navigationProperty)
    {
      List<ScalarPropertyMapping> propertyMappings = associationSetMapping.SourceEndMapping.PropertyMappings.ToList<ScalarPropertyMapping>();
      if (this._tableName != null)
      {
        EntityType targetTable = database.EntityTypes.Select(t => new
        {
          t = t,
          n = t.GetTableName()
        }).Where(_param1 =>
        {
          if (_param1.n != null)
            return _param1.n.Equals(this._tableName);
          return false;
        }).Select(_param0 => _param0.t).SingleOrDefault<EntityType>() ?? database.GetEntitySets().Where<EntitySet>((Func<EntitySet, bool>) (es => string.Equals(es.Table, this._tableName.Name, StringComparison.Ordinal))).Select<EntitySet, EntityType>((Func<EntitySet, EntityType>) (es => es.ElementType)).SingleOrDefault<EntityType>();
        if (targetTable == null)
          throw Error.TableNotFound((object) this._tableName);
        EntityType sourceTable = associationSetMapping.Table;
        if (sourceTable != targetTable)
        {
          ForeignKeyBuilder foreignKeyBuilder = sourceTable.ForeignKeyBuilders.Single<ForeignKeyBuilder>((Func<ForeignKeyBuilder, bool>) (fk => fk.DependentColumns.SequenceEqual<EdmProperty>(propertyMappings.Select<ScalarPropertyMapping, EdmProperty>((Func<ScalarPropertyMapping, EdmProperty>) (pm => pm.Column)))));
          sourceTable.RemoveForeignKey(foreignKeyBuilder);
          targetTable.AddForeignKey(foreignKeyBuilder);
          foreignKeyBuilder.DependentColumns.Each<EdmProperty>((Action<EdmProperty>) (c =>
          {
            bool primaryKeyColumn = c.IsPrimaryKeyColumn;
            sourceTable.RemoveMember((EdmMember) c);
            targetTable.AddMember((EdmMember) c);
            if (!primaryKeyColumn)
              return;
            targetTable.AddKeyMember((EdmMember) c);
          }));
          associationSetMapping.StoreEntitySet = database.GetEntitySet(targetTable);
        }
      }
      if (this._keyColumnNames.Count > 0 && this._keyColumnNames.Count != propertyMappings.Count<ScalarPropertyMapping>())
        throw Error.IncorrectColumnCount((object) string.Join(", ", (IEnumerable<string>) this._keyColumnNames));
      this._keyColumnNames.Each<string>((Action<string, int>) ((n, i) => propertyMappings[i].Column.Name = n));
      foreach (KeyValuePair<Tuple<string, string>, object> annotation in (IEnumerable<KeyValuePair<Tuple<string, string>, object>>) this._annotations)
      {
        int index = this._keyColumnNames.IndexOf(annotation.Key.Item1);
        if (index == -1)
          throw new InvalidOperationException(Strings.BadKeyNameForAnnotation((object) annotation.Key.Item1, (object) annotation.Key.Item2));
        propertyMappings[index].Column.AddAnnotation("http://schemas.microsoft.com/ado/2013/11/edm/customannotation:" + annotation.Key.Item2, annotation.Value);
      }
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override string ToString()
    {
      return base.ToString();
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool Equals(ForeignKeyAssociationMappingConfiguration other)
    {
      if (object.ReferenceEquals((object) null, (object) other))
        return false;
      if (object.ReferenceEquals((object) this, (object) other))
        return true;
      if (object.Equals((object) other._tableName, (object) this._tableName) && other._keyColumnNames.SequenceEqual<string>((IEnumerable<string>) this._keyColumnNames))
        return other._annotations.OrderBy<KeyValuePair<Tuple<string, string>, object>, Tuple<string, string>>((Func<KeyValuePair<Tuple<string, string>, object>, Tuple<string, string>>) (a => a.Key)).SequenceEqual<KeyValuePair<Tuple<string, string>, object>>((IEnumerable<KeyValuePair<Tuple<string, string>, object>>) this._annotations.OrderBy<KeyValuePair<Tuple<string, string>, object>, Tuple<string, string>>((Func<KeyValuePair<Tuple<string, string>, object>, Tuple<string, string>>) (a => a.Key)));
      return false;
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj)
    {
      if (object.ReferenceEquals((object) null, obj))
        return false;
      if (object.ReferenceEquals((object) this, obj))
        return true;
      if (obj.GetType() != typeof (ForeignKeyAssociationMappingConfiguration))
        return false;
      return this.Equals((ForeignKeyAssociationMappingConfiguration) obj);
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode()
    {
      return this._annotations.OrderBy<KeyValuePair<Tuple<string, string>, object>, Tuple<string, string>>((Func<KeyValuePair<Tuple<string, string>, object>, Tuple<string, string>>) (a => a.Key)).Aggregate<KeyValuePair<Tuple<string, string>, object>, int>(this._keyColumnNames.Aggregate<string, int>((this._tableName != null ? this._tableName.GetHashCode() : 0) * 397, (Func<int, string, int>) ((h, v) => h * 397 ^ v.GetHashCode())), (Func<int, KeyValuePair<Tuple<string, string>, object>, int>) ((h, v) => h * 397 ^ v.GetHashCode()));
    }

    /// <inheritdoc />
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new Type GetType()
    {
      return base.GetType();
    }
  }
}
