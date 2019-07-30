// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.NestedPropertyRef
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal class NestedPropertyRef : PropertyRef
  {
    private readonly PropertyRef m_inner;
    private readonly PropertyRef m_outer;

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "NestedPropertyRef")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "innerProperty")]
    internal NestedPropertyRef(PropertyRef innerProperty, PropertyRef outerProperty)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(!(innerProperty is NestedPropertyRef), "innerProperty cannot be a NestedPropertyRef");
      this.m_inner = innerProperty;
      this.m_outer = outerProperty;
    }

    internal PropertyRef OuterProperty
    {
      get
      {
        return this.m_outer;
      }
    }

    internal PropertyRef InnerProperty
    {
      get
      {
        return this.m_inner;
      }
    }

    public override bool Equals(object obj)
    {
      NestedPropertyRef nestedPropertyRef = obj as NestedPropertyRef;
      if (nestedPropertyRef != null && this.m_inner.Equals((object) nestedPropertyRef.m_inner))
        return this.m_outer.Equals((object) nestedPropertyRef.m_outer);
      return false;
    }

    public override int GetHashCode()
    {
      return this.m_inner.GetHashCode() ^ this.m_outer.GetHashCode();
    }

    public override string ToString()
    {
      return this.m_inner.ToString() + "." + (object) this.m_outer;
    }
  }
}
