// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.Internal.EntitySqlQueryBuilder
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace System.Data.Entity.Core.Objects.Internal
{
  internal static class EntitySqlQueryBuilder
  {
    private const string _setOpEpilog = "\r\n)";
    private const string _setOpProlog = "(\r\n";
    private const string _fromOp = "\r\nFROM (\r\n";
    private const string _asOp = "\r\n) AS ";
    private const string _distinctProlog = "SET(\r\n";
    private const string _distinctEpilog = "\r\n)";
    private const string _exceptOp = "\r\n) EXCEPT (\r\n";
    private const string _groupByOp = "\r\nGROUP BY\r\n";
    private const string _intersectOp = "\r\n) INTERSECT (\r\n";
    private const string _ofTypeProlog = "OFTYPE(\r\n(\r\n";
    private const string _ofTypeInfix = "\r\n),\r\n[";
    private const string _ofTypeInfix2 = "].[";
    private const string _ofTypeEpilog = "]\r\n)";
    private const string _orderByOp = "\r\nORDER BY\r\n";
    private const string _selectOp = "SELECT ";
    private const string _selectValueOp = "SELECT VALUE ";
    private const string _skipOp = "\r\nSKIP\r\n";
    private const string _limitOp = "\r\nLIMIT\r\n";
    private const string _topOp = "SELECT VALUE TOP(\r\n";
    private const string _topInfix = "\r\n) ";
    private const string _unionOp = "\r\n) UNION (\r\n";
    private const string _unionAllOp = "\r\n) UNION ALL (\r\n";
    private const string _whereOp = "\r\nWHERE\r\n";

    private static string GetCommandText(ObjectQueryState query)
    {
      string commandText = (string) null;
      if (!query.TryGetCommandText(out commandText))
        throw new NotSupportedException(Strings.ObjectQuery_QueryBuilder_NotSupportedLinqSource);
      return commandText;
    }

    private static ObjectParameterCollection MergeParameters(
      ObjectContext context,
      ObjectParameterCollection sourceQueryParams,
      ObjectParameter[] builderMethodParams)
    {
      if (sourceQueryParams == null && builderMethodParams.Length == 0)
        return (ObjectParameterCollection) null;
      ObjectParameterCollection parameterCollection = ObjectParameterCollection.DeepCopy(sourceQueryParams) ?? new ObjectParameterCollection(context.Perspective);
      foreach (ObjectParameter builderMethodParam in builderMethodParams)
        parameterCollection.Add(builderMethodParam);
      return parameterCollection;
    }

    private static ObjectParameterCollection MergeParameters(
      ObjectParameterCollection query1Params,
      ObjectParameterCollection query2Params)
    {
      if (query1Params == null && query2Params == null)
        return (ObjectParameterCollection) null;
      ObjectParameterCollection parameterCollection1;
      ObjectParameterCollection parameterCollection2;
      if (query1Params != null)
      {
        parameterCollection1 = ObjectParameterCollection.DeepCopy(query1Params);
        parameterCollection2 = query2Params;
      }
      else
      {
        parameterCollection1 = ObjectParameterCollection.DeepCopy(query2Params);
        parameterCollection2 = query1Params;
      }
      if (parameterCollection2 != null)
      {
        foreach (ObjectParameter objectParameter in parameterCollection2)
          parameterCollection1.Add(objectParameter.ShallowCopy());
      }
      return parameterCollection1;
    }

    private static ObjectQueryState NewBuilderQuery(
      ObjectQueryState sourceQuery,
      Type elementType,
      StringBuilder queryText,
      Span newSpan,
      IEnumerable<ObjectParameter> enumerableParams)
    {
      return EntitySqlQueryBuilder.NewBuilderQuery(sourceQuery, elementType, queryText, false, newSpan, enumerableParams);
    }

    private static ObjectQueryState NewBuilderQuery(
      ObjectQueryState sourceQuery,
      Type elementType,
      StringBuilder queryText,
      bool allowsLimit,
      Span newSpan,
      IEnumerable<ObjectParameter> enumerableParams)
    {
      ObjectParameterCollection parameters = enumerableParams as ObjectParameterCollection;
      if (parameters == null && enumerableParams != null)
      {
        parameters = new ObjectParameterCollection(sourceQuery.ObjectContext.Perspective);
        foreach (ObjectParameter enumerableParam in enumerableParams)
          parameters.Add(enumerableParam);
      }
      EntitySqlQueryState entitySqlQueryState = new EntitySqlQueryState(elementType, queryText.ToString(), allowsLimit, sourceQuery.ObjectContext, parameters, newSpan);
      sourceQuery.ApplySettingsTo((ObjectQueryState) entitySqlQueryState);
      return (ObjectQueryState) entitySqlQueryState;
    }

    [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
    private static ObjectQueryState BuildSetOp(
      ObjectQueryState leftQuery,
      ObjectQueryState rightQuery,
      Span newSpan,
      string setOp)
    {
      string commandText1 = EntitySqlQueryBuilder.GetCommandText(leftQuery);
      string commandText2 = EntitySqlQueryBuilder.GetCommandText(rightQuery);
      if (!object.ReferenceEquals((object) leftQuery.ObjectContext, (object) rightQuery.ObjectContext))
        throw new ArgumentException(Strings.ObjectQuery_QueryBuilder_InvalidQueryArgument, "query");
      StringBuilder queryText = new StringBuilder("(\r\n".Length + commandText1.Length + setOp.Length + commandText2.Length + "\r\n)".Length);
      queryText.Append("(\r\n");
      queryText.Append(commandText1);
      queryText.Append(setOp);
      queryText.Append(commandText2);
      queryText.Append("\r\n)");
      return EntitySqlQueryBuilder.NewBuilderQuery(leftQuery, leftQuery.ElementType, queryText, newSpan, (IEnumerable<ObjectParameter>) EntitySqlQueryBuilder.MergeParameters(leftQuery.Parameters, rightQuery.Parameters));
    }

    private static ObjectQueryState BuildSelectOrSelectValue(
      ObjectQueryState query,
      string alias,
      string projection,
      ObjectParameter[] parameters,
      string projectOp,
      Type elementType)
    {
      string commandText = EntitySqlQueryBuilder.GetCommandText(query);
      StringBuilder queryText = new StringBuilder(projectOp.Length + projection.Length + "\r\nFROM (\r\n".Length + commandText.Length + "\r\n) AS ".Length + alias.Length);
      queryText.Append(projectOp);
      queryText.Append(projection);
      queryText.Append("\r\nFROM (\r\n");
      queryText.Append(commandText);
      queryText.Append("\r\n) AS ");
      queryText.Append(alias);
      return EntitySqlQueryBuilder.NewBuilderQuery(query, elementType, queryText, (Span) null, (IEnumerable<ObjectParameter>) EntitySqlQueryBuilder.MergeParameters(query.ObjectContext, query.Parameters, parameters));
    }

    private static ObjectQueryState BuildOrderByOrWhere(
      ObjectQueryState query,
      string alias,
      string predicateOrKeys,
      ObjectParameter[] parameters,
      string op,
      string skipCount,
      bool allowsLimit)
    {
      string commandText = EntitySqlQueryBuilder.GetCommandText(query);
      int capacity = "SELECT VALUE ".Length + alias.Length + "\r\nFROM (\r\n".Length + commandText.Length + "\r\n) AS ".Length + alias.Length + op.Length + predicateOrKeys.Length;
      if (skipCount != null)
        capacity += "\r\nSKIP\r\n".Length + skipCount.Length;
      StringBuilder queryText = new StringBuilder(capacity);
      queryText.Append("SELECT VALUE ");
      queryText.Append(alias);
      queryText.Append("\r\nFROM (\r\n");
      queryText.Append(commandText);
      queryText.Append("\r\n) AS ");
      queryText.Append(alias);
      queryText.Append(op);
      queryText.Append(predicateOrKeys);
      if (skipCount != null)
      {
        queryText.Append("\r\nSKIP\r\n");
        queryText.Append(skipCount);
      }
      return EntitySqlQueryBuilder.NewBuilderQuery(query, query.ElementType, queryText, allowsLimit, query.Span, (IEnumerable<ObjectParameter>) EntitySqlQueryBuilder.MergeParameters(query.ObjectContext, query.Parameters, parameters));
    }

    internal static ObjectQueryState Distinct(ObjectQueryState query)
    {
      string commandText = EntitySqlQueryBuilder.GetCommandText(query);
      StringBuilder queryText = new StringBuilder("SET(\r\n".Length + commandText.Length + "\r\n)".Length);
      queryText.Append("SET(\r\n");
      queryText.Append(commandText);
      queryText.Append("\r\n)");
      return EntitySqlQueryBuilder.NewBuilderQuery(query, query.ElementType, queryText, query.Span, (IEnumerable<ObjectParameter>) ObjectParameterCollection.DeepCopy(query.Parameters));
    }

    internal static ObjectQueryState Except(
      ObjectQueryState leftQuery,
      ObjectQueryState rightQuery)
    {
      return EntitySqlQueryBuilder.BuildSetOp(leftQuery, rightQuery, leftQuery.Span, "\r\n) EXCEPT (\r\n");
    }

    internal static ObjectQueryState GroupBy(
      ObjectQueryState query,
      string alias,
      string keys,
      string projection,
      ObjectParameter[] parameters)
    {
      string commandText = EntitySqlQueryBuilder.GetCommandText(query);
      StringBuilder queryText = new StringBuilder("SELECT ".Length + projection.Length + "\r\nFROM (\r\n".Length + commandText.Length + "\r\n) AS ".Length + alias.Length + "\r\nGROUP BY\r\n".Length + keys.Length);
      queryText.Append("SELECT ");
      queryText.Append(projection);
      queryText.Append("\r\nFROM (\r\n");
      queryText.Append(commandText);
      queryText.Append("\r\n) AS ");
      queryText.Append(alias);
      queryText.Append("\r\nGROUP BY\r\n");
      queryText.Append(keys);
      return EntitySqlQueryBuilder.NewBuilderQuery(query, typeof (DbDataRecord), queryText, (Span) null, (IEnumerable<ObjectParameter>) EntitySqlQueryBuilder.MergeParameters(query.ObjectContext, query.Parameters, parameters));
    }

    internal static ObjectQueryState Intersect(
      ObjectQueryState leftQuery,
      ObjectQueryState rightQuery)
    {
      Span newSpan = Span.CopyUnion(leftQuery.Span, rightQuery.Span);
      return EntitySqlQueryBuilder.BuildSetOp(leftQuery, rightQuery, newSpan, "\r\n) INTERSECT (\r\n");
    }

    internal static ObjectQueryState OfType(
      ObjectQueryState query,
      EdmType newType,
      Type clrOfType)
    {
      string commandText = EntitySqlQueryBuilder.GetCommandText(query);
      StringBuilder queryText = new StringBuilder("OFTYPE(\r\n(\r\n".Length + commandText.Length + "\r\n),\r\n[".Length + newType.NamespaceName.Length + (!string.IsNullOrEmpty(newType.NamespaceName) ? "].[".Length : 0) + newType.Name.Length + "]\r\n)".Length);
      queryText.Append("OFTYPE(\r\n(\r\n");
      queryText.Append(commandText);
      queryText.Append("\r\n),\r\n[");
      if (!string.IsNullOrEmpty(newType.NamespaceName))
      {
        queryText.Append(newType.NamespaceName);
        queryText.Append("].[");
      }
      queryText.Append(newType.Name);
      queryText.Append("]\r\n)");
      return EntitySqlQueryBuilder.NewBuilderQuery(query, clrOfType, queryText, query.Span, (IEnumerable<ObjectParameter>) ObjectParameterCollection.DeepCopy(query.Parameters));
    }

    internal static ObjectQueryState OrderBy(
      ObjectQueryState query,
      string alias,
      string keys,
      ObjectParameter[] parameters)
    {
      return EntitySqlQueryBuilder.BuildOrderByOrWhere(query, alias, keys, parameters, "\r\nORDER BY\r\n", (string) null, true);
    }

    internal static ObjectQueryState Select(
      ObjectQueryState query,
      string alias,
      string projection,
      ObjectParameter[] parameters)
    {
      return EntitySqlQueryBuilder.BuildSelectOrSelectValue(query, alias, projection, parameters, "SELECT ", typeof (DbDataRecord));
    }

    internal static ObjectQueryState SelectValue(
      ObjectQueryState query,
      string alias,
      string projection,
      ObjectParameter[] parameters,
      Type projectedType)
    {
      return EntitySqlQueryBuilder.BuildSelectOrSelectValue(query, alias, projection, parameters, "SELECT VALUE ", projectedType);
    }

    internal static ObjectQueryState Skip(
      ObjectQueryState query,
      string alias,
      string keys,
      string count,
      ObjectParameter[] parameters)
    {
      return EntitySqlQueryBuilder.BuildOrderByOrWhere(query, alias, keys, parameters, "\r\nORDER BY\r\n", count, true);
    }

    internal static ObjectQueryState Top(
      ObjectQueryState query,
      string alias,
      string count,
      ObjectParameter[] parameters)
    {
      int length = count.Length;
      string commandText = EntitySqlQueryBuilder.GetCommandText(query);
      bool allowsLimitSubclause = ((EntitySqlQueryState) query).AllowsLimitSubclause;
      StringBuilder queryText = new StringBuilder(!allowsLimitSubclause ? length + ("SELECT VALUE TOP(\r\n".Length + "\r\n) ".Length + alias.Length + "\r\nFROM (\r\n".Length + commandText.Length + "\r\n) AS ".Length + alias.Length) : length + (commandText.Length + "\r\nLIMIT\r\n".Length));
      if (allowsLimitSubclause)
      {
        queryText.Append(commandText);
        queryText.Append("\r\nLIMIT\r\n");
        queryText.Append(count);
      }
      else
      {
        queryText.Append("SELECT VALUE TOP(\r\n");
        queryText.Append(count);
        queryText.Append("\r\n) ");
        queryText.Append(alias);
        queryText.Append("\r\nFROM (\r\n");
        queryText.Append(commandText);
        queryText.Append("\r\n) AS ");
        queryText.Append(alias);
      }
      return EntitySqlQueryBuilder.NewBuilderQuery(query, query.ElementType, queryText, query.Span, (IEnumerable<ObjectParameter>) EntitySqlQueryBuilder.MergeParameters(query.ObjectContext, query.Parameters, parameters));
    }

    internal static ObjectQueryState Union(
      ObjectQueryState leftQuery,
      ObjectQueryState rightQuery)
    {
      Span newSpan = Span.CopyUnion(leftQuery.Span, rightQuery.Span);
      return EntitySqlQueryBuilder.BuildSetOp(leftQuery, rightQuery, newSpan, "\r\n) UNION (\r\n");
    }

    internal static ObjectQueryState UnionAll(
      ObjectQueryState leftQuery,
      ObjectQueryState rightQuery)
    {
      Span newSpan = Span.CopyUnion(leftQuery.Span, rightQuery.Span);
      return EntitySqlQueryBuilder.BuildSetOp(leftQuery, rightQuery, newSpan, "\r\n) UNION ALL (\r\n");
    }

    internal static ObjectQueryState Where(
      ObjectQueryState query,
      string alias,
      string predicate,
      ObjectParameter[] parameters)
    {
      return EntitySqlQueryBuilder.BuildOrderByOrWhere(query, alias, predicate, parameters, "\r\nWHERE\r\n", (string) null, false);
    }
  }
}
