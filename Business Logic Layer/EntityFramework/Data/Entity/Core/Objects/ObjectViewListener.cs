// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.ObjectViewListener
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.ComponentModel;
using System.Data.Entity.Core.Objects.DataClasses;

namespace System.Data.Entity.Core.Objects
{
  internal sealed class ObjectViewListener
  {
    private readonly WeakReference _viewWeak;
    private readonly object _dataSource;
    private readonly IList _list;

    internal ObjectViewListener(IObjectView view, IList list, object dataSource)
    {
      this._viewWeak = new WeakReference((object) view);
      this._dataSource = dataSource;
      this._list = list;
      this.RegisterCollectionEvents();
      this.RegisterEntityEvents();
    }

    private void CleanUpListener()
    {
      this.UnregisterCollectionEvents();
      this.UnregisterEntityEvents();
    }

    private void RegisterCollectionEvents()
    {
      ObjectStateManager dataSource = this._dataSource as ObjectStateManager;
      if (dataSource != null)
      {
        dataSource.EntityDeleted += new CollectionChangeEventHandler(this.CollectionChanged);
      }
      else
      {
        if (this._dataSource == null)
          return;
        ((RelatedEnd) this._dataSource).AssociationChangedForObjectView += new CollectionChangeEventHandler(this.CollectionChanged);
      }
    }

    private void UnregisterCollectionEvents()
    {
      ObjectStateManager dataSource = this._dataSource as ObjectStateManager;
      if (dataSource != null)
      {
        dataSource.EntityDeleted -= new CollectionChangeEventHandler(this.CollectionChanged);
      }
      else
      {
        if (this._dataSource == null)
          return;
        ((RelatedEnd) this._dataSource).AssociationChangedForObjectView -= new CollectionChangeEventHandler(this.CollectionChanged);
      }
    }

    internal void RegisterEntityEvents(object entity)
    {
      INotifyPropertyChanged notifyPropertyChanged = entity as INotifyPropertyChanged;
      if (notifyPropertyChanged == null)
        return;
      notifyPropertyChanged.PropertyChanged += new PropertyChangedEventHandler(this.EntityPropertyChanged);
    }

    private void RegisterEntityEvents()
    {
      if (this._list == null)
        return;
      foreach (object obj in (IEnumerable) this._list)
      {
        INotifyPropertyChanged notifyPropertyChanged = obj as INotifyPropertyChanged;
        if (notifyPropertyChanged != null)
          notifyPropertyChanged.PropertyChanged += new PropertyChangedEventHandler(this.EntityPropertyChanged);
      }
    }

    internal void UnregisterEntityEvents(object entity)
    {
      INotifyPropertyChanged notifyPropertyChanged = entity as INotifyPropertyChanged;
      if (notifyPropertyChanged == null)
        return;
      notifyPropertyChanged.PropertyChanged -= new PropertyChangedEventHandler(this.EntityPropertyChanged);
    }

    private void UnregisterEntityEvents()
    {
      if (this._list == null)
        return;
      foreach (object obj in (IEnumerable) this._list)
      {
        INotifyPropertyChanged notifyPropertyChanged = obj as INotifyPropertyChanged;
        if (notifyPropertyChanged != null)
          notifyPropertyChanged.PropertyChanged -= new PropertyChangedEventHandler(this.EntityPropertyChanged);
      }
    }

    private void EntityPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      IObjectView target = (IObjectView) this._viewWeak.Target;
      if (target != null)
        target.EntityPropertyChanged(sender, e);
      else
        this.CleanUpListener();
    }

    private void CollectionChanged(object sender, CollectionChangeEventArgs e)
    {
      IObjectView target = (IObjectView) this._viewWeak.Target;
      if (target != null)
        target.CollectionChanged(sender, e);
      else
        this.CleanUpListener();
    }
  }
}
