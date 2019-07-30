// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.RelPropertyRef
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Query.InternalTrees;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal class RelPropertyRef : PropertyRef
  {
    private readonly RelProperty m_property;

    internal RelPropertyRef(RelProperty property)
    {
      this.m_property = property;
    }

    internal RelProperty Property
    {
      get
      {
        return this.m_property;
      }
    }

    public override bool Equals(object obj)
    {
      RelPropertyRef relPropertyRef = obj as RelPropertyRef;
      if (relPropertyRef != null)
        return this.m_property.Equals((object) relPropertyRef.m_property);
      return false;
    }

    public override int GetHashCode()
    {
      return this.m_property.GetHashCode();
    }

    public override string ToString()
    {
      return this.m_property.ToString();
    }
  }
}
