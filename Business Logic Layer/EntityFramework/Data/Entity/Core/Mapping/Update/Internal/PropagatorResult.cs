// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.Update.Internal.PropagatorResult
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Globalization;
using System.Linq;

namespace System.Data.Entity.Core.Mapping.Update.Internal
{
  internal abstract class PropagatorResult
  {
    internal const int NullIdentifier = -1;
    internal const int NullOrdinal = -1;

    internal abstract bool IsNull { get; }

    internal abstract bool IsSimple { get; }

    internal virtual PropagatorFlags PropagatorFlags
    {
      get
      {
        return PropagatorFlags.NoFlags;
      }
    }

    internal virtual IEntityStateEntry StateEntry
    {
      get
      {
        return (IEntityStateEntry) null;
      }
    }

    internal virtual CurrentValueRecord Record
    {
      get
      {
        return (CurrentValueRecord) null;
      }
    }

    internal virtual StructuralType StructuralType
    {
      get
      {
        return (StructuralType) null;
      }
    }

    internal virtual int RecordOrdinal
    {
      get
      {
        return -1;
      }
    }

    internal virtual int Identifier
    {
      get
      {
        return -1;
      }
    }

    internal virtual PropagatorResult Next
    {
      get
      {
        return (PropagatorResult) null;
      }
    }

    internal virtual object GetSimpleValue()
    {
      throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.UpdatePipelineResultRequestInvalid, 0, (object) "PropagatorResult.GetSimpleValue");
    }

    internal virtual PropagatorResult GetMemberValue(int ordinal)
    {
      throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.UpdatePipelineResultRequestInvalid, 0, (object) "PropagatorResult.GetMemberValue");
    }

    internal PropagatorResult GetMemberValue(EdmMember member)
    {
      return this.GetMemberValue(TypeHelpers.GetAllStructuralMembers((EdmType) this.StructuralType).IndexOf(member));
    }

    internal virtual PropagatorResult[] GetMemberValues()
    {
      throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.UpdatePipelineResultRequestInvalid, 0, (object) "PropagatorResult.GetMembersValues");
    }

    internal abstract PropagatorResult ReplicateResultWithNewFlags(
      PropagatorFlags flags);

    internal virtual PropagatorResult ReplicateResultWithNewValue(object value)
    {
      throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.UpdatePipelineResultRequestInvalid, 0, (object) "PropagatorResult.ReplicateResultWithNewValue");
    }

    internal abstract PropagatorResult Replace(
      Func<PropagatorResult, PropagatorResult> map);

    internal virtual PropagatorResult Merge(
      KeyManager keyManager,
      PropagatorResult other)
    {
      throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.UpdatePipelineResultRequestInvalid, 0, (object) "PropagatorResult.Merge");
    }

    internal virtual void SetServerGenValue(object value)
    {
      if (this.RecordOrdinal == -1)
        return;
      CurrentValueRecord record = this.Record;
      EdmMember fieldType = record.DataRecordInfo.FieldMetadata[this.RecordOrdinal].FieldType;
      value = value ?? (object) DBNull.Value;
      value = this.AlignReturnValue(value, fieldType);
      record.SetValue(this.RecordOrdinal, value);
    }

    internal object AlignReturnValue(object value, EdmMember member)
    {
      if (DBNull.Value.Equals(value))
      {
        if (BuiltInTypeKind.EdmProperty == member.BuiltInTypeKind && !((EdmProperty) member).Nullable)
          throw EntityUtil.Update(Strings.Update_NullReturnValueForNonNullableMember((object) member.Name, (object) member.DeclaringType.FullName), (Exception) null);
      }
      else if (!Helper.IsSpatialType(member.TypeUsage))
      {
        Type enumType = (Type) null;
        Type clrEquivalentType;
        if (Helper.IsEnumType(member.TypeUsage.EdmType))
        {
          PrimitiveType primitiveType = Helper.AsPrimitive(member.TypeUsage.EdmType);
          enumType = this.Record.GetFieldType(this.RecordOrdinal);
          clrEquivalentType = primitiveType.ClrEquivalentType;
        }
        else
          clrEquivalentType = ((PrimitiveType) member.TypeUsage.EdmType).ClrEquivalentType;
        try
        {
          value = Convert.ChangeType(value, clrEquivalentType, (IFormatProvider) CultureInfo.InvariantCulture);
          if (enumType != (Type) null)
            value = Enum.ToObject(enumType, value);
        }
        catch (Exception ex)
        {
          if (ex.RequiresContext())
          {
            Type type1 = enumType;
            if ((object) type1 == null)
              type1 = clrEquivalentType;
            Type type2 = type1;
            throw EntityUtil.Update(Strings.Update_ReturnValueHasUnexpectedType((object) value.GetType().FullName, (object) type2.FullName, (object) member.Name, (object) member.DeclaringType.FullName), ex);
          }
          throw;
        }
      }
      return value;
    }

    internal static PropagatorResult CreateSimpleValue(
      PropagatorFlags flags,
      object value)
    {
      return (PropagatorResult) new PropagatorResult.SimpleValue(flags, value);
    }

    internal static PropagatorResult CreateServerGenSimpleValue(
      PropagatorFlags flags,
      object value,
      CurrentValueRecord record,
      int recordOrdinal)
    {
      return (PropagatorResult) new PropagatorResult.ServerGenSimpleValue(flags, value, record, recordOrdinal);
    }

    internal static PropagatorResult CreateKeyValue(
      PropagatorFlags flags,
      object value,
      IEntityStateEntry stateEntry,
      int identifier)
    {
      return (PropagatorResult) new PropagatorResult.KeyValue(flags, value, stateEntry, identifier, (PropagatorResult.KeyValue) null);
    }

    internal static PropagatorResult CreateServerGenKeyValue(
      PropagatorFlags flags,
      object value,
      IEntityStateEntry stateEntry,
      int identifier,
      int recordOrdinal)
    {
      return (PropagatorResult) new PropagatorResult.ServerGenKeyValue(flags, value, stateEntry, identifier, recordOrdinal, (PropagatorResult.KeyValue) null);
    }

    internal static PropagatorResult CreateStructuralValue(
      PropagatorResult[] values,
      StructuralType structuralType,
      bool isModified)
    {
      if (isModified)
        return (PropagatorResult) new PropagatorResult.StructuralValue(values, structuralType);
      return (PropagatorResult) new PropagatorResult.UnmodifiedStructuralValue(values, structuralType);
    }

    private class SimpleValue : PropagatorResult
    {
      private readonly PropagatorFlags m_flags;
      protected readonly object m_value;

      internal SimpleValue(PropagatorFlags flags, object value)
      {
        this.m_flags = flags;
        this.m_value = value ?? (object) DBNull.Value;
      }

      internal override PropagatorFlags PropagatorFlags
      {
        get
        {
          return this.m_flags;
        }
      }

      internal override bool IsSimple
      {
        get
        {
          return true;
        }
      }

      internal override bool IsNull
      {
        get
        {
          if (-1 == this.Identifier)
            return DBNull.Value == this.m_value;
          return false;
        }
      }

      internal override object GetSimpleValue()
      {
        return this.m_value;
      }

      internal override PropagatorResult ReplicateResultWithNewFlags(
        PropagatorFlags flags)
      {
        return (PropagatorResult) new PropagatorResult.SimpleValue(flags, this.m_value);
      }

      internal override PropagatorResult ReplicateResultWithNewValue(object value)
      {
        return (PropagatorResult) new PropagatorResult.SimpleValue(this.PropagatorFlags, value);
      }

      internal override PropagatorResult Replace(
        Func<PropagatorResult, PropagatorResult> map)
      {
        return map((PropagatorResult) this);
      }
    }

    private class ServerGenSimpleValue : PropagatorResult.SimpleValue
    {
      private readonly CurrentValueRecord m_record;
      private readonly int m_recordOrdinal;

      internal ServerGenSimpleValue(
        PropagatorFlags flags,
        object value,
        CurrentValueRecord record,
        int recordOrdinal)
        : base(flags, value)
      {
        this.m_record = record;
        this.m_recordOrdinal = recordOrdinal;
      }

      internal override CurrentValueRecord Record
      {
        get
        {
          return this.m_record;
        }
      }

      internal override int RecordOrdinal
      {
        get
        {
          return this.m_recordOrdinal;
        }
      }

      internal override PropagatorResult ReplicateResultWithNewFlags(
        PropagatorFlags flags)
      {
        return (PropagatorResult) new PropagatorResult.ServerGenSimpleValue(flags, this.m_value, this.Record, this.RecordOrdinal);
      }

      internal override PropagatorResult ReplicateResultWithNewValue(object value)
      {
        return (PropagatorResult) new PropagatorResult.ServerGenSimpleValue(this.PropagatorFlags, value, this.Record, this.RecordOrdinal);
      }
    }

    private class KeyValue : PropagatorResult.SimpleValue
    {
      private readonly IEntityStateEntry m_stateEntry;
      private readonly int m_identifier;
      protected readonly PropagatorResult.KeyValue m_next;

      internal KeyValue(
        PropagatorFlags flags,
        object value,
        IEntityStateEntry stateEntry,
        int identifier,
        PropagatorResult.KeyValue next)
        : base(flags, value)
      {
        this.m_stateEntry = stateEntry;
        this.m_identifier = identifier;
        this.m_next = next;
      }

      internal override IEntityStateEntry StateEntry
      {
        get
        {
          return this.m_stateEntry;
        }
      }

      internal override int Identifier
      {
        get
        {
          return this.m_identifier;
        }
      }

      internal override CurrentValueRecord Record
      {
        get
        {
          return this.m_stateEntry.CurrentValues;
        }
      }

      internal override PropagatorResult Next
      {
        get
        {
          return (PropagatorResult) this.m_next;
        }
      }

      internal override PropagatorResult ReplicateResultWithNewFlags(
        PropagatorFlags flags)
      {
        return (PropagatorResult) new PropagatorResult.KeyValue(flags, this.m_value, this.StateEntry, this.Identifier, this.m_next);
      }

      internal override PropagatorResult ReplicateResultWithNewValue(object value)
      {
        return (PropagatorResult) new PropagatorResult.KeyValue(this.PropagatorFlags, value, this.StateEntry, this.Identifier, this.m_next);
      }

      internal virtual PropagatorResult.KeyValue ReplicateResultWithNewNext(
        PropagatorResult.KeyValue next)
      {
        if (this.m_next != null)
          next = this.m_next.ReplicateResultWithNewNext(next);
        return new PropagatorResult.KeyValue(this.PropagatorFlags, this.m_value, this.m_stateEntry, this.m_identifier, next);
      }

      internal override PropagatorResult Merge(
        KeyManager keyManager,
        PropagatorResult other)
      {
        PropagatorResult.KeyValue next = other as PropagatorResult.KeyValue;
        if (next == null)
          EntityUtil.InternalError(EntityUtil.InternalErrorCode.UpdatePipelineResultRequestInvalid, 0, (object) "KeyValue.Merge");
        if (this.Identifier != next.Identifier)
        {
          if (keyManager.GetPrincipals(next.Identifier).Contains<int>(this.Identifier))
            return (PropagatorResult) this.ReplicateResultWithNewNext(next);
          return (PropagatorResult) next.ReplicateResultWithNewNext(this);
        }
        if (this.m_stateEntry == null || this.m_stateEntry.IsRelationship)
          return (PropagatorResult) next.ReplicateResultWithNewNext(this);
        return (PropagatorResult) this.ReplicateResultWithNewNext(next);
      }
    }

    private class ServerGenKeyValue : PropagatorResult.KeyValue
    {
      private readonly int m_recordOrdinal;

      internal ServerGenKeyValue(
        PropagatorFlags flags,
        object value,
        IEntityStateEntry stateEntry,
        int identifier,
        int recordOrdinal,
        PropagatorResult.KeyValue next)
        : base(flags, value, stateEntry, identifier, next)
      {
        this.m_recordOrdinal = recordOrdinal;
      }

      internal override int RecordOrdinal
      {
        get
        {
          return this.m_recordOrdinal;
        }
      }

      internal override PropagatorResult ReplicateResultWithNewFlags(
        PropagatorFlags flags)
      {
        return (PropagatorResult) new PropagatorResult.ServerGenKeyValue(flags, this.m_value, this.StateEntry, this.Identifier, this.RecordOrdinal, this.m_next);
      }

      internal override PropagatorResult ReplicateResultWithNewValue(object value)
      {
        return (PropagatorResult) new PropagatorResult.ServerGenKeyValue(this.PropagatorFlags, value, this.StateEntry, this.Identifier, this.RecordOrdinal, this.m_next);
      }

      internal override PropagatorResult.KeyValue ReplicateResultWithNewNext(
        PropagatorResult.KeyValue next)
      {
        if (this.m_next != null)
          next = this.m_next.ReplicateResultWithNewNext(next);
        return (PropagatorResult.KeyValue) new PropagatorResult.ServerGenKeyValue(this.PropagatorFlags, this.m_value, this.StateEntry, this.Identifier, this.RecordOrdinal, next);
      }
    }

    private class StructuralValue : PropagatorResult
    {
      private readonly PropagatorResult[] m_values;
      protected readonly StructuralType m_structuralType;

      internal StructuralValue(PropagatorResult[] values, StructuralType structuralType)
      {
        this.m_values = values;
        this.m_structuralType = structuralType;
      }

      internal override bool IsSimple
      {
        get
        {
          return false;
        }
      }

      internal override bool IsNull
      {
        get
        {
          return false;
        }
      }

      internal override StructuralType StructuralType
      {
        get
        {
          return this.m_structuralType;
        }
      }

      internal override PropagatorResult GetMemberValue(int ordinal)
      {
        return this.m_values[ordinal];
      }

      internal override PropagatorResult[] GetMemberValues()
      {
        return this.m_values;
      }

      internal override PropagatorResult ReplicateResultWithNewFlags(
        PropagatorFlags flags)
      {
        throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.UpdatePipelineResultRequestInvalid, 0, (object) "StructuralValue.ReplicateResultWithNewFlags");
      }

      internal override PropagatorResult Replace(
        Func<PropagatorResult, PropagatorResult> map)
      {
        PropagatorResult[] values = this.ReplaceValues(map);
        if (values != null)
          return (PropagatorResult) new PropagatorResult.StructuralValue(values, this.m_structuralType);
        return (PropagatorResult) this;
      }

      protected PropagatorResult[] ReplaceValues(
        Func<PropagatorResult, PropagatorResult> map)
      {
        PropagatorResult[] propagatorResultArray = new PropagatorResult[this.m_values.Length];
        bool flag = false;
        for (int index = 0; index < propagatorResultArray.Length; ++index)
        {
          PropagatorResult propagatorResult = this.m_values[index].Replace(map);
          if (!object.ReferenceEquals((object) propagatorResult, (object) this.m_values[index]))
            flag = true;
          propagatorResultArray[index] = propagatorResult;
        }
        if (!flag)
          return (PropagatorResult[]) null;
        return propagatorResultArray;
      }
    }

    private class UnmodifiedStructuralValue : PropagatorResult.StructuralValue
    {
      internal UnmodifiedStructuralValue(PropagatorResult[] values, StructuralType structuralType)
        : base(values, structuralType)
      {
      }

      internal override PropagatorFlags PropagatorFlags
      {
        get
        {
          return PropagatorFlags.Preserve;
        }
      }

      internal override PropagatorResult Replace(
        Func<PropagatorResult, PropagatorResult> map)
      {
        PropagatorResult[] values = this.ReplaceValues(map);
        if (values != null)
          return (PropagatorResult) new PropagatorResult.UnmodifiedStructuralValue(values, this.m_structuralType);
        return (PropagatorResult) this;
      }
    }
  }
}
