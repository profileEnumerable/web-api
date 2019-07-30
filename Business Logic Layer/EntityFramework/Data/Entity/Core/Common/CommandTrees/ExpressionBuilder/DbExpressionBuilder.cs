// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder.DbExpressionBuilder
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder.Internal;
using System.Data.Entity.Core.Common.CommandTrees.Internal;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Metadata.Edm.Provider;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder
{
  /// <summary>
  /// Provides an API to construct <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" />s and allows that API to be accessed as extension methods on the expression type itself.
  /// </summary>
  [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Db")]
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
  public static class DbExpressionBuilder
  {
    private static readonly TypeUsage _booleanType = EdmProviderManifest.Instance.GetCanonicalModelTypeUsage(PrimitiveTypeKind.Boolean);
    private static readonly AliasGenerator _bindingAliases = new AliasGenerator("Var_", 0);
    private static readonly DbNullExpression _binaryNull = EdmProviderManifest.Instance.GetCanonicalModelTypeUsage(PrimitiveTypeKind.Binary).Null();
    private static readonly DbNullExpression _boolNull = EdmProviderManifest.Instance.GetCanonicalModelTypeUsage(PrimitiveTypeKind.Boolean).Null();
    private static readonly DbNullExpression _byteNull = EdmProviderManifest.Instance.GetCanonicalModelTypeUsage(PrimitiveTypeKind.Byte).Null();
    private static readonly DbNullExpression _dateTimeNull = EdmProviderManifest.Instance.GetCanonicalModelTypeUsage(PrimitiveTypeKind.DateTime).Null();
    private static readonly DbNullExpression _dateTimeOffsetNull = EdmProviderManifest.Instance.GetCanonicalModelTypeUsage(PrimitiveTypeKind.DateTimeOffset).Null();
    private static readonly DbNullExpression _decimalNull = EdmProviderManifest.Instance.GetCanonicalModelTypeUsage(PrimitiveTypeKind.Decimal).Null();
    private static readonly DbNullExpression _doubleNull = EdmProviderManifest.Instance.GetCanonicalModelTypeUsage(PrimitiveTypeKind.Double).Null();
    private static readonly DbNullExpression _geographyNull = EdmProviderManifest.Instance.GetCanonicalModelTypeUsage(PrimitiveTypeKind.Geography).Null();
    private static readonly DbNullExpression _geometryNull = EdmProviderManifest.Instance.GetCanonicalModelTypeUsage(PrimitiveTypeKind.Geometry).Null();
    private static readonly DbNullExpression _guidNull = EdmProviderManifest.Instance.GetCanonicalModelTypeUsage(PrimitiveTypeKind.Guid).Null();
    private static readonly DbNullExpression _int16Null = EdmProviderManifest.Instance.GetCanonicalModelTypeUsage(PrimitiveTypeKind.Int16).Null();
    private static readonly DbNullExpression _int32Null = EdmProviderManifest.Instance.GetCanonicalModelTypeUsage(PrimitiveTypeKind.Int32).Null();
    private static readonly DbNullExpression _int64Null = EdmProviderManifest.Instance.GetCanonicalModelTypeUsage(PrimitiveTypeKind.Int64).Null();
    private static readonly DbNullExpression _sbyteNull = EdmProviderManifest.Instance.GetCanonicalModelTypeUsage(PrimitiveTypeKind.SByte).Null();
    private static readonly DbNullExpression _singleNull = EdmProviderManifest.Instance.GetCanonicalModelTypeUsage(PrimitiveTypeKind.Single).Null();
    private static readonly DbNullExpression _stringNull = EdmProviderManifest.Instance.GetCanonicalModelTypeUsage(PrimitiveTypeKind.String).Null();
    private static readonly DbNullExpression _timeNull = EdmProviderManifest.Instance.GetCanonicalModelTypeUsage(PrimitiveTypeKind.Time).Null();
    private static readonly DbConstantExpression _boolTrue = DbExpressionBuilder.Constant((object) true);
    private static readonly DbConstantExpression _boolFalse = DbExpressionBuilder.Constant((object) false);

    /// <summary>Returns the specified arguments as a key/value pair object.</summary>
    /// <returns>A key/value pair object.</returns>
    /// <param name="value">The value in the key/value pair.</param>
    /// <param name="alias">The key in the key/value pair.</param>
    public static KeyValuePair<string, DbExpression> As(
      this DbExpression value,
      string alias)
    {
      return new KeyValuePair<string, DbExpression>(alias, value);
    }

    /// <summary>Returns the specified arguments as a key/value pair object.</summary>
    /// <returns>A key/value pair object.</returns>
    /// <param name="value">The value in the key/value pair.</param>
    /// <param name="alias">The key in the key/value pair.</param>
    public static KeyValuePair<string, DbAggregate> As(
      this DbAggregate value,
      string alias)
    {
      return new KeyValuePair<string, DbAggregate>(alias, value);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionBinding" /> that uses a generated variable name to bind the given expression.
    /// </summary>
    /// <returns>A new expression binding with the specified expression and a generated variable name.</returns>
    /// <param name="input">The expression to bind.</param>
    /// <exception cref="T:System.ArgumentNullException">input is null.</exception>
    /// <exception cref="T:System.ArgumentException">input does not have a collection result.</exception>
    public static DbExpressionBinding Bind(this DbExpression input)
    {
      Check.NotNull<DbExpression>(input, nameof (input));
      return input.BindAs(DbExpressionBuilder._bindingAliases.Next());
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionBinding" /> that uses the specified variable name to bind the given expression
    /// </summary>
    /// <returns>A new expression binding with the specified expression and variable name.</returns>
    /// <param name="input">The expression to bind.</param>
    /// <param name="varName">The variable name that should be used for the binding.</param>
    /// <exception cref="T:System.ArgumentNullException">input or varName is null.</exception>
    /// <exception cref="T:System.ArgumentException">input does not have a collection result.</exception>
    public static DbExpressionBinding BindAs(
      this DbExpression input,
      string varName)
    {
      Check.NotNull<DbExpression>(input, nameof (input));
      Check.NotNull<string>(varName, nameof (varName));
      Check.NotEmpty(varName, nameof (varName));
      TypeUsage elementType = (TypeUsage) null;
      if (!TypeHelpers.TryGetCollectionElementType(input.ResultType, out elementType))
        throw new ArgumentException(Strings.Cqt_Binding_CollectionRequired, nameof (input));
      DbVariableReferenceExpression varRef = new DbVariableReferenceExpression(elementType, varName);
      return new DbExpressionBinding(input, varRef);
    }

    /// <summary>Creates a new group expression binding that uses generated variable and group variable names to bind the given expression.</summary>
    /// <returns>A new group expression binding with the specified expression and a generated variable name and group variable name.</returns>
    /// <param name="input">The expression to bind.</param>
    /// <exception cref="T:System.ArgumentNullException">input is null.</exception>
    /// <exception cref="T:System.ArgumentException">input does not have a collection result type.</exception>
    public static DbGroupExpressionBinding GroupBind(this DbExpression input)
    {
      Check.NotNull<DbExpression>(input, nameof (input));
      string varName = DbExpressionBuilder._bindingAliases.Next();
      return input.GroupBindAs(varName, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Group{0}", (object) varName));
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbGroupExpressionBinding" /> that uses the specified variable name and group variable names to bind the given expression.
    /// </summary>
    /// <returns>A new group expression binding with the specified expression, variable name and group variable name.</returns>
    /// <param name="input">The expression to bind.</param>
    /// <param name="varName">The variable name that should be used for the binding.</param>
    /// <param name="groupVarName">The variable name that should be used to refer to the group when the new group expression binding is used in a group-by expression.</param>
    /// <exception cref="T:System.ArgumentNullException">input, varName or groupVarName is null.</exception>
    /// <exception cref="T:System.ArgumentException">input does not have a collection result type.</exception>
    public static DbGroupExpressionBinding GroupBindAs(
      this DbExpression input,
      string varName,
      string groupVarName)
    {
      Check.NotNull<DbExpression>(input, nameof (input));
      Check.NotNull<string>(varName, nameof (varName));
      Check.NotEmpty(varName, nameof (varName));
      Check.NotNull<string>(groupVarName, nameof (groupVarName));
      Check.NotEmpty(groupVarName, nameof (groupVarName));
      TypeUsage elementType = (TypeUsage) null;
      if (!TypeHelpers.TryGetCollectionElementType(input.ResultType, out elementType))
        throw new ArgumentException(Strings.Cqt_GroupBinding_CollectionRequired, nameof (input));
      DbVariableReferenceExpression inputRef = new DbVariableReferenceExpression(elementType, varName);
      DbVariableReferenceExpression groupRef = new DbVariableReferenceExpression(elementType, groupVarName);
      return new DbGroupExpressionBinding(input, inputRef, groupRef);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionAggregate" />.
    /// </summary>
    /// <returns>A new function aggregate with a reference to the given function and argument. The function aggregate's Distinct property will have the value false.</returns>
    /// <param name="function">The function that defines the aggregate operation.</param>
    /// <param name="argument">The argument over which the aggregate function should be calculated.</param>
    /// <exception cref="T:System.ArgumentNullException">function or argument null.</exception>
    /// <exception cref="T:System.ArgumentException">function is not an aggregate function or has more than one argument, or the result type of argument is not equal or promotable to the parameter type of function.</exception>
    public static DbFunctionAggregate Aggregate(
      this EdmFunction function,
      DbExpression argument)
    {
      Check.NotNull<EdmFunction>(function, nameof (function));
      Check.NotNull<DbExpression>(argument, nameof (argument));
      return DbExpressionBuilder.CreateFunctionAggregate(function, argument, false);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionAggregate" /> that is applied in a distinct fashion.
    /// </summary>
    /// <returns>A new function aggregate with a reference to the given function and argument. The function aggregate's Distinct property will have the value true.</returns>
    /// <param name="function">The function that defines the aggregate operation.</param>
    /// <param name="argument">The argument over which the aggregate function should be calculated.</param>
    /// <exception cref="T:System.ArgumentNullException">function or argument is null.</exception>
    /// <exception cref="T:System.ArgumentException">function is not an aggregate function or has more than one argument, or the result type of argument is not equal or promotable to the parameter type of function.</exception>
    public static DbFunctionAggregate AggregateDistinct(
      this EdmFunction function,
      DbExpression argument)
    {
      Check.NotNull<EdmFunction>(function, nameof (function));
      Check.NotNull<DbExpression>(argument, nameof (argument));
      return DbExpressionBuilder.CreateFunctionAggregate(function, argument, true);
    }

    private static DbFunctionAggregate CreateFunctionAggregate(
      EdmFunction function,
      DbExpression argument,
      bool isDistinct)
    {
      DbExpressionList arguments = ArgumentValidation.ValidateFunctionAggregate(function, (IEnumerable<DbExpression>) new DbExpression[1]
      {
        argument
      });
      return new DbFunctionAggregate(function.ReturnParameter.TypeUsage, arguments, function, isDistinct);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbGroupAggregate" /> over the specified argument
    /// </summary>
    /// <param name="argument"> The argument over which to perform the nest operation </param>
    /// <returns> A new group aggregate representing the elements of the group referenced by the given argument. </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="argument" />
    /// is null
    /// </exception>
    public static DbGroupAggregate GroupAggregate(DbExpression argument)
    {
      Check.NotNull<DbExpression>(argument, nameof (argument));
      DbExpressionList arguments = new DbExpressionList((IList<DbExpression>) new DbExpression[1]
      {
        argument
      });
      return new DbGroupAggregate(TypeHelpers.CreateCollectionTypeUsage(argument.ResultType), arguments);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbLambda" /> with the specified inline Lambda function implementation and formal parameters.
    /// </summary>
    /// <returns>A new expression that describes an inline Lambda function with the specified body and formal parameters.</returns>
    /// <param name="body">An expression that defines the logic of the Lambda function.</param>
    /// <param name="variables">
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbVariableReferenceExpression" /> collection that represents the formal parameters to the Lambda function. These variables are valid for use in the body expression.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">variables is null or contains null, or body is null.</exception>
    /// <exception cref="T:System.ArgumentException">variables contains more than one element with the same variable name.</exception>
    public static DbLambda Lambda(
      DbExpression body,
      IEnumerable<DbVariableReferenceExpression> variables)
    {
      Check.NotNull<DbExpression>(body, nameof (body));
      Check.NotNull<IEnumerable<DbVariableReferenceExpression>>(variables, nameof (variables));
      return DbExpressionBuilder.CreateLambda(body, variables);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbLambda" /> with the specified inline Lambda function implementation and formal parameters.
    /// </summary>
    /// <returns>A new expression that describes an inline Lambda function with the specified body and formal parameters.</returns>
    /// <param name="body">An expression that defines the logic of the Lambda function.</param>
    /// <param name="variables">
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbVariableReferenceExpression" /> collection that represents the formal parameters to the Lambda function. These variables are valid for use in the body expression.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">variables is null or contains null, or body is null.</exception>
    /// <exception cref="T:System.ArgumentException">variables contains more than one element with the same variable name.</exception>
    public static DbLambda Lambda(
      DbExpression body,
      params DbVariableReferenceExpression[] variables)
    {
      Check.NotNull<DbExpression>(body, nameof (body));
      Check.NotNull<DbVariableReferenceExpression[]>(variables, nameof (variables));
      return DbExpressionBuilder.CreateLambda(body, (IEnumerable<DbVariableReferenceExpression>) variables);
    }

    private static DbLambda CreateLambda(
      DbExpression body,
      IEnumerable<DbVariableReferenceExpression> variables)
    {
      return new DbLambda(ArgumentValidation.ValidateLambda(variables), body);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbSortClause" /> with an ascending sort order and default collation.
    /// </summary>
    /// <returns>A new sort clause with the given sort key and ascending sort order.</returns>
    /// <param name="key">The expression that defines the sort key.</param>
    /// <exception cref="T:System.ArgumentNullException">key is null.</exception>
    /// <exception cref="T:System.ArgumentException">key does not have an order-comparable result type.</exception>
    public static DbSortClause ToSortClause(this DbExpression key)
    {
      Check.NotNull<DbExpression>(key, nameof (key));
      ArgumentValidation.ValidateSortClause(key);
      return new DbSortClause(key, true, string.Empty);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbSortClause" /> with a descending sort order and default collation.
    /// </summary>
    /// <returns>A new sort clause with the given sort key and descending sort order.</returns>
    /// <param name="key">The expression that defines the sort key.</param>
    /// <exception cref="T:System.ArgumentNullException">key is null.</exception>
    /// <exception cref="T:System.ArgumentException">key does not have an order-comparable result type.</exception>
    public static DbSortClause ToSortClauseDescending(this DbExpression key)
    {
      Check.NotNull<DbExpression>(key, nameof (key));
      ArgumentValidation.ValidateSortClause(key);
      return new DbSortClause(key, false, string.Empty);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbSortClause" /> with an ascending sort order and the specified collation.
    /// </summary>
    /// <returns>A new sort clause with the given sort key and collation, and ascending sort order.</returns>
    /// <param name="key">The expression that defines the sort key.</param>
    /// <param name="collation">The collation to sort under.</param>
    /// <exception cref="T:System.ArgumentNullException">key is null.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">collation is empty or contains only space characters.</exception>
    /// <exception cref="T:System.ArgumentException">key does not have an order-comparable result type.</exception>
    public static DbSortClause ToSortClause(this DbExpression key, string collation)
    {
      Check.NotNull<DbExpression>(key, nameof (key));
      Check.NotNull<string>(collation, nameof (collation));
      ArgumentValidation.ValidateSortClause(key, collation);
      return new DbSortClause(key, true, collation);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbSortClause" /> with a descending sort order and the specified collation.
    /// </summary>
    /// <returns>A new sort clause with the given sort key and collation, and descending sort order.</returns>
    /// <param name="key">The expression that defines the sort key.</param>
    /// <param name="collation">The collation to sort under.</param>
    /// <exception cref="T:System.ArgumentNullException">key is null.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">collation is empty or contains only space characters.</exception>
    /// <exception cref="T:System.ArgumentException">key does not have an order-comparable result type.</exception>
    public static DbSortClause ToSortClauseDescending(
      this DbExpression key,
      string collation)
    {
      Check.NotNull<DbExpression>(key, nameof (key));
      Check.NotNull<string>(collation, nameof (collation));
      ArgumentValidation.ValidateSortClause(key, collation);
      return new DbSortClause(key, false, collation);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbQuantifierExpression" /> that determines whether the given predicate holds for all elements of the input set.
    /// </summary>
    /// <returns>A new DbQuantifierExpression that represents the All operation.</returns>
    /// <param name="input">An expression binding that specifies the input set.</param>
    /// <param name="predicate">An expression representing a predicate to evaluate for each member of the input set.</param>
    /// <exception cref="T:System.ArgumentNullException">input or predicate is null.</exception>
    /// <exception cref="T:System.ArgumentException">predicate  does not have a Boolean result type.</exception>
    public static DbQuantifierExpression All(
      this DbExpressionBinding input,
      DbExpression predicate)
    {
      Check.NotNull<DbExpression>(predicate, nameof (predicate));
      Check.NotNull<DbExpressionBinding>(input, nameof (input));
      return new DbQuantifierExpression(DbExpressionKind.All, ArgumentValidation.ValidateQuantifier(predicate), input, predicate);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbQuantifierExpression" /> that determines whether the given predicate holds for any element of the input set.
    /// </summary>
    /// <returns>A new DbQuantifierExpression that represents the Any operation.</returns>
    /// <param name="input">An expression binding that specifies the input set.</param>
    /// <param name="predicate">An expression representing a predicate to evaluate for each member of the input set.</param>
    /// <exception cref="T:System.ArgumentNullException">input or predicate is null.</exception>
    /// <exception cref="T:System.ArgumentException">The expression produced by predicate does not have a Boolean result type.</exception>
    public static DbQuantifierExpression Any(
      this DbExpressionBinding input,
      DbExpression predicate)
    {
      Check.NotNull<DbExpression>(predicate, nameof (predicate));
      Check.NotNull<DbExpressionBinding>(input, nameof (input));
      return new DbQuantifierExpression(DbExpressionKind.Any, ArgumentValidation.ValidateQuantifier(predicate), input, predicate);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbApplyExpression" /> that evaluates the given apply expression once for each element of a given input set, producing a collection of rows with corresponding input and apply columns. Rows for which apply evaluates to an empty set are not included.
    /// </summary>
    /// <returns>
    /// An new DbApplyExpression with the specified input and apply bindings and an
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionKind" />
    /// of CrossApply.
    /// </returns>
    /// <param name="input">
    /// An <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionBinding" /> that specifies the input set.
    /// </param>
    /// <param name="apply">
    /// An <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionBinding" /> that specifies logic to evaluate once for each member of the input set.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">input or apply is null.</exception>
    public static DbApplyExpression CrossApply(
      this DbExpressionBinding input,
      DbExpressionBinding apply)
    {
      Check.NotNull<DbExpressionBinding>(input, nameof (input));
      Check.NotNull<DbExpressionBinding>(apply, nameof (apply));
      DbExpressionBuilder.ValidateApply(input, apply);
      return new DbApplyExpression(DbExpressionKind.CrossApply, DbExpressionBuilder.CreateApplyResultType(input, apply), input, apply);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbApplyExpression" /> that evaluates the given apply expression once for each element of a given input set, producing a collection of rows with corresponding input and apply columns. Rows for which apply evaluates to an empty set have an apply column value of null.
    /// </summary>
    /// <returns>
    /// An new DbApplyExpression with the specified input and apply bindings and an
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionKind" />
    /// of OuterApply.
    /// </returns>
    /// <param name="input">
    /// An <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionBinding" /> that specifies the input set.
    /// </param>
    /// <param name="apply">
    /// An <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionBinding" /> that specifies logic to evaluate once for each member of the input set.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">input or apply is null.</exception>
    public static DbApplyExpression OuterApply(
      this DbExpressionBinding input,
      DbExpressionBinding apply)
    {
      Check.NotNull<DbExpressionBinding>(input, nameof (input));
      Check.NotNull<DbExpressionBinding>(apply, nameof (apply));
      DbExpressionBuilder.ValidateApply(input, apply);
      return new DbApplyExpression(DbExpressionKind.OuterApply, DbExpressionBuilder.CreateApplyResultType(input, apply), input, apply);
    }

    private static void ValidateApply(DbExpressionBinding input, DbExpressionBinding apply)
    {
      if (input.VariableName.Equals(apply.VariableName, StringComparison.Ordinal))
        throw new ArgumentException(Strings.Cqt_Apply_DuplicateVariableNames);
    }

    private static TypeUsage CreateApplyResultType(
      DbExpressionBinding input,
      DbExpressionBinding apply)
    {
      return ArgumentValidation.CreateCollectionOfRowResultType(new List<KeyValuePair<string, TypeUsage>>()
      {
        new KeyValuePair<string, TypeUsage>(input.VariableName, input.VariableType),
        new KeyValuePair<string, TypeUsage>(apply.VariableName, apply.VariableType)
      });
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbCrossJoinExpression" /> that unconditionally joins the sets specified by the list of input expression bindings.
    /// </summary>
    /// <returns>
    /// A new DbCrossJoinExpression, with an <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionKind" /> of CrossJoin, that represents the unconditional join of the input sets.
    /// </returns>
    /// <param name="inputs">A list of expression bindings that specifies the input sets.</param>
    /// <exception cref="T:System.ArgumentNullException">inputs is null or contains null element.</exception>
    /// <exception cref="T:System.ArgumentException">inputs contains fewer than 2 expression bindings.</exception>
    public static DbCrossJoinExpression CrossJoin(
      IEnumerable<DbExpressionBinding> inputs)
    {
      Check.NotNull<IEnumerable<DbExpressionBinding>>(inputs, nameof (inputs));
      TypeUsage resultType;
      ReadOnlyCollection<DbExpressionBinding> inputs1 = ArgumentValidation.ValidateCrossJoin(inputs, out resultType);
      return new DbCrossJoinExpression(resultType, inputs1);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbJoinExpression" /> that joins the sets specified by the left and right expression bindings, on the specified join condition, using InnerJoin as the
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionKind" />
    /// .
    /// </summary>
    /// <returns>
    /// A new DbJoinExpression, with an <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionKind" /> of InnerJoin, that represents the inner join operation applied to the left and right     input sets under the given join condition.
    /// </returns>
    /// <param name="left">
    /// An <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionBinding" /> that specifies the left set argument.
    /// </param>
    /// <param name="right">
    /// An <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionBinding" /> that specifies the right set argument.
    /// </param>
    /// <param name="joinCondition">An expression that specifies the condition on which to join.</param>
    /// <exception cref="T:System.ArgumentNullException">left, right or joinCondition is null.</exception>
    /// <exception cref="T:System.ArgumentException">joinCondition does not have a Boolean result type.</exception>
    public static DbJoinExpression InnerJoin(
      this DbExpressionBinding left,
      DbExpressionBinding right,
      DbExpression joinCondition)
    {
      Check.NotNull<DbExpressionBinding>(left, nameof (left));
      Check.NotNull<DbExpressionBinding>(right, nameof (right));
      Check.NotNull<DbExpression>(joinCondition, nameof (joinCondition));
      return new DbJoinExpression(DbExpressionKind.InnerJoin, ArgumentValidation.ValidateJoin(left, right, joinCondition), left, right, joinCondition);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbJoinExpression" /> that joins the sets specified by the left and right expression bindings, on the specified join condition, using LeftOuterJoin as the
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionKind" />
    /// .
    /// </summary>
    /// <returns>
    /// A new DbJoinExpression, with an <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionKind" /> of LeftOuterJoin, that represents the left outer join operation applied to the left and right     input sets under the given join condition.
    /// </returns>
    /// <param name="left">
    /// An <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionBinding" /> that specifies the left set argument.
    /// </param>
    /// <param name="right">
    /// An <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionBinding" /> that specifies the right set argument.
    /// </param>
    /// <param name="joinCondition">An expression that specifies the condition on which to join.</param>
    /// <exception cref="T:System.ArgumentNullException">left, right or joinCondition is null.</exception>
    /// <exception cref="T:System.ArgumentException">joinCondition does not have a Boolean result type.</exception>
    public static DbJoinExpression LeftOuterJoin(
      this DbExpressionBinding left,
      DbExpressionBinding right,
      DbExpression joinCondition)
    {
      Check.NotNull<DbExpressionBinding>(left, nameof (left));
      Check.NotNull<DbExpressionBinding>(right, nameof (right));
      Check.NotNull<DbExpression>(joinCondition, nameof (joinCondition));
      return new DbJoinExpression(DbExpressionKind.LeftOuterJoin, ArgumentValidation.ValidateJoin(left, right, joinCondition), left, right, joinCondition);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbJoinExpression" /> that joins the sets specified by the left and right expression bindings, on the specified join condition, using FullOuterJoin as the
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionKind" />
    /// .
    /// </summary>
    /// <returns>
    /// A new DbJoinExpression, with an <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionKind" /> of FullOuterJoin, that represents the full outer join operation applied to the left and right     input sets under the given join condition.
    /// </returns>
    /// <param name="left">
    /// An <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionBinding" /> that specifies the left set argument.
    /// </param>
    /// <param name="right">
    /// An <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionBinding" /> that specifies the right set argument.
    /// </param>
    /// <param name="joinCondition">An expression that specifies the condition on which to join.</param>
    /// <exception cref="T:System.ArgumentNullException">left, right or joinCondition is null.</exception>
    /// <exception cref="T:System.ArgumentException">The expression produced by joinCondition does not have a Boolean result type.</exception>
    public static DbJoinExpression FullOuterJoin(
      this DbExpressionBinding left,
      DbExpressionBinding right,
      DbExpression joinCondition)
    {
      Check.NotNull<DbExpressionBinding>(left, nameof (left));
      Check.NotNull<DbExpressionBinding>(right, nameof (right));
      Check.NotNull<DbExpression>(joinCondition, nameof (joinCondition));
      return new DbJoinExpression(DbExpressionKind.FullOuterJoin, ArgumentValidation.ValidateJoin(left, right, joinCondition), left, right, joinCondition);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFilterExpression" /> that filters the elements in the given input set using the specified predicate.
    /// </summary>
    /// <returns>A new DbFilterExpression that produces the filtered set.</returns>
    /// <param name="input">An expression binding that specifies the input set.</param>
    /// <param name="predicate">An expression representing a predicate to evaluate for each member of the input set.</param>
    /// <exception cref="T:System.ArgumentNullException">input or predicate is null.</exception>
    /// <exception cref="T:System.ArgumentException">predicate does not have a Boolean result type.</exception>
    public static DbFilterExpression Filter(
      this DbExpressionBinding input,
      DbExpression predicate)
    {
      Check.NotNull<DbExpressionBinding>(input, nameof (input));
      Check.NotNull<DbExpression>(predicate, nameof (predicate));
      return new DbFilterExpression(ArgumentValidation.ValidateFilter(input, predicate), input, predicate);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbGroupByExpression" /> that groups the elements of the input set according to the specified group keys and applies the given aggregates.
    /// </summary>
    /// <returns>A new DbGroupByExpression with the specified input set, grouping keys and aggregates.</returns>
    /// <param name="input">
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbGroupExpressionBinding" /> that specifies the input set.
    /// </param>
    /// <param name="keys">A list of string-expression pairs that define the grouping columns.</param>
    /// <param name="aggregates">A list of expressions that specify aggregates to apply.</param>
    /// <exception cref="T:System.ArgumentNullException">input, keys or aggregates is null, keys contains a null column key or expression, or aggregates contains a null aggregate column name or aggregate.</exception>
    /// <exception cref="T:System.ArgumentException">Both keys and aggregates are empty, or an invalid or duplicate column name was specified.</exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static DbGroupByExpression GroupBy(
      this DbGroupExpressionBinding input,
      IEnumerable<KeyValuePair<string, DbExpression>> keys,
      IEnumerable<KeyValuePair<string, DbAggregate>> aggregates)
    {
      Check.NotNull<DbGroupExpressionBinding>(input, nameof (input));
      Check.NotNull<IEnumerable<KeyValuePair<string, DbExpression>>>(keys, nameof (keys));
      Check.NotNull<IEnumerable<KeyValuePair<string, DbAggregate>>>(aggregates, nameof (aggregates));
      DbExpressionList validKeys;
      ReadOnlyCollection<DbAggregate> validAggregates;
      return new DbGroupByExpression(ArgumentValidation.ValidateGroupBy(keys, aggregates, out validKeys, out validAggregates), input, validKeys, validAggregates);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbProjectExpression" /> that projects the specified expression over the given input set.
    /// </summary>
    /// <returns>A new DbProjectExpression that represents the projection operation.</returns>
    /// <param name="input">An expression binding that specifies the input set.</param>
    /// <param name="projection">An expression to project over the set.</param>
    /// <exception cref="T:System.ArgumentNullException">input or projection is null.</exception>
    public static DbProjectExpression Project(
      this DbExpressionBinding input,
      DbExpression projection)
    {
      Check.NotNull<DbExpression>(projection, nameof (projection));
      Check.NotNull<DbExpressionBinding>(input, nameof (input));
      return new DbProjectExpression(DbExpressionBuilder.CreateCollectionResultType(projection.ResultType), input, projection);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbSkipExpression" /> that sorts the given input set by the given sort specifications before skipping the specified number of elements.
    /// </summary>
    /// <returns>A new DbSkipExpression that represents the skip operation.</returns>
    /// <param name="input">An expression binding that specifies the input set.</param>
    /// <param name="sortOrder">A list of sort specifications that determine how the elements of the input set should be sorted.</param>
    /// <param name="count">An expression the specifies how many elements of the ordered set to skip.</param>
    /// <exception cref="T:System.ArgumentNullException">input, sortOrder or count is null, or sortOrder contains null.</exception>
    /// <exception cref="T:System.ArgumentException">
    /// sortOrder is empty, or count is not <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbConstantExpression" /> or
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbParameterReferenceExpression" />
    /// or has a result type that is not equal or promotable to a 64-bit integer type.
    /// </exception>
    public static DbSkipExpression Skip(
      this DbExpressionBinding input,
      IEnumerable<DbSortClause> sortOrder,
      DbExpression count)
    {
      Check.NotNull<DbExpressionBinding>(input, nameof (input));
      Check.NotNull<IEnumerable<DbSortClause>>(sortOrder, nameof (sortOrder));
      Check.NotNull<DbExpression>(count, nameof (count));
      ReadOnlyCollection<DbSortClause> sortOrder1 = ArgumentValidation.ValidateSortArguments(sortOrder);
      if (!TypeSemantics.IsIntegerNumericType(count.ResultType))
        throw new ArgumentException(Strings.Cqt_Skip_IntegerRequired, nameof (count));
      if (count.ExpressionKind != DbExpressionKind.Constant && count.ExpressionKind != DbExpressionKind.ParameterReference)
        throw new ArgumentException(Strings.Cqt_Skip_ConstantOrParameterRefRequired, nameof (count));
      if (DbExpressionBuilder.IsConstantNegativeInteger(count))
        throw new ArgumentException(Strings.Cqt_Skip_NonNegativeCountRequired, nameof (count));
      return new DbSkipExpression(input.Expression.ResultType, input, sortOrder1, count);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbSortExpression" /> that sorts the given input set by the specified sort specifications.
    /// </summary>
    /// <returns>A new DbSortExpression that represents the sort operation.</returns>
    /// <param name="input">An expression binding that specifies the input set.</param>
    /// <param name="sortOrder">A list of sort specifications that determine how the elements of the input set should be sorted.</param>
    /// <exception cref="T:System.ArgumentNullException">input or sortOrder is null, or sortOrder contains null.</exception>
    /// <exception cref="T:System.ArgumentException">sortOrder is empty.</exception>
    public static DbSortExpression Sort(
      this DbExpressionBinding input,
      IEnumerable<DbSortClause> sortOrder)
    {
      Check.NotNull<DbExpressionBinding>(input, nameof (input));
      ReadOnlyCollection<DbSortClause> sortOrder1 = ArgumentValidation.ValidateSort(sortOrder);
      return new DbSortExpression(input.Expression.ResultType, input, sortOrder1);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbNullExpression" />, which represents a typed null value.
    /// </summary>
    /// <returns>An instance of DbNullExpression.</returns>
    /// <param name="nullType">The type of the null value.</param>
    /// <exception cref="T:System.ArgumentNullException">nullType is null.</exception>
    public static DbNullExpression Null(this TypeUsage nullType)
    {
      Check.NotNull<TypeUsage>(nullType, nameof (nullType));
      ArgumentValidation.CheckType(nullType, nameof (nullType));
      return new DbNullExpression(nullType);
    }

    /// <summary>
    /// Gets a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbConstantExpression" /> with the Boolean value true.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbConstantExpression" /> with the Boolean value true.
    /// </returns>
    public static DbConstantExpression True
    {
      get
      {
        return DbExpressionBuilder._boolTrue;
      }
    }

    /// <summary>
    /// Gets a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbConstantExpression" /> with the Boolean value false.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbConstantExpression" /> with the Boolean value false.
    /// </returns>
    public static DbConstantExpression False
    {
      get
      {
        return DbExpressionBuilder._boolFalse;
      }
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbConstantExpression" /> with the given constant value.
    /// </summary>
    /// <returns>A new DbConstantExpression with the given value.</returns>
    /// <param name="value">The constant value to represent.</param>
    /// <exception cref="T:System.ArgumentNullException">value is null.</exception>
    /// <exception cref="T:System.ArgumentException">value is not an instance of a valid constant type.</exception>
    public static DbConstantExpression Constant(object value)
    {
      Check.NotNull<object>(value, nameof (value));
      return new DbConstantExpression(ArgumentValidation.ValidateConstant(value), value);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbConstantExpression" /> of the specified primitive type with the given constant value.
    /// </summary>
    /// <returns>A new DbConstantExpression with the given value and a result type of constantType.</returns>
    /// <param name="constantType">The type of the constant value.</param>
    /// <param name="value">The constant value to represent.</param>
    /// <exception cref="T:System.ArgumentNullException">value or constantType is null.</exception>
    /// <exception cref="T:System.ArgumentException">value is not an instance of a valid constant type, constantType does not represent a primitive type, or value is of a different primitive type than that represented by constantType.</exception>
    public static DbConstantExpression Constant(
      this TypeUsage constantType,
      object value)
    {
      Check.NotNull<TypeUsage>(constantType, nameof (constantType));
      Check.NotNull<object>(value, nameof (value));
      ArgumentValidation.ValidateConstant(constantType, value);
      return new DbConstantExpression(constantType, value);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbParameterReferenceExpression" /> that references a parameter with the specified name and type.
    /// </summary>
    /// <returns>A DbParameterReferenceExpression that represents a reference to a parameter with the specified name and type. The result type of the expression will be the same as type.</returns>
    /// <param name="type">The type of the referenced parameter.</param>
    /// <param name="name">The name of the referenced parameter.</param>
    public static DbParameterReferenceExpression Parameter(
      this TypeUsage type,
      string name)
    {
      Check.NotNull<TypeUsage>(type, nameof (type));
      Check.NotNull<string>(name, nameof (name));
      ArgumentValidation.CheckType(type);
      if (!DbCommandTree.IsValidParameterName(name))
        throw new ArgumentException(Strings.Cqt_CommandTree_InvalidParameterName((object) name), nameof (name));
      return new DbParameterReferenceExpression(type, name);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbVariableReferenceExpression" /> that references a variable with the specified name and type.
    /// </summary>
    /// <returns>A DbVariableReferenceExpression that represents a reference to a variable with the specified name and type. The result type of the expression will be the same as type. </returns>
    /// <param name="type">The type of the referenced variable.</param>
    /// <param name="name">The name of the referenced variable.</param>
    public static DbVariableReferenceExpression Variable(
      this TypeUsage type,
      string name)
    {
      Check.NotNull<TypeUsage>(type, nameof (type));
      Check.NotNull<string>(name, nameof (name));
      Check.NotEmpty(name, nameof (name));
      ArgumentValidation.CheckType(type);
      return new DbVariableReferenceExpression(type, name);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbScanExpression" /> that references the specified entity or relationship set.
    /// </summary>
    /// <returns>A new DbScanExpression based on the specified entity or relationship set.</returns>
    /// <param name="targetSet">Metadata for the entity or relationship set to reference.</param>
    /// <exception cref="T:System.ArgumentNullException">targetSet is null.</exception>
    public static DbScanExpression Scan(this EntitySetBase targetSet)
    {
      Check.NotNull<EntitySetBase>(targetSet, nameof (targetSet));
      ArgumentValidation.CheckEntitySet(targetSet, nameof (targetSet));
      return new DbScanExpression(DbExpressionBuilder.CreateCollectionResultType((EdmType) targetSet.ElementType), targetSet);
    }

    /// <summary>
    /// Creates an <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbAndExpression" /> that performs the logical And of the left and right arguments.
    /// </summary>
    /// <returns>A new DbAndExpression with the specified arguments.</returns>
    /// <param name="left">A Boolean expression that specifies the left argument.</param>
    /// <param name="right">A Boolean expression that specifies the right argument.</param>
    /// <exception cref="T:System.ArgumentNullException">left or right is null.</exception>
    /// <exception cref="T:System.ArgumentException">left and right  does not have a Boolean result type.</exception>
    public static DbAndExpression And(this DbExpression left, DbExpression right)
    {
      Check.NotNull<DbExpression>(left, nameof (left));
      Check.NotNull<DbExpression>(right, nameof (right));
      TypeUsage commonTypeUsage = TypeHelpers.GetCommonTypeUsage(left.ResultType, right.ResultType);
      if (commonTypeUsage == null || !TypeSemantics.IsPrimitiveType(commonTypeUsage, PrimitiveTypeKind.Boolean))
        throw new ArgumentException(Strings.Cqt_And_BooleanArgumentsRequired);
      return new DbAndExpression(commonTypeUsage, left, right);
    }

    /// <summary>
    /// Creates an <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbOrExpression" /> that performs the logical Or of the left and right arguments.
    /// </summary>
    /// <returns>A new DbOrExpression with the specified arguments.</returns>
    /// <param name="left">A Boolean expression that specifies the left argument.</param>
    /// <param name="right">A Boolean expression that specifies the right argument.</param>
    /// <exception cref="T:System.ArgumentNullException">left or right is null.</exception>
    /// <exception cref="T:System.ArgumentException">left or right does not have a Boolean result type.</exception>
    public static DbOrExpression Or(this DbExpression left, DbExpression right)
    {
      Check.NotNull<DbExpression>(left, nameof (left));
      Check.NotNull<DbExpression>(right, nameof (right));
      TypeUsage commonTypeUsage = TypeHelpers.GetCommonTypeUsage(left.ResultType, right.ResultType);
      if (commonTypeUsage == null || !TypeSemantics.IsPrimitiveType(commonTypeUsage, PrimitiveTypeKind.Boolean))
        throw new ArgumentException(Strings.Cqt_Or_BooleanArgumentsRequired);
      return new DbOrExpression(commonTypeUsage, left, right);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbInExpression" /> that matches the result of the specified
    /// expression with the results of the constant expressions in the specified list.
    /// </summary>
    /// <param name="expression"> A DbExpression to be matched. </param>
    /// <param name="list"> A list of DbConstantExpression to test for a match. </param>
    /// <returns>A new DbInExpression with the specified arguments.</returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="expression" />
    /// or
    /// <paramref name="list" />
    /// is null.
    /// </exception>
    /// <exception cref="T:System.ArgumentException">
    /// The result type of
    /// <paramref name="expression" />
    /// is different than the result type of an expression from
    /// <paramref name="list" />.
    /// </exception>
    public static DbInExpression In(
      this DbExpression expression,
      IList<DbConstantExpression> list)
    {
      Check.NotNull<DbExpression>(expression, nameof (expression));
      Check.NotNull<IList<DbConstantExpression>>(list, nameof (list));
      List<DbExpression> dbExpressionList = new List<DbExpression>(list.Count);
      foreach (DbConstantExpression constantExpression in (IEnumerable<DbConstantExpression>) list)
      {
        if (!TypeSemantics.IsEqual(expression.ResultType, constantExpression.ResultType))
          throw new ArgumentException(Strings.Cqt_In_SameResultTypeRequired);
        dbExpressionList.Add((DbExpression) constantExpression);
      }
      return DbExpressionBuilder.CreateInExpression(expression, (IList<DbExpression>) dbExpressionList);
    }

    internal static DbInExpression CreateInExpression(
      DbExpression item,
      IList<DbExpression> list)
    {
      return new DbInExpression(DbExpressionBuilder._booleanType, item, new DbExpressionList(list));
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbNotExpression" /> that performs the logical negation of the given argument.
    /// </summary>
    /// <returns>A new DbNotExpression with the specified argument.</returns>
    /// <param name="argument">A Boolean expression that specifies the argument.</param>
    /// <exception cref="T:System.ArgumentNullException">argument is null.</exception>
    /// <exception cref="T:System.ArgumentException">argument does not have a Boolean result type.</exception>
    public static DbNotExpression Not(this DbExpression argument)
    {
      Check.NotNull<DbExpression>(argument, nameof (argument));
      if (!TypeSemantics.IsPrimitiveType(argument.ResultType, PrimitiveTypeKind.Boolean))
        throw new ArgumentException(Strings.Cqt_Not_BooleanArgumentRequired);
      return new DbNotExpression(argument.ResultType, argument);
    }

    private static DbArithmeticExpression CreateArithmetic(
      DbExpressionKind kind,
      DbExpression left,
      DbExpression right)
    {
      TypeUsage commonTypeUsage = TypeHelpers.GetCommonTypeUsage(left.ResultType, right.ResultType);
      if (commonTypeUsage == null || !TypeSemantics.IsNumericType(commonTypeUsage))
        throw new ArgumentException(Strings.Cqt_Arithmetic_NumericCommonType);
      DbExpressionList args = new DbExpressionList((IList<DbExpression>) new DbExpression[2]
      {
        left,
        right
      });
      return new DbArithmeticExpression(kind, commonTypeUsage, args);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbArithmeticExpression" /> that divides the left argument by the right argument.
    /// </summary>
    /// <returns>A new DbArithmeticExpression representing the division operation.</returns>
    /// <param name="left">An expression that specifies the left argument.</param>
    /// <param name="right">An expression that specifies the right argument.</param>
    /// <exception cref="T:System.ArgumentNullException">left or right is null.</exception>
    /// <exception cref="T:System.ArgumentException">No common numeric result type exists between left or right.</exception>
    public static DbArithmeticExpression Divide(
      this DbExpression left,
      DbExpression right)
    {
      Check.NotNull<DbExpression>(left, nameof (left));
      Check.NotNull<DbExpression>(right, nameof (right));
      return DbExpressionBuilder.CreateArithmetic(DbExpressionKind.Divide, left, right);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbArithmeticExpression" /> that subtracts the right argument from the left argument.
    /// </summary>
    /// <returns>A new DbArithmeticExpression representing the subtraction operation.</returns>
    /// <param name="left">An expression that specifies the left argument.</param>
    /// <param name="right">An expression that specifies the right argument.</param>
    /// <exception cref="T:System.ArgumentNullException">left or right is null.</exception>
    /// <exception cref="T:System.ArgumentException">No common numeric result type exists between left and right.</exception>
    public static DbArithmeticExpression Minus(
      this DbExpression left,
      DbExpression right)
    {
      Check.NotNull<DbExpression>(left, nameof (left));
      Check.NotNull<DbExpression>(right, nameof (right));
      return DbExpressionBuilder.CreateArithmetic(DbExpressionKind.Minus, left, right);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbArithmeticExpression" /> that computes the remainder of the left argument divided by the right argument.
    /// </summary>
    /// <returns>A new DbArithmeticExpression representing the modulo operation.</returns>
    /// <param name="left">An expression that specifies the left argument.</param>
    /// <param name="right">An expression that specifies the right argument.</param>
    /// <exception cref="T:System.ArgumentNullException">left or right is null.</exception>
    /// <exception cref="T:System.ArgumentException">No common numeric result type exists between left and right.</exception>
    public static DbArithmeticExpression Modulo(
      this DbExpression left,
      DbExpression right)
    {
      Check.NotNull<DbExpression>(left, nameof (left));
      Check.NotNull<DbExpression>(right, nameof (right));
      return DbExpressionBuilder.CreateArithmetic(DbExpressionKind.Modulo, left, right);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbArithmeticExpression" /> that multiplies the left argument by the right argument.
    /// </summary>
    /// <returns>A new DbArithmeticExpression representing the multiplication operation.</returns>
    /// <param name="left">An expression that specifies the left argument.</param>
    /// <param name="right">An expression that specifies the right argument.</param>
    /// <exception cref="T:System.ArgumentNullException">left or right is null.</exception>
    /// <exception cref="T:System.ArgumentException">No common numeric result type exists between left and right.</exception>
    public static DbArithmeticExpression Multiply(
      this DbExpression left,
      DbExpression right)
    {
      Check.NotNull<DbExpression>(left, nameof (left));
      Check.NotNull<DbExpression>(right, nameof (right));
      return DbExpressionBuilder.CreateArithmetic(DbExpressionKind.Multiply, left, right);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbArithmeticExpression" /> that adds the left argument to the right argument.
    /// </summary>
    /// <returns>A new DbArithmeticExpression representing the addition operation.</returns>
    /// <param name="left">An expression that specifies the left argument.</param>
    /// <param name="right">An expression that specifies the right argument.</param>
    /// <exception cref="T:System.ArgumentNullException">left or right is null.</exception>
    /// <exception cref="T:System.ArgumentException">No common numeric result type exists between left and right.</exception>
    public static DbArithmeticExpression Plus(
      this DbExpression left,
      DbExpression right)
    {
      Check.NotNull<DbExpression>(left, nameof (left));
      Check.NotNull<DbExpression>(right, nameof (right));
      return DbExpressionBuilder.CreateArithmetic(DbExpressionKind.Plus, left, right);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbArithmeticExpression" /> that negates the value of the argument.
    /// </summary>
    /// <returns>A new DbArithmeticExpression representing the negation operation.</returns>
    /// <param name="argument">An expression that specifies the argument.</param>
    /// <exception cref="T:System.ArgumentNullException">argument is null.</exception>
    /// <exception cref="T:System.ArgumentException">No numeric result type exists for argument.</exception>
    public static DbArithmeticExpression UnaryMinus(this DbExpression argument)
    {
      Check.NotNull<DbExpression>(argument, nameof (argument));
      TypeUsage promotableType = argument.ResultType;
      if (!TypeSemantics.IsNumericType(promotableType))
        throw new ArgumentException(Strings.Cqt_Arithmetic_NumericCommonType);
      if (TypeSemantics.IsUnsignedNumericType(argument.ResultType))
      {
        promotableType = (TypeUsage) null;
        if (!TypeHelpers.TryGetClosestPromotableType(argument.ResultType, out promotableType))
          throw new ArgumentException(Strings.Cqt_Arithmetic_InvalidUnsignedTypeForUnaryMinus((object) argument.ResultType.EdmType.FullName));
      }
      return new DbArithmeticExpression(DbExpressionKind.UnaryMinus, promotableType, new DbExpressionList((IList<DbExpression>) new DbExpression[1]
      {
        argument
      }));
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbArithmeticExpression" /> that negates the value of the argument.
    /// </summary>
    /// <returns>A new DbArithmeticExpression representing the negation operation.</returns>
    /// <param name="argument">An expression that specifies the argument.</param>
    /// <exception cref="T:System.ArgumentNullException">argument is null.</exception>
    /// <exception cref="T:System.ArgumentException">No numeric result type exists for argument.</exception>
    public static DbArithmeticExpression Negate(this DbExpression argument)
    {
      return argument.UnaryMinus();
    }

    private static DbComparisonExpression CreateComparison(
      DbExpressionKind kind,
      DbExpression left,
      DbExpression right)
    {
      bool flag1 = true;
      bool flag2 = true;
      if (DbExpressionKind.GreaterThanOrEquals == kind || DbExpressionKind.LessThanOrEquals == kind)
      {
        flag1 = TypeSemantics.IsEqualComparableTo(left.ResultType, right.ResultType);
        flag2 = TypeSemantics.IsOrderComparableTo(left.ResultType, right.ResultType);
      }
      else if (DbExpressionKind.Equals == kind || DbExpressionKind.NotEquals == kind)
        flag1 = TypeSemantics.IsEqualComparableTo(left.ResultType, right.ResultType);
      else
        flag2 = TypeSemantics.IsOrderComparableTo(left.ResultType, right.ResultType);
      if (!flag1 || !flag2)
        throw new ArgumentException(Strings.Cqt_Comparison_ComparableRequired);
      return new DbComparisonExpression(kind, DbExpressionBuilder._booleanType, left, right);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbComparisonExpression" /> that compares the left and right arguments for equality.
    /// </summary>
    /// <returns>A new DbComparisonExpression representing the equality comparison.</returns>
    /// <param name="left">An expression that specifies the left argument.</param>
    /// <param name="right">An expression that specifies the right argument.</param>
    /// <exception cref="T:System.ArgumentNullException">left or right is null.</exception>
    /// <exception cref="T:System.ArgumentException">No common equality-comparable result type exists between left and right.</exception>
    public static DbComparisonExpression Equal(
      this DbExpression left,
      DbExpression right)
    {
      Check.NotNull<DbExpression>(left, nameof (left));
      Check.NotNull<DbExpression>(right, nameof (right));
      return DbExpressionBuilder.CreateComparison(DbExpressionKind.Equals, left, right);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbComparisonExpression" /> that compares the left and right arguments for inequality.
    /// </summary>
    /// <returns>A new DbComparisonExpression representing the inequality comparison.</returns>
    /// <param name="left">An expression that specifies the left argument.</param>
    /// <param name="right">An expression that specifies the right argument.</param>
    /// <exception cref="T:System.ArgumentNullException">left or right is null.</exception>
    /// <exception cref="T:System.ArgumentException">No common equality-comparable result type exists between left and right.</exception>
    public static DbComparisonExpression NotEqual(
      this DbExpression left,
      DbExpression right)
    {
      Check.NotNull<DbExpression>(left, nameof (left));
      Check.NotNull<DbExpression>(right, nameof (right));
      return DbExpressionBuilder.CreateComparison(DbExpressionKind.NotEquals, left, right);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbComparisonExpression" /> that determines whether the left argument is greater than the right argument.
    /// </summary>
    /// <returns>A new DbComparisonExpression representing the greater-than comparison.</returns>
    /// <param name="left">An expression that specifies the left argument.</param>
    /// <param name="right">An expression that specifies the right argument.</param>
    /// <exception cref="T:System.ArgumentNullException">left or right is null.</exception>
    /// <exception cref="T:System.ArgumentException">No common order-comparable result type exists between left and right.</exception>
    public static DbComparisonExpression GreaterThan(
      this DbExpression left,
      DbExpression right)
    {
      Check.NotNull<DbExpression>(left, nameof (left));
      Check.NotNull<DbExpression>(right, nameof (right));
      return DbExpressionBuilder.CreateComparison(DbExpressionKind.GreaterThan, left, right);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbComparisonExpression" /> that determines whether the left argument is less than the right argument.
    /// </summary>
    /// <returns>A new DbComparisonExpression representing the less-than comparison.</returns>
    /// <param name="left">An expression that specifies the left argument.</param>
    /// <param name="right">An expression that specifies the right argument.</param>
    /// <exception cref="T:System.ArgumentNullException">left or right is null.</exception>
    /// <exception cref="T:System.ArgumentException">No common order-comparable result type exists between left and right.</exception>
    public static DbComparisonExpression LessThan(
      this DbExpression left,
      DbExpression right)
    {
      Check.NotNull<DbExpression>(left, nameof (left));
      Check.NotNull<DbExpression>(right, nameof (right));
      return DbExpressionBuilder.CreateComparison(DbExpressionKind.LessThan, left, right);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbComparisonExpression" /> that determines whether the left argument is greater than or equal to the right argument.
    /// </summary>
    /// <returns>A new DbComparisonExpression representing the greater-than-or-equal-to comparison.</returns>
    /// <param name="left">An expression that specifies the left argument.</param>
    /// <param name="right">An expression that specifies the right argument.</param>
    /// <exception cref="T:System.ArgumentNullException">left or right is null.</exception>
    /// <exception cref="T:System.ArgumentException">No common order-comparable result type exists between left and right.</exception>
    public static DbComparisonExpression GreaterThanOrEqual(
      this DbExpression left,
      DbExpression right)
    {
      Check.NotNull<DbExpression>(left, nameof (left));
      Check.NotNull<DbExpression>(right, nameof (right));
      return DbExpressionBuilder.CreateComparison(DbExpressionKind.GreaterThanOrEquals, left, right);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbComparisonExpression" /> that determines whether the left argument is less than or equal to the right argument.
    /// </summary>
    /// <returns>A new DbComparisonExpression representing the less-than-or-equal-to comparison.</returns>
    /// <param name="left">An expression that specifies the left argument.</param>
    /// <param name="right">An expression that specifies the right argument.</param>
    /// <exception cref="T:System.ArgumentNullException">left or right is null.</exception>
    /// <exception cref="T:System.ArgumentException">No common result type that is both equality- and order-comparable exists between left and right.</exception>
    public static DbComparisonExpression LessThanOrEqual(
      this DbExpression left,
      DbExpression right)
    {
      Check.NotNull<DbExpression>(left, nameof (left));
      Check.NotNull<DbExpression>(right, nameof (right));
      return DbExpressionBuilder.CreateComparison(DbExpressionKind.LessThanOrEquals, left, right);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbIsNullExpression" /> that determines whether the specified argument is null.
    /// </summary>
    /// <returns>A new DbIsNullExpression with the specified argument.</returns>
    /// <param name="argument">An expression that specifies the argument.</param>
    /// <exception cref="T:System.ArgumentNullException">argument is null.</exception>
    /// <exception cref="T:System.ArgumentException">argument has a collection result type.</exception>
    public static DbIsNullExpression IsNull(this DbExpression argument)
    {
      Check.NotNull<DbExpression>(argument, nameof (argument));
      DbExpressionBuilder.ValidateIsNull(argument);
      return new DbIsNullExpression(DbExpressionBuilder._booleanType, argument);
    }

    private static void ValidateIsNull(DbExpression argument)
    {
      if (TypeSemantics.IsCollectionType(argument.ResultType))
        throw new ArgumentException(Strings.Cqt_IsNull_CollectionNotAllowed);
      if (!TypeHelpers.IsValidIsNullOpType(argument.ResultType))
        throw new ArgumentException(Strings.Cqt_IsNull_InvalidType);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbLikeExpression" /> that compares the specified input string to the given pattern.
    /// </summary>
    /// <returns>A new DbLikeExpression with the specified input, pattern and a null escape.</returns>
    /// <param name="argument">An expression that specifies the input string.</param>
    /// <param name="pattern">An expression that specifies the pattern string.</param>
    /// <exception cref="T:System.ArgumentNullException">Argument or pattern is null.</exception>
    /// <exception cref="T:System.ArgumentException">Argument or pattern does not have a string result type.</exception>
    public static DbLikeExpression Like(
      this DbExpression argument,
      DbExpression pattern)
    {
      Check.NotNull<DbExpression>(argument, nameof (argument));
      Check.NotNull<DbExpression>(pattern, nameof (pattern));
      DbExpressionBuilder.ValidateLike(argument, pattern);
      DbExpression escape = (DbExpression) pattern.ResultType.Null();
      return new DbLikeExpression(DbExpressionBuilder._booleanType, argument, pattern, escape);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbLikeExpression" /> that compares the specified input string to the given pattern using the optional escape.
    /// </summary>
    /// <returns>A new DbLikeExpression with the specified input, pattern and escape.</returns>
    /// <param name="argument">An expression that specifies the input string.</param>
    /// <param name="pattern">An expression that specifies the pattern string.</param>
    /// <param name="escape">An optional expression that specifies the escape string.</param>
    /// <exception cref="T:System.ArgumentNullException">argument,  pattern or escape is null.</exception>
    /// <exception cref="T:System.ArgumentException">argument,  pattern or escape does not have a string result type.</exception>
    public static DbLikeExpression Like(
      this DbExpression argument,
      DbExpression pattern,
      DbExpression escape)
    {
      Check.NotNull<DbExpression>(argument, nameof (argument));
      Check.NotNull<DbExpression>(pattern, nameof (pattern));
      Check.NotNull<DbExpression>(escape, nameof (escape));
      DbExpressionBuilder.ValidateLike(argument, pattern, escape);
      return new DbLikeExpression(DbExpressionBuilder._booleanType, argument, pattern, escape);
    }

    private static void ValidateLike(
      DbExpression argument,
      DbExpression pattern,
      DbExpression escape)
    {
      DbExpressionBuilder.ValidateLike(argument, pattern);
      ArgumentValidation.RequireCompatibleType(escape, PrimitiveTypeKind.String, nameof (escape));
    }

    private static void ValidateLike(DbExpression argument, DbExpression pattern)
    {
      ArgumentValidation.RequireCompatibleType(argument, PrimitiveTypeKind.String, nameof (argument));
      ArgumentValidation.RequireCompatibleType(pattern, PrimitiveTypeKind.String, nameof (pattern));
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbCastExpression" /> that applies a cast operation to a polymorphic argument.
    /// </summary>
    /// <returns>A new DbCastExpression with the specified argument and target type.</returns>
    /// <param name="argument">The argument to which the cast should be applied.</param>
    /// <param name="toType">Type metadata that specifies the type to cast to.</param>
    /// <exception cref="T:System.ArgumentNullException">Argument or toType is null.</exception>
    /// <exception cref="T:System.ArgumentException">The specified cast is not valid.</exception>
    public static DbCastExpression CastTo(
      this DbExpression argument,
      TypeUsage toType)
    {
      Check.NotNull<DbExpression>(argument, nameof (argument));
      Check.NotNull<TypeUsage>(toType, nameof (toType));
      ArgumentValidation.CheckType(toType, nameof (toType));
      if (!TypeSemantics.IsCastAllowed(argument.ResultType, toType))
        throw new ArgumentException(Strings.Cqt_Cast_InvalidCast((object) argument.ResultType.ToString(), (object) toType.ToString()));
      return new DbCastExpression(toType, argument);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbTreatExpression" />.
    /// </summary>
    /// <returns>A new DbTreatExpression with the specified argument and type.</returns>
    /// <param name="argument">An expression that specifies the instance.</param>
    /// <param name="treatType">Type metadata for the treat-as type.</param>
    /// <exception cref="T:System.ArgumentNullException">argument or treatType is null.</exception>
    /// <exception cref="T:System.ArgumentException">treatType is not in the same type hierarchy as the result type of argument.</exception>
    public static DbTreatExpression TreatAs(
      this DbExpression argument,
      TypeUsage treatType)
    {
      Check.NotNull<DbExpression>(argument, nameof (argument));
      Check.NotNull<TypeUsage>(treatType, nameof (treatType));
      ArgumentValidation.CheckType(treatType, nameof (treatType));
      ArgumentValidation.RequirePolymorphicType(treatType);
      if (!TypeSemantics.IsValidPolymorphicCast(argument.ResultType, treatType))
        throw new ArgumentException(Strings.Cqt_General_PolymorphicArgRequired((object) typeof (DbTreatExpression).Name));
      return new DbTreatExpression(treatType, argument);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbOfTypeExpression" /> that produces a set consisting of the elements of the given input set that are of the specified type.
    /// </summary>
    /// <returns>
    /// A new DbOfTypeExpression with the specified set argument and type, and an ExpressionKind of
    /// <see cref="F:System.Data.Entity.Core.Common.CommandTrees.DbExpressionKind.OfType" />
    /// .
    /// </returns>
    /// <param name="argument">
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that specifies the input set.
    /// </param>
    /// <param name="type">Type metadata for the type that elements of the input set must have to be included in the resulting set.</param>
    /// <exception cref="T:System.ArgumentNullException">argument or type is null.</exception>
    /// <exception cref="T:System.ArgumentException">argument does not have a collection result type, or type is not a type in the same type hierarchy as the element type of the collection result type of argument.</exception>
    public static DbOfTypeExpression OfType(
      this DbExpression argument,
      TypeUsage type)
    {
      Check.NotNull<DbExpression>(argument, nameof (argument));
      Check.NotNull<TypeUsage>(type, nameof (type));
      DbExpressionBuilder.ValidateOfType(argument, type);
      return new DbOfTypeExpression(DbExpressionKind.OfType, DbExpressionBuilder.CreateCollectionResultType(type), argument, type);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbOfTypeExpression" /> that produces a set consisting of the elements of the given input set that are of exactly the specified type.
    /// </summary>
    /// <returns>
    /// A new DbOfTypeExpression with the specified set argument and type, and an ExpressionKind of
    /// <see cref="F:System.Data.Entity.Core.Common.CommandTrees.DbExpressionKind.OfTypeOnly" />
    /// .
    /// </returns>
    /// <param name="argument">
    /// An <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that specifies the input set.
    /// </param>
    /// <param name="type">Type metadata for the type that elements of the input set must match exactly to be included in the resulting set.</param>
    /// <exception cref="T:System.ArgumentNullException">argument or type is null.</exception>
    /// <exception cref="T:System.ArgumentException">argument does not have a collection result type, or type is not a type in the same type hierarchy as the element type of the collection result type of argument.</exception>
    public static DbOfTypeExpression OfTypeOnly(
      this DbExpression argument,
      TypeUsage type)
    {
      Check.NotNull<DbExpression>(argument, nameof (argument));
      Check.NotNull<TypeUsage>(type, nameof (type));
      DbExpressionBuilder.ValidateOfType(argument, type);
      return new DbOfTypeExpression(DbExpressionKind.OfTypeOnly, DbExpressionBuilder.CreateCollectionResultType(type), argument, type);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbIsOfExpression" /> that determines whether the given argument is of the specified type or a subtype.
    /// </summary>
    /// <returns>A new DbIsOfExpression with the specified instance and type and DbExpressionKind IsOf.</returns>
    /// <param name="argument">An expression that specifies the instance.</param>
    /// <param name="type">Type metadata that specifies the type that the instance's result type should be compared to.</param>
    /// <exception cref="T:System.ArgumentNullException">argument or type is null.</exception>
    /// <exception cref="T:System.ArgumentException">type is not in the same type hierarchy as the result type of argument.</exception>
    public static DbIsOfExpression IsOf(this DbExpression argument, TypeUsage type)
    {
      Check.NotNull<DbExpression>(argument, nameof (argument));
      Check.NotNull<TypeUsage>(type, nameof (type));
      DbExpressionBuilder.ValidateIsOf(argument, type);
      return new DbIsOfExpression(DbExpressionKind.IsOf, DbExpressionBuilder._booleanType, argument, type);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbIsOfExpression" /> expression that determines whether the given argument is of the specified type, and only that type (not a subtype).
    /// </summary>
    /// <returns>A new DbIsOfExpression with the specified instance and type and DbExpressionKind IsOfOnly.</returns>
    /// <param name="argument">An expression that specifies the instance.</param>
    /// <param name="type">Type metadata that specifies the type that the instance's result type should be compared to.</param>
    /// <exception cref="T:System.ArgumentNullException">argument or type is null.</exception>
    /// <exception cref="T:System.ArgumentException">type is not in the same type hierarchy as the result type of argument.</exception>
    public static DbIsOfExpression IsOfOnly(
      this DbExpression argument,
      TypeUsage type)
    {
      Check.NotNull<DbExpression>(argument, nameof (argument));
      Check.NotNull<TypeUsage>(type, nameof (type));
      DbExpressionBuilder.ValidateIsOf(argument, type);
      return new DbIsOfExpression(DbExpressionKind.IsOfOnly, DbExpressionBuilder._booleanType, argument, type);
    }

    private static void ValidateOfType(DbExpression argument, TypeUsage type)
    {
      ArgumentValidation.CheckType(type, nameof (type));
      ArgumentValidation.RequirePolymorphicType(type);
      ArgumentValidation.RequireCollectionArgument<DbOfTypeExpression>(argument);
      TypeUsage elementType = (TypeUsage) null;
      if (!TypeHelpers.TryGetCollectionElementType(argument.ResultType, out elementType) || !TypeSemantics.IsValidPolymorphicCast(elementType, type))
        throw new ArgumentException(Strings.Cqt_General_PolymorphicArgRequired((object) typeof (DbOfTypeExpression).Name));
    }

    private static void ValidateIsOf(DbExpression argument, TypeUsage type)
    {
      ArgumentValidation.CheckType(type, nameof (type));
      ArgumentValidation.RequirePolymorphicType(type);
      if (!TypeSemantics.IsValidPolymorphicCast(argument.ResultType, type))
        throw new ArgumentException(Strings.Cqt_General_PolymorphicArgRequired((object) typeof (DbIsOfExpression).Name));
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbDerefExpression" /> that retrieves a specific Entity given a reference expression.
    /// </summary>
    /// <returns>A new DbDerefExpression that retrieves the specified Entity.</returns>
    /// <param name="argument">
    /// An <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that provides the reference. This expression must have a reference Type.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">argument is null.</exception>
    /// <exception cref="T:System.ArgumentException">argument does not have a reference result type.</exception>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Deref")]
    public static DbDerefExpression Deref(this DbExpression argument)
    {
      Check.NotNull<DbExpression>(argument, nameof (argument));
      EntityType referencedEntityType;
      if (!TypeHelpers.TryGetRefEntityType(argument.ResultType, out referencedEntityType))
        throw new ArgumentException(Strings.Cqt_DeRef_RefRequired, nameof (argument));
      return new DbDerefExpression(TypeUsage.Create((EdmType) referencedEntityType), argument);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbEntityRefExpression" /> that retrieves the ref of the specifed entity in structural form.
    /// </summary>
    /// <returns>A new DbEntityRefExpression that retrieves a reference to the specified entity.</returns>
    /// <param name="argument">The expression that provides the entity. This expression must have an entity result type.</param>
    /// <exception cref="T:System.ArgumentNullException">argument is null.</exception>
    /// <exception cref="T:System.ArgumentException">argument does not have an entity result type.</exception>
    public static DbEntityRefExpression GetEntityRef(this DbExpression argument)
    {
      Check.NotNull<DbExpression>(argument, nameof (argument));
      EntityType type = (EntityType) null;
      if (!TypeHelpers.TryGetEdmType<EntityType>(argument.ResultType, out type))
        throw new ArgumentException(Strings.Cqt_GetEntityRef_EntityRequired, nameof (argument));
      return new DbEntityRefExpression(ArgumentValidation.CreateReferenceResultType((EntityTypeBase) type), argument);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbRefExpression" /> that encodes a reference to a specific entity based on key values.
    /// </summary>
    /// <returns>A new DbRefExpression that references the element with the specified key values in the given entity set.</returns>
    /// <param name="entitySet">The entity set in which the referenced element resides.</param>
    /// <param name="keyValues">
    /// A collection of <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" />s that provide the key values. These expressions must match (in number, type, and order) the key properties of the referenced entity type.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">entitySet is null, or keyValues is null or contains null.</exception>
    /// <exception cref="T:System.ArgumentException">The count of keyValues does not match the count of key members declared by the entitySet’s element type, or keyValues contains an expression with a result type that is not compatible with the type of the corresponding key member.</exception>
    public static DbRefExpression CreateRef(
      this EntitySet entitySet,
      IEnumerable<DbExpression> keyValues)
    {
      Check.NotNull<EntitySet>(entitySet, nameof (entitySet));
      Check.NotNull<IEnumerable<DbExpression>>(keyValues, nameof (keyValues));
      return DbExpressionBuilder.CreateRefExpression(entitySet, keyValues);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbRefExpression" /> that encodes a reference to a specific entity based on key values.
    /// </summary>
    /// <returns>A new DbRefExpression that references the element with the specified key values in the given entity set.</returns>
    /// <param name="entitySet">The entity set in which the referenced element resides.</param>
    /// <param name="keyValues">
    /// A collection of <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" />s that provide the key values. These expressions must match (in number, type, and order) the key properties of the referenced entity type.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">entitySet is null, or keyValues is null or contains null.</exception>
    /// <exception cref="T:System.ArgumentException">The count of keyValues does not match the count of key members declared by the entitySet’s element type, or keyValues contains an expression with a result type that is not compatible with the type of the corresponding key member.</exception>
    public static DbRefExpression CreateRef(
      this EntitySet entitySet,
      params DbExpression[] keyValues)
    {
      Check.NotNull<EntitySet>(entitySet, nameof (entitySet));
      Check.NotNull<DbExpression[]>(keyValues, nameof (keyValues));
      return DbExpressionBuilder.CreateRefExpression(entitySet, (IEnumerable<DbExpression>) keyValues);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbRefExpression" /> that encodes a reference to a specific entity of a given type based on key values.
    /// </summary>
    /// <returns>A new DbRefExpression that references the element with the specified key values in the given entity set.</returns>
    /// <param name="entitySet">The entity set in which the referenced element resides.</param>
    /// <param name="entityType">The specific type of the referenced entity. This must be an entity type from the same hierarchy as the entity set's element type.</param>
    /// <param name="keyValues">
    /// A collection of <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" />s that provide the key values. These expressions must match (in number, type, and order) the key properties of the referenced entity type.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">entitySet or entityType is null, or keyValues is null or contains null.</exception>
    /// <exception cref="T:System.ArgumentException">entityType is not from the same type hierarchy (a subtype, supertype, or the same type) as entitySet's element type.</exception>
    /// <exception cref="T:System.ArgumentException">The count of keyValues does not match the count of key members declared by the entitySet’s element type, or keyValues contains an expression with a result type that is not compatible with the type of the corresponding key member.</exception>
    public static DbRefExpression CreateRef(
      this EntitySet entitySet,
      EntityType entityType,
      IEnumerable<DbExpression> keyValues)
    {
      Check.NotNull<EntitySet>(entitySet, nameof (entitySet));
      Check.NotNull<EntityType>(entityType, nameof (entityType));
      Check.NotNull<IEnumerable<DbExpression>>(keyValues, nameof (keyValues));
      return DbExpressionBuilder.CreateRefExpression(entitySet, entityType, keyValues);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbRefExpression" /> that encodes a reference to a specific entity of a given type based on key values.
    /// </summary>
    /// <returns>A new DbRefExpression that references the element with the specified key values in the given entity set.</returns>
    /// <param name="entitySet">The entity set in which the referenced element resides.</param>
    /// <param name="entityType">The specific type of the referenced entity. This must be an entity type from the same hierarchy as the entity set's element type.</param>
    /// <param name="keyValues">
    /// A collection of <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" />s that provide the key values. These expressions must match (in number, type, and order) the key properties of the referenced entity type.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">entitySet or entityType is null, or keyValues is null or contains null.</exception>
    /// <exception cref="T:System.ArgumentException">entityType is not from the same type hierarchy (a subtype, supertype, or the same type) as entitySet's element type.</exception>
    /// <exception cref="T:System.ArgumentException">The count of keyValues does not match the count of key members declared by the entitySet’s element type, or keyValues contains an expression with a result type that is not compatible with the type of the corresponding key member.</exception>
    public static DbRefExpression CreateRef(
      this EntitySet entitySet,
      EntityType entityType,
      params DbExpression[] keyValues)
    {
      Check.NotNull<EntitySet>(entitySet, nameof (entitySet));
      Check.NotNull<EntityType>(entityType, nameof (entityType));
      Check.NotNull<DbExpression[]>(keyValues, nameof (keyValues));
      return DbExpressionBuilder.CreateRefExpression(entitySet, entityType, (IEnumerable<DbExpression>) keyValues);
    }

    private static DbRefExpression CreateRefExpression(
      EntitySet entitySet,
      IEnumerable<DbExpression> keyValues)
    {
      DbExpression keyConstructor;
      return new DbRefExpression(ArgumentValidation.ValidateCreateRef(entitySet, entitySet.ElementType, keyValues, out keyConstructor), entitySet, keyConstructor);
    }

    private static DbRefExpression CreateRefExpression(
      EntitySet entitySet,
      EntityType entityType,
      IEnumerable<DbExpression> keyValues)
    {
      Check.NotNull<EntitySet>(entitySet, nameof (entitySet));
      Check.NotNull<EntityType>(entityType, nameof (entityType));
      DbExpression keyConstructor;
      return new DbRefExpression(ArgumentValidation.ValidateCreateRef(entitySet, entityType, keyValues, out keyConstructor), entitySet, keyConstructor);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbRefExpression" /> that encodes a reference to a specific Entity based on key values.
    /// </summary>
    /// <returns>A new DbRefExpression that references the element with the specified key values in the given Entity set.</returns>
    /// <param name="entitySet">The Entity set in which the referenced element resides.</param>
    /// <param name="keyRow">
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that constructs a record with columns that match (in number, type, and order) the Key properties of the referenced Entity type.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">entitySet or keyRow is null.</exception>
    /// <exception cref="T:System.ArgumentException">keyRow does not have a record result type that matches the key properties of the referenced entity set's entity type.</exception>
    public static DbRefExpression RefFromKey(
      this EntitySet entitySet,
      DbExpression keyRow)
    {
      Check.NotNull<EntitySet>(entitySet, nameof (entitySet));
      Check.NotNull<DbExpression>(keyRow, nameof (keyRow));
      return new DbRefExpression(ArgumentValidation.ValidateRefFromKey(entitySet, keyRow, entitySet.ElementType), entitySet, keyRow);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbRefExpression" /> that encodes a reference to a specific Entity based on key values.
    /// </summary>
    /// <returns>A new DbRefExpression that references the element with the specified key values in the given Entity set.</returns>
    /// <param name="entitySet">The Entity set in which the referenced element resides.</param>
    /// <param name="keyRow">
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that constructs a record with columns that match (in number, type, and order) the Key properties of the referenced Entity type.
    /// </param>
    /// <param name="entityType">The type of the Entity that the reference should refer to.</param>
    /// <exception cref="T:System.ArgumentNullException">entitySet, keyRow or entityType is null.</exception>
    /// <exception cref="T:System.ArgumentException">entityType is not in the same type hierarchy as the entity set's entity type, or keyRow does not have a record result type that matches the key properties of the referenced entity set's entity type.</exception>
    public static DbRefExpression RefFromKey(
      this EntitySet entitySet,
      DbExpression keyRow,
      EntityType entityType)
    {
      Check.NotNull<EntitySet>(entitySet, nameof (entitySet));
      Check.NotNull<DbExpression>(keyRow, nameof (keyRow));
      Check.NotNull<EntityType>(entityType, nameof (entityType));
      return new DbRefExpression(ArgumentValidation.ValidateRefFromKey(entitySet, keyRow, entityType), entitySet, keyRow);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbRefKeyExpression" /> that retrieves the key values of the specifed reference in structural form.
    /// </summary>
    /// <returns>A new DbRefKeyExpression that retrieves the key values of the specified reference.</returns>
    /// <param name="argument">The expression that provides the reference. This expression must have a reference Type with an Entity element type.</param>
    /// <exception cref="T:System.ArgumentNullException">argument is null.</exception>
    /// <exception cref="T:System.ArgumentException">argument does not have a reference result type.</exception>
    public static DbRefKeyExpression GetRefKey(this DbExpression argument)
    {
      Check.NotNull<DbExpression>(argument, nameof (argument));
      RefType type = (RefType) null;
      if (!TypeHelpers.TryGetEdmType<RefType>(argument.ResultType, out type))
        throw new ArgumentException(Strings.Cqt_GetRefKey_RefRequired, nameof (argument));
      return new DbRefKeyExpression(TypeUsage.Create((EdmType) TypeHelpers.CreateKeyRowType(type.ElementType)), argument);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbRelationshipNavigationExpression" /> representing the navigation of a composition or association relationship.
    /// </summary>
    /// <returns>A new DbRelationshipNavigationExpression representing the navigation of the specified from and to relation ends of the specified relation type from the specified navigation source instance.</returns>
    /// <param name="navigateFrom">An expression that specifies the instance from which navigation should occur.</param>
    /// <param name="fromEnd">Metadata for the property that represents the end of the relationship from which navigation should occur.</param>
    /// <param name="toEnd">Metadata for the property that represents the end of the relationship to which navigation should occur.</param>
    /// <exception cref="T:System.ArgumentNullException">fromEnd, toEnd or navigateFrom is null.</exception>
    /// <exception cref="T:System.ArgumentException">fromEnd and toEnd are not declared by the same relationship type, or navigateFrom has a result type that is not compatible with the property type of fromEnd.</exception>
    public static DbRelationshipNavigationExpression Navigate(
      this DbExpression navigateFrom,
      RelationshipEndMember fromEnd,
      RelationshipEndMember toEnd)
    {
      Check.NotNull<DbExpression>(navigateFrom, nameof (navigateFrom));
      Check.NotNull<RelationshipEndMember>(fromEnd, nameof (fromEnd));
      Check.NotNull<RelationshipEndMember>(toEnd, nameof (toEnd));
      RelationshipType relType;
      return new DbRelationshipNavigationExpression(ArgumentValidation.ValidateNavigate(navigateFrom, fromEnd, toEnd, out relType, false), relType, fromEnd, toEnd, navigateFrom);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbRelationshipNavigationExpression" /> representing the navigation of a composition or association relationship.
    /// </summary>
    /// <returns>A new DbRelationshipNavigationExpression representing the navigation of the specified from and to relation ends of the specified relation type from the specified navigation source instance.</returns>
    /// <param name="type">Metadata for the relation type that represents the relationship.</param>
    /// <param name="fromEndName">The name of the property of the relation type that represents the end of the relationship from which navigation should occur.</param>
    /// <param name="toEndName">The name of the property of the relation type that represents the end of the relationship to which navigation should occur.</param>
    /// <param name="navigateFrom">An expression the specifies the instance from which naviagtion should occur.</param>
    /// <exception cref="T:System.ArgumentNullException">type, fromEndName, toEndName or navigateFrom is null.</exception>
    /// <exception cref="T:System.ArgumentException">type is not associated with this command tree's metadata workspace or navigateFrom is associated with a different command tree, or type does not declare a relation end property with name toEndName or fromEndName, or navigateFrom has a result type that is not compatible with the property type of the relation end property with name fromEndName.</exception>
    public static DbRelationshipNavigationExpression Navigate(
      this RelationshipType type,
      string fromEndName,
      string toEndName,
      DbExpression navigateFrom)
    {
      Check.NotNull<RelationshipType>(type, nameof (type));
      Check.NotNull<string>(fromEndName, nameof (fromEndName));
      Check.NotNull<string>(toEndName, nameof (toEndName));
      Check.NotNull<DbExpression>(navigateFrom, nameof (navigateFrom));
      RelationshipEndMember fromEnd;
      RelationshipEndMember toEnd;
      return new DbRelationshipNavigationExpression(ArgumentValidation.ValidateNavigate(navigateFrom, type, fromEndName, toEndName, out fromEnd, out toEnd), type, fromEnd, toEnd, navigateFrom);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbDistinctExpression" /> that removes duplicates from the given set argument.
    /// </summary>
    /// <returns>A new DbDistinctExpression that represents the distinct operation applied to the specified set argument.</returns>
    /// <param name="argument">An expression that defines the set over which to perfom the distinct operation.</param>
    /// <exception cref="T:System.ArgumentNullException">argument is null.</exception>
    /// <exception cref="T:System.ArgumentException">argument does not have a collection result type.</exception>
    public static DbDistinctExpression Distinct(this DbExpression argument)
    {
      Check.NotNull<DbExpression>(argument, nameof (argument));
      ArgumentValidation.RequireCollectionArgument<DbDistinctExpression>(argument);
      if (!TypeHelpers.IsValidDistinctOpType(TypeHelpers.GetEdmType<CollectionType>(argument.ResultType).TypeUsage))
        throw new ArgumentException(Strings.Cqt_Distinct_InvalidCollection, nameof (argument));
      return new DbDistinctExpression(argument.ResultType, argument);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbElementExpression" /> that converts a set into a singleton.
    /// </summary>
    /// <returns>A DbElementExpression that represents the conversion of the set argument to a singleton.</returns>
    /// <param name="argument">An expression that specifies the input set.</param>
    /// <exception cref="T:System.ArgumentNullException">argument is null.</exception>
    /// <exception cref="T:System.ArgumentException">argument does not have a collection result type.</exception>
    public static DbElementExpression Element(this DbExpression argument)
    {
      Check.NotNull<DbExpression>(argument, nameof (argument));
      return new DbElementExpression(ArgumentValidation.ValidateElement(argument), argument);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbIsEmptyExpression" /> that determines whether the specified set argument is an empty set.
    /// </summary>
    /// <returns>A new DbIsEmptyExpression with the specified argument.</returns>
    /// <param name="argument">An expression that specifies the input set.</param>
    /// <exception cref="T:System.ArgumentNullException">argument is null.</exception>
    /// <exception cref="T:System.ArgumentException">argument does not have a collection result type.</exception>
    public static DbIsEmptyExpression IsEmpty(this DbExpression argument)
    {
      Check.NotNull<DbExpression>(argument, nameof (argument));
      ArgumentValidation.RequireCollectionArgument<DbIsEmptyExpression>(argument);
      return new DbIsEmptyExpression(DbExpressionBuilder._booleanType, argument);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExceptExpression" /> that computes the subtraction of the right set argument from the left set argument.
    /// </summary>
    /// <returns>A new DbExceptExpression that represents the difference of the left argument from the right argument.</returns>
    /// <param name="left">An expression that defines the left set argument.</param>
    /// <param name="right">An expression that defines the right set argument.</param>
    /// <exception cref="T:System.ArgumentNullException">left or right is null.</exception>
    /// <exception cref="T:System.ArgumentException">No common collection result type exists between left and right.</exception>
    public static DbExceptExpression Except(
      this DbExpression left,
      DbExpression right)
    {
      Check.NotNull<DbExpression>(left, nameof (left));
      Check.NotNull<DbExpression>(right, nameof (right));
      ArgumentValidation.RequireComparableCollectionArguments<DbExceptExpression>(left, right);
      return new DbExceptExpression(left.ResultType, left, right);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbIntersectExpression" /> that computes the intersection of the left and right set arguments.
    /// </summary>
    /// <returns>A new DbIntersectExpression that represents the intersection of the left and right arguments.</returns>
    /// <param name="left">An expression that defines the left set argument.</param>
    /// <param name="right">An expression that defines the right set argument.</param>
    /// <exception cref="T:System.ArgumentNullException">left or right is null.</exception>
    /// <exception cref="T:System.ArgumentException">No common collection result type exists between left or right.</exception>
    public static DbIntersectExpression Intersect(
      this DbExpression left,
      DbExpression right)
    {
      Check.NotNull<DbExpression>(left, nameof (left));
      Check.NotNull<DbExpression>(right, nameof (right));
      return new DbIntersectExpression(ArgumentValidation.RequireComparableCollectionArguments<DbIntersectExpression>(left, right), left, right);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbUnionAllExpression" /> that computes the union of the left and right set arguments and does not remove duplicates.
    /// </summary>
    /// <returns>A new DbUnionAllExpression that union, including duplicates, of the the left and right arguments.</returns>
    /// <param name="left">An expression that defines the left set argument.</param>
    /// <param name="right">An expression that defines the right set argument.</param>
    /// <exception cref="T:System.ArgumentNullException">left or right is null.</exception>
    /// <exception cref="T:System.ArgumentException">No common collection result type with an equality-comparable element type exists between left and right.</exception>
    public static DbUnionAllExpression UnionAll(
      this DbExpression left,
      DbExpression right)
    {
      Check.NotNull<DbExpression>(left, nameof (left));
      Check.NotNull<DbExpression>(right, nameof (right));
      return new DbUnionAllExpression(ArgumentValidation.RequireCollectionArguments<DbUnionAllExpression>(left, right), left, right);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbLimitExpression" /> that restricts the number of elements in the Argument collection to the specified count Limit value. Tied results are not included in the output.
    /// </summary>
    /// <returns>A new DbLimitExpression with the specified argument and count limit values that does not include tied results.</returns>
    /// <param name="argument">An expression that specifies the input collection.</param>
    /// <param name="count">An expression that specifies the limit value.</param>
    /// <exception cref="T:System.ArgumentNullException">argument or count is null.</exception>
    /// <exception cref="T:System.ArgumentException">argument does not have a collection result type, or count does not have a result type that is equal or promotable to a 64-bit integer type.</exception>
    public static DbLimitExpression Limit(
      this DbExpression argument,
      DbExpression count)
    {
      Check.NotNull<DbExpression>(argument, nameof (argument));
      Check.NotNull<DbExpression>(count, nameof (count));
      ArgumentValidation.RequireCollectionArgument<DbLimitExpression>(argument);
      if (!TypeSemantics.IsIntegerNumericType(count.ResultType))
        throw new ArgumentException(Strings.Cqt_Limit_IntegerRequired, nameof (count));
      if (count.ExpressionKind != DbExpressionKind.Constant && count.ExpressionKind != DbExpressionKind.ParameterReference)
        throw new ArgumentException(Strings.Cqt_Limit_ConstantOrParameterRefRequired, nameof (count));
      if (DbExpressionBuilder.IsConstantNegativeInteger(count))
        throw new ArgumentException(Strings.Cqt_Limit_NonNegativeLimitRequired, nameof (count));
      return new DbLimitExpression(argument.ResultType, argument, count, false);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbCaseExpression" />.
    /// </summary>
    /// <returns>A new DbCaseExpression with the specified cases and default result.</returns>
    /// <param name="whenExpressions">A list of expressions that provide the conditional for of each case.</param>
    /// <param name="thenExpressions">A list of expressions that provide the result of each case.</param>
    /// <param name="elseExpression">An expression that defines the result when no case is matched.</param>
    /// <exception cref="T:System.ArgumentNullException">whenExpressions or thenExpressions is null or contains null, or elseExpression is null.</exception>
    /// <exception cref="T:System.ArgumentException">whenExpressions or thenExpressions is empty or whenExpressions contains an expression with a non-Boolean result type, or no common result type exists for all expressions in thenExpressions and elseExpression.</exception>
    public static DbCaseExpression Case(
      IEnumerable<DbExpression> whenExpressions,
      IEnumerable<DbExpression> thenExpressions,
      DbExpression elseExpression)
    {
      Check.NotNull<IEnumerable<DbExpression>>(whenExpressions, nameof (whenExpressions));
      Check.NotNull<IEnumerable<DbExpression>>(thenExpressions, nameof (thenExpressions));
      Check.NotNull<DbExpression>(elseExpression, nameof (elseExpression));
      DbExpressionList validWhens;
      DbExpressionList validThens;
      return new DbCaseExpression(ArgumentValidation.ValidateCase(whenExpressions, thenExpressions, elseExpression, out validWhens, out validThens), validWhens, validThens, elseExpression);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> representing the invocation of the specified function with the given arguments.
    /// </summary>
    /// <returns>A new DbFunctionExpression representing the function invocation.</returns>
    /// <param name="function">Metadata for the function to invoke.</param>
    /// <param name="arguments">A list of expressions that provide the arguments to the function.</param>
    /// <exception cref="T:System.ArgumentNullException">function is null, or arguments is null or contains null.</exception>
    /// <exception cref="T:System.ArgumentException">The count of arguments does not equal the number of parameters declared by function, or arguments contains an expression that has a result type that is not equal or promotable to the corresponding function parameter type.</exception>
    public static DbFunctionExpression Invoke(
      this EdmFunction function,
      IEnumerable<DbExpression> arguments)
    {
      Check.NotNull<EdmFunction>(function, nameof (function));
      return DbExpressionBuilder.InvokeFunction(function, arguments);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFunctionExpression" /> representing the invocation of the specified function with the given arguments.
    /// </summary>
    /// <returns>A new DbFunctionExpression representing the function invocation.</returns>
    /// <param name="function">Metadata for the function to invoke.</param>
    /// <param name="arguments">Expressions that provide the arguments to the function.</param>
    /// <exception cref="T:System.ArgumentNullException">function is null, or arguments is null or contains null.</exception>
    /// <exception cref="T:System.ArgumentException">The count of arguments does not equal the number of parameters declared by function, or arguments contains an expression that has a result type that is not equal or promotable to the corresponding function parameter type.</exception>
    public static DbFunctionExpression Invoke(
      this EdmFunction function,
      params DbExpression[] arguments)
    {
      Check.NotNull<EdmFunction>(function, nameof (function));
      return DbExpressionBuilder.InvokeFunction(function, (IEnumerable<DbExpression>) arguments);
    }

    private static DbFunctionExpression InvokeFunction(
      EdmFunction function,
      IEnumerable<DbExpression> arguments)
    {
      DbExpressionList validArgs;
      return new DbFunctionExpression(ArgumentValidation.ValidateFunction(function, arguments, out validArgs), function, validArgs);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbLambdaExpression" /> representing the application of the specified Lambda function to the given arguments.
    /// </summary>
    /// <returns>A new Expression representing the Lambda function application.</returns>
    /// <param name="lambda">
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbLambda" /> instance representing the Lambda function to apply.
    /// </param>
    /// <param name="arguments">A list of expressions that provide the arguments.</param>
    /// <exception cref="T:System.ArgumentNullException">lambda or arguments is null.</exception>
    /// <exception cref="T:System.ArgumentException">The count of arguments does not equal the number of variables declared by lambda, or arguments contains an expression that has a result type that is not equal or promotable to the corresponding variable type.</exception>
    public static DbLambdaExpression Invoke(
      this DbLambda lambda,
      IEnumerable<DbExpression> arguments)
    {
      Check.NotNull<DbLambda>(lambda, nameof (lambda));
      Check.NotNull<IEnumerable<DbExpression>>(arguments, nameof (arguments));
      return DbExpressionBuilder.InvokeLambda(lambda, arguments);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbLambdaExpression" /> representing the application of the specified Lambda function to the given arguments.
    /// </summary>
    /// <returns>A new expression representing the Lambda function application.</returns>
    /// <param name="lambda">
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbLambda" /> instance representing the Lambda function to apply.
    /// </param>
    /// <param name="arguments">Expressions that provide the arguments.</param>
    /// <exception cref="T:System.ArgumentNullException">lambda or arguments is null.</exception>
    /// <exception cref="T:System.ArgumentException">The count of arguments does not equal the number of variables declared by lambda, or arguments contains an expression that has a result type that is not equal or promotable to the corresponding variable type.</exception>
    public static DbLambdaExpression Invoke(
      this DbLambda lambda,
      params DbExpression[] arguments)
    {
      Check.NotNull<DbLambda>(lambda, nameof (lambda));
      Check.NotNull<DbExpression[]>(arguments, nameof (arguments));
      return DbExpressionBuilder.InvokeLambda(lambda, (IEnumerable<DbExpression>) arguments);
    }

    private static DbLambdaExpression InvokeLambda(
      DbLambda lambda,
      IEnumerable<DbExpression> arguments)
    {
      DbExpressionList validArguments;
      return new DbLambdaExpression(ArgumentValidation.ValidateInvoke(lambda, arguments, out validArguments), lambda, validArguments);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbNewInstanceExpression" />. If the type argument is a collection type, the arguments specify the elements of the collection. Otherwise the arguments are used as property or column values in the new instance.
    /// </summary>
    /// <returns>A new DbNewInstanceExpression with the specified type and arguments.</returns>
    /// <param name="instanceType">The type of the new instance.</param>
    /// <param name="arguments">Expressions that specify values of the new instances, interpreted according to the instance's type.</param>
    /// <exception cref="T:System.ArgumentNullException">instanceType or arguments is null, or arguments contains null.</exception>
    /// <exception cref="T:System.ArgumentException">arguments is empty or the result types of the contained expressions do not match the requirements of instanceType  (as explained in the remarks section).</exception>
    public static DbNewInstanceExpression New(
      this TypeUsage instanceType,
      IEnumerable<DbExpression> arguments)
    {
      Check.NotNull<TypeUsage>(instanceType, nameof (instanceType));
      return DbExpressionBuilder.NewInstance(instanceType, arguments);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbNewInstanceExpression" />. If the type argument is a collection type, the arguments specify the elements of the collection. Otherwise the arguments are used as property or column values in the new instance.
    /// </summary>
    /// <returns>A new DbNewInstanceExpression with the specified type and arguments.</returns>
    /// <param name="instanceType">The type of the new instance.</param>
    /// <param name="arguments">Expressions that specify values of the new instances, interpreted according to the instance's type.</param>
    /// <exception cref="T:System.ArgumentNullException">instanceType or arguments is null, or arguments contains null.</exception>
    /// <exception cref="T:System.ArgumentException">arguments is empty or the result types of the contained expressions do not match the requirements of instanceType  (as explained in the remarks section).</exception>
    public static DbNewInstanceExpression New(
      this TypeUsage instanceType,
      params DbExpression[] arguments)
    {
      Check.NotNull<TypeUsage>(instanceType, nameof (instanceType));
      return DbExpressionBuilder.NewInstance(instanceType, (IEnumerable<DbExpression>) arguments);
    }

    private static DbNewInstanceExpression NewInstance(
      TypeUsage instanceType,
      IEnumerable<DbExpression> arguments)
    {
      DbExpressionList validArguments;
      return new DbNewInstanceExpression(ArgumentValidation.ValidateNew(instanceType, arguments, out validArguments), validArguments);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbNewInstanceExpression" /> that constructs a collection containing the specified elements. The type of the collection is based on the common type of the elements. If no common element type exists an exception is thrown.
    /// </summary>
    /// <returns>A new DbNewInstanceExpression with the specified collection type and arguments.</returns>
    /// <param name="elements">A list of expressions that provide the elements of the collection.</param>
    /// <exception cref="T:System.ArgumentNullException">elements is null, or contains null.</exception>
    /// <exception cref="T:System.ArgumentException">elements is empty or contains expressions for which no common result type exists.</exception>
    public static DbNewInstanceExpression NewCollection(
      IEnumerable<DbExpression> elements)
    {
      return DbExpressionBuilder.CreateNewCollection(elements);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbNewInstanceExpression" /> that constructs a collection containing the specified elements. The type of the collection is based on the common type of the elements. If no common element type exists an exception is thrown.
    /// </summary>
    /// <returns>A new DbNewInstanceExpression with the specified collection type and arguments.</returns>
    /// <param name="elements">A list of expressions that provide the elements of the collection.</param>
    /// <exception cref="T:System.ArgumentNullException">elements is null, or contains null..</exception>
    /// <exception cref="T:System.ArgumentException">elements is empty or contains expressions for which no common result type exists.</exception>
    public static DbNewInstanceExpression NewCollection(
      params DbExpression[] elements)
    {
      Check.NotNull<DbExpression[]>(elements, nameof (elements));
      return DbExpressionBuilder.CreateNewCollection((IEnumerable<DbExpression>) elements);
    }

    [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
    private static DbNewInstanceExpression CreateNewCollection(
      IEnumerable<DbExpression> elements)
    {
      TypeUsage commonElementType = (TypeUsage) null;
      DbExpressionList expressionList = ArgumentValidation.CreateExpressionList(elements, nameof (elements), (Action<DbExpression, int>) ((exp, idx) =>
      {
        commonElementType = commonElementType != null ? TypeSemantics.GetCommonType(commonElementType, exp.ResultType) : exp.ResultType;
        if (commonElementType == null)
          throw new ArgumentException(Strings.Cqt_Factory_NewCollectionInvalidCommonType, "collectionElements");
      }));
      return new DbNewInstanceExpression(DbExpressionBuilder.CreateCollectionResultType(commonElementType), expressionList);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbNewInstanceExpression" /> that constructs an empty collection of the specified collection type.
    /// </summary>
    /// <returns>A new DbNewInstanceExpression with the specified collection type and an empty Arguments list.</returns>
    /// <param name="collectionType">The type metadata for the collection to create</param>
    /// <exception cref="T:System.ArgumentNullException">collectionType is null.</exception>
    /// <exception cref="T:System.ArgumentException">collectionType is not a collection type.</exception>
    public static DbNewInstanceExpression NewEmptyCollection(
      this TypeUsage collectionType)
    {
      Check.NotNull<TypeUsage>(collectionType, nameof (collectionType));
      DbExpressionList validElements;
      return new DbNewInstanceExpression(ArgumentValidation.ValidateNewEmptyCollection(collectionType, out validElements), validElements);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbNewInstanceExpression" /> that produces a row with the specified named columns and the given values, specified as expressions.
    /// </summary>
    /// <returns>A new DbNewInstanceExpression that represents the construction of the row.</returns>
    /// <param name="columnValues">A list of string-DbExpression key-value pairs that defines the structure and values of the row.</param>
    /// <exception cref="T:System.ArgumentNullException">columnValues is null or contains an element with a null column name or expression.</exception>
    /// <exception cref="T:System.ArgumentException">columnValues is empty, or contains a duplicate or invalid column name.</exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static DbNewInstanceExpression NewRow(
      IEnumerable<KeyValuePair<string, DbExpression>> columnValues)
    {
      Check.NotNull<IEnumerable<KeyValuePair<string, DbExpression>>>(columnValues, nameof (columnValues));
      DbExpressionList validElements;
      return new DbNewInstanceExpression(ArgumentValidation.ValidateNewRow(columnValues, out validElements), validElements);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbPropertyExpression" /> representing the retrieval of the specified property.
    /// </summary>
    /// <returns>A new DbPropertyExpression representing the property retrieval.</returns>
    /// <param name="instance">The instance from which to retrieve the property. May be null if the property is static.</param>
    /// <param name="propertyMetadata">Metadata for the property to retrieve.</param>
    /// <exception cref="T:System.ArgumentNullException">propertyMetadata is null or instance is null and the property is not static.</exception>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "required for this feature")]
    public static DbPropertyExpression Property(
      this DbExpression instance,
      EdmProperty propertyMetadata)
    {
      Check.NotNull<DbExpression>(instance, nameof (instance));
      Check.NotNull<EdmProperty>(propertyMetadata, nameof (propertyMetadata));
      return DbExpressionBuilder.PropertyFromMember(instance, (EdmMember) propertyMetadata, nameof (propertyMetadata));
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbPropertyExpression" /> representing the retrieval of the specified navigation property.
    /// </summary>
    /// <returns>A new DbPropertyExpression representing the navigation property retrieval.</returns>
    /// <param name="instance">The instance from which to retrieve the navigation property.</param>
    /// <param name="navigationProperty">Metadata for the navigation property to retrieve.</param>
    /// <exception cref="T:System.ArgumentNullException">navigationProperty or instance is null.</exception>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "required for this feature")]
    public static DbPropertyExpression Property(
      this DbExpression instance,
      NavigationProperty navigationProperty)
    {
      Check.NotNull<DbExpression>(instance, nameof (instance));
      Check.NotNull<NavigationProperty>(navigationProperty, nameof (navigationProperty));
      return DbExpressionBuilder.PropertyFromMember(instance, (EdmMember) navigationProperty, nameof (navigationProperty));
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbPropertyExpression" /> representing the retrieval of the specified relationship end member.
    /// </summary>
    /// <returns>A new DbPropertyExpression representing the relationship end member retrieval.</returns>
    /// <param name="instance">The instance from which to retrieve the relationship end member.</param>
    /// <param name="relationshipEnd">Metadata for the relationship end member to retrieve.</param>
    /// <exception cref="T:System.ArgumentNullException">relationshipEnd is null or instance is null and the property is not static.</exception>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "required for this feature")]
    public static DbPropertyExpression Property(
      this DbExpression instance,
      RelationshipEndMember relationshipEnd)
    {
      Check.NotNull<DbExpression>(instance, nameof (instance));
      Check.NotNull<RelationshipEndMember>(relationshipEnd, nameof (relationshipEnd));
      return DbExpressionBuilder.PropertyFromMember(instance, (EdmMember) relationshipEnd, nameof (relationshipEnd));
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbPropertyExpression" /> representing the retrieval of the instance property with the specified name from the given instance.
    /// </summary>
    /// <returns>A new DbPropertyExpression that represents the property retrieval.</returns>
    /// <param name="instance">The instance from which to retrieve the property.</param>
    /// <param name="propertyName">The name of the property to retrieve.</param>
    /// <exception cref="T:System.ArgumentNullException">propertyName is null or instance is null and the property is not static.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">No property with the specified name is declared by the type of instance.</exception>
    public static DbPropertyExpression Property(
      this DbExpression instance,
      string propertyName)
    {
      return DbExpressionBuilder.PropertyByName(instance, propertyName, false);
    }

    private static DbPropertyExpression PropertyFromMember(
      DbExpression instance,
      EdmMember property,
      string propertyArgumentName)
    {
      ArgumentValidation.CheckMember(property, propertyArgumentName);
      if (instance == null)
        throw new ArgumentException(Strings.Cqt_Property_InstanceRequiredForInstance, nameof (instance));
      TypeUsage requiredResultType = TypeUsage.Create((EdmType) property.DeclaringType);
      ArgumentValidation.RequireCompatibleType(instance, requiredResultType, nameof (instance));
      return new DbPropertyExpression(Helper.GetModelTypeUsage(property), property, instance);
    }

    private static DbPropertyExpression PropertyByName(
      DbExpression instance,
      string propertyName,
      bool ignoreCase)
    {
      Check.NotNull<DbExpression>(instance, nameof (instance));
      Check.NotNull<string>(propertyName, nameof (propertyName));
      EdmMember foundMember;
      return new DbPropertyExpression(ArgumentValidation.ValidateProperty(instance, propertyName, ignoreCase, out foundMember), foundMember, instance);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbSetClause" /> representing setting a property to a value.
    /// </summary>
    /// <param name="property">The property to be set.</param>
    /// <param name="value">The value to set the property to.</param>
    /// <returns>The newly created set clause.</returns>
    public static DbSetClause SetClause(DbExpression property, DbExpression value)
    {
      Check.NotNull<DbExpression>(property, nameof (property));
      Check.NotNull<DbExpression>(value, nameof (value));
      return new DbSetClause(property, value);
    }

    private static string ExtractAlias(MethodInfo method)
    {
      return DbExpressionBuilder.ExtractAliases(method)[0];
    }

    internal static string[] ExtractAliases(MethodInfo method)
    {
      ParameterInfo[] parameters = method.GetParameters();
      int count;
      int length;
      if (method.IsStatic && typeof (Closure) == parameters[0].ParameterType)
      {
        count = 1;
        length = parameters.Length - 1;
      }
      else
      {
        count = 0;
        length = parameters.Length;
      }
      string[] strArray = new string[length];
      bool flag = ((IEnumerable<ParameterInfo>) parameters).Skip<ParameterInfo>(count).Any<ParameterInfo>((Func<ParameterInfo, bool>) (p => p.Name == null));
      for (int index = count; index < parameters.Length; ++index)
        strArray[index - count] = flag ? DbExpressionBuilder._bindingAliases.Next() : parameters[index].Name;
      return strArray;
    }

    private static DbExpressionBinding ConvertToBinding<TResult>(
      DbExpression source,
      Func<DbExpression, TResult> argument,
      out TResult argumentResult)
    {
      string alias = DbExpressionBuilder.ExtractAlias(argument.Method);
      DbExpressionBinding expressionBinding = source.BindAs(alias);
      argumentResult = argument((DbExpression) expressionBinding.Variable);
      return expressionBinding;
    }

    private static DbExpressionBinding[] ConvertToBinding(
      DbExpression left,
      DbExpression right,
      Func<DbExpression, DbExpression, DbExpression> argument,
      out DbExpression argumentExp)
    {
      string[] aliases = DbExpressionBuilder.ExtractAliases(argument.Method);
      DbExpressionBinding expressionBinding1 = left.BindAs(aliases[0]);
      DbExpressionBinding expressionBinding2 = right.BindAs(aliases[1]);
      argumentExp = argument((DbExpression) expressionBinding1.Variable, (DbExpression) expressionBinding2.Variable);
      return new DbExpressionBinding[2]
      {
        expressionBinding1,
        expressionBinding2
      };
    }

    internal static List<KeyValuePair<string, TRequired>> TryGetAnonymousTypeValues<TInstance, TRequired>(
      object instance)
    {
      IEnumerable<PropertyInfo> instanceProperties = typeof (TInstance).GetInstanceProperties();
      if (typeof (TInstance).BaseType() != typeof (object) || instanceProperties.Any<PropertyInfo>((Func<PropertyInfo, bool>) (p => !p.IsPublic())))
        return (List<KeyValuePair<string, TRequired>>) null;
      List<KeyValuePair<string, TRequired>> keyValuePairList = (List<KeyValuePair<string, TRequired>>) null;
      foreach (PropertyInfo propertyInfo in instanceProperties.Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => p.IsPublic())))
      {
        if (!propertyInfo.CanRead || !typeof (TRequired).IsAssignableFrom(propertyInfo.PropertyType))
          return (List<KeyValuePair<string, TRequired>>) null;
        if (keyValuePairList == null)
          keyValuePairList = new List<KeyValuePair<string, TRequired>>();
        keyValuePairList.Add(new KeyValuePair<string, TRequired>(propertyInfo.Name, (TRequired) propertyInfo.GetValue(instance, (object[]) null)));
      }
      return keyValuePairList;
    }

    private static bool TryResolveToConstant(
      Type type,
      object value,
      out DbExpression constantOrNullExpression)
    {
      constantOrNullExpression = (DbExpression) null;
      Type clrType = type;
      if (type.IsGenericType() && typeof (Nullable<>).Equals(type.GetGenericTypeDefinition()))
        clrType = type.GetGenericArguments()[0];
      PrimitiveTypeKind resolvedPrimitiveTypeKind;
      if (ClrProviderManifest.TryGetPrimitiveTypeKind(clrType, out resolvedPrimitiveTypeKind))
      {
        TypeUsage literalTypeUsage = TypeHelpers.GetLiteralTypeUsage(resolvedPrimitiveTypeKind);
        constantOrNullExpression = value != null ? (DbExpression) literalTypeUsage.Constant(value) : (DbExpression) literalTypeUsage.Null();
      }
      return constantOrNullExpression != null;
    }

    private static DbExpression ResolveToExpression<TArgument>(TArgument argument)
    {
      object instance = (object) argument;
      DbExpression constantOrNullExpression;
      if (DbExpressionBuilder.TryResolveToConstant(typeof (TArgument), instance, out constantOrNullExpression))
        return constantOrNullExpression;
      if (instance == null)
        return (DbExpression) null;
      if (typeof (DbExpression).IsAssignableFrom(typeof (TArgument)))
        return (DbExpression) instance;
      if (typeof (Row).Equals(typeof (TArgument)))
        return (DbExpression) ((Row) instance).ToExpression();
      List<KeyValuePair<string, DbExpression>> anonymousTypeValues = DbExpressionBuilder.TryGetAnonymousTypeValues<TArgument, DbExpression>(instance);
      if (anonymousTypeValues != null)
        return (DbExpression) DbExpressionBuilder.NewRow((IEnumerable<KeyValuePair<string, DbExpression>>) anonymousTypeValues);
      throw new NotSupportedException(Strings.Cqt_Factory_MethodResultTypeNotSupported((object) typeof (TArgument).FullName));
    }

    private static DbApplyExpression CreateApply(
      DbExpression source,
      Func<DbExpression, KeyValuePair<string, DbExpression>> apply,
      Func<DbExpressionBinding, DbExpressionBinding, DbApplyExpression> resultBuilder)
    {
      KeyValuePair<string, DbExpression> argumentResult;
      DbExpressionBinding binding = DbExpressionBuilder.ConvertToBinding<KeyValuePair<string, DbExpression>>(source, apply, out argumentResult);
      DbExpressionBinding expressionBinding = argumentResult.Value.BindAs(argumentResult.Key);
      return resultBuilder(binding, expressionBinding);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbQuantifierExpression" /> that determines whether the given predicate holds for all elements of the input set.
    /// </summary>
    /// <returns>A new DbQuantifierExpression that represents the All operation.</returns>
    /// <param name="source">An expression that specifies the input set.</param>
    /// <param name="predicate">A method representing a predicate to evaluate for each member of the input set.    This method must produce an expression with a Boolean result type that provides the predicate logic.</param>
    /// <exception cref="T:System.ArgumentNullException">source or predicate is null.</exception>
    /// <exception cref="T:System.ArgumentNullException">The expression produced by predicate is null.</exception>
    /// <exception cref="T:System.ArgumentException">source  does not have a collection result type.</exception>
    /// <exception cref="T:System.ArgumentException">The expression produced by Predicate  does not have a Boolean result type.</exception>
    public static DbQuantifierExpression All(
      this DbExpression source,
      Func<DbExpression, DbExpression> predicate)
    {
      Check.NotNull<DbExpression>(source, nameof (source));
      Check.NotNull<Func<DbExpression, DbExpression>>(predicate, nameof (predicate));
      DbExpression argumentResult;
      return DbExpressionBuilder.ConvertToBinding<DbExpression>(source, predicate, out argumentResult).All(argumentResult);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that determines whether the specified set argument is non-empty.
    /// </summary>
    /// <returns>
    /// A new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbNotExpression" /> applied to a new
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbIsEmptyExpression" />
    /// with the specified argument.
    /// </returns>
    /// <param name="source">An expression that specifies the input set.</param>
    /// <exception cref="T:System.ArgumentNullException">source is null.</exception>
    /// <exception cref="T:System.ArgumentException">source does not have a collection result type.</exception>
    public static DbExpression Any(this DbExpression source)
    {
      return source.Exists();
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that determines whether the specified set argument is non-empty.
    /// </summary>
    /// <returns>
    /// A new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbNotExpression" /> applied to a new
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbIsEmptyExpression" />
    /// with the specified argument.
    /// </returns>
    /// <param name="argument">An expression that specifies the input set.</param>
    /// <exception cref="T:System.ArgumentNullException">argument is null.</exception>
    /// <exception cref="T:System.ArgumentException">argument does not have a collection result type.</exception>
    public static DbExpression Exists(this DbExpression argument)
    {
      return (DbExpression) argument.IsEmpty().Not();
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbQuantifierExpression" /> that determines whether the given predicate holds for any element of the input set.
    /// </summary>
    /// <returns>A new DbQuantifierExpression that represents the Any operation.</returns>
    /// <param name="source">An expression that specifies the input set.</param>
    /// <param name="predicate">A method representing the predicate to evaluate for each member of the input set. This method must produce an expression with a Boolean result type that provides the predicate logic.</param>
    /// <exception cref="T:System.ArgumentNullException">source or predicate is null.</exception>
    /// <exception cref="T:System.ArgumentNullException">The expression produced by predicate is null.</exception>
    /// <exception cref="T:System.ArgumentException">source does not have a collection result type.</exception>
    /// <exception cref="T:System.ArgumentException">The expression produced by predicate does not have a Boolean result type.</exception>
    public static DbQuantifierExpression Any(
      this DbExpression source,
      Func<DbExpression, DbExpression> predicate)
    {
      Check.NotNull<DbExpression>(source, nameof (source));
      Check.NotNull<Func<DbExpression, DbExpression>>(predicate, nameof (predicate));
      DbExpression argumentResult;
      return DbExpressionBuilder.ConvertToBinding<DbExpression>(source, predicate, out argumentResult).Any(argumentResult);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbApplyExpression" /> that evaluates the given apply expression once for each element of a given input set, producing a collection of rows with corresponding input and apply columns. Rows for which apply evaluates to an empty set are not included.
    /// </summary>
    /// <returns>
    /// An new DbApplyExpression with the specified input and apply bindings and an
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionKind" />
    /// of CrossApply.
    /// </returns>
    /// <param name="source">
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that specifies the input set.
    /// </param>
    /// <param name="apply">A method that specifies the logic to evaluate once for each member of the input set. </param>
    /// <exception cref="T:System.ArgumentNullException">source or apply is null.</exception>
    /// <exception cref="T:System.ArgumentException">source does not have a collection result type.</exception>
    /// <exception cref="T:System.ArgumentNullException">The result of apply contains a name or expression that is null.</exception>
    /// <exception cref="T:System.ArgumentException">The result of apply contains a name or expression that is not valid in an expression binding.</exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static DbApplyExpression CrossApply(
      this DbExpression source,
      Func<DbExpression, KeyValuePair<string, DbExpression>> apply)
    {
      Check.NotNull<DbExpression>(source, nameof (source));
      Check.NotNull<Func<DbExpression, KeyValuePair<string, DbExpression>>>(apply, nameof (apply));
      return DbExpressionBuilder.CreateApply(source, apply, new Func<DbExpressionBinding, DbExpressionBinding, DbApplyExpression>(DbExpressionBuilder.CrossApply));
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbApplyExpression" /> that evaluates the given apply expression once for each element of a given input set, producing a collection of rows with corresponding input and apply columns. Rows for which apply evaluates to an empty set have an apply column value of null.
    /// </summary>
    /// <returns>
    /// An new DbApplyExpression with the specified input and apply bindings and an
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionKind" />
    /// of OuterApply.
    /// </returns>
    /// <param name="source">
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that specifies the input set.
    /// </param>
    /// <param name="apply">A method that specifies the logic to evaluate once for each member of the input set.</param>
    /// <exception cref="T:System.ArgumentNullException">source or apply is null.</exception>
    /// <exception cref="T:System.ArgumentException">Source does not have a collection result type.</exception>
    /// <exception cref="T:System.ArgumentNullException">The result of apply contains a name or expression that is null.</exception>
    /// <exception cref="T:System.ArgumentException">The result of apply contains a name or expression that is not valid in an expression binding.</exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static DbApplyExpression OuterApply(
      this DbExpression source,
      Func<DbExpression, KeyValuePair<string, DbExpression>> apply)
    {
      Check.NotNull<DbExpression>(source, nameof (source));
      Check.NotNull<Func<DbExpression, KeyValuePair<string, DbExpression>>>(apply, nameof (apply));
      return DbExpressionBuilder.CreateApply(source, apply, new Func<DbExpressionBinding, DbExpressionBinding, DbApplyExpression>(DbExpressionBuilder.OuterApply));
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbJoinExpression" /> that joins the sets specified by the left and right expressions, on the specified join condition, using FullOuterJoin as the
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionKind" />
    /// .
    /// </summary>
    /// <returns>
    /// A new DbJoinExpression, with an <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionKind" /> of FullOuterJoin, that represents the full outer join operation applied to the left and right input sets under the given join condition.
    /// </returns>
    /// <param name="left">
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that specifies the left set argument.
    /// </param>
    /// <param name="right">
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that specifies the right set argument.
    /// </param>
    /// <param name="joinCondition"> A method representing the condition on which to join. This method must produce an expression with a Boolean result type that provides the logic of the join condition.</param>
    /// <exception cref="T:System.ArgumentNullException">left, right or joinCondition is null.</exception>
    /// <exception cref="T:System.ArgumentException">left or right does not have a collection result type.</exception>
    /// <exception cref="T:System.ArgumentNullException">The expression produced by joinCondition is null.</exception>
    /// <exception cref="T:System.ArgumentException">The expression produced by joinCondition does not have a Boolean result type.</exception>
    public static DbJoinExpression FullOuterJoin(
      this DbExpression left,
      DbExpression right,
      Func<DbExpression, DbExpression, DbExpression> joinCondition)
    {
      Check.NotNull<DbExpression>(left, nameof (left));
      Check.NotNull<DbExpression>(right, nameof (right));
      Check.NotNull<Func<DbExpression, DbExpression, DbExpression>>(joinCondition, nameof (joinCondition));
      DbExpression argumentExp;
      DbExpressionBinding[] binding = DbExpressionBuilder.ConvertToBinding(left, right, joinCondition, out argumentExp);
      return binding[0].FullOuterJoin(binding[1], argumentExp);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbJoinExpression" /> that joins the sets specified by the left and right expressions, on the specified join condition, using InnerJoin as the
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionKind" />
    /// .
    /// </summary>
    /// <returns>
    /// A new DbJoinExpression, with an <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionKind" /> of InnerJoin, that represents the inner join operation applied to the left and right input sets under the given join condition.
    /// </returns>
    /// <param name="left">
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that specifies the left set argument.
    /// </param>
    /// <param name="right">
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that specifies the right set argument.
    /// </param>
    /// <param name="joinCondition">A method representing the condition on which to join. This method must produce an expression with a Boolean result type that provides the logic of the join condition.</param>
    /// <exception cref="T:System.ArgumentNullException">left, right or joinCondition is null.</exception>
    /// <exception cref="T:System.ArgumentException">left or right does not have a collection result type.</exception>
    /// <exception cref="T:System.ArgumentNullException">The expression produced by joinCondition is null.</exception>
    /// <exception cref="T:System.ArgumentException">The expression produced by joinCondition does not have a Boolean result type.</exception>
    public static DbJoinExpression InnerJoin(
      this DbExpression left,
      DbExpression right,
      Func<DbExpression, DbExpression, DbExpression> joinCondition)
    {
      Check.NotNull<DbExpression>(left, nameof (left));
      Check.NotNull<DbExpression>(right, nameof (right));
      Check.NotNull<Func<DbExpression, DbExpression, DbExpression>>(joinCondition, nameof (joinCondition));
      DbExpression argumentExp;
      DbExpressionBinding[] binding = DbExpressionBuilder.ConvertToBinding(left, right, joinCondition, out argumentExp);
      return binding[0].InnerJoin(binding[1], argumentExp);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbJoinExpression" /> that joins the sets specified by the left and right expressions, on the specified join condition, using LeftOuterJoin as the
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionKind" />
    /// .
    /// </summary>
    /// <returns>
    /// A new DbJoinExpression, with an <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionKind" /> of LeftOuterJoin, that represents the left outer join operation applied to the left and right input sets under the given join condition.
    /// </returns>
    /// <param name="left">
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that specifies the left set argument.
    /// </param>
    /// <param name="right">
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that specifies the right set argument.
    /// </param>
    /// <param name="joinCondition">A method representing the condition on which to join. This method must produce an expression with a Boolean result type that provides the logic of the join condition.</param>
    /// <exception cref="T:System.ArgumentNullException">left, right or joinCondition is null.</exception>
    /// <exception cref="T:System.ArgumentException">left or right does not have a collection result type.</exception>
    /// <exception cref="T:System.ArgumentNullException">The expression produced by joinCondition is null.</exception>
    /// <exception cref="T:System.ArgumentException">The expression produced by joinCondition does not have a Boolean result type.</exception>
    public static DbJoinExpression LeftOuterJoin(
      this DbExpression left,
      DbExpression right,
      Func<DbExpression, DbExpression, DbExpression> joinCondition)
    {
      Check.NotNull<DbExpression>(left, nameof (left));
      Check.NotNull<DbExpression>(right, nameof (right));
      Check.NotNull<Func<DbExpression, DbExpression, DbExpression>>(joinCondition, nameof (joinCondition));
      DbExpression argumentExp;
      DbExpressionBinding[] binding = DbExpressionBuilder.ConvertToBinding(left, right, joinCondition, out argumentExp);
      return binding[0].LeftOuterJoin(binding[1], argumentExp);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbJoinExpression" /> that joins the sets specified by the outer and inner expressions, on an equality condition between the specified outer and inner keys, using InnerJoin as the
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionKind" />
    /// .
    /// </summary>
    /// <returns>
    /// A new DbJoinExpression, with an <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionKind" /> of InnerJoin, that represents the inner join operation applied to the left and right input sets under a join condition that compares the outer and inner key values for equality.
    /// </returns>
    /// <param name="outer">
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that specifies the outer set argument.
    /// </param>
    /// <param name="inner">
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that specifies the inner set argument.
    /// </param>
    /// <param name="outerKey">A method that specifies how the outer key value should be derived from an element of the outer set.</param>
    /// <param name="innerKey">A method that specifies how the inner key value should be derived from an element of the inner set.</param>
    /// <exception cref="T:System.ArgumentNullException">outer, inner, outerKey or innerKey is null.</exception>
    /// <exception cref="T:System.ArgumentException">outer or inner does not have a collection result type.</exception>
    /// <exception cref="T:System.ArgumentNullException">The expression produced by outerKey or innerKey is null.</exception>
    /// <exception cref="T:System.ArgumentException">The expressions produced by outerKey and innerKey are not comparable for equality.</exception>
    public static DbJoinExpression Join(
      this DbExpression outer,
      DbExpression inner,
      Func<DbExpression, DbExpression> outerKey,
      Func<DbExpression, DbExpression> innerKey)
    {
      Check.NotNull<DbExpression>(outer, nameof (outer));
      Check.NotNull<DbExpression>(inner, nameof (inner));
      Check.NotNull<Func<DbExpression, DbExpression>>(outerKey, nameof (outerKey));
      Check.NotNull<Func<DbExpression, DbExpression>>(innerKey, nameof (innerKey));
      DbExpression argumentResult1;
      DbExpression argumentResult2;
      return DbExpressionBuilder.ConvertToBinding<DbExpression>(outer, outerKey, out argumentResult1).InnerJoin(DbExpressionBuilder.ConvertToBinding<DbExpression>(inner, innerKey, out argumentResult2), (DbExpression) argumentResult1.Equal(argumentResult2));
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbProjectExpression" /> that projects the specified selector over the sets specified by the outer and inner expressions, joined on an equality condition between the specified outer and inner keys, using InnerJoin as the
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionKind" />
    /// .
    /// </summary>
    /// <returns>
    /// A new DbProjectExpression with the specified selector as its projection, and a new DbJoinExpression as its input. The input DbJoinExpression is created with an
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionKind" />
    /// of InnerJoin, that represents the inner join operation applied to the left and right input sets under a join condition that compares the outer and inner key values for equality.
    /// </returns>
    /// <param name="outer">
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that specifies the outer set argument.
    /// </param>
    /// <param name="inner">
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that specifies the inner set argument.
    /// </param>
    /// <param name="outerKey">A method that specifies how the outer key value should be derived from an element of the outer set.</param>
    /// <param name="innerKey">A method that specifies how the inner key value should be derived from an element of the inner set.</param>
    /// <param name="selector">
    /// A method that specifies how an element of the result set should be derived from elements of the inner and outer sets. This method must produce an instance of a type that is compatible with Join and can be resolved into a
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" />
    /// . Compatibility requirements for  TSelector  are described in remarks.
    /// </param>
    /// <typeparam name="TSelector">The type of the  selector .</typeparam>
    /// <exception cref="T:System.ArgumentNullException">outer, inner, outerKey, innerKey or selector is null.</exception>
    /// <exception cref="T:System.ArgumentException">outer or inner does not have a collection result type.</exception>
    /// <exception cref="T:System.ArgumentNullException">The expression produced by outerKey or innerKey is null.</exception>
    /// <exception cref="T:System.ArgumentNullException">The result of selector is null after conversion to DbExpression.</exception>
    /// <exception cref="T:System.ArgumentException">The expressions produced by outerKey and innerKey is not comparable for equality.</exception>
    /// <exception cref="T:System.ArgumentException">The result of Selector is not compatible with SelectMany.</exception>
    public static DbProjectExpression Join<TSelector>(
      this DbExpression outer,
      DbExpression inner,
      Func<DbExpression, DbExpression> outerKey,
      Func<DbExpression, DbExpression> innerKey,
      Func<DbExpression, DbExpression, TSelector> selector)
    {
      Check.NotNull<Func<DbExpression, DbExpression, TSelector>>(selector, nameof (selector));
      DbJoinExpression input1 = outer.Join(inner, outerKey, innerKey);
      DbExpressionBinding input2 = input1.Bind();
      DbExpression dbExpression1 = (DbExpression) input2.Variable.Property(input1.Left.VariableName);
      DbExpression dbExpression2 = (DbExpression) input2.Variable.Property(input1.Right.VariableName);
      DbExpression expression = DbExpressionBuilder.ResolveToExpression<TSelector>(selector(dbExpression1, dbExpression2));
      return input2.Project(expression);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbSortExpression" /> that sorts the given input set by the specified sort key, with ascending sort order and default collation.
    /// </summary>
    /// <returns>A new DbSortExpression that represents the order-by operation.</returns>
    /// <param name="source">An expression that specifies the input set.</param>
    /// <param name="sortKey">A method that specifies how to derive the sort key expression given a member of the input set. This method must produce an expression with an order-comparable result type that provides the sort key definition. </param>
    /// <exception cref="T:System.ArgumentNullException">source or sortKey is null.</exception>
    /// <exception cref="T:System.ArgumentNullException">The expression produced by sortKey is null.</exception>
    /// <exception cref="T:System.ArgumentException">source does not have a collection result type.</exception>
    /// <exception cref="T:System.ArgumentException">The expression produced by sortKey does not have an order-comparable result type.</exception>
    public static DbSortExpression OrderBy(
      this DbExpression source,
      Func<DbExpression, DbExpression> sortKey)
    {
      Check.NotNull<DbExpression>(source, nameof (source));
      Check.NotNull<Func<DbExpression, DbExpression>>(sortKey, nameof (sortKey));
      DbExpression argumentResult;
      return DbExpressionBuilder.ConvertToBinding<DbExpression>(source, sortKey, out argumentResult).Sort((IEnumerable<DbSortClause>) new DbSortClause[1]
      {
        argumentResult.ToSortClause()
      });
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbSortExpression" /> that sorts the given input set by the specified sort key, with ascending sort order and the specified collation.
    /// </summary>
    /// <returns>A new DbSortExpression that represents the order-by operation.</returns>
    /// <param name="source">An expression that specifies the input set.</param>
    /// <param name="sortKey">A method that specifies how to derive the sort key expression given a member of the input set. This method must produce an expression with an order-comparable result type that provides the sort key definition. </param>
    /// <param name="collation">The collation to sort under.</param>
    /// <exception cref="T:System.ArgumentNullException">source, sortKey or collation is null.</exception>
    /// <exception cref="T:System.ArgumentNullException">The expression produced by sortKey is null.</exception>
    /// <exception cref="T:System.ArgumentException">source does not have a collection result type.</exception>
    /// <exception cref="T:System.ArgumentException">The expression produced by sortKey  does not have an order-comparable string result type.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">collation is empty or contains only space characters.</exception>
    public static DbSortExpression OrderBy(
      this DbExpression source,
      Func<DbExpression, DbExpression> sortKey,
      string collation)
    {
      Check.NotNull<DbExpression>(source, nameof (source));
      Check.NotNull<Func<DbExpression, DbExpression>>(sortKey, nameof (sortKey));
      DbExpression argumentResult;
      return DbExpressionBuilder.ConvertToBinding<DbExpression>(source, sortKey, out argumentResult).Sort((IEnumerable<DbSortClause>) new DbSortClause[1]
      {
        argumentResult.ToSortClause(collation)
      });
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbSortExpression" /> that sorts the given input set by the specified sort key, with descending sort order and default collation.
    /// </summary>
    /// <returns>A new DbSortExpression that represents the order-by operation.</returns>
    /// <param name="source">An expression that specifies the input set.</param>
    /// <param name="sortKey">A method that specifies how to derive the sort key expression given a member of the input set. This method must produce an expression with an order-comparable result type that provides the sort key definition. </param>
    /// <exception cref="T:System.ArgumentNullException">source or sortKey is null.</exception>
    /// <exception cref="T:System.ArgumentNullException">The expression produced by sortKey is null.</exception>
    /// <exception cref="T:System.ArgumentException">source does not have a collection result type.</exception>
    /// <exception cref="T:System.ArgumentException">The expression produced by sortKey does not have an order-comparable result type.</exception>
    public static DbSortExpression OrderByDescending(
      this DbExpression source,
      Func<DbExpression, DbExpression> sortKey)
    {
      Check.NotNull<DbExpression>(source, nameof (source));
      Check.NotNull<Func<DbExpression, DbExpression>>(sortKey, nameof (sortKey));
      DbExpression argumentResult;
      return DbExpressionBuilder.ConvertToBinding<DbExpression>(source, sortKey, out argumentResult).Sort((IEnumerable<DbSortClause>) new DbSortClause[1]
      {
        argumentResult.ToSortClauseDescending()
      });
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbSortExpression" /> that sorts the given input set by the specified sort key, with descending sort order and the specified collation.
    /// </summary>
    /// <returns>A new DbSortExpression that represents the order-by operation.</returns>
    /// <param name="source">An expression that specifies the input set.</param>
    /// <param name="sortKey">A method that specifies how to derive the sort key expression given a member of the input set. This method must produce an expression with an order-comparable result type that provides the sort key definition. </param>
    /// <param name="collation">The collation to sort under.</param>
    /// <exception cref="T:System.ArgumentNullException">source, sortKey or collation is null.</exception>
    /// <exception cref="T:System.ArgumentNullException">The expression produced by sortKey is null.</exception>
    /// <exception cref="T:System.ArgumentException">source does not have a collection result type.</exception>
    /// <exception cref="T:System.ArgumentException">The expression produced by sortKey does not have an order-comparable string result type.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">collation is empty or contains only space characters.</exception>
    public static DbSortExpression OrderByDescending(
      this DbExpression source,
      Func<DbExpression, DbExpression> sortKey,
      string collation)
    {
      Check.NotNull<DbExpression>(source, nameof (source));
      Check.NotNull<Func<DbExpression, DbExpression>>(sortKey, nameof (sortKey));
      DbExpression argumentResult;
      return DbExpressionBuilder.ConvertToBinding<DbExpression>(source, sortKey, out argumentResult).Sort((IEnumerable<DbSortClause>) new DbSortClause[1]
      {
        argumentResult.ToSortClauseDescending(collation)
      });
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbProjectExpression" /> that selects the specified expression over the given input set.
    /// </summary>
    /// <returns>A new DbProjectExpression that represents the select operation.</returns>
    /// <param name="source">An expression that specifies the input set.</param>
    /// <param name="projection">
    /// A method that specifies how to derive the projected expression given a member of the input set. This method must produce an instance of a type that is compatible with Select and can be resolved into a
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" />
    /// . Compatibility requirements for  TProjection  are described in remarks.
    /// </param>
    /// <typeparam name="TProjection">The method result type of projection.</typeparam>
    /// <exception cref="T:System.ArgumentNullException">source or projection is null.</exception>
    /// <exception cref="T:System.ArgumentNullException">The result of projection is null.</exception>
    public static DbProjectExpression Select<TProjection>(
      this DbExpression source,
      Func<DbExpression, TProjection> projection)
    {
      Check.NotNull<DbExpression>(source, nameof (source));
      Check.NotNull<Func<DbExpression, TProjection>>(projection, nameof (projection));
      TProjection argumentResult;
      return DbExpressionBuilder.ConvertToBinding<TProjection>(source, projection, out argumentResult).Project(DbExpressionBuilder.ResolveToExpression<TProjection>(argumentResult));
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbApplyExpression" /> that evaluates the given apply expression once for each element of a given input set, producing a collection of rows with corresponding input and apply columns. Rows for which apply evaluates to an empty set are not included. A
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbProjectExpression" />
    /// is then created that selects the apply column from each row, producing the overall collection of apply results.
    /// </summary>
    /// <returns>
    /// An new DbProjectExpression that selects the apply column from a new DbApplyExpression with the specified input and apply bindings and an
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionKind" />
    /// of CrossApply.
    /// </returns>
    /// <param name="source">
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that specifies the input set.
    /// </param>
    /// <param name="apply">A method that represents the logic to evaluate once for each member of the input set.</param>
    /// <exception cref="T:System.ArgumentNullException">source or apply is null.</exception>
    /// <exception cref="T:System.ArgumentNullException">The expression produced by apply is null.</exception>
    /// <exception cref="T:System.ArgumentException">source  does not have a collection result type.</exception>
    /// <exception cref="T:System.ArgumentException">The expression produced by apply does not have a collection type.</exception>
    public static DbProjectExpression SelectMany(
      this DbExpression source,
      Func<DbExpression, DbExpression> apply)
    {
      Check.NotNull<DbExpression>(source, nameof (source));
      Check.NotNull<Func<DbExpression, DbExpression>>(apply, nameof (apply));
      DbExpression argumentResult;
      DbExpressionBinding binding = DbExpressionBuilder.ConvertToBinding<DbExpression>(source, apply, out argumentResult);
      DbExpressionBinding apply1 = argumentResult.Bind();
      DbExpressionBinding input = binding.CrossApply(apply1).Bind();
      return input.Project((DbExpression) input.Variable.Property(apply1.VariableName));
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbApplyExpression" /> that evaluates the given apply expression once for each element of a given input set, producing a collection of rows with corresponding input and apply columns. Rows for which apply evaluates to an empty set are not included. A
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbProjectExpression" />
    /// is then created that selects the specified selector over each row, producing the overall collection of results.
    /// </summary>
    /// <returns>
    /// An new DbProjectExpression that selects the result of the given selector from a new DbApplyExpression with the specified input and apply bindings and an
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionKind" />
    /// of CrossApply.
    /// </returns>
    /// <param name="source">
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that specifies the input set.
    /// </param>
    /// <param name="apply">A method that represents the logic to evaluate once for each member of the input set. </param>
    /// <param name="selector">
    /// A method that specifies how an element of the result set should be derived given an element of the input and apply sets. This method must produce an instance of a type that is compatible with SelectMany and can be resolved into a
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" />
    /// . Compatibility requirements for  TSelector  are described in remarks.
    /// </param>
    /// <typeparam name="TSelector">The method result type of selector.</typeparam>
    /// <exception cref="T:System.ArgumentNullException">source, apply or selector is null.</exception>
    /// <exception cref="T:System.ArgumentNullException">The expression produced by apply is null.</exception>
    /// <exception cref="T:System.ArgumentNullException">The result of selector is null on conversion to DbExpression.</exception>
    /// <exception cref="T:System.ArgumentException">source does not have a collection result type.</exception>
    /// <exception cref="T:System.ArgumentException">The expression produced by apply does not have a collection type. does not have a collection type. </exception>
    public static DbProjectExpression SelectMany<TSelector>(
      this DbExpression source,
      Func<DbExpression, DbExpression> apply,
      Func<DbExpression, DbExpression, TSelector> selector)
    {
      Check.NotNull<DbExpression>(source, nameof (source));
      Check.NotNull<Func<DbExpression, DbExpression>>(apply, nameof (apply));
      Check.NotNull<Func<DbExpression, DbExpression, TSelector>>(selector, nameof (selector));
      DbExpression argumentResult;
      DbExpressionBinding binding = DbExpressionBuilder.ConvertToBinding<DbExpression>(source, apply, out argumentResult);
      DbExpressionBinding apply1 = argumentResult.Bind();
      DbExpressionBinding input = binding.CrossApply(apply1).Bind();
      DbExpression dbExpression1 = (DbExpression) input.Variable.Property(binding.VariableName);
      DbExpression dbExpression2 = (DbExpression) input.Variable.Property(apply1.VariableName);
      DbExpression expression = DbExpressionBuilder.ResolveToExpression<TSelector>(selector(dbExpression1, dbExpression2));
      return input.Project(expression);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbSkipExpression" /> that skips the specified number of elements from the given sorted input set.
    /// </summary>
    /// <returns>A new DbSkipExpression that represents the skip operation.</returns>
    /// <param name="argument">
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbSortExpression" /> that specifies the sorted input set.
    /// </param>
    /// <param name="count">An expression the specifies how many elements of the ordered set to skip.</param>
    /// <exception cref="T:System.ArgumentNullException">argument or count is null.</exception>
    /// <exception cref="T:System.ArgumentException">
    /// count is not <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbConstantExpression" /> or
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbParameterReferenceExpression" />
    /// or has a result type that is not equal or promotable to a 64-bit integer type.
    /// </exception>
    public static DbSkipExpression Skip(
      this DbSortExpression argument,
      DbExpression count)
    {
      Check.NotNull<DbSortExpression>(argument, nameof (argument));
      return argument.Input.Skip((IEnumerable<DbSortClause>) argument.SortOrder, count);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbLimitExpression" /> that restricts the number of elements in the Argument collection to the specified count Limit value. Tied results are not included in the output.
    /// </summary>
    /// <returns>A new DbLimitExpression with the specified argument and count limit values that does not include tied results.</returns>
    /// <param name="argument">An expression that specifies the input collection.</param>
    /// <param name="count">An expression that specifies the limit value.</param>
    /// <exception cref="T:System.ArgumentNullException">argument or count is null.</exception>
    /// <exception cref="T:System.ArgumentException">argument does not have a collection result type, count does not have a result type that is equal or promotable to a 64-bit integer type.</exception>
    public static DbLimitExpression Take(
      this DbExpression argument,
      DbExpression count)
    {
      Check.NotNull<DbExpression>(argument, nameof (argument));
      Check.NotNull<DbExpression>(count, nameof (count));
      return argument.Limit(count);
    }

    private static DbSortExpression CreateThenBy(
      DbSortExpression source,
      Func<DbExpression, DbExpression> sortKey,
      bool ascending,
      string collation,
      bool useCollation)
    {
      DbExpression key = sortKey((DbExpression) source.Input.Variable);
      DbSortClause dbSortClause = !useCollation ? (ascending ? key.ToSortClause() : key.ToSortClauseDescending()) : (ascending ? key.ToSortClause(collation) : key.ToSortClauseDescending(collation));
      List<DbSortClause> dbSortClauseList = new List<DbSortClause>(source.SortOrder.Count + 1);
      dbSortClauseList.AddRange((IEnumerable<DbSortClause>) source.SortOrder);
      dbSortClauseList.Add(dbSortClause);
      return source.Input.Sort((IEnumerable<DbSortClause>) dbSortClauseList);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbSortExpression" /> that with a sort order that includes the sort order of the given order input set together with the specified sort key in ascending sort order and  with default collation.
    /// </summary>
    /// <returns>A new DbSortExpression that represents the new overall order-by operation.</returns>
    /// <param name="source">A DbSortExpression that specifies the ordered input set.</param>
    /// <param name="sortKey">A method that specifies how to derive the additional sort key expression given a member of the input set. This method must produce an expression with an order-comparable result type that provides the sort key definition.</param>
    /// <exception cref="T:System.ArgumentNullException">source or sortKey is null.</exception>
    /// <exception cref="T:System.ArgumentNullException">The expression produced by sortKey is null.</exception>
    /// <exception cref="T:System.ArgumentException">source does not have a collection result type.</exception>
    /// <exception cref="T:System.ArgumentException">sortKey does not have an order-comparable result type.</exception>
    public static DbSortExpression ThenBy(
      this DbSortExpression source,
      Func<DbExpression, DbExpression> sortKey)
    {
      Check.NotNull<DbSortExpression>(source, nameof (source));
      Check.NotNull<Func<DbExpression, DbExpression>>(sortKey, nameof (sortKey));
      return DbExpressionBuilder.CreateThenBy(source, sortKey, true, (string) null, false);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbSortExpression" /> that with a sort order that includes the sort order of the given order input set together with the specified sort key in ascending sort order and  with the specified collation.
    /// </summary>
    /// <returns>A new DbSortExpression that represents the new overall order-by operation.</returns>
    /// <param name="source">A DbSortExpression that specifies the ordered input set.</param>
    /// <param name="sortKey">A method that specifies how to derive the additional sort key expression given a member of the input set. This method must produce an expression with an order-comparable result type that provides the sort key definition. </param>
    /// <param name="collation">The collation to sort under.</param>
    /// <exception cref="T:System.ArgumentNullException">source, sortKey or collation is null.</exception>
    /// <exception cref="T:System.ArgumentNullException">The expression produced by sortKey is null.</exception>
    /// <exception cref="T:System.ArgumentException">source does not have a collection result type.</exception>
    /// <exception cref="T:System.ArgumentException">The expression produced by sortKey does not have an order-comparable string result type.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">collation is empty or contains only space characters.</exception>
    public static DbSortExpression ThenBy(
      this DbSortExpression source,
      Func<DbExpression, DbExpression> sortKey,
      string collation)
    {
      Check.NotNull<DbSortExpression>(source, nameof (source));
      Check.NotNull<Func<DbExpression, DbExpression>>(sortKey, nameof (sortKey));
      return DbExpressionBuilder.CreateThenBy(source, sortKey, true, collation, true);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbSortExpression" /> that with a sort order that includes the sort order of the given order input set together with the specified sort key in descending sort order and  with default collation.
    /// </summary>
    /// <returns>A new DbSortExpression that represents the new overall order-by operation.</returns>
    /// <param name="source">A DbSortExpression that specifies the ordered input set.</param>
    /// <param name="sortKey">A method that specifies how to derive the additional sort key expression given a member of the input set. This method must produce an expression with an order-comparable result type that provides the sort key definition.</param>
    /// <exception cref="T:System.ArgumentNullException">source or sortKey is null.</exception>
    /// <exception cref="T:System.ArgumentNullException">The expression produced by sortKey is null.</exception>
    /// <exception cref="T:System.ArgumentException">source does not have a collection result type.</exception>
    /// <exception cref="T:System.ArgumentException">The expression produced by sortKey does not have an order-comparable result type.</exception>
    public static DbSortExpression ThenByDescending(
      this DbSortExpression source,
      Func<DbExpression, DbExpression> sortKey)
    {
      Check.NotNull<DbSortExpression>(source, nameof (source));
      Check.NotNull<Func<DbExpression, DbExpression>>(sortKey, nameof (sortKey));
      return DbExpressionBuilder.CreateThenBy(source, sortKey, false, (string) null, false);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbSortExpression" /> that with a sort order that includes the sort order of the given order input set together with the specified sort key in descending sort order and  with the specified collation.
    /// </summary>
    /// <returns>A new DbSortExpression that represents the new overall order-by operation.</returns>
    /// <param name="source">A DbSortExpression that specifies the ordered input set.</param>
    /// <param name="sortKey">A method that specifies how to derive the additional sort key expression given a member of the input set. This method must produce an expression with an order-comparable result type that provides the sort key definition.</param>
    /// <param name="collation">The collation to sort under.</param>
    /// <exception cref="T:System.ArgumentNullException">source, sortKey or collation is null.</exception>
    /// <exception cref="T:System.ArgumentNullException">The expression produced by sortKey is null.</exception>
    /// <exception cref="T:System.ArgumentException">source does not have a collection result type.</exception>
    /// <exception cref="T:System.ArgumentException">The expression produced by sortKey does not have an order-comparable string result type.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">collation is empty or contains only space characters.</exception>
    public static DbSortExpression ThenByDescending(
      this DbSortExpression source,
      Func<DbExpression, DbExpression> sortKey,
      string collation)
    {
      Check.NotNull<DbSortExpression>(source, nameof (source));
      Check.NotNull<Func<DbExpression, DbExpression>>(sortKey, nameof (sortKey));
      return DbExpressionBuilder.CreateThenBy(source, sortKey, false, collation, true);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbFilterExpression" /> that filters the elements in the given input set using the specified predicate.
    /// </summary>
    /// <returns>A new DbQuantifierExpression that represents the Any operation.</returns>
    /// <param name="source">An expression that specifies the input set.</param>
    /// <param name="predicate">A method representing the predicate to evaluate for each member of the input set.    This method must produce an expression with a Boolean result type that provides the predicate logic.</param>
    /// <exception cref="T:System.ArgumentNullException">source or predicate is null.</exception>
    /// <exception cref="T:System.ArgumentNullException">The expression produced by predicate is null.</exception>
    /// <exception cref="T:System.ArgumentException">The expression produced by predicate does not have a Boolean result type.</exception>
    public static DbFilterExpression Where(
      this DbExpression source,
      Func<DbExpression, DbExpression> predicate)
    {
      Check.NotNull<DbExpression>(source, nameof (source));
      Check.NotNull<Func<DbExpression, DbExpression>>(predicate, nameof (predicate));
      DbExpression argumentResult;
      return DbExpressionBuilder.ConvertToBinding<DbExpression>(source, predicate, out argumentResult).Filter(argumentResult);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that computes the union of the left and right set arguments with duplicates removed.
    /// </summary>
    /// <returns>A new DbExpression that computes the union, without duplicates, of the the left and right arguments.</returns>
    /// <param name="left">An expression that defines the left set argument.</param>
    /// <param name="right">An expression that defines the right set argument.</param>
    /// <exception cref="T:System.ArgumentNullException">left or right is null.</exception>
    /// <exception cref="T:System.ArgumentException">No common collection result type with an equality-comparable element type exists between left and right.</exception>
    public static DbExpression Union(this DbExpression left, DbExpression right)
    {
      return (DbExpression) left.UnionAll(right).Distinct();
    }

    internal static AliasGenerator AliasGenerator
    {
      get
      {
        return DbExpressionBuilder._bindingAliases;
      }
    }

    internal static DbNullExpression CreatePrimitiveNullExpression(
      PrimitiveTypeKind primitiveType)
    {
      switch (primitiveType)
      {
        case PrimitiveTypeKind.Binary:
          return DbExpressionBuilder._binaryNull;
        case PrimitiveTypeKind.Boolean:
          return DbExpressionBuilder._boolNull;
        case PrimitiveTypeKind.Byte:
          return DbExpressionBuilder._byteNull;
        case PrimitiveTypeKind.DateTime:
          return DbExpressionBuilder._dateTimeNull;
        case PrimitiveTypeKind.Decimal:
          return DbExpressionBuilder._decimalNull;
        case PrimitiveTypeKind.Double:
          return DbExpressionBuilder._doubleNull;
        case PrimitiveTypeKind.Guid:
          return DbExpressionBuilder._guidNull;
        case PrimitiveTypeKind.Single:
          return DbExpressionBuilder._singleNull;
        case PrimitiveTypeKind.SByte:
          return DbExpressionBuilder._sbyteNull;
        case PrimitiveTypeKind.Int16:
          return DbExpressionBuilder._int16Null;
        case PrimitiveTypeKind.Int32:
          return DbExpressionBuilder._int32Null;
        case PrimitiveTypeKind.Int64:
          return DbExpressionBuilder._int64Null;
        case PrimitiveTypeKind.String:
          return DbExpressionBuilder._stringNull;
        case PrimitiveTypeKind.Time:
          return DbExpressionBuilder._timeNull;
        case PrimitiveTypeKind.DateTimeOffset:
          return DbExpressionBuilder._dateTimeOffsetNull;
        case PrimitiveTypeKind.Geometry:
          return DbExpressionBuilder._geometryNull;
        case PrimitiveTypeKind.Geography:
          return DbExpressionBuilder._geographyNull;
        default:
          string name = typeof (PrimitiveTypeKind).Name;
          throw new ArgumentOutOfRangeException(name, Strings.ADP_InvalidEnumerationValue((object) name, (object) ((int) primitiveType).ToString((IFormatProvider) CultureInfo.InvariantCulture)));
      }
    }

    internal static DbApplyExpression CreateApplyExpressionByKind(
      DbExpressionKind applyKind,
      DbExpressionBinding input,
      DbExpressionBinding apply)
    {
      switch (applyKind)
      {
        case DbExpressionKind.CrossApply:
          return input.CrossApply(apply);
        case DbExpressionKind.OuterApply:
          return input.OuterApply(apply);
        default:
          string name = typeof (DbExpressionKind).Name;
          throw new ArgumentOutOfRangeException(name, Strings.ADP_InvalidEnumerationValue((object) name, (object) ((int) applyKind).ToString((IFormatProvider) CultureInfo.InvariantCulture)));
      }
    }

    internal static DbExpression CreateJoinExpressionByKind(
      DbExpressionKind joinKind,
      DbExpression joinCondition,
      DbExpressionBinding input1,
      DbExpressionBinding input2)
    {
      if (DbExpressionKind.CrossJoin == joinKind)
        return (DbExpression) DbExpressionBuilder.CrossJoin((IEnumerable<DbExpressionBinding>) new DbExpressionBinding[2]
        {
          input1,
          input2
        });
      switch (joinKind)
      {
        case DbExpressionKind.FullOuterJoin:
          return (DbExpression) input1.FullOuterJoin(input2, joinCondition);
        case DbExpressionKind.InnerJoin:
          return (DbExpression) input1.InnerJoin(input2, joinCondition);
        case DbExpressionKind.LeftOuterJoin:
          return (DbExpression) input1.LeftOuterJoin(input2, joinCondition);
        default:
          string name = typeof (DbExpressionKind).Name;
          throw new ArgumentOutOfRangeException(name, Strings.ADP_InvalidEnumerationValue((object) name, (object) ((int) joinKind).ToString((IFormatProvider) CultureInfo.InvariantCulture)));
      }
    }

    internal static DbElementExpression CreateElementExpressionUnwrapSingleProperty(
      DbExpression argument)
    {
      IList<EdmProperty> properties = (IList<EdmProperty>) TypeHelpers.GetProperties(ArgumentValidation.ValidateElement(argument));
      if (properties == null || properties.Count != 1)
        throw new ArgumentException(Strings.Cqt_Element_InvalidArgumentForUnwrapSingleProperty, nameof (argument));
      return new DbElementExpression(properties[0].TypeUsage, argument, true);
    }

    internal static DbRelatedEntityRef CreateRelatedEntityRef(
      RelationshipEndMember sourceEnd,
      RelationshipEndMember targetEnd,
      DbExpression targetEntity)
    {
      return new DbRelatedEntityRef(sourceEnd, targetEnd, targetEntity);
    }

    internal static DbNewInstanceExpression CreateNewEntityWithRelationshipsExpression(
      EntityType entityType,
      IList<DbExpression> attributeValues,
      IList<DbRelatedEntityRef> relationships)
    {
      DbExpressionList validArguments;
      ReadOnlyCollection<DbRelatedEntityRef> validRelatedRefs;
      return new DbNewInstanceExpression(ArgumentValidation.ValidateNewEntityWithRelationships(entityType, (IEnumerable<DbExpression>) attributeValues, relationships, out validArguments, out validRelatedRefs), validArguments, validRelatedRefs);
    }

    internal static DbRelationshipNavigationExpression NavigateAllowingAllRelationshipsInSameTypeHierarchy(
      this DbExpression navigateFrom,
      RelationshipEndMember fromEnd,
      RelationshipEndMember toEnd)
    {
      RelationshipType relType;
      return new DbRelationshipNavigationExpression(ArgumentValidation.ValidateNavigate(navigateFrom, fromEnd, toEnd, out relType, true), relType, fromEnd, toEnd, navigateFrom);
    }

    internal static DbPropertyExpression CreatePropertyExpressionFromMember(
      DbExpression instance,
      EdmMember member)
    {
      return DbExpressionBuilder.PropertyFromMember(instance, member, nameof (member));
    }

    private static TypeUsage CreateCollectionResultType(EdmType type)
    {
      return TypeUsage.Create((EdmType) TypeHelpers.CreateCollectionType(TypeUsage.Create(type)));
    }

    private static TypeUsage CreateCollectionResultType(TypeUsage elementType)
    {
      return TypeUsage.Create((EdmType) TypeHelpers.CreateCollectionType(elementType));
    }

    private static bool IsConstantNegativeInteger(DbExpression expression)
    {
      if (expression.ExpressionKind == DbExpressionKind.Constant && TypeSemantics.IsIntegerNumericType(expression.ResultType))
        return Convert.ToInt64(((DbConstantExpression) expression).Value, (IFormatProvider) CultureInfo.InvariantCulture) < 0L;
      return false;
    }
  }
}
