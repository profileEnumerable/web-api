// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.IConvention
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  /// <summary>
  /// Identifies conventions that can be added to or removed from a <see cref="T:System.Data.Entity.DbModelBuilder" /> instance.
  /// </summary>
  /// <remarks>
  /// Note that implementations of this interface must be immutable.
  /// </remarks>
  [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces")]
  public interface IConvention
  {
  }
}
