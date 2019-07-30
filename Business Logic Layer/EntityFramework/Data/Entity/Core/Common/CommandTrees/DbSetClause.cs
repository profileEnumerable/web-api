// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.DbSetClause
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Common.CommandTrees.Internal;
using System.Data.Entity.Core.Common.Utils;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Common.CommandTrees
{
  /// <summary>Specifies the clause in a modification operation that sets the value of a property. This class cannot be inherited. </summary>
  public sealed class DbSetClause : DbModificationClause
  {
    private readonly DbExpression _prop;
    private readonly DbExpression _val;

    internal DbSetClause(DbExpression targetProperty, DbExpression sourceValue)
    {
      this._prop = targetProperty;
      this._val = sourceValue;
    }

    /// <summary>
    /// Gets an <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that specifies the property that should be updated.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that specifies the property that should be updated.
    /// </returns>
    public DbExpression Property
    {
      get
      {
        return this._prop;
      }
    }

    /// <summary>
    /// Gets an <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that specifies the new value with which to update the property.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that specifies the new value with which to update the property.
    /// </returns>
    public DbExpression Value
    {
      get
      {
        return this._val;
      }
    }

    internal override void DumpStructure(ExpressionDumper dumper)
    {
      dumper.Begin(nameof (DbSetClause));
      if (this.Property != null)
        dumper.Dump(this.Property, "Property");
      if (this.Value != null)
        dumper.Dump(this.Value, "Value");
      dumper.End(nameof (DbSetClause));
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Common.Utils.TreeNode.#ctor(System.String,System.Data.Entity.Core.Common.Utils.TreeNode[])")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DbSetClause")]
    internal override TreeNode Print(DbExpressionVisitor<TreeNode> visitor)
    {
      TreeNode treeNode = new TreeNode(nameof (DbSetClause), new TreeNode[0]);
      if (this.Property != null)
        treeNode.Children.Add(new TreeNode("Property", new TreeNode[1]
        {
          this.Property.Accept<TreeNode>(visitor)
        }));
      if (this.Value != null)
        treeNode.Children.Add(new TreeNode("Value", new TreeNode[1]
        {
          this.Value.Accept<TreeNode>(visitor)
        }));
      return treeNode;
    }
  }
}
