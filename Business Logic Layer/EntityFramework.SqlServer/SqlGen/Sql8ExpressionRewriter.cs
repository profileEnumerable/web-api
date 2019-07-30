// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.SqlGen.Sql8ExpressionRewriter
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.SqlServer.Utilities;
using System.Globalization;

namespace System.Data.Entity.SqlServer.SqlGen
{
  internal class Sql8ExpressionRewriter : DbExpressionRebinder
  {
    internal static DbQueryCommandTree Rewrite(DbQueryCommandTree originalTree)
    {
      DbExpression query = new Sql8ExpressionRewriter(originalTree.MetadataWorkspace).VisitExpression(originalTree.Query);
      return new DbQueryCommandTree(originalTree.MetadataWorkspace, originalTree.DataSpace, query, false);
    }

    private Sql8ExpressionRewriter(MetadataWorkspace metadata)
      : base(metadata)
    {
    }

    public override DbExpression Visit(DbExceptExpression e)
    {
      Check.NotNull<DbExceptExpression>(e, nameof (e));
      return this.TransformIntersectOrExcept(this.VisitExpression(e.Left), this.VisitExpression(e.Right), DbExpressionKind.Except);
    }

    public override DbExpression Visit(DbIntersectExpression e)
    {
      Check.NotNull<DbIntersectExpression>(e, nameof (e));
      return this.TransformIntersectOrExcept(this.VisitExpression(e.Left), this.VisitExpression(e.Right), DbExpressionKind.Intersect);
    }

    public override DbExpression Visit(DbSkipExpression e)
    {
      Check.NotNull<DbSkipExpression>(e, nameof (e));
      DbExpression right = (DbExpression) this.VisitExpressionBinding(e.Input).Sort((IEnumerable<DbSortClause>) this.VisitSortOrder(e.SortOrder)).Limit(this.VisitExpression(e.Count));
      DbExpression left = this.VisitExpression(e.Input.Expression);
      IList<DbSortClause> dbSortClauseList = this.VisitSortOrder(e.SortOrder);
      IList<DbPropertyExpression> sortExpressionsOverLeft = (IList<DbPropertyExpression>) new List<DbPropertyExpression>(e.SortOrder.Count);
      foreach (DbSortClause dbSortClause in (IEnumerable<DbSortClause>) dbSortClauseList)
      {
        if (dbSortClause.Expression.ExpressionKind == DbExpressionKind.Property)
          sortExpressionsOverLeft.Add((DbPropertyExpression) dbSortClause.Expression);
      }
      return (DbExpression) this.TransformIntersectOrExcept(left, right, DbExpressionKind.Skip, sortExpressionsOverLeft, e.Input.VariableName).BindAs(e.Input.VariableName).Sort((IEnumerable<DbSortClause>) dbSortClauseList);
    }

    private DbExpression TransformIntersectOrExcept(
      DbExpression left,
      DbExpression right,
      DbExpressionKind expressionKind)
    {
      return this.TransformIntersectOrExcept(left, right, expressionKind, (IList<DbPropertyExpression>) null, (string) null);
    }

    private DbExpression TransformIntersectOrExcept(
      DbExpression left,
      DbExpression right,
      DbExpressionKind expressionKind,
      IList<DbPropertyExpression> sortExpressionsOverLeft,
      string sortExpressionsBindingVariableName)
    {
      bool flag1 = expressionKind == DbExpressionKind.Except || expressionKind == DbExpressionKind.Skip;
      bool flag2 = expressionKind == DbExpressionKind.Except || expressionKind == DbExpressionKind.Intersect;
      DbExpressionBinding input = left.Bind();
      DbExpressionBinding expressionBinding = right.Bind();
      IList<DbPropertyExpression> propertyExpressionList1 = (IList<DbPropertyExpression>) new List<DbPropertyExpression>();
      IList<DbPropertyExpression> propertyExpressionList2 = (IList<DbPropertyExpression>) new List<DbPropertyExpression>();
      this.FlattenProperties((DbExpression) input.Variable, propertyExpressionList1);
      this.FlattenProperties((DbExpression) expressionBinding.Variable, propertyExpressionList2);
      if (expressionKind == DbExpressionKind.Skip && Sql8ExpressionRewriter.RemoveNonSortProperties(propertyExpressionList1, propertyExpressionList2, sortExpressionsOverLeft, input.VariableName, sortExpressionsBindingVariableName))
        expressionBinding = Sql8ExpressionRewriter.CapWithProject(expressionBinding, propertyExpressionList2);
      DbExpression dbExpression1 = (DbExpression) null;
      for (int index = 0; index < propertyExpressionList1.Count; ++index)
      {
        DbExpression right1 = (DbExpression) propertyExpressionList1[index].Equal((DbExpression) propertyExpressionList2[index]).Or((DbExpression) propertyExpressionList1[index].IsNull().And((DbExpression) propertyExpressionList2[index].IsNull()));
        dbExpression1 = index != 0 ? (DbExpression) dbExpression1.And(right1) : right1;
      }
      DbExpression dbExpression2 = (DbExpression) expressionBinding.Any(dbExpression1);
      DbExpression predicate = !flag1 ? dbExpression2 : (DbExpression) dbExpression2.Not();
      DbExpression dbExpression3 = (DbExpression) input.Filter(predicate);
      if (flag2)
        dbExpression3 = (DbExpression) dbExpression3.Distinct();
      return dbExpression3;
    }

    private void FlattenProperties(
      DbExpression input,
      IList<DbPropertyExpression> flattenedProperties)
    {
      foreach (EdmProperty property in input.ResultType.GetProperties())
      {
        DbPropertyExpression propertyExpression = input.Property(property);
        if (BuiltInTypeKind.PrimitiveType == property.TypeUsage.EdmType.BuiltInTypeKind)
          flattenedProperties.Add(propertyExpression);
        else
          this.FlattenProperties((DbExpression) propertyExpression, flattenedProperties);
      }
    }

    private static bool RemoveNonSortProperties(
      IList<DbPropertyExpression> list1,
      IList<DbPropertyExpression> list2,
      IList<DbPropertyExpression> sortList,
      string list1BindingVariableName,
      string sortExpressionsBindingVariableName)
    {
      bool flag = false;
      for (int index = list1.Count - 1; index >= 0; --index)
      {
        if (!Sql8ExpressionRewriter.HasMatchInList(list1[index], sortList, list1BindingVariableName, sortExpressionsBindingVariableName))
        {
          list1.RemoveAt(index);
          list2.RemoveAt(index);
          flag = true;
        }
      }
      return flag;
    }

    private static bool HasMatchInList(
      DbPropertyExpression expr,
      IList<DbPropertyExpression> list,
      string exprBindingVariableName,
      string listExpressionsBindingVariableName)
    {
      for (int index = 0; index < list.Count; ++index)
      {
        if (Sql8ExpressionRewriter.AreMatching(expr, list[index], exprBindingVariableName, listExpressionsBindingVariableName))
        {
          list.RemoveAt(index);
          return true;
        }
      }
      return false;
    }

    private static bool AreMatching(
      DbPropertyExpression expr1,
      DbPropertyExpression expr2,
      string expr1BindingVariableName,
      string expr2BindingVariableName)
    {
      if (expr1.Property.Name != expr2.Property.Name || expr1.Instance.ExpressionKind != expr2.Instance.ExpressionKind)
        return false;
      if (expr1.Instance.ExpressionKind == DbExpressionKind.Property)
        return Sql8ExpressionRewriter.AreMatching((DbPropertyExpression) expr1.Instance, (DbPropertyExpression) expr2.Instance, expr1BindingVariableName, expr2BindingVariableName);
      DbVariableReferenceExpression instance1 = (DbVariableReferenceExpression) expr1.Instance;
      DbVariableReferenceExpression instance2 = (DbVariableReferenceExpression) expr2.Instance;
      if (string.Equals(instance1.VariableName, expr1BindingVariableName, StringComparison.Ordinal))
        return string.Equals(instance2.VariableName, expr2BindingVariableName, StringComparison.Ordinal);
      return false;
    }

    private static DbExpressionBinding CapWithProject(
      DbExpressionBinding inputBinding,
      IList<DbPropertyExpression> flattenedProperties)
    {
      List<KeyValuePair<string, DbExpression>> keyValuePairList = new List<KeyValuePair<string, DbExpression>>(flattenedProperties.Count);
      Dictionary<string, int> dictionary = new Dictionary<string, int>(flattenedProperties.Count);
      foreach (DbPropertyExpression flattenedProperty in (IEnumerable<DbPropertyExpression>) flattenedProperties)
      {
        string key1 = flattenedProperty.Property.Name;
        int num;
        if (dictionary.TryGetValue(key1, out num))
        {
          string key2;
          do
          {
            ++num;
            key2 = key1 + num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          }
          while (dictionary.ContainsKey(key2));
          dictionary[key1] = num;
          key1 = key2;
        }
        dictionary[key1] = 0;
        keyValuePairList.Add(new KeyValuePair<string, DbExpression>(key1, (DbExpression) flattenedProperty));
      }
      DbExpression projection = (DbExpression) DbExpressionBuilder.NewRow((IEnumerable<KeyValuePair<string, DbExpression>>) keyValuePairList);
      DbExpressionBinding expressionBinding = inputBinding.Project(projection).Bind();
      flattenedProperties.Clear();
      RowType edmType = (RowType) projection.ResultType.EdmType;
      foreach (KeyValuePair<string, DbExpression> keyValuePair in keyValuePairList)
      {
        EdmProperty property = edmType.Properties[keyValuePair.Key];
        flattenedProperties.Add(expressionBinding.Variable.Property(property));
      }
      return expressionBinding;
    }
  }
}
