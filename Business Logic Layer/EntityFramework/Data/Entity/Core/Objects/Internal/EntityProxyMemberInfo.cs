// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.Internal.EntityProxyMemberInfo
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.Core.Objects.Internal
{
  internal sealed class EntityProxyMemberInfo
  {
    private readonly EdmMember _member;
    private readonly int _propertyIndex;

    internal EntityProxyMemberInfo(EdmMember member, int propertyIndex)
    {
      this._member = member;
      this._propertyIndex = propertyIndex;
    }

    internal EdmMember EdmMember
    {
      get
      {
        return this._member;
      }
    }

    internal int PropertyIndex
    {
      get
      {
        return this._propertyIndex;
      }
    }
  }
}
