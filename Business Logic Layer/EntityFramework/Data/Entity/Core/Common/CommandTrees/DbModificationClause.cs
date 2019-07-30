// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.DbModificationClause
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Common.CommandTrees.Internal;
using System.Data.Entity.Core.Common.Utils;

namespace System.Data.Entity.Core.Common.CommandTrees
{
  /// <summary>
  /// Specifies a single clause in an insert or update modification operation, see
  /// <see cref="P:System.Data.Entity.Core.Common.CommandTrees.DbInsertCommandTree.SetClauses" /> and <see cref="P:System.Data.Entity.Core.Common.CommandTrees.DbUpdateCommandTree.SetClauses" />
  /// </summary>
  /// <remarks>
  /// An abstract base class allows the possibility of patterns other than
  /// Property = Value in future versions, e.g.,
  /// <code>update SomeTable
  ///     set ComplexTypeColumn.SomeProperty()
  ///     where Id = 2</code>
  /// </remarks>
  public abstract class DbModificationClause
  {
    internal DbModificationClause()
    {
    }

    internal abstract void DumpStructure(ExpressionDumper dumper);

    internal abstract TreeNode Print(DbExpressionVisitor<TreeNode> visitor);
  }
}
