// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.DbDeleteCommandTree
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Common.CommandTrees.Internal;
using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.Core.Common.CommandTrees
{
  /// <summary>Represents a single row delete operation expressed as a command tree. This class cannot be inherited.  </summary>
  public sealed class DbDeleteCommandTree : DbModificationCommandTree
  {
    private readonly DbExpression _predicate;

    internal DbDeleteCommandTree()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbDeleteCommandTree" /> class.
    /// </summary>
    /// <param name="metadata">The model this command will operate on.</param>
    /// <param name="dataSpace">The data space.</param>
    /// <param name="target">The target table for the data manipulation language (DML) operation.</param>
    /// <param name="predicate">A predicate used to determine which members of the target collection should be deleted.</param>
    public DbDeleteCommandTree(
      MetadataWorkspace metadata,
      DataSpace dataSpace,
      DbExpressionBinding target,
      DbExpression predicate)
      : base(metadata, dataSpace, target)
    {
      this._predicate = predicate;
    }

    /// <summary>
    /// Gets an <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that specifies the predicate used to determine which members of the target collection should be deleted.
    /// </summary>
    /// <remarks>
    /// The predicate can include only the following elements:
    /// <list>
    ///     <item>Equality expression</item>
    ///     <item>Constant expression</item>
    ///     <item>IsNull expression</item>
    ///     <item>Property expression</item>
    ///     <item>Reference expression to the target</item>
    ///     <item>And expression</item>
    ///     <item>Or expression</item>
    ///     <item>Not expression</item>
    /// </list>
    /// </remarks>
    /// <returns>
    /// An <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that specifies the predicate used to determine which members of the target collection should be deleted.
    /// </returns>
    public DbExpression Predicate
    {
      get
      {
        return this._predicate;
      }
    }

    /// <summary>Gets the kind of this command tree.</summary>
    /// <returns>The kind of this command tree.</returns>
    public override DbCommandTreeKind CommandTreeKind
    {
      get
      {
        return DbCommandTreeKind.Delete;
      }
    }

    internal override bool HasReader
    {
      get
      {
        return false;
      }
    }

    internal override void DumpStructure(ExpressionDumper dumper)
    {
      base.DumpStructure(dumper);
      if (this.Predicate == null)
        return;
      dumper.Dump(this.Predicate, "Predicate");
    }

    internal override string PrintTree(ExpressionPrinter printer)
    {
      return printer.Print(this);
    }
  }
}
