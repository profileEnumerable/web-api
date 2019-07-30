// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.IObjectViewData`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.ComponentModel;

namespace System.Data.Entity.Core.Objects
{
  internal interface IObjectViewData<T>
  {
    IList<T> List { get; }

    bool AllowNew { get; }

    bool AllowEdit { get; }

    bool AllowRemove { get; }

    bool FiresEventOnAdd { get; }

    bool FiresEventOnRemove { get; }

    bool FiresEventOnClear { get; }

    void EnsureCanAddNew();

    int Add(T item, bool isAddNew);

    void CommitItemAt(int index);

    void Clear();

    bool Remove(T item, bool isCancelNew);

    ListChangedEventArgs OnCollectionChanged(
      object sender,
      CollectionChangeEventArgs e,
      ObjectViewListener listener);
  }
}
