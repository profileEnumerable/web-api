// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.Update.Internal.UpdateCommandOrderer
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
  internal class UpdateCommandOrderer : Graph<UpdateCommand>
  {
    private readonly UpdateCommandOrderer.ForeignKeyValueComparer _keyComparer;
    private readonly KeyToListMap<EntitySetBase, ReferentialConstraint> _sourceMap;
    private readonly KeyToListMap<EntitySetBase, ReferentialConstraint> _targetMap;
    private readonly bool _hasFunctionCommands;
    private readonly UpdateTranslator _translator;

    internal UpdateCommandOrderer(IEnumerable<UpdateCommand> commands, UpdateTranslator translator)
      : base((IEqualityComparer<UpdateCommand>) EqualityComparer<UpdateCommand>.Default)
    {
      this._translator = translator;
      this._keyComparer = new UpdateCommandOrderer.ForeignKeyValueComparer(this._translator.KeyComparer);
      HashSet<EntitySet> tables = new HashSet<EntitySet>();
      HashSet<EntityContainer> containers = new HashSet<EntityContainer>();
      foreach (UpdateCommand command in commands)
      {
        if (command.Table != null)
        {
          tables.Add(command.Table);
          containers.Add(command.Table.EntityContainer);
        }
        this.AddVertex(command);
        if (command.Kind == UpdateCommandKind.Function)
          this._hasFunctionCommands = true;
      }
      UpdateCommandOrderer.InitializeForeignKeyMaps(containers, tables, out this._sourceMap, out this._targetMap);
      this.AddServerGenDependencies();
      this.AddForeignKeyDependencies();
      if (!this._hasFunctionCommands)
        return;
      this.AddModelDependencies();
    }

    private static void InitializeForeignKeyMaps(
      HashSet<EntityContainer> containers,
      HashSet<EntitySet> tables,
      out KeyToListMap<EntitySetBase, ReferentialConstraint> sourceMap,
      out KeyToListMap<EntitySetBase, ReferentialConstraint> targetMap)
    {
      sourceMap = new KeyToListMap<EntitySetBase, ReferentialConstraint>((IEqualityComparer<EntitySetBase>) EqualityComparer<EntitySetBase>.Default);
      targetMap = new KeyToListMap<EntitySetBase, ReferentialConstraint>((IEqualityComparer<EntitySetBase>) EqualityComparer<EntitySetBase>.Default);
      foreach (EntityContainer container in containers)
      {
        foreach (EntitySetBase baseEntitySet in container.BaseEntitySets)
        {
          AssociationSet associationSet = baseEntitySet as AssociationSet;
          if (associationSet != null)
          {
            AssociationSetEnd associationSetEnd1 = (AssociationSetEnd) null;
            AssociationSetEnd associationSetEnd2 = (AssociationSetEnd) null;
            if (2 == associationSet.AssociationSetEnds.Count)
            {
              AssociationType elementType = associationSet.ElementType;
              bool flag = false;
              ReferentialConstraint referentialConstraint1 = (ReferentialConstraint) null;
              foreach (ReferentialConstraint referentialConstraint2 in elementType.ReferentialConstraints)
              {
                if (!flag)
                  flag = true;
                associationSetEnd1 = associationSet.AssociationSetEnds[referentialConstraint2.ToRole.Name];
                associationSetEnd2 = associationSet.AssociationSetEnds[referentialConstraint2.FromRole.Name];
                referentialConstraint1 = referentialConstraint2;
              }
              if (associationSetEnd2 != null && associationSetEnd1 != null && (tables.Contains(associationSetEnd2.EntitySet) && tables.Contains(associationSetEnd1.EntitySet)))
              {
                sourceMap.Add((EntitySetBase) associationSetEnd1.EntitySet, referentialConstraint1);
                targetMap.Add((EntitySetBase) associationSetEnd2.EntitySet, referentialConstraint1);
              }
            }
          }
        }
      }
    }

    private void AddServerGenDependencies()
    {
      Dictionary<int, UpdateCommand> dictionary = new Dictionary<int, UpdateCommand>();
      foreach (UpdateCommand vertex in this.Vertices)
      {
        foreach (int outputIdentifier in vertex.OutputIdentifiers)
        {
          try
          {
            dictionary.Add(outputIdentifier, vertex);
          }
          catch (ArgumentException ex)
          {
            throw new UpdateException(Strings.Update_AmbiguousServerGenIdentifier, (Exception) ex, vertex.GetStateEntries(this._translator).Cast<ObjectStateEntry>().Distinct<ObjectStateEntry>());
          }
        }
      }
      foreach (UpdateCommand vertex in this.Vertices)
      {
        foreach (int inputIdentifier in vertex.InputIdentifiers)
        {
          UpdateCommand from;
          if (dictionary.TryGetValue(inputIdentifier, out from))
            this.AddEdge(from, vertex);
        }
      }
    }

    private void AddForeignKeyDependencies()
    {
      this.AddForeignKeyEdges(this.DetermineForeignKeyPredecessors());
    }

    private void AddForeignKeyEdges(
      KeyToListMap<UpdateCommandOrderer.ForeignKeyValue, UpdateCommand> predecessors)
    {
      foreach (DynamicUpdateCommand dynamicUpdateCommand in this.Vertices.OfType<DynamicUpdateCommand>())
      {
        if (dynamicUpdateCommand.Operator == ModificationOperator.Update || ModificationOperator.Insert == dynamicUpdateCommand.Operator)
        {
          foreach (ReferentialConstraint enumerateValue1 in this._sourceMap.EnumerateValues((EntitySetBase) dynamicUpdateCommand.Table))
          {
            UpdateCommandOrderer.ForeignKeyValue key1;
            UpdateCommandOrderer.ForeignKeyValue key2;
            if (UpdateCommandOrderer.ForeignKeyValue.TryCreateSourceKey(enumerateValue1, dynamicUpdateCommand.CurrentValues, true, out key1) && (dynamicUpdateCommand.Operator != ModificationOperator.Update || !UpdateCommandOrderer.ForeignKeyValue.TryCreateSourceKey(enumerateValue1, dynamicUpdateCommand.OriginalValues, true, out key2) || !this._keyComparer.Equals(key2, key1)))
            {
              foreach (UpdateCommand enumerateValue2 in predecessors.EnumerateValues(key1))
              {
                if (enumerateValue2 != dynamicUpdateCommand)
                  this.AddEdge(enumerateValue2, (UpdateCommand) dynamicUpdateCommand);
              }
            }
          }
        }
        if (dynamicUpdateCommand.Operator == ModificationOperator.Update || ModificationOperator.Delete == dynamicUpdateCommand.Operator)
        {
          foreach (ReferentialConstraint enumerateValue1 in this._targetMap.EnumerateValues((EntitySetBase) dynamicUpdateCommand.Table))
          {
            UpdateCommandOrderer.ForeignKeyValue key1;
            UpdateCommandOrderer.ForeignKeyValue key2;
            if (UpdateCommandOrderer.ForeignKeyValue.TryCreateTargetKey(enumerateValue1, dynamicUpdateCommand.OriginalValues, false, out key1) && (dynamicUpdateCommand.Operator != ModificationOperator.Update || !UpdateCommandOrderer.ForeignKeyValue.TryCreateTargetKey(enumerateValue1, dynamicUpdateCommand.CurrentValues, false, out key2) || !this._keyComparer.Equals(key2, key1)))
            {
              foreach (UpdateCommand enumerateValue2 in predecessors.EnumerateValues(key1))
              {
                if (enumerateValue2 != dynamicUpdateCommand)
                  this.AddEdge(enumerateValue2, (UpdateCommand) dynamicUpdateCommand);
              }
            }
          }
        }
      }
    }

    private KeyToListMap<UpdateCommandOrderer.ForeignKeyValue, UpdateCommand> DetermineForeignKeyPredecessors()
    {
      KeyToListMap<UpdateCommandOrderer.ForeignKeyValue, UpdateCommand> keyToListMap = new KeyToListMap<UpdateCommandOrderer.ForeignKeyValue, UpdateCommand>((IEqualityComparer<UpdateCommandOrderer.ForeignKeyValue>) this._keyComparer);
      foreach (DynamicUpdateCommand dynamicUpdateCommand in this.Vertices.OfType<DynamicUpdateCommand>())
      {
        if (dynamicUpdateCommand.Operator == ModificationOperator.Update || ModificationOperator.Insert == dynamicUpdateCommand.Operator)
        {
          foreach (ReferentialConstraint enumerateValue in this._targetMap.EnumerateValues((EntitySetBase) dynamicUpdateCommand.Table))
          {
            UpdateCommandOrderer.ForeignKeyValue key1;
            UpdateCommandOrderer.ForeignKeyValue key2;
            if (UpdateCommandOrderer.ForeignKeyValue.TryCreateTargetKey(enumerateValue, dynamicUpdateCommand.CurrentValues, true, out key1) && (dynamicUpdateCommand.Operator != ModificationOperator.Update || !UpdateCommandOrderer.ForeignKeyValue.TryCreateTargetKey(enumerateValue, dynamicUpdateCommand.OriginalValues, true, out key2) || !this._keyComparer.Equals(key2, key1)))
              keyToListMap.Add(key1, (UpdateCommand) dynamicUpdateCommand);
          }
        }
        if (dynamicUpdateCommand.Operator == ModificationOperator.Update || ModificationOperator.Delete == dynamicUpdateCommand.Operator)
        {
          foreach (ReferentialConstraint enumerateValue in this._sourceMap.EnumerateValues((EntitySetBase) dynamicUpdateCommand.Table))
          {
            UpdateCommandOrderer.ForeignKeyValue key1;
            UpdateCommandOrderer.ForeignKeyValue key2;
            if (UpdateCommandOrderer.ForeignKeyValue.TryCreateSourceKey(enumerateValue, dynamicUpdateCommand.OriginalValues, false, out key1) && (dynamicUpdateCommand.Operator != ModificationOperator.Update || !UpdateCommandOrderer.ForeignKeyValue.TryCreateSourceKey(enumerateValue, dynamicUpdateCommand.CurrentValues, false, out key2) || !this._keyComparer.Equals(key2, key1)))
              keyToListMap.Add(key1, (UpdateCommand) dynamicUpdateCommand);
          }
        }
      }
      return keyToListMap;
    }

    private void AddModelDependencies()
    {
      KeyToListMap<EntityKey, UpdateCommand> keyToListMap1 = new KeyToListMap<EntityKey, UpdateCommand>((IEqualityComparer<EntityKey>) EqualityComparer<EntityKey>.Default);
      KeyToListMap<EntityKey, UpdateCommand> keyToListMap2 = new KeyToListMap<EntityKey, UpdateCommand>((IEqualityComparer<EntityKey>) EqualityComparer<EntityKey>.Default);
      KeyToListMap<EntityKey, UpdateCommand> keyToListMap3 = new KeyToListMap<EntityKey, UpdateCommand>((IEqualityComparer<EntityKey>) EqualityComparer<EntityKey>.Default);
      KeyToListMap<EntityKey, UpdateCommand> keyToListMap4 = new KeyToListMap<EntityKey, UpdateCommand>((IEqualityComparer<EntityKey>) EqualityComparer<EntityKey>.Default);
      foreach (UpdateCommand vertex in this.Vertices)
        vertex.GetRequiredAndProducedEntities(this._translator, keyToListMap1, keyToListMap2, keyToListMap3, keyToListMap4);
      this.AddModelDependencies(keyToListMap1, keyToListMap3);
      this.AddModelDependencies(keyToListMap4, keyToListMap2);
    }

    private void AddModelDependencies(
      KeyToListMap<EntityKey, UpdateCommand> producedMap,
      KeyToListMap<EntityKey, UpdateCommand> requiredMap)
    {
      foreach (KeyValuePair<EntityKey, List<UpdateCommand>> keyValuePair in requiredMap.KeyValuePairs)
      {
        EntityKey key = keyValuePair.Key;
        List<UpdateCommand> updateCommandList = keyValuePair.Value;
        foreach (UpdateCommand enumerateValue in producedMap.EnumerateValues(key))
        {
          foreach (UpdateCommand to in updateCommandList)
          {
            if (!object.ReferenceEquals((object) enumerateValue, (object) to) && (enumerateValue.Kind == UpdateCommandKind.Function || to.Kind == UpdateCommandKind.Function))
              this.AddEdge(enumerateValue, to);
          }
        }
      }
    }

    private struct ForeignKeyValue
    {
      internal readonly ReferentialConstraint Metadata;
      internal readonly CompositeKey Key;
      internal readonly bool IsInsert;

      private ForeignKeyValue(
        ReferentialConstraint metadata,
        PropagatorResult record,
        bool isTarget,
        bool isInsert)
      {
        this.Metadata = metadata;
        IList<EdmProperty> edmPropertyList = isTarget ? (IList<EdmProperty>) metadata.FromProperties : (IList<EdmProperty>) metadata.ToProperties;
        PropagatorResult[] constants = new PropagatorResult[edmPropertyList.Count];
        bool flag = false;
        for (int index = 0; index < constants.Length; ++index)
        {
          constants[index] = record.GetMemberValue((EdmMember) edmPropertyList[index]);
          if (constants[index].IsNull)
          {
            flag = true;
            break;
          }
        }
        this.Key = !flag ? new CompositeKey(constants) : (CompositeKey) null;
        this.IsInsert = isInsert;
      }

      internal static bool TryCreateTargetKey(
        ReferentialConstraint metadata,
        PropagatorResult record,
        bool isInsert,
        out UpdateCommandOrderer.ForeignKeyValue key)
      {
        key = new UpdateCommandOrderer.ForeignKeyValue(metadata, record, true, isInsert);
        return key.Key != null;
      }

      internal static bool TryCreateSourceKey(
        ReferentialConstraint metadata,
        PropagatorResult record,
        bool isInsert,
        out UpdateCommandOrderer.ForeignKeyValue key)
      {
        key = new UpdateCommandOrderer.ForeignKeyValue(metadata, record, false, isInsert);
        return key.Key != null;
      }
    }

    private class ForeignKeyValueComparer : IEqualityComparer<UpdateCommandOrderer.ForeignKeyValue>
    {
      private readonly IEqualityComparer<CompositeKey> _baseComparer;

      internal ForeignKeyValueComparer(IEqualityComparer<CompositeKey> baseComparer)
      {
        this._baseComparer = baseComparer;
      }

      public bool Equals(
        UpdateCommandOrderer.ForeignKeyValue x,
        UpdateCommandOrderer.ForeignKeyValue y)
      {
        if (x.IsInsert == y.IsInsert && x.Metadata == y.Metadata)
          return this._baseComparer.Equals(x.Key, y.Key);
        return false;
      }

      public int GetHashCode(UpdateCommandOrderer.ForeignKeyValue obj)
      {
        return this._baseComparer.GetHashCode(obj.Key);
      }
    }
  }
}
