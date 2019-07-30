// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.StateEntryAdapter
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;

namespace System.Data.Entity.Internal
{
  internal class StateEntryAdapter : IEntityStateEntry
  {
    private readonly ObjectStateEntry _stateEntry;

    public StateEntryAdapter(ObjectStateEntry stateEntry)
    {
      this._stateEntry = stateEntry;
    }

    public object Entity
    {
      get
      {
        return this._stateEntry.Entity;
      }
    }

    public EntityState State
    {
      get
      {
        return this._stateEntry.State;
      }
    }

    public void ChangeState(EntityState state)
    {
      this._stateEntry.ChangeState(state);
    }

    public DbUpdatableDataRecord CurrentValues
    {
      get
      {
        return (DbUpdatableDataRecord) this._stateEntry.CurrentValues;
      }
    }

    public DbUpdatableDataRecord GetUpdatableOriginalValues()
    {
      return (DbUpdatableDataRecord) this._stateEntry.GetUpdatableOriginalValues();
    }

    public EntitySetBase EntitySet
    {
      get
      {
        return this._stateEntry.EntitySet;
      }
    }

    public EntityKey EntityKey
    {
      get
      {
        return this._stateEntry.EntityKey;
      }
    }

    public IEnumerable<string> GetModifiedProperties()
    {
      return this._stateEntry.GetModifiedProperties();
    }

    public void SetModifiedProperty(string propertyName)
    {
      this._stateEntry.SetModifiedProperty(propertyName);
    }

    public void RejectPropertyChanges(string propertyName)
    {
      this._stateEntry.RejectPropertyChanges(propertyName);
    }

    public bool IsPropertyChanged(string propertyName)
    {
      return this._stateEntry.IsPropertyChanged(propertyName);
    }
  }
}
