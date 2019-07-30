// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.ExtentPair
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal class ExtentPair
  {
    private readonly EntitySetBase m_left;
    private readonly EntitySetBase m_right;

    internal EntitySetBase Left
    {
      get
      {
        return this.m_left;
      }
    }

    internal EntitySetBase Right
    {
      get
      {
        return this.m_right;
      }
    }

    public override bool Equals(object obj)
    {
      ExtentPair extentPair = obj as ExtentPair;
      if (extentPair != null && extentPair.Left.Equals((object) this.Left))
        return extentPair.Right.Equals((object) this.Right);
      return false;
    }

    public override int GetHashCode()
    {
      return this.Left.GetHashCode() << 4 ^ this.Right.GetHashCode();
    }

    internal ExtentPair(EntitySetBase left, EntitySetBase right)
    {
      this.m_left = left;
      this.m_right = right;
    }
  }
}
