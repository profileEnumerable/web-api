// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.FilteredSchemaElementLookUpTable`2
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Resources;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal sealed class FilteredSchemaElementLookUpTable<T, S> : IEnumerable<T>, IEnumerable, ISchemaElementLookUpTable<T>
    where T : S
    where S : SchemaElement
  {
    private readonly SchemaElementLookUpTable<S> _lookUpTable;

    public FilteredSchemaElementLookUpTable(SchemaElementLookUpTable<S> lookUpTable)
    {
      this._lookUpTable = lookUpTable;
    }

    public IEnumerator<T> GetEnumerator()
    {
      return this._lookUpTable.GetFilteredEnumerator<T>();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this._lookUpTable.GetFilteredEnumerator<T>();
    }

    public int Count
    {
      get
      {
        int num = 0;
        foreach (S s in this._lookUpTable)
        {
          if ((SchemaElement) s is T)
            ++num;
        }
        return num;
      }
    }

    public bool ContainsKey(string key)
    {
      if (!this._lookUpTable.ContainsKey(key))
        return false;
      return (object) ((object) this._lookUpTable[key] as T) != null;
    }

    public T this[string key]
    {
      get
      {
        S s = this._lookUpTable[key];
        if ((object) s == null)
          return default (T);
        T obj = (object) s as T;
        if ((object) obj != null)
          return obj;
        throw new InvalidOperationException(Strings.UnexpectedTypeInCollection((object) s.GetType(), (object) key));
      }
    }

    public T LookUpEquivalentKey(string key)
    {
      return (object) this._lookUpTable.LookUpEquivalentKey(key) as T;
    }
  }
}
