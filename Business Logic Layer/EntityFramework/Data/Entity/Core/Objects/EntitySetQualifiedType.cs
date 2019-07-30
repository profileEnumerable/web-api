// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.EntitySetQualifiedType
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Objects
{
  internal struct EntitySetQualifiedType : IEqualityComparer<EntitySetQualifiedType>
  {
    internal static readonly IEqualityComparer<EntitySetQualifiedType> EqualityComparer = (IEqualityComparer<EntitySetQualifiedType>) new EntitySetQualifiedType();
    internal readonly Type ClrType;
    internal readonly EntitySet EntitySet;

    internal EntitySetQualifiedType(Type type, EntitySet set)
    {
      this.ClrType = EntityUtil.GetEntityIdentityType(type);
      this.EntitySet = set;
    }

    public bool Equals(EntitySetQualifiedType x, EntitySetQualifiedType y)
    {
      if (object.ReferenceEquals((object) x.ClrType, (object) y.ClrType))
        return object.ReferenceEquals((object) x.EntitySet, (object) y.EntitySet);
      return false;
    }

    [SuppressMessage("Microsoft.Usage", "CA2303", Justification = "ClrType is not expected to be an Embedded Interop Type.")]
    public int GetHashCode(EntitySetQualifiedType obj)
    {
      return obj.ClrType.GetHashCode() + obj.EntitySet.Name.GetHashCode() + obj.EntitySet.EntityContainer.Name.GetHashCode();
    }
  }
}
