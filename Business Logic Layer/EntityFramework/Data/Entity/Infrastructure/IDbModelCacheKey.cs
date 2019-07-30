// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.IDbModelCacheKey
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// Represents a key value that uniquely identifies an Entity Framework model that has been loaded into memory.
  /// </summary>
  public interface IDbModelCacheKey
  {
    /// <summary>Determines whether the current cached model key is equal to the specified cached model key.</summary>
    /// <returns>true if the current cached model key is equal to the specified cached model key; otherwise, false.</returns>
    /// <param name="other">The cached model key to compare to the current cached model key. </param>
    bool Equals(object other);

    /// <summary>Returns the hash function for this cached model key.</summary>
    /// <returns>The hash function for this cached model key.</returns>
    int GetHashCode();
  }
}
