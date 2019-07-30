// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.EntityContainer
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>Class for representing an entity container</summary>
  public class EntityContainer : GlobalItem
  {
    private readonly object _baseEntitySetsLock = new object();
    private string _name;
    private readonly ReadOnlyMetadataCollection<EntitySetBase> _baseEntitySets;
    private readonly ReadOnlyMetadataCollection<EdmFunction> _functionImports;
    private ReadOnlyMetadataCollection<AssociationSet> _associationSetsCache;
    private ReadOnlyMetadataCollection<EntitySet> _entitySetsCache;

    internal EntityContainer()
    {
    }

    /// <summary>
    /// Creates an entity container with the specified name and data space.
    /// </summary>
    /// <param name="name">The entity container name.</param>
    /// <param name="dataSpace">The entity container data space.</param>
    /// <exception cref="T:System.ArgumentNullException">Thrown if the name argument is null.</exception>
    /// <exception cref="T:System.ArgumentException">Thrown if the name argument is empty string.</exception>
    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public EntityContainer(string name, DataSpace dataSpace)
    {
      Check.NotEmpty(name, nameof (name));
      this._name = name;
      this.DataSpace = dataSpace;
      this._baseEntitySets = new ReadOnlyMetadataCollection<EntitySetBase>((MetadataCollection<EntitySetBase>) new EntitySetBaseCollection(this));
      this._functionImports = new ReadOnlyMetadataCollection<EdmFunction>(new MetadataCollection<EdmFunction>());
    }

    /// <summary>
    /// Gets the built-in type kind for this <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityContainer" />.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.BuiltInTypeKind" /> object that represents the built-in type kind for this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityContainer" />
    /// .
    /// </returns>
    public override BuiltInTypeKind BuiltInTypeKind
    {
      get
      {
        return BuiltInTypeKind.EntityContainer;
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
    /// Gets the name of this <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityContainer" />.
    /// </summary>
    /// <returns>
    /// The name of this <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityContainer" />.
    /// </returns>
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
        this._name = value;
      }
    }

    /// <summary>
    /// Gets a list of entity sets and association sets that this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityContainer" />
    /// includes.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.ReadOnlyMetadataCollection`1" /> object that contains a list of entity sets and association sets that this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityContainer" />
    /// includes.
    /// </returns>
    [MetadataProperty(BuiltInTypeKind.EntitySetBase, true)]
    public ReadOnlyMetadataCollection<EntitySetBase> BaseEntitySets
    {
      get
      {
        return this._baseEntitySets;
      }
    }

    /// <summary> Gets the association sets for this entity container. </summary>
    /// <returns> The association sets for this entity container .</returns>
    public ReadOnlyMetadataCollection<AssociationSet> AssociationSets
    {
      get
      {
        ReadOnlyMetadataCollection<AssociationSet> associationSetsCache = this._associationSetsCache;
        if (associationSetsCache == null)
        {
          lock (this._baseEntitySetsLock)
          {
            if (this._associationSetsCache == null)
            {
              this._baseEntitySets.SourceAccessed += new EventHandler(this.ResetAssociationSetsCache);
              this._associationSetsCache = (ReadOnlyMetadataCollection<AssociationSet>) new FilteredReadOnlyMetadataCollection<AssociationSet, EntitySetBase>(this._baseEntitySets, new Predicate<EntitySetBase>(Helper.IsAssociationSet));
            }
            associationSetsCache = this._associationSetsCache;
          }
        }
        return associationSetsCache;
      }
    }

    private void ResetAssociationSetsCache(object sender, EventArgs e)
    {
      if (this._associationSetsCache == null)
        return;
      lock (this._baseEntitySetsLock)
      {
        if (this._associationSetsCache == null)
          return;
        this._associationSetsCache = (ReadOnlyMetadataCollection<AssociationSet>) null;
        this._baseEntitySets.SourceAccessed -= new EventHandler(this.ResetAssociationSetsCache);
      }
    }

    /// <summary> Gets the entity sets for this entity container. </summary>
    /// <returns> The entity sets for this entity container .</returns>
    public ReadOnlyMetadataCollection<EntitySet> EntitySets
    {
      get
      {
        ReadOnlyMetadataCollection<EntitySet> entitySetsCache = this._entitySetsCache;
        if (entitySetsCache == null)
        {
          lock (this._baseEntitySetsLock)
          {
            if (this._entitySetsCache == null)
            {
              this._baseEntitySets.SourceAccessed += new EventHandler(this.ResetEntitySetsCache);
              this._entitySetsCache = (ReadOnlyMetadataCollection<EntitySet>) new FilteredReadOnlyMetadataCollection<EntitySet, EntitySetBase>(this._baseEntitySets, new Predicate<EntitySetBase>(Helper.IsEntitySet));
            }
            entitySetsCache = this._entitySetsCache;
          }
        }
        return entitySetsCache;
      }
    }

    private void ResetEntitySetsCache(object sender, EventArgs e)
    {
      if (this._entitySetsCache == null)
        return;
      lock (this._baseEntitySetsLock)
      {
        if (this._entitySetsCache == null)
          return;
        this._entitySetsCache = (ReadOnlyMetadataCollection<EntitySet>) null;
        this._baseEntitySets.SourceAccessed -= new EventHandler(this.ResetEntitySetsCache);
      }
    }

    /// <summary>
    /// Specifies a collection of <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmFunction" /> elements. Each function contains the details of a stored procedure that exists in the database or equivalent CommandText that is mapped to an entity and its properties.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.ReadOnlyMetadataCollection`1" /> that contains
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmFunction" />
    /// elements.
    /// </returns>
    [MetadataProperty(BuiltInTypeKind.EdmFunction, true)]
    public ReadOnlyMetadataCollection<EdmFunction> FunctionImports
    {
      get
      {
        return this._functionImports;
      }
    }

    internal override void SetReadOnly()
    {
      if (this.IsReadOnly)
        return;
      base.SetReadOnly();
      this.BaseEntitySets.Source.SetReadOnly();
      this.FunctionImports.Source.SetReadOnly();
    }

    /// <summary>
    /// Returns an <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntitySet" /> object by using the specified name for the entity set.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntitySet" /> object that represents the entity set that has the specified name.
    /// </returns>
    /// <param name="name">The name of the entity set that is searched for.</param>
    /// <param name="ignoreCase">true to perform the case-insensitive search; otherwise, false.</param>
    public EntitySet GetEntitySetByName(string name, bool ignoreCase)
    {
      EntitySet entitySet = this.BaseEntitySets.GetValue(name, ignoreCase) as EntitySet;
      if (entitySet != null)
        return entitySet;
      throw new ArgumentException(Strings.InvalidEntitySetName((object) name));
    }

    /// <summary>
    /// Returns an <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntitySet" /> object by using the specified name for the entity set.
    /// </summary>
    /// <returns>true if there is an entity set that matches the search criteria; otherwise, false.</returns>
    /// <param name="name">The name of the entity set that is searched for.</param>
    /// <param name="ignoreCase">true to perform the case-insensitive search; otherwise, false.</param>
    /// <param name="entitySet">
    /// When this method returns, contains an <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntitySet" /> object. If there is no entity set, this output parameter contains null.
    /// </param>
    public bool TryGetEntitySetByName(string name, bool ignoreCase, out EntitySet entitySet)
    {
      Check.NotNull<string>(name, nameof (name));
      EntitySetBase entitySetBase = (EntitySetBase) null;
      entitySet = (EntitySet) null;
      if (!this.BaseEntitySets.TryGetValue(name, ignoreCase, out entitySetBase) || !Helper.IsEntitySet(entitySetBase))
        return false;
      entitySet = (EntitySet) entitySetBase;
      return true;
    }

    /// <summary>
    /// Returns a <see cref="T:System.Data.Entity.Core.Metadata.Edm.RelationshipSet" /> object by using the specified name for the relationship set.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Data.Entity.Core.Metadata.Edm.RelationshipSet" /> object that represents the relationship set that has the specified name.
    /// </returns>
    /// <param name="name">The name of the relationship set that is searched for.</param>
    /// <param name="ignoreCase">true to perform the case-insensitive search; otherwise, false.</param>
    public RelationshipSet GetRelationshipSetByName(string name, bool ignoreCase)
    {
      RelationshipSet relationshipSet;
      if (!this.TryGetRelationshipSetByName(name, ignoreCase, out relationshipSet))
        throw new ArgumentException(Strings.InvalidRelationshipSetName((object) name));
      return relationshipSet;
    }

    /// <summary>
    /// Returns a <see cref="T:System.Data.Entity.Core.Metadata.Edm.RelationshipSet" /> object by using the specified name for the relationship set.
    /// </summary>
    /// <returns>true if there is a relationship set that matches the search criteria; otherwise, false. </returns>
    /// <param name="name">The name of the relationship set that is searched for.</param>
    /// <param name="ignoreCase">true to perform the case-insensitive search; otherwise, false.</param>
    /// <param name="relationshipSet">
    /// When this method returns, contains a <see cref="T:System.Data.Entity.Core.Metadata.Edm.RelationshipSet" /> object.
    /// </param>
    public bool TryGetRelationshipSetByName(
      string name,
      bool ignoreCase,
      out RelationshipSet relationshipSet)
    {
      Check.NotNull<string>(name, nameof (name));
      EntitySetBase entitySetBase = (EntitySetBase) null;
      relationshipSet = (RelationshipSet) null;
      if (!this.BaseEntitySets.TryGetValue(name, ignoreCase, out entitySetBase) || !Helper.IsRelationshipSet(entitySetBase))
        return false;
      relationshipSet = (RelationshipSet) entitySetBase;
      return true;
    }

    /// <summary>
    /// Returns the name of this <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityContainer" />.
    /// </summary>
    /// <returns>
    /// The name of this <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityContainer" />.
    /// </returns>
    public override string ToString()
    {
      return this.Name;
    }

    /// <summary>Adds the specified entity set to the container.</summary>
    /// <param name="entitySetBase">The entity set to add.</param>
    public void AddEntitySetBase(EntitySetBase entitySetBase)
    {
      Check.NotNull<EntitySetBase>(entitySetBase, nameof (entitySetBase));
      Util.ThrowIfReadOnly((MetadataItem) this);
      this._baseEntitySets.Source.Add(entitySetBase);
      entitySetBase.ChangeEntityContainerWithoutCollectionFixup(this);
    }

    /// <summary>Removes a specific entity set from the container.</summary>
    /// <param name="entitySetBase">The entity set to remove.</param>
    public void RemoveEntitySetBase(EntitySetBase entitySetBase)
    {
      Check.NotNull<EntitySetBase>(entitySetBase, nameof (entitySetBase));
      Util.ThrowIfReadOnly((MetadataItem) this);
      this._baseEntitySets.Source.Remove(entitySetBase);
      entitySetBase.ChangeEntityContainerWithoutCollectionFixup((EntityContainer) null);
    }

    /// <summary>Adds a function import to the container.</summary>
    /// <param name="function">The function import to add.</param>
    public void AddFunctionImport(EdmFunction function)
    {
      Check.NotNull<EdmFunction>(function, nameof (function));
      Util.ThrowIfReadOnly((MetadataItem) this);
      if (!function.IsFunctionImport)
        throw new ArgumentException(Strings.OnlyFunctionImportsCanBeAddedToEntityContainer((object) function.Name));
      this._functionImports.Source.Add(function);
    }

    /// <summary>
    /// The factory method for constructing the EntityContainer object.
    /// </summary>
    /// <param name="name">The name of the entity container to be created.</param>
    /// <param name="dataSpace">DataSpace in which this entity container belongs to.</param>
    /// <param name="entitySets">Entity sets that will be included in the new container. Can be null.</param>
    /// <param name="functionImports">Functions that will be included in the new container. Can be null.</param>
    /// <param name="metadataProperties">Metadata properties to be associated with the instance.</param>
    /// <returns>The EntityContainer object.</returns>
    /// <exception cref="T:System.ArgumentException">Thrown if the name argument is null or empty string.</exception>
    /// <remarks>The newly created EntityContainer will be read only.</remarks>
    public static EntityContainer Create(
      string name,
      DataSpace dataSpace,
      IEnumerable<EntitySetBase> entitySets,
      IEnumerable<EdmFunction> functionImports,
      IEnumerable<MetadataProperty> metadataProperties)
    {
      Check.NotEmpty(name, nameof (name));
      EntityContainer entityContainer = new EntityContainer(name, dataSpace);
      if (entitySets != null)
      {
        foreach (EntitySetBase entitySet in entitySets)
          entityContainer.AddEntitySetBase(entitySet);
      }
      if (functionImports != null)
      {
        foreach (EdmFunction functionImport in functionImports)
        {
          if (!functionImport.IsFunctionImport)
            throw new ArgumentException(Strings.OnlyFunctionImportsCanBeAddedToEntityContainer((object) functionImport.Name));
          entityContainer.AddFunctionImport(functionImport);
        }
      }
      if (metadataProperties != null)
        entityContainer.AddMetadataProperties(metadataProperties.ToList<MetadataProperty>());
      entityContainer.SetReadOnly();
      return entityContainer;
    }

    internal virtual void NotifyItemIdentityChanged(EntitySetBase item, string initialIdentity)
    {
      this._baseEntitySets.Source.HandleIdentityChange(item, initialIdentity);
    }
  }
}
