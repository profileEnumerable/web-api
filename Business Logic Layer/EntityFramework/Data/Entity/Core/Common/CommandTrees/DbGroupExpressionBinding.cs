// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.DbGroupExpressionBinding
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.Core.Common.CommandTrees
{
  /// <summary>
  /// Defines the binding for the input set to a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbGroupByExpression" />.
  /// In addition to the properties of <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionBinding" />, DbGroupExpressionBinding
  /// also provides access to the group element via the <seealso cref="P:System.Data.Entity.Core.Common.CommandTrees.DbGroupExpressionBinding.GroupVariable" /> variable reference
  /// and to the group aggregate via the <seealso cref="P:System.Data.Entity.Core.Common.CommandTrees.DbGroupExpressionBinding.GroupAggregate" /> property.
  /// </summary>
  public sealed class DbGroupExpressionBinding
  {
    private readonly DbExpression _expr;
    private readonly DbVariableReferenceExpression _varRef;
    private readonly DbVariableReferenceExpression _groupVarRef;
    private DbGroupAggregate _groupAggregate;

    internal DbGroupExpressionBinding(
      DbExpression input,
      DbVariableReferenceExpression inputRef,
      DbVariableReferenceExpression groupRef)
    {
      this._expr = input;
      this._varRef = inputRef;
      this._groupVarRef = groupRef;
    }

    /// <summary>
    /// Gets the <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that defines the input set.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that defines the input set.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">The expression is null.</exception>
    /// <exception cref="T:System.ArgumentException">
    /// The expression is not associated with the command tree of the
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbGroupExpressionBinding" />
    /// , or its result type is not equal or promotable to the result type of the current value of the property.
    /// </exception>
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
    /// <returns>The type metadata of the element variable.</returns>
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
    /// <returns>A reference to the element variable.</returns>
    public DbVariableReferenceExpression Variable
    {
      get
      {
        return this._varRef;
      }
    }

    /// <summary>Gets the name assigned to the group element variable.</summary>
    /// <returns>The name assigned to the group element variable.</returns>
    public string GroupVariableName
    {
      get
      {
        return this._groupVarRef.VariableName;
      }
    }

    /// <summary>Gets the type metadata of the group element variable.</summary>
    /// <returns>The type metadata of the group element variable.</returns>
    public TypeUsage GroupVariableType
    {
      get
      {
        return this._groupVarRef.ResultType;
      }
    }

    /// <summary>
    /// Gets the <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbVariableReferenceExpression" /> that references the group element variable.
    /// </summary>
    /// <returns>A reference to the group element variable.</returns>
    public DbVariableReferenceExpression GroupVariable
    {
      get
      {
        return this._groupVarRef;
      }
    }

    /// <summary>
    /// Gets the <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbGroupAggregate" /> that represents the collection of elements in the group.
    /// </summary>
    /// <returns>The elements in the group.</returns>
    public DbGroupAggregate GroupAggregate
    {
      get
      {
        if (this._groupAggregate == null)
          this._groupAggregate = DbExpressionBuilder.GroupAggregate((DbExpression) this.GroupVariable);
        return this._groupAggregate;
      }
    }
  }
}
