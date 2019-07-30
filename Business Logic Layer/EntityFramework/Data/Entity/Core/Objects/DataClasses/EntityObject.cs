// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.DataClasses.EntityObject
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace System.Data.Entity.Core.Objects.DataClasses
{
  /// <summary>
  /// This is the class is the basis for all perscribed EntityObject classes.
  /// </summary>
  [DataContract(IsReference = true)]
  [Serializable]
  public abstract class EntityObject : StructuralObject, IEntityWithKey, IEntityWithChangeTracker, IEntityWithRelationships
  {
    [NonSerialized]
    private static readonly EntityObject.DetachedEntityChangeTracker _detachedEntityChangeTracker = new EntityObject.DetachedEntityChangeTracker();
    [NonSerialized]
    private IEntityChangeTracker _entityChangeTracker = (IEntityChangeTracker) EntityObject._detachedEntityChangeTracker;
    private RelationshipManager _relationships;
    private EntityKey _entityKey;

    private IEntityChangeTracker EntityChangeTracker
    {
      get
      {
        if (this._entityChangeTracker == null)
          this._entityChangeTracker = (IEntityChangeTracker) EntityObject._detachedEntityChangeTracker;
        return this._entityChangeTracker;
      }
      set
      {
        this._entityChangeTracker = value;
      }
    }

    /// <summary>Gets the entity state of the object.</summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.EntityState" /> of this object.
    /// </returns>
    [XmlIgnore]
    [Browsable(false)]
    public EntityState EntityState
    {
      get
      {
        return this.EntityChangeTracker.EntityState;
      }
    }

    /// <summary>Gets or sets the key for this object.</summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.EntityKey" /> for this object.
    /// </returns>
    [DataMember]
    [Browsable(false)]
    public EntityKey EntityKey
    {
      get
      {
        return this._entityKey;
      }
      set
      {
        this.EntityChangeTracker.EntityMemberChanging("-EntityKey-");
        this._entityKey = value;
        this.EntityChangeTracker.EntityMemberChanged("-EntityKey-");
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    void IEntityWithChangeTracker.SetChangeTracker(
      IEntityChangeTracker changeTracker)
    {
      if (changeTracker != null && this.EntityChangeTracker != EntityObject._detachedEntityChangeTracker && !object.ReferenceEquals((object) changeTracker, (object) this.EntityChangeTracker))
      {
        EntityEntry entityChangeTracker = this.EntityChangeTracker as EntityEntry;
        if (entityChangeTracker == null || !entityChangeTracker.ObjectStateManager.IsDisposed)
          throw new InvalidOperationException(Strings.Entity_EntityCantHaveMultipleChangeTrackers);
      }
      this.EntityChangeTracker = changeTracker;
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    RelationshipManager IEntityWithRelationships.RelationshipManager
    {
      get
      {
        if (this._relationships == null)
          this._relationships = RelationshipManager.Create((IEntityWithRelationships) this);
        return this._relationships;
      }
    }

    /// <summary>Notifies the change tracker that a property change is pending.</summary>
    /// <param name="property">The name of the changing property.</param>
    /// <exception cref="T:System.ArgumentNullException"> property  is null.</exception>
    protected override sealed void ReportPropertyChanging(string property)
    {
      Check.NotEmpty(property, nameof (property));
      base.ReportPropertyChanging(property);
      this.EntityChangeTracker.EntityMemberChanging(property);
    }

    /// <summary>Notifies the change tracker that a property has changed.</summary>
    /// <param name="property">The name of the changed property.</param>
    /// <exception cref="T:System.ArgumentNullException"> property  is null.</exception>
    protected override sealed void ReportPropertyChanged(string property)
    {
      Check.NotEmpty(property, nameof (property));
      this.EntityChangeTracker.EntityMemberChanged(property);
      base.ReportPropertyChanged(property);
    }

    internal override sealed bool IsChangeTracked
    {
      get
      {
        return this.EntityState != EntityState.Detached;
      }
    }

    internal override sealed void ReportComplexPropertyChanging(
      string entityMemberName,
      ComplexObject complexObject,
      string complexMemberName)
    {
      this.EntityChangeTracker.EntityComplexMemberChanging(entityMemberName, (object) complexObject, complexMemberName);
    }

    internal override sealed void ReportComplexPropertyChanged(
      string entityMemberName,
      ComplexObject complexObject,
      string complexMemberName)
    {
      this.EntityChangeTracker.EntityComplexMemberChanged(entityMemberName, (object) complexObject, complexMemberName);
    }

    private class DetachedEntityChangeTracker : IEntityChangeTracker
    {
      void IEntityChangeTracker.EntityMemberChanging(string entityMemberName)
      {
      }

      void IEntityChangeTracker.EntityMemberChanged(string entityMemberName)
      {
      }

      void IEntityChangeTracker.EntityComplexMemberChanging(
        string entityMemberName,
        object complexObject,
        string complexMemberName)
      {
      }

      void IEntityChangeTracker.EntityComplexMemberChanged(
        string entityMemberName,
        object complexObject,
        string complexMemberName)
      {
      }

      EntityState IEntityChangeTracker.EntityState
      {
        get
        {
          return EntityState.Detached;
        }
      }
    }
  }
}
