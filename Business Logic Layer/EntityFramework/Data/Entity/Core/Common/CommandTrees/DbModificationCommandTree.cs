// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.DbModificationCommandTree
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.CommandTrees.Internal;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;

namespace System.Data.Entity.Core.Common.CommandTrees
{
  /// <summary>Represents a data manipulation language (DML) operation expressed as a command tree.</summary>
  public abstract class DbModificationCommandTree : DbCommandTree
  {
    private readonly DbExpressionBinding _target;
    private ReadOnlyCollection<DbParameterReferenceExpression> _parameters;

    internal DbModificationCommandTree()
    {
    }

    internal DbModificationCommandTree(
      MetadataWorkspace metadata,
      DataSpace dataSpace,
      DbExpressionBinding target)
      : base(metadata, dataSpace, true)
    {
      this._target = target;
    }

    /// <summary>
    /// Gets the <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionBinding" /> that specifies the target table for the data manipulation language (DML) operation.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionBinding" /> that specifies the target table for the DML operation.
    /// </returns>
    public DbExpressionBinding Target
    {
      get
      {
        return this._target;
      }
    }

    internal abstract bool HasReader { get; }

    internal override IEnumerable<KeyValuePair<string, TypeUsage>> GetParameters()
    {
      if (this._parameters == null)
        this._parameters = ParameterRetriever.GetParameters((DbCommandTree) this);
      return this._parameters.Select<DbParameterReferenceExpression, KeyValuePair<string, TypeUsage>>((Func<DbParameterReferenceExpression, KeyValuePair<string, TypeUsage>>) (p => new KeyValuePair<string, TypeUsage>(p.ParameterName, p.ResultType)));
    }

    internal override void DumpStructure(ExpressionDumper dumper)
    {
      if (this.Target == null)
        return;
      dumper.Dump(this.Target, "Target");
    }
  }
}
