// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.NewEntityBaseOp
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal abstract class NewEntityBaseOp : ScalarOp
  {
    private readonly bool m_scoped;
    private readonly EntitySet m_entitySet;
    private readonly List<RelProperty> m_relProperties;

    internal NewEntityBaseOp(
      OpType opType,
      TypeUsage type,
      bool scoped,
      EntitySet entitySet,
      List<RelProperty> relProperties)
      : base(opType, type)
    {
      this.m_scoped = scoped;
      this.m_entitySet = entitySet;
      this.m_relProperties = relProperties;
    }

    protected NewEntityBaseOp(OpType opType)
      : base(opType)
    {
    }

    internal bool Scoped
    {
      get
      {
        return this.m_scoped;
      }
    }

    internal EntitySet EntitySet
    {
      get
      {
        return this.m_entitySet;
      }
    }

    internal List<RelProperty> RelationshipProperties
    {
      get
      {
        return this.m_relProperties;
      }
    }
  }
}
