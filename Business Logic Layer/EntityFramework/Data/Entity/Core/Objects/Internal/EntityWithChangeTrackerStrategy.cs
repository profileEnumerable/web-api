// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.Internal.EntityWithChangeTrackerStrategy
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Objects.DataClasses;

namespace System.Data.Entity.Core.Objects.Internal
{
  internal sealed class EntityWithChangeTrackerStrategy : IChangeTrackingStrategy
  {
    private readonly IEntityWithChangeTracker _entity;

    public EntityWithChangeTrackerStrategy(IEntityWithChangeTracker entity)
    {
      this._entity = entity;
    }

    public void SetChangeTracker(IEntityChangeTracker changeTracker)
    {
      this._entity.SetChangeTracker(changeTracker);
    }

    public void TakeSnapshot(EntityEntry entry)
    {
      if (entry == null || !entry.RequiresComplexChangeTracking)
        return;
      entry.TakeSnapshot(true);
    }

    public void SetCurrentValue(
      EntityEntry entry,
      StateManagerMemberMetadata member,
      int ordinal,
      object target,
      object value)
    {
      member.SetValue(target, value);
    }

    public void UpdateCurrentValueRecord(object value, EntityEntry entry)
    {
      bool flag = entry.WrappedEntity.IdentityType != this._entity.GetType();
      entry.UpdateRecordWithoutSetModified(value, (DbUpdatableDataRecord) entry.CurrentValues);
      if (!flag)
        return;
      entry.DetectChangesInProperties(true);
    }
  }
}
