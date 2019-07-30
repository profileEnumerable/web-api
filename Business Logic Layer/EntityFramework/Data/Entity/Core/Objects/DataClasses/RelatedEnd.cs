// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.DataClasses.RelatedEnd
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects.Internal;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace System.Data.Entity.Core.Objects.DataClasses
{
  /// <summary>Base class for EntityCollection and EntityReference</summary>
  [DataContract]
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
  [Serializable]
  public abstract class RelatedEnd : IRelatedEnd
  {
    private const string _entityKeyParamName = "EntityKeyValue";
    [Obsolete]
    private IEntityWithRelationships _owner;
    private RelationshipNavigation _navigation;
    private IRelationshipFixer _relationshipFixer;
    internal bool _isLoaded;
    [NonSerialized]
    private RelationshipSet _relationshipSet;
    [NonSerialized]
    private ObjectContext _context;
    [NonSerialized]
    private bool _usingNoTracking;
    [NonSerialized]
    private RelationshipType _relationMetadata;
    [NonSerialized]
    private RelationshipEndMember _fromEndMember;
    [NonSerialized]
    private RelationshipEndMember _toEndMember;
    [NonSerialized]
    private string _sourceQuery;
    [NonSerialized]
    private IEnumerable<EdmMember> _sourceQueryParamProperties;
    [NonSerialized]
    internal bool _suppressEvents;
    [NonSerialized]
    internal CollectionChangeEventHandler _onAssociationChanged;
    [NonSerialized]
    private IEntityWrapper _wrappedOwner;
    [NonSerialized]
    private EntityWrapperFactory _entityWrapperFactory;
    [NonSerialized]
    private NavigationProperty navigationPropertyCache;

    internal RelatedEnd()
    {
      this._wrappedOwner = NullEntityWrapper.NullWrapper;
    }

    internal RelatedEnd(
      IEntityWrapper wrappedOwner,
      RelationshipNavigation navigation,
      IRelationshipFixer relationshipFixer)
    {
      this.InitializeRelatedEnd(wrappedOwner, navigation, relationshipFixer);
    }

    /// <summary>Occurs when a change is made to a related end.</summary>
    public event CollectionChangeEventHandler AssociationChanged
    {
      add
      {
        this.CheckOwnerNull();
        this._onAssociationChanged += value;
      }
      remove
      {
        this.CheckOwnerNull();
        this._onAssociationChanged -= value;
      }
    }

    internal virtual event CollectionChangeEventHandler AssociationChangedForObjectView
    {
      add
      {
      }
      remove
      {
      }
    }

    internal bool IsForeignKey
    {
      get
      {
        return ((AssociationType) this._relationMetadata).IsForeignKey;
      }
    }

    internal RelationshipNavigation RelationshipNavigation
    {
      get
      {
        return this._navigation;
      }
    }

    /// <summary>Gets the name of the relationship in which this related end participates.</summary>
    /// <returns>
    /// The name of the relationship in which this <see cref="T:System.Data.Entity.Core.Objects.DataClasses.RelatedEnd" /> participates. The relationship name is not namespace qualified.
    /// </returns>
    [XmlIgnore]
    [SoapIgnore]
    public string RelationshipName
    {
      get
      {
        this.CheckOwnerNull();
        return this._navigation.RelationshipName;
      }
    }

    /// <summary>Gets the role name at the source end of the relationship.</summary>
    /// <returns>
    /// A <see cref="T:System.String" /> that is the role name.
    /// </returns>
    [XmlIgnore]
    [SoapIgnore]
    public virtual string SourceRoleName
    {
      get
      {
        this.CheckOwnerNull();
        return this._navigation.From;
      }
    }

    /// <summary>Gets the role name at the target end of the relationship.</summary>
    /// <returns>
    /// A <see cref="T:System.String" /> that is the role name.
    /// </returns>
    [XmlIgnore]
    [SoapIgnore]
    public virtual string TargetRoleName
    {
      get
      {
        this.CheckOwnerNull();
        return this._navigation.To;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    IEnumerable IRelatedEnd.CreateSourceQuery()
    {
      this.CheckOwnerNull();
      return this.CreateSourceQueryInternal();
    }

    internal virtual IEntityWrapper WrappedOwner
    {
      get
      {
        return this._wrappedOwner;
      }
    }

    internal virtual ObjectContext ObjectContext
    {
      get
      {
        return this._context;
      }
    }

    internal virtual EntityWrapperFactory EntityWrapperFactory
    {
      get
      {
        if (this._entityWrapperFactory == null)
          this._entityWrapperFactory = new EntityWrapperFactory();
        return this._entityWrapperFactory;
      }
    }

    /// <summary>Gets a reference to the metadata for the related end.</summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.RelationshipSet" /> object that contains metadata for the end of a relationship.
    /// </returns>
    [XmlIgnore]
    [SoapIgnore]
    public virtual RelationshipSet RelationshipSet
    {
      get
      {
        this.CheckOwnerNull();
        return this._relationshipSet;
      }
    }

    internal virtual RelationshipType RelationMetadata
    {
      get
      {
        return this._relationMetadata;
      }
    }

    internal virtual RelationshipEndMember ToEndMember
    {
      get
      {
        return this._toEndMember;
      }
    }

    internal bool UsingNoTracking
    {
      get
      {
        return this._usingNoTracking;
      }
    }

    internal MergeOption DefaultMergeOption
    {
      get
      {
        return !this.UsingNoTracking ? MergeOption.AppendOnly : MergeOption.NoTracking;
      }
    }

    internal virtual RelationshipEndMember FromEndMember
    {
      get
      {
        return this._fromEndMember;
      }
    }

    /// <inheritdoc />
    [SoapIgnore]
    [XmlIgnore]
    public bool IsLoaded
    {
      get
      {
        this.CheckOwnerNull();
        return this._isLoaded;
      }
      set
      {
        this.CheckOwnerNull();
        this._isLoaded = value;
      }
    }

    internal ObjectQuery<TEntity> CreateSourceQuery<TEntity>(
      MergeOption mergeOption,
      out bool hasResults)
    {
      if (this._context == null)
      {
        hasResults = false;
        return (ObjectQuery<TEntity>) null;
      }
      EntityEntry entityEntry = this._context.ObjectStateManager.FindEntityEntry(this._wrappedOwner.Entity);
      EntityState entityState;
      if (entityEntry == null)
      {
        if (!this.UsingNoTracking)
          throw Error.Collections_InvalidEntityStateSource();
        entityState = EntityState.Detached;
      }
      else
        entityState = entityEntry.State;
      if (entityState == EntityState.Added && (!this.IsForeignKey || !this.IsDependentEndOfReferentialConstraint(false)))
        throw Error.Collections_InvalidEntityStateSource();
      if ((entityState != EntityState.Detached || !this.UsingNoTracking) && (entityState != EntityState.Modified && entityState != EntityState.Unchanged) && (entityState != EntityState.Deleted && entityState != EntityState.Added))
      {
        hasResults = false;
        return (ObjectQuery<TEntity>) null;
      }
      if (this._sourceQuery == null)
        this._sourceQuery = this.GenerateQueryText();
      ObjectQuery<TEntity> query = new ObjectQuery<TEntity>(this._sourceQuery, this._context, mergeOption);
      hasResults = this.AddQueryParameters<TEntity>(query);
      query.Parameters.SetReadOnly(true);
      return query;
    }

    private string GenerateQueryText()
    {
      EntityKey entityKey = this._wrappedOwner.EntityKey;
      if (entityKey == (EntityKey) null)
        throw Error.EntityKey_UnexpectedNull();
      AssociationType relationMetadata = (AssociationType) this._relationMetadata;
      EntitySet entitySet1 = ((AssociationSet) this._relationshipSet).AssociationSetEnds[this._fromEndMember.Name].EntitySet;
      EntitySet entitySet2 = ((AssociationSet) this._relationshipSet).AssociationSetEnds[this._toEndMember.Name].EntitySet;
      EntityType targetEntityType = MetadataHelper.GetEntityTypeForEnd((AssociationEndMember) this._toEndMember);
      bool ofTypeRequired = false;
      if (!entitySet2.ElementType.EdmEquals((MetadataItem) targetEntityType) && !TypeSemantics.IsSubTypeOf((EdmType) entitySet2.ElementType, (EdmType) targetEntityType))
      {
        ofTypeRequired = true;
        targetEntityType = (EntityType) this.ObjectContext.MetadataWorkspace.GetOSpaceTypeUsage(TypeUsage.Create((EdmType) targetEntityType)).EdmType;
      }
      StringBuilder sourceBuilder1;
      if (relationMetadata.IsForeignKey)
      {
        ReferentialConstraint referentialConstraint = relationMetadata.ReferentialConstraints[0];
        ReadOnlyMetadataCollection<EdmProperty> fromProperties = referentialConstraint.FromProperties;
        ReadOnlyMetadataCollection<EdmProperty> toProperties = referentialConstraint.ToProperties;
        if (referentialConstraint.ToRole.EdmEquals((MetadataItem) this._toEndMember))
        {
          sourceBuilder1 = new StringBuilder("SELECT VALUE D FROM ");
          RelatedEnd.AppendEntitySet(sourceBuilder1, entitySet2, targetEntityType, ofTypeRequired);
          sourceBuilder1.Append(" AS D WHERE ");
          AliasGenerator aliasGenerator = new AliasGenerator("EntityKeyValue");
          this._sourceQueryParamProperties = (IEnumerable<EdmMember>) fromProperties;
          for (int index = 0; index < toProperties.Count; ++index)
          {
            if (index > 0)
              sourceBuilder1.Append(" AND ");
            sourceBuilder1.Append("D.[");
            sourceBuilder1.Append(toProperties[index].Name);
            sourceBuilder1.Append("] = @");
            sourceBuilder1.Append(aliasGenerator.Next());
          }
        }
        else
        {
          StringBuilder sourceBuilder2 = new StringBuilder("SELECT VALUE P FROM ");
          RelatedEnd.AppendEntitySet(sourceBuilder2, entitySet2, targetEntityType, ofTypeRequired);
          sourceBuilder2.Append(" AS P WHERE ");
          AliasGenerator aliasGenerator = new AliasGenerator("EntityKeyValue");
          this._sourceQueryParamProperties = (IEnumerable<EdmMember>) toProperties;
          for (int index = 0; index < fromProperties.Count; ++index)
          {
            if (index > 0)
              sourceBuilder2.Append(" AND ");
            sourceBuilder2.Append("P.[");
            sourceBuilder2.Append(fromProperties[index].Name);
            sourceBuilder2.Append("] = @");
            sourceBuilder2.Append(aliasGenerator.Next());
          }
          return sourceBuilder2.ToString();
        }
      }
      else
      {
        sourceBuilder1 = new StringBuilder("SELECT VALUE [TargetEntity] FROM (SELECT VALUE x FROM ");
        sourceBuilder1.Append("[");
        sourceBuilder1.Append(this._relationshipSet.EntityContainer.Name);
        sourceBuilder1.Append("].[");
        sourceBuilder1.Append(this._relationshipSet.Name);
        sourceBuilder1.Append("] AS x WHERE Key(x.[");
        sourceBuilder1.Append(this._fromEndMember.Name);
        sourceBuilder1.Append("]) = ");
        RelatedEnd.AppendKeyParameterRow(sourceBuilder1, (IList<EdmMember>) entityKey.GetEntitySet(this.ObjectContext.MetadataWorkspace).ElementType.KeyMembers);
        sourceBuilder1.Append(") AS [AssociationEntry] INNER JOIN ");
        RelatedEnd.AppendEntitySet(sourceBuilder1, entitySet2, targetEntityType, ofTypeRequired);
        sourceBuilder1.Append(" AS [TargetEntity] ON Key([AssociationEntry].[");
        sourceBuilder1.Append(this._toEndMember.Name);
        sourceBuilder1.Append("]) = Key(Ref([TargetEntity]))");
      }
      return sourceBuilder1.ToString();
    }

    private bool AddQueryParameters<TEntity>(ObjectQuery<TEntity> query)
    {
      EntityKey entityKey = this._wrappedOwner.EntityKey;
      if (entityKey == (EntityKey) null)
        throw Error.EntityKey_UnexpectedNull();
      bool flag = true;
      AliasGenerator aliasGenerator = new AliasGenerator("EntityKeyValue");
      foreach (EdmMember edmMember in this._sourceQueryParamProperties ?? (IEnumerable<EdmMember>) entityKey.GetEntitySet(this.ObjectContext.MetadataWorkspace).ElementType.KeyMembers)
      {
        EdmMember parameterMember = edmMember;
        object obj = this._sourceQueryParamProperties != null ? (!this.CachedForeignKeyIsConceptualNull() ? this.GetCurrentValueFromEntity(parameterMember) : (object) null) : ((IEnumerable<EntityKeyMember>) this._wrappedOwner.EntityKey.EntityKeyValues).Single<EntityKeyMember>((Func<EntityKeyMember, bool>) (ekv => ekv.Key == parameterMember.Name)).Value;
        ObjectParameter objectParameter;
        if (obj == null)
        {
          EdmType edmType = parameterMember.TypeUsage.EdmType;
          Type type = Helper.IsPrimitiveType(edmType) ? ((PrimitiveType) edmType).ClrEquivalentType : this.ObjectContext.MetadataWorkspace.GetObjectSpaceType((EnumType) edmType).ClrType;
          objectParameter = new ObjectParameter(aliasGenerator.Next(), type);
          flag = false;
        }
        else
          objectParameter = new ObjectParameter(aliasGenerator.Next(), obj);
        objectParameter.TypeUsage = Helper.GetModelTypeUsage(parameterMember);
        query.Parameters.Add(objectParameter);
      }
      return flag;
    }

    private object GetCurrentValueFromEntity(EdmMember member)
    {
      StateManagerTypeMetadata managerTypeMetadata = this._context.ObjectStateManager.GetOrAddStateManagerTypeMetadata((EdmType) member.DeclaringType);
      return managerTypeMetadata.Member(managerTypeMetadata.GetOrdinalforCLayerMemberName(member.Name)).GetValue(this._wrappedOwner.Entity);
    }

    private static void AppendKeyParameterRow(
      StringBuilder sourceBuilder,
      IList<EdmMember> keyMembers)
    {
      sourceBuilder.Append("ROW(");
      AliasGenerator aliasGenerator = new AliasGenerator("EntityKeyValue");
      int count = keyMembers.Count;
      for (int index = 0; index < count; ++index)
      {
        string str = aliasGenerator.Next();
        sourceBuilder.Append("@");
        sourceBuilder.Append(str);
        sourceBuilder.Append(" AS ");
        sourceBuilder.Append(str);
        if (index < count - 1)
          sourceBuilder.Append(",");
      }
      sourceBuilder.Append(")");
    }

    private static void AppendEntitySet(
      StringBuilder sourceBuilder,
      EntitySet targetEntitySet,
      EntityType targetEntityType,
      bool ofTypeRequired)
    {
      if (ofTypeRequired)
        sourceBuilder.Append("OfType(");
      sourceBuilder.Append("[");
      sourceBuilder.Append(targetEntitySet.EntityContainer.Name);
      sourceBuilder.Append("].[");
      sourceBuilder.Append(targetEntitySet.Name);
      sourceBuilder.Append("]");
      if (!ofTypeRequired)
        return;
      sourceBuilder.Append(", [");
      if (!string.IsNullOrEmpty(targetEntityType.NamespaceName))
      {
        sourceBuilder.Append(targetEntityType.NamespaceName);
        sourceBuilder.Append("].[");
      }
      sourceBuilder.Append(targetEntityType.Name);
      sourceBuilder.Append("])");
    }

    internal virtual ObjectQuery<TEntity> ValidateLoad<TEntity>(
      MergeOption mergeOption,
      string relatedEndName,
      out bool hasResults)
    {
      ObjectQuery<TEntity> sourceQuery = this.CreateSourceQuery<TEntity>(mergeOption, out hasResults);
      if (sourceQuery == null)
        throw Error.RelatedEnd_RelatedEndNotAttachedToContext((object) relatedEndName);
      EntityEntry entityEntry = this.ObjectContext.ObjectStateManager.FindEntityEntry(this._wrappedOwner.Entity);
      if (entityEntry != null && entityEntry.State == EntityState.Deleted)
        throw Error.Collections_InvalidEntityStateLoad((object) relatedEndName);
      if (this.UsingNoTracking != (mergeOption == MergeOption.NoTracking))
        throw Error.RelatedEnd_MismatchedMergeOptionOnLoad((object) mergeOption);
      if (this.UsingNoTracking)
      {
        if (this.IsLoaded)
          throw Error.RelatedEnd_LoadCalledOnAlreadyLoadedNoTrackedRelatedEnd();
        if (!this.IsEmpty())
          throw Error.RelatedEnd_LoadCalledOnNonEmptyNoTrackedRelatedEnd();
      }
      return sourceQuery;
    }

    /// <summary>
    /// Loads the related object or objects into the related end with the default merge option.
    /// </summary>
    /// <exception cref="T:System.InvalidOperationException">
    /// When the source object was retrieved by using a <see cref="F:System.Data.Entity.Core.Objects.MergeOption.NoTracking" /> query
    /// and the <see cref="T:System.Data.Entity.Core.Objects.MergeOption" /> is not <see cref="F:System.Data.Entity.Core.Objects.MergeOption.NoTracking" />
    /// or the related objects are already loaded
    /// or when the source object is not attached to the <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />
    /// or when the source object is being tracked but is in the
    /// <see cref="F:System.Data.Entity.EntityState.Added" /> or <see cref="F:System.Data.Entity.EntityState.Deleted" /> state
    /// or the <see cref="T:System.Data.Entity.Core.Objects.MergeOption" />
    /// used for <see cref="M:System.Data.Entity.Core.Objects.DataClasses.RelatedEnd.Load" />
    /// is <see cref="F:System.Data.Entity.Core.Objects.MergeOption.NoTracking" />.
    /// </exception>
    public void Load()
    {
      this.Load(this.DefaultMergeOption);
    }

    /// <summary>
    /// Asynchronously loads the related object or objects into the related end with the default merge option.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="T:System.InvalidOperationException">
    /// When the source object was retrieved by using a <see cref="F:System.Data.Entity.Core.Objects.MergeOption.NoTracking" /> query
    /// and the <see cref="T:System.Data.Entity.Core.Objects.MergeOption" /> is not <see cref="F:System.Data.Entity.Core.Objects.MergeOption.NoTracking" />
    /// or the related objects are already loaded
    /// or when the source object is not attached to the <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />
    /// or when the source object is being tracked but is in the
    /// <see cref="F:System.Data.Entity.EntityState.Added" /> or <see cref="F:System.Data.Entity.EntityState.Deleted" /> state
    /// or the <see cref="T:System.Data.Entity.Core.Objects.MergeOption" />
    /// used for <see cref="M:System.Data.Entity.Core.Objects.DataClasses.RelatedEnd.Load" />
    /// is <see cref="F:System.Data.Entity.Core.Objects.MergeOption.NoTracking" />.
    /// </exception>
    public Task LoadAsync(CancellationToken cancellationToken)
    {
      return this.LoadAsync(this.DefaultMergeOption, cancellationToken);
    }

    /// <summary>
    /// Loads an object or objects from the related end with the specified merge option.
    /// </summary>
    /// <param name="mergeOption">
    /// The <see cref="T:System.Data.Entity.Core.Objects.MergeOption" /> to use when merging objects into an existing
    /// <see cref="T:System.Data.Entity.Core.Objects.DataClasses.EntityCollection`1" />.
    /// </param>
    /// <exception cref="T:System.InvalidOperationException">
    /// When the source object was retrieved by using a <see cref="F:System.Data.Entity.Core.Objects.MergeOption.NoTracking" />  query
    /// and the <see cref="T:System.Data.Entity.Core.Objects.MergeOption" />
    /// is not <see cref="F:System.Data.Entity.Core.Objects.MergeOption.NoTracking" />
    /// or the related objects are already loaded
    /// or when the source object is not attached to the <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />
    /// or when the source object is being tracked but is in the
    /// <see cref="F:System.Data.Entity.EntityState.Added" />  or <see cref="F:System.Data.Entity.EntityState.Deleted" /> state
    /// or the <see cref="T:System.Data.Entity.Core.Objects.MergeOption" />
    /// used for <see cref="M:System.Data.Entity.Core.Objects.DataClasses.RelatedEnd.Load" />
    /// is <see cref="F:System.Data.Entity.Core.Objects.MergeOption.NoTracking" />.
    /// </exception>
    public abstract void Load(MergeOption mergeOption);

    /// <summary>
    /// Asynchronously loads an object or objects from the related end with the specified merge option.
    /// </summary>
    /// <remarks>
    /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    /// that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <param name="mergeOption">
    /// The <see cref="T:System.Data.Entity.Core.Objects.MergeOption" /> to use when merging objects into an existing
    /// <see cref="T:System.Data.Entity.Core.Objects.DataClasses.EntityCollection`1" />.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="T:System.InvalidOperationException">
    /// When the source object was retrieved by using a <see cref="F:System.Data.Entity.Core.Objects.MergeOption.NoTracking" />  query
    /// and the <see cref="T:System.Data.Entity.Core.Objects.MergeOption" />
    /// is not <see cref="F:System.Data.Entity.Core.Objects.MergeOption.NoTracking" />
    /// or the related objects are already loaded
    /// or when the source object is not attached to the <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />
    /// or when the source object is being tracked but is in the
    /// <see cref="F:System.Data.Entity.EntityState.Added" />  or <see cref="F:System.Data.Entity.EntityState.Deleted" /> state
    /// or the <see cref="T:System.Data.Entity.Core.Objects.MergeOption" />
    /// used for <see cref="M:System.Data.Entity.Core.Objects.DataClasses.RelatedEnd.Load" />
    /// is <see cref="F:System.Data.Entity.Core.Objects.MergeOption.NoTracking" />.
    /// </exception>
    public abstract Task LoadAsync(MergeOption mergeOption, CancellationToken cancellationToken);

    internal void DeferredLoad()
    {
      if (this._wrappedOwner == null || this._wrappedOwner == NullEntityWrapper.NullWrapper || (this.IsLoaded || this._context == null) || (!this._context.ContextOptions.LazyLoadingEnabled || this._context.InMaterialization || !this.CanDeferredLoad) || !this.UsingNoTracking && (this._wrappedOwner.ObjectStateEntry == null || this._wrappedOwner.ObjectStateEntry.State != EntityState.Unchanged && this._wrappedOwner.ObjectStateEntry.State != EntityState.Modified && (this._wrappedOwner.ObjectStateEntry.State != EntityState.Added || !this.IsForeignKey || !this.IsDependentEndOfReferentialConstraint(false))))
        return;
      this._context.ContextOptions.LazyLoadingEnabled = false;
      try
      {
        this.Load();
      }
      finally
      {
        this._context.ContextOptions.LazyLoadingEnabled = true;
      }
    }

    internal virtual bool CanDeferredLoad
    {
      get
      {
        return true;
      }
    }

    internal virtual void Merge<TEntity>(
      IEnumerable<TEntity> collection,
      MergeOption mergeOption,
      bool setIsLoaded)
    {
      List<IEntityWrapper> collection1 = collection as List<IEntityWrapper>;
      if (collection1 == null)
      {
        collection1 = new List<IEntityWrapper>();
        EntitySet entitySet = ((AssociationSet) this.RelationshipSet).AssociationSetEnds[this.TargetRoleName].EntitySet;
        foreach (TEntity entity in collection)
        {
          IEntityWrapper wrapper = this.EntityWrapperFactory.WrapEntityUsingContext((object) entity, this.ObjectContext);
          if (mergeOption == MergeOption.NoTracking)
            this.EntityWrapperFactory.UpdateNoTrackingWrapper(wrapper, this.ObjectContext, entitySet);
          collection1.Add(wrapper);
        }
      }
      this.Merge<TEntity>(collection1, mergeOption, setIsLoaded);
    }

    internal virtual void Merge<TEntity>(
      List<IEntityWrapper> collection,
      MergeOption mergeOption,
      bool setIsLoaded)
    {
      if (this.WrappedOwner.EntityKey == (EntityKey) null)
        throw Error.EntityKey_UnexpectedNull();
      this.ObjectContext.ObjectStateManager.UpdateRelationships(this.ObjectContext, mergeOption, (AssociationSet) this.RelationshipSet, (AssociationEndMember) this.FromEndMember, this.WrappedOwner, (AssociationEndMember) this.ToEndMember, (IList) collection, setIsLoaded);
      if (!setIsLoaded)
        return;
      this._isLoaded = true;
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    void IRelatedEnd.Attach(IEntityWithRelationships entity)
    {
      Check.NotNull<IEntityWithRelationships>(entity, nameof (entity));
      ((IRelatedEnd) this).Attach((object) entity);
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    void IRelatedEnd.Attach(object entity)
    {
      Check.NotNull<object>(entity, nameof (entity));
      this.CheckOwnerNull();
      this.Attach((IEnumerable<IEntityWrapper>) new IEntityWrapper[1]
      {
        this.EntityWrapperFactory.WrapEntityUsingContext(entity, this.ObjectContext)
      }, false);
    }

    internal void Attach(IEnumerable<IEntityWrapper> wrappedEntities, bool allowCollection)
    {
      this.CheckOwnerNull();
      this.ValidateOwnerForAttach();
      int num = 0;
      List<IEntityWrapper> entityWrapperList = new List<IEntityWrapper>();
      foreach (IEntityWrapper wrappedEntity in wrappedEntities)
      {
        this.ValidateEntityForAttach(wrappedEntity, num++, allowCollection);
        entityWrapperList.Add(wrappedEntity);
      }
      this._suppressEvents = true;
      try
      {
        this.Merge<IEntityWrapper>((IEnumerable<IEntityWrapper>) entityWrapperList, MergeOption.OverwriteChanges, false);
        ReferentialConstraint constraint = ((AssociationType) this.RelationMetadata).ReferentialConstraints.FirstOrDefault<ReferentialConstraint>();
        if (constraint != null)
        {
          ObjectStateManager objectStateManager = this.ObjectContext.ObjectStateManager;
          EntityEntry entityEntry1 = objectStateManager.FindEntityEntry(this._wrappedOwner.Entity);
          if (this.IsDependentEndOfReferentialConstraint(false))
          {
            if (!RelatedEnd.VerifyRIConstraintsWithRelatedEntry(constraint, new Func<string, object>(entityEntry1.GetCurrentEntityValue), entityWrapperList[0].ObjectStateEntry.EntityKey))
              throw new InvalidOperationException(constraint.BuildConstraintExceptionMessage());
          }
          else
          {
            foreach (IEntityWrapper wrappedEntity in entityWrapperList)
            {
              RelatedEnd endOfRelationship = this.GetOtherEndOfRelationship(wrappedEntity);
              if (endOfRelationship.IsDependentEndOfReferentialConstraint(false))
              {
                EntityEntry entityEntry2 = objectStateManager.FindEntityEntry(endOfRelationship.WrappedOwner.Entity);
                if (!RelatedEnd.VerifyRIConstraintsWithRelatedEntry(constraint, new Func<string, object>(entityEntry2.GetCurrentEntityValue), entityEntry1.EntityKey))
                  throw new InvalidOperationException(constraint.BuildConstraintExceptionMessage());
              }
            }
          }
        }
      }
      finally
      {
        this._suppressEvents = false;
      }
      this.OnAssociationChanged(CollectionChangeAction.Refresh, (object) null);
    }

    internal void ValidateOwnerForAttach()
    {
      if (this.ObjectContext == null || this.UsingNoTracking)
        throw Error.RelatedEnd_InvalidOwnerStateForAttach();
      EntityEntry entityEntry = this.ObjectContext.ObjectStateManager.GetEntityEntry(this._wrappedOwner.Entity);
      if (entityEntry.State != EntityState.Modified && entityEntry.State != EntityState.Unchanged)
        throw Error.RelatedEnd_InvalidOwnerStateForAttach();
    }

    internal void ValidateEntityForAttach(
      IEntityWrapper wrappedEntity,
      int index,
      bool allowCollection)
    {
      if (wrappedEntity == null || wrappedEntity.Entity == null)
      {
        if (allowCollection)
          throw Error.RelatedEnd_InvalidNthElementNullForAttach((object) index);
        throw new ArgumentNullException(nameof (wrappedEntity));
      }
      this.VerifyType(wrappedEntity);
      EntityEntry entityEntry = this.ObjectContext.ObjectStateManager.FindEntityEntry(wrappedEntity.Entity);
      if (entityEntry == null || !object.ReferenceEquals(entityEntry.Entity, wrappedEntity.Entity))
      {
        if (allowCollection)
          throw Error.RelatedEnd_InvalidNthElementContextForAttach((object) index);
        throw Error.RelatedEnd_InvalidEntityContextForAttach();
      }
      if (entityEntry.State == EntityState.Unchanged || entityEntry.State == EntityState.Modified)
        return;
      if (allowCollection)
        throw Error.RelatedEnd_InvalidNthElementStateForAttach((object) index);
      throw Error.RelatedEnd_InvalidEntityStateForAttach();
    }

    internal abstract IEnumerable CreateSourceQueryInternal();

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    void IRelatedEnd.Add(IEntityWithRelationships entity)
    {
      Check.NotNull<IEntityWithRelationships>(entity, nameof (entity));
      ((IRelatedEnd) this).Add((object) entity);
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    void IRelatedEnd.Add(object entity)
    {
      Check.NotNull<object>(entity, nameof (entity));
      this.Add(this.EntityWrapperFactory.WrapEntityUsingContext(entity, this.ObjectContext));
    }

    internal void Add(IEntityWrapper wrappedEntity)
    {
      if (this._wrappedOwner.Entity != null)
        this.Add(wrappedEntity, true);
      else
        this.DisconnectedAdd(wrappedEntity);
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    bool IRelatedEnd.Remove(IEntityWithRelationships entity)
    {
      Check.NotNull<IEntityWithRelationships>(entity, nameof (entity));
      return ((IRelatedEnd) this).Remove((object) entity);
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    bool IRelatedEnd.Remove(object entity)
    {
      Check.NotNull<object>(entity, nameof (entity));
      this.DeferredLoad();
      return this.Remove(this.EntityWrapperFactory.WrapEntityUsingContext(entity, this.ObjectContext), false);
    }

    internal bool Remove(IEntityWrapper wrappedEntity, bool preserveForeignKey)
    {
      if (this._wrappedOwner.Entity == null)
        return this.DisconnectedRemove(wrappedEntity);
      if (!this.ContainsEntity(wrappedEntity))
        return false;
      this.Remove(wrappedEntity, true, false, false, true, preserveForeignKey);
      return true;
    }

    internal abstract void DisconnectedAdd(IEntityWrapper wrappedEntity);

    internal abstract bool DisconnectedRemove(IEntityWrapper wrappedEntity);

    internal void Add(IEntityWrapper wrappedEntity, bool applyConstraints)
    {
      if (this._context != null && !this.UsingNoTracking)
      {
        this.ValidateStateForAdd(this._wrappedOwner);
        this.ValidateStateForAdd(wrappedEntity);
      }
      this.Add(wrappedEntity, applyConstraints, false, false, true, true);
    }

    internal void CheckRelationEntitySet(EntitySet set)
    {
      if (((AssociationSet) this._relationshipSet).AssociationSetEnds[this._navigation.To] != null && ((AssociationSet) this._relationshipSet).AssociationSetEnds[this._navigation.To].EntitySet != set)
        throw Error.RelatedEnd_EntitySetIsNotValidForRelationship((object) set.EntityContainer.Name, (object) set.Name, (object) this._navigation.To, (object) this._relationshipSet.EntityContainer.Name, (object) this._relationshipSet.Name);
    }

    internal void ValidateStateForAdd(IEntityWrapper wrappedEntity)
    {
      EntityEntry entityEntry = this.ObjectContext.ObjectStateManager.FindEntityEntry(wrappedEntity.Entity);
      if (entityEntry != null && entityEntry.State == EntityState.Deleted)
        throw Error.RelatedEnd_UnableToAddRelationshipWithDeletedEntity();
    }

    internal void Add(
      IEntityWrapper wrappedTarget,
      bool applyConstraints,
      bool addRelationshipAsUnchanged,
      bool relationshipAlreadyExists,
      bool allowModifyingOtherEndOfRelationship,
      bool forceForeignKeyChanges)
    {
      if (!this.VerifyEntityForAdd(wrappedTarget, relationshipAlreadyExists))
        return;
      EntityKey entityKey = wrappedTarget.EntityKey;
      if (entityKey != (EntityKey) null && this.ObjectContext != null)
        this.CheckRelationEntitySet(entityKey.GetEntitySet(this.ObjectContext.MetadataWorkspace));
      RelatedEnd endOfRelationship = this.GetOtherEndOfRelationship(wrappedTarget);
      this.ValidateContextsAreCompatible(endOfRelationship);
      endOfRelationship.VerifyEntityForAdd(this._wrappedOwner, relationshipAlreadyExists);
      endOfRelationship.VerifyMultiplicityConstraintsForAdd(!allowModifyingOtherEndOfRelationship);
      if (this.CheckIfNavigationPropertyContainsEntity(wrappedTarget))
        this.AddToLocalCache(wrappedTarget, applyConstraints);
      else
        this.AddToCache(wrappedTarget, applyConstraints);
      if (endOfRelationship.CheckIfNavigationPropertyContainsEntity(this.WrappedOwner))
        endOfRelationship.AddToLocalCache(this._wrappedOwner, false);
      else
        endOfRelationship.AddToCache(this._wrappedOwner, false);
      this.SynchronizeContexts(endOfRelationship, relationshipAlreadyExists, addRelationshipAsUnchanged);
      if (this.ObjectContext != null && this.IsForeignKey && (!this.ObjectContext.ObjectStateManager.TransactionManager.IsGraphUpdate && !this.UpdateDependentEndForeignKey(endOfRelationship, forceForeignKeyChanges)))
        endOfRelationship.UpdateDependentEndForeignKey(this, forceForeignKeyChanges);
      endOfRelationship.OnAssociationChanged(CollectionChangeAction.Add, this._wrappedOwner.Entity);
      this.OnAssociationChanged(CollectionChangeAction.Add, wrappedTarget.Entity);
    }

    internal virtual void AddToNavigationPropertyIfCompatible(RelatedEnd otherRelatedEnd)
    {
      this.AddToNavigationProperty(otherRelatedEnd.WrappedOwner);
    }

    internal virtual bool CachedForeignKeyIsConceptualNull()
    {
      return false;
    }

    internal virtual bool UpdateDependentEndForeignKey(
      RelatedEnd targetRelatedEnd,
      bool forceForeignKeyChanges)
    {
      return false;
    }

    internal virtual void VerifyDetachedKeyMatches(EntityKey entityKey)
    {
    }

    private void ValidateContextsAreCompatible(RelatedEnd targetRelatedEnd)
    {
      if (object.ReferenceEquals((object) this.ObjectContext, (object) targetRelatedEnd.ObjectContext) && this.ObjectContext != null)
      {
        if (this.UsingNoTracking != targetRelatedEnd.UsingNoTracking)
          throw Error.RelatedEnd_CannotCreateRelationshipBetweenTrackedAndNoTrackedEntities(this.UsingNoTracking ? (object) this._navigation.From : (object) this._navigation.To);
      }
      else if (this.ObjectContext != null && targetRelatedEnd.ObjectContext != null)
      {
        if (!this.UsingNoTracking || !targetRelatedEnd.UsingNoTracking)
          throw Error.RelatedEnd_CannotCreateRelationshipEntitiesInDifferentContexts();
        targetRelatedEnd.WrappedOwner.ResetContext(this.ObjectContext, this.GetTargetEntitySetFromRelationshipSet(), MergeOption.NoTracking);
      }
      else
      {
        if (this._context != null && !this.UsingNoTracking || (targetRelatedEnd.ObjectContext == null || targetRelatedEnd.UsingNoTracking))
          return;
        targetRelatedEnd.ValidateStateForAdd(targetRelatedEnd.WrappedOwner);
      }
    }

    private void SynchronizeContexts(
      RelatedEnd targetRelatedEnd,
      bool relationshipAlreadyExists,
      bool addRelationshipAsUnchanged)
    {
      IEntityWrapper wrappedOwner = targetRelatedEnd.WrappedOwner;
      if (object.ReferenceEquals((object) this.ObjectContext, (object) targetRelatedEnd.ObjectContext) && this.ObjectContext != null)
      {
        if (!this.IsForeignKey && !relationshipAlreadyExists && !this.UsingNoTracking)
        {
          if (!this.ObjectContext.ObjectStateManager.TransactionManager.IsLocalPublicAPI && this.WrappedOwner.EntityKey != (EntityKey) null && (!this.WrappedOwner.EntityKey.IsTemporary && this.IsDependentEndOfReferentialConstraint(false)))
            addRelationshipAsUnchanged = true;
          this.AddRelationshipToObjectStateManager(wrappedOwner, addRelationshipAsUnchanged, false);
        }
        if (!wrappedOwner.RequiresRelationshipChangeTracking || !this.ObjectContext.ObjectStateManager.TransactionManager.IsAddTracking && !this.ObjectContext.ObjectStateManager.TransactionManager.IsAttachTracking && !this.ObjectContext.ObjectStateManager.TransactionManager.IsDetectChanges)
          return;
        this.AddToNavigationProperty(wrappedOwner);
        targetRelatedEnd.AddToNavigationProperty(this._wrappedOwner);
      }
      else
      {
        if (this.ObjectContext == null && targetRelatedEnd.ObjectContext == null)
          return;
        RelatedEnd relatedEnd;
        IEntityWrapper wrappedEntity;
        if (this.ObjectContext == null)
        {
          relatedEnd = targetRelatedEnd;
          wrappedEntity = this._wrappedOwner;
        }
        else
        {
          relatedEnd = this;
          wrappedEntity = wrappedOwner;
        }
        if (relatedEnd.UsingNoTracking)
          return;
        TransactionManager transactionManager = relatedEnd.WrappedOwner.Context.ObjectStateManager.TransactionManager;
        transactionManager.BeginAddTracking();
        try
        {
          bool flag = true;
          try
          {
            if (transactionManager.TrackProcessedEntities)
            {
              if (!transactionManager.WrappedEntities.ContainsKey(wrappedEntity.Entity))
                transactionManager.WrappedEntities.Add(wrappedEntity.Entity, wrappedEntity);
              transactionManager.ProcessedEntities.Add(relatedEnd.WrappedOwner);
            }
            relatedEnd.AddGraphToObjectStateManager(wrappedEntity, relationshipAlreadyExists, addRelationshipAsUnchanged, false);
            if (wrappedEntity.RequiresRelationshipChangeTracking && this.TargetAccessor.HasProperty)
              targetRelatedEnd.AddToNavigationProperty(this._wrappedOwner);
            flag = false;
          }
          finally
          {
            if (flag)
            {
              relatedEnd.WrappedOwner.Context.ObjectStateManager.DegradePromotedRelationships();
              relatedEnd.FixupOtherEndOfRelationshipForRemove(wrappedEntity, false);
              relatedEnd.RemoveFromCache(wrappedEntity, false, false);
              wrappedEntity.RelationshipManager.NodeVisited = true;
              RelationshipManager.RemoveRelatedEntitiesFromObjectStateManager(wrappedEntity);
              RelatedEnd.RemoveEntityFromObjectStateManager(wrappedEntity);
            }
          }
        }
        finally
        {
          transactionManager.EndAddTracking();
        }
      }
    }

    private void AddGraphToObjectStateManager(
      IEntityWrapper wrappedEntity,
      bool relationshipAlreadyExists,
      bool addRelationshipAsUnchanged,
      bool doAttach)
    {
      this.AddEntityToObjectStateManager(wrappedEntity, doAttach);
      if (!relationshipAlreadyExists && this.ObjectContext != null && wrappedEntity.Context != null)
      {
        if (!this.IsForeignKey)
          this.AddRelationshipToObjectStateManager(wrappedEntity, addRelationshipAsUnchanged, doAttach);
        if (wrappedEntity.RequiresRelationshipChangeTracking || this.WrappedOwner.RequiresRelationshipChangeTracking)
        {
          this.UpdateSnapshotOfRelationships(wrappedEntity);
          if (doAttach)
          {
            EntityEntry entityEntry = this._context.ObjectStateManager.GetEntityEntry(wrappedEntity.Entity);
            wrappedEntity.RelationshipManager.CheckReferentialConstraintProperties(entityEntry);
          }
        }
      }
      RelatedEnd.WalkObjectGraphToIncludeAllRelatedEntities(wrappedEntity, addRelationshipAsUnchanged, doAttach);
    }

    private void UpdateSnapshotOfRelationships(IEntityWrapper wrappedEntity)
    {
      RelatedEnd endOfRelationship = this.GetOtherEndOfRelationship(wrappedEntity);
      if (endOfRelationship.ContainsEntity(this.WrappedOwner))
        return;
      endOfRelationship.AddToLocalCache(this.WrappedOwner, false);
    }

    internal void Remove(
      IEntityWrapper wrappedEntity,
      bool doFixup,
      bool deleteEntity,
      bool deleteOwner,
      bool applyReferentialConstraints,
      bool preserveForeignKey)
    {
      if (wrappedEntity.RequiresRelationshipChangeTracking && doFixup && (this.TargetAccessor.HasProperty && !this.CheckIfNavigationPropertyContainsEntity(wrappedEntity)))
        this.GetOtherEndOfRelationship(wrappedEntity).RemoveFromNavigationProperty(this.WrappedOwner);
      if (!this.ContainsEntity(wrappedEntity))
        return;
      if (this._context != null && doFixup && (applyReferentialConstraints && this.IsDependentEndOfReferentialConstraint(false)))
      {
        this.GetOtherEndOfRelationship(wrappedEntity).Remove(this._wrappedOwner, doFixup, deleteEntity, deleteOwner, applyReferentialConstraints, preserveForeignKey);
      }
      else
      {
        bool flag = this.RemoveFromCache(wrappedEntity, false, preserveForeignKey);
        if (!this.UsingNoTracking && this.ObjectContext != null && !this.IsForeignKey)
          RelatedEnd.MarkRelationshipAsDeletedInObjectStateManager(wrappedEntity, this._wrappedOwner, this._relationshipSet, this._navigation);
        if (doFixup)
        {
          this.FixupOtherEndOfRelationshipForRemove(wrappedEntity, preserveForeignKey);
          if ((this._context == null || !this._context.ObjectStateManager.TransactionManager.IsLocalPublicAPI) && this._context != null && (deleteEntity || deleteOwner && RelatedEnd.CheckCascadeDeleteFlag(this._fromEndMember) || applyReferentialConstraints && this.IsPrincipalEndOfReferentialConstraint()) && (!object.ReferenceEquals(wrappedEntity.Entity, this._context.ObjectStateManager.TransactionManager.EntityBeingReparented) && !object.ReferenceEquals(this._context.ObjectStateManager.EntityInvokingFKSetter, wrappedEntity.Entity)))
          {
            this.EnsureRelationshipNavigationAccessorsInitialized();
            RelatedEnd.RemoveEntityFromRelatedEnds(wrappedEntity, this._wrappedOwner, this._navigation.Reverse);
            RelatedEnd.MarkEntityAsDeletedInObjectStateManager(wrappedEntity);
          }
        }
        if (!flag)
          return;
        this.OnAssociationChanged(CollectionChangeAction.Remove, wrappedEntity.Entity);
      }
    }

    internal bool IsDependentEndOfReferentialConstraint(bool checkIdentifying)
    {
      if (this._relationMetadata != null)
      {
        foreach (ReferentialConstraint referentialConstraint in ((AssociationType) this.RelationMetadata).ReferentialConstraints)
        {
          if (referentialConstraint.ToRole == this.FromEndMember)
          {
            if (checkIdentifying)
              return RelatedEnd.CheckIfAllPropertiesAreKeyProperties(referentialConstraint.ToRole.GetEntityType().KeyMemberNames, referentialConstraint.ToProperties);
            return true;
          }
        }
      }
      return false;
    }

    internal bool IsPrincipalEndOfReferentialConstraint()
    {
      if (this._relationMetadata != null)
      {
        foreach (ReferentialConstraint referentialConstraint in ((AssociationType) this._relationMetadata).ReferentialConstraints)
        {
          if (referentialConstraint.FromRole == this._fromEndMember)
            return RelatedEnd.CheckIfAllPropertiesAreKeyProperties(referentialConstraint.ToRole.GetEntityType().KeyMemberNames, referentialConstraint.ToProperties);
        }
      }
      return false;
    }

    internal static bool CheckIfAllPropertiesAreKeyProperties(
      string[] keyMemberNames,
      ReadOnlyMetadataCollection<EdmProperty> toProperties)
    {
      foreach (EdmProperty toProperty in toProperties)
      {
        bool flag = false;
        foreach (string keyMemberName in keyMemberNames)
        {
          if (keyMemberName == toProperty.Name)
          {
            flag = true;
            break;
          }
        }
        if (!flag)
          return false;
      }
      return true;
    }

    internal void IncludeEntity(
      IEntityWrapper wrappedEntity,
      bool addRelationshipAsUnchanged,
      bool doAttach)
    {
      EntityEntry entityEntry1 = this._context.ObjectStateManager.FindEntityEntry(wrappedEntity.Entity);
      if (entityEntry1 != null && entityEntry1.State == EntityState.Deleted)
        throw Error.RelatedEnd_UnableToAddRelationshipWithDeletedEntity();
      if (wrappedEntity.RequiresRelationshipChangeTracking || this.WrappedOwner.RequiresRelationshipChangeTracking)
      {
        RelatedEnd endOfRelationship = this.GetOtherEndOfRelationship(wrappedEntity);
        this.ObjectContext.GetTypeUsage(endOfRelationship.WrappedOwner.IdentityType);
        endOfRelationship.AddToNavigationPropertyIfCompatible(this);
      }
      if (entityEntry1 == null)
      {
        this.AddGraphToObjectStateManager(wrappedEntity, false, addRelationshipAsUnchanged, doAttach);
      }
      else
      {
        if (this.FindRelationshipEntryInObjectStateManager(wrappedEntity) != null)
          return;
        this.VerifyDetachedKeyMatches(wrappedEntity.EntityKey);
        if (this.ObjectContext == null || wrappedEntity.Context == null)
          return;
        if (!this.IsForeignKey)
        {
          if (entityEntry1.State == EntityState.Added)
            this.AddRelationshipToObjectStateManager(wrappedEntity, addRelationshipAsUnchanged, false);
          else
            this.AddRelationshipToObjectStateManager(wrappedEntity, addRelationshipAsUnchanged, doAttach);
        }
        if (!wrappedEntity.RequiresRelationshipChangeTracking && !this.WrappedOwner.RequiresRelationshipChangeTracking)
          return;
        this.UpdateSnapshotOfRelationships(wrappedEntity);
        if (!doAttach || entityEntry1.State == EntityState.Added)
          return;
        EntityEntry entityEntry2 = this.ObjectContext.ObjectStateManager.GetEntityEntry(wrappedEntity.Entity);
        wrappedEntity.RelationshipManager.CheckReferentialConstraintProperties(entityEntry2);
      }
    }

    internal void MarkForeignKeyPropertiesModified()
    {
      ReferentialConstraint referentialConstraint = ((AssociationType) this.RelationMetadata).ReferentialConstraints[0];
      EntityEntry objectStateEntry = this.WrappedOwner.ObjectStateEntry;
      if (objectStateEntry.State != EntityState.Unchanged && objectStateEntry.State != EntityState.Modified)
        return;
      foreach (EdmProperty toProperty in referentialConstraint.ToProperties)
        objectStateEntry.SetModifiedProperty(toProperty.Name);
    }

    internal abstract bool CheckIfNavigationPropertyContainsEntity(IEntityWrapper wrapper);

    internal abstract void VerifyNavigationPropertyForAdd(IEntityWrapper wrapper);

    internal void AddToNavigationProperty(IEntityWrapper wrapper)
    {
      if (!this.TargetAccessor.HasProperty || this.CheckIfNavigationPropertyContainsEntity(wrapper))
        return;
      TransactionManager transactionManager = wrapper.Context.ObjectStateManager.TransactionManager;
      if (transactionManager.IsAddTracking || transactionManager.IsAttachTracking)
        wrapper.Context.ObjectStateManager.TrackPromotedRelationship(this, wrapper);
      this.AddToObjectCache(wrapper);
    }

    internal void RemoveFromNavigationProperty(IEntityWrapper wrapper)
    {
      if (!this.TargetAccessor.HasProperty || !this.CheckIfNavigationPropertyContainsEntity(wrapper))
        return;
      this.RemoveFromObjectCache(wrapper);
    }

    internal void ExcludeEntity(IEntityWrapper wrappedEntity)
    {
      if (this._context.ObjectStateManager.TransactionManager.TrackProcessedEntities && (this._context.ObjectStateManager.TransactionManager.IsAttachTracking || this._context.ObjectStateManager.TransactionManager.IsAddTracking) && !this._context.ObjectStateManager.TransactionManager.ProcessedEntities.Contains(wrappedEntity))
        return;
      EntityEntry entityEntry = this._context.ObjectStateManager.FindEntityEntry(wrappedEntity.Entity);
      if (entityEntry != null && entityEntry.State != EntityState.Deleted && !wrappedEntity.RelationshipManager.NodeVisited)
      {
        wrappedEntity.RelationshipManager.NodeVisited = true;
        RelationshipManager.RemoveRelatedEntitiesFromObjectStateManager(wrappedEntity);
        if (!this.IsForeignKey)
          RelatedEnd.RemoveRelationshipFromObjectStateManager(wrappedEntity, this._wrappedOwner, this._relationshipSet, this._navigation);
        RelatedEnd.RemoveEntityFromObjectStateManager(wrappedEntity);
      }
      else
      {
        if (this.IsForeignKey || this.FindRelationshipEntryInObjectStateManager(wrappedEntity) == null)
          return;
        RelatedEnd.RemoveRelationshipFromObjectStateManager(wrappedEntity, this._wrappedOwner, this._relationshipSet, this._navigation);
      }
    }

    internal RelationshipEntry FindRelationshipEntryInObjectStateManager(
      IEntityWrapper wrappedEntity)
    {
      return this._context.ObjectStateManager.FindRelationship(this._relationshipSet, new KeyValuePair<string, EntityKey>(this._navigation.From, this._wrappedOwner.EntityKey), new KeyValuePair<string, EntityKey>(this._navigation.To, wrappedEntity.EntityKey));
    }

    internal void Clear(
      IEntityWrapper wrappedEntity,
      RelationshipNavigation navigation,
      bool doCascadeDelete)
    {
      this.ClearCollectionOrRef(wrappedEntity, navigation, doCascadeDelete);
    }

    internal void CheckReferentialConstraintProperties(EntityEntry ownerEntry)
    {
      foreach (ReferentialConstraint referentialConstraint in ((AssociationType) this.RelationMetadata).ReferentialConstraints)
      {
        if (referentialConstraint.ToRole == this.FromEndMember)
        {
          if (!this.CheckReferentialConstraintPrincipalProperty(ownerEntry, referentialConstraint))
            throw new InvalidOperationException(referentialConstraint.BuildConstraintExceptionMessage());
        }
        else if (referentialConstraint.FromRole == this.FromEndMember && !this.CheckReferentialConstraintDependentProperty(ownerEntry, referentialConstraint))
          throw new InvalidOperationException(referentialConstraint.BuildConstraintExceptionMessage());
      }
    }

    internal virtual bool CheckReferentialConstraintPrincipalProperty(
      EntityEntry ownerEntry,
      ReferentialConstraint constraint)
    {
      return false;
    }

    internal virtual bool CheckReferentialConstraintDependentProperty(
      EntityEntry ownerEntry,
      ReferentialConstraint constraint)
    {
      if (!this.IsEmpty())
      {
        foreach (IEntityWrapper wrappedEntity in this.GetWrappedEntities())
        {
          EntityEntry objectStateEntry = wrappedEntity.ObjectStateEntry;
          if (objectStateEntry != null && objectStateEntry.State != EntityState.Added && (objectStateEntry.State != EntityState.Deleted && objectStateEntry.State != EntityState.Detached) && !RelatedEnd.VerifyRIConstraintsWithRelatedEntry(constraint, new Func<string, object>(objectStateEntry.GetCurrentEntityValue), ownerEntry.EntityKey))
            return false;
        }
      }
      return true;
    }

    internal static bool VerifyRIConstraintsWithRelatedEntry(
      ReferentialConstraint constraint,
      Func<string, object> getDependentPropertyValue,
      EntityKey principalKey)
    {
      for (int index = 0; index < constraint.FromProperties.Count; ++index)
      {
        string name1 = constraint.FromProperties[index].Name;
        string name2 = constraint.ToProperties[index].Name;
        object valueByName = principalKey.FindValueByName(name1);
        object y = getDependentPropertyValue(name2);
        if (!ByValueEqualityComparer.Default.Equals(valueByName, y))
          return false;
      }
      return true;
    }

    /// <summary>
    /// Returns an <see cref="T:System.Collections.IEnumerator" /> that iterates through the collection of related objects.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator" /> that iterates through the collection of related objects.
    /// </returns>
    public IEnumerator GetEnumerator()
    {
      this.DeferredLoad();
      return this.GetInternalEnumerable().GetEnumerator();
    }

    internal void RemoveAll()
    {
      List<IEntityWrapper> entityWrapperList = (List<IEntityWrapper>) null;
      bool flag = false;
      try
      {
        this._suppressEvents = true;
        foreach (IEntityWrapper wrappedEntity in this.GetWrappedEntities())
        {
          if (entityWrapperList == null)
            entityWrapperList = new List<IEntityWrapper>();
          entityWrapperList.Add(wrappedEntity);
        }
        if (flag = entityWrapperList != null && entityWrapperList.Count > 0)
        {
          foreach (IEntityWrapper wrappedEntity in entityWrapperList)
            this.Remove(wrappedEntity, true, false, true, true, false);
        }
      }
      finally
      {
        this._suppressEvents = false;
      }
      if (!flag)
        return;
      this.OnAssociationChanged(CollectionChangeAction.Refresh, (object) null);
    }

    internal virtual void DetachAll(EntityState ownerEntityState)
    {
      List<IEntityWrapper> entityWrapperList = new List<IEntityWrapper>();
      foreach (IEntityWrapper wrappedEntity in this.GetWrappedEntities())
        entityWrapperList.Add(wrappedEntity);
      bool flag = ownerEntityState == EntityState.Added || this._fromEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many;
      foreach (IEntityWrapper wrappedEntity in entityWrapperList)
      {
        if (!this.ContainsEntity(wrappedEntity))
          return;
        if (flag)
          RelatedEnd.DetachRelationshipFromObjectStateManager(wrappedEntity, this._wrappedOwner, this._relationshipSet, this._navigation);
        RelatedEnd endOfRelationship = this.GetOtherEndOfRelationship(wrappedEntity);
        endOfRelationship.RemoveFromCache(this._wrappedOwner, true, false);
        endOfRelationship.OnAssociationChanged(CollectionChangeAction.Remove, this._wrappedOwner.Entity);
      }
      foreach (IEntityWrapper wrappedEntity in entityWrapperList)
      {
        this.GetOtherEndOfRelationship(wrappedEntity);
        this.RemoveFromCache(wrappedEntity, false, false);
      }
      this.OnAssociationChanged(CollectionChangeAction.Refresh, (object) null);
    }

    internal void AddToCache(IEntityWrapper wrappedEntity, bool applyConstraints)
    {
      this.AddToLocalCache(wrappedEntity, applyConstraints);
      this.AddToObjectCache(wrappedEntity);
    }

    internal abstract void AddToLocalCache(IEntityWrapper wrappedEntity, bool applyConstraints);

    internal abstract void AddToObjectCache(IEntityWrapper wrappedEntity);

    internal bool RemoveFromCache(
      IEntityWrapper wrappedEntity,
      bool resetIsLoaded,
      bool preserveForeignKey)
    {
      bool flag = this.RemoveFromLocalCache(wrappedEntity, resetIsLoaded, preserveForeignKey);
      this.RemoveFromObjectCache(wrappedEntity);
      return flag;
    }

    internal abstract bool RemoveFromLocalCache(
      IEntityWrapper wrappedEntity,
      bool resetIsLoaded,
      bool preserveForeignKey);

    internal abstract bool RemoveFromObjectCache(IEntityWrapper wrappedEntity);

    internal abstract bool VerifyEntityForAdd(
      IEntityWrapper wrappedEntity,
      bool relationshipAlreadyExists);

    internal abstract void VerifyType(IEntityWrapper wrappedEntity);

    internal abstract bool CanSetEntityType(IEntityWrapper wrappedEntity);

    internal abstract void Include(bool addRelationshipAsUnchanged, bool doAttach);

    internal abstract void Exclude();

    internal abstract void ClearCollectionOrRef(
      IEntityWrapper wrappedEntity,
      RelationshipNavigation navigation,
      bool doCascadeDelete);

    internal abstract bool ContainsEntity(IEntityWrapper wrappedEntity);

    internal abstract IEnumerable GetInternalEnumerable();

    internal abstract IEnumerable<IEntityWrapper> GetWrappedEntities();

    internal abstract void RetrieveReferentialConstraintProperties(
      Dictionary<string, KeyValuePair<object, IntBox>> keyValues,
      HashSet<object> visited);

    internal abstract bool IsEmpty();

    internal abstract void OnRelatedEndClear();

    internal abstract void ClearWrappedValues();

    internal abstract void VerifyMultiplicityConstraintsForAdd(bool applyConstraints);

    internal virtual void OnAssociationChanged(
      CollectionChangeAction collectionChangeAction,
      object entity)
    {
      if (this._suppressEvents || this._onAssociationChanged == null)
        return;
      this._onAssociationChanged((object) this, new CollectionChangeEventArgs(collectionChangeAction, entity));
    }

    internal virtual void AddEntityToObjectStateManager(IEntityWrapper wrappedEntity, bool doAttach)
    {
      EntitySet fromRelationshipSet = this.GetTargetEntitySetFromRelationshipSet();
      if (!doAttach)
        this._context.AddSingleObject(fromRelationshipSet, wrappedEntity, "entity");
      else
        this._context.AttachSingleObject(wrappedEntity, fromRelationshipSet);
    }

    internal EntitySet GetTargetEntitySetFromRelationshipSet()
    {
      return ((AssociationSet) this._relationshipSet).AssociationSetEnds[this.ToEndMember.Name].EntitySet;
    }

    private RelationshipEntry AddRelationshipToObjectStateManager(
      IEntityWrapper wrappedEntity,
      bool addRelationshipAsUnchanged,
      bool doAttach)
    {
      EntityKey entityKey1 = this._wrappedOwner.EntityKey;
      EntityKey entityKey2 = wrappedEntity.EntityKey;
      if ((object) entityKey1 == null)
        throw Error.EntityKey_UnexpectedNull();
      if ((object) entityKey2 == null)
        throw Error.EntityKey_UnexpectedNull();
      return this.ObjectContext.ObjectStateManager.AddRelation(new RelationshipWrapper((AssociationSet) this._relationshipSet, new KeyValuePair<string, EntityKey>(this._navigation.From, entityKey1), new KeyValuePair<string, EntityKey>(this._navigation.To, entityKey2)), addRelationshipAsUnchanged || doAttach ? EntityState.Unchanged : EntityState.Added);
    }

    private static void WalkObjectGraphToIncludeAllRelatedEntities(
      IEntityWrapper wrappedEntity,
      bool addRelationshipAsUnchanged,
      bool doAttach)
    {
      foreach (RelatedEnd relationship in wrappedEntity.RelationshipManager.Relationships)
        relationship.Include(addRelationshipAsUnchanged, doAttach);
    }

    internal static void RemoveEntityFromObjectStateManager(IEntityWrapper wrappedEntity)
    {
      EntityEntry entityEntry1;
      if (wrappedEntity.Context != null && wrappedEntity.Context.ObjectStateManager.TransactionManager.IsAttachTracking && wrappedEntity.Context.ObjectStateManager.TransactionManager.PromotedKeyEntries.TryGetValue(wrappedEntity.Entity, out entityEntry1))
      {
        entityEntry1.DegradeEntry();
      }
      else
      {
        EntityEntry entityEntry2 = RelatedEnd.MarkEntityAsDeletedInObjectStateManager(wrappedEntity);
        if (entityEntry2 == null || entityEntry2.State == EntityState.Detached)
          return;
        entityEntry2.AcceptChanges();
      }
    }

    private static void RemoveRelationshipFromObjectStateManager(
      IEntityWrapper wrappedEntity,
      IEntityWrapper wrappedOwner,
      RelationshipSet relationshipSet,
      RelationshipNavigation navigation)
    {
      RelationshipEntry relationshipEntry = RelatedEnd.MarkRelationshipAsDeletedInObjectStateManager(wrappedEntity, wrappedOwner, relationshipSet, navigation);
      if (relationshipEntry == null || relationshipEntry.State == EntityState.Detached)
        return;
      relationshipEntry.AcceptChanges();
    }

    private void FixupOtherEndOfRelationshipForRemove(
      IEntityWrapper wrappedEntity,
      bool preserveForeignKey)
    {
      RelatedEnd endOfRelationship = this.GetOtherEndOfRelationship(wrappedEntity);
      endOfRelationship.Remove(this._wrappedOwner, false, false, false, false, preserveForeignKey);
      endOfRelationship.RemoveFromNavigationProperty(this._wrappedOwner);
    }

    private static EntityEntry MarkEntityAsDeletedInObjectStateManager(
      IEntityWrapper wrappedEntity)
    {
      EntityEntry entityEntry = (EntityEntry) null;
      if (wrappedEntity.Context != null)
      {
        entityEntry = wrappedEntity.Context.ObjectStateManager.FindEntityEntry(wrappedEntity.Entity);
        entityEntry?.Delete(false);
      }
      return entityEntry;
    }

    private static RelationshipEntry MarkRelationshipAsDeletedInObjectStateManager(
      IEntityWrapper wrappedEntity,
      IEntityWrapper wrappedOwner,
      RelationshipSet relationshipSet,
      RelationshipNavigation navigation)
    {
      RelationshipEntry relationshipEntry = (RelationshipEntry) null;
      if (wrappedOwner.Context != null && wrappedEntity.Context != null && relationshipSet != null)
      {
        EntityKey entityKey1 = wrappedOwner.EntityKey;
        EntityKey entityKey2 = wrappedEntity.EntityKey;
        relationshipEntry = wrappedEntity.Context.ObjectStateManager.DeleteRelationship(relationshipSet, new KeyValuePair<string, EntityKey>(navigation.From, entityKey1), new KeyValuePair<string, EntityKey>(navigation.To, entityKey2));
      }
      return relationshipEntry;
    }

    private static void DetachRelationshipFromObjectStateManager(
      IEntityWrapper wrappedEntity,
      IEntityWrapper wrappedOwner,
      RelationshipSet relationshipSet,
      RelationshipNavigation navigation)
    {
      if (wrappedOwner.Context == null || wrappedEntity.Context == null || relationshipSet == null)
        return;
      EntityKey entityKey1 = wrappedOwner.EntityKey;
      EntityKey entityKey2 = wrappedEntity.EntityKey;
      wrappedEntity.Context.ObjectStateManager.FindRelationship(relationshipSet, new KeyValuePair<string, EntityKey>(navigation.From, entityKey1), new KeyValuePair<string, EntityKey>(navigation.To, entityKey2))?.DetachRelationshipEntry();
    }

    private static void RemoveEntityFromRelatedEnds(
      IEntityWrapper wrappedEntity1,
      IEntityWrapper wrappedEntity2,
      RelationshipNavigation navigation)
    {
      foreach (RelatedEnd relationship in wrappedEntity1.RelationshipManager.Relationships)
      {
        bool doCascadeDelete = RelatedEnd.CheckCascadeDeleteFlag(relationship.FromEndMember) || relationship.IsPrincipalEndOfReferentialConstraint();
        relationship.Clear(wrappedEntity2, navigation, doCascadeDelete);
      }
    }

    private static bool CheckCascadeDeleteFlag(RelationshipEndMember relationEndProperty)
    {
      if (relationEndProperty != null)
        return relationEndProperty.DeleteBehavior == OperationAction.Cascade;
      return false;
    }

    internal void AttachContext(ObjectContext context, MergeOption mergeOption)
    {
      if (this._wrappedOwner.InitializingProxyRelatedEnds)
        return;
      EntityKey entityKey = this._wrappedOwner.EntityKey;
      if ((object) entityKey == null)
        throw Error.EntityKey_UnexpectedNull();
      EntitySet entitySet = entityKey.GetEntitySet(context.MetadataWorkspace);
      this.AttachContext(context, entitySet, mergeOption);
    }

    [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
    internal void AttachContext(
      ObjectContext context,
      EntitySet entitySet,
      MergeOption mergeOption)
    {
      EntityUtil.CheckArgumentMergeOption(mergeOption);
      this._wrappedOwner.RelationshipManager.NodeVisited = false;
      if (this._context == context && this._usingNoTracking == (mergeOption == MergeOption.NoTracking))
        return;
      bool flag1 = true;
      try
      {
        this._sourceQuery = (string) null;
        this._context = context;
        this._entityWrapperFactory = context.EntityWrapperFactory;
        this._usingNoTracking = mergeOption == MergeOption.NoTracking;
        EdmType relationshipType;
        RelationshipSet relationshipSet;
        this.FindRelationshipSet(this._context, entitySet, out relationshipType, out relationshipSet);
        if (relationshipSet != null)
        {
          this._relationshipSet = relationshipSet;
          this._relationMetadata = (RelationshipType) relationshipType;
          bool flag2 = false;
          bool flag3 = false;
          foreach (AssociationEndMember associationEndMember in ((AssociationType) this._relationMetadata).AssociationEndMembers)
          {
            if (associationEndMember.Name == this._navigation.From)
            {
              flag2 = true;
              this._fromEndMember = (RelationshipEndMember) associationEndMember;
            }
            if (associationEndMember.Name == this._navigation.To)
            {
              flag3 = true;
              this._toEndMember = (RelationshipEndMember) associationEndMember;
            }
          }
          if (!flag2 || !flag3)
            throw Error.RelatedEnd_RelatedEndNotFound();
          this.ValidateDetachedEntityKey();
          flag1 = false;
        }
        else
        {
          foreach (EntitySetBase baseEntitySet in entitySet.EntityContainer.BaseEntitySets)
          {
            AssociationSet associationSet = baseEntitySet as AssociationSet;
            if (associationSet != null && associationSet.ElementType == relationshipType && (associationSet.AssociationSetEnds[this._navigation.From].EntitySet != entitySet && associationSet.AssociationSetEnds[this._navigation.From].EntitySet.ElementType == entitySet.ElementType))
              throw Error.RelatedEnd_EntitySetIsNotValidForRelationship((object) entitySet.EntityContainer.Name, (object) entitySet.Name, (object) this._navigation.From, (object) baseEntitySet.EntityContainer.Name, (object) baseEntitySet.Name);
          }
          throw Error.Collections_NoRelationshipSetMatched((object) this._navigation.RelationshipName);
        }
      }
      finally
      {
        if (flag1)
          this.DetachContext();
      }
    }

    internal virtual void ValidateDetachedEntityKey()
    {
    }

    internal void FindRelationshipSet(
      ObjectContext context,
      EntitySet entitySet,
      out EdmType relationshipType,
      out RelationshipSet relationshipSet)
    {
      if (this._navigation.AssociationType == null || this._navigation.AssociationType.Index < 0)
      {
        RelatedEnd.FindRelationshipSet(context, this._navigation, entitySet, out relationshipType, out relationshipSet);
      }
      else
      {
        MetadataOptimization metadataOptimization = context.MetadataWorkspace.MetadataOptimization;
        AssociationType cspaceAssociationType = metadataOptimization.GetCSpaceAssociationType(this._navigation.AssociationType);
        relationshipType = (EdmType) cspaceAssociationType;
        relationshipSet = (RelationshipSet) metadataOptimization.FindCSpaceAssociationSet(cspaceAssociationType, this._navigation.From, entitySet);
      }
    }

    internal static void FindRelationshipSet(
      ObjectContext context,
      RelationshipNavigation navigation,
      EntitySet entitySet,
      out EdmType relationshipType,
      out RelationshipSet relationshipSet)
    {
      relationshipType = context.MetadataWorkspace.GetItem<EdmType>(navigation.RelationshipName, DataSpace.CSpace);
      if (relationshipType == null)
        throw Error.Collections_NoRelationshipSetMatched((object) navigation.RelationshipName);
      foreach (EntitySetBase baseEntitySet in entitySet.EntityContainer.BaseEntitySets)
      {
        if (baseEntitySet.ElementType == relationshipType && ((AssociationSet) baseEntitySet).AssociationSetEnds[navigation.From].EntitySet == entitySet)
        {
          relationshipSet = (RelationshipSet) baseEntitySet;
          return;
        }
      }
      relationshipSet = (RelationshipSet) null;
    }

    internal void DetachContext()
    {
      if (this._context != null && this.ObjectContext.ObjectStateManager.TransactionManager.IsAttachTracking)
      {
        MergeOption? originalMergeOption = this.ObjectContext.ObjectStateManager.TransactionManager.OriginalMergeOption;
        if ((originalMergeOption.GetValueOrDefault() != MergeOption.NoTracking ? 0 : (originalMergeOption.HasValue ? 1 : 0)) != 0)
        {
          this._usingNoTracking = true;
          return;
        }
      }
      this._sourceQuery = (string) null;
      this._context = (ObjectContext) null;
      this._relationshipSet = (RelationshipSet) null;
      this._fromEndMember = (RelationshipEndMember) null;
      this._toEndMember = (RelationshipEndMember) null;
      this._relationMetadata = (RelationshipType) null;
      this._isLoaded = false;
    }

    internal RelatedEnd GetOtherEndOfRelationship(IEntityWrapper wrappedEntity)
    {
      this.EnsureRelationshipNavigationAccessorsInitialized();
      return wrappedEntity.RelationshipManager.GetRelatedEnd(this._navigation.Reverse, this._relationshipFixer);
    }

    internal virtual void CheckOwnerNull()
    {
      if (this._wrappedOwner.Entity == null)
        throw Error.RelatedEnd_OwnerIsNull();
    }

    internal void InitializeRelatedEnd(
      IEntityWrapper wrappedOwner,
      RelationshipNavigation navigation,
      IRelationshipFixer relationshipFixer)
    {
      this.SetWrappedOwner(wrappedOwner);
      this._navigation = navigation;
      this._relationshipFixer = relationshipFixer;
    }

    internal void SetWrappedOwner(IEntityWrapper wrappedOwner)
    {
      this._wrappedOwner = wrappedOwner != null ? wrappedOwner : NullEntityWrapper.NullWrapper;
      this._owner = wrappedOwner.Entity as IEntityWithRelationships;
    }

    internal static bool IsValidEntityKeyType(EntityKey entityKey)
    {
      if (!entityKey.IsTemporary && !object.ReferenceEquals((object) EntityKey.EntityNotValidKey, (object) entityKey))
        return !object.ReferenceEquals((object) EntityKey.NoEntitySetKey, (object) entityKey);
      return false;
    }

    /// <summary>
    /// Used internally to deserialize entity objects along with the
    /// <see cref="T:System.Data.Entity.Core.Objects.DataClasses.RelationshipManager" />
    /// instances.
    /// </summary>
    /// <param name="context">The serialized stream.</param>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [SuppressMessage("Microsoft.Usage", "CA2238:ImplementSerializationMethodsCorrectly")]
    [OnDeserialized]
    public void OnDeserialized(StreamingContext context)
    {
      this._wrappedOwner = this.EntityWrapperFactory.WrapEntityUsingContext((object) this._owner, this.ObjectContext);
    }

    internal NavigationProperty NavigationProperty
    {
      get
      {
        if (this.navigationPropertyCache == null && this._wrappedOwner.Context != null && this.TargetAccessor.HasProperty)
        {
          string propertyName = this.TargetAccessor.PropertyName;
          NavigationProperty navigationProperty;
          if (!this._wrappedOwner.Context.MetadataWorkspace.GetItem<EntityType>(this._wrappedOwner.IdentityType.FullNameWithNesting(), DataSpace.OSpace).NavigationProperties.TryGetValue(propertyName, false, out navigationProperty))
            throw Error.RelationshipManager_NavigationPropertyNotFound((object) propertyName);
          this.navigationPropertyCache = navigationProperty;
        }
        return this.navigationPropertyCache;
      }
    }

    internal NavigationPropertyAccessor TargetAccessor
    {
      get
      {
        if (this._wrappedOwner.Entity == null)
          return NavigationPropertyAccessor.NoNavigationProperty;
        this.EnsureRelationshipNavigationAccessorsInitialized();
        return this.RelationshipNavigation.ToPropertyAccessor;
      }
    }

    private void EnsureRelationshipNavigationAccessorsInitialized()
    {
      if (this.RelationshipNavigation.IsInitialized)
        return;
      NavigationPropertyAccessor fromAccessor = (NavigationPropertyAccessor) null;
      NavigationPropertyAccessor toAccessor = (NavigationPropertyAccessor) null;
      string relationshipName = this._navigation.RelationshipName;
      string from = this._navigation.From;
      string to = this._navigation.To;
      AssociationType associationType = this.RelationMetadata as AssociationType ?? this._wrappedOwner.RelationshipManager.GetRelationshipType(relationshipName);
      AssociationEndMember end1;
      if (associationType.AssociationEndMembers.TryGetValue(from, false, out end1))
        toAccessor = MetadataHelper.GetNavigationPropertyAccessor(MetadataHelper.GetEntityTypeForEnd(end1), relationshipName, from, to);
      AssociationEndMember end2;
      if (associationType.AssociationEndMembers.TryGetValue(to, false, out end2))
        fromAccessor = MetadataHelper.GetNavigationPropertyAccessor(MetadataHelper.GetEntityTypeForEnd(end2), relationshipName, to, from);
      if (fromAccessor == null || toAccessor == null)
        throw RelationshipManager.UnableToGetMetadata(this.WrappedOwner, relationshipName);
      this.RelationshipNavigation.InitializeAccessors(fromAccessor, toAccessor);
    }

    internal bool DisableLazyLoading()
    {
      if (this._context == null)
        return false;
      bool lazyLoadingEnabled = this._context.ContextOptions.LazyLoadingEnabled;
      this._context.ContextOptions.LazyLoadingEnabled = false;
      return lazyLoadingEnabled;
    }

    internal void ResetLazyLoading(bool state)
    {
      if (this._context == null)
        return;
      this._context.ContextOptions.LazyLoadingEnabled = state;
    }
  }
}
