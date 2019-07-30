// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.StateManagerTypeMetadata
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.Core.Objects
{
  internal class StateManagerTypeMetadata
  {
    private readonly TypeUsage _typeUsage;
    private readonly StateManagerMemberMetadata[] _members;
    private readonly Dictionary<string, int> _objectNameToOrdinal;
    private readonly Dictionary<string, int> _cLayerNameToOrdinal;
    private readonly DataRecordInfo _recordInfo;

    internal StateManagerTypeMetadata()
    {
    }

    internal StateManagerTypeMetadata(EdmType edmType, ObjectTypeMapping mapping)
    {
      this._typeUsage = TypeUsage.Create(edmType);
      this._recordInfo = new DataRecordInfo(this._typeUsage);
      ReadOnlyMetadataCollection<EdmProperty> properties = TypeHelpers.GetProperties(edmType);
      this._members = new StateManagerMemberMetadata[properties.Count];
      this._objectNameToOrdinal = new Dictionary<string, int>(properties.Count);
      this._cLayerNameToOrdinal = new Dictionary<string, int>(properties.Count);
      ReadOnlyMetadataCollection<EdmMember> metadataCollection = (ReadOnlyMetadataCollection<EdmMember>) null;
      if (Helper.IsEntityType(edmType))
        metadataCollection = ((EntityTypeBase) edmType).KeyMembers;
      for (int index = 0; index < this._members.Length; ++index)
      {
        EdmProperty memberMetadata = properties[index];
        ObjectPropertyMapping memberMap = (ObjectPropertyMapping) null;
        if (mapping != null)
        {
          memberMap = mapping.GetPropertyMap(memberMetadata.Name);
          if (memberMap != null)
            this._objectNameToOrdinal.Add(memberMap.ClrProperty.Name, index);
        }
        this._cLayerNameToOrdinal.Add(memberMetadata.Name, index);
        this._members[index] = new StateManagerMemberMetadata(memberMap, memberMetadata, metadataCollection != null && metadataCollection.Contains((EdmMember) memberMetadata));
      }
    }

    internal TypeUsage CdmMetadata
    {
      get
      {
        return this._typeUsage;
      }
    }

    internal DataRecordInfo DataRecordInfo
    {
      get
      {
        return this._recordInfo;
      }
    }

    internal virtual int FieldCount
    {
      get
      {
        return this._members.Length;
      }
    }

    internal Type GetFieldType(int ordinal)
    {
      return this.Member(ordinal).ClrType;
    }

    internal virtual StateManagerMemberMetadata Member(int ordinal)
    {
      if ((uint) ordinal < (uint) this._members.Length)
        return this._members[ordinal];
      throw new ArgumentOutOfRangeException(nameof (ordinal));
    }

    internal IEnumerable<StateManagerMemberMetadata> Members
    {
      get
      {
        return (IEnumerable<StateManagerMemberMetadata>) this._members;
      }
    }

    internal string CLayerMemberName(int ordinal)
    {
      return this.Member(ordinal).CLayerName;
    }

    internal int GetOrdinalforOLayerMemberName(string name)
    {
      int num;
      if (string.IsNullOrEmpty(name) || !this._objectNameToOrdinal.TryGetValue(name, out num))
        num = -1;
      return num;
    }

    internal int GetOrdinalforCLayerMemberName(string name)
    {
      int num;
      if (string.IsNullOrEmpty(name) || !this._cLayerNameToOrdinal.TryGetValue(name, out num))
        num = -1;
      return num;
    }
  }
}
