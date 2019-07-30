// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.Internal.EntityWithKeyStrategy
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Objects.DataClasses;

namespace System.Data.Entity.Core.Objects.Internal
{
  internal sealed class EntityWithKeyStrategy : IEntityKeyStrategy
  {
    private readonly IEntityWithKey _entity;

    public EntityWithKeyStrategy(IEntityWithKey entity)
    {
      this._entity = entity;
    }

    public EntityKey GetEntityKey()
    {
      return this._entity.EntityKey;
    }

    public void SetEntityKey(EntityKey key)
    {
      this._entity.EntityKey = key;
    }

    public EntityKey GetEntityKeyFromEntity()
    {
      return this._entity.EntityKey;
    }
  }
}
