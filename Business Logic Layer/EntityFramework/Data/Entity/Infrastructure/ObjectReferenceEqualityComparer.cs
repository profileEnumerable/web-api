// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.ObjectReferenceEqualityComparer
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>Compares objects using reference equality.</summary>
  [Serializable]
  public sealed class ObjectReferenceEqualityComparer : IEqualityComparer<object>
  {
    private static readonly ObjectReferenceEqualityComparer _default = new ObjectReferenceEqualityComparer();

    /// <summary>Gets the default instance.</summary>
    public static ObjectReferenceEqualityComparer Default
    {
      get
      {
        return ObjectReferenceEqualityComparer._default;
      }
    }

    bool IEqualityComparer<object>.Equals(object x, object y)
    {
      return object.ReferenceEquals(x, y);
    }

    int IEqualityComparer<object>.GetHashCode(object obj)
    {
      return RuntimeHelpers.GetHashCode(obj);
    }
  }
}
