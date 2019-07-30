// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.DbLocalView`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Internal
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Name is intentional")]
  internal class DbLocalView<TEntity> : ObservableCollection<TEntity>, ICollection<TEntity>, IEnumerable<TEntity>, IList, ICollection, IEnumerable
    where TEntity : class
  {
    private readonly InternalContext _internalContext;
    private bool _inStateManagerChanged;
    private ObservableBackedBindingList<TEntity> _bindingList;

    public DbLocalView()
    {
    }

    public DbLocalView(IEnumerable<TEntity> collection)
    {
      Check.NotNull<IEnumerable<TEntity>>(collection, nameof (collection));
      collection.Each<TEntity>(new Action<TEntity>(((Collection<TEntity>) this).Add));
    }

    internal DbLocalView(InternalContext internalContext)
    {
      this._internalContext = internalContext;
      try
      {
        this._inStateManagerChanged = true;
        foreach (TEntity localEntity in this._internalContext.GetLocalEntities<TEntity>())
          this.Add(localEntity);
      }
      finally
      {
        this._inStateManagerChanged = false;
      }
      this._internalContext.RegisterObjectStateManagerChangedEvent(new CollectionChangeEventHandler(this.StateManagerChangedHandler));
    }

    internal ObservableBackedBindingList<TEntity> BindingList
    {
      get
      {
        return this._bindingList ?? (this._bindingList = new ObservableBackedBindingList<TEntity>((ObservableCollection<TEntity>) this));
      }
    }

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      if (!this._inStateManagerChanged && this._internalContext != null)
      {
        if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
        {
          foreach (TEntity oldItem in (IEnumerable) e.OldItems)
            this._internalContext.Set<TEntity>().Remove(oldItem);
        }
        if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace)
        {
          foreach (TEntity newItem in (IEnumerable) e.NewItems)
          {
            if (!this._internalContext.EntityInContextAndNotDeleted((object) newItem))
              this._internalContext.Set<TEntity>().Add(newItem);
          }
        }
      }
      base.OnCollectionChanged(e);
    }

    private void StateManagerChangedHandler(object sender, CollectionChangeEventArgs e)
    {
      try
      {
        this._inStateManagerChanged = true;
        TEntity element = e.Element as TEntity;
        if ((object) element == null)
          return;
        if (e.Action == CollectionChangeAction.Remove && this.Contains(element))
        {
          this.Remove(element);
        }
        else
        {
          if (e.Action != CollectionChangeAction.Add || this.Contains(element))
            return;
          this.Add(element);
        }
      }
      finally
      {
        this._inStateManagerChanged = false;
      }
    }

    protected override void ClearItems()
    {
      new List<TEntity>((IEnumerable<TEntity>) this).Each<TEntity, bool>((Func<TEntity, bool>) (t => this.Remove(t)));
    }

    protected override void InsertItem(int index, TEntity item)
    {
      if (this.Contains(item))
        return;
      base.InsertItem(index, item);
    }

    public new virtual bool Contains(TEntity item)
    {
      IEqualityComparer<TEntity> equalityComparer = (IEqualityComparer<TEntity>) ObjectReferenceEqualityComparer.Default;
      foreach (TEntity x in (IEnumerable<TEntity>) this.Items)
      {
        if (equalityComparer.Equals(x, item))
          return true;
      }
      return false;
    }

    public new virtual bool Remove(TEntity item)
    {
      IEqualityComparer<TEntity> equalityComparer = (IEqualityComparer<TEntity>) ObjectReferenceEqualityComparer.Default;
      int index = 0;
      while (index < this.Count && !equalityComparer.Equals(this.Items[index], item))
        ++index;
      if (index == this.Count)
        return false;
      this.RemoveItem(index);
      return true;
    }

    bool ICollection<TEntity>.Contains(TEntity item)
    {
      return this.Contains(item);
    }

    bool ICollection<TEntity>.Remove(TEntity item)
    {
      return this.Remove(item);
    }

    bool IList.Contains(object value)
    {
      if (DbLocalView<TEntity>.IsCompatibleObject(value))
        return this.Contains((TEntity) value);
      return false;
    }

    void IList.Remove(object value)
    {
      if (!DbLocalView<TEntity>.IsCompatibleObject(value))
        return;
      this.Remove((TEntity) value);
    }

    private static bool IsCompatibleObject(object value)
    {
      if (!(value is TEntity))
        return value == null;
      return true;
    }
  }
}
