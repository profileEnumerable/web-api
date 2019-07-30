// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.ColumnMD
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal class ColumnMD
  {
    private readonly string m_name;
    private readonly TypeUsage m_type;
    private readonly EdmMember m_property;

    internal ColumnMD(string name, TypeUsage type)
    {
      this.m_name = name;
      this.m_type = type;
    }

    internal ColumnMD(EdmMember property)
      : this(property.Name, property.TypeUsage)
    {
      this.m_property = property;
    }

    internal string Name
    {
      get
      {
        return this.m_name;
      }
    }

    internal TypeUsage Type
    {
      get
      {
        return this.m_type;
      }
    }

    internal bool IsNullable
    {
      get
      {
        if (this.m_property != null)
          return TypeSemantics.IsNullable(this.m_property);
        return true;
      }
    }

    public override string ToString()
    {
      return this.m_name;
    }
  }
}
