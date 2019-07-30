// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ObjectTypeMapping
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.Mapping
{
  internal class ObjectTypeMapping : MappingBase
  {
    private static readonly Dictionary<string, ObjectMemberMapping> EmptyMemberMapping = new Dictionary<string, ObjectMemberMapping>(0);
    private readonly EdmType m_clrType;
    private readonly EdmType m_cdmType;
    private readonly string identity;
    private readonly Dictionary<string, ObjectMemberMapping> m_memberMapping;

    internal ObjectTypeMapping(EdmType clrType, EdmType cdmType)
    {
      this.m_clrType = clrType;
      this.m_cdmType = cdmType;
      this.identity = clrType.Identity + (object) ':' + cdmType.Identity;
      if (Helper.IsStructuralType(cdmType))
        this.m_memberMapping = new Dictionary<string, ObjectMemberMapping>(((StructuralType) cdmType).Members.Count);
      else
        this.m_memberMapping = ObjectTypeMapping.EmptyMemberMapping;
    }

    public override BuiltInTypeKind BuiltInTypeKind
    {
      get
      {
        return BuiltInTypeKind.MetadataItem;
      }
    }

    internal EdmType ClrType
    {
      get
      {
        return this.m_clrType;
      }
    }

    internal override MetadataItem EdmItem
    {
      get
      {
        return (MetadataItem) this.EdmType;
      }
    }

    internal EdmType EdmType
    {
      get
      {
        return this.m_cdmType;
      }
    }

    internal override string Identity
    {
      get
      {
        return this.identity;
      }
    }

    internal ObjectPropertyMapping GetPropertyMap(string propertyName)
    {
      ObjectMemberMapping memberMap = this.GetMemberMap(propertyName, false);
      if (memberMap != null && memberMap.MemberMappingKind == MemberMappingKind.ScalarPropertyMapping || memberMap.MemberMappingKind == MemberMappingKind.ComplexPropertyMapping)
        return (ObjectPropertyMapping) memberMap;
      return (ObjectPropertyMapping) null;
    }

    internal void AddMemberMap(ObjectMemberMapping memberMapping)
    {
      this.m_memberMapping.Add(memberMapping.EdmMember.Name, memberMapping);
    }

    internal ObjectMemberMapping GetMemberMapForClrMember(
      string clrMemberName,
      bool ignoreCase)
    {
      return this.GetMemberMap(clrMemberName, ignoreCase);
    }

    private ObjectMemberMapping GetMemberMap(
      string propertyName,
      bool ignoreCase)
    {
      Check.NotEmpty(propertyName, nameof (propertyName));
      ObjectMemberMapping objectMemberMapping = (ObjectMemberMapping) null;
      if (!ignoreCase)
      {
        this.m_memberMapping.TryGetValue(propertyName, out objectMemberMapping);
      }
      else
      {
        foreach (KeyValuePair<string, ObjectMemberMapping> keyValuePair in this.m_memberMapping)
        {
          if (keyValuePair.Key.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
          {
            if (objectMemberMapping != null)
              throw new MappingException(Strings.Mapping_Duplicate_PropertyMap_CaseInsensitive((object) propertyName));
            objectMemberMapping = keyValuePair.Value;
          }
        }
      }
      return objectMemberMapping;
    }

    public override string ToString()
    {
      return this.Identity;
    }
  }
}
