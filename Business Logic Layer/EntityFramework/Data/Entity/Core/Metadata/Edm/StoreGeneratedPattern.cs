// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.StoreGeneratedPattern
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>The pattern for Server Generated Properties.</summary>
  public enum StoreGeneratedPattern
  {
    /// <summary>Not a Server Generated Property. This is the default.</summary>
    None,
    /// <summary>
    /// A value is generated on INSERT, and remains unchanged on update.
    /// </summary>
    Identity,
    /// <summary>A value is generated on both INSERT and UPDATE.</summary>
    Computed,
  }
}
