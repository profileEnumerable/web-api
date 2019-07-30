// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.Update.Internal.ModificationFunctionMappingTranslator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Resources;
using System.Linq;

namespace System.Data.Entity.Core.Mapping.Update.Internal
{
  internal abstract class ModificationFunctionMappingTranslator
  {
    internal abstract FunctionUpdateCommand Translate(
      UpdateTranslator translator,
      ExtractedStateEntry stateEntry);

    internal static ModificationFunctionMappingTranslator CreateEntitySetTranslator(
      EntitySetMapping setMapping)
    {
      return (ModificationFunctionMappingTranslator) new ModificationFunctionMappingTranslator.EntitySetTranslator(setMapping);
    }

    internal static ModificationFunctionMappingTranslator CreateAssociationSetTranslator(
      AssociationSetMapping setMapping)
    {
      return (ModificationFunctionMappingTranslator) new ModificationFunctionMappingTranslator.AssociationSetTranslator(setMapping);
    }

    private sealed class EntitySetTranslator : ModificationFunctionMappingTranslator
    {
      private readonly Dictionary<EntityType, EntityTypeModificationFunctionMapping> m_typeMappings;

      internal EntitySetTranslator(EntitySetMapping setMapping)
      {
        this.m_typeMappings = new Dictionary<EntityType, EntityTypeModificationFunctionMapping>();
        foreach (EntityTypeModificationFunctionMapping modificationFunctionMapping in setMapping.ModificationFunctionMappings)
          this.m_typeMappings.Add(modificationFunctionMapping.EntityType, modificationFunctionMapping);
      }

      internal override FunctionUpdateCommand Translate(
        UpdateTranslator translator,
        ExtractedStateEntry stateEntry)
      {
        ModificationFunctionMapping functionMapping = this.GetFunctionMapping(stateEntry).Item2;
        EntityKey entityKey = stateEntry.Source.EntityKey;
        HashSet<IEntityStateEntry> entityStateEntrySet = new HashSet<IEntityStateEntry>()
        {
          stateEntry.Source
        };
        IEnumerable<Tuple<AssociationEndMember, IEntityStateEntry>> tuples = functionMapping.CollocatedAssociationSetEnds.Join<AssociationSetEnd, IEntityStateEntry, StructuralType, Tuple<AssociationEndMember, IEntityStateEntry>>(translator.GetRelationships(entityKey), (Func<AssociationSetEnd, StructuralType>) (end => end.CorrespondingAssociationEndMember.DeclaringType), (Func<IEntityStateEntry, StructuralType>) (candidateEntry => (StructuralType) candidateEntry.EntitySet.ElementType), (Func<AssociationSetEnd, IEntityStateEntry, Tuple<AssociationEndMember, IEntityStateEntry>>) ((end, candidateEntry) => Tuple.Create<AssociationEndMember, IEntityStateEntry>(end.CorrespondingAssociationEndMember, candidateEntry)));
        Dictionary<AssociationEndMember, IEntityStateEntry> dictionary1 = new Dictionary<AssociationEndMember, IEntityStateEntry>();
        Dictionary<AssociationEndMember, IEntityStateEntry> dictionary2 = new Dictionary<AssociationEndMember, IEntityStateEntry>();
        foreach (Tuple<AssociationEndMember, IEntityStateEntry> tuple in tuples)
          ModificationFunctionMappingTranslator.EntitySetTranslator.ProcessReferenceCandidate(entityKey, entityStateEntrySet, dictionary1, dictionary2, tuple.Item1, tuple.Item2);
        FunctionUpdateCommand command;
        if (entityStateEntrySet.All<IEntityStateEntry>((Func<IEntityStateEntry, bool>) (e => e.State == EntityState.Unchanged)))
        {
          command = (FunctionUpdateCommand) null;
        }
        else
        {
          command = new FunctionUpdateCommand(functionMapping, translator, new ReadOnlyCollection<IEntityStateEntry>((IList<IEntityStateEntry>) entityStateEntrySet.ToList<IEntityStateEntry>()), stateEntry);
          ModificationFunctionMappingTranslator.EntitySetTranslator.BindFunctionParameters(translator, stateEntry, functionMapping, command, dictionary1, dictionary2);
          if (functionMapping.ResultBindings != null)
          {
            foreach (ModificationFunctionResultBinding resultBinding in functionMapping.ResultBindings)
            {
              PropagatorResult memberValue = stateEntry.Current.GetMemberValue((EdmMember) resultBinding.Property);
              command.AddResultColumn(translator, resultBinding.ColumnName, memberValue);
            }
          }
        }
        return command;
      }

      private static void ProcessReferenceCandidate(
        EntityKey source,
        HashSet<IEntityStateEntry> stateEntries,
        Dictionary<AssociationEndMember, IEntityStateEntry> currentReferenceEnd,
        Dictionary<AssociationEndMember, IEntityStateEntry> originalReferenceEnd,
        AssociationEndMember endMember,
        IEntityStateEntry candidateEntry)
      {
        Func<DbDataRecord, int, EntityKey> getEntityKey = (Func<DbDataRecord, int, EntityKey>) ((record, ordinal) => (EntityKey) record[ordinal]);
        Action<DbDataRecord, Action<IEntityStateEntry>> action = (Action<DbDataRecord, Action<IEntityStateEntry>>) ((record, registerTarget) =>
        {
          int num = record.GetOrdinal(endMember.Name) == 0 ? 1 : 0;
          if (!(getEntityKey(record, num) == source))
            return;
          stateEntries.Add(candidateEntry);
          registerTarget(candidateEntry);
        });
        switch (candidateEntry.State)
        {
          case EntityState.Unchanged:
            action((DbDataRecord) candidateEntry.CurrentValues, (Action<IEntityStateEntry>) (target =>
            {
              currentReferenceEnd.Add(endMember, target);
              originalReferenceEnd.Add(endMember, target);
            }));
            break;
          case EntityState.Added:
            action((DbDataRecord) candidateEntry.CurrentValues, (Action<IEntityStateEntry>) (target => currentReferenceEnd.Add(endMember, target)));
            break;
          case EntityState.Deleted:
            action(candidateEntry.OriginalValues, (Action<IEntityStateEntry>) (target => originalReferenceEnd.Add(endMember, target)));
            break;
        }
      }

      private Tuple<EntityTypeModificationFunctionMapping, ModificationFunctionMapping> GetFunctionMapping(
        ExtractedStateEntry stateEntry)
      {
        EntityType index = stateEntry.Current == null ? (EntityType) stateEntry.Original.StructuralType : (EntityType) stateEntry.Current.StructuralType;
        EntityTypeModificationFunctionMapping typeMapping = this.m_typeMappings[index];
        ModificationFunctionMapping mapping;
        switch (stateEntry.State)
        {
          case EntityState.Unchanged:
          case EntityState.Modified:
            mapping = typeMapping.UpdateFunctionMapping;
            EntityUtil.ValidateNecessaryModificationFunctionMapping(mapping, "Update", stateEntry.Source, "EntityType", index.Name);
            break;
          case EntityState.Added:
            mapping = typeMapping.InsertFunctionMapping;
            EntityUtil.ValidateNecessaryModificationFunctionMapping(mapping, "Insert", stateEntry.Source, "EntityType", index.Name);
            break;
          case EntityState.Deleted:
            mapping = typeMapping.DeleteFunctionMapping;
            EntityUtil.ValidateNecessaryModificationFunctionMapping(mapping, "Delete", stateEntry.Source, "EntityType", index.Name);
            break;
          default:
            mapping = (ModificationFunctionMapping) null;
            break;
        }
        return Tuple.Create<EntityTypeModificationFunctionMapping, ModificationFunctionMapping>(typeMapping, mapping);
      }

      private static void BindFunctionParameters(
        UpdateTranslator translator,
        ExtractedStateEntry stateEntry,
        ModificationFunctionMapping functionMapping,
        FunctionUpdateCommand command,
        Dictionary<AssociationEndMember, IEntityStateEntry> currentReferenceEnds,
        Dictionary<AssociationEndMember, IEntityStateEntry> originalReferenceEnds)
      {
        foreach (ModificationFunctionParameterBinding parameterBinding in functionMapping.ParameterBindings)
        {
          PropagatorResult result;
          if (parameterBinding.MemberPath.AssociationSetEnd != null)
          {
            AssociationEndMember associationEndMember = parameterBinding.MemberPath.AssociationSetEnd.CorrespondingAssociationEndMember;
            IEntityStateEntry stateEntry1;
            if (!(parameterBinding.IsCurrent ? currentReferenceEnds.TryGetValue(associationEndMember, out stateEntry1) : originalReferenceEnds.TryGetValue(associationEndMember, out stateEntry1)))
            {
              if (associationEndMember.RelationshipMultiplicity == RelationshipMultiplicity.One)
                throw new UpdateException(Strings.Update_MissingRequiredRelationshipValue((object) stateEntry.Source.EntitySet.Name, (object) parameterBinding.MemberPath.AssociationSetEnd.ParentAssociationSet.Name), (Exception) null, command.GetStateEntries(translator).Cast<ObjectStateEntry>().Distinct<ObjectStateEntry>());
              result = PropagatorResult.CreateSimpleValue(PropagatorFlags.NoFlags, (object) null);
            }
            else
              result = (parameterBinding.IsCurrent ? translator.RecordConverter.ConvertCurrentValuesToPropagatorResult(stateEntry1, ModifiedPropertiesBehavior.AllModified) : translator.RecordConverter.ConvertOriginalValuesToPropagatorResult(stateEntry1, ModifiedPropertiesBehavior.AllModified)).GetMemberValue((EdmMember) associationEndMember).GetMemberValue(parameterBinding.MemberPath.Members[0]);
          }
          else
          {
            result = parameterBinding.IsCurrent ? stateEntry.Current : stateEntry.Original;
            int count = parameterBinding.MemberPath.Members.Count;
            while (count > 0)
            {
              --count;
              EdmMember member = parameterBinding.MemberPath.Members[count];
              result = result.GetMemberValue(member);
            }
          }
          command.SetParameterValue(result, parameterBinding, translator);
        }
        command.RegisterRowsAffectedParameter(functionMapping.RowsAffectedParameter);
      }
    }

    private sealed class AssociationSetTranslator : ModificationFunctionMappingTranslator
    {
      private readonly AssociationSetModificationFunctionMapping m_mapping;

      internal AssociationSetTranslator(AssociationSetMapping setMapping)
      {
        if (setMapping == null)
          return;
        this.m_mapping = setMapping.ModificationFunctionMapping;
      }

      internal override FunctionUpdateCommand Translate(
        UpdateTranslator translator,
        ExtractedStateEntry stateEntry)
      {
        if (this.m_mapping == null)
          return (FunctionUpdateCommand) null;
        bool flag = EntityState.Added == stateEntry.State;
        EntityUtil.ValidateNecessaryModificationFunctionMapping(flag ? this.m_mapping.InsertFunctionMapping : this.m_mapping.DeleteFunctionMapping, flag ? "Insert" : "Delete", stateEntry.Source, "AssociationSet", this.m_mapping.AssociationSet.Name);
        ModificationFunctionMapping functionMapping = flag ? this.m_mapping.InsertFunctionMapping : this.m_mapping.DeleteFunctionMapping;
        FunctionUpdateCommand functionUpdateCommand = new FunctionUpdateCommand(functionMapping, translator, new ReadOnlyCollection<IEntityStateEntry>((IList<IEntityStateEntry>) ((IEnumerable<IEntityStateEntry>) new IEntityStateEntry[1]
        {
          stateEntry.Source
        }).ToList<IEntityStateEntry>()), stateEntry);
        PropagatorResult propagatorResult = !flag ? stateEntry.Original : stateEntry.Current;
        foreach (ModificationFunctionParameterBinding parameterBinding in functionMapping.ParameterBindings)
        {
          EdmProperty member1 = (EdmProperty) parameterBinding.MemberPath.Members[0];
          AssociationEndMember member2 = (AssociationEndMember) parameterBinding.MemberPath.Members[1];
          PropagatorResult memberValue = propagatorResult.GetMemberValue((EdmMember) member2).GetMemberValue((EdmMember) member1);
          functionUpdateCommand.SetParameterValue(memberValue, parameterBinding, translator);
        }
        functionUpdateCommand.RegisterRowsAffectedParameter(functionMapping.RowsAffectedParameter);
        return functionUpdateCommand;
      }
    }
  }
}
