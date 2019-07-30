// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.IObjectContextAdapter
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Objects;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// Interface implemented by objects that can provide an <see cref="P:System.Data.Entity.Infrastructure.IObjectContextAdapter.ObjectContext" /> instance.
  /// The <see cref="T:System.Data.Entity.DbContext" /> class implements this interface to provide access to the underlying
  /// ObjectContext.
  /// </summary>
  public interface IObjectContextAdapter
  {
    /// <summary>Gets the object context.</summary>
    /// <value> The object context. </value>
    ObjectContext ObjectContext { get; }
  }
}
