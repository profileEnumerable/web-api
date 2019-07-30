// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.SortableBindingList`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Utilities;
using System.Xml.Linq;

namespace System.Data.Entity.Internal
{
  internal class SortableBindingList<T> : BindingList<T>
  {
    private bool _isSorted;
    private ListSortDirection _sortDirection;
    private PropertyDescriptor _sortProperty;

    public SortableBindingList(List<T> list)
      : base((IList<T>) list)
    {
    }

    protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
    {
      if (!SortableBindingList<T>.PropertyComparer.CanSort(prop.PropertyType))
        return;
      ((List<T>) this.Items).Sort((IComparer<T>) new SortableBindingList<T>.PropertyComparer(prop, direction));
      this._sortDirection = direction;
      this._sortProperty = prop;
      this._isSorted = true;
      this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
    }

    protected override void RemoveSortCore()
    {
      this._isSorted = false;
      this._sortProperty = (PropertyDescriptor) null;
    }

    protected override bool IsSortedCore
    {
      get
      {
        return this._isSorted;
      }
    }

    protected override ListSortDirection SortDirectionCore
    {
      get
      {
        return this._sortDirection;
      }
    }

    protected override PropertyDescriptor SortPropertyCore
    {
      get
      {
        return this._sortProperty;
      }
    }

    protected override bool SupportsSortingCore
    {
      get
      {
        return true;
      }
    }

    internal class PropertyComparer : Comparer<T>
    {
      private readonly IComparer _comparer;
      private readonly ListSortDirection _direction;
      private readonly PropertyDescriptor _prop;
      private readonly bool _useToString;

      public PropertyComparer(PropertyDescriptor prop, ListSortDirection direction)
      {
        if (!prop.ComponentType.IsAssignableFrom(typeof (T)))
          throw new MissingMemberException(typeof (T).Name, prop.Name);
        this._prop = prop;
        this._direction = direction;
        if (SortableBindingList<T>.PropertyComparer.CanSortWithIComparable(prop.PropertyType))
        {
          this._comparer = (IComparer) typeof (Comparer<>).MakeGenericType(prop.PropertyType).GetDeclaredProperty("Default").GetValue((object) null, (object[]) null);
          this._useToString = false;
        }
        else
        {
          this._comparer = (IComparer) StringComparer.CurrentCultureIgnoreCase;
          this._useToString = true;
        }
      }

      public override int Compare(T left, T right)
      {
        object obj1 = this._prop.GetValue((object) left);
        object obj2 = this._prop.GetValue((object) right);
        if (this._useToString)
        {
          obj1 = (object) obj1?.ToString();
          obj2 = (object) obj2?.ToString();
        }
        if (this._direction != ListSortDirection.Ascending)
          return this._comparer.Compare(obj2, obj1);
        return this._comparer.Compare(obj1, obj2);
      }

      public static bool CanSort(Type type)
      {
        if (!SortableBindingList<T>.PropertyComparer.CanSortWithToString(type))
          return SortableBindingList<T>.PropertyComparer.CanSortWithIComparable(type);
        return true;
      }

      private static bool CanSortWithIComparable(Type type)
      {
        if (type.GetInterface("IComparable") != (Type) null)
          return true;
        if (type.IsGenericType())
          return type.GetGenericTypeDefinition() == typeof (Nullable<>);
        return false;
      }

      private static bool CanSortWithToString(Type type)
      {
        if (!type.Equals(typeof (XNode)))
          return type.IsSubclassOf(typeof (XNode));
        return true;
      }
    }
  }
}
