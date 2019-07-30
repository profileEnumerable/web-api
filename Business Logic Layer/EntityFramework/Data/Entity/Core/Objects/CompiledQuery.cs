// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.CompiledQuery
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Data.Entity.Core.Objects.ELinq;
using System.Data.Entity.Core.Objects.Internal;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Data.Entity.Core.Objects
{
  /// <summary>Caches an ELinq query</summary>
  public sealed class CompiledQuery
  {
    private readonly Guid _cacheToken = Guid.NewGuid();
    private readonly LambdaExpression _query;

    private CompiledQuery(LambdaExpression query)
    {
      Func<bool> recompileRequired;
      this._query = (LambdaExpression) Funcletizer.CreateCompiledQueryLockdownFuncletizer().Funcletize((Expression) query, out recompileRequired);
    }

    /// <summary>Creates a new delegate that represents the compiled LINQ to Entities query.</summary>
    /// <returns>
    /// <see cref="T:System.Func`17" />, a generic delegate that represents the compiled LINQ to Entities query.
    /// </returns>
    /// <param name="query">The lambda expression to compile.</param>
    /// <typeparam name="TArg0">
    /// A type derived from <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />.
    /// </typeparam>
    /// <typeparam name="TArg1">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg1  must be a primitive type.</typeparam>
    /// <typeparam name="TArg2">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg2  must be a primitive type.</typeparam>
    /// <typeparam name="TArg3">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg3  must be a primitive type.</typeparam>
    /// <typeparam name="TArg4">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg4  must be a primitive type.</typeparam>
    /// <typeparam name="TArg5">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg5  must be a primitive type.</typeparam>
    /// <typeparam name="TArg6">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg6  must be a primitive type.</typeparam>
    /// <typeparam name="TArg7">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg7  must be a primitive type.</typeparam>
    /// <typeparam name="TArg8">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg8  must be a primitive type.</typeparam>
    /// <typeparam name="TArg9">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg9  must be a primitive type.</typeparam>
    /// <typeparam name="TArg10">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg10  must be a primitive type.</typeparam>
    /// <typeparam name="TArg11">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg11  must be a primitive type.</typeparam>
    /// <typeparam name="TArg12">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg12  must be a primitive type.</typeparam>
    /// <typeparam name="TArg13">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg13  must be a primitive type.</typeparam>
    /// <typeparam name="TArg14">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg14  must be a primitive type.</typeparam>
    /// <typeparam name="TArg15">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg15  must be a primitive type.</typeparam>
    /// <typeparam name="TResult">
    /// The type  T  of the query results returned by executing the delegate returned by the
    /// <see cref="M:System.Data.Entity.Core.Objects.CompiledQuery.Compile``17(System.Linq.Expressions.Expression{System.Func{``0,``1,``2,``3,``4,``5,``6,``7,``8,``9,``10,``11,``12,``13,``14,``15,``16}})" />
    /// method.
    /// </typeparam>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "required for this feature")]
    public static Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TResult> Compile<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TResult>(
      Expression<Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TResult>> query)
      where TArg0 : ObjectContext
    {
      return new Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TResult>(new CompiledQuery((LambdaExpression) query).Invoke<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TResult>);
    }

    /// <summary>Creates a new delegate that represents the compiled LINQ to Entities query.</summary>
    /// <returns>
    /// <see cref="T:System.Func`16" />, a generic delegate that represents the compiled LINQ to Entities query.
    /// </returns>
    /// <param name="query">The lambda expression to compile.</param>
    /// <typeparam name="TArg0">
    /// A type derived from <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />.
    /// </typeparam>
    /// <typeparam name="TArg1">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg1  must be a primitive type.</typeparam>
    /// <typeparam name="TArg2">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg2  must be a primitive type.</typeparam>
    /// <typeparam name="TArg3">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg3  must be a primitive type.</typeparam>
    /// <typeparam name="TArg4">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg4  must be a primitive type.</typeparam>
    /// <typeparam name="TArg5">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg5  must be a primitive type.</typeparam>
    /// <typeparam name="TArg6">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg6  must be a primitive type.</typeparam>
    /// <typeparam name="TArg7">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg7  must be a primitive type.</typeparam>
    /// <typeparam name="TArg8">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg8  must be a primitive type.</typeparam>
    /// <typeparam name="TArg9">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg9  must be a primitive type.</typeparam>
    /// <typeparam name="TArg10">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg10  must be a primitive type.</typeparam>
    /// <typeparam name="TArg11">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg11  must be a primitive type.</typeparam>
    /// <typeparam name="TArg12">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg12  must be a primitive type.</typeparam>
    /// <typeparam name="TArg13">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg13  must be a primitive type.</typeparam>
    /// <typeparam name="TArg14">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg14  must be a primitive type.</typeparam>
    /// <typeparam name="TResult">
    /// The type  T  of the query results returned by executing the delegate returned by the
    /// <see cref="M:System.Data.Entity.Core.Objects.CompiledQuery.Compile``16(System.Linq.Expressions.Expression{System.Func{``0,``1,``2,``3,``4,``5,``6,``7,``8,``9,``10,``11,``12,``13,``14,``15}})" />
    /// method.
    /// </typeparam>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "required for this feature")]
    public static Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TResult> Compile<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TResult>(
      Expression<Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TResult>> query)
      where TArg0 : ObjectContext
    {
      return new Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TResult>(new CompiledQuery((LambdaExpression) query).Invoke<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TResult>);
    }

    /// <summary>Creates a new delegate that represents the compiled LINQ to Entities query.</summary>
    /// <returns>
    /// <see cref="T:System.Func`15" />, a generic delegate that represents the compiled LINQ to Entities query.
    /// </returns>
    /// <param name="query">The lambda expression to compile.</param>
    /// <typeparam name="TArg0">
    /// A type derived from <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />.
    /// </typeparam>
    /// <typeparam name="TArg1">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg1  must be a primitive type.</typeparam>
    /// <typeparam name="TArg2">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg2  must be a primitive type.</typeparam>
    /// <typeparam name="TArg3">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg3  must be a primitive type.</typeparam>
    /// <typeparam name="TArg4">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg4  must be a primitive type.</typeparam>
    /// <typeparam name="TArg5">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg5  must be a primitive type.</typeparam>
    /// <typeparam name="TArg6">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg6  must be a primitive type.</typeparam>
    /// <typeparam name="TArg7">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg7  must be a primitive type.</typeparam>
    /// <typeparam name="TArg8">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg8  must be a primitive type.</typeparam>
    /// <typeparam name="TArg9">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg9  must be a primitive type.</typeparam>
    /// <typeparam name="TArg10">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg10  must be a primitive type.</typeparam>
    /// <typeparam name="TArg11">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg11  must be a primitive type.</typeparam>
    /// <typeparam name="TArg12">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg12  must be a primitive type.</typeparam>
    /// <typeparam name="TArg13">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg13  must be a primitive type.</typeparam>
    /// <typeparam name="TResult">
    /// The type  T  of the query results returned by executing the delegate returned by the
    /// <see cref="M:System.Data.Entity.Core.Objects.CompiledQuery.Compile``15(System.Linq.Expressions.Expression{System.Func{``0,``1,``2,``3,``4,``5,``6,``7,``8,``9,``10,``11,``12,``13,``14}})" />
    /// method.
    /// </typeparam>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "required for this feature")]
    public static Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TResult> Compile<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TResult>(
      Expression<Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TResult>> query)
      where TArg0 : ObjectContext
    {
      return new Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TResult>(new CompiledQuery((LambdaExpression) query).Invoke<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TResult>);
    }

    /// <summary>Creates a new delegate that represents the compiled LINQ to Entities query.</summary>
    /// <returns>
    /// <see cref="T:System.Func`14" />, a generic delegate that represents the compiled LINQ to Entities query.
    /// </returns>
    /// <param name="query">The lambda expression to compile.</param>
    /// <typeparam name="TArg0">
    /// A type derived from <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />.
    /// </typeparam>
    /// <typeparam name="TArg1">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg1  must be a primitive type.</typeparam>
    /// <typeparam name="TArg2">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg2  must be a primitive type.</typeparam>
    /// <typeparam name="TArg3">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg3  must be a primitive type.</typeparam>
    /// <typeparam name="TArg4">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg4  must be a primitive type.</typeparam>
    /// <typeparam name="TArg5">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg5  must be a primitive type.</typeparam>
    /// <typeparam name="TArg6">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg6  must be a primitive type.</typeparam>
    /// <typeparam name="TArg7">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg7  must be a primitive type.</typeparam>
    /// <typeparam name="TArg8">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg8  must be a primitive type.</typeparam>
    /// <typeparam name="TArg9">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg9  must be a primitive type.</typeparam>
    /// <typeparam name="TArg10">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg10  must be a primitive type.</typeparam>
    /// <typeparam name="TArg11">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg11  must be a primitive type.</typeparam>
    /// <typeparam name="TArg12">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg12  must be a primitive type.</typeparam>
    /// <typeparam name="TResult">
    /// The type  T  of the query results returned by executing the delegate returned by the
    /// <see cref="M:System.Data.Entity.Core.Objects.CompiledQuery.Compile``14(System.Linq.Expressions.Expression{System.Func{``0,``1,``2,``3,``4,``5,``6,``7,``8,``9,``10,``11,``12,``13}})" />
    /// method.
    /// </typeparam>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "required for this feature")]
    public static Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TResult> Compile<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TResult>(
      Expression<Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TResult>> query)
      where TArg0 : ObjectContext
    {
      return new Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TResult>(new CompiledQuery((LambdaExpression) query).Invoke<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TResult>);
    }

    /// <summary>Creates a new delegate that represents the compiled LINQ to Entities query.</summary>
    /// <returns>
    /// <see cref="T:System.Func`13" />, a generic delegate that represents the compiled LINQ to Entities query.
    /// </returns>
    /// <param name="query">The lambda expression to compile.</param>
    /// <typeparam name="TArg0">
    /// A type derived from <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />.
    /// </typeparam>
    /// <typeparam name="TArg1">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg1  must be a primitive type.</typeparam>
    /// <typeparam name="TArg2">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg2  must be a primitive type.</typeparam>
    /// <typeparam name="TArg3">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg3  must be a primitive type.</typeparam>
    /// <typeparam name="TArg4">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg4  must be a primitive type.</typeparam>
    /// <typeparam name="TArg5">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg5  must be a primitive type.</typeparam>
    /// <typeparam name="TArg6">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg6  must be a primitive type.</typeparam>
    /// <typeparam name="TArg7">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg7  must be a primitive type.</typeparam>
    /// <typeparam name="TArg8">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg8  must be a primitive type.</typeparam>
    /// <typeparam name="TArg9">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg9  must be a primitive type.</typeparam>
    /// <typeparam name="TArg10">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg10  must be a primitive type.</typeparam>
    /// <typeparam name="TArg11">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg11  must be a primitive type.</typeparam>
    /// <typeparam name="TResult">
    /// The type  T  of the query results returned by executing the delegate returned by the
    /// <see cref="M:System.Data.Entity.Core.Objects.CompiledQuery.Compile``13(System.Linq.Expressions.Expression{System.Func{``0,``1,``2,``3,``4,``5,``6,``7,``8,``9,``10,``11,``12}})" />
    /// method.
    /// </typeparam>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "required for this feature")]
    public static Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TResult> Compile<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TResult>(
      Expression<Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TResult>> query)
      where TArg0 : ObjectContext
    {
      return new Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TResult>(new CompiledQuery((LambdaExpression) query).Invoke<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TResult>);
    }

    /// <summary>Creates a new delegate that represents the compiled LINQ to Entities query.</summary>
    /// <returns>
    /// <see cref="T:System.Func`12" />, a generic delegate that represents the compiled LINQ to Entities query.
    /// </returns>
    /// <param name="query">The lambda expression to compile.</param>
    /// <typeparam name="TArg0">
    /// A type derived from <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />.
    /// </typeparam>
    /// <typeparam name="TArg1">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg1  must be a primitive type.</typeparam>
    /// <typeparam name="TArg2">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg2  must be a primitive type.</typeparam>
    /// <typeparam name="TArg3">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg3  must be a primitive type.</typeparam>
    /// <typeparam name="TArg4">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg4  must be a primitive type.</typeparam>
    /// <typeparam name="TArg5">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg5  must be a primitive type.</typeparam>
    /// <typeparam name="TArg6">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg6  must be a primitive type.</typeparam>
    /// <typeparam name="TArg7">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg7  must be a primitive type.</typeparam>
    /// <typeparam name="TArg8">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg8  must be a primitive type.</typeparam>
    /// <typeparam name="TArg9">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg9  must be a primitive type.</typeparam>
    /// <typeparam name="TArg10">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg10  must be a primitive type.</typeparam>
    /// <typeparam name="TResult">
    /// The type  T  of the query results returned by executing the delegate returned by the
    /// <see cref="M:System.Data.Entity.Core.Objects.CompiledQuery.Compile``12(System.Linq.Expressions.Expression{System.Func{``0,``1,``2,``3,``4,``5,``6,``7,``8,``9,``10,``11}})" />
    /// method.
    /// </typeparam>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "required for this feature")]
    public static Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TResult> Compile<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TResult>(
      Expression<Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TResult>> query)
      where TArg0 : ObjectContext
    {
      return new Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TResult>(new CompiledQuery((LambdaExpression) query).Invoke<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TResult>);
    }

    /// <summary>Creates a new delegate that represents the compiled LINQ to Entities query.</summary>
    /// <returns>
    /// <see cref="T:System.Func`11" />, a generic delegate that represents the compiled LINQ to Entities query.
    /// </returns>
    /// <param name="query">The lambda expression to compile.</param>
    /// <typeparam name="TArg0">
    /// A type derived from <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />.
    /// </typeparam>
    /// <typeparam name="TArg1">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg1  must be a primitive type.</typeparam>
    /// <typeparam name="TArg2">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg2  must be a primitive type.</typeparam>
    /// <typeparam name="TArg3">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg3  must be a primitive type.</typeparam>
    /// <typeparam name="TArg4">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg4  must be a primitive type.</typeparam>
    /// <typeparam name="TArg5">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg5  must be a primitive type.</typeparam>
    /// <typeparam name="TArg6">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg6  must be a primitive type.</typeparam>
    /// <typeparam name="TArg7">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg7  must be a primitive type.</typeparam>
    /// <typeparam name="TArg8">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg8  must be a primitive type.</typeparam>
    /// <typeparam name="TArg9">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg9  must be a primitive type.</typeparam>
    /// <typeparam name="TResult">
    /// The type  T  of the query results returned by executing the delegate returned by the
    /// <see cref="M:System.Data.Entity.Core.Objects.CompiledQuery.Compile``11(System.Linq.Expressions.Expression{System.Func{``0,``1,``2,``3,``4,``5,``6,``7,``8,``9,``10}})" />
    /// method.
    /// </typeparam>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "required for this feature")]
    public static Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult> Compile<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult>(
      Expression<Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult>> query)
      where TArg0 : ObjectContext
    {
      return new Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult>(new CompiledQuery((LambdaExpression) query).Invoke<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult>);
    }

    /// <summary>Creates a new delegate that represents the compiled LINQ to Entities query.</summary>
    /// <returns>
    /// <see cref="T:System.Func`10" />, a generic delegate that represents the compiled LINQ to Entities query.
    /// </returns>
    /// <param name="query">The lambda expression to compile.</param>
    /// <typeparam name="TArg0">
    /// A type derived from <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />.
    /// </typeparam>
    /// <typeparam name="TArg1">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg1  must be a primitive type.</typeparam>
    /// <typeparam name="TArg2">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg2  must be a primitive type.</typeparam>
    /// <typeparam name="TArg3">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg3  must be a primitive type.</typeparam>
    /// <typeparam name="TArg4">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg4  must be a primitive type.</typeparam>
    /// <typeparam name="TArg5">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg5  must be a primitive type.</typeparam>
    /// <typeparam name="TArg6">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg6  must be a primitive type.</typeparam>
    /// <typeparam name="TArg7">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg7  must be a primitive type.</typeparam>
    /// <typeparam name="TArg8">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg8  must be a primitive type.</typeparam>
    /// <typeparam name="TResult">
    /// The type  T  of the query results returned by executing the delegate returned by the
    /// <see cref="M:System.Data.Entity.Core.Objects.CompiledQuery.Compile``10(System.Linq.Expressions.Expression{System.Func{``0,``1,``2,``3,``4,``5,``6,``7,``8,``9}})" />
    /// method.
    /// </typeparam>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "required for this feature")]
    public static Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult> Compile<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>(
      Expression<Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>> query)
      where TArg0 : ObjectContext
    {
      return new Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>(new CompiledQuery((LambdaExpression) query).Invoke<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>);
    }

    /// <summary>Creates a new delegate that represents the compiled LINQ to Entities query.</summary>
    /// <returns>
    /// <see cref="T:System.Func`9" />, a generic delegate that represents the compiled LINQ to Entities query.
    /// </returns>
    /// <param name="query">The lambda expression to compile.</param>
    /// <typeparam name="TArg0">
    /// A type derived from <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />.
    /// </typeparam>
    /// <typeparam name="TArg1">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg1  must be a primitive type.</typeparam>
    /// <typeparam name="TArg2">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg2  must be a primitive type.</typeparam>
    /// <typeparam name="TArg3">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg3  must be a primitive type.</typeparam>
    /// <typeparam name="TArg4">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg4  must be a primitive type.</typeparam>
    /// <typeparam name="TArg5">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg5  must be a primitive type.</typeparam>
    /// <typeparam name="TArg6">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg6  must be a primitive type.</typeparam>
    /// <typeparam name="TArg7">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg7  must be a primitive type.</typeparam>
    /// <typeparam name="TResult">
    /// The type  T  of the query results returned by executing the delegate returned by the
    /// <see cref="M:System.Data.Entity.Core.Objects.CompiledQuery.Compile``9(System.Linq.Expressions.Expression{System.Func{``0,``1,``2,``3,``4,``5,``6,``7,``8}})" />
    /// method.
    /// </typeparam>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "required for this feature")]
    public static Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult> Compile<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>(
      Expression<Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>> query)
      where TArg0 : ObjectContext
    {
      return new Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>(new CompiledQuery((LambdaExpression) query).Invoke<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>);
    }

    /// <summary>Creates a new delegate that represents the compiled LINQ to Entities query.</summary>
    /// <returns>
    /// <see cref="T:System.Func`8" />, a generic delegate that represents the compiled LINQ to Entities query.
    /// </returns>
    /// <param name="query">The lambda expression to compile.</param>
    /// <typeparam name="TArg0">
    /// A type derived from <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />.
    /// </typeparam>
    /// <typeparam name="TArg1">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg1  must be a primitive type.</typeparam>
    /// <typeparam name="TArg2">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg2  must be a primitive type.</typeparam>
    /// <typeparam name="TArg3">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg3  must be a primitive type.</typeparam>
    /// <typeparam name="TArg4">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg4  must be a primitive type.</typeparam>
    /// <typeparam name="TArg5">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg5  must be a primitive type.</typeparam>
    /// <typeparam name="TArg6">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg6  must be a primitive type.</typeparam>
    /// <typeparam name="TResult">
    /// The type  T  of the query results returned by executing the delegate returned by the
    /// <see cref="M:System.Data.Entity.Core.Objects.CompiledQuery.Compile``8(System.Linq.Expressions.Expression{System.Func{``0,``1,``2,``3,``4,``5,``6,``7}})" />
    /// method.
    /// </typeparam>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "required for this feature")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult> Compile<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>(
      Expression<Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>> query)
      where TArg0 : ObjectContext
    {
      return new Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>(new CompiledQuery((LambdaExpression) query).Invoke<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>);
    }

    /// <summary>Creates a new delegate that represents the compiled LINQ to Entities query.</summary>
    /// <returns>
    /// <see cref="T:System.Func`7" />, a generic delegate that represents the compiled LINQ to Entities query.
    /// </returns>
    /// <param name="query">The lambda expression to compile.</param>
    /// <typeparam name="TArg0">
    /// A type derived from <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />.
    /// </typeparam>
    /// <typeparam name="TArg1">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg1  must be a primitive type.</typeparam>
    /// <typeparam name="TArg2">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg2  must be a primitive type.</typeparam>
    /// <typeparam name="TArg3">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg3  must be a primitive type.</typeparam>
    /// <typeparam name="TArg4">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg4  must be a primitive type.</typeparam>
    /// <typeparam name="TArg5">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg5  must be a primitive type.</typeparam>
    /// <typeparam name="TResult">
    /// The type  T  of the query results returned by executing the delegate returned by the
    /// <see cref="M:System.Data.Entity.Core.Objects.CompiledQuery.Compile``7(System.Linq.Expressions.Expression{System.Func{``0,``1,``2,``3,``4,``5,``6}})" />
    /// method.
    /// </typeparam>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "required for this feature")]
    public static Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TResult> Compile<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(
      Expression<Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TResult>> query)
      where TArg0 : ObjectContext
    {
      return new Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(new CompiledQuery((LambdaExpression) query).Invoke<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TResult>);
    }

    /// <summary>Creates a new delegate that represents the compiled LINQ to Entities query.</summary>
    /// <returns>
    /// <see cref="T:System.Func`6" />, a generic delegate that represents the compiled LINQ to Entities query.
    /// </returns>
    /// <param name="query">The lambda expression to compile.</param>
    /// <typeparam name="TArg0">
    /// A type derived from <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />.
    /// </typeparam>
    /// <typeparam name="TArg1">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg1  must be a primitive type.</typeparam>
    /// <typeparam name="TArg2">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg2  must be a primitive type.</typeparam>
    /// <typeparam name="TArg3">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg3  must be a primitive type.</typeparam>
    /// <typeparam name="TArg4">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg4  must be a primitive type.</typeparam>
    /// <typeparam name="TResult">
    /// The type  T  of the query results returned by executing the delegate returned by the
    /// <see cref="M:System.Data.Entity.Core.Objects.CompiledQuery.Compile``6(System.Linq.Expressions.Expression{System.Func{``0,``1,``2,``3,``4,``5}})" />
    /// method.
    /// </typeparam>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "required for this feature")]
    public static Func<TArg0, TArg1, TArg2, TArg3, TArg4, TResult> Compile<TArg0, TArg1, TArg2, TArg3, TArg4, TResult>(
      Expression<Func<TArg0, TArg1, TArg2, TArg3, TArg4, TResult>> query)
      where TArg0 : ObjectContext
    {
      return new Func<TArg0, TArg1, TArg2, TArg3, TArg4, TResult>(new CompiledQuery((LambdaExpression) query).Invoke<TArg0, TArg1, TArg2, TArg3, TArg4, TResult>);
    }

    /// <summary>Creates a new delegate that represents the compiled LINQ to Entities query.</summary>
    /// <returns>
    /// <see cref="T:System.Func`5" />, a generic delegate that represents the compiled LINQ to Entities query.
    /// </returns>
    /// <param name="query">The lambda expression to compile.</param>
    /// <typeparam name="TArg0">
    /// A type derived from <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />.
    /// </typeparam>
    /// <typeparam name="TArg1">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg1  must be a primitive type.</typeparam>
    /// <typeparam name="TArg2">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg2  must be a primitive type.</typeparam>
    /// <typeparam name="TArg3">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg3  must be a primitive type.</typeparam>
    /// <typeparam name="TResult">
    /// The type  T  of the query results returned by executing the delegate returned by the
    /// <see cref="M:System.Data.Entity.Core.Objects.CompiledQuery.Compile``5(System.Linq.Expressions.Expression{System.Func{``0,``1,``2,``3,``4}})" />
    /// method.
    /// </typeparam>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "required for this feature")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Func<TArg0, TArg1, TArg2, TArg3, TResult> Compile<TArg0, TArg1, TArg2, TArg3, TResult>(
      Expression<Func<TArg0, TArg1, TArg2, TArg3, TResult>> query)
      where TArg0 : ObjectContext
    {
      return new Func<TArg0, TArg1, TArg2, TArg3, TResult>(new CompiledQuery((LambdaExpression) query).Invoke<TArg0, TArg1, TArg2, TArg3, TResult>);
    }

    /// <summary>Creates a new delegate that represents the compiled LINQ to Entities query.</summary>
    /// <returns>
    /// <see cref="T:System.Func`4" />, a generic delegate that represents the compiled LINQ to Entities query.
    /// </returns>
    /// <param name="query">The lambda expression to compile.</param>
    /// <typeparam name="TArg0">
    /// A type derived from <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />.
    /// </typeparam>
    /// <typeparam name="TArg1">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg1  must be a primitive type.</typeparam>
    /// <typeparam name="TArg2">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg2  must be a primitive type.</typeparam>
    /// <typeparam name="TResult">
    /// The type  T  of the query results returned by executing the delegate returned by the
    /// <see cref="M:System.Data.Entity.Core.Objects.CompiledQuery.Compile``4(System.Linq.Expressions.Expression{System.Func{``0,``1,``2,``3}})" />
    /// method.
    /// </typeparam>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "required for this feature")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Func<TArg0, TArg1, TArg2, TResult> Compile<TArg0, TArg1, TArg2, TResult>(
      Expression<Func<TArg0, TArg1, TArg2, TResult>> query)
      where TArg0 : ObjectContext
    {
      return new Func<TArg0, TArg1, TArg2, TResult>(new CompiledQuery((LambdaExpression) query).Invoke<TArg0, TArg1, TArg2, TResult>);
    }

    /// <summary>Creates a new delegate that represents the compiled LINQ to Entities query.</summary>
    /// <returns>
    /// <see cref="T:System.Func`3" />, a generic delegate that represents the compiled LINQ to Entities query.
    /// </returns>
    /// <param name="query">The lambda expression to compile.</param>
    /// <typeparam name="TArg0">
    /// A type derived from <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />.
    /// </typeparam>
    /// <typeparam name="TArg1">Represents the type of the parameter that has to be passed in when executing the delegate returned by this method.  TArg1 must be a primitive type.</typeparam>
    /// <typeparam name="TResult">
    /// The type  T  of the query results returned by executing the delegate returned by the
    /// <see cref="M:System.Data.Entity.Core.Objects.CompiledQuery.Compile``3(System.Linq.Expressions.Expression{System.Func{``0,``1,``2}})" />
    /// method.
    /// </typeparam>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "required for this feature")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Func<TArg0, TArg1, TResult> Compile<TArg0, TArg1, TResult>(
      Expression<Func<TArg0, TArg1, TResult>> query)
      where TArg0 : ObjectContext
    {
      return new Func<TArg0, TArg1, TResult>(new CompiledQuery((LambdaExpression) query).Invoke<TArg0, TArg1, TResult>);
    }

    /// <summary>Creates a new delegate that represents the compiled LINQ to Entities query.</summary>
    /// <returns>
    /// <see cref="T:System.Func`2" />, a generic delegate that represents the compiled LINQ to Entities query.
    /// </returns>
    /// <param name="query">The lambda expression to compile.</param>
    /// <typeparam name="TArg0">
    /// A type derived from <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />.
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The type  T  of the query results returned by executing the delegate returned by the
    /// <see cref="M:System.Data.Entity.Core.Objects.CompiledQuery.Compile``2(System.Linq.Expressions.Expression{System.Func{``0,``1}})" />
    /// method.
    /// </typeparam>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "required for this feature")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Func<TArg0, TResult> Compile<TArg0, TResult>(
      Expression<Func<TArg0, TResult>> query)
      where TArg0 : ObjectContext
    {
      return new Func<TArg0, TResult>(new CompiledQuery((LambdaExpression) query).Invoke<TArg0, TResult>);
    }

    private TResult Invoke<TArg0, TResult>(TArg0 arg0) where TArg0 : ObjectContext
    {
      arg0.MetadataWorkspace.ImplicitLoadAssemblyForType(typeof (TResult), Assembly.GetCallingAssembly());
      return this.ExecuteQuery<TResult>((ObjectContext) arg0);
    }

    private TResult Invoke<TArg0, TArg1, TResult>(TArg0 arg0, TArg1 arg1) where TArg0 : ObjectContext
    {
      arg0.MetadataWorkspace.ImplicitLoadAssemblyForType(typeof (TResult), Assembly.GetCallingAssembly());
      return this.ExecuteQuery<TResult>((ObjectContext) arg0, (object) arg1);
    }

    private TResult Invoke<TArg0, TArg1, TArg2, TResult>(TArg0 arg0, TArg1 arg1, TArg2 arg2) where TArg0 : ObjectContext
    {
      arg0.MetadataWorkspace.ImplicitLoadAssemblyForType(typeof (TResult), Assembly.GetCallingAssembly());
      return this.ExecuteQuery<TResult>((ObjectContext) arg0, (object) arg1, (object) arg2);
    }

    private TResult Invoke<TArg0, TArg1, TArg2, TArg3, TResult>(
      TArg0 arg0,
      TArg1 arg1,
      TArg2 arg2,
      TArg3 arg3)
      where TArg0 : ObjectContext
    {
      arg0.MetadataWorkspace.ImplicitLoadAssemblyForType(typeof (TResult), Assembly.GetCallingAssembly());
      return this.ExecuteQuery<TResult>((ObjectContext) arg0, (object) arg1, (object) arg2, (object) arg3);
    }

    private TResult Invoke<TArg0, TArg1, TArg2, TArg3, TArg4, TResult>(
      TArg0 arg0,
      TArg1 arg1,
      TArg2 arg2,
      TArg3 arg3,
      TArg4 arg4)
      where TArg0 : ObjectContext
    {
      arg0.MetadataWorkspace.ImplicitLoadAssemblyForType(typeof (TResult), Assembly.GetCallingAssembly());
      return this.ExecuteQuery<TResult>((ObjectContext) arg0, (object) arg1, (object) arg2, (object) arg3, (object) arg4);
    }

    private TResult Invoke<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(
      TArg0 arg0,
      TArg1 arg1,
      TArg2 arg2,
      TArg3 arg3,
      TArg4 arg4,
      TArg5 arg5)
      where TArg0 : ObjectContext
    {
      arg0.MetadataWorkspace.ImplicitLoadAssemblyForType(typeof (TResult), Assembly.GetCallingAssembly());
      return this.ExecuteQuery<TResult>((ObjectContext) arg0, (object) arg1, (object) arg2, (object) arg3, (object) arg4, (object) arg5);
    }

    private TResult Invoke<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>(
      TArg0 arg0,
      TArg1 arg1,
      TArg2 arg2,
      TArg3 arg3,
      TArg4 arg4,
      TArg5 arg5,
      TArg6 arg6)
      where TArg0 : ObjectContext
    {
      arg0.MetadataWorkspace.ImplicitLoadAssemblyForType(typeof (TResult), Assembly.GetCallingAssembly());
      return this.ExecuteQuery<TResult>((ObjectContext) arg0, (object) arg1, (object) arg2, (object) arg3, (object) arg4, (object) arg5, (object) arg6);
    }

    private TResult Invoke<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>(
      TArg0 arg0,
      TArg1 arg1,
      TArg2 arg2,
      TArg3 arg3,
      TArg4 arg4,
      TArg5 arg5,
      TArg6 arg6,
      TArg7 arg7)
      where TArg0 : ObjectContext
    {
      arg0.MetadataWorkspace.ImplicitLoadAssemblyForType(typeof (TResult), Assembly.GetCallingAssembly());
      return this.ExecuteQuery<TResult>((ObjectContext) arg0, (object) arg1, (object) arg2, (object) arg3, (object) arg4, (object) arg5, (object) arg6, (object) arg7);
    }

    private TResult Invoke<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>(
      TArg0 arg0,
      TArg1 arg1,
      TArg2 arg2,
      TArg3 arg3,
      TArg4 arg4,
      TArg5 arg5,
      TArg6 arg6,
      TArg7 arg7,
      TArg8 arg8)
      where TArg0 : ObjectContext
    {
      arg0.MetadataWorkspace.ImplicitLoadAssemblyForType(typeof (TResult), Assembly.GetCallingAssembly());
      return this.ExecuteQuery<TResult>((ObjectContext) arg0, (object) arg1, (object) arg2, (object) arg3, (object) arg4, (object) arg5, (object) arg6, (object) arg7, (object) arg8);
    }

    private TResult Invoke<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult>(
      TArg0 arg0,
      TArg1 arg1,
      TArg2 arg2,
      TArg3 arg3,
      TArg4 arg4,
      TArg5 arg5,
      TArg6 arg6,
      TArg7 arg7,
      TArg8 arg8,
      TArg9 arg9)
      where TArg0 : ObjectContext
    {
      arg0.MetadataWorkspace.ImplicitLoadAssemblyForType(typeof (TResult), Assembly.GetCallingAssembly());
      return this.ExecuteQuery<TResult>((ObjectContext) arg0, (object) arg1, (object) arg2, (object) arg3, (object) arg4, (object) arg5, (object) arg6, (object) arg7, (object) arg8, (object) arg9);
    }

    private TResult Invoke<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TResult>(
      TArg0 arg0,
      TArg1 arg1,
      TArg2 arg2,
      TArg3 arg3,
      TArg4 arg4,
      TArg5 arg5,
      TArg6 arg6,
      TArg7 arg7,
      TArg8 arg8,
      TArg9 arg9,
      TArg10 arg10)
      where TArg0 : ObjectContext
    {
      arg0.MetadataWorkspace.ImplicitLoadAssemblyForType(typeof (TResult), Assembly.GetCallingAssembly());
      return this.ExecuteQuery<TResult>((ObjectContext) arg0, (object) arg1, (object) arg2, (object) arg3, (object) arg4, (object) arg5, (object) arg6, (object) arg7, (object) arg8, (object) arg9, (object) arg10);
    }

    private TResult Invoke<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TResult>(
      TArg0 arg0,
      TArg1 arg1,
      TArg2 arg2,
      TArg3 arg3,
      TArg4 arg4,
      TArg5 arg5,
      TArg6 arg6,
      TArg7 arg7,
      TArg8 arg8,
      TArg9 arg9,
      TArg10 arg10,
      TArg11 arg11)
      where TArg0 : ObjectContext
    {
      arg0.MetadataWorkspace.ImplicitLoadAssemblyForType(typeof (TResult), Assembly.GetCallingAssembly());
      return this.ExecuteQuery<TResult>((ObjectContext) arg0, (object) arg1, (object) arg2, (object) arg3, (object) arg4, (object) arg5, (object) arg6, (object) arg7, (object) arg8, (object) arg9, (object) arg10, (object) arg11);
    }

    private TResult Invoke<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TResult>(
      TArg0 arg0,
      TArg1 arg1,
      TArg2 arg2,
      TArg3 arg3,
      TArg4 arg4,
      TArg5 arg5,
      TArg6 arg6,
      TArg7 arg7,
      TArg8 arg8,
      TArg9 arg9,
      TArg10 arg10,
      TArg11 arg11,
      TArg12 arg12)
      where TArg0 : ObjectContext
    {
      arg0.MetadataWorkspace.ImplicitLoadAssemblyForType(typeof (TResult), Assembly.GetCallingAssembly());
      return this.ExecuteQuery<TResult>((ObjectContext) arg0, (object) arg1, (object) arg2, (object) arg3, (object) arg4, (object) arg5, (object) arg6, (object) arg7, (object) arg8, (object) arg9, (object) arg10, (object) arg11, (object) arg12);
    }

    private TResult Invoke<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TResult>(
      TArg0 arg0,
      TArg1 arg1,
      TArg2 arg2,
      TArg3 arg3,
      TArg4 arg4,
      TArg5 arg5,
      TArg6 arg6,
      TArg7 arg7,
      TArg8 arg8,
      TArg9 arg9,
      TArg10 arg10,
      TArg11 arg11,
      TArg12 arg12,
      TArg13 arg13)
      where TArg0 : ObjectContext
    {
      arg0.MetadataWorkspace.ImplicitLoadAssemblyForType(typeof (TResult), Assembly.GetCallingAssembly());
      return this.ExecuteQuery<TResult>((ObjectContext) arg0, (object) arg1, (object) arg2, (object) arg3, (object) arg4, (object) arg5, (object) arg6, (object) arg7, (object) arg8, (object) arg9, (object) arg10, (object) arg11, (object) arg12, (object) arg13);
    }

    private TResult Invoke<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TResult>(
      TArg0 arg0,
      TArg1 arg1,
      TArg2 arg2,
      TArg3 arg3,
      TArg4 arg4,
      TArg5 arg5,
      TArg6 arg6,
      TArg7 arg7,
      TArg8 arg8,
      TArg9 arg9,
      TArg10 arg10,
      TArg11 arg11,
      TArg12 arg12,
      TArg13 arg13,
      TArg14 arg14)
      where TArg0 : ObjectContext
    {
      arg0.MetadataWorkspace.ImplicitLoadAssemblyForType(typeof (TResult), Assembly.GetCallingAssembly());
      return this.ExecuteQuery<TResult>((ObjectContext) arg0, (object) arg1, (object) arg2, (object) arg3, (object) arg4, (object) arg5, (object) arg6, (object) arg7, (object) arg8, (object) arg9, (object) arg10, (object) arg11, (object) arg12, (object) arg13, (object) arg14);
    }

    private TResult Invoke<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TResult>(
      TArg0 arg0,
      TArg1 arg1,
      TArg2 arg2,
      TArg3 arg3,
      TArg4 arg4,
      TArg5 arg5,
      TArg6 arg6,
      TArg7 arg7,
      TArg8 arg8,
      TArg9 arg9,
      TArg10 arg10,
      TArg11 arg11,
      TArg12 arg12,
      TArg13 arg13,
      TArg14 arg14,
      TArg15 arg15)
      where TArg0 : ObjectContext
    {
      arg0.MetadataWorkspace.ImplicitLoadAssemblyForType(typeof (TResult), Assembly.GetCallingAssembly());
      return this.ExecuteQuery<TResult>((ObjectContext) arg0, (object) arg1, (object) arg2, (object) arg3, (object) arg4, (object) arg5, (object) arg6, (object) arg7, (object) arg8, (object) arg9, (object) arg10, (object) arg11, (object) arg12, (object) arg13, (object) arg14, (object) arg15);
    }

    private TResult ExecuteQuery<TResult>(ObjectContext context, params object[] parameterValues)
    {
      bool isSingleton;
      IEnumerable query = (IEnumerable) new CompiledELinqQueryState(CompiledQuery.GetElementType(typeof (TResult), out isSingleton), context, this._query, this._cacheToken, parameterValues, (ObjectQueryExecutionPlanFactory) null).CreateQuery();
      if (isSingleton)
        return ObjectQueryProvider.ExecuteSingle<TResult>(query.Cast<TResult>(), (Expression) this._query);
      return (TResult) query;
    }

    private static Type GetElementType(Type resultType, out bool isSingleton)
    {
      Type elementType = TypeSystem.GetElementType(resultType);
      ref bool local = ref isSingleton;
      int num;
      if (!(elementType == resultType))
        num = !resultType.IsAssignableFrom(typeof (ObjectQuery<>).MakeGenericType(elementType)) ? 1 : 0;
      else
        num = 1;
      local = num != 0;
      if (isSingleton)
        return resultType;
      return elementType;
    }
  }
}
