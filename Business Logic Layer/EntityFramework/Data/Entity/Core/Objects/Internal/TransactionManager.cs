// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.Internal.TransactionManager
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Infrastructure;

namespace System.Data.Entity.Core.Objects.Internal
{
  internal class TransactionManager
  {
    private MergeOption? _originalMergeOption;
    private int _graphUpdateCount;

    internal Dictionary<RelatedEnd, IList<IEntityWrapper>> PromotedRelationships { get; private set; }

    internal Dictionary<object, EntityEntry> PromotedKeyEntries { get; private set; }

    internal HashSet<EntityReference> PopulatedEntityReferences { get; private set; }

    internal HashSet<EntityReference> AlignedEntityReferences { get; private set; }

    internal MergeOption? OriginalMergeOption
    {
      get
      {
        return this._originalMergeOption;
      }
      set
      {
        this._originalMergeOption = value;
      }
    }

    internal HashSet<IEntityWrapper> ProcessedEntities { get; private set; }

    internal Dictionary<object, IEntityWrapper> WrappedEntities { get; private set; }

    internal bool TrackProcessedEntities { get; private set; }

    internal bool IsAddTracking { get; private set; }

    internal bool IsAttachTracking { get; private set; }

    internal Dictionary<IEntityWrapper, Dictionary<RelatedEnd, HashSet<IEntityWrapper>>> AddedRelationshipsByGraph { get; private set; }

    internal Dictionary<IEntityWrapper, Dictionary<RelatedEnd, HashSet<IEntityWrapper>>> DeletedRelationshipsByGraph { get; private set; }

    internal Dictionary<IEntityWrapper, Dictionary<RelatedEnd, HashSet<EntityKey>>> AddedRelationshipsByForeignKey { get; private set; }

    internal Dictionary<IEntityWrapper, Dictionary<RelatedEnd, HashSet<EntityKey>>> AddedRelationshipsByPrincipalKey { get; private set; }

    internal Dictionary<IEntityWrapper, Dictionary<RelatedEnd, HashSet<EntityKey>>> DeletedRelationshipsByForeignKey { get; private set; }

    internal Dictionary<IEntityWrapper, HashSet<RelatedEnd>> ChangedForeignKeys { get; private set; }

    internal bool IsDetectChanges { get; private set; }

    internal bool IsAlignChanges { get; private set; }

    internal bool IsLocalPublicAPI { get; private set; }

    internal bool IsOriginalValuesGetter { get; private set; }

    internal bool IsForeignKeyUpdate { get; private set; }

    internal bool IsRelatedEndAdd { get; private set; }

    internal bool IsGraphUpdate
    {
      get
      {
        return this._graphUpdateCount != 0;
      }
    }

    internal object EntityBeingReparented { get; set; }

    internal bool IsDetaching { get; private set; }

    internal EntityReference RelationshipBeingUpdated { get; private set; }

    internal bool IsFixupByReference { get; private set; }

    internal void BeginAddTracking()
    {
      this.IsAddTracking = true;
      this.PopulatedEntityReferences = new HashSet<EntityReference>();
      this.AlignedEntityReferences = new HashSet<EntityReference>();
      this.PromotedRelationships = new Dictionary<RelatedEnd, IList<IEntityWrapper>>();
      if (this.IsDetectChanges)
        return;
      this.TrackProcessedEntities = true;
      this.ProcessedEntities = new HashSet<IEntityWrapper>();
      this.WrappedEntities = new Dictionary<object, IEntityWrapper>((IEqualityComparer<object>) ObjectReferenceEqualityComparer.Default);
    }

    internal void EndAddTracking()
    {
      this.IsAddTracking = false;
      this.PopulatedEntityReferences = (HashSet<EntityReference>) null;
      this.AlignedEntityReferences = (HashSet<EntityReference>) null;
      this.PromotedRelationships = (Dictionary<RelatedEnd, IList<IEntityWrapper>>) null;
      if (this.IsDetectChanges)
        return;
      this.TrackProcessedEntities = false;
      this.ProcessedEntities = (HashSet<IEntityWrapper>) null;
      this.WrappedEntities = (Dictionary<object, IEntityWrapper>) null;
    }

    internal void BeginAttachTracking()
    {
      this.IsAttachTracking = true;
      this.PromotedRelationships = new Dictionary<RelatedEnd, IList<IEntityWrapper>>();
      this.PromotedKeyEntries = new Dictionary<object, EntityEntry>((IEqualityComparer<object>) ObjectReferenceEqualityComparer.Default);
      this.PopulatedEntityReferences = new HashSet<EntityReference>();
      this.AlignedEntityReferences = new HashSet<EntityReference>();
      this.TrackProcessedEntities = true;
      this.ProcessedEntities = new HashSet<IEntityWrapper>();
      this.WrappedEntities = new Dictionary<object, IEntityWrapper>((IEqualityComparer<object>) ObjectReferenceEqualityComparer.Default);
      this.OriginalMergeOption = new MergeOption?();
    }

    internal void EndAttachTracking()
    {
      this.IsAttachTracking = false;
      this.PromotedRelationships = (Dictionary<RelatedEnd, IList<IEntityWrapper>>) null;
      this.PromotedKeyEntries = (Dictionary<object, EntityEntry>) null;
      this.PopulatedEntityReferences = (HashSet<EntityReference>) null;
      this.AlignedEntityReferences = (HashSet<EntityReference>) null;
      this.TrackProcessedEntities = false;
      this.ProcessedEntities = (HashSet<IEntityWrapper>) null;
      this.WrappedEntities = (Dictionary<object, IEntityWrapper>) null;
      this.OriginalMergeOption = new MergeOption?();
    }

    internal bool BeginDetectChanges()
    {
      if (this.IsDetectChanges)
        return false;
      this.IsDetectChanges = true;
      this.TrackProcessedEntities = true;
      this.ProcessedEntities = new HashSet<IEntityWrapper>();
      this.WrappedEntities = new Dictionary<object, IEntityWrapper>((IEqualityComparer<object>) ObjectReferenceEqualityComparer.Default);
      this.DeletedRelationshipsByGraph = new Dictionary<IEntityWrapper, Dictionary<RelatedEnd, HashSet<IEntityWrapper>>>();
      this.AddedRelationshipsByGraph = new Dictionary<IEntityWrapper, Dictionary<RelatedEnd, HashSet<IEntityWrapper>>>();
      this.DeletedRelationshipsByForeignKey = new Dictionary<IEntityWrapper, Dictionary<RelatedEnd, HashSet<EntityKey>>>();
      this.AddedRelationshipsByForeignKey = new Dictionary<IEntityWrapper, Dictionary<RelatedEnd, HashSet<EntityKey>>>();
      this.AddedRelationshipsByPrincipalKey = new Dictionary<IEntityWrapper, Dictionary<RelatedEnd, HashSet<EntityKey>>>();
      this.ChangedForeignKeys = new Dictionary<IEntityWrapper, HashSet<RelatedEnd>>();
      return true;
    }

    internal void EndDetectChanges()
    {
      this.IsDetectChanges = false;
      this.TrackProcessedEntities = false;
      this.ProcessedEntities = (HashSet<IEntityWrapper>) null;
      this.WrappedEntities = (Dictionary<object, IEntityWrapper>) null;
      this.DeletedRelationshipsByGraph = (Dictionary<IEntityWrapper, Dictionary<RelatedEnd, HashSet<IEntityWrapper>>>) null;
      this.AddedRelationshipsByGraph = (Dictionary<IEntityWrapper, Dictionary<RelatedEnd, HashSet<IEntityWrapper>>>) null;
      this.DeletedRelationshipsByForeignKey = (Dictionary<IEntityWrapper, Dictionary<RelatedEnd, HashSet<EntityKey>>>) null;
      this.AddedRelationshipsByForeignKey = (Dictionary<IEntityWrapper, Dictionary<RelatedEnd, HashSet<EntityKey>>>) null;
      this.AddedRelationshipsByPrincipalKey = (Dictionary<IEntityWrapper, Dictionary<RelatedEnd, HashSet<EntityKey>>>) null;
      this.ChangedForeignKeys = (Dictionary<IEntityWrapper, HashSet<RelatedEnd>>) null;
    }

    internal void BeginAlignChanges()
    {
      this.IsAlignChanges = true;
    }

    internal void EndAlignChanges()
    {
      this.IsAlignChanges = false;
    }

    internal void ResetProcessedEntities()
    {
      this.ProcessedEntities.Clear();
    }

    internal void BeginLocalPublicAPI()
    {
      this.IsLocalPublicAPI = true;
    }

    internal void EndLocalPublicAPI()
    {
      this.IsLocalPublicAPI = false;
    }

    internal void BeginOriginalValuesGetter()
    {
      this.IsOriginalValuesGetter = true;
    }

    internal void EndOriginalValuesGetter()
    {
      this.IsOriginalValuesGetter = false;
    }

    internal void BeginForeignKeyUpdate(EntityReference relationship)
    {
      this.RelationshipBeingUpdated = relationship;
      this.IsForeignKeyUpdate = true;
    }

    internal void EndForeignKeyUpdate()
    {
      this.RelationshipBeingUpdated = (EntityReference) null;
      this.IsForeignKeyUpdate = false;
    }

    internal void BeginRelatedEndAdd()
    {
      this.IsRelatedEndAdd = true;
    }

    internal void EndRelatedEndAdd()
    {
      this.IsRelatedEndAdd = false;
    }

    internal void BeginGraphUpdate()
    {
      ++this._graphUpdateCount;
    }

    internal void EndGraphUpdate()
    {
      --this._graphUpdateCount;
    }

    internal void BeginDetaching()
    {
      this.IsDetaching = true;
    }

    internal void EndDetaching()
    {
      this.IsDetaching = false;
    }

    internal void BeginFixupKeysByReference()
    {
      this.IsFixupByReference = true;
    }

    internal void EndFixupKeysByReference()
    {
      this.IsFixupByReference = false;
    }
  }
}
