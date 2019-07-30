// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.DbCommandTree
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.Internal;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Common.CommandTrees
{
  /// <summary>An immutable class that implements the basic functionality for the Query, Insert, Update, Delete, and function invocation command tree types. </summary>
  public abstract class DbCommandTree
  {
    private readonly MetadataWorkspace _metadata;
    private readonly DataSpace _dataSpace;
    private readonly bool _useDatabaseNullSemantics;

    internal DbCommandTree()
    {
      this._useDatabaseNullSemantics = true;
    }

    internal DbCommandTree(
      MetadataWorkspace metadata,
      DataSpace dataSpace,
      bool useDatabaseNullSemantics = true)
    {
      if (!DbCommandTree.IsValidDataSpace(dataSpace))
        throw new ArgumentException(Strings.Cqt_CommandTree_InvalidDataSpace, nameof (dataSpace));
      this._metadata = metadata;
      this._dataSpace = dataSpace;
      this._useDatabaseNullSemantics = useDatabaseNullSemantics;
    }

    /// <summary>
    /// Gets a value indicating whether database null semantics are exhibited when comparing
    /// two operands, both of which are potentially nullable. The default value is true.
    /// 
    /// For example (operand1 == operand2) will be translated as:
    /// 
    /// (operand1 = operand2)
    /// 
    /// if UseDatabaseNullSemantics is true, respectively
    /// 
    /// (((operand1 = operand2) AND (NOT (operand1 IS NULL OR operand2 IS NULL))) OR ((operand1 IS NULL) AND (operand2 IS NULL)))
    /// 
    /// if UseDatabaseNullSemantics is false.
    /// </summary>
    /// <value>
    /// <c>true</c> if database null comparison behavior is enabled, otherwise <c>false</c> .
    /// </value>
    public bool UseDatabaseNullSemantics
    {
      get
      {
        return this._useDatabaseNullSemantics;
      }
    }

    /// <summary>
    /// Gets the name and corresponding type of each parameter that can be referenced within this
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbCommandTree" />
    /// .
    /// </summary>
    /// <returns>
    /// The name and corresponding type of each parameter that can be referenced within this
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbCommandTree" />
    /// .
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public IEnumerable<KeyValuePair<string, TypeUsage>> Parameters
    {
      get
      {
        return this.GetParameters();
      }
    }

    /// <summary>Gets the kind of this command tree.</summary>
    public abstract DbCommandTreeKind CommandTreeKind { get; }

    internal abstract IEnumerable<KeyValuePair<string, TypeUsage>> GetParameters();

    /// <summary>
    /// Gets the metadata workspace used by this command tree.
    /// </summary>
    public virtual MetadataWorkspace MetadataWorkspace
    {
      get
      {
        return this._metadata;
      }
    }

    /// <summary>
    /// Gets the data space in which metadata used by this command tree must reside.
    /// </summary>
    public virtual DataSpace DataSpace
    {
      get
      {
        return this._dataSpace;
      }
    }

    internal void Dump(ExpressionDumper dumper)
    {
      dumper.Begin(this.GetType().Name, new Dictionary<string, object>()
      {
        {
          "DataSpace",
          (object) this.DataSpace
        }
      });
      dumper.Begin("Parameters", (Dictionary<string, object>) null);
      foreach (KeyValuePair<string, TypeUsage> parameter in this.Parameters)
      {
        dumper.Begin("Parameter", new Dictionary<string, object>()
        {
          {
            "Name",
            (object) parameter.Key
          }
        });
        dumper.Dump(parameter.Value, "ParameterType");
        dumper.End("Parameter");
      }
      dumper.End("Parameters");
      this.DumpStructure(dumper);
      dumper.End(this.GetType().Name);
    }

    internal abstract void DumpStructure(ExpressionDumper dumper);

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents this command.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String" /> that represents this command.
    /// </returns>
    public override string ToString()
    {
      return this.Print();
    }

    internal string Print()
    {
      return this.PrintTree(new ExpressionPrinter());
    }

    internal abstract string PrintTree(ExpressionPrinter printer);

    internal static bool IsValidDataSpace(DataSpace dataSpace)
    {
      if (dataSpace != DataSpace.OSpace && DataSpace.CSpace != dataSpace)
        return DataSpace.SSpace == dataSpace;
      return true;
    }

    internal static bool IsValidParameterName(string name)
    {
      if (!string.IsNullOrWhiteSpace(name))
        return name.IsValidUndottedName();
      return false;
    }
  }
}
