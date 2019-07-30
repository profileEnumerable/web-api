// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Utilities.DynamicEqualityComparer`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;

namespace System.Data.Entity.Utilities
{
  internal sealed class DynamicEqualityComparer<T> : IEqualityComparer<T> where T : class
  {
    private readonly Func<T, T, bool> _func;

    public DynamicEqualityComparer(Func<T, T, bool> func)
    {
      this._func = func;
    }

    public bool Equals(T x, T y)
    {
      return this._func(x, y);
    }

    public int GetHashCode(T obj)
    {
      return 0;
    }
  }
}
