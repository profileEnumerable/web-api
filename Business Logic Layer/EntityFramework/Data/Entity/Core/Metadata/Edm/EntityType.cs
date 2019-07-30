// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.EntityType
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>
  /// Represents the structure of an <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntitySet" />. In the conceptual-model this represents the shape and structure
  /// of an entity. In the store model this represents the structure of a table. To change the Schema and Table name use EntitySet.
  /// </summary>
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance")]
  public class EntityType : EntityTypeBase
  {
    private readonly List<ForeignKeyBuilder> _foreignKeyBuilders = new List<ForeignKeyBuilder>();
    private readonly object _navigationPropertiesCacheLock = new object();
    private ReadOnlyMetadataCollection<EdmProperty> _properties;
    private RefType _referenceType;
    private RowType _keyRow;
    private ReadOnlyMetadataCollection<NavigationProperty> _navigationPropertiesCache;

    internal EntityType(string name, string namespaceName, DataSpace dataSpace)
      : base(name, namespaceName, dataSpace)
    {
    }

    internal EntityType(
      string name,
      string namespaceName,
      DataSpace dataSpace,
      IEnumerable<string> keyMemberNames,
      IEnumerable<EdmMember> members)
      : base(name, namespaceName, dataSpace)
    {
      if (members != null)
        EntityTypeBase.CheckAndAddMembers(members, this);
      if (keyMemberNames == null)
        return;
      this.CheckAndAddKeyMembers(keyMemberNames);
    }

    internal IEnumerable<ForeignKeyBuilder> ForeignKeyBuilders
    {
      get
      {
        return (IEnumerable<ForeignKeyBuilder>) this._foreignKeyBuilders;
      }
    }

    internal void RemoveForeignKey(ForeignKeyBuilder foreignKeyBuilder)
    {
      Util.ThrowIfReadOnly((MetadataItem) this);
      foreignKeyBuilder.SetOwner((EntityType) null);
      this._foreignKeyBuilders.Remove(foreignKeyBuilder);
    }

    internal void AddForeignKey(ForeignKeyBuilder foreignKeyBuilder)
    {
      Util.ThrowIfReadOnly((MetadataItem) this);
      foreignKeyBuilder.SetOwner(this);
      this._foreignKeyBuilders.Add(foreignKeyBuilder);
    }

    /// <summary>
    /// Gets the built-in type kind for this <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityType" />.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.BuiltInTypeKind" /> object that represents the built-in type kind for this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityType" />
    /// .
    /// </returns>
    public override BuiltInTypeKind BuiltInTypeKind
    {
      get
      {
        return BuiltInTypeKind.EntityType;
      }
    }

    internal override void ValidateMemberForAdd(EdmMember member)
    {
    }

    /// <summary>Gets the declared navigation properties associated with the entity type.</summary>
    /// <returns>The declared navigation properties associated with the entity type.</returns>
    public ReadOnlyMetadataCollection<NavigationProperty> DeclaredNavigationProperties
    {
      get
      {
        return this.GetDeclaredOnlyMembers<NavigationProperty>();
      }
    }

    /// <summary>
    /// Gets the navigation properties of this <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityType" />.
    /// </summary>
    /// <returns>
    /// A collection of type <see cref="T:System.Data.Entity.Core.Metadata.Edm.ReadOnlyMetadataCollection`1" /> that contains the list of navigation properties on this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityType" />
    /// .
    /// </returns>
    public ReadOnlyMetadataCollection<NavigationProperty> NavigationProperties
    {
      get
      {
        ReadOnlyMetadataCollection<NavigationProperty> navigationPropertiesCache = this._navigationPropertiesCache;
        if (navigationPropertiesCache == null)
        {
          lock (this._navigationPropertiesCacheLock)
          {
            if (this._navigationPropertiesCache == null)
            {
              this.Members.SourceAccessed += new EventHandler(this.ResetNavigationProperties);
              this._navigationPropertiesCache = (ReadOnlyMetadataCollection<NavigationProperty>) new FilteredReadOnlyMetadataCollection<NavigationProperty, EdmMember>(this.Members, new Predicate<EdmMember>(Helper.IsNavigationProperty));
            }
            navigationPropertiesCache = this._navigationPropertiesCache;
          }
        }
        return navigationPropertiesCache;
      }
    }

    private void ResetNavigationProperties(object sender, EventArgs e)
    {
      if (this._navigationPropertiesCache == null)
        return;
      lock (this._navigationPropertiesCacheLock)
      {
        if (this._navigationPropertiesCache == null)
          return;
        this._navigationPropertiesCache = (ReadOnlyMetadataCollection<NavigationProperty>) null;
        this.Members.SourceAccessed -= new EventHandler(this.ResetNavigationProperties);
      }
    }

    /// <summary>Gets the list of declared properties for the entity type.</summary>
    /// <returns>The declared properties for the entity type.</returns>
    public ReadOnlyMetadataCollection<EdmProperty> DeclaredProperties
    {
      get
      {
        return this.GetDeclaredOnlyMembers<EdmProperty>();
      }
    }

    /// <summary>Gets the collection of declared members for the entity type.</summary>
    /// <returns>The collection of declared members for the entity type.</returns>
    public ReadOnlyMetadataCollection<EdmMember> DeclaredMembers
    {
      get
      {
        return this.GetDeclaredOnlyMembers<EdmMember>();
      }
    }

    /// <summary>
    /// Gets the list of properties for this <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityType" />.
    /// </summary>
    /// <returns>
    /// A collection of type <see cref="T:System.Data.Entity.Core.Metadata.Edm.ReadOnlyMetadataCollection`1" /> that contains the list of properties for this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityType" />
    /// .
    /// </returns>
    public virtual ReadOnlyMetadataCollection<EdmProperty> Properties
    {
      get
      {
        if (!this.IsReadOnly)
          return (ReadOnlyMetadataCollection<EdmProperty>) new FilteredReadOnlyMetadataCollection<EdmProperty, EdmMember>(this.Members, new Predicate<EdmMember>(Helper.IsEdmProperty));
        if (this._properties == null)
          Interlocked.CompareExchange<ReadOnlyMetadataCollection<EdmProperty>>(ref this._properties, (ReadOnlyMetadataCollection<EdmProperty>) new FilteredReadOnlyMetadataCollection<EdmProperty, EdmMember>(this.Members, new Predicate<EdmMember>(Helper.IsEdmProperty)), (ReadOnlyMetadataCollection<EdmProperty>) null);
        return this._properties;
      }
    }

    /// <summary>
    /// Returns a <see cref="T:System.Data.Entity.Core.Metadata.Edm.RefType" /> object that references this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityType" />
    /// .
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.RefType" /> object that references this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityType" />
    /// .
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public RefType GetReferenceType()
    {
      if (this._referenceType == null)
        Interlocked.CompareExchange<RefType>(ref this._referenceType, new RefType(this), (RefType) null);
      return this._referenceType;
    }

    internal RowType GetKeyRowType()
    {
      if (this._keyRow == null)
      {
        List<EdmProperty> edmPropertyList = new List<EdmProperty>(this.KeyMembers.Count);
        edmPropertyList.AddRange(this.KeyMembers.Select<EdmMember, EdmProperty>((Func<EdmMember, EdmProperty>) (keyMember => new EdmProperty(keyMember.Name, Helper.GetModelTypeUsage(keyMember)))));
        Interlocked.CompareExchange<RowType>(ref this._keyRow, new RowType((IEnumerable<EdmProperty>) edmPropertyList), (RowType) null);
      }
      return this._keyRow;
    }

    internal bool TryGetNavigationProperty(
      string relationshipType,
      string fromName,
      string toName,
      out NavigationProperty navigationProperty)
    {
      foreach (NavigationProperty navigationProperty1 in this.NavigationProperties)
      {
        if (navigationProperty1.RelationshipType.FullName == relationshipType && navigationProperty1.FromEndMember.Name == fromName && navigationProperty1.ToEndMember.Name == toName)
        {
          navigationProperty = navigationProperty1;
          return true;
        }
      }
      navigationProperty = (NavigationProperty) null;
      return false;
    }

    /// <summary>
    /// The factory method for constructing the EntityType object.
    /// </summary>
    /// <param name="name">The name of the entity type.</param>
    /// <param name="namespaceName">The namespace of the entity type.</param>
    /// <param name="dataSpace">The dataspace in which the EntityType belongs to.</param>
    /// <param name="keyMemberNames">Name of key members for the type.</param>
    /// <param name="members">Members of the entity type (primitive and navigation properties).</param>
    /// <param name="metadataProperties">Metadata properties to be associated with the instance.</param>
    /// <returns>The EntityType object.</returns>
    /// <exception cref="T:System.ArgumentException">Thrown if either name, namespace arguments are null.</exception>
    /// <remarks>The newly created EntityType will be read only.</remarks>
    public static EntityType Create(
      string name,
      string namespaceName,
      DataSpace dataSpace,
      IEnumerable<string> keyMemberNames,
      IEnumerable<EdmMember> members,
      IEnumerable<MetadataProperty> metadataProperties)
    {
      Check.NotEmpty(name, nameof (name));
      Check.NotEmpty(namespaceName, nameof (namespaceName));
      EntityType entityType = new EntityType(name, namespaceName, dataSpace, keyMemberNames, members);
      if (metadataProperties != null)
        entityType.AddMetadataProperties(metadataProperties.ToList<MetadataProperty>());
      entityType.SetReadOnly();
      return entityType;
    }

    /// <summary>
    /// The factory method for constructing the EntityType object.
    /// </summary>
    /// <param name="name">The name of the entity type.</param>
    /// <param name="namespaceName">The namespace of the entity type.</param>
    /// <param name="dataSpace">The dataspace in which the EntityType belongs to.</param>
    /// <param name="baseType">The base type.</param>
    /// <param name="keyMemberNames">Name of key members for the type.</param>
    /// <param name="members">Members of the entity type (primitive and navigation properties).</param>
    /// <param name="metadataProperties">Metadata properties to be associated with the instance.</param>
    /// <returns>The EntityType object.</returns>
    /// <exception cref="T:System.ArgumentException">Thrown if either name, namespace arguments are null.</exception>
    /// <remarks>The newly created EntityType will be read only.</remarks>
    public static EntityType Create(
      string name,
      string namespaceName,
      DataSpace dataSpace,
      EntityType baseType,
      IEnumerable<string> keyMemberNames,
      IEnumerable<EdmMember> members,
      IEnumerable<MetadataProperty> metadataProperties)
    {
      Check.NotEmpty(name, nameof (name));
      Check.NotEmpty(namespaceName, nameof (namespaceName));
      Check.NotNull<EntityType>(baseType, nameof (baseType));
      EntityType entityType1 = new EntityType(name, namespaceName, dataSpace, keyMemberNames, members);
      entityType1.BaseType = (EdmType) baseType;
      EntityType entityType2 = entityType1;
      if (metadataProperties != null)
        entityType2.AddMetadataProperties(metadataProperties.ToList<MetadataProperty>());
      entityType2.SetReadOnly();
      return entityType2;
    }

    /// <summary>
    /// Adds the specified navigation property to the members of this type.
    /// The navigation property is added regardless of the read-only flag.
    /// </summary>
    /// <param name="property">The navigation property to be added.</param>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    public void AddNavigationProperty(NavigationProperty property)
    {
      Check.NotNull<NavigationProperty>(property, nameof (property));
      this.AddMember((EdmMember) property, true);
    }
  }
}
