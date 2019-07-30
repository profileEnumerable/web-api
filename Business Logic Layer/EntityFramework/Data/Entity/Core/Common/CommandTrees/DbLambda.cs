// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.DbLambda
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Utilities;
using System.Reflection;

namespace System.Data.Entity.Core.Common.CommandTrees
{
  /// <summary>
  /// Represents a Lambda function that can be invoked to produce a
  /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbLambdaExpression" />
  /// .
  /// </summary>
  public sealed class DbLambda
  {
    private readonly ReadOnlyCollection<DbVariableReferenceExpression> _variables;
    private readonly DbExpression _body;

    internal DbLambda(
      ReadOnlyCollection<DbVariableReferenceExpression> variables,
      DbExpression bodyExp)
    {
      this._variables = variables;
      this._body = bodyExp;
    }

    /// <summary>Gets the body of the lambda expression.</summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that represents the body of the lambda function.
    /// </returns>
    public DbExpression Body
    {
      get
      {
        return this._body;
      }
    }

    /// <summary>Gets the parameters of the lambda expression.</summary>
    /// <returns>The list of lambda function parameters represented as DbVariableReferenceExpression objects.</returns>
    public IList<DbVariableReferenceExpression> Variables
    {
      get
      {
        return (IList<DbVariableReferenceExpression>) this._variables;
      }
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbLambda" /> with the specified inline Lambda function implementation and formal parameters.
    /// </summary>
    /// <returns>A new DbLambda that describes an inline Lambda function with the specified body and formal parameters</returns>
    /// <param name="body">An expression that defines the logic of the Lambda function</param>
    /// <param name="variables">
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbVariableReferenceExpression" /> collection that represents the formal parameters to the Lambda function.    These variables are valid for use in the body expression.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="variables" />
    /// is null or contains null, or
    /// <paramref name="body" />
    /// is null
    /// </exception>
    /// <exception cref="T:System.ArgumentException">
    /// <paramref name="variables" />
    /// contains more than one element with the same variable name.
    /// </exception>
    public static DbLambda Create(
      DbExpression body,
      IEnumerable<DbVariableReferenceExpression> variables)
    {
      return DbExpressionBuilder.Lambda(body, variables);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbLambda" /> with the specified inline Lambda function implementation and formal parameters.
    /// </summary>
    /// <returns>A new DbLambda that describes an inline Lambda function with the specified body and formal parameters</returns>
    /// <param name="body">An expression that defines the logic of the Lambda function</param>
    /// <param name="variables">
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbVariableReferenceExpression" /> collection that represents the formal parameters to the Lambda function.    These variables are valid for use in the body expression.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="variables" />
    /// is null or contains null, or
    /// <paramref name="body" />
    /// is null.
    /// </exception>
    /// <exception cref="T:System.ArgumentException">
    /// <paramref name="variables" />
    /// contains more than one element with the same variable name.
    /// </exception>
    public static DbLambda Create(
      DbExpression body,
      params DbVariableReferenceExpression[] variables)
    {
      return DbExpressionBuilder.Lambda(body, variables);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbLambda" /> with a single argument of the specified type, as defined by the specified function.
    /// </summary>
    /// <returns>A new DbLambda that describes an inline Lambda function with the specified body and single formal parameter.</returns>
    /// <param name="argument1Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the argument to the Lambda function
    /// </param>
    /// <param name="lambdaFunction">
    /// A function that defines the logic of the Lambda function as a
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" />
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="argument1Type" />
    /// is null, or
    /// <paramref name="lambdaFunction" />
    /// is null or produces a result of null.
    /// </exception>
    public static DbLambda Create(
      TypeUsage argument1Type,
      Func<DbExpression, DbExpression> lambdaFunction)
    {
      Check.NotNull<TypeUsage>(argument1Type, nameof (argument1Type));
      Check.NotNull<Func<DbExpression, DbExpression>>(lambdaFunction, nameof (lambdaFunction));
      DbVariableReferenceExpression[] variables = DbLambda.CreateVariables(lambdaFunction.Method, argument1Type);
      return DbExpressionBuilder.Lambda(lambdaFunction((DbExpression) variables[0]), variables);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbLambda" /> with arguments of the specified types, as defined by the specified function.
    /// </summary>
    /// <returns>A new DbLambda that describes an inline Lambda function with the specified body and formal parameters.</returns>
    /// <param name="argument1Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the first argument to the Lambda function
    /// </param>
    /// <param name="argument2Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the second argument to the Lambda function
    /// </param>
    /// <param name="lambdaFunction">
    /// A function that defines the logic of the Lambda function as a
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" />
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="argument1Type" />
    /// is null,
    /// <paramref name="argument2Type" />
    /// is null, or
    /// <paramref name="lambdaFunction" />
    /// is null or produces a result of null.
    /// </exception>
    public static DbLambda Create(
      TypeUsage argument1Type,
      TypeUsage argument2Type,
      Func<DbExpression, DbExpression, DbExpression> lambdaFunction)
    {
      Check.NotNull<TypeUsage>(argument1Type, nameof (argument1Type));
      Check.NotNull<TypeUsage>(argument2Type, nameof (argument2Type));
      Check.NotNull<Func<DbExpression, DbExpression, DbExpression>>(lambdaFunction, nameof (lambdaFunction));
      DbVariableReferenceExpression[] variables = DbLambda.CreateVariables(lambdaFunction.Method, argument1Type, argument2Type);
      return DbExpressionBuilder.Lambda(lambdaFunction((DbExpression) variables[0], (DbExpression) variables[1]), variables);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbLambda" /> with arguments of the specified types, as defined by the specified function.
    /// </summary>
    /// <returns>A new DbLambda that describes an inline Lambda function with the specified body and formal parameters.</returns>
    /// <param name="argument1Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the first argument to the Lambda function
    /// </param>
    /// <param name="argument2Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the second argument to the Lambda function
    /// </param>
    /// <param name="argument3Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the third argument to the Lambda function
    /// </param>
    /// <param name="lambdaFunction">
    /// A function that defines the logic of the Lambda function as a
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" />
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="argument1Type" />
    /// is null,
    /// <paramref name="argument2Type" />
    /// is null,
    /// <paramref name="argument3Type" />
    /// is null, or
    /// <paramref name="lambdaFunction" />
    /// is null or produces a result of null.
    /// </exception>
    public static DbLambda Create(
      TypeUsage argument1Type,
      TypeUsage argument2Type,
      TypeUsage argument3Type,
      Func<DbExpression, DbExpression, DbExpression, DbExpression> lambdaFunction)
    {
      Check.NotNull<TypeUsage>(argument1Type, nameof (argument1Type));
      Check.NotNull<TypeUsage>(argument2Type, nameof (argument2Type));
      Check.NotNull<TypeUsage>(argument3Type, nameof (argument3Type));
      Check.NotNull<Func<DbExpression, DbExpression, DbExpression, DbExpression>>(lambdaFunction, nameof (lambdaFunction));
      DbVariableReferenceExpression[] variables = DbLambda.CreateVariables(lambdaFunction.Method, argument1Type, argument2Type, argument3Type);
      return DbExpressionBuilder.Lambda(lambdaFunction((DbExpression) variables[0], (DbExpression) variables[1], (DbExpression) variables[2]), variables);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbLambda" /> with arguments of the specified types, as defined by the specified function.
    /// </summary>
    /// <returns>A new DbLambda that describes an inline Lambda function with the specified body and formal parameters.</returns>
    /// <param name="argument1Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the first argument to the Lambda function
    /// </param>
    /// <param name="argument2Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the second argument to the Lambda function
    /// </param>
    /// <param name="argument3Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the third argument to the Lambda function
    /// </param>
    /// <param name="argument4Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the fourth argument to the Lambda function
    /// </param>
    /// <param name="lambdaFunction">
    /// A function that defines the logic of the Lambda function as a
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" />
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="argument1Type" />
    /// is null,
    /// <paramref name="argument2Type" />
    /// is null,
    /// <paramref name="argument3Type" />
    /// is null,
    /// <paramref name="argument4Type" />
    /// is null, or
    /// <paramref name="lambdaFunction" />
    /// is null or produces a result of null.
    /// </exception>
    public static DbLambda Create(
      TypeUsage argument1Type,
      TypeUsage argument2Type,
      TypeUsage argument3Type,
      TypeUsage argument4Type,
      Func<DbExpression, DbExpression, DbExpression, DbExpression, DbExpression> lambdaFunction)
    {
      Check.NotNull<TypeUsage>(argument1Type, nameof (argument1Type));
      Check.NotNull<TypeUsage>(argument2Type, nameof (argument2Type));
      Check.NotNull<TypeUsage>(argument3Type, nameof (argument3Type));
      Check.NotNull<TypeUsage>(argument4Type, nameof (argument4Type));
      Check.NotNull<Func<DbExpression, DbExpression, DbExpression, DbExpression, DbExpression>>(lambdaFunction, nameof (lambdaFunction));
      DbVariableReferenceExpression[] variables = DbLambda.CreateVariables(lambdaFunction.Method, argument1Type, argument2Type, argument3Type, argument4Type);
      return DbExpressionBuilder.Lambda(lambdaFunction((DbExpression) variables[0], (DbExpression) variables[1], (DbExpression) variables[2], (DbExpression) variables[3]), variables);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbLambda" /> with arguments of the specified types, as defined by the specified function.
    /// </summary>
    /// <returns>A new DbLambda that describes an inline Lambda function with the specified body and formal parameters.</returns>
    /// <param name="argument1Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the first argument to the Lambda function
    /// </param>
    /// <param name="argument2Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the second argument to the Lambda function
    /// </param>
    /// <param name="argument3Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the third argument to the Lambda function
    /// </param>
    /// <param name="argument4Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the fourth argument to the Lambda function
    /// </param>
    /// <param name="argument5Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the fifth argument to the Lambda function
    /// </param>
    /// <param name="lambdaFunction">
    /// A function that defines the logic of the Lambda function as a
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" />
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="argument1Type" />
    /// is null,
    /// <paramref name="argument2Type" />
    /// is null,
    /// <paramref name="argument3Type" />
    /// is null,
    /// <paramref name="argument4Type" />
    /// is null,
    /// <paramref name="argument5Type" />
    /// is null, or
    /// <paramref name="lambdaFunction" />
    /// is null or produces a result of null.
    /// </exception>
    public static DbLambda Create(
      TypeUsage argument1Type,
      TypeUsage argument2Type,
      TypeUsage argument3Type,
      TypeUsage argument4Type,
      TypeUsage argument5Type,
      Func<DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression> lambdaFunction)
    {
      Check.NotNull<TypeUsage>(argument1Type, nameof (argument1Type));
      Check.NotNull<TypeUsage>(argument2Type, nameof (argument2Type));
      Check.NotNull<TypeUsage>(argument3Type, nameof (argument3Type));
      Check.NotNull<TypeUsage>(argument4Type, nameof (argument4Type));
      Check.NotNull<TypeUsage>(argument5Type, nameof (argument5Type));
      Check.NotNull<Func<DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression>>(lambdaFunction, nameof (lambdaFunction));
      DbVariableReferenceExpression[] variables = DbLambda.CreateVariables(lambdaFunction.Method, argument1Type, argument2Type, argument3Type, argument4Type, argument5Type);
      return DbExpressionBuilder.Lambda(lambdaFunction((DbExpression) variables[0], (DbExpression) variables[1], (DbExpression) variables[2], (DbExpression) variables[3], (DbExpression) variables[4]), variables);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbLambda" /> with arguments of the specified types, as defined by the specified function.
    /// </summary>
    /// <returns>A new DbLambda that describes an inline Lambda function with the specified body and formal parameters.</returns>
    /// <param name="argument1Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the first argument to the Lambda function
    /// </param>
    /// <param name="argument2Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the second argument to the Lambda function
    /// </param>
    /// <param name="argument3Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the third argument to the Lambda function
    /// </param>
    /// <param name="argument4Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the fourth argument to the Lambda function
    /// </param>
    /// <param name="argument5Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the fifth argument to the Lambda function
    /// </param>
    /// <param name="argument6Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the sixth argument to the Lambda function
    /// </param>
    /// <param name="lambdaFunction">
    /// A function that defines the logic of the Lambda function as a
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" />
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="argument1Type" />
    /// is null,
    /// <paramref name="argument2Type" />
    /// is null,
    /// <paramref name="argument3Type" />
    /// is null,
    /// <paramref name="argument4Type" />
    /// is null,
    /// <paramref name="argument5Type" />
    /// is null,
    /// <paramref name="argument6Type" />
    /// is null, or
    /// <paramref name="lambdaFunction" />
    /// is null or produces a result of null.
    /// </exception>
    public static DbLambda Create(
      TypeUsage argument1Type,
      TypeUsage argument2Type,
      TypeUsage argument3Type,
      TypeUsage argument4Type,
      TypeUsage argument5Type,
      TypeUsage argument6Type,
      Func<DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression> lambdaFunction)
    {
      Check.NotNull<TypeUsage>(argument1Type, nameof (argument1Type));
      Check.NotNull<TypeUsage>(argument2Type, nameof (argument2Type));
      Check.NotNull<TypeUsage>(argument3Type, nameof (argument3Type));
      Check.NotNull<TypeUsage>(argument4Type, nameof (argument4Type));
      Check.NotNull<TypeUsage>(argument5Type, nameof (argument5Type));
      Check.NotNull<TypeUsage>(argument6Type, nameof (argument6Type));
      Check.NotNull<Func<DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression>>(lambdaFunction, nameof (lambdaFunction));
      DbVariableReferenceExpression[] variables = DbLambda.CreateVariables(lambdaFunction.Method, argument1Type, argument2Type, argument3Type, argument4Type, argument5Type, argument6Type);
      return DbExpressionBuilder.Lambda(lambdaFunction((DbExpression) variables[0], (DbExpression) variables[1], (DbExpression) variables[2], (DbExpression) variables[3], (DbExpression) variables[4], (DbExpression) variables[5]), variables);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbLambda" /> with arguments of the specified types, as defined by the specified function.
    /// </summary>
    /// <returns>A new DbLambda that describes an inline Lambda function with the specified body and formal parameters.</returns>
    /// <param name="argument1Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the first argument to the Lambda function
    /// </param>
    /// <param name="argument2Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the second argument to the Lambda function
    /// </param>
    /// <param name="argument3Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the third argument to the Lambda function
    /// </param>
    /// <param name="argument4Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the fourth argument to the Lambda function
    /// </param>
    /// <param name="argument5Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the fifth argument to the Lambda function
    /// </param>
    /// <param name="argument6Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the sixth argument to the Lambda function
    /// </param>
    /// <param name="argument7Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the seventh argument to the Lambda function
    /// </param>
    /// <param name="lambdaFunction">
    /// A function that defines the logic of the Lambda function as a
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" />
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="argument1Type" />
    /// is null,
    /// <paramref name="argument2Type" />
    /// is null,
    /// <paramref name="argument3Type" />
    /// is null,
    /// <paramref name="argument4Type" />
    /// is null,
    /// <paramref name="argument5Type" />
    /// is null,
    /// <paramref name="argument6Type" />
    /// is null,
    /// <paramref name="argument7Type" />
    /// is null, or
    /// <paramref name="lambdaFunction" />
    /// is null or produces a result of null.
    /// </exception>
    public static DbLambda Create(
      TypeUsage argument1Type,
      TypeUsage argument2Type,
      TypeUsage argument3Type,
      TypeUsage argument4Type,
      TypeUsage argument5Type,
      TypeUsage argument6Type,
      TypeUsage argument7Type,
      Func<DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression> lambdaFunction)
    {
      Check.NotNull<TypeUsage>(argument1Type, nameof (argument1Type));
      Check.NotNull<TypeUsage>(argument2Type, nameof (argument2Type));
      Check.NotNull<TypeUsage>(argument3Type, nameof (argument3Type));
      Check.NotNull<TypeUsage>(argument4Type, nameof (argument4Type));
      Check.NotNull<TypeUsage>(argument5Type, nameof (argument5Type));
      Check.NotNull<TypeUsage>(argument6Type, nameof (argument6Type));
      Check.NotNull<TypeUsage>(argument7Type, nameof (argument7Type));
      Check.NotNull<Func<DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression>>(lambdaFunction, nameof (lambdaFunction));
      DbVariableReferenceExpression[] variables = DbLambda.CreateVariables(lambdaFunction.Method, argument1Type, argument2Type, argument3Type, argument4Type, argument5Type, argument6Type, argument7Type);
      return DbExpressionBuilder.Lambda(lambdaFunction((DbExpression) variables[0], (DbExpression) variables[1], (DbExpression) variables[2], (DbExpression) variables[3], (DbExpression) variables[4], (DbExpression) variables[5], (DbExpression) variables[6]), variables);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbLambda" /> with arguments of the specified types, as defined by the specified function.
    /// </summary>
    /// <returns>A new DbLambda that describes an inline Lambda function with the specified body and formal parameters.</returns>
    /// <param name="argument1Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the first argument to the Lambda function
    /// </param>
    /// <param name="argument2Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the second argument to the Lambda function
    /// </param>
    /// <param name="argument3Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the third argument to the Lambda function
    /// </param>
    /// <param name="argument4Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the fourth argument to the Lambda function
    /// </param>
    /// <param name="argument5Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the fifth argument to the Lambda function
    /// </param>
    /// <param name="argument6Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the sixth argument to the Lambda function
    /// </param>
    /// <param name="argument7Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the seventh argument to the Lambda function
    /// </param>
    /// <param name="argument8Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the eighth argument to the Lambda function
    /// </param>
    /// <param name="lambdaFunction">
    /// A function that defines the logic of the Lambda function as a
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" />
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="argument1Type" />
    /// is null,
    /// <paramref name="argument2Type" />
    /// is null,
    /// <paramref name="argument3Type" />
    /// is null,
    /// <paramref name="argument4Type" />
    /// is null,
    /// <paramref name="argument5Type" />
    /// is null,
    /// <paramref name="argument6Type" />
    /// is null,
    /// <paramref name="argument7Type" />
    /// is null,
    /// <paramref name="argument8Type" />
    /// is null, or
    /// <paramref name="lambdaFunction" />
    /// is null or produces a result of null.
    /// </exception>
    public static DbLambda Create(
      TypeUsage argument1Type,
      TypeUsage argument2Type,
      TypeUsage argument3Type,
      TypeUsage argument4Type,
      TypeUsage argument5Type,
      TypeUsage argument6Type,
      TypeUsage argument7Type,
      TypeUsage argument8Type,
      Func<DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression> lambdaFunction)
    {
      Check.NotNull<TypeUsage>(argument1Type, nameof (argument1Type));
      Check.NotNull<TypeUsage>(argument2Type, nameof (argument2Type));
      Check.NotNull<TypeUsage>(argument3Type, nameof (argument3Type));
      Check.NotNull<TypeUsage>(argument4Type, nameof (argument4Type));
      Check.NotNull<TypeUsage>(argument5Type, nameof (argument5Type));
      Check.NotNull<TypeUsage>(argument6Type, nameof (argument6Type));
      Check.NotNull<TypeUsage>(argument7Type, nameof (argument7Type));
      Check.NotNull<TypeUsage>(argument8Type, nameof (argument8Type));
      Check.NotNull<Func<DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression>>(lambdaFunction, nameof (lambdaFunction));
      DbVariableReferenceExpression[] variables = DbLambda.CreateVariables(lambdaFunction.Method, argument1Type, argument2Type, argument3Type, argument4Type, argument5Type, argument6Type, argument7Type, argument8Type);
      return DbExpressionBuilder.Lambda(lambdaFunction((DbExpression) variables[0], (DbExpression) variables[1], (DbExpression) variables[2], (DbExpression) variables[3], (DbExpression) variables[4], (DbExpression) variables[5], (DbExpression) variables[6], (DbExpression) variables[7]), variables);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbLambda" /> with arguments of the specified types, as defined by the specified function.
    /// </summary>
    /// <returns>A new DbLambda that describes an inline Lambda function with the specified body and formal parameters.</returns>
    /// <param name="argument1Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the first argument to the Lambda function
    /// </param>
    /// <param name="argument2Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the second argument to the Lambda function
    /// </param>
    /// <param name="argument3Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the third argument to the Lambda function
    /// </param>
    /// <param name="argument4Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the fourth argument to the Lambda function
    /// </param>
    /// <param name="argument5Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the fifth argument to the Lambda function
    /// </param>
    /// <param name="argument6Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the sixth argument to the Lambda function
    /// </param>
    /// <param name="argument7Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the seventh argument to the Lambda function
    /// </param>
    /// <param name="argument8Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the eighth argument to the Lambda function
    /// </param>
    /// <param name="argument9Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the ninth argument to the Lambda function
    /// </param>
    /// <param name="lambdaFunction">
    /// A function that defines the logic of the Lambda function as a
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" />
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="argument1Type" />
    /// is null,
    /// <paramref name="argument2Type" />
    /// is null,
    /// <paramref name="argument3Type" />
    /// is null,
    /// <paramref name="argument4Type" />
    /// is null,
    /// <paramref name="argument5Type" />
    /// is null,
    /// <paramref name="argument6Type" />
    /// is null,
    /// <paramref name="argument7Type" />
    /// is null,
    /// <paramref name="argument8Type" />
    /// is null,
    /// <paramref name="argument9Type" />
    /// is null, or
    /// <paramref name="lambdaFunction" />
    /// is null or produces a result of null.
    /// </exception>
    public static DbLambda Create(
      TypeUsage argument1Type,
      TypeUsage argument2Type,
      TypeUsage argument3Type,
      TypeUsage argument4Type,
      TypeUsage argument5Type,
      TypeUsage argument6Type,
      TypeUsage argument7Type,
      TypeUsage argument8Type,
      TypeUsage argument9Type,
      Func<DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression> lambdaFunction)
    {
      Check.NotNull<TypeUsage>(argument1Type, nameof (argument1Type));
      Check.NotNull<TypeUsage>(argument2Type, nameof (argument2Type));
      Check.NotNull<TypeUsage>(argument3Type, nameof (argument3Type));
      Check.NotNull<TypeUsage>(argument4Type, nameof (argument4Type));
      Check.NotNull<TypeUsage>(argument5Type, nameof (argument5Type));
      Check.NotNull<TypeUsage>(argument6Type, nameof (argument6Type));
      Check.NotNull<TypeUsage>(argument7Type, nameof (argument7Type));
      Check.NotNull<TypeUsage>(argument8Type, nameof (argument8Type));
      Check.NotNull<TypeUsage>(argument9Type, nameof (argument9Type));
      Check.NotNull<Func<DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression>>(lambdaFunction, nameof (lambdaFunction));
      DbVariableReferenceExpression[] variables = DbLambda.CreateVariables(lambdaFunction.Method, argument1Type, argument2Type, argument3Type, argument4Type, argument5Type, argument6Type, argument7Type, argument8Type, argument9Type);
      return DbExpressionBuilder.Lambda(lambdaFunction((DbExpression) variables[0], (DbExpression) variables[1], (DbExpression) variables[2], (DbExpression) variables[3], (DbExpression) variables[4], (DbExpression) variables[5], (DbExpression) variables[6], (DbExpression) variables[7], (DbExpression) variables[8]), variables);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbLambda" /> with arguments of the specified types, as defined by the specified function.
    /// </summary>
    /// <returns>A new DbLambda that describes an inline Lambda function with the specified body and formal parameters.</returns>
    /// <param name="argument1Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the first argument to the Lambda function
    /// </param>
    /// <param name="argument2Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the second argument to the Lambda function
    /// </param>
    /// <param name="argument3Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the third argument to the Lambda function
    /// </param>
    /// <param name="argument4Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the fourth argument to the Lambda function
    /// </param>
    /// <param name="argument5Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the fifth argument to the Lambda function
    /// </param>
    /// <param name="argument6Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the sixth argument to the Lambda function
    /// </param>
    /// <param name="argument7Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the seventh argument to the Lambda function
    /// </param>
    /// <param name="argument8Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the eighth argument to the Lambda function
    /// </param>
    /// <param name="argument9Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the ninth argument to the Lambda function
    /// </param>
    /// <param name="argument10Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the tenth argument to the Lambda function
    /// </param>
    /// <param name="lambdaFunction">
    /// A function that defines the logic of the Lambda function as a
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" />
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="argument1Type" />
    /// is null,
    /// <paramref name="argument2Type" />
    /// is null,
    /// <paramref name="argument3Type" />
    /// is null,
    /// <paramref name="argument4Type" />
    /// is null,
    /// <paramref name="argument5Type" />
    /// is null,
    /// <paramref name="argument6Type" />
    /// is null,
    /// <paramref name="argument7Type" />
    /// is null,
    /// <paramref name="argument8Type" />
    /// is null,
    /// <paramref name="argument9Type" />
    /// is null,
    /// <paramref name="argument10Type" />
    /// is null, or
    /// <paramref name="lambdaFunction" />
    /// is null or produces a result of null.
    /// </exception>
    public static DbLambda Create(
      TypeUsage argument1Type,
      TypeUsage argument2Type,
      TypeUsage argument3Type,
      TypeUsage argument4Type,
      TypeUsage argument5Type,
      TypeUsage argument6Type,
      TypeUsage argument7Type,
      TypeUsage argument8Type,
      TypeUsage argument9Type,
      TypeUsage argument10Type,
      Func<DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression> lambdaFunction)
    {
      Check.NotNull<TypeUsage>(argument1Type, nameof (argument1Type));
      Check.NotNull<TypeUsage>(argument2Type, nameof (argument2Type));
      Check.NotNull<TypeUsage>(argument3Type, nameof (argument3Type));
      Check.NotNull<TypeUsage>(argument4Type, nameof (argument4Type));
      Check.NotNull<TypeUsage>(argument5Type, nameof (argument5Type));
      Check.NotNull<TypeUsage>(argument6Type, nameof (argument6Type));
      Check.NotNull<TypeUsage>(argument7Type, nameof (argument7Type));
      Check.NotNull<TypeUsage>(argument8Type, nameof (argument8Type));
      Check.NotNull<TypeUsage>(argument9Type, nameof (argument9Type));
      Check.NotNull<TypeUsage>(argument10Type, nameof (argument10Type));
      Check.NotNull<Func<DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression>>(lambdaFunction, nameof (lambdaFunction));
      DbVariableReferenceExpression[] variables = DbLambda.CreateVariables(lambdaFunction.Method, argument1Type, argument2Type, argument3Type, argument4Type, argument5Type, argument6Type, argument7Type, argument8Type, argument9Type, argument10Type);
      return DbExpressionBuilder.Lambda(lambdaFunction((DbExpression) variables[0], (DbExpression) variables[1], (DbExpression) variables[2], (DbExpression) variables[3], (DbExpression) variables[4], (DbExpression) variables[5], (DbExpression) variables[6], (DbExpression) variables[7], (DbExpression) variables[8], (DbExpression) variables[9]), variables);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbLambda" /> with arguments of the specified types, as defined by the specified function.
    /// </summary>
    /// <returns>A new DbLambda that describes an inline Lambda function with the specified body and formal parameters.</returns>
    /// <param name="argument1Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the first argument to the Lambda function
    /// </param>
    /// <param name="argument2Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the second argument to the Lambda function
    /// </param>
    /// <param name="argument3Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the third argument to the Lambda function
    /// </param>
    /// <param name="argument4Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the fourth argument to the Lambda function
    /// </param>
    /// <param name="argument5Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the fifth argument to the Lambda function
    /// </param>
    /// <param name="argument6Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the sixth argument to the Lambda function
    /// </param>
    /// <param name="argument7Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the seventh argument to the Lambda function
    /// </param>
    /// <param name="argument8Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the eighth argument to the Lambda function
    /// </param>
    /// <param name="argument9Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the ninth argument to the Lambda function
    /// </param>
    /// <param name="argument10Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the tenth argument to the Lambda function
    /// </param>
    /// <param name="argument11Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the eleventh argument to the Lambda function
    /// </param>
    /// <param name="lambdaFunction">
    /// A function that defines the logic of the Lambda function as a
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" />
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="argument1Type" />
    /// is null,
    /// <paramref name="argument2Type" />
    /// is null,
    /// <paramref name="argument3Type" />
    /// is null,
    /// <paramref name="argument4Type" />
    /// is null,
    /// <paramref name="argument5Type" />
    /// is null,
    /// <paramref name="argument6Type" />
    /// is null,
    /// <paramref name="argument7Type" />
    /// is null,
    /// <paramref name="argument8Type" />
    /// is null,
    /// <paramref name="argument9Type" />
    /// is null,
    /// <paramref name="argument10Type" />
    /// is null,
    /// <paramref name="argument11Type" />
    /// is null, or
    /// <paramref name="lambdaFunction" />
    /// is null or produces a result of null.
    /// </exception>
    public static DbLambda Create(
      TypeUsage argument1Type,
      TypeUsage argument2Type,
      TypeUsage argument3Type,
      TypeUsage argument4Type,
      TypeUsage argument5Type,
      TypeUsage argument6Type,
      TypeUsage argument7Type,
      TypeUsage argument8Type,
      TypeUsage argument9Type,
      TypeUsage argument10Type,
      TypeUsage argument11Type,
      Func<DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression> lambdaFunction)
    {
      Check.NotNull<TypeUsage>(argument1Type, nameof (argument1Type));
      Check.NotNull<TypeUsage>(argument2Type, nameof (argument2Type));
      Check.NotNull<TypeUsage>(argument3Type, nameof (argument3Type));
      Check.NotNull<TypeUsage>(argument4Type, nameof (argument4Type));
      Check.NotNull<TypeUsage>(argument5Type, nameof (argument5Type));
      Check.NotNull<TypeUsage>(argument6Type, nameof (argument6Type));
      Check.NotNull<TypeUsage>(argument7Type, nameof (argument7Type));
      Check.NotNull<TypeUsage>(argument8Type, nameof (argument8Type));
      Check.NotNull<TypeUsage>(argument9Type, nameof (argument9Type));
      Check.NotNull<TypeUsage>(argument10Type, nameof (argument10Type));
      Check.NotNull<TypeUsage>(argument11Type, nameof (argument11Type));
      Check.NotNull<Func<DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression>>(lambdaFunction, nameof (lambdaFunction));
      DbVariableReferenceExpression[] variables = DbLambda.CreateVariables(lambdaFunction.Method, argument1Type, argument2Type, argument3Type, argument4Type, argument5Type, argument6Type, argument7Type, argument8Type, argument9Type, argument10Type, argument11Type);
      return DbExpressionBuilder.Lambda(lambdaFunction((DbExpression) variables[0], (DbExpression) variables[1], (DbExpression) variables[2], (DbExpression) variables[3], (DbExpression) variables[4], (DbExpression) variables[5], (DbExpression) variables[6], (DbExpression) variables[7], (DbExpression) variables[8], (DbExpression) variables[9], (DbExpression) variables[10]), variables);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbLambda" /> with arguments of the specified types, as defined by the specified function.
    /// </summary>
    /// <returns>A new DbLambda that describes an inline Lambda function with the specified body and formal parameters.</returns>
    /// <param name="argument1Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the first argument to the Lambda function
    /// </param>
    /// <param name="argument2Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the second argument to the Lambda function
    /// </param>
    /// <param name="argument3Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the third argument to the Lambda function
    /// </param>
    /// <param name="argument4Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the fourth argument to the Lambda function
    /// </param>
    /// <param name="argument5Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the fifth argument to the Lambda function
    /// </param>
    /// <param name="argument6Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the sixth argument to the Lambda function
    /// </param>
    /// <param name="argument7Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the seventh argument to the Lambda function
    /// </param>
    /// <param name="argument8Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the eighth argument to the Lambda function
    /// </param>
    /// <param name="argument9Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the ninth argument to the Lambda function
    /// </param>
    /// <param name="argument10Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the tenth argument to the Lambda function
    /// </param>
    /// <param name="argument11Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the eleventh argument to the Lambda function
    /// </param>
    /// <param name="argument12Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the twelfth argument to the Lambda function
    /// </param>
    /// <param name="lambdaFunction">
    /// A function that defines the logic of the Lambda function as a
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" />
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="argument1Type" />
    /// is null,
    /// <paramref name="argument2Type" />
    /// is null,
    /// <paramref name="argument3Type" />
    /// is null,
    /// <paramref name="argument4Type" />
    /// is null,
    /// <paramref name="argument5Type" />
    /// is null,
    /// <paramref name="argument6Type" />
    /// is null,
    /// <paramref name="argument7Type" />
    /// is null,
    /// <paramref name="argument8Type" />
    /// is null,
    /// <paramref name="argument9Type" />
    /// is null,
    /// <paramref name="argument10Type" />
    /// is null,
    /// <paramref name="argument11Type" />
    /// is null,
    /// <paramref name="argument12Type" />
    /// is null, or
    /// <paramref name="lambdaFunction" />
    /// is null or produces a result of null.
    /// </exception>
    public static DbLambda Create(
      TypeUsage argument1Type,
      TypeUsage argument2Type,
      TypeUsage argument3Type,
      TypeUsage argument4Type,
      TypeUsage argument5Type,
      TypeUsage argument6Type,
      TypeUsage argument7Type,
      TypeUsage argument8Type,
      TypeUsage argument9Type,
      TypeUsage argument10Type,
      TypeUsage argument11Type,
      TypeUsage argument12Type,
      Func<DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression> lambdaFunction)
    {
      Check.NotNull<TypeUsage>(argument1Type, nameof (argument1Type));
      Check.NotNull<TypeUsage>(argument2Type, nameof (argument2Type));
      Check.NotNull<TypeUsage>(argument3Type, nameof (argument3Type));
      Check.NotNull<TypeUsage>(argument4Type, nameof (argument4Type));
      Check.NotNull<TypeUsage>(argument5Type, nameof (argument5Type));
      Check.NotNull<TypeUsage>(argument6Type, nameof (argument6Type));
      Check.NotNull<TypeUsage>(argument7Type, nameof (argument7Type));
      Check.NotNull<TypeUsage>(argument8Type, nameof (argument8Type));
      Check.NotNull<TypeUsage>(argument9Type, nameof (argument9Type));
      Check.NotNull<TypeUsage>(argument10Type, nameof (argument10Type));
      Check.NotNull<TypeUsage>(argument11Type, nameof (argument11Type));
      Check.NotNull<TypeUsage>(argument12Type, nameof (argument12Type));
      Check.NotNull<Func<DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression>>(lambdaFunction, nameof (lambdaFunction));
      DbVariableReferenceExpression[] variables = DbLambda.CreateVariables(lambdaFunction.Method, argument1Type, argument2Type, argument3Type, argument4Type, argument5Type, argument6Type, argument7Type, argument8Type, argument9Type, argument10Type, argument11Type, argument12Type);
      return DbExpressionBuilder.Lambda(lambdaFunction((DbExpression) variables[0], (DbExpression) variables[1], (DbExpression) variables[2], (DbExpression) variables[3], (DbExpression) variables[4], (DbExpression) variables[5], (DbExpression) variables[6], (DbExpression) variables[7], (DbExpression) variables[8], (DbExpression) variables[9], (DbExpression) variables[10], (DbExpression) variables[11]), variables);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbLambda" /> with arguments of the specified types, as defined by the specified function.
    /// </summary>
    /// <returns>A new DbLambda that describes an inline Lambda function with the specified body and formal parameters.</returns>
    /// <param name="argument1Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the first argument to the Lambda function
    /// </param>
    /// <param name="argument2Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the second argument to the Lambda function
    /// </param>
    /// <param name="argument3Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the third argument to the Lambda function
    /// </param>
    /// <param name="argument4Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the fourth argument to the Lambda function
    /// </param>
    /// <param name="argument5Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the fifth argument to the Lambda function
    /// </param>
    /// <param name="argument6Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the sixth argument to the Lambda function
    /// </param>
    /// <param name="argument7Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the seventh argument to the Lambda function
    /// </param>
    /// <param name="argument8Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the eighth argument to the Lambda function
    /// </param>
    /// <param name="argument9Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the ninth argument to the Lambda function
    /// </param>
    /// <param name="argument10Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the tenth argument to the Lambda function
    /// </param>
    /// <param name="argument11Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the eleventh argument to the Lambda function
    /// </param>
    /// <param name="argument12Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the twelfth argument to the Lambda function
    /// </param>
    /// <param name="argument13Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the thirteenth argument to the Lambda function
    /// </param>
    /// <param name="lambdaFunction">
    /// A function that defines the logic of the Lambda function as a
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" />
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="argument1Type" />
    /// is null,
    /// <paramref name="argument2Type" />
    /// is null,
    /// <paramref name="argument3Type" />
    /// is null,
    /// <paramref name="argument4Type" />
    /// is null,
    /// <paramref name="argument5Type" />
    /// is null,
    /// <paramref name="argument6Type" />
    /// is null,
    /// <paramref name="argument7Type" />
    /// is null,
    /// <paramref name="argument8Type" />
    /// is null,
    /// <paramref name="argument9Type" />
    /// is null,
    /// <paramref name="argument10Type" />
    /// is null,
    /// <paramref name="argument11Type" />
    /// is null,
    /// <paramref name="argument12Type" />
    /// is null,
    /// <paramref name="argument13Type" />
    /// is null, or
    /// <paramref name="lambdaFunction" />
    /// is null or produces a result of null.
    /// </exception>
    public static DbLambda Create(
      TypeUsage argument1Type,
      TypeUsage argument2Type,
      TypeUsage argument3Type,
      TypeUsage argument4Type,
      TypeUsage argument5Type,
      TypeUsage argument6Type,
      TypeUsage argument7Type,
      TypeUsage argument8Type,
      TypeUsage argument9Type,
      TypeUsage argument10Type,
      TypeUsage argument11Type,
      TypeUsage argument12Type,
      TypeUsage argument13Type,
      Func<DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression> lambdaFunction)
    {
      Check.NotNull<TypeUsage>(argument1Type, nameof (argument1Type));
      Check.NotNull<TypeUsage>(argument2Type, nameof (argument2Type));
      Check.NotNull<TypeUsage>(argument3Type, nameof (argument3Type));
      Check.NotNull<TypeUsage>(argument4Type, nameof (argument4Type));
      Check.NotNull<TypeUsage>(argument5Type, nameof (argument5Type));
      Check.NotNull<TypeUsage>(argument6Type, nameof (argument6Type));
      Check.NotNull<TypeUsage>(argument7Type, nameof (argument7Type));
      Check.NotNull<TypeUsage>(argument8Type, nameof (argument8Type));
      Check.NotNull<TypeUsage>(argument9Type, nameof (argument9Type));
      Check.NotNull<TypeUsage>(argument10Type, nameof (argument10Type));
      Check.NotNull<TypeUsage>(argument11Type, nameof (argument11Type));
      Check.NotNull<TypeUsage>(argument12Type, nameof (argument12Type));
      Check.NotNull<TypeUsage>(argument13Type, nameof (argument13Type));
      Check.NotNull<Func<DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression>>(lambdaFunction, nameof (lambdaFunction));
      DbVariableReferenceExpression[] variables = DbLambda.CreateVariables(lambdaFunction.Method, argument1Type, argument2Type, argument3Type, argument4Type, argument5Type, argument6Type, argument7Type, argument8Type, argument9Type, argument10Type, argument11Type, argument12Type, argument13Type);
      return DbExpressionBuilder.Lambda(lambdaFunction((DbExpression) variables[0], (DbExpression) variables[1], (DbExpression) variables[2], (DbExpression) variables[3], (DbExpression) variables[4], (DbExpression) variables[5], (DbExpression) variables[6], (DbExpression) variables[7], (DbExpression) variables[8], (DbExpression) variables[9], (DbExpression) variables[10], (DbExpression) variables[11], (DbExpression) variables[12]), variables);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbLambda" /> with arguments of the specified types, as defined by the specified function.
    /// </summary>
    /// <returns>A new DbLambda that describes an inline Lambda function with the specified body and formal parameters.</returns>
    /// <param name="argument1Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the first argument to the Lambda function
    /// </param>
    /// <param name="argument2Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the second argument to the Lambda function
    /// </param>
    /// <param name="argument3Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the third argument to the Lambda function
    /// </param>
    /// <param name="argument4Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the fourth argument to the Lambda function
    /// </param>
    /// <param name="argument5Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the fifth argument to the Lambda function
    /// </param>
    /// <param name="argument6Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the sixth argument to the Lambda function
    /// </param>
    /// <param name="argument7Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the seventh argument to the Lambda function
    /// </param>
    /// <param name="argument8Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the eighth argument to the Lambda function
    /// </param>
    /// <param name="argument9Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the ninth argument to the Lambda function
    /// </param>
    /// <param name="argument10Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the tenth argument to the Lambda function
    /// </param>
    /// <param name="argument11Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the eleventh argument to the Lambda function
    /// </param>
    /// <param name="argument12Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the twelfth argument to the Lambda function
    /// </param>
    /// <param name="argument13Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the thirteenth argument to the Lambda function
    /// </param>
    /// <param name="argument14Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the fourteenth argument to the Lambda function
    /// </param>
    /// <param name="lambdaFunction">
    /// A function that defines the logic of the Lambda function as a
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" />
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="argument1Type" />
    /// is null,
    /// <paramref name="argument2Type" />
    /// is null,
    /// <paramref name="argument3Type" />
    /// is null,
    /// <paramref name="argument4Type" />
    /// is null,
    /// <paramref name="argument5Type" />
    /// is null,
    /// <paramref name="argument6Type" />
    /// is null,
    /// <paramref name="argument7Type" />
    /// is null,
    /// <paramref name="argument8Type" />
    /// is null,
    /// <paramref name="argument9Type" />
    /// is null,
    /// <paramref name="argument10Type" />
    /// is null,
    /// <paramref name="argument11Type" />
    /// is null,
    /// <paramref name="argument12Type" />
    /// is null,
    /// <paramref name="argument13Type" />
    /// is null,
    /// <paramref name="argument14Type" />
    /// is null, or
    /// <paramref name="lambdaFunction" />
    /// is null or produces a result of null.
    /// </exception>
    public static DbLambda Create(
      TypeUsage argument1Type,
      TypeUsage argument2Type,
      TypeUsage argument3Type,
      TypeUsage argument4Type,
      TypeUsage argument5Type,
      TypeUsage argument6Type,
      TypeUsage argument7Type,
      TypeUsage argument8Type,
      TypeUsage argument9Type,
      TypeUsage argument10Type,
      TypeUsage argument11Type,
      TypeUsage argument12Type,
      TypeUsage argument13Type,
      TypeUsage argument14Type,
      Func<DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression> lambdaFunction)
    {
      Check.NotNull<TypeUsage>(argument1Type, nameof (argument1Type));
      Check.NotNull<TypeUsage>(argument2Type, nameof (argument2Type));
      Check.NotNull<TypeUsage>(argument3Type, nameof (argument3Type));
      Check.NotNull<TypeUsage>(argument4Type, nameof (argument4Type));
      Check.NotNull<TypeUsage>(argument5Type, nameof (argument5Type));
      Check.NotNull<TypeUsage>(argument6Type, nameof (argument6Type));
      Check.NotNull<TypeUsage>(argument7Type, nameof (argument7Type));
      Check.NotNull<TypeUsage>(argument8Type, nameof (argument8Type));
      Check.NotNull<TypeUsage>(argument9Type, nameof (argument9Type));
      Check.NotNull<TypeUsage>(argument10Type, nameof (argument10Type));
      Check.NotNull<TypeUsage>(argument11Type, nameof (argument11Type));
      Check.NotNull<TypeUsage>(argument12Type, nameof (argument12Type));
      Check.NotNull<TypeUsage>(argument13Type, nameof (argument13Type));
      Check.NotNull<TypeUsage>(argument14Type, nameof (argument14Type));
      Check.NotNull<Func<DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression>>(lambdaFunction, nameof (lambdaFunction));
      DbVariableReferenceExpression[] variables = DbLambda.CreateVariables(lambdaFunction.Method, argument1Type, argument2Type, argument3Type, argument4Type, argument5Type, argument6Type, argument7Type, argument8Type, argument9Type, argument10Type, argument11Type, argument12Type, argument13Type, argument14Type);
      return DbExpressionBuilder.Lambda(lambdaFunction((DbExpression) variables[0], (DbExpression) variables[1], (DbExpression) variables[2], (DbExpression) variables[3], (DbExpression) variables[4], (DbExpression) variables[5], (DbExpression) variables[6], (DbExpression) variables[7], (DbExpression) variables[8], (DbExpression) variables[9], (DbExpression) variables[10], (DbExpression) variables[11], (DbExpression) variables[12], (DbExpression) variables[13]), variables);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbLambda" /> with arguments of the specified types, as defined by the specified function.
    /// </summary>
    /// <returns>A new DbLambda that describes an inline Lambda function with the specified body and formal parameters.</returns>
    /// <param name="argument1Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the first argument to the Lambda function
    /// </param>
    /// <param name="argument2Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the second argument to the Lambda function
    /// </param>
    /// <param name="argument3Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the third argument to the Lambda function
    /// </param>
    /// <param name="argument4Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the fourth argument to the Lambda function
    /// </param>
    /// <param name="argument5Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the fifth argument to the Lambda function
    /// </param>
    /// <param name="argument6Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the sixth argument to the Lambda function
    /// </param>
    /// <param name="argument7Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the seventh argument to the Lambda function
    /// </param>
    /// <param name="argument8Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the eighth argument to the Lambda function
    /// </param>
    /// <param name="argument9Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the ninth argument to the Lambda function
    /// </param>
    /// <param name="argument10Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the tenth argument to the Lambda function
    /// </param>
    /// <param name="argument11Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the eleventh argument to the Lambda function
    /// </param>
    /// <param name="argument12Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the twelfth argument to the Lambda function
    /// </param>
    /// <param name="argument13Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the thirteenth argument to the Lambda function
    /// </param>
    /// <param name="argument14Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the fourteenth argument to the Lambda function
    /// </param>
    /// <param name="argument15Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the fifteenth argument to the Lambda function
    /// </param>
    /// <param name="lambdaFunction">
    /// A function that defines the logic of the Lambda function as a
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" />
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="argument1Type" />
    /// is null,
    /// <paramref name="argument2Type" />
    /// is null,
    /// <paramref name="argument3Type" />
    /// is null,
    /// <paramref name="argument4Type" />
    /// is null,
    /// <paramref name="argument5Type" />
    /// is null,
    /// <paramref name="argument6Type" />
    /// is null,
    /// <paramref name="argument7Type" />
    /// is null,
    /// <paramref name="argument8Type" />
    /// is null,
    /// <paramref name="argument9Type" />
    /// is null,
    /// <paramref name="argument10Type" />
    /// is null,
    /// <paramref name="argument11Type" />
    /// is null,
    /// <paramref name="argument12Type" />
    /// is null,
    /// <paramref name="argument13Type" />
    /// is null,
    /// <paramref name="argument14Type" />
    /// is null,
    /// <paramref name="argument15Type" />
    /// is null,
    /// or
    /// <paramref name="lambdaFunction" />
    /// is null or produces a result of null.
    /// </exception>
    public static DbLambda Create(
      TypeUsage argument1Type,
      TypeUsage argument2Type,
      TypeUsage argument3Type,
      TypeUsage argument4Type,
      TypeUsage argument5Type,
      TypeUsage argument6Type,
      TypeUsage argument7Type,
      TypeUsage argument8Type,
      TypeUsage argument9Type,
      TypeUsage argument10Type,
      TypeUsage argument11Type,
      TypeUsage argument12Type,
      TypeUsage argument13Type,
      TypeUsage argument14Type,
      TypeUsage argument15Type,
      Func<DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression> lambdaFunction)
    {
      Check.NotNull<TypeUsage>(argument1Type, nameof (argument1Type));
      Check.NotNull<TypeUsage>(argument2Type, nameof (argument2Type));
      Check.NotNull<TypeUsage>(argument3Type, nameof (argument3Type));
      Check.NotNull<TypeUsage>(argument4Type, nameof (argument4Type));
      Check.NotNull<TypeUsage>(argument5Type, nameof (argument5Type));
      Check.NotNull<TypeUsage>(argument6Type, nameof (argument6Type));
      Check.NotNull<TypeUsage>(argument7Type, nameof (argument7Type));
      Check.NotNull<TypeUsage>(argument8Type, nameof (argument8Type));
      Check.NotNull<TypeUsage>(argument9Type, nameof (argument9Type));
      Check.NotNull<TypeUsage>(argument10Type, nameof (argument10Type));
      Check.NotNull<TypeUsage>(argument11Type, nameof (argument11Type));
      Check.NotNull<TypeUsage>(argument12Type, nameof (argument12Type));
      Check.NotNull<TypeUsage>(argument13Type, nameof (argument13Type));
      Check.NotNull<TypeUsage>(argument14Type, nameof (argument14Type));
      Check.NotNull<TypeUsage>(argument15Type, nameof (argument15Type));
      Check.NotNull<Func<DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression>>(lambdaFunction, nameof (lambdaFunction));
      DbVariableReferenceExpression[] variables = DbLambda.CreateVariables(lambdaFunction.Method, argument1Type, argument2Type, argument3Type, argument4Type, argument5Type, argument6Type, argument7Type, argument8Type, argument9Type, argument10Type, argument11Type, argument12Type, argument13Type, argument14Type, argument15Type);
      return DbExpressionBuilder.Lambda(lambdaFunction((DbExpression) variables[0], (DbExpression) variables[1], (DbExpression) variables[2], (DbExpression) variables[3], (DbExpression) variables[4], (DbExpression) variables[5], (DbExpression) variables[6], (DbExpression) variables[7], (DbExpression) variables[8], (DbExpression) variables[9], (DbExpression) variables[10], (DbExpression) variables[11], (DbExpression) variables[12], (DbExpression) variables[13], (DbExpression) variables[14]), variables);
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbLambda" /> with arguments of the specified types, as defined by the specified function.
    /// </summary>
    /// <returns>A new DbLambda that describes an inline Lambda function with the specified body and formal parameters.</returns>
    /// <param name="argument1Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the first argument to the Lambda function
    /// </param>
    /// <param name="argument2Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the second argument to the Lambda function
    /// </param>
    /// <param name="argument3Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the third argument to the Lambda function
    /// </param>
    /// <param name="argument4Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the fourth argument to the Lambda function
    /// </param>
    /// <param name="argument5Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the fifth argument to the Lambda function
    /// </param>
    /// <param name="argument6Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the sixth argument to the Lambda function
    /// </param>
    /// <param name="argument7Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the seventh argument to the Lambda function
    /// </param>
    /// <param name="argument8Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the eighth argument to the Lambda function
    /// </param>
    /// <param name="argument9Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the ninth argument to the Lambda function
    /// </param>
    /// <param name="argument10Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the tenth argument to the Lambda function
    /// </param>
    /// <param name="argument11Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the eleventh argument to the Lambda function
    /// </param>
    /// <param name="argument12Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the twelfth argument to the Lambda function
    /// </param>
    /// <param name="argument13Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the thirteenth argument to the Lambda function
    /// </param>
    /// <param name="argument14Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the fourteenth argument to the Lambda function
    /// </param>
    /// <param name="argument15Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the fifteenth argument to the Lambda function
    /// </param>
    /// <param name="argument16Type">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage" /> that defines the EDM type of the sixteenth argument to the Lambda function
    /// </param>
    /// <param name="lambdaFunction">
    /// A function that defines the logic of the Lambda function as a
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" />
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="argument1Type" />
    /// is null,
    /// <paramref name="argument2Type" />
    /// is null,
    /// <paramref name="argument3Type" />
    /// is null,
    /// <paramref name="argument4Type" />
    /// is null,
    /// <paramref name="argument5Type" />
    /// is null,
    /// <paramref name="argument6Type" />
    /// is null,
    /// <paramref name="argument7Type" />
    /// is null,
    /// <paramref name="argument8Type" />
    /// is null,
    /// <paramref name="argument9Type" />
    /// is null,
    /// <paramref name="argument10Type" />
    /// is null,
    /// <paramref name="argument11Type" />
    /// is null,
    /// <paramref name="argument12Type" />
    /// is null,
    /// <paramref name="argument13Type" />
    /// is null,
    /// <paramref name="argument14Type" />
    /// is null,
    /// <paramref name="argument15Type" />
    /// is null,
    /// <paramref name="argument16Type" />
    /// is null, or
    /// <paramref name="lambdaFunction" />
    /// is null or produces a result of null.
    /// </exception>
    public static DbLambda Create(
      TypeUsage argument1Type,
      TypeUsage argument2Type,
      TypeUsage argument3Type,
      TypeUsage argument4Type,
      TypeUsage argument5Type,
      TypeUsage argument6Type,
      TypeUsage argument7Type,
      TypeUsage argument8Type,
      TypeUsage argument9Type,
      TypeUsage argument10Type,
      TypeUsage argument11Type,
      TypeUsage argument12Type,
      TypeUsage argument13Type,
      TypeUsage argument14Type,
      TypeUsage argument15Type,
      TypeUsage argument16Type,
      Func<DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression> lambdaFunction)
    {
      Check.NotNull<TypeUsage>(argument1Type, nameof (argument1Type));
      Check.NotNull<TypeUsage>(argument2Type, nameof (argument2Type));
      Check.NotNull<TypeUsage>(argument3Type, nameof (argument3Type));
      Check.NotNull<TypeUsage>(argument4Type, nameof (argument4Type));
      Check.NotNull<TypeUsage>(argument5Type, nameof (argument5Type));
      Check.NotNull<TypeUsage>(argument6Type, nameof (argument6Type));
      Check.NotNull<TypeUsage>(argument7Type, nameof (argument7Type));
      Check.NotNull<TypeUsage>(argument8Type, nameof (argument8Type));
      Check.NotNull<TypeUsage>(argument9Type, nameof (argument9Type));
      Check.NotNull<TypeUsage>(argument10Type, nameof (argument10Type));
      Check.NotNull<TypeUsage>(argument11Type, nameof (argument11Type));
      Check.NotNull<TypeUsage>(argument12Type, nameof (argument12Type));
      Check.NotNull<TypeUsage>(argument13Type, nameof (argument13Type));
      Check.NotNull<TypeUsage>(argument14Type, nameof (argument14Type));
      Check.NotNull<TypeUsage>(argument15Type, nameof (argument15Type));
      Check.NotNull<TypeUsage>(argument16Type, nameof (argument16Type));
      Check.NotNull<Func<DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression, DbExpression>>(lambdaFunction, nameof (lambdaFunction));
      DbVariableReferenceExpression[] variables = DbLambda.CreateVariables(lambdaFunction.Method, argument1Type, argument2Type, argument3Type, argument4Type, argument5Type, argument6Type, argument7Type, argument8Type, argument9Type, argument10Type, argument11Type, argument12Type, argument13Type, argument14Type, argument15Type, argument16Type);
      return DbExpressionBuilder.Lambda(lambdaFunction((DbExpression) variables[0], (DbExpression) variables[1], (DbExpression) variables[2], (DbExpression) variables[3], (DbExpression) variables[4], (DbExpression) variables[5], (DbExpression) variables[6], (DbExpression) variables[7], (DbExpression) variables[8], (DbExpression) variables[9], (DbExpression) variables[10], (DbExpression) variables[11], (DbExpression) variables[12], (DbExpression) variables[13], (DbExpression) variables[14], (DbExpression) variables[15]), variables);
    }

    private static DbVariableReferenceExpression[] CreateVariables(
      MethodInfo lambdaMethod,
      params TypeUsage[] argumentTypes)
    {
      string[] aliases = DbExpressionBuilder.ExtractAliases(lambdaMethod);
      DbVariableReferenceExpression[] referenceExpressionArray = new DbVariableReferenceExpression[argumentTypes.Length];
      for (int index = 0; index < aliases.Length; ++index)
        referenceExpressionArray[index] = argumentTypes[index].Variable(aliases[index]);
      return referenceExpressionArray;
    }
  }
}
