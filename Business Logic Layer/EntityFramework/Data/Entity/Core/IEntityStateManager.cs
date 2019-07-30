// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.IEntityStateManager
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.Core
{
  internal interface IEntityStateManager
  {
    IEnumerable<IEntityStateEntry> GetEntityStateEntries(
      EntityState state);

    IEnumerable<IEntityStateEntry> FindRelationshipsByKey(EntityKey key);

    IEntityStateEntry GetEntityStateEntry(EntityKey key);

    bool TryGetEntityStateEntry(EntityKey key, out IEntityStateEntry stateEntry);

    bool TryGetReferenceKey(
      EntityKey dependentKey,
      AssociationEndMember principalRole,
      out EntityKey principalKey);
  }
}
