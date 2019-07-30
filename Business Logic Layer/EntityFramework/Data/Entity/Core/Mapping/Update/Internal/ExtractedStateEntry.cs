// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.Update.Internal.ExtractedStateEntry
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Mapping.Update.Internal
{
  internal struct ExtractedStateEntry
  {
    internal readonly EntityState State;
    internal readonly PropagatorResult Original;
    internal readonly PropagatorResult Current;
    internal readonly IEntityStateEntry Source;

    internal ExtractedStateEntry(
      EntityState state,
      PropagatorResult original,
      PropagatorResult current,
      IEntityStateEntry source)
    {
      this.State = state;
      this.Original = original;
      this.Current = current;
      this.Source = source;
    }

    internal ExtractedStateEntry(UpdateTranslator translator, IEntityStateEntry stateEntry)
    {
      this.State = stateEntry.State;
      this.Source = stateEntry;
      switch (stateEntry.State)
      {
        case EntityState.Unchanged:
          this.Original = translator.RecordConverter.ConvertOriginalValuesToPropagatorResult(stateEntry, ModifiedPropertiesBehavior.NoneModified);
          this.Current = translator.RecordConverter.ConvertCurrentValuesToPropagatorResult(stateEntry, ModifiedPropertiesBehavior.NoneModified);
          break;
        case EntityState.Added:
          this.Original = (PropagatorResult) null;
          this.Current = translator.RecordConverter.ConvertCurrentValuesToPropagatorResult(stateEntry, ModifiedPropertiesBehavior.AllModified);
          break;
        case EntityState.Deleted:
          this.Original = translator.RecordConverter.ConvertOriginalValuesToPropagatorResult(stateEntry, ModifiedPropertiesBehavior.AllModified);
          this.Current = (PropagatorResult) null;
          break;
        case EntityState.Modified:
          this.Original = translator.RecordConverter.ConvertOriginalValuesToPropagatorResult(stateEntry, ModifiedPropertiesBehavior.SomeModified);
          this.Current = translator.RecordConverter.ConvertCurrentValuesToPropagatorResult(stateEntry, ModifiedPropertiesBehavior.SomeModified);
          break;
        default:
          this.Original = (PropagatorResult) null;
          this.Current = (PropagatorResult) null;
          break;
      }
    }
  }
}
