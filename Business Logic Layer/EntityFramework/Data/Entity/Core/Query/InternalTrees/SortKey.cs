// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.SortKey
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal class SortKey
  {
    private readonly bool m_asc;
    private readonly string m_collation;

    internal SortKey(Var v, bool asc, string collation)
    {
      this.Var = v;
      this.m_asc = asc;
      this.m_collation = collation;
    }

    internal Var Var { get; set; }

    internal bool AscendingSort
    {
      get
      {
        return this.m_asc;
      }
    }

    internal string Collation
    {
      get
      {
        return this.m_collation;
      }
    }
  }
}
