// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.NavigationPropertyAccessor
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Threading;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal class NavigationPropertyAccessor
  {
    private Func<object, object> _memberGetter;
    private Action<object, object> _memberSetter;
    private Action<object, object> _collectionAdd;
    private Func<object, object, bool> _collectionRemove;
    private Func<object> _collectionCreate;
    private readonly string _propertyName;

    public NavigationPropertyAccessor(string propertyName)
    {
      this._propertyName = propertyName;
    }

    public bool HasProperty
    {
      get
      {
        return this._propertyName != null;
      }
    }

    public string PropertyName
    {
      get
      {
        return this._propertyName;
      }
    }

    public Func<object, object> ValueGetter
    {
      get
      {
        return this._memberGetter;
      }
      set
      {
        Interlocked.CompareExchange<Func<object, object>>(ref this._memberGetter, value, (Func<object, object>) null);
      }
    }

    public Action<object, object> ValueSetter
    {
      get
      {
        return this._memberSetter;
      }
      set
      {
        Interlocked.CompareExchange<Action<object, object>>(ref this._memberSetter, value, (Action<object, object>) null);
      }
    }

    public Action<object, object> CollectionAdd
    {
      get
      {
        return this._collectionAdd;
      }
      set
      {
        Interlocked.CompareExchange<Action<object, object>>(ref this._collectionAdd, value, (Action<object, object>) null);
      }
    }

    public Func<object, object, bool> CollectionRemove
    {
      get
      {
        return this._collectionRemove;
      }
      set
      {
        Interlocked.CompareExchange<Func<object, object, bool>>(ref this._collectionRemove, value, (Func<object, object, bool>) null);
      }
    }

    public Func<object> CollectionCreate
    {
      get
      {
        return this._collectionCreate;
      }
      set
      {
        Interlocked.CompareExchange<Func<object>>(ref this._collectionCreate, value, (Func<object>) null);
      }
    }

    public static NavigationPropertyAccessor NoNavigationProperty
    {
      get
      {
        return new NavigationPropertyAccessor((string) null);
      }
    }
  }
}
