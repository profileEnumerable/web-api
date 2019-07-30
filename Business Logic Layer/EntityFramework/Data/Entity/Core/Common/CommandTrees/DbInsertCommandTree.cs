// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.DbInsertCommandTree
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.CommandTrees.Internal;
using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.Core.Common.CommandTrees
{
  /// <summary>Represents a single row insert operation expressed as a command tree. This class cannot be inherited.</summary>
  /// <remarks>
  /// Represents a single row insert operation expressed as a canonical command tree.
  /// When the <see cref="P:System.Data.Entity.Core.Common.CommandTrees.DbInsertCommandTree.Returning" /> property is set, the command returns a reader; otherwise,
  /// it returns a scalar value indicating the number of rows affected.
  /// </remarks>
  public sealed class DbInsertCommandTree : DbModificationCommandTree
  {
    private readonly ReadOnlyCollection<DbModificationClause> _setClauses;
    private readonly DbExpression _returning;

    internal DbInsertCommandTree()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbInsertCommandTree" /> class.
    /// </summary>
    /// <param name="metadata">The model this command will operate on.</param>
    /// <param name="dataSpace">The data space.</param>
    /// <param name="target">The target table for the data manipulation language (DML) operation.</param>
    /// <param name="setClauses">The list of insert set clauses that define the insert operation. .</param>
    /// <param name="returning">A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that specifies a projection of results to be returned, based on the modified rows.</param>
    public DbInsertCommandTree(
      MetadataWorkspace metadata,
      DataSpace dataSpace,
      DbExpressionBinding target,
      ReadOnlyCollection<DbModificationClause> setClauses,
      DbExpression returning)
      : base(metadata, dataSpace, target)
    {
      this._setClauses = setClauses;
      this._returning = returning;
    }

    /// <summary>Gets the list of insert set clauses that define the insert operation. </summary>
    /// <returns>The list of insert set clauses that define the insert operation. </returns>
    public IList<DbModificationClause> SetClauses
    {
      get
      {
        return (IList<DbModificationClause>) this._setClauses;
      }
    }

    /// <summary>
    /// Gets an <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that specifies a projection of results to be returned based on the modified rows.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that specifies a projection of results to be returned based on the modified rows. null indicates that no results should be returned from this command.
    /// </returns>
    public DbExpression Returning
    {
      get
      {
        return this._returning;
      }
    }

    /// <summary>Gets the command tree kind.</summary>
    /// <returns>The command tree kind.</returns>
    public override DbCommandTreeKind CommandTreeKind
    {
      get
      {
        return DbCommandTreeKind.Insert;
      }
    }

    internal override bool HasReader
    {
      get
      {
        return null != this.Returning;
      }
    }

    internal override void DumpStructure(ExpressionDumper dumper)
    {
      base.DumpStructure(dumper);
      dumper.Begin("SetClauses");
      foreach (DbModificationClause setClause in (IEnumerable<DbModificationClause>) this.SetClauses)
        setClause?.DumpStructure(dumper);
      dumper.End("SetClauses");
      if (this.Returning == null)
        return;
      dumper.Dump(this.Returning, "Returning");
    }

    internal override string PrintTree(ExpressionPrinter printer)
    {
      return printer.Print(this);
    }
  }
}
