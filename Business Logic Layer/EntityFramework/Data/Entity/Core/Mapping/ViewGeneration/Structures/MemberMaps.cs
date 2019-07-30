// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Structures.MemberMaps
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Structures
{
  internal class MemberMaps
  {
    private readonly MemberProjectionIndex m_projectedSlotMap;
    private readonly MemberDomainMap m_queryDomainMap;
    private readonly MemberDomainMap m_updateDomainMap;
    private readonly ViewTarget m_viewTarget;

    internal MemberMaps(
      ViewTarget viewTarget,
      MemberProjectionIndex projectedSlotMap,
      MemberDomainMap queryDomainMap,
      MemberDomainMap updateDomainMap)
    {
      this.m_projectedSlotMap = projectedSlotMap;
      this.m_queryDomainMap = queryDomainMap;
      this.m_updateDomainMap = updateDomainMap;
      this.m_viewTarget = viewTarget;
    }

    internal MemberProjectionIndex ProjectedSlotMap
    {
      get
      {
        return this.m_projectedSlotMap;
      }
    }

    internal MemberDomainMap QueryDomainMap
    {
      get
      {
        return this.m_queryDomainMap;
      }
    }

    internal MemberDomainMap UpdateDomainMap
    {
      get
      {
        return this.m_updateDomainMap;
      }
    }

    internal MemberDomainMap RightDomainMap
    {
      get
      {
        if (this.m_viewTarget != ViewTarget.QueryView)
          return this.m_queryDomainMap;
        return this.m_updateDomainMap;
      }
    }

    internal MemberDomainMap LeftDomainMap
    {
      get
      {
        if (this.m_viewTarget != ViewTarget.QueryView)
          return this.m_updateDomainMap;
        return this.m_queryDomainMap;
      }
    }
  }
}
