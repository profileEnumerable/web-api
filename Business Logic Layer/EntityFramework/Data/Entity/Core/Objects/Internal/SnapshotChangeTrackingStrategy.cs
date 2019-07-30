// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.Internal.SnapshotChangeTrackingStrategy
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Objects.DataClasses;

namespace System.Data.Entity.Core.Objects.Internal
{
  internal sealed class SnapshotChangeTrackingStrategy : IChangeTrackingStrategy
  {
    private static readonly SnapshotChangeTrackingStrategy _instance = new SnapshotChangeTrackingStrategy();

    public static SnapshotChangeTrackingStrategy Instance
    {
      get
      {
        return SnapshotChangeTrackingStrategy._instance;
      }
    }

    private SnapshotChangeTrackingStrategy()
    {
    }

    public void SetChangeTracker(IEntityChangeTracker changeTracker)
    {
    }

    public void TakeSnapshot(EntityEntry entry)
    {
      entry?.TakeSnapshot(false);
    }

    public void SetCurrentValue(
      EntityEntry entry,
      StateManagerMemberMetadata member,
      int ordinal,
      object target,
      object value)
    {
      if (object.ReferenceEquals(target, entry.Entity))
      {
        ((IEntityChangeTracker) entry).EntityMemberChanging(member.CLayerName);
        member.SetValue(target, value);
        ((IEntityChangeTracker) entry).EntityMemberChanged(member.CLayerName);
        if (!member.IsComplex)
          return;
        entry.UpdateComplexObjectSnapshot(member, target, ordinal, value);
      }
      else
      {
        member.SetValue(target, value);
        if (entry.State == EntityState.Added)
          return;
        entry.DetectChangesInProperties(true);
      }
    }

    public void UpdateCurrentValueRecord(object value, EntityEntry entry)
    {
      entry.UpdateRecordWithoutSetModified(value, (DbUpdatableDataRecord) entry.CurrentValues);
      entry.DetectChangesInProperties(false);
    }
  }
}
