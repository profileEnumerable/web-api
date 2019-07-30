// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.InternalNestedPropertyEntry
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Internal
{
  internal class InternalNestedPropertyEntry : InternalPropertyEntry
  {
    private readonly InternalPropertyEntry _parentPropertyEntry;

    public InternalNestedPropertyEntry(
      InternalPropertyEntry parentPropertyEntry,
      PropertyEntryMetadata propertyMetadata)
      : base(parentPropertyEntry.InternalEntityEntry, propertyMetadata)
    {
      this._parentPropertyEntry = parentPropertyEntry;
    }

    public override InternalPropertyEntry ParentPropertyEntry
    {
      get
      {
        return this._parentPropertyEntry;
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
    public override InternalPropertyValues ParentCurrentValues
    {
      get
      {
        InternalPropertyValues parentCurrentValues = this._parentPropertyEntry.ParentCurrentValues;
        return parentCurrentValues == null ? (InternalPropertyValues) (object) null : (InternalPropertyValues) parentCurrentValues[this._parentPropertyEntry.Name];
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
    public override InternalPropertyValues ParentOriginalValues
    {
      get
      {
        InternalPropertyValues parentOriginalValues = this._parentPropertyEntry.ParentOriginalValues;
        return parentOriginalValues == null ? (InternalPropertyValues) (object) null : (InternalPropertyValues) parentOriginalValues[this._parentPropertyEntry.Name];
      }
    }

    protected override Func<object, object> CreateGetter()
    {
      Func<object, object> parentGetter = this._parentPropertyEntry.Getter;
      if (parentGetter == null)
        return (Func<object, object>) null;
      Func<object, object> getter;
      if (!DbHelpers.GetPropertyGetters(this.EntryMetadata.DeclaringType).TryGetValue(this.Name, out getter))
        return (Func<object, object>) null;
      return (Func<object, object>) (o =>
      {
        object obj = parentGetter(o);
        if (obj != null)
          return getter(obj);
        return (object) null;
      });
    }

    protected override Action<object, object> CreateSetter()
    {
      Func<object, object> parentGetter = this._parentPropertyEntry.Getter;
      if (parentGetter == null)
        return (Action<object, object>) null;
      Action<object, object> setter;
      if (!DbHelpers.GetPropertySetters(this.EntryMetadata.DeclaringType).TryGetValue(this.Name, out setter))
        return (Action<object, object>) null;
      return (Action<object, object>) ((o, v) =>
      {
        if (parentGetter(o) == null)
          throw Error.DbPropertyValues_CannotSetPropertyOnNullCurrentValue((object) this.Name, (object) this.ParentPropertyEntry.Name);
        setter(parentGetter(o), v);
      });
    }

    public override bool EntityPropertyIsModified()
    {
      return this._parentPropertyEntry.EntityPropertyIsModified();
    }

    public override void SetEntityPropertyModified()
    {
      this._parentPropertyEntry.SetEntityPropertyModified();
    }

    public override void RejectEntityPropertyChanges()
    {
      this.CurrentValue = this.OriginalValue;
      this.UpdateComplexPropertyState();
    }

    public override void UpdateComplexPropertyState()
    {
      this._parentPropertyEntry.UpdateComplexPropertyState();
    }
  }
}
