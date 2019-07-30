// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ObservableCollectionExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity.Internal;
using System.Data.Entity.Utilities;

namespace System.Data.Entity
{
  /// <summary>
  /// Extension methods for <see cref="T:System.Collections.ObjectModel.ObservableCollection`1" />.
  /// </summary>
  public static class ObservableCollectionExtensions
  {
    /// <summary>
    /// Returns an <see cref="T:System.ComponentModel.BindingList`1" /> implementation that stays in sync with the given
    /// <see cref="T:System.Collections.ObjectModel.ObservableCollection`1" />.
    /// </summary>
    /// <typeparam name="T"> The element type. </typeparam>
    /// <param name="source"> The collection that the binding list will stay in sync with. </param>
    /// <returns> The binding list. </returns>
    public static BindingList<T> ToBindingList<T>(this ObservableCollection<T> source) where T : class
    {
      Check.NotNull<ObservableCollection<T>>(source, nameof (source));
      DbLocalView<T> dbLocalView = source as DbLocalView<T>;
      if (dbLocalView == null)
        return (BindingList<T>) new ObservableBackedBindingList<T>(source);
      return (BindingList<T>) dbLocalView.BindingList;
    }
  }
}
