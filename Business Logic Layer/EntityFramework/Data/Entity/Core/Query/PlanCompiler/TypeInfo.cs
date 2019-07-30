// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.TypeInfo
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Query.InternalTrees;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal class TypeInfo
  {
    private readonly TypeUsage m_type;
    private readonly List<TypeInfo> m_immediateSubTypes;
    private readonly TypeInfo m_superType;
    private readonly RootTypeInfo m_rootType;

    internal static TypeInfo Create(
      TypeUsage type,
      TypeInfo superTypeInfo,
      ExplicitDiscriminatorMap discriminatorMap)
    {
      return superTypeInfo != null ? new TypeInfo(type, superTypeInfo) : (TypeInfo) new RootTypeInfo(type, discriminatorMap);
    }

    protected TypeInfo(TypeUsage type, TypeInfo superType)
    {
      this.m_type = type;
      this.m_immediateSubTypes = new List<TypeInfo>();
      this.m_superType = superType;
      if (superType == null)
        return;
      superType.m_immediateSubTypes.Add(this);
      this.m_rootType = superType.RootType;
    }

    internal bool IsRootType
    {
      get
      {
        return this.m_rootType == null;
      }
    }

    internal List<TypeInfo> ImmediateSubTypes
    {
      get
      {
        return this.m_immediateSubTypes;
      }
    }

    internal TypeInfo SuperType
    {
      get
      {
        return this.m_superType;
      }
    }

    internal RootTypeInfo RootType
    {
      get
      {
        return this.m_rootType ?? (RootTypeInfo) this;
      }
    }

    internal TypeUsage Type
    {
      get
      {
        return this.m_type;
      }
    }

    internal object TypeId { get; set; }

    internal virtual RowType FlattenedType
    {
      get
      {
        return this.RootType.FlattenedType;
      }
    }

    internal virtual TypeUsage FlattenedTypeUsage
    {
      get
      {
        return this.RootType.FlattenedTypeUsage;
      }
    }

    internal virtual EdmProperty EntitySetIdProperty
    {
      get
      {
        return this.RootType.EntitySetIdProperty;
      }
    }

    internal bool HasEntitySetIdProperty
    {
      get
      {
        return this.RootType.EntitySetIdProperty != null;
      }
    }

    internal virtual EdmProperty NullSentinelProperty
    {
      get
      {
        return this.RootType.NullSentinelProperty;
      }
    }

    internal bool HasNullSentinelProperty
    {
      get
      {
        return this.RootType.NullSentinelProperty != null;
      }
    }

    internal virtual EdmProperty TypeIdProperty
    {
      get
      {
        return this.RootType.TypeIdProperty;
      }
    }

    internal bool HasTypeIdProperty
    {
      get
      {
        return this.RootType.TypeIdProperty != null;
      }
    }

    internal virtual IEnumerable<PropertyRef> PropertyRefList
    {
      get
      {
        return this.RootType.PropertyRefList;
      }
    }

    internal EdmProperty GetNewProperty(PropertyRef propertyRef)
    {
      EdmProperty newProperty;
      this.TryGetNewProperty(propertyRef, true, out newProperty);
      return newProperty;
    }

    internal bool TryGetNewProperty(
      PropertyRef propertyRef,
      bool throwIfMissing,
      out EdmProperty newProperty)
    {
      return this.RootType.TryGetNewProperty(propertyRef, throwIfMissing, out newProperty);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Non-EdmProperty")]
    internal IEnumerable<PropertyRef> GetKeyPropertyRefs()
    {
      EntityTypeBase entityType = (EntityTypeBase) null;
      RefType refType = (RefType) null;
      entityType = !TypeHelpers.TryGetEdmType<RefType>(this.m_type, out refType) ? TypeHelpers.GetEdmType<EntityTypeBase>(this.m_type) : refType.ElementType;
      foreach (EdmMember keyMember in entityType.KeyMembers)
      {
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(keyMember is EdmProperty, "Non-EdmProperty key members are not supported");
        SimplePropertyRef spr = new SimplePropertyRef(keyMember);
        yield return (PropertyRef) spr;
      }
    }

    internal IEnumerable<PropertyRef> GetIdentityPropertyRefs()
    {
      if (this.HasEntitySetIdProperty)
        yield return (PropertyRef) EntitySetIdPropertyRef.Instance;
      foreach (PropertyRef keyPropertyRef in this.GetKeyPropertyRefs())
        yield return keyPropertyRef;
    }

    internal IEnumerable<PropertyRef> GetAllPropertyRefs()
    {
      foreach (PropertyRef propertyRef in this.PropertyRefList)
        yield return propertyRef;
    }

    internal IEnumerable<EdmProperty> GetAllProperties()
    {
      foreach (EdmProperty property in this.FlattenedType.Properties)
        yield return property;
    }

    internal List<TypeInfo> GetTypeHierarchy()
    {
      List<TypeInfo> result = new List<TypeInfo>();
      this.GetTypeHierarchy(result);
      return result;
    }

    private void GetTypeHierarchy(List<TypeInfo> result)
    {
      result.Add(this);
      foreach (TypeInfo immediateSubType in this.ImmediateSubTypes)
        immediateSubType.GetTypeHierarchy(result);
    }
  }
}
