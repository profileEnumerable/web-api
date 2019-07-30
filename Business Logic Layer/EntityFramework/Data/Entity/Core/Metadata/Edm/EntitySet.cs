// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.EntitySet
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Threading;

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>
  /// Represents a particular usage of a structure defined in EntityType. In the conceptual-model, this represents a set that can
  /// query and persist entities. In the store-model it represents a table.
  /// From a store-space model-convention it can be used to configure
  /// table name with <see cref="P:System.Data.Entity.Core.Metadata.Edm.EntitySetBase.Table" /> property and table schema with <see cref="P:System.Data.Entity.Core.Metadata.Edm.EntitySetBase.Schema" /> property.
  /// </summary>
  public class EntitySet : EntitySetBase
  {
    private ReadOnlyCollection<Tuple<AssociationSet, ReferentialConstraint>> _foreignKeyDependents;
    private ReadOnlyCollection<Tuple<AssociationSet, ReferentialConstraint>> _foreignKeyPrincipals;
    private volatile bool _hasForeignKeyRelationships;
    private volatile bool _hasIndependentRelationships;

    internal EntitySet()
    {
    }

    internal EntitySet(
      string name,
      string schema,
      string table,
      string definingQuery,
      EntityType entityType)
      : base(name, schema, table, definingQuery, (EntityTypeBase) entityType)
    {
    }

    /// <summary>
    /// Gets the built-in type kind for this <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntitySet" />.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.BuiltInTypeKind" /> object that represents the built-in type kind for this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntitySet" />
    /// .
    /// </returns>
    public override BuiltInTypeKind BuiltInTypeKind
    {
      get
      {
        return BuiltInTypeKind.EntitySet;
      }
    }

    /// <summary>
    /// Gets the entity type of this <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntitySet" />.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityType" /> object that represents the entity type of this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntitySet" />
    /// .
    /// </returns>
    public virtual EntityType ElementType
    {
      get
      {
        return (EntityType) base.ElementType;
      }
    }

    internal ReadOnlyCollection<Tuple<AssociationSet, ReferentialConstraint>> ForeignKeyDependents
    {
      get
      {
        if (this._foreignKeyDependents == null)
          this.InitializeForeignKeyLists();
        return this._foreignKeyDependents;
      }
    }

    internal ReadOnlyCollection<Tuple<AssociationSet, ReferentialConstraint>> ForeignKeyPrincipals
    {
      get
      {
        if (this._foreignKeyPrincipals == null)
          this.InitializeForeignKeyLists();
        return this._foreignKeyPrincipals;
      }
    }

    internal bool HasForeignKeyRelationships
    {
      get
      {
        if (this._foreignKeyPrincipals == null)
          this.InitializeForeignKeyLists();
        return this._hasForeignKeyRelationships;
      }
    }

    internal bool HasIndependentRelationships
    {
      get
      {
        if (this._foreignKeyPrincipals == null)
          this.InitializeForeignKeyLists();
        return this._hasIndependentRelationships;
      }
    }

    private void InitializeForeignKeyLists()
    {
      List<Tuple<AssociationSet, ReferentialConstraint>> tupleList1 = new List<Tuple<AssociationSet, ReferentialConstraint>>();
      List<Tuple<AssociationSet, ReferentialConstraint>> tupleList2 = new List<Tuple<AssociationSet, ReferentialConstraint>>();
      bool flag1 = false;
      bool flag2 = false;
      foreach (AssociationSet associationsForEntity in MetadataHelper.GetAssociationsForEntitySet((EntitySetBase) this))
      {
        if (associationsForEntity.ElementType.IsForeignKey)
        {
          flag1 = true;
          ReferentialConstraint referentialConstraint = associationsForEntity.ElementType.ReferentialConstraints[0];
          if (referentialConstraint.ToRole.GetEntityType().IsAssignableFrom((EdmType) this.ElementType) || this.ElementType.IsAssignableFrom((EdmType) referentialConstraint.ToRole.GetEntityType()))
            tupleList1.Add(new Tuple<AssociationSet, ReferentialConstraint>(associationsForEntity, referentialConstraint));
          if (referentialConstraint.FromRole.GetEntityType().IsAssignableFrom((EdmType) this.ElementType) || this.ElementType.IsAssignableFrom((EdmType) referentialConstraint.FromRole.GetEntityType()))
            tupleList2.Add(new Tuple<AssociationSet, ReferentialConstraint>(associationsForEntity, referentialConstraint));
        }
        else
          flag2 = true;
      }
      this._hasForeignKeyRelationships = flag1;
      this._hasIndependentRelationships = flag2;
      ReadOnlyCollection<Tuple<AssociationSet, ReferentialConstraint>> readOnlyCollection1 = new ReadOnlyCollection<Tuple<AssociationSet, ReferentialConstraint>>((IList<Tuple<AssociationSet, ReferentialConstraint>>) tupleList1);
      ReadOnlyCollection<Tuple<AssociationSet, ReferentialConstraint>> readOnlyCollection2 = new ReadOnlyCollection<Tuple<AssociationSet, ReferentialConstraint>>((IList<Tuple<AssociationSet, ReferentialConstraint>>) tupleList2);
      Interlocked.CompareExchange<ReadOnlyCollection<Tuple<AssociationSet, ReferentialConstraint>>>(ref this._foreignKeyDependents, readOnlyCollection1, (ReadOnlyCollection<Tuple<AssociationSet, ReferentialConstraint>>) null);
      Interlocked.CompareExchange<ReadOnlyCollection<Tuple<AssociationSet, ReferentialConstraint>>>(ref this._foreignKeyPrincipals, readOnlyCollection2, (ReadOnlyCollection<Tuple<AssociationSet, ReferentialConstraint>>) null);
    }

    /// <summary>
    /// The factory method for constructing the EntitySet object.
    /// </summary>
    /// <param name="name">The name of the EntitySet.</param>
    /// <param name="schema">The db schema. Can be null.</param>
    /// <param name="table">The db table. Can be null.</param>
    /// <param name="definingQuery">
    /// The provider specific query that should be used to retrieve data for this EntitySet. Can be null.
    /// </param>
    /// <param name="entityType">The entity type of the entities that this entity set type contains.</param>
    /// <param name="metadataProperties">
    /// Metadata properties that will be added to the newly created EntitySet. Can be null.
    /// </param>
    /// <returns>The EntitySet object.</returns>
    /// <exception cref="T:System.ArgumentException">Thrown if the name argument is null or empty string.</exception>
    /// <remarks>The newly created EntitySet will be read only.</remarks>
    public static EntitySet Create(
      string name,
      string schema,
      string table,
      string definingQuery,
      EntityType entityType,
      IEnumerable<MetadataProperty> metadataProperties)
    {
      Check.NotEmpty(name, nameof (name));
      Check.NotNull<EntityType>(entityType, nameof (entityType));
      EntitySet entitySet = new EntitySet(name, schema, table, definingQuery, entityType);
      if (metadataProperties != null)
        entitySet.AddMetadataProperties(metadataProperties.ToList<MetadataProperty>());
      entitySet.SetReadOnly();
      return entitySet;
    }
  }
}
