// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.IDbAsyncEnumerable
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// Asynchronous version of the <see cref="T:System.Collections.IEnumerable" /> interface that allows elements to be retrieved asynchronously.
  /// This interface is used to interact with Entity Framework queries and shouldn't be implemented by custom classes.
  /// </summary>
  public interface IDbAsyncEnumerable
  {
    /// <summary>
    /// Gets an enumerator that can be used to asynchronously enumerate the sequence.
    /// </summary>
    /// <returns> Enumerator for asynchronous enumeration over the sequence. </returns>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    IDbAsyncEnumerator GetAsyncEnumerator();
  }
}
