// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.InternalEntityEntry
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Internal.Validation;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity.Internal
{
  internal class InternalEntityEntry
  {
    private readonly Type _entityType;
    private readonly InternalContext _internalContext;
    private readonly object _entity;
    private IEntityStateEntry _stateEntry;
    private System.Data.Entity.Core.Metadata.Edm.EntityType _edmEntityType;

    public InternalEntityEntry(InternalContext internalContext, IEntityStateEntry stateEntry)
    {
      this._internalContext = internalContext;
      this._stateEntry = stateEntry;
      this._entity = stateEntry.Entity;
      this._entityType = ObjectContextTypeCache.GetObjectType(this._entity.GetType());
    }

    public InternalEntityEntry(InternalContext internalContext, object entity)
    {
      this._internalContext = internalContext;
      this._entity = entity;
      this._entityType = ObjectContextTypeCache.GetObjectType(this._entity.GetType());
      this._stateEntry = this._internalContext.GetStateEntry(entity);
      if (this._stateEntry != null)
        return;
      this._internalContext.Set(this._entityType).InternalSet.Initialize();
    }

    public virtual object Entity
    {
      get
      {
        return this._entity;
      }
    }

    public virtual EntityState State
    {
      get
      {
        if (!this.IsDetached)
          return this._stateEntry.State;
        return EntityState.Detached;
      }
      set
      {
        if (!this.IsDetached)
        {
          if (this._stateEntry.State == EntityState.Modified && value == EntityState.Unchanged)
            this.CurrentValues.SetValues(this.OriginalValues);
          this._stateEntry.ChangeState(value);
        }
        else
        {
          switch (value)
          {
            case EntityState.Unchanged:
              this._internalContext.Set(this._entityType).InternalSet.Attach(this._entity);
              break;
            case EntityState.Added:
              this._internalContext.Set(this._entityType).InternalSet.Add(this._entity);
              break;
            case EntityState.Deleted:
            case EntityState.Modified:
              this._internalContext.Set(this._entityType).InternalSet.Attach(this._entity);
              this._stateEntry = this._internalContext.GetStateEntry(this._entity);
              this._stateEntry.ChangeState(value);
              break;
          }
        }
      }
    }

    public virtual InternalPropertyValues CurrentValues
    {
      get
      {
        this.ValidateStateToGetValues(nameof (CurrentValues), EntityState.Deleted);
        return (InternalPropertyValues) new DbDataRecordPropertyValues(this._internalContext, this._entityType, this._stateEntry.CurrentValues, true);
      }
    }

    public virtual InternalPropertyValues OriginalValues
    {
      get
      {
        this.ValidateStateToGetValues(nameof (OriginalValues), EntityState.Added);
        return (InternalPropertyValues) new DbDataRecordPropertyValues(this._internalContext, this._entityType, this._stateEntry.GetUpdatableOriginalValues(), true);
      }
    }

    public virtual InternalPropertyValues GetDatabaseValues()
    {
      this.ValidateStateToGetValues(nameof (GetDatabaseValues), EntityState.Added);
      DbDataRecord valuesRecord = this.GetDatabaseValuesQuery().SingleOrDefault<DbDataRecord>();
      if (valuesRecord != null)
        return (InternalPropertyValues) new ClonedPropertyValues(this.OriginalValues, valuesRecord);
      return (InternalPropertyValues) null;
    }

    public virtual async Task<InternalPropertyValues> GetDatabaseValuesAsync(
      CancellationToken cancellationToken)
    {
      this.ValidateStateToGetValues(nameof (GetDatabaseValuesAsync), EntityState.Added);
      cancellationToken.ThrowIfCancellationRequested();
      DbDataRecord dataRecord = await this.GetDatabaseValuesQuery().SingleOrDefaultAsync<DbDataRecord>(cancellationToken).WithCurrentCulture<DbDataRecord>();
      return dataRecord == null ? (InternalPropertyValues) null : (InternalPropertyValues) new ClonedPropertyValues(this.OriginalValues, dataRecord);
    }

    private ObjectQuery<DbDataRecord> GetDatabaseValuesQuery()
    {
      StringBuilder queryBuilder = new StringBuilder();
      queryBuilder.Append("SELECT ");
      this.AppendEntitySqlRow(queryBuilder, "X", this.OriginalValues);
      string str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) DbHelpers.QuoteIdentifier(this._stateEntry.EntitySet.EntityContainer.Name), (object) DbHelpers.QuoteIdentifier(this._stateEntry.EntitySet.Name));
      string str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) DbHelpers.QuoteIdentifier(this.EntityType.NestingNamespace()), (object) DbHelpers.QuoteIdentifier(this.EntityType.Name));
      queryBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, " FROM (SELECT VALUE TREAT (Y AS {0}) FROM {1} AS Y) AS X WHERE ", (object) str2, (object) str1);
      EntityKeyMember[] entityKeyValues = this._stateEntry.EntityKey.EntityKeyValues;
      ObjectParameter[] objectParameterArray = new ObjectParameter[entityKeyValues.Length];
      for (int index = 0; index < entityKeyValues.Length; ++index)
      {
        if (index > 0)
          queryBuilder.Append(" AND ");
        string name = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "p{0}", (object) index.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        queryBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "X.{0} = @{1}", (object) DbHelpers.QuoteIdentifier(entityKeyValues[index].Key), (object) name);
        objectParameterArray[index] = new ObjectParameter(name, entityKeyValues[index].Value);
      }
      return this._internalContext.ObjectContext.CreateQuery<DbDataRecord>(queryBuilder.ToString(), objectParameterArray);
    }

    private void AppendEntitySqlRow(
      StringBuilder queryBuilder,
      string prefix,
      InternalPropertyValues templateValues)
    {
      bool flag = false;
      foreach (string propertyName in (IEnumerable<string>) templateValues.PropertyNames)
      {
        if (flag)
          queryBuilder.Append(", ");
        else
          flag = true;
        string str = DbHelpers.QuoteIdentifier(propertyName);
        IPropertyValuesItem propertyValuesItem = templateValues.GetItem(propertyName);
        if (propertyValuesItem.IsComplex)
        {
          InternalPropertyValues templateValues1 = propertyValuesItem.Value as InternalPropertyValues;
          if (templateValues1 == null)
            throw Error.DbPropertyValues_CannotGetStoreValuesWhenComplexPropertyIsNull((object) propertyName, (object) this.EntityType.Name);
          queryBuilder.Append("ROW(");
          this.AppendEntitySqlRow(queryBuilder, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) prefix, (object) str), templateValues1);
          queryBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, ") AS {0}", (object) str);
        }
        else
          queryBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1} ", (object) prefix, (object) str);
      }
    }

    private void ValidateStateToGetValues(string method, EntityState invalidState)
    {
      this.ValidateNotDetachedAndInitializeRelatedEnd(method);
      if (this.State == invalidState)
        throw Error.DbPropertyValues_CannotGetValuesForState((object) method, (object) this.State);
    }

    public virtual void Reload()
    {
      this.ValidateStateToGetValues(nameof (Reload), EntityState.Added);
      this._internalContext.ObjectContext.Refresh(RefreshMode.StoreWins, this.Entity);
    }

    public virtual Task ReloadAsync(CancellationToken cancellationToken)
    {
      this.ValidateStateToGetValues(nameof (ReloadAsync), EntityState.Added);
      return this._internalContext.ObjectContext.RefreshAsync(RefreshMode.StoreWins, this.Entity, cancellationToken);
    }

    public virtual InternalReferenceEntry Reference(
      string navigationProperty,
      Type requestedType = null)
    {
      string navigationProperty1 = navigationProperty;
      Type requestedType1 = requestedType;
      if ((object) requestedType1 == null)
        requestedType1 = typeof (object);
      return (InternalReferenceEntry) this.ValidateAndGetNavigationMetadata(navigationProperty1, requestedType1, false).CreateMemberEntry(this, (InternalPropertyEntry) null);
    }

    public virtual InternalCollectionEntry Collection(
      string navigationProperty,
      Type requestedType = null)
    {
      string navigationProperty1 = navigationProperty;
      Type requestedType1 = requestedType;
      if ((object) requestedType1 == null)
        requestedType1 = typeof (object);
      return (InternalCollectionEntry) this.ValidateAndGetNavigationMetadata(navigationProperty1, requestedType1, true).CreateMemberEntry(this, (InternalPropertyEntry) null);
    }

    public virtual InternalMemberEntry Member(
      string propertyName,
      Type requestedType = null)
    {
      Type type = requestedType;
      if ((object) type == null)
        type = typeof (object);
      requestedType = type;
      IList<string> properties = InternalEntityEntry.SplitName(propertyName);
      if (properties.Count > 1)
        return (InternalMemberEntry) this.Property((InternalPropertyEntry) null, propertyName, properties, requestedType, false);
      MemberEntryMetadata memberEntryMetadata = (MemberEntryMetadata) this.GetNavigationMetadata(propertyName) ?? (MemberEntryMetadata) this.ValidateAndGetPropertyMetadata(propertyName, this.EntityType, requestedType);
      if (memberEntryMetadata == null)
        throw Error.DbEntityEntry_NotAProperty((object) propertyName, (object) this.EntityType.Name);
      if (memberEntryMetadata.MemberEntryType != MemberEntryType.CollectionNavigationProperty && !requestedType.IsAssignableFrom(memberEntryMetadata.MemberType))
        throw Error.DbEntityEntry_WrongGenericForNavProp((object) propertyName, (object) this.EntityType.Name, (object) requestedType.Name, (object) memberEntryMetadata.MemberType.Name);
      return memberEntryMetadata.CreateMemberEntry(this, (InternalPropertyEntry) null);
    }

    public virtual InternalPropertyEntry Property(
      string property,
      Type requestedType = null,
      bool requireComplex = false)
    {
      string propertyName = property;
      Type requestedType1 = requestedType;
      if ((object) requestedType1 == null)
        requestedType1 = typeof (object);
      int num = requireComplex ? 1 : 0;
      return this.Property((InternalPropertyEntry) null, propertyName, requestedType1, num != 0);
    }

    public InternalPropertyEntry Property(
      InternalPropertyEntry parentProperty,
      string propertyName,
      Type requestedType,
      bool requireComplex)
    {
      return this.Property(parentProperty, propertyName, InternalEntityEntry.SplitName(propertyName), requestedType, requireComplex);
    }

    private InternalPropertyEntry Property(
      InternalPropertyEntry parentProperty,
      string propertyName,
      IList<string> properties,
      Type requestedType,
      bool requireComplex)
    {
      bool flag = properties.Count > 1;
      Type requestedType1 = flag ? typeof (object) : requestedType;
      Type declaringType = parentProperty != null ? parentProperty.EntryMetadata.ElementType : this.EntityType;
      PropertyEntryMetadata propertyMetadata = this.ValidateAndGetPropertyMetadata(properties[0], declaringType, requestedType1);
      if (propertyMetadata == null || (flag || requireComplex) && !propertyMetadata.IsComplex)
      {
        if (flag)
          throw Error.DbEntityEntry_DottedPartNotComplex((object) properties[0], (object) propertyName, (object) declaringType.Name);
        throw requireComplex ? Error.DbEntityEntry_NotAComplexProperty((object) properties[0], (object) declaringType.Name) : Error.DbEntityEntry_NotAScalarProperty((object) properties[0], (object) declaringType.Name);
      }
      InternalPropertyEntry memberEntry = (InternalPropertyEntry) propertyMetadata.CreateMemberEntry(this, parentProperty);
      if (!flag)
        return memberEntry;
      return this.Property(memberEntry, propertyName, (IList<string>) properties.Skip<string>(1).ToList<string>(), requestedType, requireComplex);
    }

    private NavigationEntryMetadata ValidateAndGetNavigationMetadata(
      string navigationProperty,
      Type requestedType,
      bool requireCollection)
    {
      if (InternalEntityEntry.SplitName(navigationProperty).Count != 1)
        throw Error.DbEntityEntry_DottedPathMustBeProperty((object) navigationProperty);
      NavigationEntryMetadata navigationMetadata = this.GetNavigationMetadata(navigationProperty);
      if (navigationMetadata == null)
        throw Error.DbEntityEntry_NotANavigationProperty((object) navigationProperty, (object) this.EntityType.Name);
      if (requireCollection)
      {
        if (navigationMetadata.MemberEntryType == MemberEntryType.ReferenceNavigationProperty)
          throw Error.DbEntityEntry_UsedCollectionForReferenceProp((object) navigationProperty, (object) this.EntityType.Name);
      }
      else if (navigationMetadata.MemberEntryType == MemberEntryType.CollectionNavigationProperty)
        throw Error.DbEntityEntry_UsedReferenceForCollectionProp((object) navigationProperty, (object) this.EntityType.Name);
      if (!requestedType.IsAssignableFrom(navigationMetadata.ElementType))
        throw Error.DbEntityEntry_WrongGenericForNavProp((object) navigationProperty, (object) this.EntityType.Name, (object) requestedType.Name, (object) navigationMetadata.ElementType.Name);
      return navigationMetadata;
    }

    public virtual NavigationEntryMetadata GetNavigationMetadata(
      string propertyName)
    {
      EdmMember edmMember;
      this.EdmEntityType.Members.TryGetValue(propertyName, false, out edmMember);
      NavigationProperty navigationProperty = edmMember as NavigationProperty;
      if (navigationProperty != null)
        return new NavigationEntryMetadata(this.EntityType, this.GetNavigationTargetType(navigationProperty), propertyName, navigationProperty.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many);
      return (NavigationEntryMetadata) null;
    }

    private Type GetNavigationTargetType(NavigationProperty navigationProperty)
    {
      MetadataWorkspace metadataWorkspace = this._internalContext.ObjectContext.MetadataWorkspace;
      System.Data.Entity.Core.Metadata.Edm.EntityType entityType = navigationProperty.RelationshipType.RelationshipEndMembers.Single<RelationshipEndMember>((Func<RelationshipEndMember, bool>) (e => navigationProperty.ToEndMember.Name == e.Name)).GetEntityType();
      StructuralType objectSpaceType = metadataWorkspace.GetObjectSpaceType((StructuralType) entityType);
      return ((ObjectItemCollection) metadataWorkspace.GetItemCollection(DataSpace.OSpace)).GetClrType(objectSpaceType);
    }

    public virtual IRelatedEnd GetRelatedEnd(string navigationProperty)
    {
      EdmMember edmMember;
      this.EdmEntityType.Members.TryGetValue(navigationProperty, false, out edmMember);
      NavigationProperty navigationProperty1 = (NavigationProperty) edmMember;
      return this._internalContext.ObjectContext.ObjectStateManager.GetRelationshipManager(this.Entity).GetRelatedEnd(navigationProperty1.RelationshipType.FullName, navigationProperty1.ToEndMember.Name);
    }

    public virtual PropertyEntryMetadata ValidateAndGetPropertyMetadata(
      string propertyName,
      Type declaringType,
      Type requestedType)
    {
      return PropertyEntryMetadata.ValidateNameAndGetMetadata(this._internalContext, declaringType, requestedType, propertyName);
    }

    private static IList<string> SplitName(string propertyName)
    {
      return (IList<string>) propertyName.Split('.');
    }

    private void ValidateNotDetachedAndInitializeRelatedEnd(string method)
    {
      if (this.IsDetached)
        throw Error.DbEntityEntry_NotSupportedForDetached((object) method, (object) this._entityType.Name);
    }

    public virtual bool IsDetached
    {
      get
      {
        if (this._stateEntry == null || this._stateEntry.State == EntityState.Detached)
        {
          this._stateEntry = this._internalContext.GetStateEntry(this._entity);
          if (this._stateEntry == null)
            return true;
        }
        return false;
      }
    }

    public virtual Type EntityType
    {
      get
      {
        return this._entityType;
      }
    }

    public virtual System.Data.Entity.Core.Metadata.Edm.EntityType EdmEntityType
    {
      get
      {
        if (this._edmEntityType == null)
        {
          MetadataWorkspace metadataWorkspace = this._internalContext.ObjectContext.MetadataWorkspace;
          System.Data.Entity.Core.Metadata.Edm.EntityType entityType = metadataWorkspace.GetItem<System.Data.Entity.Core.Metadata.Edm.EntityType>(this._entityType.FullNameWithNesting(), DataSpace.OSpace);
          this._edmEntityType = (System.Data.Entity.Core.Metadata.Edm.EntityType) metadataWorkspace.GetEdmSpaceType((StructuralType) entityType);
        }
        return this._edmEntityType;
      }
    }

    public IEntityStateEntry ObjectStateEntry
    {
      get
      {
        return this._stateEntry;
      }
    }

    public InternalContext InternalContext
    {
      get
      {
        return this._internalContext;
      }
    }

    public virtual DbEntityValidationResult GetValidationResult(
      IDictionary<object, object> items)
    {
      EntityValidator entityValidator = this.InternalContext.ValidationProvider.GetEntityValidator(this);
      bool lazyLoadingEnabled = this.InternalContext.LazyLoadingEnabled;
      this.InternalContext.LazyLoadingEnabled = false;
      try
      {
        return entityValidator != null ? entityValidator.Validate(this.InternalContext.ValidationProvider.GetEntityValidationContext(this, items)) : new DbEntityValidationResult(this, Enumerable.Empty<DbValidationError>());
      }
      finally
      {
        this.InternalContext.LazyLoadingEnabled = lazyLoadingEnabled;
      }
    }

    public override bool Equals(object obj)
    {
      if (object.ReferenceEquals((object) null, obj) || obj.GetType() != typeof (InternalEntityEntry))
        return false;
      return this.Equals((InternalEntityEntry) obj);
    }

    public bool Equals(InternalEntityEntry other)
    {
      if (object.ReferenceEquals((object) this, (object) other))
        return true;
      if (!object.ReferenceEquals((object) null, (object) other) && object.ReferenceEquals(this._entity, other._entity))
        return object.ReferenceEquals((object) this._internalContext, (object) other._internalContext);
      return false;
    }

    public override int GetHashCode()
    {
      return RuntimeHelpers.GetHashCode(this._entity);
    }
  }
}
