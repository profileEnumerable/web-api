// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.DataClasses.RelationshipManager
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects.Internal;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace System.Data.Entity.Core.Objects.DataClasses
{
  /// <summary>
  /// Container for the lazily created relationship navigation
  /// property objects (collections and refs).
  /// </summary>
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
  [Serializable]
  public class RelationshipManager
  {
    private IEntityWithRelationships _owner;
    private List<RelatedEnd> _relationships;
    [NonSerialized]
    private bool _nodeVisited;
    [NonSerialized]
    private IEntityWrapper _wrappedOwner;
    [NonSerialized]
    private EntityWrapperFactory _entityWrapperFactory;
    [NonSerialized]
    private ExpensiveOSpaceLoader _expensiveLoader;

    private RelationshipManager()
    {
      this._entityWrapperFactory = new EntityWrapperFactory();
      this._expensiveLoader = new ExpensiveOSpaceLoader();
    }

    internal RelationshipManager(ExpensiveOSpaceLoader expensiveLoader)
    {
      this._entityWrapperFactory = new EntityWrapperFactory();
      this._expensiveLoader = expensiveLoader ?? new ExpensiveOSpaceLoader();
    }

    internal void SetExpensiveLoader(ExpensiveOSpaceLoader loader)
    {
      this._expensiveLoader = loader;
    }

    internal IEnumerable<RelatedEnd> Relationships
    {
      get
      {
        this.EnsureRelationshipsInitialized();
        return (IEnumerable<RelatedEnd>) this._relationships.ToArray();
      }
    }

    private void EnsureRelationshipsInitialized()
    {
      if (this._relationships != null)
        return;
      this._relationships = new List<RelatedEnd>();
    }

    internal bool NodeVisited
    {
      get
      {
        return this._nodeVisited;
      }
      set
      {
        this._nodeVisited = value;
      }
    }

    internal IEntityWrapper WrappedOwner
    {
      get
      {
        if (this._wrappedOwner == null)
          this._wrappedOwner = EntityWrapperFactory.CreateNewWrapper((object) this._owner, (EntityKey) null);
        return this._wrappedOwner;
      }
    }

    internal virtual EntityWrapperFactory EntityWrapperFactory
    {
      get
      {
        return this._entityWrapperFactory;
      }
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Objects.DataClasses.RelationshipManager" /> object.
    /// </summary>
    /// <remarks>
    /// Used by data classes that support relationships. If the change tracker
    /// requests the RelationshipManager property and the data class does not
    /// already have a reference to one of these objects, it calls this method
    /// to create one, then saves a reference to that object. On subsequent accesses
    /// to that property, the data class should return the saved reference.
    /// The reason for using a factory method instead of a public constructor is to
    /// emphasize that this is not something you would normally call outside of a data class.
    /// By requiring that these objects are created via this method, developers should
    /// give more thought to the operation, and will generally only use it when
    /// they explicitly need to get an object of this type. It helps define the intended usage.
    /// </remarks>
    /// <returns>
    /// The requested <see cref="T:System.Data.Entity.Core.Objects.DataClasses.RelationshipManager" />.
    /// </returns>
    /// <param name="owner">Reference to the entity that is calling this method.</param>
    public static RelationshipManager Create(IEntityWithRelationships owner)
    {
      Check.NotNull<IEntityWithRelationships>(owner, nameof (owner));
      return new RelationshipManager() { _owner = owner };
    }

    internal static RelationshipManager Create()
    {
      return new RelationshipManager();
    }

    internal void SetWrappedOwner(IEntityWrapper wrappedOwner, object expectedOwner)
    {
      this._wrappedOwner = wrappedOwner;
      if (this._owner != null && !object.ReferenceEquals(expectedOwner, (object) this._owner))
        throw new InvalidOperationException(Strings.RelationshipManager_InvalidRelationshipManagerOwner);
      if (this._relationships == null)
        return;
      foreach (RelatedEnd relationship in this._relationships)
        relationship.SetWrappedOwner(wrappedOwner);
    }

    internal EntityCollection<TTargetEntity> GetRelatedCollection<TSourceEntity, TTargetEntity>(
      AssociationEndMember sourceMember,
      AssociationEndMember targetMember,
      NavigationPropertyAccessor sourceAccessor,
      NavigationPropertyAccessor targetAccessor,
      RelatedEnd existingRelatedEnd)
      where TSourceEntity : class
      where TTargetEntity : class
    {
      string fullName = sourceMember.DeclaringType.FullName;
      string name = targetMember.Name;
      RelationshipMultiplicity relationshipMultiplicity = sourceMember.RelationshipMultiplicity;
      RelatedEnd relatedEnd1;
      this.TryGetCachedRelatedEnd(fullName, name, out relatedEnd1);
      EntityCollection<TTargetEntity> previousCollection = relatedEnd1 as EntityCollection<TTargetEntity>;
      if (existingRelatedEnd == null)
      {
        if (relatedEnd1 != null)
          return previousCollection;
        return this.CreateRelatedEnd<TSourceEntity, TTargetEntity>(new RelationshipNavigation((AssociationType) sourceMember.DeclaringType, sourceMember.Name, targetMember.Name, sourceAccessor, targetAccessor), relationshipMultiplicity, RelationshipMultiplicity.Many, existingRelatedEnd) as EntityCollection<TTargetEntity>;
      }
      if (relatedEnd1 != null)
        this._relationships.Remove(relatedEnd1);
      EntityCollection<TTargetEntity> relatedEnd2 = this.CreateRelatedEnd<TSourceEntity, TTargetEntity>(new RelationshipNavigation((AssociationType) sourceMember.DeclaringType, sourceMember.Name, targetMember.Name, sourceAccessor, targetAccessor), relationshipMultiplicity, RelationshipMultiplicity.Many, existingRelatedEnd) as EntityCollection<TTargetEntity>;
      if (relatedEnd2 != null)
      {
        bool flag = true;
        try
        {
          RelationshipManager.RemergeCollections<TTargetEntity>(previousCollection, relatedEnd2);
          flag = false;
        }
        finally
        {
          if (flag && relatedEnd1 != null)
          {
            this._relationships.Remove((RelatedEnd) relatedEnd2);
            this._relationships.Add(relatedEnd1);
          }
        }
      }
      return relatedEnd2;
    }

    private static void RemergeCollections<TTargetEntity>(
      EntityCollection<TTargetEntity> previousCollection,
      EntityCollection<TTargetEntity> collection)
      where TTargetEntity : class
    {
      int num = 0;
      List<IEntityWrapper> entityWrapperList = new List<IEntityWrapper>(collection.CountInternal);
      foreach (IEntityWrapper wrappedEntity in collection.GetWrappedEntities())
        entityWrapperList.Add(wrappedEntity);
      foreach (IEntityWrapper wrappedEntity in entityWrapperList)
      {
        bool flag = true;
        if (previousCollection != null && previousCollection.ContainsEntity(wrappedEntity))
        {
          ++num;
          flag = false;
        }
        if (flag)
        {
          collection.Remove(wrappedEntity, false);
          collection.Add(wrappedEntity);
        }
      }
      if (previousCollection != null && num != previousCollection.CountInternal)
        throw new InvalidOperationException(Strings.Collections_UnableToMergeCollections);
    }

    internal EntityReference<TTargetEntity> GetRelatedReference<TSourceEntity, TTargetEntity>(
      AssociationEndMember sourceMember,
      AssociationEndMember targetMember,
      NavigationPropertyAccessor sourceAccessor,
      NavigationPropertyAccessor targetAccessor,
      RelatedEnd existingRelatedEnd)
      where TSourceEntity : class
      where TTargetEntity : class
    {
      string fullName = sourceMember.DeclaringType.FullName;
      string name = targetMember.Name;
      RelationshipMultiplicity relationshipMultiplicity = sourceMember.RelationshipMultiplicity;
      RelatedEnd relatedEnd;
      if (this.TryGetCachedRelatedEnd(fullName, name, out relatedEnd))
        return relatedEnd as EntityReference<TTargetEntity>;
      return this.CreateRelatedEnd<TSourceEntity, TTargetEntity>(new RelationshipNavigation((AssociationType) sourceMember.DeclaringType, sourceMember.Name, targetMember.Name, sourceAccessor, targetAccessor), relationshipMultiplicity, RelationshipMultiplicity.One, existingRelatedEnd) as EntityReference<TTargetEntity>;
    }

    [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
    internal RelatedEnd GetRelatedEnd(
      string navigationProperty,
      bool throwArgumentException = false)
    {
      IEntityWrapper wrappedOwner = this.WrappedOwner;
      EntityType entityType = wrappedOwner.Context.MetadataWorkspace.GetItem<EntityType>(wrappedOwner.IdentityType.FullNameWithNesting(), DataSpace.OSpace);
      EdmMember outMember;
      if (!wrappedOwner.Context.Perspective.TryGetMember((StructuralType) entityType, navigationProperty, false, out outMember) || !(outMember is NavigationProperty))
      {
        string message = Strings.RelationshipManager_NavigationPropertyNotFound((object) navigationProperty);
        throw throwArgumentException ? (SystemException) new ArgumentException(message) : (SystemException) new InvalidOperationException(message);
      }
      NavigationProperty navigationProperty1 = (NavigationProperty) outMember;
      return this.GetRelatedEndInternal(navigationProperty1.RelationshipType.FullName, navigationProperty1.ToEndMember.Name);
    }

    /// <summary>
    /// Returns either an <see cref="T:System.Data.Entity.Core.Objects.DataClasses.EntityCollection`1" /> or
    /// <see cref="T:System.Data.Entity.Core.Objects.DataClasses.EntityReference`1" />
    /// of the correct type for the specified target role in a relationship.
    /// </summary>
    /// <returns>
    /// <see cref="T:System.Data.Entity.Core.Objects.DataClasses.IRelatedEnd" /> representing the
    /// <see cref="T:System.Data.Entity.Core.Objects.DataClasses.EntityCollection`1" />
    /// or
    /// <see cref="T:System.Data.Entity.Core.Objects.DataClasses.EntityReference`1" />
    /// that was retrieved.
    /// </returns>
    /// <param name="relationshipName">Name of the relationship in which  targetRoleName  is defined. The relationship name is not namespace qualified.</param>
    /// <param name="targetRoleName">Target role to use to retrieve the other end of  relationshipName .</param>
    /// <exception cref="T:System.ArgumentNullException"> relationshipName  or  targetRoleName  is null.</exception>
    /// <exception cref="T:System.InvalidOperationException">The source type does not match the type of the owner.</exception>
    /// <exception cref="T:System.ArgumentException"> targetRoleName  is invalid or unable to find the relationship type in the metadata.</exception>
    public IRelatedEnd GetRelatedEnd(string relationshipName, string targetRoleName)
    {
      return (IRelatedEnd) this.GetRelatedEndInternal(this.PrependNamespaceToRelationshipName(relationshipName), targetRoleName);
    }

    internal RelatedEnd GetRelatedEndInternal(
      string relationshipName,
      string targetRoleName)
    {
      IEntityWrapper wrappedOwner = this.WrappedOwner;
      if (wrappedOwner.Context == null && wrappedOwner.RequiresRelationshipChangeTracking)
        throw new InvalidOperationException(Strings.RelationshipManager_CannotGetRelatEndForDetachedPocoEntity);
      AssociationType relationshipType = this.GetRelationshipType(relationshipName);
      return this.GetRelatedEndInternal(relationshipName, targetRoleName, (RelatedEnd) null, relationshipType);
    }

    private RelatedEnd GetRelatedEndInternal(
      string relationshipName,
      string targetRoleName,
      RelatedEnd existingRelatedEnd,
      AssociationType relationship)
    {
      AssociationEndMember sourceEnd;
      AssociationEndMember targetEnd;
      RelationshipManager.GetAssociationEnds(relationship, targetRoleName, out sourceEnd, out targetEnd);
      Type clrType = MetadataHelper.GetEntityTypeForEnd(sourceEnd).ClrType;
      IEntityWrapper wrappedOwner = this.WrappedOwner;
      if (!clrType.IsAssignableFrom(wrappedOwner.IdentityType))
        throw new InvalidOperationException(Strings.RelationshipManager_OwnerIsNotSourceType((object) wrappedOwner.IdentityType.FullName, (object) clrType.FullName, (object) sourceEnd.Name, (object) relationshipName));
      if (!this.VerifyRelationship(relationship, sourceEnd.Name))
        return (RelatedEnd) null;
      return DelegateFactory.GetRelatedEnd(this, sourceEnd, targetEnd, existingRelatedEnd);
    }

    internal RelatedEnd GetRelatedEndInternal(
      AssociationType csAssociationType,
      AssociationEndMember csTargetEnd)
    {
      IEntityWrapper wrappedOwner = this.WrappedOwner;
      if (wrappedOwner.Context == null && wrappedOwner.RequiresRelationshipChangeTracking)
        throw new InvalidOperationException(Strings.RelationshipManager_CannotGetRelatEndForDetachedPocoEntity);
      AssociationType relationshipType = this.GetRelationshipType(csAssociationType);
      AssociationEndMember sourceEnd;
      AssociationEndMember targetEnd;
      RelationshipManager.GetAssociationEnds(relationshipType, csTargetEnd.Name, out sourceEnd, out targetEnd);
      Type clrType = MetadataHelper.GetEntityTypeForEnd(sourceEnd).ClrType;
      if (!clrType.IsAssignableFrom(wrappedOwner.IdentityType))
        throw new InvalidOperationException(Strings.RelationshipManager_OwnerIsNotSourceType((object) wrappedOwner.IdentityType.FullName, (object) clrType.FullName, (object) sourceEnd.Name, (object) csAssociationType.FullName));
      if (!this.VerifyRelationship(relationshipType, csAssociationType, sourceEnd.Name))
        return (RelatedEnd) null;
      return DelegateFactory.GetRelatedEnd(this, sourceEnd, targetEnd, (RelatedEnd) null);
    }

    private static void GetAssociationEnds(
      AssociationType associationType,
      string targetRoleName,
      out AssociationEndMember sourceEnd,
      out AssociationEndMember targetEnd)
    {
      targetEnd = associationType.TargetEnd;
      if (targetEnd.Identity != targetRoleName)
      {
        sourceEnd = targetEnd;
        targetEnd = associationType.SourceEnd;
        if (targetEnd.Identity != targetRoleName)
          throw new InvalidOperationException(Strings.RelationshipManager_InvalidTargetRole((object) associationType.FullName, (object) targetRoleName));
      }
      else
        sourceEnd = associationType.SourceEnd;
    }

    /// <summary>
    /// Takes an existing EntityReference that was created with the default constructor and initializes it using the provided relationship and target role names.
    /// This method is designed to be used during deserialization only, and will throw an exception if the provided EntityReference has already been initialized,
    /// if the relationship manager already contains a relationship with this name and target role, or if the relationship manager is already attached to a ObjectContext.W
    /// </summary>
    /// <param name="relationshipName">The relationship name.</param>
    /// <param name="targetRoleName">The role name of the related end.</param>
    /// <param name="entityReference">
    /// The <see cref="T:System.Data.Entity.Core.Objects.DataClasses.EntityReference`1" /> to initialize.
    /// </param>
    /// <typeparam name="TTargetEntity">
    /// The type of the <see cref="T:System.Data.Entity.Core.Objects.DataClasses.EntityReference`1" /> being initialized.
    /// </typeparam>
    /// <exception cref="T:System.InvalidOperationException">
    /// When the provided <see cref="T:System.Data.Entity.Core.Objects.DataClasses.EntityReference`1" />
    /// is already initialized.-or-When the relationship manager is already attached to an
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />
    /// or when the relationship manager already contains a relationship with this name and target role.
    /// </exception>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    public void InitializeRelatedReference<TTargetEntity>(
      string relationshipName,
      string targetRoleName,
      EntityReference<TTargetEntity> entityReference)
      where TTargetEntity : class
    {
      Check.NotNull<string>(relationshipName, nameof (relationshipName));
      Check.NotNull<string>(targetRoleName, nameof (targetRoleName));
      Check.NotNull<EntityReference<TTargetEntity>>(entityReference, nameof (entityReference));
      if (entityReference.WrappedOwner.Entity != null)
        throw new InvalidOperationException(Strings.RelationshipManager_ReferenceAlreadyInitialized((object) Strings.RelationshipManager_InitializeIsForDeserialization));
      IEntityWrapper wrappedOwner = this.WrappedOwner;
      if (wrappedOwner.Context != null && wrappedOwner.MergeOption != MergeOption.NoTracking)
        throw new InvalidOperationException(Strings.RelationshipManager_RelationshipManagerAttached((object) Strings.RelationshipManager_InitializeIsForDeserialization));
      relationshipName = this.PrependNamespaceToRelationshipName(relationshipName);
      AssociationType relationshipType = this.GetRelationshipType(relationshipName);
      RelatedEnd relatedEnd;
      if (this.TryGetCachedRelatedEnd(relationshipName, targetRoleName, out relatedEnd))
      {
        if (!relatedEnd.IsEmpty())
          entityReference.InitializeWithValue(relatedEnd);
        this._relationships.Remove(relatedEnd);
      }
      if (!(this.GetRelatedEndInternal(relationshipName, targetRoleName, (RelatedEnd) entityReference, relationshipType) is EntityReference<TTargetEntity>))
        throw new InvalidOperationException(Strings.EntityReference_ExpectedReferenceGotCollection((object) typeof (TTargetEntity).Name, (object) targetRoleName, (object) relationshipName));
    }

    /// <summary>
    /// Takes an existing EntityCollection that was created with the default constructor and initializes it using the provided relationship and target role names.
    /// This method is designed to be used during deserialization only, and will throw an exception if the provided EntityCollection has already been initialized,
    /// or if the relationship manager is already attached to a ObjectContext.
    /// </summary>
    /// <param name="relationshipName">The relationship name.</param>
    /// <param name="targetRoleName">The target role name.</param>
    /// <param name="entityCollection">An existing EntityCollection.</param>
    /// <typeparam name="TTargetEntity"> Type of the entity represented by targetRoleName </typeparam>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void InitializeRelatedCollection<TTargetEntity>(
      string relationshipName,
      string targetRoleName,
      EntityCollection<TTargetEntity> entityCollection)
      where TTargetEntity : class
    {
      Check.NotNull<string>(relationshipName, nameof (relationshipName));
      Check.NotNull<string>(targetRoleName, nameof (targetRoleName));
      Check.NotNull<EntityCollection<TTargetEntity>>(entityCollection, nameof (entityCollection));
      if (entityCollection.WrappedOwner.Entity != null)
        throw new InvalidOperationException(Strings.RelationshipManager_CollectionAlreadyInitialized((object) Strings.RelationshipManager_CollectionInitializeIsForDeserialization));
      IEntityWrapper wrappedOwner = this.WrappedOwner;
      if (wrappedOwner.Context != null && wrappedOwner.MergeOption != MergeOption.NoTracking)
        throw new InvalidOperationException(Strings.RelationshipManager_CollectionRelationshipManagerAttached((object) Strings.RelationshipManager_CollectionInitializeIsForDeserialization));
      relationshipName = this.PrependNamespaceToRelationshipName(relationshipName);
      AssociationType relationshipType = this.GetRelationshipType(relationshipName);
      if (!(this.GetRelatedEndInternal(relationshipName, targetRoleName, (RelatedEnd) entityCollection, relationshipType) is EntityCollection<TTargetEntity>))
        throw new InvalidOperationException(Strings.Collections_ExpectedCollectionGotReference((object) typeof (TTargetEntity).Name, (object) targetRoleName, (object) relationshipName));
    }

    internal string PrependNamespaceToRelationshipName(string relationshipName)
    {
      if (!relationshipName.Contains("."))
      {
        AssociationType associationType;
        if (EntityProxyFactory.TryGetAssociationTypeFromProxyInfo(this.WrappedOwner, relationshipName, out associationType))
          return associationType.FullName;
        if (this._relationships != null)
        {
          string str = this._relationships.Select<RelatedEnd, string>((Func<RelatedEnd, string>) (r => r.RelationshipName)).FirstOrDefault<string>((Func<string, bool>) (n => n.Substring(n.LastIndexOf('.') + 1) == relationshipName));
          if (str != null)
            return str;
        }
        string str1 = this.WrappedOwner.IdentityType.FullNameWithNesting();
        ObjectItemCollection objectItemCollection = RelationshipManager.GetObjectItemCollection(this.WrappedOwner);
        EdmType edmType = (EdmType) null;
        if (objectItemCollection != null)
          objectItemCollection.TryGetItem<EdmType>(str1, out edmType);
        else
          this._expensiveLoader.LoadTypesExpensiveWay(this.WrappedOwner.IdentityType.Assembly())?.TryGetValue(str1, out edmType);
        ClrEntityType clrEntityType = edmType as ClrEntityType;
        if (clrEntityType != null)
          return clrEntityType.CSpaceNamespaceName + "." + relationshipName;
      }
      return relationshipName;
    }

    private static ObjectItemCollection GetObjectItemCollection(
      IEntityWrapper wrappedOwner)
    {
      if (wrappedOwner.Context != null)
        return (ObjectItemCollection) wrappedOwner.Context.MetadataWorkspace.GetItemCollection(DataSpace.OSpace);
      return (ObjectItemCollection) null;
    }

    private bool TryGetOwnerEntityType(out EntityType entityType)
    {
      DefaultObjectMappingItemCollection collection;
      MappingBase map;
      if (RelationshipManager.TryGetObjectMappingItemCollection(this.WrappedOwner, out collection) && collection.TryGetMap(this.WrappedOwner.IdentityType.FullNameWithNesting(), DataSpace.OSpace, out map))
      {
        ObjectTypeMapping objectTypeMapping = (ObjectTypeMapping) map;
        if (Helper.IsEntityType(objectTypeMapping.EdmType))
        {
          entityType = (EntityType) objectTypeMapping.EdmType;
          return true;
        }
      }
      entityType = (EntityType) null;
      return false;
    }

    private static bool TryGetObjectMappingItemCollection(
      IEntityWrapper wrappedOwner,
      out DefaultObjectMappingItemCollection collection)
    {
      if (wrappedOwner.Context != null && wrappedOwner.Context.MetadataWorkspace != null)
      {
        collection = (DefaultObjectMappingItemCollection) wrappedOwner.Context.MetadataWorkspace.GetItemCollection(DataSpace.OCSpace);
        return collection != null;
      }
      collection = (DefaultObjectMappingItemCollection) null;
      return false;
    }

    internal AssociationType GetRelationshipType(AssociationType csAssociationType)
    {
      MetadataWorkspace metadataWorkspace = this.WrappedOwner.Context.MetadataWorkspace;
      if (metadataWorkspace != null)
        return metadataWorkspace.MetadataOptimization.GetOSpaceAssociationType(csAssociationType, (Func<AssociationType>) (() => this.GetRelationshipType(csAssociationType.FullName)));
      return this.GetRelationshipType(csAssociationType.FullName);
    }

    internal AssociationType GetRelationshipType(string relationshipName)
    {
      AssociationType associationType = (AssociationType) null;
      ObjectItemCollection objectItemCollection = RelationshipManager.GetObjectItemCollection(this.WrappedOwner);
      if (objectItemCollection != null)
        associationType = objectItemCollection.GetRelationshipType(relationshipName);
      if (associationType == null)
        EntityProxyFactory.TryGetAssociationTypeFromProxyInfo(this.WrappedOwner, relationshipName, out associationType);
      if (associationType == null && this._relationships != null)
        associationType = this._relationships.Where<RelatedEnd>((Func<RelatedEnd, bool>) (e => e.RelationshipName == relationshipName)).Select<RelatedEnd, RelationshipType>((Func<RelatedEnd, RelationshipType>) (e => e.RelationMetadata)).OfType<AssociationType>().FirstOrDefault<AssociationType>();
      if (associationType == null)
        associationType = this._expensiveLoader.GetRelationshipTypeExpensiveWay(this.WrappedOwner.IdentityType, relationshipName);
      if (associationType == null)
        throw RelationshipManager.UnableToGetMetadata(this.WrappedOwner, relationshipName);
      return associationType;
    }

    internal static Exception UnableToGetMetadata(
      IEntityWrapper wrappedOwner,
      string relationshipName)
    {
      ArgumentException argumentException = new ArgumentException(Strings.RelationshipManager_UnableToFindRelationshipTypeInMetadata((object) relationshipName), nameof (relationshipName));
      if (EntityProxyFactory.IsProxyType(wrappedOwner.Entity.GetType()))
        return (Exception) new InvalidOperationException(Strings.EntityProxyTypeInfo_ProxyMetadataIsUnavailable((object) wrappedOwner.IdentityType.FullName), (Exception) argumentException);
      return (Exception) argumentException;
    }

    private static IEnumerable<AssociationEndMember> GetAllTargetEnds(
      EntityType ownerEntityType,
      EntitySet ownerEntitySet)
    {
      foreach (AssociationSet associationsForEntity in MetadataHelper.GetAssociationsForEntitySet((EntitySetBase) ownerEntitySet))
      {
        EntityType end2EntityType = associationsForEntity.ElementType.AssociationEndMembers[1].GetEntityType();
        if (end2EntityType.IsAssignableFrom((EdmType) ownerEntityType))
          yield return associationsForEntity.ElementType.AssociationEndMembers[0];
        EntityType end1EntityType = associationsForEntity.ElementType.AssociationEndMembers[0].GetEntityType();
        if (end1EntityType.IsAssignableFrom((EdmType) ownerEntityType))
          yield return associationsForEntity.ElementType.AssociationEndMembers[1];
      }
    }

    private IEnumerable<AssociationEndMember> GetAllTargetEnds(
      Type entityClrType)
    {
      ObjectItemCollection objectItemCollection = RelationshipManager.GetObjectItemCollection(this.WrappedOwner);
      IEnumerable<AssociationType> associations = (IEnumerable<AssociationType>) null;
      associations = objectItemCollection == null ? EntityProxyFactory.TryGetAllAssociationTypesFromProxyInfo(this.WrappedOwner) ?? this._expensiveLoader.GetAllRelationshipTypesExpensiveWay(entityClrType.Assembly()) : (IEnumerable<AssociationType>) objectItemCollection.GetItems<AssociationType>();
      foreach (AssociationType associationType in associations)
      {
        RefType referenceType = associationType.AssociationEndMembers[0].TypeUsage.EdmType as RefType;
        if (referenceType != null && referenceType.ElementType.ClrType.IsAssignableFrom(entityClrType))
          yield return associationType.AssociationEndMembers[1];
        referenceType = associationType.AssociationEndMembers[1].TypeUsage.EdmType as RefType;
        if (referenceType != null && referenceType.ElementType.ClrType.IsAssignableFrom(entityClrType))
          yield return associationType.AssociationEndMembers[0];
      }
    }

    private bool VerifyRelationship(AssociationType relationship, string sourceEndName)
    {
      IEntityWrapper wrappedOwner = this.WrappedOwner;
      if (wrappedOwner.Context == null)
        return true;
      EntityKey entityKey = wrappedOwner.EntityKey;
      if (entityKey == (EntityKey) null)
        return true;
      return RelationshipManager.VerifyRelationship(wrappedOwner, entityKey, relationship, sourceEndName);
    }

    private bool VerifyRelationship(
      AssociationType osAssociationType,
      AssociationType csAssociationType,
      string sourceEndName)
    {
      IEntityWrapper wrappedOwner = this.WrappedOwner;
      if (wrappedOwner.Context == null)
        return true;
      EntityKey entityKey = wrappedOwner.EntityKey;
      if (entityKey == (EntityKey) null)
        return true;
      if (osAssociationType.Index < 0)
        return RelationshipManager.VerifyRelationship(wrappedOwner, entityKey, osAssociationType, sourceEndName);
      EntitySet endEntitySet;
      if (wrappedOwner.Context.MetadataWorkspace.MetadataOptimization.FindCSpaceAssociationSet(csAssociationType, sourceEndName, entityKey.EntitySetName, entityKey.EntityContainerName, out endEntitySet) == null)
        throw Error.Collections_NoRelationshipSetMatched((object) osAssociationType.FullName);
      return true;
    }

    private static bool VerifyRelationship(
      IEntityWrapper wrappedOwner,
      EntityKey ownerKey,
      AssociationType relationship,
      string sourceEndName)
    {
      TypeUsage typeUsage;
      EntitySet endEntitySet;
      if (wrappedOwner.Context.Perspective.TryGetTypeByName(relationship.FullName, false, out typeUsage) && wrappedOwner.Context.MetadataWorkspace.MetadataOptimization.FindCSpaceAssociationSet((AssociationType) typeUsage.EdmType, sourceEndName, ownerKey.EntitySetName, ownerKey.EntityContainerName, out endEntitySet) == null)
        throw Error.Collections_NoRelationshipSetMatched((object) relationship.FullName);
      return true;
    }

    /// <summary>
    /// Gets an <see cref="T:System.Data.Entity.Core.Objects.DataClasses.EntityCollection`1" /> of related objects with the specified relationship name and target role name.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.Objects.DataClasses.EntityCollection`1" /> of related objects.
    /// </returns>
    /// <param name="relationshipName">Name of the relationship to navigate. The relationship name is not namespace qualified.</param>
    /// <param name="targetRoleName">Name of the target role for the navigation. Indicates the direction of navigation across the relationship.</param>
    /// <typeparam name="TTargetEntity">
    /// The type of the returned <see cref="T:System.Data.Entity.Core.Objects.DataClasses.EntityCollection`1" />.
    /// </typeparam>
    /// <exception cref="T:System.InvalidOperationException">
    /// The specified role returned an <see cref="T:System.Data.Entity.Core.Objects.DataClasses.EntityReference`1" /> instead of an
    /// <see cref="T:System.Data.Entity.Core.Objects.DataClasses.EntityCollection`1" />
    /// .
    /// </exception>
    public EntityCollection<TTargetEntity> GetRelatedCollection<TTargetEntity>(
      string relationshipName,
      string targetRoleName)
      where TTargetEntity : class
    {
      EntityCollection<TTargetEntity> relatedEndInternal = this.GetRelatedEndInternal(this.PrependNamespaceToRelationshipName(relationshipName), targetRoleName) as EntityCollection<TTargetEntity>;
      if (relatedEndInternal == null)
        throw new InvalidOperationException(Strings.Collections_ExpectedCollectionGotReference((object) typeof (TTargetEntity).Name, (object) targetRoleName, (object) relationshipName));
      return relatedEndInternal;
    }

    /// <summary>
    /// Gets the <see cref="T:System.Data.Entity.Core.Objects.DataClasses.EntityReference`1" /> for a related object by using the specified combination of relationship name and target role name.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.Objects.DataClasses.EntityReference`1" /> of a related object.
    /// </returns>
    /// <param name="relationshipName">Name of the relationship to navigate. The relationship name is not namespace qualified.</param>
    /// <param name="targetRoleName">Name of the target role for the navigation. Indicates the direction of navigation across the relationship.</param>
    /// <typeparam name="TTargetEntity">
    /// The type of the returned <see cref="T:System.Data.Entity.Core.Objects.DataClasses.EntityReference`1" />.
    /// </typeparam>
    /// <exception cref="T:System.InvalidOperationException">
    /// The specified role returned an <see cref="T:System.Data.Entity.Core.Objects.DataClasses.EntityCollection`1" /> instead of an
    /// <see cref="T:System.Data.Entity.Core.Objects.DataClasses.EntityReference`1" />
    /// .
    /// </exception>
    public EntityReference<TTargetEntity> GetRelatedReference<TTargetEntity>(
      string relationshipName,
      string targetRoleName)
      where TTargetEntity : class
    {
      EntityReference<TTargetEntity> relatedEndInternal = this.GetRelatedEndInternal(this.PrependNamespaceToRelationshipName(relationshipName), targetRoleName) as EntityReference<TTargetEntity>;
      if (relatedEndInternal == null)
        throw new InvalidOperationException(Strings.EntityReference_ExpectedReferenceGotCollection((object) typeof (TTargetEntity).Name, (object) targetRoleName, (object) relationshipName));
      return relatedEndInternal;
    }

    internal RelatedEnd GetRelatedEnd(
      RelationshipNavigation navigation,
      IRelationshipFixer relationshipFixer)
    {
      RelatedEnd relatedEnd;
      if (this.TryGetCachedRelatedEnd(navigation.RelationshipName, navigation.To, out relatedEnd))
        return relatedEnd;
      return relationshipFixer.CreateSourceEnd(navigation, this);
    }

    internal RelatedEnd CreateRelatedEnd<TSourceEntity, TTargetEntity>(
      RelationshipNavigation navigation,
      RelationshipMultiplicity sourceRoleMultiplicity,
      RelationshipMultiplicity targetRoleMultiplicity,
      RelatedEnd existingRelatedEnd)
      where TSourceEntity : class
      where TTargetEntity : class
    {
      IRelationshipFixer relationshipFixer = (IRelationshipFixer) new RelationshipFixer<TSourceEntity, TTargetEntity>(sourceRoleMultiplicity, targetRoleMultiplicity);
      IEntityWrapper wrappedOwner = this.WrappedOwner;
      RelatedEnd relatedEnd;
      switch (targetRoleMultiplicity)
      {
        case RelationshipMultiplicity.ZeroOrOne:
        case RelationshipMultiplicity.One:
          if (existingRelatedEnd != null)
          {
            existingRelatedEnd.InitializeRelatedEnd(wrappedOwner, navigation, relationshipFixer);
            relatedEnd = existingRelatedEnd;
            break;
          }
          relatedEnd = (RelatedEnd) new EntityReference<TTargetEntity>(wrappedOwner, navigation, relationshipFixer);
          break;
        case RelationshipMultiplicity.Many:
          if (existingRelatedEnd != null)
          {
            existingRelatedEnd.InitializeRelatedEnd(wrappedOwner, navigation, relationshipFixer);
            relatedEnd = existingRelatedEnd;
            break;
          }
          relatedEnd = (RelatedEnd) new EntityCollection<TTargetEntity>(wrappedOwner, navigation, relationshipFixer);
          break;
        default:
          Type type = typeof (RelationshipMultiplicity);
          throw new ArgumentOutOfRangeException(type.Name, Strings.ADP_InvalidEnumerationValue((object) type.Name, (object) ((int) targetRoleMultiplicity).ToString((IFormatProvider) CultureInfo.InvariantCulture)));
      }
      if (wrappedOwner.Context != null)
        relatedEnd.AttachContext(wrappedOwner.Context, wrappedOwner.MergeOption);
      this.EnsureRelationshipsInitialized();
      this._relationships.Add(relatedEnd);
      return relatedEnd;
    }

    /// <summary>Returns an enumeration of all the related ends managed by the relationship manager.</summary>
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of objects that implement
    /// <see cref="T:System.Data.Entity.Core.Objects.DataClasses.IRelatedEnd" />
    /// . An empty enumeration is returned when the relationships have not yet been populated.
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public IEnumerable<IRelatedEnd> GetAllRelatedEnds()
    {
      IEntityWrapper wrappedOwner = this.WrappedOwner;
      EntityType entityType;
      if (wrappedOwner.Context != null && wrappedOwner.Context.MetadataWorkspace != null && this.TryGetOwnerEntityType(out entityType))
      {
        EntitySet entitySet = wrappedOwner.Context.GetEntitySet(wrappedOwner.EntityKey.EntitySetName, wrappedOwner.EntityKey.EntityContainerName);
        foreach (AssociationEndMember allTargetEnd in RelationshipManager.GetAllTargetEnds(entityType, entitySet))
          yield return this.GetRelatedEnd(allTargetEnd.DeclaringType.FullName, allTargetEnd.Name);
      }
      else if (wrappedOwner.Entity != null)
      {
        foreach (AssociationEndMember allTargetEnd in this.GetAllTargetEnds(wrappedOwner.IdentityType))
          yield return this.GetRelatedEnd(allTargetEnd.DeclaringType.FullName, allTargetEnd.Name);
      }
    }

    /// <summary>
    /// Called by Object Services to prepare an <see cref="T:System.Data.Entity.Core.EntityKey" /> for binary serialization with a serialized relationship.
    /// </summary>
    /// <param name="context">Describes the source and destination of a given serialized stream, and provides an additional caller-defined context.</param>
    [OnSerializing]
    [SuppressMessage("Microsoft.Usage", "CA2238:ImplementSerializationMethodsCorrectly")]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void OnSerializing(StreamingContext context)
    {
      IEntityWrapper wrappedOwner = this.WrappedOwner;
      if (!(wrappedOwner.Entity is IEntityWithRelationships))
        throw new InvalidOperationException(Strings.RelatedEnd_CannotSerialize((object) nameof (RelationshipManager)));
      if (wrappedOwner.Context == null || wrappedOwner.MergeOption == MergeOption.NoTracking)
        return;
      foreach (RelatedEnd allRelatedEnd in this.GetAllRelatedEnds())
      {
        EntityReference entityReference = allRelatedEnd as EntityReference;
        if (entityReference != null && entityReference.EntityKey != (EntityKey) null)
          entityReference.DetachedEntityKey = entityReference.EntityKey;
      }
    }

    internal bool HasRelationships
    {
      get
      {
        return this._relationships != null;
      }
    }

    internal void AddRelatedEntitiesToObjectStateManager(bool doAttach)
    {
      if (this._relationships == null)
        return;
      bool flag = true;
      try
      {
        foreach (RelatedEnd relationship in this.Relationships)
          relationship.Include(false, doAttach);
        flag = false;
      }
      finally
      {
        if (flag)
        {
          IEntityWrapper wrappedOwner = this.WrappedOwner;
          TransactionManager transactionManager = wrappedOwner.Context.ObjectStateManager.TransactionManager;
          wrappedOwner.Context.ObjectStateManager.DegradePromotedRelationships();
          this.NodeVisited = true;
          RelationshipManager.RemoveRelatedEntitiesFromObjectStateManager(wrappedOwner);
          EntityEntry entityEntry;
          if (transactionManager.IsAttachTracking && transactionManager.PromotedKeyEntries.TryGetValue(wrappedOwner.Entity, out entityEntry))
            entityEntry.DegradeEntry();
          else
            RelatedEnd.RemoveEntityFromObjectStateManager(wrappedOwner);
        }
      }
    }

    internal static void RemoveRelatedEntitiesFromObjectStateManager(IEntityWrapper wrappedEntity)
    {
      foreach (RelatedEnd relationship in wrappedEntity.RelationshipManager.Relationships)
      {
        if (relationship.ObjectContext != null)
        {
          relationship.Exclude();
          relationship.DetachContext();
        }
      }
    }

    internal void RemoveEntityFromRelationships()
    {
      if (this._relationships == null)
        return;
      foreach (RelatedEnd relationship in this.Relationships)
        relationship.RemoveAll();
    }

    internal void NullAllFKsInDependentsForWhichThisIsThePrincipal()
    {
      if (this._relationships == null)
        return;
      List<EntityReference> entityReferenceList = new List<EntityReference>();
      foreach (RelatedEnd relationship in this.Relationships)
      {
        if (relationship.IsForeignKey)
        {
          foreach (IEntityWrapper wrappedEntity in relationship.GetWrappedEntities())
          {
            RelatedEnd endOfRelationship = relationship.GetOtherEndOfRelationship(wrappedEntity);
            if (endOfRelationship.IsDependentEndOfReferentialConstraint(false))
              entityReferenceList.Add((EntityReference) endOfRelationship);
          }
        }
      }
      foreach (EntityReference entityReference in entityReferenceList)
        entityReference.NullAllForeignKeys();
    }

    internal void DetachEntityFromRelationships(EntityState ownerEntityState)
    {
      if (this._relationships == null)
        return;
      foreach (RelatedEnd relationship in this.Relationships)
        relationship.DetachAll(ownerEntityState);
    }

    internal void RemoveEntity(
      string toRole,
      string relationshipName,
      IEntityWrapper wrappedEntity)
    {
      RelatedEnd relatedEnd;
      if (!this.TryGetCachedRelatedEnd(relationshipName, toRole, out relatedEnd))
        return;
      relatedEnd.Remove(wrappedEntity, false);
    }

    internal void ClearRelatedEndWrappers()
    {
      if (this._relationships == null)
        return;
      foreach (RelatedEnd relationship in this.Relationships)
        relationship.ClearWrappedValues();
    }

    internal void RetrieveReferentialConstraintProperties(
      out Dictionary<string, KeyValuePair<object, IntBox>> properties,
      HashSet<object> visited,
      bool includeOwnValues)
    {
      IEntityWrapper wrappedOwner = this.WrappedOwner;
      properties = new Dictionary<string, KeyValuePair<object, IntBox>>();
      EntityKey entityKey = wrappedOwner.EntityKey;
      if (entityKey.IsTemporary)
      {
        List<string> propertiesToRetrieve;
        bool propertiesToPropagateExist;
        this.FindNamesOfReferentialConstraintProperties(out propertiesToRetrieve, out propertiesToPropagateExist, false);
        if (propertiesToRetrieve != null)
        {
          if (this._relationships != null)
          {
            foreach (RelatedEnd relationship in this._relationships)
              relationship.RetrieveReferentialConstraintProperties(properties, visited);
          }
          if (!RelationshipManager.CheckIfAllPropertiesWereRetrieved(properties, propertiesToRetrieve))
          {
            wrappedOwner.Context.ObjectStateManager.FindEntityEntry(entityKey).RetrieveReferentialConstraintPropertiesFromKeyEntries(properties);
            if (!RelationshipManager.CheckIfAllPropertiesWereRetrieved(properties, propertiesToRetrieve))
              throw new InvalidOperationException(Strings.RelationshipManager_UnableToRetrieveReferentialConstraintProperties);
          }
        }
      }
      if (entityKey.IsTemporary && !includeOwnValues)
        return;
      wrappedOwner.Context.ObjectStateManager.FindEntityEntry(entityKey).GetOtherKeyProperties(properties);
    }

    private static bool CheckIfAllPropertiesWereRetrieved(
      Dictionary<string, KeyValuePair<object, IntBox>> properties,
      List<string> propertiesToRetrieve)
    {
      bool flag = true;
      List<int> intList = new List<int>();
      ICollection<KeyValuePair<object, IntBox>> values = (ICollection<KeyValuePair<object, IntBox>>) properties.Values;
      foreach (KeyValuePair<object, IntBox> keyValuePair in (IEnumerable<KeyValuePair<object, IntBox>>) values)
        intList.Add(keyValuePair.Value.Value);
      foreach (string key in propertiesToRetrieve)
      {
        if (!properties.ContainsKey(key))
        {
          flag = false;
          break;
        }
        KeyValuePair<object, IntBox> property = properties[key];
        --property.Value.Value;
        if (property.Value.Value < 0)
        {
          flag = false;
          break;
        }
      }
      if (flag)
      {
        foreach (KeyValuePair<object, IntBox> keyValuePair in (IEnumerable<KeyValuePair<object, IntBox>>) values)
        {
          if (keyValuePair.Value.Value != 0)
          {
            flag = false;
            break;
          }
        }
      }
      if (!flag)
      {
        IEnumerator<int> enumerator = (IEnumerator<int>) intList.GetEnumerator();
        foreach (KeyValuePair<object, IntBox> keyValuePair in (IEnumerable<KeyValuePair<object, IntBox>>) values)
        {
          enumerator.MoveNext();
          keyValuePair.Value.Value = enumerator.Current;
        }
      }
      return flag;
    }

    internal void CheckReferentialConstraintProperties(EntityEntry ownerEntry)
    {
      List<string> propertiesToRetrieve;
      bool propertiesToPropagateExist;
      this.FindNamesOfReferentialConstraintProperties(out propertiesToRetrieve, out propertiesToPropagateExist, false);
      if (propertiesToRetrieve == null && !propertiesToPropagateExist || this._relationships == null)
        return;
      foreach (RelatedEnd relationship in this._relationships)
        relationship.CheckReferentialConstraintProperties(ownerEntry);
    }

    /// <summary>
    /// Used internally to deserialize entity objects along with the
    /// <see cref="T:System.Data.Entity.Core.Objects.DataClasses.RelationshipManager" />
    /// instances.
    /// </summary>
    /// <param name="context">The serialized stream.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    [OnDeserialized]
    [SuppressMessage("Microsoft.Usage", "CA2238:ImplementSerializationMethodsCorrectly")]
    public void OnDeserialized(StreamingContext context)
    {
      this._entityWrapperFactory = new EntityWrapperFactory();
      this._expensiveLoader = new ExpensiveOSpaceLoader();
      this._wrappedOwner = this.EntityWrapperFactory.WrapEntityUsingContext((object) this._owner, (ObjectContext) null);
    }

    private bool TryGetCachedRelatedEnd(
      string relationshipName,
      string targetRoleName,
      out RelatedEnd relatedEnd)
    {
      relatedEnd = (RelatedEnd) null;
      if (this._relationships != null)
      {
        foreach (RelatedEnd relationship in this._relationships)
        {
          RelationshipNavigation relationshipNavigation = relationship.RelationshipNavigation;
          if (relationshipNavigation.RelationshipName == relationshipName && relationshipNavigation.To == targetRoleName)
          {
            relatedEnd = relationship;
            return true;
          }
        }
      }
      return false;
    }

    internal bool FindNamesOfReferentialConstraintProperties(
      out List<string> propertiesToRetrieve,
      out bool propertiesToPropagateExist,
      bool skipFK)
    {
      IEntityWrapper wrappedOwner = this.WrappedOwner;
      EntityKey entityKey = wrappedOwner.EntityKey;
      if ((object) entityKey == null)
        throw Error.EntityKey_UnexpectedNull();
      propertiesToRetrieve = (List<string>) null;
      propertiesToPropagateExist = false;
      if (wrappedOwner.Context == null)
        throw new InvalidOperationException(Strings.RelationshipManager_UnexpectedNullContext);
      EntitySet entitySet = entityKey.GetEntitySet(wrappedOwner.Context.MetadataWorkspace);
      List<AssociationSet> associationsForEntitySet = MetadataHelper.GetAssociationsForEntitySet((EntitySetBase) entitySet);
      bool flag = false;
      foreach (AssociationSet associationSet in associationsForEntitySet)
      {
        if (skipFK && associationSet.ElementType.IsForeignKey)
        {
          flag = true;
        }
        else
        {
          foreach (ReferentialConstraint referentialConstraint in associationSet.ElementType.ReferentialConstraints)
          {
            if (referentialConstraint.ToRole.TypeUsage.EdmType == entitySet.ElementType.GetReferenceType())
            {
              propertiesToRetrieve = propertiesToRetrieve ?? new List<string>();
              foreach (EdmProperty toProperty in referentialConstraint.ToProperties)
                propertiesToRetrieve.Add(toProperty.Name);
            }
            if (referentialConstraint.FromRole.TypeUsage.EdmType == entitySet.ElementType.GetReferenceType())
              propertiesToPropagateExist = true;
          }
        }
      }
      return flag;
    }

    internal bool IsOwner(IEntityWrapper wrappedEntity)
    {
      IEntityWrapper wrappedOwner = this.WrappedOwner;
      return object.ReferenceEquals(wrappedEntity.Entity, wrappedOwner.Entity);
    }

    internal void AttachContextToRelatedEnds(
      ObjectContext context,
      EntitySet entitySet,
      MergeOption mergeOption)
    {
      if (this._relationships == null)
        return;
      foreach (RelatedEnd relationship in this.Relationships)
      {
        EdmType relationshipType;
        RelationshipSet relationshipSet;
        relationship.FindRelationshipSet(context, entitySet, out relationshipType, out relationshipSet);
        if (relationshipSet != null || !relationship.IsEmpty())
          relationship.AttachContext(context, entitySet, mergeOption);
        else
          this._relationships.Remove(relationship);
      }
    }

    internal void ResetContextOnRelatedEnds(
      ObjectContext context,
      EntitySet entitySet,
      MergeOption mergeOption)
    {
      if (this._relationships == null)
        return;
      foreach (RelatedEnd relationship in this.Relationships)
      {
        relationship.AttachContext(context, entitySet, mergeOption);
        foreach (IEntityWrapper wrappedEntity in relationship.GetWrappedEntities())
          wrappedEntity.ResetContext(context, relationship.GetTargetEntitySetFromRelationshipSet(), mergeOption);
      }
    }

    internal void DetachContextFromRelatedEnds()
    {
      if (this._relationships == null)
        return;
      foreach (RelatedEnd relationship in this._relationships)
        relationship.DetachContext();
    }

    [Conditional("DEBUG")]
    internal void VerifyIsNotRelated()
    {
      if (this._relationships == null)
        return;
      foreach (RelatedEnd relationship in this._relationships)
        relationship.IsEmpty();
    }
  }
}
