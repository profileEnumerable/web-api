// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.Update.Internal.ExtractorMetadata
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.Core.Mapping.Update.Internal
{
  internal class ExtractorMetadata
  {
    private readonly ExtractorMetadata.MemberInformation[] m_memberMap;
    private readonly StructuralType m_type;
    private readonly UpdateTranslator m_translator;

    internal ExtractorMetadata(
      EntitySetBase entitySetBase,
      StructuralType type,
      UpdateTranslator translator)
    {
      this.m_type = type;
      this.m_translator = translator;
      EntityType entityType = (EntityType) null;
      Set<EdmMember> set1;
      Set<EdmMember> set2;
      switch (type.BuiltInTypeKind)
      {
        case BuiltInTypeKind.EntityType:
          entityType = (EntityType) type;
          set1 = new Set<EdmMember>((IEnumerable<EdmMember>) entityType.KeyMembers).MakeReadOnly();
          set2 = new Set<EdmMember>((IEnumerable<EdmMember>) ((EntitySet) entitySetBase).ForeignKeyDependents.SelectMany<Tuple<AssociationSet, ReferentialConstraint>, EdmProperty>((Func<Tuple<AssociationSet, ReferentialConstraint>, IEnumerable<EdmProperty>>) (fk => (IEnumerable<EdmProperty>) fk.Item2.ToProperties))).MakeReadOnly();
          break;
        case BuiltInTypeKind.RowType:
          set1 = new Set<EdmMember>((IEnumerable<EdmMember>) ((RowType) type).Properties).MakeReadOnly();
          set2 = Set<EdmMember>.Empty;
          break;
        default:
          set1 = Set<EdmMember>.Empty;
          set2 = Set<EdmMember>.Empty;
          break;
      }
      IBaseList<EdmMember> structuralMembers = TypeHelpers.GetAllStructuralMembers((EdmType) type);
      this.m_memberMap = new ExtractorMetadata.MemberInformation[structuralMembers.Count];
      for (int ordinal = 0; ordinal < structuralMembers.Count; ++ordinal)
      {
        EdmMember edmMember = structuralMembers[ordinal];
        PropagatorFlags flags = PropagatorFlags.NoFlags;
        int? entityKeyOrdinal = new int?();
        if (set1.Contains(edmMember))
        {
          flags |= PropagatorFlags.Key;
          if (entityType != null)
            entityKeyOrdinal = new int?(entityType.KeyMembers.IndexOf(edmMember));
        }
        if (set2.Contains(edmMember))
          flags |= PropagatorFlags.ForeignKey;
        if (MetadataHelper.GetConcurrencyMode(edmMember) == ConcurrencyMode.Fixed)
          flags |= PropagatorFlags.ConcurrencyValue;
        bool isServerGenerated = this.m_translator.ViewLoader.IsServerGen(entitySetBase, this.m_translator.MetadataWorkspace, edmMember);
        bool isNullConditionMember = this.m_translator.ViewLoader.IsNullConditionMember(entitySetBase, this.m_translator.MetadataWorkspace, edmMember);
        this.m_memberMap[ordinal] = new ExtractorMetadata.MemberInformation(ordinal, entityKeyOrdinal, flags, edmMember, isServerGenerated, isNullConditionMember);
      }
    }

    internal PropagatorResult RetrieveMember(
      IEntityStateEntry stateEntry,
      IExtendedDataRecord record,
      bool useCurrentValues,
      EntityKey key,
      int ordinal,
      ModifiedPropertiesBehavior modifiedPropertiesBehavior)
    {
      ExtractorMetadata.MemberInformation member = this.m_memberMap[ordinal];
      int identifier;
      if (member.IsKeyMember)
      {
        int memberOffset = member.EntityKeyOrdinal.Value;
        identifier = this.m_translator.KeyManager.GetKeyIdentifierForMemberOffset(key, memberOffset, ((EntityTypeBase) this.m_type).KeyMembers.Count);
      }
      else
        identifier = !member.IsForeignKeyMember ? -1 : this.m_translator.KeyManager.GetKeyIdentifierForMember(key, record.GetName(ordinal), useCurrentValues);
      int num;
      switch (modifiedPropertiesBehavior)
      {
        case ModifiedPropertiesBehavior.AllModified:
          num = 1;
          break;
        case ModifiedPropertiesBehavior.SomeModified:
          if (stateEntry.ModifiedProperties != null)
          {
            num = stateEntry.ModifiedProperties[member.Ordinal] ? 1 : 0;
            break;
          }
          goto default;
        default:
          num = 0;
          break;
      }
      bool isModified = num != 0;
      if (member.CheckIsNotNull && record.IsDBNull(ordinal))
        throw EntityUtil.Update(Strings.Update_NullValue((object) record.GetName(ordinal)), (Exception) null, stateEntry);
      object obj = record.GetValue(ordinal);
      EntityKey entityKey = obj as EntityKey;
      if ((object) entityKey != null)
        return this.CreateEntityKeyResult(stateEntry, entityKey);
      IExtendedDataRecord record1 = obj as IExtendedDataRecord;
      if (record1 == null)
        return this.CreateSimpleResult(stateEntry, record, member, identifier, isModified, ordinal, obj);
      ModifiedPropertiesBehavior modifiedPropertiesBehavior1 = isModified ? ModifiedPropertiesBehavior.AllModified : ModifiedPropertiesBehavior.NoneModified;
      UpdateTranslator translator = this.m_translator;
      return ExtractorMetadata.ExtractResultFromRecord(stateEntry, isModified, record1, useCurrentValues, translator, modifiedPropertiesBehavior1);
    }

    private PropagatorResult CreateEntityKeyResult(
      IEntityStateEntry stateEntry,
      EntityKey entityKey)
    {
      RowType keyRowType = entityKey.GetEntitySet(this.m_translator.MetadataWorkspace).ElementType.GetKeyRowType();
      ExtractorMetadata extractorMetadata = this.m_translator.GetExtractorMetadata(stateEntry.EntitySet, (StructuralType) keyRowType);
      PropagatorResult[] values = new PropagatorResult[keyRowType.Properties.Count];
      for (int memberOffset = 0; memberOffset < keyRowType.Properties.Count; ++memberOffset)
      {
        EdmMember property = (EdmMember) keyRowType.Properties[memberOffset];
        ExtractorMetadata.MemberInformation member = extractorMetadata.m_memberMap[memberOffset];
        int identifierForMemberOffset = this.m_translator.KeyManager.GetKeyIdentifierForMemberOffset(entityKey, memberOffset, keyRowType.Properties.Count);
        object obj = !entityKey.IsTemporary ? entityKey.FindValueByName(property.Name) : stateEntry.StateManager.GetEntityStateEntry(entityKey).CurrentValues[property.Name];
        values[memberOffset] = PropagatorResult.CreateKeyValue(member.Flags, obj, stateEntry, identifierForMemberOffset);
      }
      return PropagatorResult.CreateStructuralValue(values, extractorMetadata.m_type, false);
    }

    private PropagatorResult CreateSimpleResult(
      IEntityStateEntry stateEntry,
      IExtendedDataRecord record,
      ExtractorMetadata.MemberInformation memberInformation,
      int identifier,
      bool isModified,
      int recordOrdinal,
      object value)
    {
      CurrentValueRecord record1 = record as CurrentValueRecord;
      PropagatorFlags flags = memberInformation.Flags;
      if (!isModified)
        flags |= PropagatorFlags.Preserve;
      if (-1 != identifier)
      {
        PropagatorResult owner = !memberInformation.IsServerGenerated && !memberInformation.IsForeignKeyMember || record1 == null ? PropagatorResult.CreateKeyValue(flags, value, stateEntry, identifier) : PropagatorResult.CreateServerGenKeyValue(flags, value, stateEntry, identifier, recordOrdinal);
        this.m_translator.KeyManager.RegisterIdentifierOwner(owner);
        return owner;
      }
      if ((memberInformation.IsServerGenerated || memberInformation.IsForeignKeyMember) && record1 != null)
        return PropagatorResult.CreateServerGenSimpleValue(flags, value, record1, recordOrdinal);
      return PropagatorResult.CreateSimpleValue(flags, value);
    }

    internal static PropagatorResult ExtractResultFromRecord(
      IEntityStateEntry stateEntry,
      bool isModified,
      IExtendedDataRecord record,
      bool useCurrentValues,
      UpdateTranslator translator,
      ModifiedPropertiesBehavior modifiedPropertiesBehavior)
    {
      StructuralType edmType = (StructuralType) record.DataRecordInfo.RecordType.EdmType;
      ExtractorMetadata extractorMetadata = translator.GetExtractorMetadata(stateEntry.EntitySet, edmType);
      EntityKey entityKey = stateEntry.EntityKey;
      PropagatorResult[] values = new PropagatorResult[record.FieldCount];
      for (int ordinal = 0; ordinal < values.Length; ++ordinal)
        values[ordinal] = extractorMetadata.RetrieveMember(stateEntry, record, useCurrentValues, entityKey, ordinal, modifiedPropertiesBehavior);
      return PropagatorResult.CreateStructuralValue(values, edmType, isModified);
    }

    private class MemberInformation
    {
      internal readonly int Ordinal;
      internal readonly int? EntityKeyOrdinal;
      internal readonly PropagatorFlags Flags;
      internal readonly bool IsServerGenerated;
      internal readonly bool CheckIsNotNull;
      [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
      internal readonly EdmMember Member;

      internal bool IsKeyMember
      {
        get
        {
          return PropagatorFlags.Key == (this.Flags & PropagatorFlags.Key);
        }
      }

      internal bool IsForeignKeyMember
      {
        get
        {
          return PropagatorFlags.ForeignKey == (this.Flags & PropagatorFlags.ForeignKey);
        }
      }

      internal MemberInformation(
        int ordinal,
        int? entityKeyOrdinal,
        PropagatorFlags flags,
        EdmMember member,
        bool isServerGenerated,
        bool isNullConditionMember)
      {
        this.Ordinal = ordinal;
        this.EntityKeyOrdinal = entityKeyOrdinal;
        this.Flags = flags;
        this.Member = member;
        this.IsServerGenerated = isServerGenerated;
        this.CheckIsNotNull = !TypeSemantics.IsNullable(member) && (isNullConditionMember || member.TypeUsage.EdmType.BuiltInTypeKind == BuiltInTypeKind.ComplexType);
      }
    }
  }
}
