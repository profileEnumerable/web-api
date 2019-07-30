// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.ObjectStateEntry
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Resources;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Objects
{
  /// <summary>
  /// Represents either a entity, entity stub or relationship
  /// </summary>
  public abstract class ObjectStateEntry : IEntityStateEntry, IEntityChangeTracker
  {
    internal ObjectStateManager _cache;
    internal EntitySetBase _entitySet;
    internal EntityState _state;

    internal ObjectStateEntry()
    {
    }

    internal ObjectStateEntry(ObjectStateManager cache, System.Data.Entity.Core.Metadata.Edm.EntitySet entitySet, EntityState state)
    {
      this._cache = cache;
      this._entitySet = (EntitySetBase) entitySet;
      this._state = state;
    }

    /// <summary>
    /// Gets the <see cref="T:System.Data.Entity.Core.Objects.ObjectStateManager" /> for the
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" />
    /// .
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.Objects.ObjectStateManager" /> for the
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" />
    /// .
    /// </returns>
    public ObjectStateManager ObjectStateManager
    {
      get
      {
        this.ValidateState();
        return this._cache;
      }
    }

    /// <summary>
    /// Gets the <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntitySetBase" /> for the object or relationship.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntitySetBase" /> for the object or relationship.
    /// </returns>
    public EntitySetBase EntitySet
    {
      get
      {
        this.ValidateState();
        return this._entitySet;
      }
    }

    /// <summary>
    /// Gets the state of the <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" />.
    /// </summary>
    /// <returns>
    /// The state of the <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" />.
    /// </returns>
    public EntityState State
    {
      get
      {
        return this._state;
      }
      internal set
      {
        this._state = value;
      }
    }

    /// <summary>Gets the entity object.</summary>
    /// <returns>The entity object.</returns>
    public abstract object Entity { get; }

    /// <summary>Gets the entity key.</summary>
    /// <returns>The entity key.</returns>
    public abstract EntityKey EntityKey { get; internal set; }

    /// <summary>
    /// Gets a value that indicates whether the <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" /> represents a relationship.
    /// </summary>
    /// <returns>
    /// true if the <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" /> represents a relationship; otherwise, false.
    /// </returns>
    public abstract bool IsRelationship { get; }

    internal abstract BitArray ModifiedProperties { get; }

    BitArray IEntityStateEntry.ModifiedProperties
    {
      get
      {
        return this.ModifiedProperties;
      }
    }

    /// <summary>Gets the read-only version of original values of the object or relationship.</summary>
    /// <returns>The read-only version of original values of the relationship set entry or entity.</returns>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public abstract DbDataRecord OriginalValues { get; }

    /// <summary>
    /// Gets the updatable version of original values of the object associated with this
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" />
    /// .
    /// </summary>
    /// <returns>The updatable original values of object data.</returns>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public abstract OriginalValueRecord GetUpdatableOriginalValues();

    /// <summary>
    /// Gets the current property values of the object or relationship associated with this
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" />
    /// .
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Objects.CurrentValueRecord" /> that contains the current values of the object or relationship associated with this
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" />
    /// .
    /// </returns>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public abstract CurrentValueRecord CurrentValues { get; }

    /// <summary>Accepts the current values as original values.</summary>
    public abstract void AcceptChanges();

    /// <summary>Marks an entity as deleted.</summary>
    public abstract void Delete();

    /// <summary>
    /// Returns the names of an object’s properties that have changed since the last time
    /// <see cref="M:System.Data.Entity.Core.Objects.ObjectContext.SaveChanges" />
    /// was called.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1" /> collection of names as string.
    /// </returns>
    public abstract IEnumerable<string> GetModifiedProperties();

    /// <summary>Sets the state of the object or relationship to modify.</summary>
    /// <exception cref="T:System.InvalidOperationException">If State is not Modified or Unchanged</exception>
    public abstract void SetModified();

    /// <summary>Marks the specified property as modified.</summary>
    /// <param name="propertyName">The name of the property.</param>
    /// <exception cref="T:System.InvalidOperationException"> If State is not Modified or Unchanged </exception>
    public abstract void SetModifiedProperty(string propertyName);

    /// <summary>Rejects any changes made to the property with the given name since the property was last loaded, attached, saved, or changes were accepted. The orginal value of the property is stored and the property will no longer be marked as modified.</summary>
    /// <param name="propertyName">The name of the property to change.</param>
    public abstract void RejectPropertyChanges(string propertyName);

    /// <summary>Uses DetectChanges to determine whether or not the current value of the property with the given name is different from its original value. Note that this may be different from the property being marked as modified since a property which has not changed can still be marked as modified.</summary>
    /// <remarks>
    /// Note that this property always returns the same result as the modified state of the property for change tracking
    /// proxies and entities that derive from the EntityObject base class. This is because original values are not tracked
    /// for these entity types and hence there is no way to know if the current value is really different from the
    /// original value.
    /// </remarks>
    /// <returns>true if the property has changed; otherwise, false.</returns>
    /// <param name="propertyName">The name of the property.</param>
    public abstract bool IsPropertyChanged(string propertyName);

    /// <summary>
    /// Gets the <see cref="T:System.Data.Entity.Core.Objects.DataClasses.RelationshipManager" /> instance for the object represented by entry.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.Objects.DataClasses.RelationshipManager" /> object.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">The entry is a stub or represents a relationship</exception>
    public abstract RelationshipManager RelationshipManager { get; }

    /// <summary>
    /// Changes state of the entry to the specified <see cref="T:System.Data.Entity.EntityState" /> value.
    /// </summary>
    /// <param name="state">
    /// The <see cref="T:System.Data.Entity.EntityState" /> value to set for the
    /// <see cref="P:System.Data.Entity.Core.Objects.ObjectStateEntry.State" />
    /// property of the entry.
    /// </param>
    public abstract void ChangeState(EntityState state);

    /// <summary>Sets the current values of the entry to match the property values of a supplied object.</summary>
    /// <param name="currentEntity">The detached object that has updated values to apply to the object.  currentEntity  can also be the object’s entity key.</param>
    public abstract void ApplyCurrentValues(object currentEntity);

    /// <summary>Sets the original values of the entry to match the property values of a supplied object.</summary>
    /// <param name="originalEntity">The detached object that has original values to apply to the object.  originalEntity  can also be the object’s entity key.</param>
    public abstract void ApplyOriginalValues(object originalEntity);

    IEntityStateManager IEntityStateEntry.StateManager
    {
      get
      {
        return (IEntityStateManager) this.ObjectStateManager;
      }
    }

    bool IEntityStateEntry.IsKeyEntry
    {
      get
      {
        return this.IsKeyEntry;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    void IEntityChangeTracker.EntityMemberChanging(string entityMemberName)
    {
      this.EntityMemberChanging(entityMemberName);
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    void IEntityChangeTracker.EntityMemberChanged(string entityMemberName)
    {
      this.EntityMemberChanged(entityMemberName);
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    void IEntityChangeTracker.EntityComplexMemberChanging(
      string entityMemberName,
      object complexObject,
      string complexObjectMemberName)
    {
      this.EntityComplexMemberChanging(entityMemberName, complexObject, complexObjectMemberName);
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    void IEntityChangeTracker.EntityComplexMemberChanged(
      string entityMemberName,
      object complexObject,
      string complexObjectMemberName)
    {
      this.EntityComplexMemberChanged(entityMemberName, complexObject, complexObjectMemberName);
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    EntityState IEntityChangeTracker.EntityState
    {
      get
      {
        return this.State;
      }
    }

    internal abstract bool IsKeyEntry { get; }

    internal abstract int GetFieldCount(StateManagerTypeMetadata metadata);

    internal abstract Type GetFieldType(int ordinal, StateManagerTypeMetadata metadata);

    internal abstract string GetCLayerName(int ordinal, StateManagerTypeMetadata metadata);

    internal abstract int GetOrdinalforCLayerName(string name, StateManagerTypeMetadata metadata);

    internal abstract void RevertDelete();

    internal abstract void SetModifiedAll();

    internal abstract void EntityMemberChanging(string entityMemberName);

    internal abstract void EntityMemberChanged(string entityMemberName);

    internal abstract void EntityComplexMemberChanging(
      string entityMemberName,
      object complexObject,
      string complexObjectMemberName);

    internal abstract void EntityComplexMemberChanged(
      string entityMemberName,
      object complexObject,
      string complexObjectMemberName);

    internal abstract DataRecordInfo GetDataRecordInfo(
      StateManagerTypeMetadata metadata,
      object userObject);

    internal virtual void Reset()
    {
      this._cache = (ObjectStateManager) null;
      this._entitySet = (EntitySetBase) null;
      this._state = EntityState.Detached;
    }

    internal void ValidateState()
    {
      if (this._state == EntityState.Detached)
        throw new InvalidOperationException(Strings.ObjectStateEntry_InvalidState);
    }
  }
}
