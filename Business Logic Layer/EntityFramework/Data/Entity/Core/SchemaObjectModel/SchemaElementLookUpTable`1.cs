// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.SchemaElementLookUpTable`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal sealed class SchemaElementLookUpTable<T> : IEnumerable<T>, IEnumerable, ISchemaElementLookUpTable<T>
    where T : SchemaElement
  {
    private readonly List<string> _keysInDefOrder = new List<string>();
    private Dictionary<string, T> _keyToType;

    public int Count
    {
      get
      {
        return this.KeyToType.Count;
      }
    }

    public bool ContainsKey(string key)
    {
      return this.KeyToType.ContainsKey(SchemaElementLookUpTable<T>.KeyFromName(key));
    }

    public T LookUpEquivalentKey(string key)
    {
      key = SchemaElementLookUpTable<T>.KeyFromName(key);
      T obj;
      if (this.KeyToType.TryGetValue(key, out obj))
        return obj;
      return default (T);
    }

    public T this[string key]
    {
      get
      {
        return this.KeyToType[SchemaElementLookUpTable<T>.KeyFromName(key)];
      }
    }

    public T GetElementAt(int index)
    {
      return this.KeyToType[this._keysInDefOrder[index]];
    }

    public IEnumerator<T> GetEnumerator()
    {
      return (IEnumerator<T>) new SchemaElementLookUpTableEnumerator<T, T>(this.KeyToType, this._keysInDefOrder);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) new SchemaElementLookUpTableEnumerator<T, T>(this.KeyToType, this._keysInDefOrder);
    }

    public IEnumerator<S> GetFilteredEnumerator<S>() where S : T
    {
      return (IEnumerator<S>) new SchemaElementLookUpTableEnumerator<S, T>(this.KeyToType, this._keysInDefOrder);
    }

    public AddErrorKind TryAdd(T type)
    {
      if (string.IsNullOrEmpty(type.Identity))
        return AddErrorKind.MissingNameError;
      string key = SchemaElementLookUpTable<T>.KeyFromElement(type);
      T obj;
      if (this.KeyToType.TryGetValue(key, out obj))
        return AddErrorKind.DuplicateNameError;
      this.KeyToType.Add(key, type);
      this._keysInDefOrder.Add(key);
      return AddErrorKind.Succeeded;
    }

    public void Add(
      T type,
      bool doNotAddErrorForEmptyName,
      Func<object, string> duplicateKeyErrorFormat)
    {
      switch (this.TryAdd(type))
      {
        case AddErrorKind.MissingNameError:
          if (doNotAddErrorForEmptyName)
            break;
          type.AddError(ErrorCode.InvalidName, EdmSchemaErrorSeverity.Error, (object) Strings.MissingName);
          break;
        case AddErrorKind.DuplicateNameError:
          type.AddError(ErrorCode.AlreadyDefined, EdmSchemaErrorSeverity.Error, (object) duplicateKeyErrorFormat((object) type.FQName));
          break;
      }
    }

    private static string KeyFromElement(T type)
    {
      return SchemaElementLookUpTable<T>.KeyFromName(type.Identity);
    }

    private static string KeyFromName(string unnormalizedKey)
    {
      return unnormalizedKey;
    }

    private Dictionary<string, T> KeyToType
    {
      get
      {
        if (this._keyToType == null)
          this._keyToType = new Dictionary<string, T>((IEqualityComparer<string>) StringComparer.Ordinal);
        return this._keyToType;
      }
    }
  }
}
