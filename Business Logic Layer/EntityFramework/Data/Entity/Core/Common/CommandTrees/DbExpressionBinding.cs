// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.DbExpressionBinding
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.Core.Common.CommandTrees
{
  /// <summary>
  /// Describes a binding for an expression. Conceptually similar to a foreach loop
  /// in C#. The DbExpression property defines the collection being iterated over,
  /// while the Var property provides a means to reference the current element
  /// of the collection during the iteration. DbExpressionBinding is used to describe the set arguments
  /// to relational expressions such as <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFilterExpression" />, <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbProjectExpression" />
  /// and <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbJoinExpression" />.
  /// </summary>
  /// <seealso cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" />
  /// <seealso cref="P:System.Data.Entity.Core.Common.CommandTrees.DbExpressionBinding.Variable" />
  public sealed class DbExpressionBinding
  {
    private readonly DbExpression _expr;
    private readonly DbVariableReferenceExpression _varRef;

    internal DbExpressionBinding(DbExpression input, DbVariableReferenceExpression varRef)
    {
      this._expr = input;
      this._varRef = varRef;
    }

    /// <summary>
    /// Gets the <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that defines the input set.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that defines the input set.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">The expression is null.</exception>
    /// <exception cref="T:System.ArgumentException">The expression is not associated with the command tree of the binding, or its result type is not equal or promotable to the result type of the current value of the property.</exception>
    public DbExpression Expression
    {
      get
      {
        return this._expr;
      }
    }

    /// <summary>Gets the name assigned to the element variable.</summary>
    /// <returns>The name assigned to the element variable.</returns>
    public string VariableName
    {
      get
      {
        return this._varRef.VariableName;
      }
    }

    /// <summary>Gets the type metadata of the element variable.</summary>
    /// <returns>The type metadata of the element variable. </returns>
    public TypeUsage VariableType
    {
      get
      {
        return this._varRef.ResultType;
      }
    }

    /// <summary>
    /// Gets the <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbVariableReferenceExpression" /> that references the element variable.
    /// </summary>
    /// <returns>The variable reference.</returns>
    public DbVariableReferenceExpression Variable
    {
      get
      {
        return this._varRef;
      }
    }
  }
}
