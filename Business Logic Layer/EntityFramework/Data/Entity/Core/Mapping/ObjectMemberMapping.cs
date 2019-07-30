// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ObjectMemberMapping
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.Core.Mapping
{
  internal abstract class ObjectMemberMapping
  {
    private readonly EdmMember m_edmMember;
    private readonly EdmMember m_clrMember;

    protected ObjectMemberMapping(EdmMember edmMember, EdmMember clrMember)
    {
      this.m_edmMember = edmMember;
      this.m_clrMember = clrMember;
    }

    internal EdmMember EdmMember
    {
      get
      {
        return this.m_edmMember;
      }
    }

    internal EdmMember ClrMember
    {
      get
      {
        return this.m_clrMember;
      }
    }

    internal abstract MemberMappingKind MemberMappingKind { get; }
  }
}
