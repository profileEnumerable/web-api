// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.RootTypeInfo
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Query.InternalTrees;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal class RootTypeInfo : TypeInfo
  {
    private readonly List<PropertyRef> m_propertyRefList;
    private readonly Dictionary<PropertyRef, EdmProperty> m_propertyMap;
    private EdmProperty m_nullSentinelProperty;
    private EdmProperty m_typeIdProperty;
    private readonly ExplicitDiscriminatorMap m_discriminatorMap;
    private EdmProperty m_entitySetIdProperty;
    private RowType m_flattenedType;
    private TypeUsage m_flattenedTypeUsage;

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    internal RootTypeInfo(TypeUsage type, ExplicitDiscriminatorMap discriminatorMap)
      : base(type, (TypeInfo) null)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(type.EdmType.BaseType == null, "only root types allowed here");
      this.m_propertyMap = new Dictionary<PropertyRef, EdmProperty>();
      this.m_propertyRefList = new List<PropertyRef>();
      this.m_discriminatorMap = discriminatorMap;
      this.TypeIdKind = TypeIdKind.Generated;
    }

    internal TypeIdKind TypeIdKind { get; set; }

    internal TypeUsage TypeIdType { get; set; }

    internal void AddPropertyMapping(PropertyRef propertyRef, EdmProperty newProperty)
    {
      this.m_propertyMap[propertyRef] = newProperty;
      if (propertyRef is TypeIdPropertyRef)
        this.m_typeIdProperty = newProperty;
      else if (propertyRef is EntitySetIdPropertyRef)
      {
        this.m_entitySetIdProperty = newProperty;
      }
      else
      {
        if (!(propertyRef is NullSentinelPropertyRef))
          return;
        this.m_nullSentinelProperty = newProperty;
      }
    }

    internal void AddPropertyRef(PropertyRef propertyRef)
    {
      this.m_propertyRefList.Add(propertyRef);
    }

    internal new RowType FlattenedType
    {
      get
      {
        return this.m_flattenedType;
      }
      set
      {
        this.m_flattenedType = value;
        this.m_flattenedTypeUsage = TypeUsage.Create((EdmType) value);
      }
    }

    internal new TypeUsage FlattenedTypeUsage
    {
      get
      {
        return this.m_flattenedTypeUsage;
      }
    }

    internal ExplicitDiscriminatorMap DiscriminatorMap
    {
      get
      {
        return this.m_discriminatorMap;
      }
    }

    internal new EdmProperty EntitySetIdProperty
    {
      get
      {
        return this.m_entitySetIdProperty;
      }
    }

    internal new EdmProperty NullSentinelProperty
    {
      get
      {
        return this.m_nullSentinelProperty;
      }
    }

    internal new IEnumerable<PropertyRef> PropertyRefList
    {
      get
      {
        return (IEnumerable<PropertyRef>) this.m_propertyRefList;
      }
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "TypeInfo")]
    internal int GetNestedStructureOffset(PropertyRef property)
    {
      for (int index = 0; index < this.m_propertyRefList.Count; ++index)
      {
        NestedPropertyRef propertyRef = this.m_propertyRefList[index] as NestedPropertyRef;
        if (propertyRef != null && propertyRef.InnerProperty.Equals((object) property))
          return index;
      }
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(false, "no complex structure " + (object) property + " found in TypeInfo");
      return 0;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    internal new bool TryGetNewProperty(
      PropertyRef propertyRef,
      bool throwIfMissing,
      out EdmProperty property)
    {
      bool flag = this.m_propertyMap.TryGetValue(propertyRef, out property);
      if (throwIfMissing && !flag)
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(false, "Unable to find property " + (object) propertyRef + " in type " + this.Type.EdmType.Identity);
      return flag;
    }

    internal new EdmProperty TypeIdProperty
    {
      get
      {
        return this.m_typeIdProperty;
      }
    }
  }
}
