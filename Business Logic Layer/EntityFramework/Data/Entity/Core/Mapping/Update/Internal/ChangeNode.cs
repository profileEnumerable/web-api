// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.Update.Internal.ChangeNode
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.Core.Mapping.Update.Internal
{
  internal class ChangeNode
  {
    private readonly List<PropagatorResult> m_inserted = new List<PropagatorResult>();
    private readonly List<PropagatorResult> m_deleted = new List<PropagatorResult>();
    private readonly TypeUsage m_elementType;

    internal ChangeNode(TypeUsage elementType)
    {
      this.m_elementType = elementType;
    }

    internal TypeUsage ElementType
    {
      get
      {
        return this.m_elementType;
      }
    }

    internal List<PropagatorResult> Inserted
    {
      get
      {
        return this.m_inserted;
      }
    }

    internal List<PropagatorResult> Deleted
    {
      get
      {
        return this.m_deleted;
      }
    }

    internal PropagatorResult Placeholder { get; set; }
  }
}
