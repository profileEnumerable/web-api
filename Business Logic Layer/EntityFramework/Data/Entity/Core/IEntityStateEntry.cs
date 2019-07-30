// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.IEntityStateEntry
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;

namespace System.Data.Entity.Core
{
  internal interface IEntityStateEntry
  {
    IEntityStateManager StateManager { get; }

    EntityKey EntityKey { get; }

    EntitySetBase EntitySet { get; }

    bool IsRelationship { get; }

    bool IsKeyEntry { get; }

    EntityState State { get; }

    DbDataRecord OriginalValues { get; }

    CurrentValueRecord CurrentValues { get; }

    BitArray ModifiedProperties { get; }

    void AcceptChanges();

    void Delete();

    void SetModified();

    void SetModifiedProperty(string propertyName);

    IEnumerable<string> GetModifiedProperties();
  }
}
