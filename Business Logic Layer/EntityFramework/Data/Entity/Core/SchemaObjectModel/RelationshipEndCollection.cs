// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.RelationshipEndCollection
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal sealed class RelationshipEndCollection : IList<IRelationshipEnd>, ICollection<IRelationshipEnd>, IEnumerable<IRelationshipEnd>, IEnumerable
  {
    private Dictionary<string, IRelationshipEnd> _endLookup;
    private List<string> _keysInDefOrder;

    public int Count
    {
      get
      {
        return this.KeysInDefOrder.Count;
      }
    }

    public void Add(IRelationshipEnd end)
    {
      SchemaElement end1 = end as SchemaElement;
      if (!RelationshipEndCollection.IsEndValid(end) || !this.ValidateUniqueName(end1, end.Name))
        return;
      this.EndLookup.Add(end.Name, end);
      this.KeysInDefOrder.Add(end.Name);
    }

    private static bool IsEndValid(IRelationshipEnd end)
    {
      return !string.IsNullOrEmpty(end.Name);
    }

    private bool ValidateUniqueName(SchemaElement end, string name)
    {
      if (!this.EndLookup.ContainsKey(name))
        return true;
      end.AddError(ErrorCode.AlreadyDefined, EdmSchemaErrorSeverity.Error, (object) Strings.EndNameAlreadyDefinedDuplicate((object) name));
      return false;
    }

    public bool Remove(IRelationshipEnd end)
    {
      if (!RelationshipEndCollection.IsEndValid(end))
        return false;
      this.KeysInDefOrder.Remove(end.Name);
      return this.EndLookup.Remove(end.Name);
    }

    public bool Contains(string name)
    {
      return this.EndLookup.ContainsKey(name);
    }

    public bool Contains(IRelationshipEnd end)
    {
      return this.Contains(end.Name);
    }

    public IRelationshipEnd this[int index]
    {
      get
      {
        return this.EndLookup[this.KeysInDefOrder[index]];
      }
      set
      {
        throw new NotSupportedException();
      }
    }

    public IEnumerator<IRelationshipEnd> GetEnumerator()
    {
      return (IEnumerator<IRelationshipEnd>) new RelationshipEndCollection.Enumerator(this.EndLookup, this.KeysInDefOrder);
    }

    public bool TryGetEnd(string name, out IRelationshipEnd end)
    {
      return this.EndLookup.TryGetValue(name, out end);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) new RelationshipEndCollection.Enumerator(this.EndLookup, this.KeysInDefOrder);
    }

    private Dictionary<string, IRelationshipEnd> EndLookup
    {
      get
      {
        if (this._endLookup == null)
          this._endLookup = new Dictionary<string, IRelationshipEnd>((IEqualityComparer<string>) StringComparer.Ordinal);
        return this._endLookup;
      }
    }

    private List<string> KeysInDefOrder
    {
      get
      {
        if (this._keysInDefOrder == null)
          this._keysInDefOrder = new List<string>();
        return this._keysInDefOrder;
      }
    }

    public void Clear()
    {
      this.EndLookup.Clear();
      this.KeysInDefOrder.Clear();
    }

    public bool IsReadOnly
    {
      get
      {
        return false;
      }
    }

    int IList<IRelationshipEnd>.IndexOf(IRelationshipEnd end)
    {
      throw new NotSupportedException();
    }

    void IList<IRelationshipEnd>.Insert(int index, IRelationshipEnd end)
    {
      throw new NotSupportedException();
    }

    void IList<IRelationshipEnd>.RemoveAt(int index)
    {
      throw new NotSupportedException();
    }

    public void CopyTo(IRelationshipEnd[] ends, int index)
    {
      foreach (IRelationshipEnd relationshipEnd in this)
        ends[index++] = relationshipEnd;
    }

    private sealed class Enumerator : IEnumerator<IRelationshipEnd>, IDisposable, IEnumerator
    {
      private List<string>.Enumerator _Enumerator;
      private readonly Dictionary<string, IRelationshipEnd> _Data;

      public Enumerator(Dictionary<string, IRelationshipEnd> data, List<string> keysInDefOrder)
      {
        this._Enumerator = keysInDefOrder.GetEnumerator();
        this._Data = data;
      }

      public void Reset()
      {
        ((IEnumerator) this._Enumerator).Reset();
      }

      public IRelationshipEnd Current
      {
        get
        {
          return this._Data[this._Enumerator.Current];
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return (object) this._Data[this._Enumerator.Current];
        }
      }

      public bool MoveNext()
      {
        return this._Enumerator.MoveNext();
      }

      public void Dispose()
      {
      }
    }
  }
}
