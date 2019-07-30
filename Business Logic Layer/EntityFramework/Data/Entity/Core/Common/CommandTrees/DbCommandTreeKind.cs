// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.DbCommandTreeKind
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Common.CommandTrees
{
  /// <summary>
  /// Describes the different "kinds" (classes) of command trees.
  /// </summary>
  public enum DbCommandTreeKind
  {
    /// <summary>A query to retrieve data</summary>
    Query,
    /// <summary>Update existing data</summary>
    Update,
    /// <summary>Insert new data</summary>
    Insert,
    /// <summary>Deleted existing data</summary>
    Delete,
    /// <summary>Call a function</summary>
    Function,
  }
}
