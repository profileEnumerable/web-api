// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.Update.Internal.KeyManager
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Resources;
using System.Linq;

namespace System.Data.Entity.Core.Mapping.Update.Internal
{
  internal class KeyManager
  {
    private readonly Dictionary<Tuple<EntityKey, string, bool>, int> _foreignKeyIdentifiers = new Dictionary<Tuple<EntityKey, string, bool>, int>();
    private readonly Dictionary<EntityKey, EntityKey> _valueKeyToTempKey = new Dictionary<EntityKey, EntityKey>();
    private readonly Dictionary<EntityKey, int> _keyIdentifiers = new Dictionary<EntityKey, int>();
    private readonly List<KeyManager.IdentifierInfo> _identifiers = new List<KeyManager.IdentifierInfo>()
    {
      new KeyManager.IdentifierInfo()
    };
    private const byte White = 0;
    private const byte Black = 1;
    private const byte Gray = 2;

    internal int GetCliqueIdentifier(int identifier)
    {
      KeyManager.Partition partition = this._identifiers[identifier].Partition;
      if (partition != null)
        return partition.PartitionId;
      return identifier;
    }

    internal void AddReferentialConstraint(
      IEntityStateEntry dependentStateEntry,
      int dependentIdentifier,
      int principalIdentifier)
    {
      KeyManager.IdentifierInfo identifier = this._identifiers[dependentIdentifier];
      if (dependentIdentifier != principalIdentifier)
      {
        this.AssociateNodes(dependentIdentifier, principalIdentifier);
        KeyManager.LinkedList<int>.Add(ref identifier.References, principalIdentifier);
        KeyManager.LinkedList<int>.Add(ref this._identifiers[principalIdentifier].ReferencedBy, dependentIdentifier);
      }
      KeyManager.LinkedList<IEntityStateEntry>.Add(ref identifier.DependentStateEntries, dependentStateEntry);
    }

    internal void RegisterIdentifierOwner(PropagatorResult owner)
    {
      this._identifiers[owner.Identifier].Owner = owner;
    }

    internal bool TryGetIdentifierOwner(int identifier, out PropagatorResult owner)
    {
      owner = this._identifiers[identifier].Owner;
      return null != owner;
    }

    internal int GetKeyIdentifierForMemberOffset(
      EntityKey entityKey,
      int memberOffset,
      int keyMemberCount)
    {
      int count;
      if (!this._keyIdentifiers.TryGetValue(entityKey, out count))
      {
        count = this._identifiers.Count;
        for (int index = 0; index < keyMemberCount; ++index)
          this._identifiers.Add(new KeyManager.IdentifierInfo());
        this._keyIdentifiers.Add(entityKey, count);
      }
      return count + memberOffset;
    }

    internal int GetKeyIdentifierForMember(EntityKey entityKey, string member, bool currentValues)
    {
      Tuple<EntityKey, string, bool> key = Tuple.Create<EntityKey, string, bool>(entityKey, member, currentValues);
      int count;
      if (!this._foreignKeyIdentifiers.TryGetValue(key, out count))
      {
        count = this._identifiers.Count;
        this._identifiers.Add(new KeyManager.IdentifierInfo());
        this._foreignKeyIdentifiers.Add(key, count);
      }
      return count;
    }

    internal IEnumerable<IEntityStateEntry> GetDependentStateEntries(
      int identifier)
    {
      return KeyManager.LinkedList<IEntityStateEntry>.Enumerate(this._identifiers[identifier].DependentStateEntries);
    }

    internal object GetPrincipalValue(PropagatorResult result)
    {
      int identifier = result.Identifier;
      if (-1 == identifier)
        return result.GetSimpleValue();
      bool flag = true;
      object x = (object) null;
      foreach (int principal in this.GetPrincipals(identifier))
      {
        PropagatorResult owner = this._identifiers[principal].Owner;
        if (owner != null)
        {
          if (flag)
          {
            x = owner.GetSimpleValue();
            flag = false;
          }
          else if (!ByValueEqualityComparer.Default.Equals(x, owner.GetSimpleValue()))
            throw new ConstraintException(Strings.Update_ReferentialConstraintIntegrityViolation);
        }
      }
      if (flag)
        x = result.GetSimpleValue();
      return x;
    }

    internal IEnumerable<int> GetPrincipals(int identifier)
    {
      return this.WalkGraph(identifier, (Func<KeyManager.IdentifierInfo, KeyManager.LinkedList<int>>) (info => info.References), true);
    }

    internal IEnumerable<int> GetDirectReferences(int identifier)
    {
      KeyManager.LinkedList<int> references = this._identifiers[identifier].References;
      foreach (int num in KeyManager.LinkedList<int>.Enumerate(references))
        yield return num;
    }

    internal IEnumerable<int> GetDependents(int identifier)
    {
      return this.WalkGraph(identifier, (Func<KeyManager.IdentifierInfo, KeyManager.LinkedList<int>>) (info => info.ReferencedBy), false);
    }

    private IEnumerable<int> WalkGraph(
      int identifier,
      Func<KeyManager.IdentifierInfo, KeyManager.LinkedList<int>> successorFunction,
      bool leavesOnly)
    {
      Stack<int> stack = new Stack<int>();
      stack.Push(identifier);
      while (stack.Count > 0)
      {
        int currentIdentifier = stack.Pop();
        KeyManager.LinkedList<int> successors = successorFunction(this._identifiers[currentIdentifier]);
        if (successors != null)
        {
          foreach (int num in KeyManager.LinkedList<int>.Enumerate(successors))
            stack.Push(num);
          if (!leavesOnly)
            yield return currentIdentifier;
        }
        else
          yield return currentIdentifier;
      }
    }

    internal bool HasPrincipals(int identifier)
    {
      return null != this._identifiers[identifier].References;
    }

    internal void ValidateReferentialIntegrityGraphAcyclic()
    {
      byte[] color = new byte[this._identifiers.Count];
      int node = 0;
      for (int count = this._identifiers.Count; node < count; ++node)
      {
        if (color[node] == (byte) 0)
          this.ValidateReferentialIntegrityGraphAcyclic(node, color, (KeyManager.LinkedList<int>) null);
      }
    }

    internal void RegisterKeyValueForAddedEntity(IEntityStateEntry addedEntry)
    {
      EntityKey entityKey = addedEntry.EntityKey;
      ReadOnlyMetadataCollection<EdmMember> keyMembers = addedEntry.EntitySet.ElementType.KeyMembers;
      CurrentValueRecord currentValues = addedEntry.CurrentValues;
      object[] compositeKeyValues = new object[keyMembers.Count];
      bool flag = false;
      int index = 0;
      for (int count = keyMembers.Count; index < count; ++index)
      {
        int ordinal = currentValues.GetOrdinal(keyMembers[index].Name);
        if (currentValues.IsDBNull(ordinal))
        {
          flag = true;
          break;
        }
        compositeKeyValues[index] = currentValues.GetValue(ordinal);
      }
      if (flag)
        return;
      EntityKey key = compositeKeyValues.Length == 1 ? new EntityKey(addedEntry.EntitySet, compositeKeyValues[0]) : new EntityKey(addedEntry.EntitySet, compositeKeyValues);
      if (this._valueKeyToTempKey.ContainsKey(key))
        this._valueKeyToTempKey[key] = (EntityKey) null;
      else
        this._valueKeyToTempKey.Add(key, entityKey);
    }

    internal bool TryGetTempKey(EntityKey valueKey, out EntityKey tempKey)
    {
      return this._valueKeyToTempKey.TryGetValue(valueKey, out tempKey);
    }

    private void ValidateReferentialIntegrityGraphAcyclic(
      int node,
      byte[] color,
      KeyManager.LinkedList<int> parent)
    {
      color[node] = (byte) 2;
      KeyManager.LinkedList<int>.Add(ref parent, node);
      foreach (int node1 in KeyManager.LinkedList<int>.Enumerate(this._identifiers[node].References))
      {
        switch (color[node1])
        {
          case 0:
            this.ValidateReferentialIntegrityGraphAcyclic(node1, color, parent);
            continue;
          case 2:
            List<IEntityStateEntry> source = new List<IEntityStateEntry>();
            foreach (int index in KeyManager.LinkedList<int>.Enumerate(parent))
            {
              PropagatorResult owner = this._identifiers[index].Owner;
              if (owner != null)
                source.Add(owner.StateEntry);
              if (index == node1)
                break;
            }
            throw new UpdateException(Strings.Update_CircularRelationships, (Exception) null, source.Cast<ObjectStateEntry>().Distinct<ObjectStateEntry>());
          default:
            continue;
        }
      }
      color[node] = (byte) 1;
    }

    internal void AssociateNodes(int firstId, int secondId)
    {
      if (firstId == secondId)
        return;
      KeyManager.Partition partition1 = this._identifiers[firstId].Partition;
      if (partition1 != null)
      {
        KeyManager.Partition partition2 = this._identifiers[secondId].Partition;
        if (partition2 != null)
          partition1.Merge(this, partition2);
        else
          partition1.AddNode(this, secondId);
      }
      else
      {
        KeyManager.Partition partition2 = this._identifiers[secondId].Partition;
        if (partition2 != null)
          partition2.AddNode(this, firstId);
        else
          KeyManager.Partition.CreatePartition(this, firstId, secondId);
      }
    }

    private sealed class Partition
    {
      internal readonly int PartitionId;
      private readonly List<int> _nodeIds;

      private Partition(int partitionId)
      {
        this._nodeIds = new List<int>(2);
        this.PartitionId = partitionId;
      }

      internal static void CreatePartition(KeyManager manager, int firstId, int secondId)
      {
        KeyManager.Partition partition = new KeyManager.Partition(firstId);
        partition.AddNode(manager, firstId);
        partition.AddNode(manager, secondId);
      }

      internal void AddNode(KeyManager manager, int nodeId)
      {
        this._nodeIds.Add(nodeId);
        manager._identifiers[nodeId].Partition = this;
      }

      internal void Merge(KeyManager manager, KeyManager.Partition other)
      {
        if (other.PartitionId == this.PartitionId)
          return;
        foreach (int nodeId in other._nodeIds)
          this.AddNode(manager, nodeId);
      }
    }

    private sealed class LinkedList<T>
    {
      private readonly T _value;
      private readonly KeyManager.LinkedList<T> _previous;

      private LinkedList(T value, KeyManager.LinkedList<T> previous)
      {
        this._value = value;
        this._previous = previous;
      }

      internal static IEnumerable<T> Enumerate(KeyManager.LinkedList<T> current)
      {
        for (; current != null; current = current._previous)
          yield return current._value;
      }

      internal static void Add(ref KeyManager.LinkedList<T> list, T value)
      {
        list = new KeyManager.LinkedList<T>(value, list);
      }
    }

    private sealed class IdentifierInfo
    {
      internal KeyManager.Partition Partition;
      internal PropagatorResult Owner;
      internal KeyManager.LinkedList<IEntityStateEntry> DependentStateEntries;
      internal KeyManager.LinkedList<int> References;
      internal KeyManager.LinkedList<int> ReferencedBy;
    }
  }
}
