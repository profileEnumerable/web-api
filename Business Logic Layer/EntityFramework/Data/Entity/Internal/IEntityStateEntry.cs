// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.IEntityStateEntry
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;

namespace System.Data.Entity.Internal
{
  internal interface IEntityStateEntry
  {
    object Entity { get; }

    EntityState State { get; }

    void ChangeState(EntityState state);

    DbUpdatableDataRecord CurrentValues { get; }

    DbUpdatableDataRecord GetUpdatableOriginalValues();

    EntitySetBase EntitySet { get; }

    EntityKey EntityKey { get; }

    IEnumerable<string> GetModifiedProperties();

    void SetModifiedProperty(string propertyName);

    bool IsPropertyChanged(string propertyName);

    void RejectPropertyChanges(string propertyName);
  }
}
