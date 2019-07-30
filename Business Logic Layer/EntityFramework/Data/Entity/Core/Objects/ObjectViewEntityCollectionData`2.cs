// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.ObjectViewEntityCollectionData`2
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Core.Objects.DataClasses;

namespace System.Data.Entity.Core.Objects
{
  internal sealed class ObjectViewEntityCollectionData<TViewElement, TItemElement> : IObjectViewData<TViewElement>
    where TViewElement : TItemElement
    where TItemElement : class
  {
    private readonly System.Collections.Generic.List<TViewElement> _bindingList;
    private readonly EntityCollection<TItemElement> _entityCollection;
    private readonly bool _canEditItems;
    private bool _itemCommitPending;

    internal ObjectViewEntityCollectionData(EntityCollection<TItemElement> entityCollection)
    {
      this._entityCollection = entityCollection;
      this._canEditItems = true;
      this._bindingList = new System.Collections.Generic.List<TViewElement>(entityCollection.Count);
      foreach (TItemElement entity in entityCollection)
        this._bindingList.Add((TViewElement) (object) entity);
    }

    public IList<TViewElement> List
    {
      get
      {
        return (IList<TViewElement>) this._bindingList;
      }
    }

    public bool AllowNew
    {
      get
      {
        return !this._entityCollection.IsReadOnly;
      }
    }

    public bool AllowEdit
    {
      get
      {
        return this._canEditItems;
      }
    }

    public bool AllowRemove
    {
      get
      {
        return !this._entityCollection.IsReadOnly;
      }
    }

    public bool FiresEventOnAdd
    {
      get
      {
        return true;
      }
    }

    public bool FiresEventOnRemove
    {
      get
      {
        return true;
      }
    }

    public bool FiresEventOnClear
    {
      get
      {
        return true;
      }
    }

    public void EnsureCanAddNew()
    {
    }

    public int Add(TViewElement item, bool isAddNew)
    {
      if (isAddNew)
        this._bindingList.Add(item);
      else
        this._entityCollection.Add((TItemElement) item);
      return this._bindingList.Count - 1;
    }

    public void CommitItemAt(int index)
    {
      TViewElement binding = this._bindingList[index];
      try
      {
        this._itemCommitPending = true;
        this._entityCollection.Add((TItemElement) binding);
      }
      finally
      {
        this._itemCommitPending = false;
      }
    }

    public void Clear()
    {
      if (0 >= this._bindingList.Count)
        return;
      System.Collections.Generic.List<object> list = new System.Collections.Generic.List<object>();
      foreach (TViewElement binding in this._bindingList)
      {
        object obj = (object) binding;
        list.Add(obj);
      }
      this._entityCollection.BulkDeleteAll(list);
    }

    public bool Remove(TViewElement item, bool isCancelNew)
    {
      return !isCancelNew ? this._entityCollection.RemoveInternal((TItemElement) item) : this._bindingList.Remove(item);
    }

    public ListChangedEventArgs OnCollectionChanged(
      object sender,
      CollectionChangeEventArgs e,
      ObjectViewListener listener)
    {
      ListChangedEventArgs changedEventArgs = (ListChangedEventArgs) null;
      switch (e.Action)
      {
        case CollectionChangeAction.Add:
          if (e.Element is TViewElement && !this._itemCommitPending)
          {
            TViewElement element = (TViewElement) e.Element;
            this._bindingList.Add(element);
            listener.RegisterEntityEvents((object) element);
            changedEventArgs = new ListChangedEventArgs(ListChangedType.ItemAdded, this._bindingList.Count - 1, -1);
            break;
          }
          break;
        case CollectionChangeAction.Remove:
          if (e.Element is TViewElement)
          {
            TViewElement element = (TViewElement) e.Element;
            int newIndex = this._bindingList.IndexOf(element);
            if (newIndex != -1)
            {
              this._bindingList.Remove(element);
              listener.UnregisterEntityEvents((object) element);
              changedEventArgs = new ListChangedEventArgs(ListChangedType.ItemDeleted, newIndex, -1);
              break;
            }
            break;
          }
          break;
        case CollectionChangeAction.Refresh:
          foreach (TViewElement binding in this._bindingList)
            listener.UnregisterEntityEvents((object) binding);
          this._bindingList.Clear();
          foreach (TViewElement viewElement in this._entityCollection.GetInternalEnumerable())
          {
            this._bindingList.Add(viewElement);
            listener.RegisterEntityEvents((object) viewElement);
          }
          changedEventArgs = new ListChangedEventArgs(ListChangedType.Reset, -1, -1);
          break;
      }
      return changedEventArgs;
    }
  }
}
