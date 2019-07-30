// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.EntitySetBase
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>Class for representing a entity set</summary>
  public abstract class EntitySetBase : MetadataItem, INamedDataModelItem
  {
    private EntityContainer _entityContainer;
    private string _name;
    private EntityTypeBase _elementType;
    private string _table;
    private string _schema;
    private string _definingQuery;

    internal EntitySetBase()
    {
    }

    internal EntitySetBase(
      string name,
      string schema,
      string table,
      string definingQuery,
      EntityTypeBase entityType)
    {
      Check.NotNull<EntityTypeBase>(entityType, nameof (entityType));
      Check.NotEmpty(name, nameof (name));
      this._name = name;
      this._schema = schema;
      this._table = table;
      this._definingQuery = definingQuery;
      this.ElementType = entityType;
    }

    /// <summary>
    /// Gets the built-in type kind for this <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntitySetBase" />.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.BuiltInTypeKind" /> object that represents the built-in type kind for this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntitySetBase" />
    /// .
    /// </returns>
    public override BuiltInTypeKind BuiltInTypeKind
    {
      get
      {
        return BuiltInTypeKind.EntitySetBase;
      }
    }

    string INamedDataModelItem.Identity
    {
      get
      {
        return this.Identity;
      }
    }

    internal override string Identity
    {
      get
      {
        return this.Name;
      }
    }

    /// <summary>
    /// Gets escaped provider specific SQL describing this entity set.
    /// </summary>
    [MetadataProperty(PrimitiveTypeKind.String, false)]
    public string DefiningQuery
    {
      get
      {
        return this._definingQuery;
      }
      internal set
      {
        Check.NotEmpty(value, nameof (value));
        Util.ThrowIfReadOnly((MetadataItem) this);
        this._definingQuery = value;
      }
    }

    /// <summary>
    /// Gets or sets the name of the current entity or relationship set.
    /// If this property is changed from store-space, the mapping layer must also be updated to reflect the new name.
    /// To change the table name of a store space <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntitySet" /> use the Table property.
    /// </summary>
    /// <returns>The name of the current entity or relationship set.</returns>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the setter is called when EntitySetBase instance is in ReadOnly state</exception>
    [MetadataProperty(PrimitiveTypeKind.String, false)]
    public virtual string Name
    {
      get
      {
        return this._name;
      }
      set
      {
        Check.NotEmpty(value, nameof (value));
        Util.ThrowIfReadOnly((MetadataItem) this);
        if (string.Equals(this._name, value, StringComparison.Ordinal))
          return;
        string identity = this.Identity;
        this._name = value;
        if (this._entityContainer == null)
          return;
        this._entityContainer.NotifyItemIdentityChanged(this, identity);
      }
    }

    /// <summary>Gets the entity container of the current entity or relationship set.</summary>
    /// <returns>
    /// An <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityContainer" /> object that represents the entity container of the current entity or relationship set.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the setter is called when the EntitySetBase instance or the EntityContainer passed into the setter is in ReadOnly state</exception>
    public virtual EntityContainer EntityContainer
    {
      get
      {
        return this._entityContainer;
      }
    }

    /// <summary>
    /// Gets the entity type of this <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityTypeBase" />.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityTypeBase" /> object that represents the entity type of this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityTypeBase" />
    /// .
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the setter is called when EntitySetBase instance is in ReadOnly state</exception>
    [MetadataProperty(BuiltInTypeKind.EntityTypeBase, false)]
    public EntityTypeBase ElementType
    {
      get
      {
        return this._elementType;
      }
      internal set
      {
        Check.NotNull<EntityTypeBase>(value, nameof (value));
        Util.ThrowIfReadOnly((MetadataItem) this);
        this._elementType = value;
      }
    }

    /// <summary>
    /// Gets or sets the database table name for this entity set.
    /// </summary>
    /// <exception cref="T:System.ArgumentNullException">if value passed into setter is null</exception>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the setter is called when EntitySetBase instance is in ReadOnly state</exception>
    [MetadataProperty(PrimitiveTypeKind.String, false)]
    public string Table
    {
      get
      {
        return this._table;
      }
      set
      {
        Util.ThrowIfReadOnly((MetadataItem) this);
        this._table = value;
      }
    }

    /// <summary>Gets or sets the database schema for this entity set.</summary>
    /// <exception cref="T:System.ArgumentNullException">if value passed into setter is null</exception>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the setter is called when EntitySetBase instance is in ReadOnly state</exception>
    [MetadataProperty(PrimitiveTypeKind.String, false)]
    public string Schema
    {
      get
      {
        return this._schema;
      }
      set
      {
        Util.ThrowIfReadOnly((MetadataItem) this);
        this._schema = value;
      }
    }

    /// <summary>Returns the name of the current entity or relationship set.</summary>
    /// <returns>The name of the current entity or relationship set.</returns>
    public override string ToString()
    {
      return this.Name;
    }

    internal override void SetReadOnly()
    {
      if (this.IsReadOnly)
        return;
      base.SetReadOnly();
      this.ElementType?.SetReadOnly();
    }

    internal void ChangeEntityContainerWithoutCollectionFixup(EntityContainer newEntityContainer)
    {
      this._entityContainer = newEntityContainer;
    }
  }
}
