// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.Update.Internal.UpdateCommand
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity.Core.Mapping.Update.Internal
{
  internal abstract class UpdateCommand : IComparable<UpdateCommand>, IEquatable<UpdateCommand>
  {
    private static int OrderingIdentifierCounter;
    private int _orderingIdentifier;

    protected UpdateCommand(
      UpdateTranslator translator,
      PropagatorResult originalValues,
      PropagatorResult currentValues)
    {
      this.OriginalValues = originalValues;
      this.CurrentValues = currentValues;
      this.Translator = translator;
    }

    internal abstract IEnumerable<int> OutputIdentifiers { get; }

    internal abstract IEnumerable<int> InputIdentifiers { get; }

    internal virtual EntitySet Table
    {
      get
      {
        return (EntitySet) null;
      }
    }

    internal abstract UpdateCommandKind Kind { get; }

    internal PropagatorResult OriginalValues { get; private set; }

    internal PropagatorResult CurrentValues { get; private set; }

    protected UpdateTranslator Translator { get; private set; }

    internal abstract IList<IEntityStateEntry> GetStateEntries(
      UpdateTranslator translator);

    internal void GetRequiredAndProducedEntities(
      UpdateTranslator translator,
      KeyToListMap<EntityKey, UpdateCommand> addedEntities,
      KeyToListMap<EntityKey, UpdateCommand> deletedEntities,
      KeyToListMap<EntityKey, UpdateCommand> addedRelationships,
      KeyToListMap<EntityKey, UpdateCommand> deletedRelationships)
    {
      IList<IEntityStateEntry> stateEntries = this.GetStateEntries(translator);
      foreach (IEntityStateEntry entityStateEntry in (IEnumerable<IEntityStateEntry>) stateEntries)
      {
        if (!entityStateEntry.IsRelationship)
        {
          if (entityStateEntry.State == EntityState.Added)
            addedEntities.Add(entityStateEntry.EntityKey, this);
          else if (entityStateEntry.State == EntityState.Deleted)
            deletedEntities.Add(entityStateEntry.EntityKey, this);
        }
      }
      if (this.OriginalValues != null)
        this.AddReferencedEntities(translator, this.OriginalValues, deletedRelationships);
      if (this.CurrentValues != null)
        this.AddReferencedEntities(translator, this.CurrentValues, addedRelationships);
      foreach (IEntityStateEntry entityStateEntry in (IEnumerable<IEntityStateEntry>) stateEntries)
      {
        if (entityStateEntry.IsRelationship)
        {
          bool flag = entityStateEntry.State == EntityState.Added;
          if (flag || entityStateEntry.State == EntityState.Deleted)
          {
            DbDataRecord dbDataRecord = flag ? (DbDataRecord) entityStateEntry.CurrentValues : entityStateEntry.OriginalValues;
            EntityKey key1 = (EntityKey) dbDataRecord[0];
            EntityKey key2 = (EntityKey) dbDataRecord[1];
            KeyToListMap<EntityKey, UpdateCommand> keyToListMap = flag ? addedRelationships : deletedRelationships;
            keyToListMap.Add(key1, this);
            keyToListMap.Add(key2, this);
          }
        }
      }
    }

    private void AddReferencedEntities(
      UpdateTranslator translator,
      PropagatorResult result,
      KeyToListMap<EntityKey, UpdateCommand> referencedEntities)
    {
      foreach (PropagatorResult memberValue in result.GetMemberValues())
      {
        if (memberValue.IsSimple && memberValue.Identifier != -1 && PropagatorFlags.ForeignKey == (memberValue.PropagatorFlags & PropagatorFlags.ForeignKey))
        {
          foreach (int directReference in translator.KeyManager.GetDirectReferences(memberValue.Identifier))
          {
            PropagatorResult owner;
            if (translator.KeyManager.TryGetIdentifierOwner(directReference, out owner) && owner.StateEntry != null)
              referencedEntities.Add(owner.StateEntry.EntityKey, this);
          }
        }
      }
    }

    internal abstract long Execute(
      Dictionary<int, object> identifierValues,
      List<KeyValuePair<PropagatorResult, object>> generatedValues);

    internal abstract Task<long> ExecuteAsync(
      Dictionary<int, object> identifierValues,
      List<KeyValuePair<PropagatorResult, object>> generatedValues,
      CancellationToken cancellationToken);

    internal abstract int CompareToType(UpdateCommand other);

    public int CompareTo(UpdateCommand other)
    {
      if (this.Equals(other))
        return 0;
      int num = this.Kind - other.Kind;
      if (num != 0)
        return num;
      int type = this.CompareToType(other);
      if (type != 0)
        return type;
      if (this._orderingIdentifier == 0)
        this._orderingIdentifier = Interlocked.Increment(ref UpdateCommand.OrderingIdentifierCounter);
      if (other._orderingIdentifier == 0)
        other._orderingIdentifier = Interlocked.Increment(ref UpdateCommand.OrderingIdentifierCounter);
      return this._orderingIdentifier - other._orderingIdentifier;
    }

    public bool Equals(UpdateCommand other)
    {
      return base.Equals((object) other);
    }

    public override bool Equals(object obj)
    {
      return base.Equals(obj);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
  }
}
