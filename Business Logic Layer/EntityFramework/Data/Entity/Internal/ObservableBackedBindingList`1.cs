// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.ObservableBackedBindingList`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace System.Data.Entity.Internal
{
  internal class ObservableBackedBindingList<T> : SortableBindingList<T>
  {
    private bool _addingNewInstance;
    private T _addNewInstance;
    private T _cancelNewInstance;
    private readonly ObservableCollection<T> _obervableCollection;
    private bool _inCollectionChanged;
    private bool _changingObservableCollection;

    public ObservableBackedBindingList(ObservableCollection<T> obervableCollection)
      : base(obervableCollection.ToList<T>())
    {
      this._obervableCollection = obervableCollection;
      this._obervableCollection.CollectionChanged += new NotifyCollectionChangedEventHandler(this.ObservableCollectionChanged);
    }

    protected override object AddNewCore()
    {
      this._addingNewInstance = true;
      this._addNewInstance = (T) base.AddNewCore();
      return (object) this._addNewInstance;
    }

    public override void CancelNew(int itemIndex)
    {
      if (itemIndex >= 0 && itemIndex < this.Count && object.Equals((object) this[itemIndex], (object) this._addNewInstance))
      {
        this._cancelNewInstance = this._addNewInstance;
        this._addNewInstance = default (T);
        this._addingNewInstance = false;
      }
      base.CancelNew(itemIndex);
    }

    protected override void ClearItems()
    {
      foreach (T obj in (IEnumerable<T>) this.Items)
        this.RemoveFromObservableCollection(obj);
      base.ClearItems();
    }

    public override void EndNew(int itemIndex)
    {
      if (itemIndex >= 0 && itemIndex < this.Count && object.Equals((object) this[itemIndex], (object) this._addNewInstance))
      {
        this.AddToObservableCollection(this._addNewInstance);
        this._addNewInstance = default (T);
        this._addingNewInstance = false;
      }
      base.EndNew(itemIndex);
    }

    protected override void InsertItem(int index, T item)
    {
      base.InsertItem(index, item);
      if (this._addingNewInstance || index < 0 || index > this.Count)
        return;
      this.AddToObservableCollection(item);
    }

    protected override void RemoveItem(int index)
    {
      if (index >= 0 && index < this.Count && object.Equals((object) this[index], (object) this._cancelNewInstance))
        this._cancelNewInstance = default (T);
      else
        this.RemoveFromObservableCollection(this[index]);
      base.RemoveItem(index);
    }

    protected override void SetItem(int index, T item)
    {
      T obj = this[index];
      base.SetItem(index, item);
      if (index < 0 || index >= this.Count)
        return;
      if (object.Equals((object) obj, (object) this._addNewInstance))
      {
        this._addNewInstance = default (T);
        this._addingNewInstance = false;
      }
      else
        this.RemoveFromObservableCollection(obj);
      this.AddToObservableCollection(item);
    }

    private void ObservableCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (this._changingObservableCollection)
        return;
      try
      {
        this._inCollectionChanged = true;
        if (e.Action == NotifyCollectionChangedAction.Reset)
          this.Clear();
        if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
        {
          foreach (T oldItem in (IEnumerable) e.OldItems)
            this.Remove(oldItem);
        }
        if (e.Action != NotifyCollectionChangedAction.Add && e.Action != NotifyCollectionChangedAction.Replace)
          return;
        foreach (T newItem in (IEnumerable) e.NewItems)
          this.Add(newItem);
      }
      finally
      {
        this._inCollectionChanged = false;
      }
    }

    private void AddToObservableCollection(T item)
    {
      if (this._inCollectionChanged)
        return;
      try
      {
        this._changingObservableCollection = true;
        this._obervableCollection.Add(item);
      }
      finally
      {
        this._changingObservableCollection = false;
      }
    }

    private void RemoveFromObservableCollection(T item)
    {
      if (this._inCollectionChanged)
        return;
      try
      {
        this._changingObservableCollection = true;
        this._obervableCollection.Remove(item);
      }
      finally
      {
        this._changingObservableCollection = false;
      }
    }
  }
}
