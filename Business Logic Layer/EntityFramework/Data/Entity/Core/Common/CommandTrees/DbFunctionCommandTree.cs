// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.DbFunctionCommandTree
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.CommandTrees.Internal;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Common.CommandTrees
{
  /// <summary>Represents the invocation of a database function. </summary>
  public sealed class DbFunctionCommandTree : DbCommandTree
  {
    private readonly EdmFunction _edmFunction;
    private readonly TypeUsage _resultType;
    private readonly ReadOnlyCollection<string> _parameterNames;
    private readonly ReadOnlyCollection<TypeUsage> _parameterTypes;

    /// <summary>
    /// Constructs a new DbFunctionCommandTree that uses the specified metadata workspace, data space and function metadata
    /// </summary>
    /// <param name="metadata"> The metadata workspace that the command tree should use. </param>
    /// <param name="dataSpace"> The logical 'space' that metadata in the expressions used in this command tree must belong to. </param>
    /// <param name="edmFunction">The <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmFunction" /> that represents the function that is being invoked.</param>
    /// <param name="resultType">The expected result type for the function’s first result set.</param>
    /// <param name="parameters">The function's parameters.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="metadata" />, <paramref name="dataSpace" /> or <paramref name="edmFunction" /> is null
    /// </exception>
    /// <exception cref="T:System.ArgumentException">
    /// <paramref name="dataSpace" /> does not represent a valid data space or <paramref name="edmFunction" />
    /// is a composable function
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public DbFunctionCommandTree(
      MetadataWorkspace metadata,
      DataSpace dataSpace,
      EdmFunction edmFunction,
      TypeUsage resultType,
      IEnumerable<KeyValuePair<string, TypeUsage>> parameters)
      : base(metadata, dataSpace, true)
    {
      Check.NotNull<EdmFunction>(edmFunction, nameof (edmFunction));
      this._edmFunction = edmFunction;
      this._resultType = resultType;
      List<string> stringList = new List<string>();
      List<TypeUsage> typeUsageList = new List<TypeUsage>();
      if (parameters != null)
      {
        foreach (KeyValuePair<string, TypeUsage> parameter in parameters)
        {
          stringList.Add(parameter.Key);
          typeUsageList.Add(parameter.Value);
        }
      }
      this._parameterNames = new ReadOnlyCollection<string>((IList<string>) stringList);
      this._parameterTypes = new ReadOnlyCollection<TypeUsage>((IList<TypeUsage>) typeUsageList);
    }

    /// <summary>
    /// Gets the <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmFunction" /> that represents the function that is being invoked.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmFunction" /> that represents the function that is being invoked.
    /// </returns>
    public EdmFunction EdmFunction
    {
      get
      {
        return this._edmFunction;
      }
    }

    /// <summary>Gets the expected result type for the function’s first result set.</summary>
    /// <returns>The expected result type for the function’s first result set.</returns>
    public TypeUsage ResultType
    {
      get
      {
        return this._resultType;
      }
    }

    /// <summary>Gets or sets the command tree kind.</summary>
    /// <returns>The command tree kind.</returns>
    public override DbCommandTreeKind CommandTreeKind
    {
      get
      {
        return DbCommandTreeKind.Function;
      }
    }

    internal override IEnumerable<KeyValuePair<string, TypeUsage>> GetParameters()
    {
      for (int idx = 0; idx < this._parameterNames.Count; ++idx)
        yield return new KeyValuePair<string, TypeUsage>(this._parameterNames[idx], this._parameterTypes[idx]);
    }

    internal override void DumpStructure(ExpressionDumper dumper)
    {
      if (this.EdmFunction == null)
        return;
      dumper.Dump(this.EdmFunction);
    }

    internal override string PrintTree(ExpressionPrinter printer)
    {
      return printer.Print(this);
    }
  }
}
