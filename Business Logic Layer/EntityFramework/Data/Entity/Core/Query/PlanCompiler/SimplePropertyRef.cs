// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.SimplePropertyRef
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Query.InternalTrees;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal class SimplePropertyRef : PropertyRef
  {
    private readonly EdmMember m_property;

    internal SimplePropertyRef(EdmMember property)
    {
      this.m_property = property;
    }

    internal EdmMember Property
    {
      get
      {
        return this.m_property;
      }
    }

    public override bool Equals(object obj)
    {
      SimplePropertyRef simplePropertyRef = obj as SimplePropertyRef;
      if (simplePropertyRef != null && Command.EqualTypes((EdmType) this.m_property.DeclaringType, (EdmType) simplePropertyRef.m_property.DeclaringType))
        return simplePropertyRef.m_property.Name.Equals(this.m_property.Name);
      return false;
    }

    public override int GetHashCode()
    {
      return this.m_property.Name.GetHashCode();
    }

    public override string ToString()
    {
      return this.m_property.Name;
    }
  }
}
