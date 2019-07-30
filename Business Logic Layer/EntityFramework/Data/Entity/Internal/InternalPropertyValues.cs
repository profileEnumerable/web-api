// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.InternalPropertyValues
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Linq.Expressions;

namespace System.Data.Entity.Internal
{
  internal abstract class InternalPropertyValues
  {
    private static readonly ConcurrentDictionary<Type, Func<object>> _nonEntityFactories = new ConcurrentDictionary<Type, Func<object>>();
    private readonly InternalContext _internalContext;
    private readonly Type _type;
    private readonly bool _isEntityValues;

    protected InternalPropertyValues(
      InternalContext internalContext,
      Type type,
      bool isEntityValues)
    {
      this._internalContext = internalContext;
      this._type = type;
      this._isEntityValues = isEntityValues;
    }

    protected abstract IPropertyValuesItem GetItemImpl(string propertyName);

    public abstract ISet<string> PropertyNames { get; }

    public object ToObject()
    {
      object obj1 = this.CreateObject();
      IDictionary<string, Action<object, object>> propertySetters = DbHelpers.GetPropertySetters(this._type);
      foreach (string propertyName in (IEnumerable<string>) this.PropertyNames)
      {
        object obj2 = this.GetItem(propertyName).Value;
        InternalPropertyValues internalPropertyValues = obj2 as InternalPropertyValues;
        if (internalPropertyValues != null)
          obj2 = internalPropertyValues.ToObject();
        Action<object, object> action;
        if (propertySetters.TryGetValue(propertyName, out action))
          action(obj1, obj2);
      }
      return obj1;
    }

    private object CreateObject()
    {
      if (this._isEntityValues)
        return this._internalContext.CreateObject(this._type);
      Func<object> func;
      if (!InternalPropertyValues._nonEntityFactories.TryGetValue(this._type, out func))
      {
        func = ((Expression<Func<object>>) (() => Expression.New(this._type.GetDeclaredConstructor()))).Compile();
        InternalPropertyValues._nonEntityFactories.TryAdd(this._type, func);
      }
      return func();
    }

    public void SetValues(object value)
    {
      IDictionary<string, Func<object, object>> propertyGetters = DbHelpers.GetPropertyGetters(value.GetType());
      foreach (string propertyName in (IEnumerable<string>) this.PropertyNames)
      {
        Func<object, object> func;
        if (propertyGetters.TryGetValue(propertyName, out func))
        {
          object newValue = func(value);
          IPropertyValuesItem propertyValuesItem = this.GetItem(propertyName);
          if (newValue == null && propertyValuesItem.IsComplex)
            throw Error.DbPropertyValues_ComplexObjectCannotBeNull((object) propertyName, (object) this._type.Name);
          InternalPropertyValues internalPropertyValues = propertyValuesItem.Value as InternalPropertyValues;
          if (internalPropertyValues == null)
            this.SetValue(propertyValuesItem, newValue);
          else
            internalPropertyValues.SetValues(newValue);
        }
      }
    }

    public InternalPropertyValues Clone()
    {
      return (InternalPropertyValues) new ClonedPropertyValues(this, (DbDataRecord) null);
    }

    public void SetValues(InternalPropertyValues values)
    {
      if (!this._type.IsAssignableFrom(values.ObjectType))
        throw Error.DbPropertyValues_AttemptToSetValuesFromWrongType((object) values.ObjectType.Name, (object) this._type.Name);
      foreach (string propertyName in (IEnumerable<string>) this.PropertyNames)
      {
        IPropertyValuesItem propertyValuesItem = values.GetItem(propertyName);
        if (propertyValuesItem.Value == null && propertyValuesItem.IsComplex)
          throw Error.DbPropertyValues_NestedPropertyValuesNull((object) propertyName, (object) this._type.Name);
        this[propertyName] = propertyValuesItem.Value;
      }
    }

    public object this[string propertyName]
    {
      get
      {
        return this.GetItem(propertyName).Value;
      }
      set
      {
        DbPropertyValues dbPropertyValues = value as DbPropertyValues;
        if (dbPropertyValues != null)
          value = (object) dbPropertyValues.InternalPropertyValues;
        IPropertyValuesItem propertyValuesItem = this.GetItem(propertyName);
        InternalPropertyValues internalPropertyValues = propertyValuesItem.Value as InternalPropertyValues;
        if (internalPropertyValues == null)
        {
          this.SetValue(propertyValuesItem, value);
        }
        else
        {
          InternalPropertyValues values = value as InternalPropertyValues;
          if (values == null)
            throw Error.DbPropertyValues_AttemptToSetNonValuesOnComplexProperty();
          internalPropertyValues.SetValues(values);
        }
      }
    }

    public IPropertyValuesItem GetItem(string propertyName)
    {
      if (!this.PropertyNames.Contains(propertyName))
        throw Error.DbPropertyValues_PropertyDoesNotExist((object) propertyName, (object) this._type.Name);
      return this.GetItemImpl(propertyName);
    }

    private void SetValue(IPropertyValuesItem item, object newValue)
    {
      if (DbHelpers.PropertyValuesEqual(item.Value, newValue))
        return;
      if (item.Value == null && item.IsComplex)
        throw Error.DbPropertyValues_NestedPropertyValuesNull((object) item.Name, (object) this._type.Name);
      if (newValue != null && !item.Type.IsAssignableFrom(newValue.GetType()))
        throw Error.DbPropertyValues_WrongTypeForAssignment((object) newValue.GetType().Name, (object) item.Name, (object) item.Type.Name, (object) this._type.Name);
      item.Value = newValue;
    }

    public Type ObjectType
    {
      get
      {
        return this._type;
      }
    }

    public InternalContext InternalContext
    {
      get
      {
        return this._internalContext;
      }
    }

    public bool IsEntityValues
    {
      get
      {
        return this._isEntityValues;
      }
    }
  }
}
