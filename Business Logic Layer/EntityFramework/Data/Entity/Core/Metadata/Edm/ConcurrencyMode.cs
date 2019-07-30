// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.ConcurrencyMode
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>The concurrency mode for properties.</summary>
  public enum ConcurrencyMode
  {
    /// <summary>
    /// Default concurrency mode: the property is never validated
    /// at write time
    /// </summary>
    None,
    /// <summary>
    /// Fixed concurrency mode: the property is always validated at
    /// write time
    /// </summary>
    Fixed,
  }
}
