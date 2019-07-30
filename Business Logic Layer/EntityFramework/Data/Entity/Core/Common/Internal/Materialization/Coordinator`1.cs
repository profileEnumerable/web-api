// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Internal.Materialization.Coordinator`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Objects.Internal;
using System.Data.Entity.Utilities;
using System.Linq;

namespace System.Data.Entity.Core.Common.Internal.Materialization
{
  internal class Coordinator<T> : Coordinator
  {
    internal readonly CoordinatorFactory<T> TypedCoordinatorFactory;
    private T _current;
    private ICollection<T> _elements;
    private List<IEntityWrapper> _wrappedElements;
    private Action<Shaper, List<IEntityWrapper>> _handleClose;
    private readonly bool IsUsingElementCollection;

    internal virtual T Current
    {
      get
      {
        return this._current;
      }
    }

    internal Coordinator(
      CoordinatorFactory<T> coordinatorFactory,
      Coordinator parent,
      Coordinator next)
      : base((CoordinatorFactory) coordinatorFactory, parent, next)
    {
      this.TypedCoordinatorFactory = coordinatorFactory;
      Coordinator next1 = (Coordinator) null;
      foreach (CoordinatorFactory coordinatorFactory1 in coordinatorFactory.NestedCoordinators.Reverse<CoordinatorFactory>())
      {
        this.Child = coordinatorFactory1.CreateCoordinator((Coordinator) this, next1);
        next1 = this.Child;
      }
      this.IsUsingElementCollection = !this.IsRoot && typeof (T) != typeof (RecordState);
    }

    internal override void ResetCollection(Shaper shaper)
    {
      if (this._handleClose != null)
      {
        this._handleClose(shaper, this._wrappedElements);
        this._handleClose = (Action<Shaper, List<IEntityWrapper>>) null;
      }
      this.IsEntered = false;
      if (this.IsUsingElementCollection)
      {
        this._elements = this.TypedCoordinatorFactory.InitializeCollection(shaper);
        this._wrappedElements = new List<IEntityWrapper>();
      }
      if (this.Child != null)
        this.Child.ResetCollection(shaper);
      if (this.Next == null)
        return;
      this.Next.ResetCollection(shaper);
    }

    internal override void ReadNextElement(Shaper shaper)
    {
      IEntityWrapper entityWrapper = (IEntityWrapper) null;
      T obj1;
      try
      {
        if (this.TypedCoordinatorFactory.WrappedElement == null)
        {
          obj1 = this.TypedCoordinatorFactory.Element(shaper);
        }
        else
        {
          entityWrapper = this.TypedCoordinatorFactory.WrappedElement(shaper);
          obj1 = (T) entityWrapper.Entity;
        }
      }
      catch (Exception ex)
      {
        if (ex.IsCatchableExceptionType() && !shaper.Reader.IsClosed)
        {
          this.ResetCollection(shaper);
          T obj2 = this.TypedCoordinatorFactory.ElementWithErrorHandling(shaper);
        }
        throw;
      }
      if (this.IsUsingElementCollection)
      {
        this._elements.Add(obj1);
        if (entityWrapper == null)
          return;
        this._wrappedElements.Add(entityWrapper);
      }
      else
        this._current = obj1;
    }

    internal void RegisterCloseHandler(Action<Shaper, List<IEntityWrapper>> closeHandler)
    {
      this._handleClose = closeHandler;
    }

    internal void SetCurrentToDefault()
    {
      this._current = default (T);
    }

    private IEnumerable<T> GetElements()
    {
      return (IEnumerable<T>) this._elements;
    }
  }
}
