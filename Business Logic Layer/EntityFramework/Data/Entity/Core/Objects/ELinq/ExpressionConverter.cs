// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.ELinq.ExpressionConverter
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Common.EntitySql;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Metadata.Edm.Provider;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Resources;
using System.Data.Entity.Spatial;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Data.Entity.Core.Objects.ELinq
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
  internal sealed class ExpressionConverter
  {
    private static readonly Dictionary<ExpressionType, ExpressionConverter.Translator> _translators = ExpressionConverter.InitializeTranslators();
    private readonly AliasGenerator _aliasGenerator = new AliasGenerator("LQ", 0);
    private const string s_visualBasicAssemblyFullName = "Microsoft.VisualBasic, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
    internal const string KeyColumnName = "Key";
    internal const string GroupColumnName = "Group";
    internal const string EntityCollectionOwnerColumnName = "Owner";
    internal const string EntityCollectionElementsColumnName = "Elements";
    internal const string EdmNamespaceName = "Edm";
    private const string Concat = "Concat";
    private const string IndexOf = "IndexOf";
    private const string Length = "Length";
    private const string Right = "Right";
    private const string Substring = "Substring";
    private const string ToUpper = "ToUpper";
    private const string ToLower = "ToLower";
    private const string Trim = "Trim";
    private const string LTrim = "LTrim";
    private const string RTrim = "RTrim";
    private const string Reverse = "Reverse";
    private const string BitwiseAnd = "BitwiseAnd";
    private const string BitwiseOr = "BitwiseOr";
    private const string BitwiseNot = "BitwiseNot";
    private const string BitwiseXor = "BitwiseXor";
    private const string CurrentUtcDateTime = "CurrentUtcDateTime";
    private const string CurrentDateTimeOffset = "CurrentDateTimeOffset";
    private const string CurrentDateTime = "CurrentDateTime";
    private const string Year = "Year";
    private const string Month = "Month";
    private const string Day = "Day";
    private const string Hour = "Hour";
    private const string Minute = "Minute";
    private const string Second = "Second";
    private const string Millisecond = "Millisecond";
    private const string AsUnicode = "AsUnicode";
    private const string AsNonUnicode = "AsNonUnicode";
    private readonly Funcletizer _funcletizer;
    private readonly Perspective _perspective;
    private readonly Expression _expression;
    private readonly BindingContext _bindingContext;
    private Func<bool> _recompileRequired;
    private List<Tuple<ObjectParameter, QueryParameterExpression>> _parameters;
    private Dictionary<DbExpression, Span> _spanMappings;
    private MergeOption? _mergeOption;
    private Dictionary<Type, InitializerMetadata> _initializers;
    private Span _span;
    private HashSet<ObjectQuery> _inlineEntitySqlQueries;
    private int _ignoreInclude;
    private readonly ExpressionConverter.OrderByLifter _orderByLifter;

    internal ExpressionConverter(Funcletizer funcletizer, Expression expression)
    {
      this._funcletizer = funcletizer;
      expression = funcletizer.Funcletize(expression, out this._recompileRequired);
      this._expression = new LinqExpressionNormalizer().Visit(expression);
      this._perspective = (Perspective) funcletizer.RootContext.Perspective;
      this._bindingContext = new BindingContext();
      this._ignoreInclude = 0;
      this._orderByLifter = new ExpressionConverter.OrderByLifter(this._aliasGenerator);
    }

    private static Dictionary<ExpressionType, ExpressionConverter.Translator> InitializeTranslators()
    {
      Dictionary<ExpressionType, ExpressionConverter.Translator> dictionary = new Dictionary<ExpressionType, ExpressionConverter.Translator>();
      foreach (ExpressionConverter.Translator translator in ExpressionConverter.GetTranslators())
      {
        foreach (ExpressionType nodeType in translator.NodeTypes)
          dictionary.Add(nodeType, translator);
      }
      return dictionary;
    }

    private static IEnumerable<ExpressionConverter.Translator> GetTranslators()
    {
      yield return (ExpressionConverter.Translator) new ExpressionConverter.AndAlsoTranslator();
      yield return (ExpressionConverter.Translator) new ExpressionConverter.OrElseTranslator();
      yield return (ExpressionConverter.Translator) new ExpressionConverter.LessThanTranslator();
      yield return (ExpressionConverter.Translator) new ExpressionConverter.LessThanOrEqualsTranslator();
      yield return (ExpressionConverter.Translator) new ExpressionConverter.GreaterThanTranslator();
      yield return (ExpressionConverter.Translator) new ExpressionConverter.GreaterThanOrEqualsTranslator();
      yield return (ExpressionConverter.Translator) new ExpressionConverter.EqualsTranslator();
      yield return (ExpressionConverter.Translator) new ExpressionConverter.NotEqualsTranslator();
      yield return (ExpressionConverter.Translator) new ExpressionConverter.ConvertTranslator();
      yield return (ExpressionConverter.Translator) new ExpressionConverter.ConstantTranslator();
      yield return (ExpressionConverter.Translator) new ExpressionConverter.NotTranslator();
      yield return (ExpressionConverter.Translator) new ExpressionConverter.MemberAccessTranslator();
      yield return (ExpressionConverter.Translator) new ExpressionConverter.ParameterTranslator();
      yield return (ExpressionConverter.Translator) new ExpressionConverter.MemberInitTranslator();
      yield return (ExpressionConverter.Translator) new ExpressionConverter.NewTranslator();
      yield return (ExpressionConverter.Translator) new ExpressionConverter.AddTranslator();
      yield return (ExpressionConverter.Translator) new ExpressionConverter.ConditionalTranslator();
      yield return (ExpressionConverter.Translator) new ExpressionConverter.DivideTranslator();
      yield return (ExpressionConverter.Translator) new ExpressionConverter.ModuloTranslator();
      yield return (ExpressionConverter.Translator) new ExpressionConverter.SubtractTranslator();
      yield return (ExpressionConverter.Translator) new ExpressionConverter.MultiplyTranslator();
      yield return (ExpressionConverter.Translator) new ExpressionConverter.PowerTranslator();
      yield return (ExpressionConverter.Translator) new ExpressionConverter.NegateTranslator();
      yield return (ExpressionConverter.Translator) new ExpressionConverter.UnaryPlusTranslator();
      yield return (ExpressionConverter.Translator) new ExpressionConverter.MethodCallTranslator();
      yield return (ExpressionConverter.Translator) new ExpressionConverter.CoalesceTranslator();
      yield return (ExpressionConverter.Translator) new ExpressionConverter.AsTranslator();
      yield return (ExpressionConverter.Translator) new ExpressionConverter.IsTranslator();
      yield return (ExpressionConverter.Translator) new ExpressionConverter.QuoteTranslator();
      yield return (ExpressionConverter.Translator) new ExpressionConverter.AndTranslator();
      yield return (ExpressionConverter.Translator) new ExpressionConverter.OrTranslator();
      yield return (ExpressionConverter.Translator) new ExpressionConverter.ExclusiveOrTranslator();
      yield return (ExpressionConverter.Translator) new ExpressionConverter.ExtensionTranslator();
      yield return (ExpressionConverter.Translator) new ExpressionConverter.NewArrayInitTranslator();
      yield return (ExpressionConverter.Translator) new ExpressionConverter.ListInitTranslator();
      yield return (ExpressionConverter.Translator) new ExpressionConverter.NotSupportedTranslator(new ExpressionType[7]
      {
        ExpressionType.LeftShift,
        ExpressionType.RightShift,
        ExpressionType.ArrayLength,
        ExpressionType.ArrayIndex,
        ExpressionType.Invoke,
        ExpressionType.Lambda,
        ExpressionType.NewArrayBounds
      });
    }

    private EdmItemCollection EdmItemCollection
    {
      get
      {
        return (EdmItemCollection) this._funcletizer.RootContext.MetadataWorkspace.GetItemCollection(DataSpace.CSpace, true);
      }
    }

    internal DbProviderManifest ProviderManifest
    {
      get
      {
        return ((StoreItemCollection) this._funcletizer.RootContext.MetadataWorkspace.GetItemCollection(DataSpace.SSpace)).ProviderManifest;
      }
    }

    internal IEnumerable<Tuple<ObjectParameter, QueryParameterExpression>> GetParameters()
    {
      if (this._parameters != null)
        return (IEnumerable<Tuple<ObjectParameter, QueryParameterExpression>>) this._parameters;
      return (IEnumerable<Tuple<ObjectParameter, QueryParameterExpression>>) null;
    }

    internal MergeOption? PropagatedMergeOption
    {
      get
      {
        return this._mergeOption;
      }
    }

    internal Span PropagatedSpan
    {
      get
      {
        return this._span;
      }
    }

    internal Func<bool> RecompileRequired
    {
      get
      {
        return this._recompileRequired;
      }
    }

    internal int IgnoreInclude
    {
      get
      {
        return this._ignoreInclude;
      }
      set
      {
        this._ignoreInclude = value;
      }
    }

    internal AliasGenerator AliasGenerator
    {
      get
      {
        return this._aliasGenerator;
      }
    }

    internal DbExpression Convert()
    {
      DbExpression expression = this.TranslateExpression(this._expression);
      if (!this.TryGetSpan(expression, out this._span))
        this._span = (Span) null;
      return expression;
    }

    internal static bool CanFuncletizePropertyInfo(PropertyInfo propertyInfo)
    {
      return ExpressionConverter.MemberAccessTranslator.CanFuncletizePropertyInfo(propertyInfo);
    }

    internal bool CanIncludeSpanInfo()
    {
      return this._ignoreInclude == 0;
    }

    private void NotifyMergeOption(MergeOption mergeOption)
    {
      if (this._mergeOption.HasValue)
        return;
      this._mergeOption = new MergeOption?(mergeOption);
    }

    [SuppressMessage("Microsoft.Usage", "CA2301", Justification = "metadata.ClrType is not expected to be an Embedded Interop Type.")]
    internal void ValidateInitializerMetadata(InitializerMetadata metadata)
    {
      InitializerMetadata other;
      if (this._initializers != null && this._initializers.TryGetValue(metadata.ClrType, out other))
      {
        if (!metadata.Equals(other))
          throw new NotSupportedException(Strings.ELinq_UnsupportedHeterogeneousInitializers((object) ExpressionConverter.DescribeClrType(metadata.ClrType)));
      }
      else
      {
        if (this._initializers == null)
          this._initializers = new Dictionary<Type, InitializerMetadata>();
        this._initializers.Add(metadata.ClrType, metadata);
      }
    }

    private void AddParameter(QueryParameterExpression queryParameter)
    {
      if (this._parameters == null)
        this._parameters = new List<Tuple<ObjectParameter, QueryParameterExpression>>();
      if (this._parameters.Select<Tuple<ObjectParameter, QueryParameterExpression>, QueryParameterExpression>((Func<Tuple<ObjectParameter, QueryParameterExpression>, QueryParameterExpression>) (p => p.Item2)).Contains<QueryParameterExpression>(queryParameter))
        return;
      this._parameters.Add(new Tuple<ObjectParameter, QueryParameterExpression>(new ObjectParameter(queryParameter.ParameterReference.ParameterName, queryParameter.Type), queryParameter));
    }

    private bool IsQueryRoot(Expression Expression)
    {
      return object.ReferenceEquals((object) this._expression, (object) Expression);
    }

    private DbExpression AddSpanMapping(DbExpression expression, Span span)
    {
      if (span != null && this.CanIncludeSpanInfo())
      {
        if (this._spanMappings == null)
          this._spanMappings = new Dictionary<DbExpression, Span>();
        Span span1 = (Span) null;
        if (this._spanMappings.TryGetValue(expression, out span1))
        {
          foreach (Span.SpanPath span2 in span.SpanList)
            span1.AddSpanPath(span2);
          this._spanMappings[expression] = span1;
        }
        else
          this._spanMappings[expression] = span;
      }
      return expression;
    }

    private bool TryGetSpan(DbExpression expression, out Span span)
    {
      if (this._spanMappings != null)
        return this._spanMappings.TryGetValue(expression, out span);
      span = (Span) null;
      return false;
    }

    private void ApplySpanMapping(DbExpression from, DbExpression to)
    {
      Span span;
      if (!this.TryGetSpan(from, out span))
        return;
      this.AddSpanMapping(to, span);
    }

    private void UnifySpanMappings(DbExpression left, DbExpression right, DbExpression to)
    {
      Span span1 = (Span) null;
      Span span2 = (Span) null;
      bool span3 = this.TryGetSpan(left, out span1);
      bool span4 = this.TryGetSpan(right, out span2);
      if (!span3 && !span4)
        return;
      this.AddSpanMapping(to, Span.CopyUnion(span1, span2));
    }

    private DbDistinctExpression Distinct(DbExpression argument)
    {
      DbDistinctExpression distinctExpression = argument.Distinct();
      this.ApplySpanMapping(argument, (DbExpression) distinctExpression);
      return distinctExpression;
    }

    private DbExceptExpression Except(DbExpression left, DbExpression right)
    {
      DbExceptExpression exceptExpression = left.Except(right);
      this.ApplySpanMapping(left, (DbExpression) exceptExpression);
      return exceptExpression;
    }

    private DbExpression Filter(DbExpressionBinding input, DbExpression predicate)
    {
      DbExpression to = this._orderByLifter.Filter(input, predicate);
      this.ApplySpanMapping(input.Expression, to);
      return to;
    }

    private DbIntersectExpression Intersect(
      DbExpression left,
      DbExpression right)
    {
      DbIntersectExpression intersectExpression = left.Intersect(right);
      this.UnifySpanMappings(left, right, (DbExpression) intersectExpression);
      return intersectExpression;
    }

    private DbExpression Limit(DbExpression argument, DbExpression limit)
    {
      DbExpression to = this._orderByLifter.Limit(argument, limit);
      this.ApplySpanMapping(argument, to);
      return to;
    }

    private DbExpression OfType(DbExpression argument, TypeUsage ofType)
    {
      DbExpression to = this._orderByLifter.OfType(argument, ofType);
      this.ApplySpanMapping(argument, to);
      return to;
    }

    private DbExpression Project(DbExpressionBinding input, DbExpression projection)
    {
      DbExpression to = this._orderByLifter.Project(input, projection);
      if (projection.ExpressionKind == DbExpressionKind.VariableReference && ((DbVariableReferenceExpression) projection).VariableName.Equals(input.VariableName, StringComparison.Ordinal))
        this.ApplySpanMapping(input.Expression, to);
      return to;
    }

    private DbSortExpression Sort(
      DbExpressionBinding input,
      IList<DbSortClause> keys)
    {
      DbSortExpression dbSortExpression = input.Sort((IEnumerable<DbSortClause>) keys);
      this.ApplySpanMapping(input.Expression, (DbExpression) dbSortExpression);
      return dbSortExpression;
    }

    private DbExpression Skip(DbExpressionBinding input, DbExpression skipCount)
    {
      DbExpression to = this._orderByLifter.Skip(input, skipCount);
      this.ApplySpanMapping(input.Expression, to);
      return to;
    }

    private DbUnionAllExpression UnionAll(DbExpression left, DbExpression right)
    {
      DbUnionAllExpression unionAllExpression = left.UnionAll(right);
      this.UnifySpanMappings(left, right, (DbExpression) unionAllExpression);
      return unionAllExpression;
    }

    private TypeUsage GetCastTargetType(
      TypeUsage fromType,
      Type toClrType,
      Type fromClrType,
      bool preserveCastForDateTime)
    {
      if (fromClrType != (Type) null && fromClrType.IsGenericType() && toClrType.IsGenericType() && (fromClrType.GetGenericTypeDefinition() == typeof (ObjectQuery<>) || fromClrType.GetGenericTypeDefinition() == typeof (IQueryable<>) || fromClrType.GetGenericTypeDefinition() == typeof (IOrderedQueryable<>)) && ((toClrType.GetGenericTypeDefinition() == typeof (ObjectQuery<>) || toClrType.GetGenericTypeDefinition() == typeof (IQueryable<>) || toClrType.GetGenericTypeDefinition() == typeof (IOrderedQueryable<>)) && fromClrType.GetGenericArguments()[0] == toClrType.GetGenericArguments()[0]))
        return (TypeUsage) null;
      if (fromClrType != (Type) null && TypeSystem.GetNonNullableType(fromClrType).IsEnum && toClrType == typeof (Enum))
        return (TypeUsage) null;
      TypeUsage type;
      if (this.TryGetValueLayerType(toClrType, out type) && ExpressionConverter.CanOmitCast(fromType, type, preserveCastForDateTime))
        return (TypeUsage) null;
      return ExpressionConverter.ValidateAndAdjustCastTypes(type, fromType, toClrType, fromClrType);
    }

    private static TypeUsage ValidateAndAdjustCastTypes(
      TypeUsage toType,
      TypeUsage fromType,
      Type toClrType,
      Type fromClrType)
    {
      if (toType == null || !TypeSemantics.IsScalarType(toType) || !TypeSemantics.IsScalarType(fromType))
        throw new NotSupportedException(Strings.ELinq_UnsupportedCast((object) ExpressionConverter.DescribeClrType(fromClrType), (object) ExpressionConverter.DescribeClrType(toClrType)));
      PrimitiveTypeKind primitiveTypeKind = Helper.AsPrimitive(fromType.EdmType).PrimitiveTypeKind;
      if (Helper.AsPrimitive(toType.EdmType).PrimitiveTypeKind == PrimitiveTypeKind.Decimal)
      {
        switch (primitiveTypeKind)
        {
          case PrimitiveTypeKind.Byte:
          case PrimitiveTypeKind.SByte:
          case PrimitiveTypeKind.Int16:
          case PrimitiveTypeKind.Int32:
          case PrimitiveTypeKind.Int64:
            toType = TypeUsage.CreateDecimalTypeUsage((PrimitiveType) toType.EdmType, (byte) 19, (byte) 0);
            break;
          default:
            throw new NotSupportedException(Strings.ELinq_UnsupportedCastToDecimal);
        }
      }
      return toType;
    }

    private static bool CanOmitCast(
      TypeUsage fromType,
      TypeUsage toType,
      bool preserveCastForDateTime)
    {
      bool flag = TypeSemantics.IsPrimitiveType(fromType);
      if (flag && preserveCastForDateTime && ((PrimitiveType) fromType.EdmType).PrimitiveTypeKind == PrimitiveTypeKind.DateTime)
        return false;
      if (ExpressionConverter.TypeUsageEquals(fromType, toType))
        return true;
      if (flag)
        return fromType.EdmType.EdmEquals((MetadataItem) toType.EdmType);
      return TypeSemantics.IsSubTypeOf(fromType, toType);
    }

    private TypeUsage GetIsOrAsTargetType(
      ExpressionType operationType,
      Type toClrType,
      Type fromClrType)
    {
      TypeUsage type;
      if (!this.TryGetValueLayerType(toClrType, out type) || !TypeSemantics.IsEntityType(type) && !TypeSemantics.IsComplexType(type))
        throw new NotSupportedException(Strings.ELinq_UnsupportedIsOrAs((object) operationType, (object) ExpressionConverter.DescribeClrType(fromClrType), (object) ExpressionConverter.DescribeClrType(toClrType)));
      return type;
    }

    private DbExpression TranslateInlineQueryOfT(ObjectQuery inlineQuery)
    {
      if (!object.ReferenceEquals((object) this._funcletizer.RootContext, (object) inlineQuery.QueryState.ObjectContext))
        throw new NotSupportedException(Strings.ELinq_UnsupportedDifferentContexts);
      if (this._inlineEntitySqlQueries == null)
        this._inlineEntitySqlQueries = new HashSet<ObjectQuery>();
      bool flag = this._inlineEntitySqlQueries.Add(inlineQuery);
      EntitySqlQueryState queryState = (EntitySqlQueryState) inlineQuery.QueryState;
      ObjectParameterCollection parameters = inlineQuery.QueryState.Parameters;
      DbExpression dbExpression;
      if (!this._funcletizer.IsCompiledQuery || parameters == null || parameters.Count == 0)
      {
        if (flag && parameters != null)
        {
          if (this._parameters == null)
            this._parameters = new List<Tuple<ObjectParameter, QueryParameterExpression>>();
          foreach (ObjectParameter parameter in inlineQuery.QueryState.Parameters)
            this._parameters.Add(new Tuple<ObjectParameter, QueryParameterExpression>(parameter.ShallowCopy(), (QueryParameterExpression) null));
        }
        dbExpression = queryState.Parse();
      }
      else
        dbExpression = ExpressionConverter.ParameterReferenceRemover.RemoveParameterReferences(queryState.Parse(), parameters);
      return dbExpression;
    }

    private DbExpression CreateCastExpression(
      DbExpression source,
      Type toClrType,
      Type fromClrType)
    {
      DbExpression dbExpression = this.NormalizeSetSource(source);
      if (!object.ReferenceEquals((object) source, (object) dbExpression) && this.GetCastTargetType(dbExpression.ResultType, toClrType, fromClrType, true) == null)
        return source;
      TypeUsage castTargetType = this.GetCastTargetType(source.ResultType, toClrType, fromClrType, true);
      if (castTargetType == null)
        return source;
      return (DbExpression) source.CastTo(castTargetType);
    }

    private DbExpression TranslateLambda(
      LambdaExpression lambda,
      DbExpression input,
      out DbExpressionBinding binding)
    {
      input = this.NormalizeSetSource(input);
      binding = input.BindAs(this._aliasGenerator.Next());
      return this.TranslateLambda(lambda, (DbExpression) binding.Variable);
    }

    private DbExpression TranslateLambda(
      LambdaExpression lambda,
      DbExpression input,
      string bindingName,
      out DbExpressionBinding binding)
    {
      input = this.NormalizeSetSource(input);
      binding = input.BindAs(bindingName);
      return this.TranslateLambda(lambda, (DbExpression) binding.Variable);
    }

    private DbExpression TranslateLambda(
      LambdaExpression lambda,
      DbExpression input,
      out DbGroupExpressionBinding binding)
    {
      input = this.NormalizeSetSource(input);
      string varName = this._aliasGenerator.Next();
      binding = input.GroupBindAs(varName, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Group{0}", (object) varName));
      return this.TranslateLambda(lambda, (DbExpression) binding.Variable);
    }

    private DbExpression TranslateLambda(LambdaExpression lambda, DbExpression input)
    {
      this._bindingContext.PushBindingScope(new Binding((Expression) lambda.Parameters[0], input));
      ++this._ignoreInclude;
      DbExpression dbExpression = this.TranslateExpression(lambda.Body);
      --this._ignoreInclude;
      this._bindingContext.PopBindingScope();
      return dbExpression;
    }

    private DbExpression NormalizeSetSource(DbExpression input)
    {
      Span span;
      if (input.ExpressionKind == DbExpressionKind.Project && !this.TryGetSpan(input, out span))
      {
        DbProjectExpression projectExpression = (DbProjectExpression) input;
        if (projectExpression.Projection == projectExpression.Input.Variable)
          input = projectExpression.Input.Expression;
      }
      InitializerMetadata initializerMetadata;
      if (InitializerMetadata.TryGetInitializerMetadata(input.ResultType, out initializerMetadata))
      {
        if (initializerMetadata.Kind == InitializerMetadataKind.Grouping)
          input = (DbExpression) input.Property("Group");
        else if (initializerMetadata.Kind == InitializerMetadataKind.EntityCollection)
          input = (DbExpression) input.Property("Elements");
      }
      return input;
    }

    private LambdaExpression GetLambdaExpression(
      MethodCallExpression callExpression,
      int argumentOrdinal)
    {
      return (LambdaExpression) this.GetLambdaExpression(callExpression.Arguments[argumentOrdinal]);
    }

    private Expression GetLambdaExpression(Expression argument)
    {
      if (ExpressionType.Lambda == argument.NodeType)
        return argument;
      if (ExpressionType.Quote == argument.NodeType)
        return this.GetLambdaExpression(((UnaryExpression) argument).Operand);
      if (ExpressionType.Call == argument.NodeType)
      {
        if (typeof (Expression).IsAssignableFrom(argument.Type))
          return this.GetLambdaExpression(Expression.Lambda<Func<Expression>>(argument, new ParameterExpression[0]).Compile()());
      }
      else if (ExpressionType.Invoke == argument.NodeType && typeof (Expression).IsAssignableFrom(argument.Type))
        return this.GetLambdaExpression(Expression.Lambda<Func<Expression>>(argument, new ParameterExpression[0]).Compile()());
      throw new InvalidOperationException(Strings.ADP_InternalProviderError((object) 1025));
    }

    private DbExpression TranslateSet(Expression linq)
    {
      return this.NormalizeSetSource(this.TranslateExpression(linq));
    }

    private DbExpression TranslateExpression(Expression linq)
    {
      DbExpression cqtExpression;
      if (!this._bindingContext.TryGetBoundExpression(linq, out cqtExpression))
      {
        ExpressionConverter.Translator translator;
        if (!ExpressionConverter._translators.TryGetValue(linq.NodeType, out translator))
          throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.UnknownLinqNodeType, -1, (object) linq.NodeType.ToString());
        cqtExpression = translator.Translate(this, linq);
      }
      return cqtExpression;
    }

    private DbExpression AlignTypes(DbExpression cqt, Type toClrType)
    {
      Type fromClrType = (Type) null;
      TypeUsage castTargetType = this.GetCastTargetType(cqt.ResultType, toClrType, fromClrType, false);
      if (castTargetType != null)
        return (DbExpression) cqt.CastTo(castTargetType);
      return cqt;
    }

    private void CheckInitializerType(Type type)
    {
      TypeUsage outTypeUsage;
      if (this._funcletizer.RootContext.Perspective.TryGetType(type, out outTypeUsage))
      {
        BuiltInTypeKind builtInTypeKind = outTypeUsage.EdmType.BuiltInTypeKind;
        if (BuiltInTypeKind.EntityType == builtInTypeKind || BuiltInTypeKind.ComplexType == builtInTypeKind)
          throw new NotSupportedException(Strings.ELinq_UnsupportedNominalType((object) outTypeUsage.EdmType.FullName));
      }
      if (TypeSystem.IsSequenceType(type))
        throw new NotSupportedException(Strings.ELinq_UnsupportedEnumerableType((object) ExpressionConverter.DescribeClrType(type)));
    }

    private static bool TypeUsageEquals(TypeUsage left, TypeUsage right)
    {
      if (left.EdmType.EdmEquals((MetadataItem) right.EdmType))
        return true;
      if (BuiltInTypeKind.CollectionType == left.EdmType.BuiltInTypeKind && BuiltInTypeKind.CollectionType == right.EdmType.BuiltInTypeKind)
        return ExpressionConverter.TypeUsageEquals(((CollectionType) left.EdmType).TypeUsage, ((CollectionType) right.EdmType).TypeUsage);
      if (BuiltInTypeKind.PrimitiveType == left.EdmType.BuiltInTypeKind && BuiltInTypeKind.PrimitiveType == right.EdmType.BuiltInTypeKind)
        return ((PrimitiveType) left.EdmType).ClrEquivalentType.Equals(((PrimitiveType) right.EdmType).ClrEquivalentType);
      return false;
    }

    private TypeUsage GetValueLayerType(Type linqType)
    {
      TypeUsage type;
      if (!this.TryGetValueLayerType(linqType, out type))
        throw new NotSupportedException(Strings.ELinq_UnsupportedType((object) linqType));
      return type;
    }

    private bool TryGetValueLayerType(Type linqType, out TypeUsage type)
    {
      Type type1 = TypeSystem.GetNonNullableType(linqType);
      if (type1.IsEnum() && this.EdmItemCollection.EdmVersion < 3.0)
        type1 = type1.GetEnumUnderlyingType();
      PrimitiveTypeKind resolvedPrimitiveTypeKind;
      if (ClrProviderManifest.TryGetPrimitiveTypeKind(type1, out resolvedPrimitiveTypeKind))
      {
        type = EdmProviderManifest.Instance.GetCanonicalModelTypeUsage(resolvedPrimitiveTypeKind);
        return true;
      }
      Type elementType = TypeSystem.GetElementType(type1);
      TypeUsage type2;
      if (elementType != type1 && this.TryGetValueLayerType(elementType, out type2))
      {
        type = TypeHelpers.CreateCollectionTypeUsage(type2);
        return true;
      }
      this._perspective.MetadataWorkspace.ImplicitLoadAssemblyForType(linqType, (Assembly) null);
      if (!this._perspective.TryGetTypeByName(type1.FullNameWithNesting(), false, out type) && type1.IsEnum() && ClrProviderManifest.TryGetPrimitiveTypeKind(type1.GetEnumUnderlyingType(), out resolvedPrimitiveTypeKind))
        type = EdmProviderManifest.Instance.GetCanonicalModelTypeUsage(resolvedPrimitiveTypeKind);
      return type != null;
    }

    private static void VerifyTypeSupportedForComparison(
      Type clrType,
      TypeUsage edmType,
      Stack<EdmMember> memberPath)
    {
      switch (edmType.EdmType.BuiltInTypeKind)
      {
        case BuiltInTypeKind.EntityType:
          return;
        case BuiltInTypeKind.EnumType:
          return;
        case BuiltInTypeKind.PrimitiveType:
          return;
        case BuiltInTypeKind.RefType:
          return;
        case BuiltInTypeKind.RowType:
          InitializerMetadata initializerMetadata;
          if (!InitializerMetadata.TryGetInitializerMetadata(edmType, out initializerMetadata) || initializerMetadata.Kind == InitializerMetadataKind.ProjectionInitializer || initializerMetadata.Kind == InitializerMetadataKind.ProjectionNew)
          {
            ExpressionConverter.VerifyRowTypeSupportedForComparison(clrType, (RowType) edmType.EdmType, memberPath);
            return;
          }
          break;
      }
      if (memberPath == null)
        throw new NotSupportedException(Strings.ELinq_UnsupportedComparison((object) ExpressionConverter.DescribeClrType(clrType)));
      StringBuilder stringBuilder = new StringBuilder();
      foreach (EdmMember edmMember in memberPath)
        stringBuilder.Append(Strings.ELinq_UnsupportedRowMemberComparison((object) edmMember.Name));
      stringBuilder.Append(Strings.ELinq_UnsupportedRowTypeComparison((object) ExpressionConverter.DescribeClrType(clrType)));
      throw new NotSupportedException(Strings.ELinq_UnsupportedRowComparison((object) stringBuilder.ToString()));
    }

    private static void VerifyRowTypeSupportedForComparison(
      Type clrType,
      RowType rowType,
      Stack<EdmMember> memberPath)
    {
      foreach (EdmMember property in rowType.Properties)
      {
        if (memberPath == null)
          memberPath = new Stack<EdmMember>();
        memberPath.Push(property);
        ExpressionConverter.VerifyTypeSupportedForComparison(clrType, property.TypeUsage, memberPath);
        memberPath.Pop();
      }
    }

    internal static string DescribeClrType(Type clrType)
    {
      if (ExpressionConverter.IsCSharpGeneratedClass(clrType.Name, "DisplayClass") || ExpressionConverter.IsVBGeneratedClass(clrType.Name, "Closure"))
        return Strings.ELinq_ClosureType;
      if (ExpressionConverter.IsCSharpGeneratedClass(clrType.Name, "AnonymousType") || ExpressionConverter.IsVBGeneratedClass(clrType.Name, "AnonymousType"))
        return Strings.ELinq_AnonymousType;
      return clrType.FullName;
    }

    private static bool IsCSharpGeneratedClass(string typeName, string pattern)
    {
      if (typeName.Contains("<>") && typeName.Contains("__"))
        return typeName.Contains(pattern);
      return false;
    }

    private static bool IsVBGeneratedClass(string typeName, string pattern)
    {
      if (typeName.Contains("_") && typeName.Contains("$"))
        return typeName.Contains(pattern);
      return false;
    }

    private static DbExpression CreateIsNullExpression(
      DbExpression operand,
      Type operandClrType)
    {
      ExpressionConverter.VerifyTypeSupportedForComparison(operandClrType, operand.ResultType, (Stack<EdmMember>) null);
      return (DbExpression) operand.IsNull();
    }

    private DbExpression CreateEqualsExpression(
      DbExpression left,
      DbExpression right,
      ExpressionConverter.EqualsPattern pattern,
      Type leftClrType,
      Type rightClrType)
    {
      ExpressionConverter.VerifyTypeSupportedForComparison(leftClrType, left.ResultType, (Stack<EdmMember>) null);
      ExpressionConverter.VerifyTypeSupportedForComparison(rightClrType, right.ResultType, (Stack<EdmMember>) null);
      TypeUsage resultType1 = left.ResultType;
      TypeUsage resultType2 = right.ResultType;
      TypeUsage commonType;
      if (resultType1.EdmType.BuiltInTypeKind == BuiltInTypeKind.RefType && resultType2.EdmType.BuiltInTypeKind == BuiltInTypeKind.RefType && !TypeSemantics.TryGetCommonType(resultType1, resultType2, out commonType))
        throw new NotSupportedException(Strings.ELinq_UnsupportedRefComparison((object) (left.ResultType.EdmType as RefType).ElementType.FullName, (object) (right.ResultType.EdmType as RefType).ElementType.FullName));
      return this.RecursivelyRewriteEqualsExpression(left, right, pattern);
    }

    private DbExpression RecursivelyRewriteEqualsExpression(
      DbExpression left,
      DbExpression right,
      ExpressionConverter.EqualsPattern pattern)
    {
      RowType edmType1 = left.ResultType.EdmType as RowType;
      RowType edmType2 = right.ResultType.EdmType as RowType;
      if (edmType1 != null || edmType2 != null)
      {
        if (edmType1 == null || edmType2 == null)
          return (DbExpression) DbExpressionBuilder.False;
        DbExpression left1 = (DbExpression) null;
        foreach (EdmProperty property in edmType1.Properties)
        {
          DbExpression right1 = this.RecursivelyRewriteEqualsExpression((DbExpression) left.Property(property), (DbExpression) right.Property(property), pattern);
          left1 = left1 != null ? (DbExpression) left1.And(right1) : right1;
        }
        return left1;
      }
      if (!this._funcletizer.RootContext.ContextOptions.UseCSharpNullComparisonBehavior)
        return this.ImplementEquality(left, right, pattern);
      return this.ImplementEquality(left, right, ExpressionConverter.EqualsPattern.Store);
    }

    private DbExpression ImplementEquality(
      DbExpression left,
      DbExpression right,
      ExpressionConverter.EqualsPattern pattern)
    {
      switch (left.ExpressionKind)
      {
        case DbExpressionKind.Constant:
          switch (right.ExpressionKind)
          {
            case DbExpressionKind.Constant:
              return (DbExpression) left.Equal(right);
            case DbExpressionKind.Null:
              return (DbExpression) DbExpressionBuilder.False;
            default:
              return this.ImplementEqualityConstantAndUnknown((DbConstantExpression) left, right, pattern);
          }
        case DbExpressionKind.Null:
          switch (right.ExpressionKind)
          {
            case DbExpressionKind.Constant:
              return (DbExpression) DbExpressionBuilder.False;
            case DbExpressionKind.Null:
              return (DbExpression) DbExpressionBuilder.True;
            default:
              return (DbExpression) right.IsNull();
          }
        default:
          switch (right.ExpressionKind)
          {
            case DbExpressionKind.Constant:
              return this.ImplementEqualityConstantAndUnknown((DbConstantExpression) right, left, pattern);
            case DbExpressionKind.Null:
              return (DbExpression) left.IsNull();
            default:
              return this.ImplementEqualityUnknownArguments(left, right, pattern);
          }
      }
    }

    private DbExpression ImplementEqualityConstantAndUnknown(
      DbConstantExpression constant,
      DbExpression unknown,
      ExpressionConverter.EqualsPattern pattern)
    {
      switch (pattern)
      {
        case ExpressionConverter.EqualsPattern.Store:
        case ExpressionConverter.EqualsPattern.PositiveNullEqualityNonComposable:
          return (DbExpression) constant.Equal(unknown);
        case ExpressionConverter.EqualsPattern.PositiveNullEqualityComposable:
          if (!this._funcletizer.RootContext.ContextOptions.UseCSharpNullComparisonBehavior)
            return (DbExpression) constant.Equal(unknown);
          return (DbExpression) constant.Equal(unknown).And((DbExpression) unknown.IsNull().Not());
        default:
          return (DbExpression) null;
      }
    }

    private DbExpression ImplementEqualityUnknownArguments(
      DbExpression left,
      DbExpression right,
      ExpressionConverter.EqualsPattern pattern)
    {
      switch (pattern)
      {
        case ExpressionConverter.EqualsPattern.Store:
          return (DbExpression) left.Equal(right);
        case ExpressionConverter.EqualsPattern.PositiveNullEqualityNonComposable:
          return (DbExpression) left.Equal(right).Or((DbExpression) left.IsNull().And((DbExpression) right.IsNull()));
        case ExpressionConverter.EqualsPattern.PositiveNullEqualityComposable:
          DbComparisonExpression left1 = left.Equal(right);
          DbAndExpression dbAndExpression = left.IsNull().And((DbExpression) right.IsNull());
          if (!this._funcletizer.RootContext.ContextOptions.UseCSharpNullComparisonBehavior)
            return (DbExpression) left1.Or((DbExpression) dbAndExpression);
          DbOrExpression dbOrExpression = left.IsNull().Or((DbExpression) right.IsNull());
          return (DbExpression) left1.And((DbExpression) dbOrExpression.Not()).Or((DbExpression) dbAndExpression);
        default:
          return (DbExpression) null;
      }
    }

    private DbExpression TranslateFunctionIntoLike(
      MethodCallExpression call,
      bool insertPercentAtStart,
      bool insertPercentAtEnd,
      Func<ExpressionConverter, MethodCallExpression, DbExpression, DbExpression, DbExpression> defaultTranslator)
    {
      char escapeCharacter;
      bool flag1 = this.ProviderManifest.SupportsEscapingLikeArgument(out escapeCharacter);
      bool flag2 = false;
      bool specifyEscape = true;
      Expression linq1 = call.Arguments[0];
      Expression linq2 = call.Object;
      QueryParameterExpression parameterExpression = linq1 as QueryParameterExpression;
      if (flag1 && parameterExpression != null)
      {
        flag2 = true;
        bool specifyEscapeDummy;
        linq1 = (Expression) parameterExpression.EscapeParameterForLike((Func<string, string>) (input => this.PreparePattern(input, insertPercentAtStart, insertPercentAtEnd, out specifyEscapeDummy)));
      }
      DbExpression pattern = this.TranslateExpression(linq1);
      DbExpression dbExpression1 = this.TranslateExpression(linq2);
      if (flag1 && pattern.ExpressionKind == DbExpressionKind.Constant)
      {
        flag2 = true;
        DbConstantExpression constantExpression = (DbConstantExpression) pattern;
        string str = this.PreparePattern((string) constantExpression.Value, insertPercentAtStart, insertPercentAtEnd, out specifyEscape);
        pattern = (DbExpression) constantExpression.ResultType.Constant((object) str);
      }
      DbExpression dbExpression2;
      if (flag2)
      {
        if (specifyEscape)
        {
          DbConstantExpression constantExpression = EdmProviderManifest.Instance.GetCanonicalModelTypeUsage(PrimitiveTypeKind.String).Constant((object) new string(new char[1]
          {
            escapeCharacter
          }));
          dbExpression2 = (DbExpression) dbExpression1.Like(pattern, (DbExpression) constantExpression);
        }
        else
          dbExpression2 = (DbExpression) dbExpression1.Like(pattern);
      }
      else
        dbExpression2 = defaultTranslator(this, call, pattern, dbExpression1);
      return dbExpression2;
    }

    private string PreparePattern(
      string patternValue,
      bool insertPercentAtStart,
      bool insertPercentAtEnd,
      out bool specifyEscape)
    {
      if (patternValue == null)
      {
        specifyEscape = false;
        return (string) null;
      }
      string str = this.ProviderManifest.EscapeLikeArgument(patternValue);
      if (str == null)
        throw new ProviderIncompatibleException(Strings.ProviderEscapeLikeArgumentReturnedNull);
      specifyEscape = patternValue != str;
      StringBuilder stringBuilder = new StringBuilder();
      if (insertPercentAtStart)
        stringBuilder.Append("%");
      stringBuilder.Append(str);
      if (insertPercentAtEnd)
        stringBuilder.Append("%");
      return stringBuilder.ToString();
    }

    private DbFunctionExpression TranslateIntoCanonicalFunction(
      string functionName,
      Expression Expression,
      params Expression[] linqArguments)
    {
      DbExpression[] dbExpressionArray = new DbExpression[linqArguments.Length];
      for (int index = 0; index < linqArguments.Length; ++index)
        dbExpressionArray[index] = this.TranslateExpression(linqArguments[index]);
      return this.CreateCanonicalFunction(functionName, Expression, dbExpressionArray);
    }

    private DbFunctionExpression CreateCanonicalFunction(
      string functionName,
      Expression Expression,
      params DbExpression[] translatedArguments)
    {
      List<TypeUsage> typeUsageList = new List<TypeUsage>(translatedArguments.Length);
      foreach (DbExpression translatedArgument in translatedArguments)
        typeUsageList.Add(translatedArgument.ResultType);
      return this.FindCanonicalFunction(functionName, (IList<TypeUsage>) typeUsageList, false, Expression).Invoke(translatedArguments);
    }

    private EdmFunction FindCanonicalFunction(
      string functionName,
      IList<TypeUsage> argumentTypes,
      bool isGroupAggregateFunction,
      Expression Expression)
    {
      return this.FindFunction("Edm", functionName, argumentTypes, isGroupAggregateFunction, Expression);
    }

    private EdmFunction FindFunction(
      string namespaceName,
      string functionName,
      IList<TypeUsage> argumentTypes,
      bool isGroupAggregateFunction,
      Expression Expression)
    {
      IList<EdmFunction> functionOverloads;
      if (!this._perspective.TryGetFunctionByName(namespaceName, functionName, false, out functionOverloads))
        ExpressionConverter.ThrowUnresolvableFunction(Expression);
      bool isAmbiguous;
      EdmFunction edmFunction = FunctionOverloadResolver.ResolveFunctionOverloads(functionOverloads, argumentTypes, isGroupAggregateFunction, out isAmbiguous);
      if (isAmbiguous || edmFunction == null)
        ExpressionConverter.ThrowUnresolvableFunctionOverload(Expression, isAmbiguous);
      return edmFunction;
    }

    private static void ThrowUnresolvableFunction(Expression Expression)
    {
      if (Expression.NodeType == ExpressionType.Call)
      {
        MethodInfo method = ((MethodCallExpression) Expression).Method;
        throw new NotSupportedException(Strings.ELinq_UnresolvableFunctionForMethod((object) method, (object) method.DeclaringType));
      }
      if (Expression.NodeType == ExpressionType.MemberAccess)
      {
        string name;
        Type type;
        MemberInfo memberInfo = TypeSystem.PropertyOrField(((MemberExpression) Expression).Member, out name, out type);
        throw new NotSupportedException(Strings.ELinq_UnresolvableFunctionForMember((object) memberInfo, (object) memberInfo.DeclaringType));
      }
      throw new NotSupportedException(Strings.ELinq_UnresolvableFunctionForExpression((object) Expression.NodeType));
    }

    private static void ThrowUnresolvableFunctionOverload(Expression Expression, bool isAmbiguous)
    {
      if (Expression.NodeType == ExpressionType.Call)
      {
        MethodInfo method = ((MethodCallExpression) Expression).Method;
        if (isAmbiguous)
          throw new NotSupportedException(Strings.ELinq_UnresolvableFunctionForMethodAmbiguousMatch((object) method, (object) method.DeclaringType));
        throw new NotSupportedException(Strings.ELinq_UnresolvableFunctionForMethodNotFound((object) method, (object) method.DeclaringType));
      }
      if (Expression.NodeType == ExpressionType.MemberAccess)
      {
        string name;
        Type type;
        MemberInfo memberInfo = TypeSystem.PropertyOrField(((MemberExpression) Expression).Member, out name, out type);
        throw new NotSupportedException(Strings.ELinq_UnresolvableStoreFunctionForMember((object) memberInfo, (object) memberInfo.DeclaringType));
      }
      throw new NotSupportedException(Strings.ELinq_UnresolvableStoreFunctionForExpression((object) Expression.NodeType));
    }

    private static DbNewInstanceExpression CreateNewRowExpression(
      List<KeyValuePair<string, DbExpression>> columns,
      InitializerMetadata initializerMetadata)
    {
      List<DbExpression> dbExpressionList = new List<DbExpression>(columns.Count);
      List<EdmProperty> edmPropertyList = new List<EdmProperty>(columns.Count);
      for (int index = 0; index < columns.Count; ++index)
      {
        KeyValuePair<string, DbExpression> column = columns[index];
        dbExpressionList.Add(column.Value);
        edmPropertyList.Add(new EdmProperty(column.Key, column.Value.ResultType));
      }
      return TypeUsage.Create((EdmType) new RowType((IEnumerable<EdmProperty>) edmPropertyList, initializerMetadata)).New((IEnumerable<DbExpression>) dbExpressionList);
    }

    internal static class StringTranslatorUtil
    {
      internal static IEnumerable<Expression> GetConcatArgs(Expression linq)
      {
        if (linq.IsStringAddExpression())
        {
          foreach (Expression concatArg in ExpressionConverter.StringTranslatorUtil.GetConcatArgs((BinaryExpression) linq))
            yield return concatArg;
        }
        else
          yield return linq;
      }

      internal static IEnumerable<Expression> GetConcatArgs(
        BinaryExpression linq)
      {
        foreach (Expression concatArg in ExpressionConverter.StringTranslatorUtil.GetConcatArgs(linq.Left))
          yield return concatArg;
        foreach (Expression concatArg in ExpressionConverter.StringTranslatorUtil.GetConcatArgs(linq.Right))
          yield return concatArg;
      }

      internal static DbExpression ConcatArgs(
        ExpressionConverter parent,
        BinaryExpression linq)
      {
        return ExpressionConverter.StringTranslatorUtil.ConcatArgs(parent, (Expression) linq, ExpressionConverter.StringTranslatorUtil.GetConcatArgs(linq).ToArray<Expression>());
      }

      internal static DbExpression ConcatArgs(
        ExpressionConverter parent,
        Expression linq,
        Expression[] linqArgs)
      {
        DbExpression[] array = ((IEnumerable<Expression>) linqArgs).Where<Expression>((Func<Expression, bool>) (arg => !arg.IsNullConstant())).Select<Expression, DbExpression>((Func<Expression, DbExpression>) (arg => ExpressionConverter.StringTranslatorUtil.ConvertToString(parent, arg))).ToArray<DbExpression>();
        if (array.Length == 0)
          return (DbExpression) DbExpressionBuilder.Constant((object) string.Empty);
        DbExpression dbExpression1 = ((IEnumerable<DbExpression>) array).First<DbExpression>();
        foreach (DbExpression dbExpression2 in ((IEnumerable<DbExpression>) array).Skip<DbExpression>(1))
          dbExpression1 = (DbExpression) parent.CreateCanonicalFunction("Concat", linq, dbExpression1, dbExpression2);
        return dbExpression1;
      }

      internal static DbExpression StripNull(
        Expression sourceExpression,
        DbExpression inputExpression,
        DbExpression outputExpression,
        bool useDatabaseNullSemantics)
      {
        if (sourceExpression.IsNullConstant())
          return (DbExpression) DbExpressionBuilder.Constant((object) string.Empty);
        if (sourceExpression.NodeType == ExpressionType.Constant || useDatabaseNullSemantics)
          return outputExpression;
        return (DbExpression) DbExpressionBuilder.Case((IEnumerable<DbExpression>) new DbIsNullExpression[1]
        {
          inputExpression.IsNull()
        }, (IEnumerable<DbExpression>) new DbConstantExpression[1]
        {
          DbExpressionBuilder.Constant((object) string.Empty)
        }, outputExpression);
      }

      [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "the same linqExpression value is never cast to ConstantExpression twice")]
      internal static DbExpression ConvertToString(
        ExpressionConverter parent,
        Expression linqExpression)
      {
        if (linqExpression.Type == typeof (object))
        {
          ConstantExpression constantExpression = linqExpression as ConstantExpression;
          linqExpression = constantExpression != null ? (Expression) Expression.Constant(constantExpression.Value) : linqExpression.RemoveConvert();
        }
        DbExpression expression = parent.TranslateExpression(linqExpression);
        Type nonNullableType = TypeSystem.GetNonNullableType(linqExpression.Type);
        bool useDatabaseNullSemantics = !parent._funcletizer.RootContext.ContextOptions.UseCSharpNullComparisonBehavior;
        if (nonNullableType.IsEnum)
        {
          if (Attribute.IsDefined((MemberInfo) nonNullableType, typeof (FlagsAttribute)))
            throw new NotSupportedException(Strings.Elinq_ToStringNotSupportedForEnumsWithFlags);
          if (linqExpression.IsNullConstant())
            return (DbExpression) DbExpressionBuilder.Constant((object) string.Empty);
          if (linqExpression.NodeType == ExpressionType.Constant)
          {
            object obj = ((ConstantExpression) linqExpression).Value;
            return (DbExpression) DbExpressionBuilder.Constant((object) (Enum.GetName(nonNullableType, obj) ?? obj.ToString()));
          }
          Type integralType = nonNullableType.GetEnumUnderlyingType();
          TypeUsage type = parent.GetValueLayerType(integralType);
          IEnumerable<DbExpression> whenExpressions = nonNullableType.GetEnumValues().Cast<object>().Select<object, object>((Func<object, object>) (v => Convert.ChangeType(v, integralType, (IFormatProvider) CultureInfo.InvariantCulture))).Select<object, DbConstantExpression>((Func<object, DbConstantExpression>) (v => DbExpressionBuilder.Constant(v))).Select<DbConstantExpression, DbExpression>((Func<DbConstantExpression, DbExpression>) (c => (DbExpression) expression.CastTo(type).Equal((DbExpression) c))).Concat<DbExpression>((IEnumerable<DbExpression>) new DbIsNullExpression[1]
          {
            expression.CastTo(type).IsNull()
          });
          IEnumerable<DbConstantExpression> constantExpressions = ((IEnumerable<string>) nonNullableType.GetEnumNames()).Select<string, DbConstantExpression>((Func<string, DbConstantExpression>) (s => DbExpressionBuilder.Constant((object) s))).Concat<DbConstantExpression>((IEnumerable<DbConstantExpression>) new DbConstantExpression[1]
          {
            DbExpressionBuilder.Constant((object) string.Empty)
          });
          UnaryExpression unaryExpression = Expression.Convert(linqExpression, integralType);
          DbCastExpression dbCastExpression = parent.TranslateExpression((Expression) unaryExpression).CastTo(parent.GetValueLayerType(typeof (string)));
          return (DbExpression) DbExpressionBuilder.Case(whenExpressions, (IEnumerable<DbExpression>) constantExpressions, (DbExpression) dbCastExpression);
        }
        if (TypeSemantics.IsPrimitiveType(expression.ResultType, PrimitiveTypeKind.String))
          return ExpressionConverter.StringTranslatorUtil.StripNull(linqExpression, expression, expression, useDatabaseNullSemantics);
        if (TypeSemantics.IsPrimitiveType(expression.ResultType, PrimitiveTypeKind.Guid))
          return ExpressionConverter.StringTranslatorUtil.StripNull(linqExpression, expression, (DbExpression) expression.CastTo(parent.GetValueLayerType(typeof (string))).ToLower(), useDatabaseNullSemantics);
        if (TypeSemantics.IsPrimitiveType(expression.ResultType, PrimitiveTypeKind.Boolean))
        {
          if (linqExpression.IsNullConstant())
            return (DbExpression) DbExpressionBuilder.Constant((object) string.Empty);
          if (linqExpression.NodeType == ExpressionType.Constant)
            return (DbExpression) DbExpressionBuilder.Constant((object) ((ConstantExpression) linqExpression).Value.ToString());
          return (DbExpression) DbExpressionBuilder.Case((IEnumerable<DbExpression>) new DbComparisonExpression[2]
          {
            expression.Equal((DbExpression) DbExpressionBuilder.True),
            expression.Equal((DbExpression) DbExpressionBuilder.False)
          }, (IEnumerable<DbExpression>) new DbConstantExpression[2]
          {
            DbExpressionBuilder.Constant((object) true.ToString()),
            DbExpressionBuilder.Constant((object) false.ToString())
          }, (DbExpression) DbExpressionBuilder.Constant((object) string.Empty));
        }
        if (!ExpressionConverter.StringTranslatorUtil.SupportsCastToString(expression.ResultType))
          throw new NotSupportedException(Strings.Elinq_ToStringNotSupportedForType((object) expression.ResultType.EdmType.Name));
        return ExpressionConverter.StringTranslatorUtil.StripNull(linqExpression, expression, (DbExpression) expression.CastTo(parent.GetValueLayerType(typeof (string))), useDatabaseNullSemantics);
      }

      internal static bool SupportsCastToString(TypeUsage typeUsage)
      {
        if (!TypeSemantics.IsPrimitiveType(typeUsage, PrimitiveTypeKind.String) && !TypeSemantics.IsNumericType(typeUsage) && (!TypeSemantics.IsBooleanType(typeUsage) && !TypeSemantics.IsPrimitiveType(typeUsage, PrimitiveTypeKind.DateTime)) && (!TypeSemantics.IsPrimitiveType(typeUsage, PrimitiveTypeKind.DateTimeOffset) && !TypeSemantics.IsPrimitiveType(typeUsage, PrimitiveTypeKind.Time)))
          return TypeSemantics.IsPrimitiveType(typeUsage, PrimitiveTypeKind.Guid);
        return true;
      }
    }

    private class ParameterReferenceRemover : DefaultExpressionVisitor
    {
      private readonly ObjectParameterCollection objectParameters;

      internal static DbExpression RemoveParameterReferences(
        DbExpression expression,
        ObjectParameterCollection availableParameters)
      {
        return new ExpressionConverter.ParameterReferenceRemover(availableParameters).VisitExpression(expression);
      }

      private ParameterReferenceRemover(ObjectParameterCollection availableParams)
      {
        this.objectParameters = availableParams;
      }

      public override DbExpression Visit(DbParameterReferenceExpression expression)
      {
        Check.NotNull<DbParameterReferenceExpression>(expression, nameof (expression));
        if (!this.objectParameters.Contains(expression.ParameterName))
          return (DbExpression) expression;
        ObjectParameter objectParameter = this.objectParameters[expression.ParameterName];
        if (objectParameter.Value == null)
          return (DbExpression) expression.ResultType.Null();
        return (DbExpression) expression.ResultType.Constant(objectParameter.Value);
      }
    }

    private enum EqualsPattern
    {
      Store,
      PositiveNullEqualityNonComposable,
      PositiveNullEqualityComposable,
    }

    internal abstract class Translator
    {
      private readonly ExpressionType[] _nodeTypes;

      protected Translator(params ExpressionType[] nodeTypes)
      {
        this._nodeTypes = nodeTypes;
      }

      internal IEnumerable<ExpressionType> NodeTypes
      {
        get
        {
          return (IEnumerable<ExpressionType>) this._nodeTypes;
        }
      }

      internal abstract DbExpression Translate(
        ExpressionConverter parent,
        Expression linq);

      public override string ToString()
      {
        return this.GetType().Name;
      }
    }

    internal abstract class TypedTranslator<T_Linq> : ExpressionConverter.Translator
      where T_Linq : Expression
    {
      protected TypedTranslator(params ExpressionType[] nodeTypes)
        : base(nodeTypes)
      {
      }

      internal override DbExpression Translate(
        ExpressionConverter parent,
        Expression linq)
      {
        return this.TypedTranslate(parent, (T_Linq) linq);
      }

      protected abstract DbExpression TypedTranslate(
        ExpressionConverter parent,
        T_Linq linq);
    }

    internal sealed class MethodCallTranslator : ExpressionConverter.TypedTranslator<MethodCallExpression>
    {
      private static readonly ExpressionConverter.MethodCallTranslator.CallTranslator _defaultTranslator = (ExpressionConverter.MethodCallTranslator.CallTranslator) new ExpressionConverter.MethodCallTranslator.DefaultTranslator();
      private static readonly ExpressionConverter.MethodCallTranslator.FunctionCallTranslator _functionCallTranslator = new ExpressionConverter.MethodCallTranslator.FunctionCallTranslator();
      private static readonly Dictionary<MethodInfo, ExpressionConverter.MethodCallTranslator.CallTranslator> _methodTranslators = ExpressionConverter.MethodCallTranslator.InitializeMethodTranslators();
      private static readonly Dictionary<SequenceMethod, ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator> _sequenceTranslators = ExpressionConverter.MethodCallTranslator.InitializeSequenceMethodTranslators();
      private static readonly Dictionary<string, ExpressionConverter.MethodCallTranslator.ObjectQueryCallTranslator> _objectQueryTranslators = ExpressionConverter.MethodCallTranslator.InitializeObjectQueryTranslators();
      private static readonly object _vbInitializerLock = new object();
      private const string s_stringsTypeFullName = "Microsoft.VisualBasic.Strings";
      private static bool s_vbMethodsInitialized;

      internal MethodCallTranslator()
        : base(ExpressionType.Call)
      {
      }

      protected override DbExpression TypedTranslate(
        ExpressionConverter parent,
        MethodCallExpression linq)
      {
        SequenceMethod sequenceMethod;
        ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator methodTranslator;
        if (ReflectionUtil.TryIdentifySequenceMethod(linq.Method, out sequenceMethod) && ExpressionConverter.MethodCallTranslator._sequenceTranslators.TryGetValue(sequenceMethod, out methodTranslator))
          return methodTranslator.Translate(parent, linq, sequenceMethod);
        ExpressionConverter.MethodCallTranslator.CallTranslator callTranslator;
        if (ExpressionConverter.MethodCallTranslator.TryGetCallTranslator(linq.Method, out callTranslator))
          return callTranslator.Translate(parent, linq);
        ExpressionConverter.MethodCallTranslator.ObjectQueryCallTranslator queryCallTranslator;
        if (ExpressionConverter.MethodCallTranslator.ObjectQueryCallTranslator.IsCandidateMethod(linq.Method) && ExpressionConverter.MethodCallTranslator._objectQueryTranslators.TryGetValue(linq.Method.Name, out queryCallTranslator))
          return queryCallTranslator.Translate(parent, linq);
        DbFunctionAttribute functionAttribute = linq.Method.GetCustomAttributes<DbFunctionAttribute>(false).FirstOrDefault<DbFunctionAttribute>();
        if (functionAttribute != null)
          return ExpressionConverter.MethodCallTranslator._functionCallTranslator.TranslateFunctionCall(parent, linq, functionAttribute);
        switch (linq.Method.Name)
        {
          case "Contains":
            Type[] genericTypeArguments;
            if (((IEnumerable<ParameterInfo>) linq.Method.GetParameters()).Count<ParameterInfo>() == 1 && linq.Method.ReturnType.Equals(typeof (bool)) && linq.Method.IsImplementationOfGenericInterfaceMethod(typeof (ICollection<>), out genericTypeArguments))
              return ExpressionConverter.MethodCallTranslator.ContainsTranslator.TranslateContains(parent, linq.Object, linq.Arguments[0]);
            break;
        }
        return ExpressionConverter.MethodCallTranslator._defaultTranslator.Translate(parent, linq);
      }

      private static Dictionary<MethodInfo, ExpressionConverter.MethodCallTranslator.CallTranslator> InitializeMethodTranslators()
      {
        Dictionary<MethodInfo, ExpressionConverter.MethodCallTranslator.CallTranslator> dictionary = new Dictionary<MethodInfo, ExpressionConverter.MethodCallTranslator.CallTranslator>();
        foreach (ExpressionConverter.MethodCallTranslator.CallTranslator callTranslator in ExpressionConverter.MethodCallTranslator.GetCallTranslators())
        {
          foreach (MethodInfo method in callTranslator.Methods)
            dictionary.Add(method, callTranslator);
        }
        return dictionary;
      }

      private static Dictionary<SequenceMethod, ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator> InitializeSequenceMethodTranslators()
      {
        Dictionary<SequenceMethod, ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator> dictionary = new Dictionary<SequenceMethod, ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator>();
        foreach (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator methodTranslator in ExpressionConverter.MethodCallTranslator.GetSequenceMethodTranslators())
        {
          foreach (SequenceMethod method in methodTranslator.Methods)
            dictionary.Add(method, methodTranslator);
        }
        return dictionary;
      }

      private static Dictionary<string, ExpressionConverter.MethodCallTranslator.ObjectQueryCallTranslator> InitializeObjectQueryTranslators()
      {
        Dictionary<string, ExpressionConverter.MethodCallTranslator.ObjectQueryCallTranslator> dictionary = new Dictionary<string, ExpressionConverter.MethodCallTranslator.ObjectQueryCallTranslator>((IEqualityComparer<string>) StringComparer.Ordinal);
        foreach (ExpressionConverter.MethodCallTranslator.ObjectQueryCallTranslator queryCallTranslator in ExpressionConverter.MethodCallTranslator.GetObjectQueryCallTranslators())
          dictionary[queryCallTranslator.MethodName] = queryCallTranslator;
        return dictionary;
      }

      private static bool TryGetCallTranslator(
        MethodInfo methodInfo,
        out ExpressionConverter.MethodCallTranslator.CallTranslator callTranslator)
      {
        if (ExpressionConverter.MethodCallTranslator._methodTranslators.TryGetValue(methodInfo, out callTranslator))
          return true;
        if ("Microsoft.VisualBasic, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" == methodInfo.DeclaringType.Assembly().FullName)
        {
          lock (ExpressionConverter.MethodCallTranslator._vbInitializerLock)
          {
            if (!ExpressionConverter.MethodCallTranslator.s_vbMethodsInitialized)
            {
              ExpressionConverter.MethodCallTranslator.InitializeVBMethods(methodInfo.DeclaringType.Assembly());
              ExpressionConverter.MethodCallTranslator.s_vbMethodsInitialized = true;
            }
            return ExpressionConverter.MethodCallTranslator._methodTranslators.TryGetValue(methodInfo, out callTranslator);
          }
        }
        else
        {
          callTranslator = (ExpressionConverter.MethodCallTranslator.CallTranslator) null;
          return false;
        }
      }

      private static void InitializeVBMethods(Assembly vbAssembly)
      {
        foreach (ExpressionConverter.MethodCallTranslator.CallTranslator basicCallTranslator in ExpressionConverter.MethodCallTranslator.GetVisualBasicCallTranslators(vbAssembly))
        {
          foreach (MethodInfo method in basicCallTranslator.Methods)
            ExpressionConverter.MethodCallTranslator._methodTranslators.Add(method, basicCallTranslator);
        }
      }

      private static IEnumerable<ExpressionConverter.MethodCallTranslator.CallTranslator> GetVisualBasicCallTranslators(
        Assembly vbAssembly)
      {
        yield return (ExpressionConverter.MethodCallTranslator.CallTranslator) new ExpressionConverter.MethodCallTranslator.VBCanonicalFunctionDefaultTranslator(vbAssembly);
        yield return (ExpressionConverter.MethodCallTranslator.CallTranslator) new ExpressionConverter.MethodCallTranslator.VBCanonicalFunctionRenameTranslator(vbAssembly);
        yield return (ExpressionConverter.MethodCallTranslator.CallTranslator) new ExpressionConverter.MethodCallTranslator.VBDatePartTranslator(vbAssembly);
      }

      private static IEnumerable<ExpressionConverter.MethodCallTranslator.CallTranslator> GetCallTranslators()
      {
        return (IEnumerable<ExpressionConverter.MethodCallTranslator.CallTranslator>) new ExpressionConverter.MethodCallTranslator.CallTranslator[21]
        {
          (ExpressionConverter.MethodCallTranslator.CallTranslator) new ExpressionConverter.MethodCallTranslator.CanonicalFunctionDefaultTranslator(),
          (ExpressionConverter.MethodCallTranslator.CallTranslator) new ExpressionConverter.MethodCallTranslator.AsUnicodeFunctionTranslator(),
          (ExpressionConverter.MethodCallTranslator.CallTranslator) new ExpressionConverter.MethodCallTranslator.AsNonUnicodeFunctionTranslator(),
          (ExpressionConverter.MethodCallTranslator.CallTranslator) new ExpressionConverter.MethodCallTranslator.MathTruncateTranslator(),
          (ExpressionConverter.MethodCallTranslator.CallTranslator) new ExpressionConverter.MethodCallTranslator.MathPowerTranslator(),
          (ExpressionConverter.MethodCallTranslator.CallTranslator) new ExpressionConverter.MethodCallTranslator.GuidNewGuidTranslator(),
          (ExpressionConverter.MethodCallTranslator.CallTranslator) new ExpressionConverter.MethodCallTranslator.StringContainsTranslator(),
          (ExpressionConverter.MethodCallTranslator.CallTranslator) new ExpressionConverter.MethodCallTranslator.StartsWithTranslator(),
          (ExpressionConverter.MethodCallTranslator.CallTranslator) new ExpressionConverter.MethodCallTranslator.EndsWithTranslator(),
          (ExpressionConverter.MethodCallTranslator.CallTranslator) new ExpressionConverter.MethodCallTranslator.IndexOfTranslator(),
          (ExpressionConverter.MethodCallTranslator.CallTranslator) new ExpressionConverter.MethodCallTranslator.SubstringTranslator(),
          (ExpressionConverter.MethodCallTranslator.CallTranslator) new ExpressionConverter.MethodCallTranslator.RemoveTranslator(),
          (ExpressionConverter.MethodCallTranslator.CallTranslator) new ExpressionConverter.MethodCallTranslator.InsertTranslator(),
          (ExpressionConverter.MethodCallTranslator.CallTranslator) new ExpressionConverter.MethodCallTranslator.IsNullOrEmptyTranslator(),
          (ExpressionConverter.MethodCallTranslator.CallTranslator) new ExpressionConverter.MethodCallTranslator.StringConcatTranslator(),
          (ExpressionConverter.MethodCallTranslator.CallTranslator) new ExpressionConverter.MethodCallTranslator.TrimTranslator(),
          (ExpressionConverter.MethodCallTranslator.CallTranslator) new ExpressionConverter.MethodCallTranslator.TrimStartTranslator(),
          (ExpressionConverter.MethodCallTranslator.CallTranslator) new ExpressionConverter.MethodCallTranslator.TrimEndTranslator(),
          (ExpressionConverter.MethodCallTranslator.CallTranslator) new ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator(),
          (ExpressionConverter.MethodCallTranslator.CallTranslator) new ExpressionConverter.MethodCallTranslator.HasFlagTranslator(),
          (ExpressionConverter.MethodCallTranslator.CallTranslator) new ExpressionConverter.MethodCallTranslator.ToStringTranslator()
        };
      }

      private static IEnumerable<ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator> GetSequenceMethodTranslators()
      {
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.ConcatTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.UnionTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.IntersectTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.ExceptTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.DistinctTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.WhereTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.SelectTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.OrderByTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.OrderByDescendingTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.ThenByTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.ThenByDescendingTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.SelectManyTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.AnyTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.AnyPredicateTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.AllTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.JoinTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.GroupByTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.MaxTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.MinTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.AverageTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.SumTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.CountTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.LongCountTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.CastMethodTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.GroupJoinTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.OfTypeTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.PassthroughTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.DefaultIfEmptyTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.FirstTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.FirstPredicateTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.FirstOrDefaultTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.FirstOrDefaultPredicateTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.TakeTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.SkipTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.SingleTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.SinglePredicateTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.SingleOrDefaultTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.SingleOrDefaultPredicateTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator) new ExpressionConverter.MethodCallTranslator.ContainsTranslator();
      }

      private static IEnumerable<ExpressionConverter.MethodCallTranslator.ObjectQueryCallTranslator> GetObjectQueryCallTranslators()
      {
        yield return (ExpressionConverter.MethodCallTranslator.ObjectQueryCallTranslator) new ExpressionConverter.MethodCallTranslator.ObjectQueryBuilderDistinctTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.ObjectQueryCallTranslator) new ExpressionConverter.MethodCallTranslator.ObjectQueryBuilderExceptTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.ObjectQueryCallTranslator) new ExpressionConverter.MethodCallTranslator.ObjectQueryBuilderFirstTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.ObjectQueryCallTranslator) new ExpressionConverter.MethodCallTranslator.ObjectQueryBuilderToListTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.ObjectQueryCallTranslator) new ExpressionConverter.MethodCallTranslator.ObjectQueryIncludeTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.ObjectQueryCallTranslator) new ExpressionConverter.MethodCallTranslator.ObjectQueryBuilderIntersectTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.ObjectQueryCallTranslator) new ExpressionConverter.MethodCallTranslator.ObjectQueryBuilderOfTypeTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.ObjectQueryCallTranslator) new ExpressionConverter.MethodCallTranslator.ObjectQueryBuilderUnionTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.ObjectQueryCallTranslator) new ExpressionConverter.MethodCallTranslator.ObjectQueryMergeAsTranslator();
        yield return (ExpressionConverter.MethodCallTranslator.ObjectQueryCallTranslator) new ExpressionConverter.MethodCallTranslator.ObjectQueryIncludeSpanTranslator();
      }

      private static bool IsTrivialRename(
        LambdaExpression selectorLambda,
        ExpressionConverter converter,
        out string leftName,
        out string rightName,
        out InitializerMetadata initializerMetadata)
      {
        leftName = (string) null;
        rightName = (string) null;
        initializerMetadata = (InitializerMetadata) null;
        if (selectorLambda.Parameters.Count != 2 || selectorLambda.Body.NodeType != ExpressionType.New)
          return false;
        NewExpression body = (NewExpression) selectorLambda.Body;
        if (body.Arguments.Count != 2 || body.Arguments[0] != selectorLambda.Parameters[0] || body.Arguments[1] != selectorLambda.Parameters[1])
          return false;
        leftName = body.Members[0].Name;
        rightName = body.Members[1].Name;
        initializerMetadata = InitializerMetadata.CreateProjectionInitializer(converter.EdmItemCollection, body);
        converter.ValidateInitializerMetadata(initializerMetadata);
        return true;
      }

      internal abstract class CallTranslator
      {
        private readonly IEnumerable<MethodInfo> _methods;

        protected CallTranslator(params MethodInfo[] methods)
        {
          this._methods = (IEnumerable<MethodInfo>) methods;
        }

        protected CallTranslator(IEnumerable<MethodInfo> methods)
        {
          this._methods = methods;
        }

        internal IEnumerable<MethodInfo> Methods
        {
          get
          {
            return this._methods;
          }
        }

        internal abstract DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call);

        public override string ToString()
        {
          return this.GetType().Name;
        }
      }

      private abstract class ObjectQueryCallTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
      {
        private readonly string _methodName;

        internal static bool IsCandidateMethod(MethodInfo method)
        {
          Type declaringType = method.DeclaringType;
          if ((method.IsPublic || method.IsAssembly && (method.Name == "MergeAs" || method.Name == "IncludeSpan")) && ((Type) null != declaringType && declaringType.IsGenericType()))
            return typeof (ObjectQuery<>) == declaringType.GetGenericTypeDefinition();
          return false;
        }

        internal static Expression RemoveConvertToObjectQuery(Expression queryExpression)
        {
          if (queryExpression.NodeType == ExpressionType.Convert)
          {
            UnaryExpression unaryExpression = (UnaryExpression) queryExpression;
            Type type = unaryExpression.Operand.Type;
            if (type.IsGenericType() && (typeof (IQueryable<>) == type.GetGenericTypeDefinition() || typeof (IOrderedQueryable<>) == type.GetGenericTypeDefinition()))
              queryExpression = unaryExpression.Operand;
          }
          return queryExpression;
        }

        protected ObjectQueryCallTranslator(string methodName)
          : base()
        {
          this._methodName = methodName;
        }

        internal string MethodName
        {
          get
          {
            return this._methodName;
          }
        }
      }

      private abstract class ObjectQueryBuilderCallTranslator : ExpressionConverter.MethodCallTranslator.ObjectQueryCallTranslator
      {
        private readonly ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator _translator;

        protected ObjectQueryBuilderCallTranslator(
          string methodName,
          SequenceMethod sequenceEquivalent)
          : base(methodName)
        {
          ExpressionConverter.MethodCallTranslator._sequenceTranslators.TryGetValue(sequenceEquivalent, out this._translator);
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          return this._translator.Translate(parent, call);
        }
      }

      private sealed class ObjectQueryBuilderUnionTranslator : ExpressionConverter.MethodCallTranslator.ObjectQueryBuilderCallTranslator
      {
        internal ObjectQueryBuilderUnionTranslator()
          : base("Union", SequenceMethod.Union)
        {
        }
      }

      private sealed class ObjectQueryBuilderIntersectTranslator : ExpressionConverter.MethodCallTranslator.ObjectQueryBuilderCallTranslator
      {
        internal ObjectQueryBuilderIntersectTranslator()
          : base("Intersect", SequenceMethod.Intersect)
        {
        }
      }

      private sealed class ObjectQueryBuilderExceptTranslator : ExpressionConverter.MethodCallTranslator.ObjectQueryBuilderCallTranslator
      {
        internal ObjectQueryBuilderExceptTranslator()
          : base("Except", SequenceMethod.Except)
        {
        }
      }

      private sealed class ObjectQueryBuilderDistinctTranslator : ExpressionConverter.MethodCallTranslator.ObjectQueryBuilderCallTranslator
      {
        internal ObjectQueryBuilderDistinctTranslator()
          : base("Distinct", SequenceMethod.Distinct)
        {
        }
      }

      private sealed class ObjectQueryBuilderOfTypeTranslator : ExpressionConverter.MethodCallTranslator.ObjectQueryBuilderCallTranslator
      {
        internal ObjectQueryBuilderOfTypeTranslator()
          : base("OfType", SequenceMethod.OfType)
        {
        }
      }

      private sealed class ObjectQueryBuilderFirstTranslator : ExpressionConverter.MethodCallTranslator.ObjectQueryBuilderCallTranslator
      {
        internal ObjectQueryBuilderFirstTranslator()
          : base("First", SequenceMethod.First)
        {
        }
      }

      private sealed class ObjectQueryBuilderToListTranslator : ExpressionConverter.MethodCallTranslator.ObjectQueryBuilderCallTranslator
      {
        internal ObjectQueryBuilderToListTranslator()
          : base("ToList", SequenceMethod.ToList)
        {
        }
      }

      private sealed class ObjectQueryIncludeTranslator : ExpressionConverter.MethodCallTranslator.ObjectQueryCallTranslator
      {
        internal ObjectQueryIncludeTranslator()
          : base("Include")
        {
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          DbExpression expression = parent.TranslateExpression(call.Object);
          Span span;
          if (!parent.TryGetSpan(expression, out span))
            span = (Span) null;
          DbExpression dbExpression = parent.TranslateExpression(call.Arguments[0]);
          if (dbExpression.ExpressionKind != DbExpressionKind.Constant)
            throw new NotSupportedException(Strings.ELinq_UnsupportedInclude);
          string pathToInclude = (string) ((DbConstantExpression) dbExpression).Value;
          if (parent.CanIncludeSpanInfo())
            span = Span.IncludeIn(span, pathToInclude);
          return parent.AddSpanMapping(expression, span);
        }
      }

      private sealed class ObjectQueryMergeAsTranslator : ExpressionConverter.MethodCallTranslator.ObjectQueryCallTranslator
      {
        internal ObjectQueryMergeAsTranslator()
          : base("MergeAs")
        {
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          if (call.Arguments[0].NodeType != ExpressionType.Constant)
            throw new NotSupportedException(Strings.ELinq_UnsupportedMergeAs);
          MergeOption mergeOption = (MergeOption) ((ConstantExpression) call.Arguments[0]).Value;
          EntityUtil.CheckArgumentMergeOption(mergeOption);
          parent.NotifyMergeOption(mergeOption);
          Expression objectQuery = ExpressionConverter.MethodCallTranslator.ObjectQueryCallTranslator.RemoveConvertToObjectQuery(call.Object);
          DbExpression expression = parent.TranslateExpression(objectQuery);
          Span span;
          if (!parent.TryGetSpan(expression, out span))
            span = (Span) null;
          return parent.AddSpanMapping(expression, span);
        }
      }

      private sealed class ObjectQueryIncludeSpanTranslator : ExpressionConverter.MethodCallTranslator.ObjectQueryCallTranslator
      {
        internal ObjectQueryIncludeSpanTranslator()
          : base("IncludeSpan")
        {
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          Span span = (Span) ((ConstantExpression) call.Arguments[0]).Value;
          Expression objectQuery = ExpressionConverter.MethodCallTranslator.ObjectQueryCallTranslator.RemoveConvertToObjectQuery(call.Object);
          DbExpression expression = parent.TranslateExpression(objectQuery);
          if (!parent.CanIncludeSpanInfo())
            span = (Span) null;
          return parent.AddSpanMapping(expression, span);
        }
      }

      internal sealed class DefaultTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
      {
        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          MethodInfo method = call.Method;
          if (method.DeclaringType.Assembly().FullName == "Microsoft.VisualBasic, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" && method.Name == "Mid")
          {
            if (((IEnumerable<Type>) new Type[2]
            {
              typeof (string),
              typeof (int)
            }).SequenceEqual<Type>(((IEnumerable<ParameterInfo>) method.GetParameters()).Select<ParameterInfo, Type>((Func<ParameterInfo, Type>) (p => p.ParameterType))))
              throw new NotSupportedException(Strings.ELinq_UnsupportedMethodSuggestedAlternative((object) method, (object) "System.String Mid(System.String, Int32, Int32)"));
          }
          throw new NotSupportedException(Strings.ELinq_UnsupportedMethod((object) method));
        }

        public DefaultTranslator()
          : base()
        {
        }
      }

      private sealed class FunctionCallTranslator
      {
        internal DbExpression TranslateFunctionCall(
          ExpressionConverter parent,
          MethodCallExpression call,
          DbFunctionAttribute functionAttribute)
        {
          List<DbExpression> list1 = call.Arguments.Select<Expression, Expression>((Func<Expression, Expression>) (a => this.UnwrapNoOpConverts(a))).Select<Expression, DbExpression>((Func<Expression, DbExpression>) (b => this.NormalizeAllSetSources(parent, parent.TranslateExpression(b)))).ToList<DbExpression>();
          List<TypeUsage> list2 = list1.Select<DbExpression, TypeUsage>((Func<DbExpression, TypeUsage>) (a => a.ResultType)).ToList<TypeUsage>();
          EdmFunction function = parent.FindFunction(functionAttribute.NamespaceName, functionAttribute.FunctionName, (IList<TypeUsage>) list2, false, (Expression) call);
          if (!function.IsComposableAttribute)
            throw new NotSupportedException(Strings.CannotCallNoncomposableFunction((object) function.FullName));
          DbExpression result = (DbExpression) function.Invoke((IEnumerable<DbExpression>) list1);
          return this.ValidateReturnType(result, result.ResultType, parent, call, call.Type, false);
        }

        private DbExpression NormalizeAllSetSources(
          ExpressionConverter parent,
          DbExpression argumentExpr)
        {
          DbExpression input1 = (DbExpression) null;
          switch (argumentExpr.ResultType.EdmType.BuiltInTypeKind)
          {
            case BuiltInTypeKind.CollectionType:
              DbExpressionBinding input2 = argumentExpr.BindAs(parent.AliasGenerator.Next());
              DbExpression projection = this.NormalizeAllSetSources(parent, (DbExpression) input2.Variable);
              if (projection != input2.Variable)
              {
                input1 = (DbExpression) input2.Project(projection);
                break;
              }
              break;
            case BuiltInTypeKind.RowType:
              List<KeyValuePair<string, DbExpression>> keyValuePairList = new List<KeyValuePair<string, DbExpression>>();
              RowType edmType = argumentExpr.ResultType.EdmType as RowType;
              bool flag = false;
              foreach (EdmProperty property in edmType.Properties)
              {
                DbPropertyExpression propertyExpression = argumentExpr.Property(property);
                DbExpression dbExpression = this.NormalizeAllSetSources(parent, (DbExpression) propertyExpression);
                if (dbExpression != propertyExpression)
                {
                  flag = true;
                  keyValuePairList.Add(new KeyValuePair<string, DbExpression>(propertyExpression.Property.Name, dbExpression));
                }
                else
                  keyValuePairList.Add(new KeyValuePair<string, DbExpression>(propertyExpression.Property.Name, (DbExpression) propertyExpression));
              }
              input1 = !flag ? argumentExpr : (DbExpression) DbExpressionBuilder.NewRow((IEnumerable<KeyValuePair<string, DbExpression>>) keyValuePairList);
              break;
          }
          if (input1 != null && input1 != argumentExpr)
            return parent.NormalizeSetSource(input1);
          return parent.NormalizeSetSource(argumentExpr);
        }

        private Expression UnwrapNoOpConverts(Expression expression)
        {
          if (expression.NodeType == ExpressionType.Convert)
          {
            Expression expression1 = this.UnwrapNoOpConverts(((UnaryExpression) expression).Operand);
            if (expression.Type.IsAssignableFrom(expression1.Type))
              return expression1;
          }
          return expression;
        }

        private DbExpression ValidateReturnType(
          DbExpression result,
          TypeUsage actualReturnType,
          ExpressionConverter parent,
          MethodCallExpression call,
          Type clrReturnType,
          bool isElementOfCollection)
        {
          switch (actualReturnType.EdmType.BuiltInTypeKind)
          {
            case BuiltInTypeKind.CollectionType:
              if (!clrReturnType.IsGenericType())
                throw new NotSupportedException(Strings.ELinq_DbFunctionAttributedFunctionWithWrongReturnType((object) call.Method, (object) call.Method.DeclaringType));
              Type genericTypeDefinition = clrReturnType.GetGenericTypeDefinition();
              if (genericTypeDefinition != typeof (IEnumerable<>) && genericTypeDefinition != typeof (IQueryable<>))
                throw new NotSupportedException(Strings.ELinq_DbFunctionAttributedFunctionWithWrongReturnType((object) call.Method, (object) call.Method.DeclaringType));
              Type genericArgument = clrReturnType.GetGenericArguments()[0];
              result = this.ValidateReturnType(result, TypeHelpers.GetElementTypeUsage(actualReturnType), parent, call, genericArgument, true);
              break;
            case BuiltInTypeKind.RefType:
              if (clrReturnType != typeof (EntityKey))
                throw new NotSupportedException(Strings.ELinq_DbFunctionAttributedFunctionWithWrongReturnType((object) call.Method, (object) call.Method.DeclaringType));
              break;
            case BuiltInTypeKind.RowType:
              if (clrReturnType != typeof (DbDataRecord))
                throw new NotSupportedException(Strings.ELinq_DbFunctionAttributedFunctionWithWrongReturnType((object) call.Method, (object) call.Method.DeclaringType));
              break;
            default:
              if (isElementOfCollection && parent.GetCastTargetType(actualReturnType, clrReturnType, (Type) null, false) != null)
                throw new NotSupportedException(Strings.ELinq_DbFunctionAttributedFunctionWithWrongReturnType((object) call.Method, (object) call.Method.DeclaringType));
              TypeUsage valueLayerType = parent.GetValueLayerType(clrReturnType);
              if (!TypeSemantics.IsPromotableTo(actualReturnType, valueLayerType))
                throw new NotSupportedException(Strings.ELinq_DbFunctionAttributedFunctionWithWrongReturnType((object) call.Method, (object) call.Method.DeclaringType));
              if (!isElementOfCollection)
              {
                result = parent.AlignTypes(result, clrReturnType);
                break;
              }
              break;
          }
          return result;
        }
      }

      internal sealed class CanonicalFunctionDefaultTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
      {
        internal CanonicalFunctionDefaultTranslator()
          : base(ExpressionConverter.MethodCallTranslator.CanonicalFunctionDefaultTranslator.GetMethods())
        {
        }

        private static IEnumerable<MethodInfo> GetMethods()
        {
          List<MethodInfo> methodInfoList = new List<MethodInfo>()
          {
            typeof (Math).GetDeclaredMethod("Ceiling", typeof (Decimal)),
            typeof (Math).GetDeclaredMethod("Ceiling", typeof (double)),
            typeof (Math).GetDeclaredMethod("Floor", typeof (Decimal)),
            typeof (Math).GetDeclaredMethod("Floor", typeof (double)),
            typeof (Math).GetDeclaredMethod("Round", typeof (Decimal)),
            typeof (Math).GetDeclaredMethod("Round", typeof (double)),
            typeof (Math).GetDeclaredMethod("Round", typeof (Decimal), typeof (int)),
            typeof (Math).GetDeclaredMethod("Round", typeof (double), typeof (int)),
            typeof (Decimal).GetDeclaredMethod("Floor", typeof (Decimal)),
            typeof (Decimal).GetDeclaredMethod("Ceiling", typeof (Decimal)),
            typeof (Decimal).GetDeclaredMethod("Round", typeof (Decimal)),
            typeof (Decimal).GetDeclaredMethod("Round", typeof (Decimal), typeof (int)),
            typeof (string).GetDeclaredMethod("Replace", typeof (string), typeof (string)),
            typeof (string).GetDeclaredMethod("ToLower"),
            typeof (string).GetDeclaredMethod("ToUpper"),
            typeof (string).GetDeclaredMethod("Trim")
          };
          methodInfoList.AddRange(((IEnumerable<Type>) new Type[7]
          {
            typeof (Decimal),
            typeof (double),
            typeof (float),
            typeof (int),
            typeof (long),
            typeof (sbyte),
            typeof (short)
          }).Select<Type, MethodInfo>((Func<Type, MethodInfo>) (a => typeof (Math).GetDeclaredMethod("Abs", a))));
          return (IEnumerable<MethodInfo>) methodInfoList;
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          Expression[] array;
          if (!call.Method.IsStatic)
          {
            List<Expression> expressionList = new List<Expression>(call.Arguments.Count + 1);
            expressionList.Add(call.Object);
            expressionList.AddRange((IEnumerable<Expression>) call.Arguments);
            array = expressionList.ToArray();
          }
          else
            array = call.Arguments.ToArray<Expression>();
          return (DbExpression) parent.TranslateIntoCanonicalFunction(call.Method.Name, (Expression) call, array);
        }
      }

      internal abstract class AsUnicodeNonUnicodeBaseFunctionTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
      {
        private readonly bool _isUnicode;

        protected AsUnicodeNonUnicodeBaseFunctionTranslator(
          IEnumerable<MethodInfo> methods,
          bool isUnicode)
          : base(methods)
        {
          this._isUnicode = isUnicode;
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          DbExpression dbExpression = parent.TranslateExpression(call.Arguments[0]);
          TypeUsage typeUsage = dbExpression.ResultType.ShallowCopy(new FacetValues()
          {
            Unicode = (FacetValueContainer<bool?>) new bool?(this._isUnicode)
          });
          switch (dbExpression.ExpressionKind)
          {
            case DbExpressionKind.Constant:
              return (DbExpression) typeUsage.Constant(((DbConstantExpression) dbExpression).Value);
            case DbExpressionKind.Null:
              return (DbExpression) typeUsage.Null();
            case DbExpressionKind.ParameterReference:
              return (DbExpression) typeUsage.Parameter(((DbParameterReferenceExpression) dbExpression).ParameterName);
            default:
              throw new NotSupportedException(Strings.ELinq_UnsupportedAsUnicodeAndAsNonUnicode((object) call.Method));
          }
        }
      }

      internal sealed class AsUnicodeFunctionTranslator : ExpressionConverter.MethodCallTranslator.AsUnicodeNonUnicodeBaseFunctionTranslator
      {
        internal AsUnicodeFunctionTranslator()
          : base(ExpressionConverter.MethodCallTranslator.AsUnicodeFunctionTranslator.GetMethods(), true)
        {
        }

        private static IEnumerable<MethodInfo> GetMethods()
        {
          yield return typeof (DbFunctions).GetDeclaredMethod("AsUnicode", typeof (string));
          yield return typeof (EntityFunctions).GetDeclaredMethod("AsUnicode", typeof (string));
        }
      }

      internal sealed class AsNonUnicodeFunctionTranslator : ExpressionConverter.MethodCallTranslator.AsUnicodeNonUnicodeBaseFunctionTranslator
      {
        internal AsNonUnicodeFunctionTranslator()
          : base(ExpressionConverter.MethodCallTranslator.AsNonUnicodeFunctionTranslator.GetMethods(), false)
        {
        }

        private static IEnumerable<MethodInfo> GetMethods()
        {
          yield return typeof (DbFunctions).GetDeclaredMethod("AsNonUnicode", typeof (string));
          yield return typeof (EntityFunctions).GetDeclaredMethod("AsNonUnicode", typeof (string));
        }
      }

      internal sealed class HasFlagTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
      {
        private static readonly MethodInfo _hasFlagMethod = typeof (Enum).GetDeclaredMethod("HasFlag", typeof (Enum));

        internal HasFlagTranslator()
          : base(ExpressionConverter.MethodCallTranslator.HasFlagTranslator._hasFlagMethod)
        {
        }

        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "The argument name passed to ArgumentNullException matches the name of the argument of the HasFlag method being translated.", Scope = "member")]
        private static DbExpression TranslateHasFlag(
          ExpressionConverter parent,
          Expression sourceExpression,
          Expression valueExpression)
        {
          if (valueExpression.NodeType == ExpressionType.Constant && ((ConstantExpression) valueExpression).Value == null)
            throw new ArgumentNullException("flag");
          DbExpression dbExpression1 = parent.TranslateExpression(valueExpression);
          DbExpression dbExpression2 = parent.TranslateExpression(sourceExpression);
          if (dbExpression2.ResultType.EdmType != dbExpression1.ResultType.EdmType)
            throw new NotSupportedException(Strings.ELinq_HasFlagArgumentAndSourceTypeMismatch((object) dbExpression1.ResultType.EdmType.Name, (object) dbExpression2.ResultType.EdmType.Name));
          TypeUsage underlyingTypeUsage = TypeHelpers.CreateEnumUnderlyingTypeUsage(dbExpression2.ResultType);
          DbCastExpression dbCastExpression = dbExpression1.CastTo(underlyingTypeUsage);
          return (DbExpression) dbExpression2.CastTo(underlyingTypeUsage).BitwiseAnd((DbExpression) dbCastExpression).Equal((DbExpression) dbCastExpression);
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          return ExpressionConverter.MethodCallTranslator.HasFlagTranslator.TranslateHasFlag(parent, call.Object, call.Arguments[0]);
        }
      }

      internal sealed class MathTruncateTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
      {
        internal MathTruncateTranslator()
          : base(typeof (Math).GetDeclaredMethod("Truncate", typeof (Decimal)), typeof (Math).GetDeclaredMethod("Truncate", typeof (double)))
        {
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          return (DbExpression) parent.TranslateExpression(call.Arguments[0]).Truncate((DbExpression) DbExpressionBuilder.Constant((object) 0));
        }
      }

      internal sealed class MathPowerTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
      {
        internal MathPowerTranslator()
          : base(typeof (Math).GetDeclaredMethod("Pow", typeof (double), typeof (double)))
        {
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          return (DbExpression) parent.TranslateExpression(call.Arguments[0]).Power(parent.TranslateExpression(call.Arguments[1]));
        }
      }

      internal sealed class GuidNewGuidTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
      {
        internal GuidNewGuidTranslator()
          : base(typeof (Guid).GetDeclaredMethod("NewGuid"))
        {
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          return (DbExpression) EdmFunctions.NewGuid();
        }
      }

      internal sealed class StringContainsTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
      {
        internal StringContainsTranslator()
          : base(ExpressionConverter.MethodCallTranslator.StringContainsTranslator.GetMethods())
        {
        }

        private static IEnumerable<MethodInfo> GetMethods()
        {
          yield return typeof (string).GetDeclaredMethod("Contains", typeof (string));
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          return parent.TranslateFunctionIntoLike(call, true, true, new Func<ExpressionConverter, MethodCallExpression, DbExpression, DbExpression, DbExpression>(ExpressionConverter.MethodCallTranslator.StringContainsTranslator.CreateDefaultTranslation));
        }

        private static DbExpression CreateDefaultTranslation(
          ExpressionConverter parent,
          MethodCallExpression call,
          DbExpression patternExpression,
          DbExpression inputExpression)
        {
          return (DbExpression) parent.CreateCanonicalFunction("IndexOf", (Expression) call, patternExpression, inputExpression).GreaterThan((DbExpression) DbExpressionBuilder.Constant((object) 0));
        }
      }

      internal sealed class IndexOfTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
      {
        internal IndexOfTranslator()
          : base(ExpressionConverter.MethodCallTranslator.IndexOfTranslator.GetMethods())
        {
        }

        private static IEnumerable<MethodInfo> GetMethods()
        {
          yield return typeof (string).GetDeclaredMethod("IndexOf", typeof (string));
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          return (DbExpression) parent.TranslateIntoCanonicalFunction("IndexOf", (Expression) call, call.Arguments[0], call.Object).Minus((DbExpression) DbExpressionBuilder.Constant((object) 1));
        }
      }

      internal sealed class StartsWithTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
      {
        internal StartsWithTranslator()
          : base(ExpressionConverter.MethodCallTranslator.StartsWithTranslator.GetMethods())
        {
        }

        private static IEnumerable<MethodInfo> GetMethods()
        {
          yield return typeof (string).GetDeclaredMethod("StartsWith", typeof (string));
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          return parent.TranslateFunctionIntoLike(call, false, true, new Func<ExpressionConverter, MethodCallExpression, DbExpression, DbExpression, DbExpression>(ExpressionConverter.MethodCallTranslator.StartsWithTranslator.CreateDefaultTranslation));
        }

        private static DbExpression CreateDefaultTranslation(
          ExpressionConverter parent,
          MethodCallExpression call,
          DbExpression patternExpression,
          DbExpression inputExpression)
        {
          return (DbExpression) parent.CreateCanonicalFunction("IndexOf", (Expression) call, patternExpression, inputExpression).Equal((DbExpression) DbExpressionBuilder.Constant((object) 1));
        }
      }

      internal sealed class EndsWithTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
      {
        internal EndsWithTranslator()
          : base(ExpressionConverter.MethodCallTranslator.EndsWithTranslator.GetMethods())
        {
        }

        private static IEnumerable<MethodInfo> GetMethods()
        {
          yield return typeof (string).GetDeclaredMethod("EndsWith", typeof (string));
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          return parent.TranslateFunctionIntoLike(call, true, false, new Func<ExpressionConverter, MethodCallExpression, DbExpression, DbExpression, DbExpression>(ExpressionConverter.MethodCallTranslator.EndsWithTranslator.CreateDefaultTranslation));
        }

        private static DbExpression CreateDefaultTranslation(
          ExpressionConverter parent,
          MethodCallExpression call,
          DbExpression patternExpression,
          DbExpression inputExpression)
        {
          DbFunctionExpression canonicalFunction1 = parent.CreateCanonicalFunction("Reverse", (Expression) call, patternExpression);
          DbFunctionExpression canonicalFunction2 = parent.CreateCanonicalFunction("Reverse", (Expression) call, inputExpression);
          return (DbExpression) parent.CreateCanonicalFunction("IndexOf", (Expression) call, (DbExpression) canonicalFunction1, (DbExpression) canonicalFunction2).Equal((DbExpression) DbExpressionBuilder.Constant((object) 1));
        }
      }

      internal sealed class SubstringTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
      {
        internal SubstringTranslator()
          : base(ExpressionConverter.MethodCallTranslator.SubstringTranslator.GetMethods())
        {
        }

        private static IEnumerable<MethodInfo> GetMethods()
        {
          yield return typeof (string).GetDeclaredMethod("Substring", typeof (int));
          yield return typeof (string).GetDeclaredMethod("Substring", typeof (int), typeof (int));
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          DbExpression dbExpression1 = parent.TranslateExpression(call.Arguments[0]);
          DbExpression dbExpression2 = parent.TranslateExpression(call.Object);
          DbExpression dbExpression3 = (DbExpression) dbExpression1.Plus((DbExpression) DbExpressionBuilder.Constant((object) 1));
          DbExpression dbExpression4;
          if (call.Arguments.Count == 1)
            dbExpression4 = (DbExpression) parent.CreateCanonicalFunction("Length", (Expression) call, dbExpression2).Minus(dbExpression1);
          else
            dbExpression4 = parent.TranslateExpression(call.Arguments[1]);
          return (DbExpression) parent.CreateCanonicalFunction("Substring", (Expression) call, dbExpression2, dbExpression3, dbExpression4);
        }
      }

      internal sealed class RemoveTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
      {
        internal RemoveTranslator()
          : base(ExpressionConverter.MethodCallTranslator.RemoveTranslator.GetMethods())
        {
        }

        private static IEnumerable<MethodInfo> GetMethods()
        {
          yield return typeof (string).GetDeclaredMethod("Remove", typeof (int));
          yield return typeof (string).GetDeclaredMethod("Remove", typeof (int), typeof (int));
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          DbExpression dbExpression1 = parent.TranslateExpression(call.Object);
          DbExpression left = parent.TranslateExpression(call.Arguments[0]);
          DbExpression canonicalFunction1 = (DbExpression) parent.CreateCanonicalFunction("Substring", (Expression) call, dbExpression1, (DbExpression) DbExpressionBuilder.Constant((object) 1), left);
          if (call.Arguments.Count == 2)
          {
            DbExpression right = parent.TranslateExpression(call.Arguments[1]);
            if (!ExpressionConverter.MethodCallTranslator.RemoveTranslator.IsNonNegativeIntegerConstant(right))
              throw new NotSupportedException(Strings.ELinq_UnsupportedStringRemoveCase((object) call.Method, (object) call.Method.GetParameters()[1].Name));
            DbExpression dbExpression2 = (DbExpression) left.Plus(right).Plus((DbExpression) DbExpressionBuilder.Constant((object) 1));
            DbExpression dbExpression3 = (DbExpression) parent.CreateCanonicalFunction("Length", (Expression) call, dbExpression1).Minus((DbExpression) left.Plus(right));
            DbExpression canonicalFunction2 = (DbExpression) parent.CreateCanonicalFunction("Substring", (Expression) call, dbExpression1, dbExpression2, dbExpression3);
            canonicalFunction1 = (DbExpression) parent.CreateCanonicalFunction("Concat", (Expression) call, canonicalFunction1, canonicalFunction2);
          }
          return canonicalFunction1;
        }

        private static bool IsNonNegativeIntegerConstant(DbExpression argument)
        {
          return argument.ExpressionKind == DbExpressionKind.Constant && TypeSemantics.IsPrimitiveType(argument.ResultType, PrimitiveTypeKind.Int32) && (int) ((DbConstantExpression) argument).Value >= 0;
        }
      }

      internal sealed class InsertTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
      {
        internal InsertTranslator()
          : base(ExpressionConverter.MethodCallTranslator.InsertTranslator.GetMethods())
        {
        }

        private static IEnumerable<MethodInfo> GetMethods()
        {
          yield return typeof (string).GetDeclaredMethod("Insert", typeof (int), typeof (string));
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          DbExpression dbExpression1 = parent.TranslateExpression(call.Object);
          DbExpression dbExpression2 = parent.TranslateExpression(call.Arguments[0]);
          DbExpression canonicalFunction1 = (DbExpression) parent.CreateCanonicalFunction("Substring", (Expression) call, dbExpression1, (DbExpression) DbExpressionBuilder.Constant((object) 1), dbExpression2);
          DbExpression canonicalFunction2 = (DbExpression) parent.CreateCanonicalFunction("Substring", (Expression) call, dbExpression1, (DbExpression) dbExpression2.Plus((DbExpression) DbExpressionBuilder.Constant((object) 1)), (DbExpression) parent.CreateCanonicalFunction("Length", (Expression) call, dbExpression1).Minus(dbExpression2));
          DbExpression dbExpression3 = parent.TranslateExpression(call.Arguments[1]);
          return (DbExpression) parent.CreateCanonicalFunction("Concat", (Expression) call, (DbExpression) parent.CreateCanonicalFunction("Concat", (Expression) call, canonicalFunction1, dbExpression3), canonicalFunction2);
        }
      }

      internal sealed class IsNullOrEmptyTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
      {
        internal IsNullOrEmptyTranslator()
          : base(ExpressionConverter.MethodCallTranslator.IsNullOrEmptyTranslator.GetMethods())
        {
        }

        private static IEnumerable<MethodInfo> GetMethods()
        {
          yield return typeof (string).GetDeclaredMethod("IsNullOrEmpty", typeof (string));
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          DbExpression dbExpression = parent.TranslateExpression(call.Arguments[0]);
          return (DbExpression) dbExpression.IsNull().Or((DbExpression) parent.CreateCanonicalFunction("Length", (Expression) call, dbExpression).Equal((DbExpression) DbExpressionBuilder.Constant((object) 0)));
        }
      }

      internal sealed class StringConcatTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
      {
        internal StringConcatTranslator()
          : base(ExpressionConverter.MethodCallTranslator.StringConcatTranslator.GetMethods())
        {
        }

        private static IEnumerable<MethodInfo> GetMethods()
        {
          yield return typeof (string).GetDeclaredMethod("Concat", typeof (string), typeof (string));
          yield return typeof (string).GetDeclaredMethod("Concat", typeof (string), typeof (string), typeof (string));
          yield return typeof (string).GetDeclaredMethod("Concat", typeof (string), typeof (string), typeof (string), typeof (string));
          yield return typeof (string).GetDeclaredMethod("Concat", typeof (object), typeof (object));
          yield return typeof (string).GetDeclaredMethod("Concat", typeof (object), typeof (object), typeof (object));
          yield return typeof (string).GetDeclaredMethod("Concat", typeof (object), typeof (object), typeof (object), typeof (object));
          yield return typeof (string).GetDeclaredMethod("Concat", typeof (object[]));
          yield return typeof (string).GetDeclaredMethod("Concat", typeof (string[]));
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          Expression[] array;
          if (call.Arguments.Count == 1 && (call.Arguments.First<Expression>().Type == typeof (object[]) || call.Arguments.First<Expression>().Type == typeof (string[])))
          {
            if (call.Arguments[0] is NewArrayExpression)
            {
              array = ((NewArrayExpression) call.Arguments[0]).Expressions.ToArray<Expression>();
            }
            else
            {
              ConstantExpression constantExpression = (ConstantExpression) call.Arguments[0];
              if (constantExpression.Value == null)
                throw new ArgumentNullException(constantExpression.Type == typeof (object[]) ? "args" : "values");
              array = (Expression[]) ((IEnumerable<object>) (object[]) constantExpression.Value).Select<object, ConstantExpression>((Func<object, ConstantExpression>) (v => Expression.Constant(v))).ToArray<ConstantExpression>();
            }
          }
          else
            array = call.Arguments.ToArray<Expression>();
          return ExpressionConverter.StringTranslatorUtil.ConcatArgs(parent, (Expression) call, array);
        }
      }

      internal sealed class ToStringTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
      {
        private static readonly MethodInfo[] _methods = new MethodInfo[15]
        {
          typeof (string).GetDeclaredMethod("ToString"),
          typeof (byte).GetDeclaredMethod("ToString"),
          typeof (sbyte).GetDeclaredMethod("ToString"),
          typeof (short).GetDeclaredMethod("ToString"),
          typeof (int).GetDeclaredMethod("ToString"),
          typeof (long).GetDeclaredMethod("ToString"),
          typeof (double).GetDeclaredMethod("ToString"),
          typeof (float).GetDeclaredMethod("ToString"),
          typeof (Guid).GetDeclaredMethod("ToString"),
          typeof (DateTime).GetDeclaredMethod("ToString"),
          typeof (DateTimeOffset).GetDeclaredMethod("ToString"),
          typeof (TimeSpan).GetDeclaredMethod("ToString"),
          typeof (Decimal).GetDeclaredMethod("ToString"),
          typeof (bool).GetDeclaredMethod("ToString"),
          typeof (object).GetDeclaredMethod("ToString")
        };

        internal ToStringTranslator()
          : base(ExpressionConverter.MethodCallTranslator.ToStringTranslator._methods)
        {
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          return ExpressionConverter.StringTranslatorUtil.ConvertToString(parent, call.Object);
        }
      }

      internal abstract class TrimBaseTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
      {
        private readonly string _canonicalFunctionName;

        protected TrimBaseTranslator(IEnumerable<MethodInfo> methods, string canonicalFunctionName)
          : base(methods)
        {
          this._canonicalFunctionName = canonicalFunctionName;
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          if (!ExpressionConverter.MethodCallTranslator.TrimBaseTranslator.IsEmptyArray(call.Arguments[0]))
            throw new NotSupportedException(Strings.ELinq_UnsupportedTrimStartTrimEndCase((object) call.Method));
          return (DbExpression) parent.TranslateIntoCanonicalFunction(this._canonicalFunctionName, (Expression) call, call.Object);
        }

        internal static bool IsEmptyArray(Expression expression)
        {
          NewArrayExpression newArrayExpression = (NewArrayExpression) expression;
          if (expression.NodeType == ExpressionType.NewArrayInit)
          {
            if (newArrayExpression.Expressions.Count == 0)
              return true;
          }
          else if (expression.NodeType == ExpressionType.NewArrayBounds && newArrayExpression.Expressions.Count == 1 && newArrayExpression.Expressions[0].NodeType == ExpressionType.Constant)
            return object.Equals(((ConstantExpression) newArrayExpression.Expressions[0]).Value, (object) 0);
          return false;
        }
      }

      internal sealed class TrimTranslator : ExpressionConverter.MethodCallTranslator.TrimBaseTranslator
      {
        internal TrimTranslator()
          : base(ExpressionConverter.MethodCallTranslator.TrimTranslator.GetMethods(), "Trim")
        {
        }

        private static IEnumerable<MethodInfo> GetMethods()
        {
          yield return typeof (string).GetDeclaredMethod("Trim", typeof (char[]));
        }
      }

      internal sealed class TrimStartTranslator : ExpressionConverter.MethodCallTranslator.TrimBaseTranslator
      {
        internal TrimStartTranslator()
          : base(ExpressionConverter.MethodCallTranslator.TrimStartTranslator.GetMethods(), "LTrim")
        {
        }

        private static IEnumerable<MethodInfo> GetMethods()
        {
          yield return typeof (string).GetDeclaredMethod("TrimStart", typeof (char[]));
        }
      }

      internal sealed class TrimEndTranslator : ExpressionConverter.MethodCallTranslator.TrimBaseTranslator
      {
        internal TrimEndTranslator()
          : base(ExpressionConverter.MethodCallTranslator.TrimEndTranslator.GetMethods(), "RTrim")
        {
        }

        private static IEnumerable<MethodInfo> GetMethods()
        {
          yield return typeof (string).GetDeclaredMethod("TrimEnd", typeof (char[]));
        }
      }

      internal sealed class VBCanonicalFunctionDefaultTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
      {
        private const string s_stringsTypeFullName = "Microsoft.VisualBasic.Strings";
        private const string s_dateAndTimeTypeFullName = "Microsoft.VisualBasic.DateAndTime";

        internal VBCanonicalFunctionDefaultTranslator(Assembly vbAssembly)
          : base(ExpressionConverter.MethodCallTranslator.VBCanonicalFunctionDefaultTranslator.GetMethods(vbAssembly))
        {
        }

        private static IEnumerable<MethodInfo> GetMethods(Assembly vbAssembly)
        {
          Type stringsType = vbAssembly.GetType("Microsoft.VisualBasic.Strings");
          yield return stringsType.GetDeclaredMethod("Trim", typeof (string));
          yield return stringsType.GetDeclaredMethod("LTrim", typeof (string));
          yield return stringsType.GetDeclaredMethod("RTrim", typeof (string));
          yield return stringsType.GetDeclaredMethod("Left", typeof (string), typeof (int));
          yield return stringsType.GetDeclaredMethod("Right", typeof (string), typeof (int));
          Type dateTimeType = vbAssembly.GetType("Microsoft.VisualBasic.DateAndTime");
          yield return dateTimeType.GetDeclaredMethod("Year", typeof (DateTime));
          yield return dateTimeType.GetDeclaredMethod("Month", typeof (DateTime));
          yield return dateTimeType.GetDeclaredMethod("Day", typeof (DateTime));
          yield return dateTimeType.GetDeclaredMethod("Hour", typeof (DateTime));
          yield return dateTimeType.GetDeclaredMethod("Minute", typeof (DateTime));
          yield return dateTimeType.GetDeclaredMethod("Second", typeof (DateTime));
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          return (DbExpression) parent.TranslateIntoCanonicalFunction(call.Method.Name, (Expression) call, call.Arguments.ToArray<Expression>());
        }
      }

      internal sealed class VBCanonicalFunctionRenameTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
      {
        private static readonly Dictionary<MethodInfo, string> s_methodNameMap = new Dictionary<MethodInfo, string>(4);
        private const string s_stringsTypeFullName = "Microsoft.VisualBasic.Strings";

        internal VBCanonicalFunctionRenameTranslator(Assembly vbAssembly)
          : base(ExpressionConverter.MethodCallTranslator.VBCanonicalFunctionRenameTranslator.GetMethods(vbAssembly).ToArray<MethodInfo>())
        {
        }

        private static IEnumerable<MethodInfo> GetMethods(Assembly vbAssembly)
        {
          Type stringsType = vbAssembly.GetType("Microsoft.VisualBasic.Strings");
          yield return ExpressionConverter.MethodCallTranslator.VBCanonicalFunctionRenameTranslator.GetMethodInfo(stringsType, "Len", "Length", new Type[1]
          {
            typeof (string)
          });
          yield return ExpressionConverter.MethodCallTranslator.VBCanonicalFunctionRenameTranslator.GetMethodInfo(stringsType, "Mid", "Substring", new Type[3]
          {
            typeof (string),
            typeof (int),
            typeof (int)
          });
          yield return ExpressionConverter.MethodCallTranslator.VBCanonicalFunctionRenameTranslator.GetMethodInfo(stringsType, "UCase", "ToUpper", new Type[1]
          {
            typeof (string)
          });
          yield return ExpressionConverter.MethodCallTranslator.VBCanonicalFunctionRenameTranslator.GetMethodInfo(stringsType, "LCase", "ToLower", new Type[1]
          {
            typeof (string)
          });
        }

        private static MethodInfo GetMethodInfo(
          Type declaringType,
          string methodName,
          string canonicalFunctionName,
          Type[] argumentTypes)
        {
          MethodInfo declaredMethod = declaringType.GetDeclaredMethod(methodName, argumentTypes);
          ExpressionConverter.MethodCallTranslator.VBCanonicalFunctionRenameTranslator.s_methodNameMap.Add(declaredMethod, canonicalFunctionName);
          return declaredMethod;
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          return (DbExpression) parent.TranslateIntoCanonicalFunction(ExpressionConverter.MethodCallTranslator.VBCanonicalFunctionRenameTranslator.s_methodNameMap[call.Method], (Expression) call, call.Arguments.ToArray<Expression>());
        }
      }

      internal sealed class VBDatePartTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
      {
        private static readonly HashSet<string> _supportedIntervals = new HashSet<string>()
        {
          "Year",
          "Month",
          "Day",
          "Hour",
          "Minute",
          "Second"
        };
        private const string s_dateAndTimeTypeFullName = "Microsoft.VisualBasic.DateAndTime";
        private const string s_DateIntervalFullName = "Microsoft.VisualBasic.DateInterval";
        private const string s_FirstDayOfWeekFullName = "Microsoft.VisualBasic.FirstDayOfWeek";
        private const string s_FirstWeekOfYearFullName = "Microsoft.VisualBasic.FirstWeekOfYear";

        internal VBDatePartTranslator(Assembly vbAssembly)
          : base(ExpressionConverter.MethodCallTranslator.VBDatePartTranslator.GetMethods(vbAssembly))
        {
        }

        private static IEnumerable<MethodInfo> GetMethods(Assembly vbAssembly)
        {
          Type dateAndTimeType = vbAssembly.GetType("Microsoft.VisualBasic.DateAndTime");
          Type dateIntervalEnum = vbAssembly.GetType("Microsoft.VisualBasic.DateInterval");
          Type firstDayOfWeekEnum = vbAssembly.GetType("Microsoft.VisualBasic.FirstDayOfWeek");
          Type firstWeekOfYearEnum = vbAssembly.GetType("Microsoft.VisualBasic.FirstWeekOfYear");
          yield return dateAndTimeType.GetDeclaredMethod("DatePart", dateIntervalEnum, typeof (DateTime), firstDayOfWeekEnum, firstWeekOfYearEnum);
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          ConstantExpression constantExpression = call.Arguments[0] as ConstantExpression;
          if (constantExpression == null)
            throw new NotSupportedException(Strings.ELinq_UnsupportedVBDatePartNonConstantInterval((object) call.Method, (object) call.Method.GetParameters()[0].Name));
          string functionName = constantExpression.Value.ToString();
          if (!ExpressionConverter.MethodCallTranslator.VBDatePartTranslator._supportedIntervals.Contains(functionName))
            throw new NotSupportedException(Strings.ELinq_UnsupportedVBDatePartInvalidInterval((object) call.Method, (object) call.Method.GetParameters()[0].Name, (object) functionName));
          return (DbExpression) parent.TranslateIntoCanonicalFunction(functionName, (Expression) call, call.Arguments[1]);
        }
      }

      private abstract class SequenceMethodTranslator
      {
        private readonly IEnumerable<SequenceMethod> _methods;

        protected SequenceMethodTranslator(params SequenceMethod[] methods)
        {
          this._methods = (IEnumerable<SequenceMethod>) methods;
        }

        internal IEnumerable<SequenceMethod> Methods
        {
          get
          {
            return this._methods;
          }
        }

        internal virtual DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call,
          SequenceMethod sequenceMethod)
        {
          return this.Translate(parent, call);
        }

        internal abstract DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call);

        public override string ToString()
        {
          return this.GetType().Name;
        }
      }

      private abstract class UnarySequenceMethodTranslator : ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator
      {
        protected UnarySequenceMethodTranslator(params SequenceMethod[] methods)
          : base(methods)
        {
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          if (call.Object != null)
          {
            DbExpression operand = parent.TranslateSet(call.Object);
            return this.TranslateUnary(parent, operand, call);
          }
          DbExpression operand1 = parent.TranslateSet(call.Arguments[0]);
          return this.TranslateUnary(parent, operand1, call);
        }

        protected abstract DbExpression TranslateUnary(
          ExpressionConverter parent,
          DbExpression operand,
          MethodCallExpression call);
      }

      private abstract class PagingTranslator : ExpressionConverter.MethodCallTranslator.UnarySequenceMethodTranslator
      {
        protected PagingTranslator(params SequenceMethod[] methods)
          : base(methods)
        {
        }

        protected override DbExpression TranslateUnary(
          ExpressionConverter parent,
          DbExpression operand,
          MethodCallExpression call)
        {
          Expression linq = call.Arguments[1];
          DbExpression count = parent.TranslateExpression(linq);
          return this.TranslatePagingOperator(parent, operand, count);
        }

        protected abstract DbExpression TranslatePagingOperator(
          ExpressionConverter parent,
          DbExpression operand,
          DbExpression count);
      }

      private sealed class TakeTranslator : ExpressionConverter.MethodCallTranslator.PagingTranslator
      {
        internal TakeTranslator()
          : base(SequenceMethod.Take)
        {
        }

        protected override DbExpression TranslatePagingOperator(
          ExpressionConverter parent,
          DbExpression operand,
          DbExpression count)
        {
          DbConstantExpression constantExpression = count as DbConstantExpression;
          if (constantExpression != null && constantExpression.Value.Equals((object) 0))
            return parent.Filter(operand.BindAs(parent.AliasGenerator.Next()), (DbExpression) DbExpressionBuilder.False);
          return parent.Limit(operand, count);
        }
      }

      private sealed class SkipTranslator : ExpressionConverter.MethodCallTranslator.PagingTranslator
      {
        internal SkipTranslator()
          : base(SequenceMethod.Skip)
        {
        }

        protected override DbExpression TranslatePagingOperator(
          ExpressionConverter parent,
          DbExpression operand,
          DbExpression count)
        {
          return parent.Skip(operand.BindAs(parent.AliasGenerator.Next()), count);
        }
      }

      private sealed class JoinTranslator : ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator
      {
        internal JoinTranslator()
          : base(SequenceMethod.Join)
        {
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          DbExpression input1 = parent.TranslateSet(call.Arguments[0]);
          DbExpression input2 = parent.TranslateSet(call.Arguments[1]);
          LambdaExpression lambdaExpression1 = parent.GetLambdaExpression(call, 2);
          LambdaExpression lambdaExpression2 = parent.GetLambdaExpression(call, 3);
          LambdaExpression lambdaExpression3 = parent.GetLambdaExpression(call, 4);
          string leftName;
          string rightName;
          InitializerMetadata initializerMetadata;
          bool flag = ExpressionConverter.MethodCallTranslator.IsTrivialRename(lambdaExpression3, parent, out leftName, out rightName, out initializerMetadata);
          DbExpressionBinding binding1;
          DbExpression left = flag ? parent.TranslateLambda(lambdaExpression1, input1, leftName, out binding1) : parent.TranslateLambda(lambdaExpression1, input1, out binding1);
          DbExpressionBinding binding2;
          DbExpression right = flag ? parent.TranslateLambda(lambdaExpression2, input2, rightName, out binding2) : parent.TranslateLambda(lambdaExpression2, input2, out binding2);
          if (!TypeSemantics.IsEqualComparable(left.ResultType) || !TypeSemantics.IsEqualComparable(right.ResultType))
            throw new NotSupportedException(Strings.ELinq_UnsupportedKeySelector((object) call.Method.Name));
          DbExpression equalsExpression = parent.CreateEqualsExpression(left, right, ExpressionConverter.EqualsPattern.PositiveNullEqualityNonComposable, lambdaExpression1.Body.Type, lambdaExpression2.Body.Type);
          if (flag)
            return (DbExpression) new DbJoinExpression(DbExpressionKind.InnerJoin, TypeUsage.Create((EdmType) TypeHelpers.CreateCollectionType(TypeUsage.Create((EdmType) TypeHelpers.CreateRowType((IEnumerable<KeyValuePair<string, TypeUsage>>) new List<KeyValuePair<string, TypeUsage>>()
            {
              new KeyValuePair<string, TypeUsage>(binding1.VariableName, binding1.VariableType),
              new KeyValuePair<string, TypeUsage>(binding2.VariableName, binding2.VariableType)
            }, initializerMetadata)))), binding1, binding2, equalsExpression);
          DbExpressionBinding input3 = binding1.InnerJoin(binding2, equalsExpression).BindAs(parent.AliasGenerator.Next());
          DbPropertyExpression propertyExpression1 = input3.Variable.Property(binding1.VariableName);
          DbPropertyExpression propertyExpression2 = input3.Variable.Property(binding2.VariableName);
          parent._bindingContext.PushBindingScope(new Binding((Expression) lambdaExpression3.Parameters[0], (DbExpression) propertyExpression1));
          parent._bindingContext.PushBindingScope(new Binding((Expression) lambdaExpression3.Parameters[1], (DbExpression) propertyExpression2));
          DbExpression projection = parent.TranslateExpression(lambdaExpression3.Body);
          parent._bindingContext.PopBindingScope();
          parent._bindingContext.PopBindingScope();
          return (DbExpression) input3.Project(projection);
        }
      }

      private abstract class BinarySequenceMethodTranslator : ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator
      {
        protected BinarySequenceMethodTranslator(params SequenceMethod[] methods)
          : base(methods)
        {
        }

        private static DbExpression TranslateLeft(
          ExpressionConverter parent,
          Expression expr)
        {
          return parent.TranslateSet(expr);
        }

        protected virtual DbExpression TranslateRight(
          ExpressionConverter parent,
          Expression expr)
        {
          return parent.TranslateSet(expr);
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          if (call.Object != null)
          {
            DbExpression left = ExpressionConverter.MethodCallTranslator.BinarySequenceMethodTranslator.TranslateLeft(parent, call.Object);
            DbExpression right = this.TranslateRight(parent, call.Arguments[0]);
            return this.TranslateBinary(parent, left, right);
          }
          DbExpression left1 = ExpressionConverter.MethodCallTranslator.BinarySequenceMethodTranslator.TranslateLeft(parent, call.Arguments[0]);
          DbExpression right1 = this.TranslateRight(parent, call.Arguments[1]);
          return this.TranslateBinary(parent, left1, right1);
        }

        protected abstract DbExpression TranslateBinary(
          ExpressionConverter parent,
          DbExpression left,
          DbExpression right);
      }

      private class ConcatTranslator : ExpressionConverter.MethodCallTranslator.BinarySequenceMethodTranslator
      {
        internal ConcatTranslator()
          : base(SequenceMethod.Concat)
        {
        }

        protected override DbExpression TranslateBinary(
          ExpressionConverter parent,
          DbExpression left,
          DbExpression right)
        {
          return (DbExpression) parent.UnionAll(left, right);
        }
      }

      private sealed class UnionTranslator : ExpressionConverter.MethodCallTranslator.BinarySequenceMethodTranslator
      {
        internal UnionTranslator()
          : base(SequenceMethod.Union)
        {
        }

        protected override DbExpression TranslateBinary(
          ExpressionConverter parent,
          DbExpression left,
          DbExpression right)
        {
          return (DbExpression) parent.Distinct((DbExpression) parent.UnionAll(left, right));
        }
      }

      private sealed class IntersectTranslator : ExpressionConverter.MethodCallTranslator.BinarySequenceMethodTranslator
      {
        internal IntersectTranslator()
          : base(SequenceMethod.Intersect)
        {
        }

        protected override DbExpression TranslateBinary(
          ExpressionConverter parent,
          DbExpression left,
          DbExpression right)
        {
          return (DbExpression) parent.Intersect(left, right);
        }
      }

      private sealed class ExceptTranslator : ExpressionConverter.MethodCallTranslator.BinarySequenceMethodTranslator
      {
        internal ExceptTranslator()
          : base(SequenceMethod.Except)
        {
        }

        protected override DbExpression TranslateBinary(
          ExpressionConverter parent,
          DbExpression left,
          DbExpression right)
        {
          return (DbExpression) parent.Except(left, right);
        }

        protected override DbExpression TranslateRight(
          ExpressionConverter parent,
          Expression expr)
        {
          ++parent.IgnoreInclude;
          DbExpression dbExpression = base.TranslateRight(parent, expr);
          --parent.IgnoreInclude;
          return dbExpression;
        }
      }

      private abstract class AggregateTranslator : ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator
      {
        private readonly string _functionName;
        private readonly bool _takesPredicate;

        protected AggregateTranslator(
          string functionName,
          bool takesPredicate,
          params SequenceMethod[] methods)
          : base(methods)
        {
          this._takesPredicate = takesPredicate;
          this._functionName = functionName;
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          bool flag = 1 == call.Arguments.Count;
          DbExpression dbExpression1 = parent.TranslateSet(call.Arguments[0]);
          if (!flag)
          {
            LambdaExpression lambdaExpression = parent.GetLambdaExpression(call, 1);
            DbExpressionBinding binding;
            DbExpression dbExpression2 = parent.TranslateLambda(lambdaExpression, dbExpression1, out binding);
            dbExpression1 = !this._takesPredicate ? (DbExpression) binding.Project(dbExpression2) : parent.Filter(binding, dbExpression2);
          }
          TypeUsage returnType = this.GetReturnType(parent, call);
          DbExpression cqt = (DbExpression) this.FindFunction(parent, call, returnType).Invoke((IEnumerable<DbExpression>) new List<DbExpression>(1)
          {
            this.WrapCollectionOperand(parent, dbExpression1, returnType)
          });
          return parent.AlignTypes(cqt, call.Type);
        }

        protected virtual TypeUsage GetReturnType(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          return parent.GetValueLayerType(call.Type);
        }

        protected virtual DbExpression WrapCollectionOperand(
          ExpressionConverter parent,
          DbExpression operand,
          TypeUsage returnType)
        {
          if (!ExpressionConverter.TypeUsageEquals(returnType, ((CollectionType) operand.ResultType.EdmType).TypeUsage))
          {
            DbExpressionBinding input = operand.BindAs(parent.AliasGenerator.Next());
            operand = (DbExpression) input.Project((DbExpression) input.Variable.CastTo(returnType));
          }
          return operand;
        }

        protected virtual DbExpression WrapNonCollectionOperand(
          ExpressionConverter parent,
          DbExpression operand,
          TypeUsage returnType)
        {
          if (!ExpressionConverter.TypeUsageEquals(returnType, operand.ResultType))
            operand = (DbExpression) operand.CastTo(returnType);
          return operand;
        }

        protected virtual EdmFunction FindFunction(
          ExpressionConverter parent,
          MethodCallExpression call,
          TypeUsage argumentType)
        {
          return parent.FindCanonicalFunction(this._functionName, (IList<TypeUsage>) new List<TypeUsage>(1)
          {
            argumentType
          }, true, (Expression) call);
        }
      }

      private sealed class MaxTranslator : ExpressionConverter.MethodCallTranslator.AggregateTranslator
      {
        internal MaxTranslator()
          : base("Max", false, SequenceMethod.Max, SequenceMethod.MaxSelector, SequenceMethod.MaxInt, SequenceMethod.MaxIntSelector, SequenceMethod.MaxDecimal, SequenceMethod.MaxDecimalSelector, SequenceMethod.MaxDouble, SequenceMethod.MaxDoubleSelector, SequenceMethod.MaxLong, SequenceMethod.MaxLongSelector, SequenceMethod.MaxSingle, SequenceMethod.MaxSingleSelector, SequenceMethod.MaxNullableDecimal, SequenceMethod.MaxNullableDecimalSelector, SequenceMethod.MaxNullableDouble, SequenceMethod.MaxNullableDoubleSelector, SequenceMethod.MaxNullableInt, SequenceMethod.MaxNullableIntSelector, SequenceMethod.MaxNullableLong, SequenceMethod.MaxNullableLongSelector, SequenceMethod.MaxNullableSingle, SequenceMethod.MaxNullableSingleSelector)
        {
        }

        protected override TypeUsage GetReturnType(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          TypeUsage returnType = base.GetReturnType(parent, call);
          if (!TypeSemantics.IsEnumerationType(returnType))
            return returnType;
          return TypeUsage.Create((EdmType) Helper.GetUnderlyingEdmTypeForEnumType(returnType.EdmType), (IEnumerable<Facet>) returnType.Facets);
        }
      }

      private sealed class MinTranslator : ExpressionConverter.MethodCallTranslator.AggregateTranslator
      {
        internal MinTranslator()
          : base("Min", false, SequenceMethod.Min, SequenceMethod.MinSelector, SequenceMethod.MinDecimal, SequenceMethod.MinDecimalSelector, SequenceMethod.MinDouble, SequenceMethod.MinDoubleSelector, SequenceMethod.MinInt, SequenceMethod.MinIntSelector, SequenceMethod.MinLong, SequenceMethod.MinLongSelector, SequenceMethod.MinNullableDecimal, SequenceMethod.MinSingle, SequenceMethod.MinSingleSelector, SequenceMethod.MinNullableDecimalSelector, SequenceMethod.MinNullableDouble, SequenceMethod.MinNullableDoubleSelector, SequenceMethod.MinNullableInt, SequenceMethod.MinNullableIntSelector, SequenceMethod.MinNullableLong, SequenceMethod.MinNullableLongSelector, SequenceMethod.MinNullableSingle, SequenceMethod.MinNullableSingleSelector)
        {
        }

        protected override TypeUsage GetReturnType(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          TypeUsage returnType = base.GetReturnType(parent, call);
          if (!TypeSemantics.IsEnumerationType(returnType))
            return returnType;
          return TypeUsage.Create((EdmType) Helper.GetUnderlyingEdmTypeForEnumType(returnType.EdmType), (IEnumerable<Facet>) returnType.Facets);
        }
      }

      private sealed class AverageTranslator : ExpressionConverter.MethodCallTranslator.AggregateTranslator
      {
        internal AverageTranslator()
          : base("Avg", false, SequenceMethod.AverageDecimal, SequenceMethod.AverageDecimalSelector, SequenceMethod.AverageDouble, SequenceMethod.AverageDoubleSelector, SequenceMethod.AverageInt, SequenceMethod.AverageIntSelector, SequenceMethod.AverageLong, SequenceMethod.AverageLongSelector, SequenceMethod.AverageSingle, SequenceMethod.AverageSingleSelector, SequenceMethod.AverageNullableDecimal, SequenceMethod.AverageNullableDecimalSelector, SequenceMethod.AverageNullableDouble, SequenceMethod.AverageNullableDoubleSelector, SequenceMethod.AverageNullableInt, SequenceMethod.AverageNullableIntSelector, SequenceMethod.AverageNullableLong, SequenceMethod.AverageNullableLongSelector, SequenceMethod.AverageNullableSingle, SequenceMethod.AverageNullableSingleSelector)
        {
        }
      }

      private sealed class SumTranslator : ExpressionConverter.MethodCallTranslator.AggregateTranslator
      {
        internal SumTranslator()
          : base("Sum", false, SequenceMethod.SumDecimal, SequenceMethod.SumDecimalSelector, SequenceMethod.SumDouble, SequenceMethod.SumDoubleSelector, SequenceMethod.SumInt, SequenceMethod.SumIntSelector, SequenceMethod.SumLong, SequenceMethod.SumLongSelector, SequenceMethod.SumSingle, SequenceMethod.SumSingleSelector, SequenceMethod.SumNullableDecimal, SequenceMethod.SumNullableDecimalSelector, SequenceMethod.SumNullableDouble, SequenceMethod.SumNullableDoubleSelector, SequenceMethod.SumNullableInt, SequenceMethod.SumNullableIntSelector, SequenceMethod.SumNullableLong, SequenceMethod.SumNullableLongSelector, SequenceMethod.SumNullableSingle, SequenceMethod.SumNullableSingleSelector)
        {
        }
      }

      private abstract class CountTranslatorBase : ExpressionConverter.MethodCallTranslator.AggregateTranslator
      {
        protected CountTranslatorBase(string functionName, params SequenceMethod[] methods)
          : base(functionName, true, methods)
        {
        }

        protected override DbExpression WrapCollectionOperand(
          ExpressionConverter parent,
          DbExpression operand,
          TypeUsage returnType)
        {
          return (DbExpression) operand.BindAs(parent.AliasGenerator.Next()).Project((DbExpression) DbExpressionBuilder.Constant((object) 1));
        }

        protected override DbExpression WrapNonCollectionOperand(
          ExpressionConverter parent,
          DbExpression operand,
          TypeUsage returnType)
        {
          DbExpression dbExpression = (DbExpression) DbExpressionBuilder.Constant((object) 1);
          if (!ExpressionConverter.TypeUsageEquals(dbExpression.ResultType, returnType))
            dbExpression = (DbExpression) dbExpression.CastTo(returnType);
          return dbExpression;
        }

        protected override EdmFunction FindFunction(
          ExpressionConverter parent,
          MethodCallExpression call,
          TypeUsage argumentType)
        {
          TypeUsage defaultTypeUsage = TypeUsage.CreateDefaultTypeUsage((EdmType) EdmProviderManifest.Instance.GetPrimitiveType(PrimitiveTypeKind.Int32));
          return base.FindFunction(parent, call, defaultTypeUsage);
        }
      }

      private sealed class CountTranslator : ExpressionConverter.MethodCallTranslator.CountTranslatorBase
      {
        internal CountTranslator()
          : base("Count", SequenceMethod.Count, SequenceMethod.CountPredicate)
        {
        }
      }

      private sealed class LongCountTranslator : ExpressionConverter.MethodCallTranslator.CountTranslatorBase
      {
        internal LongCountTranslator()
          : base("BigCount", SequenceMethod.LongCount, SequenceMethod.LongCountPredicate)
        {
        }
      }

      private sealed class PassthroughTranslator : ExpressionConverter.MethodCallTranslator.UnarySequenceMethodTranslator
      {
        internal PassthroughTranslator()
          : base(SequenceMethod.AsQueryableGeneric, SequenceMethod.AsQueryable, SequenceMethod.AsEnumerable, SequenceMethod.ToList)
        {
        }

        protected override DbExpression TranslateUnary(
          ExpressionConverter parent,
          DbExpression operand,
          MethodCallExpression call)
        {
          if (TypeSemantics.IsCollectionType(operand.ResultType))
            return operand;
          throw new NotSupportedException(Strings.ELinq_UnsupportedPassthrough((object) call.Method.Name, (object) operand.ResultType.EdmType.Name));
        }
      }

      private sealed class OfTypeTranslator : ExpressionConverter.MethodCallTranslator.UnarySequenceMethodTranslator
      {
        internal OfTypeTranslator()
          : base(SequenceMethod.OfType)
        {
        }

        protected override DbExpression TranslateUnary(
          ExpressionConverter parent,
          DbExpression operand,
          MethodCallExpression call)
        {
          Type genericArgument = call.Method.GetGenericArguments()[0];
          TypeUsage type;
          if (!parent.TryGetValueLayerType(genericArgument, out type) || !TypeSemantics.IsEntityType(type) && !TypeSemantics.IsComplexType(type))
            throw new NotSupportedException(Strings.ELinq_InvalidOfTypeResult((object) ExpressionConverter.DescribeClrType(genericArgument)));
          return parent.OfType(operand, type);
        }
      }

      private sealed class DistinctTranslator : ExpressionConverter.MethodCallTranslator.UnarySequenceMethodTranslator
      {
        internal DistinctTranslator()
          : base(SequenceMethod.Distinct)
        {
        }

        protected override DbExpression TranslateUnary(
          ExpressionConverter parent,
          DbExpression operand,
          MethodCallExpression call)
        {
          return (DbExpression) parent.Distinct(operand);
        }
      }

      private sealed class AnyTranslator : ExpressionConverter.MethodCallTranslator.UnarySequenceMethodTranslator
      {
        internal AnyTranslator()
          : base(SequenceMethod.Any)
        {
        }

        protected override DbExpression TranslateUnary(
          ExpressionConverter parent,
          DbExpression operand,
          MethodCallExpression call)
        {
          return (DbExpression) operand.IsEmpty().Not();
        }
      }

      private abstract class OneLambdaTranslator : ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator
      {
        internal OneLambdaTranslator(params SequenceMethod[] methods)
          : base(methods)
        {
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          DbExpression source;
          DbExpressionBinding sourceBinding;
          DbExpression lambda;
          return this.Translate(parent, call, out source, out sourceBinding, out lambda);
        }

        protected DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call,
          out DbExpression source,
          out DbExpressionBinding sourceBinding,
          out DbExpression lambda)
        {
          source = parent.TranslateExpression(call.Arguments[0]);
          LambdaExpression lambdaExpression = parent.GetLambdaExpression(call, 1);
          lambda = parent.TranslateLambda(lambdaExpression, source, out sourceBinding);
          return this.TranslateOneLambda(parent, sourceBinding, lambda);
        }

        protected abstract DbExpression TranslateOneLambda(
          ExpressionConverter parent,
          DbExpressionBinding sourceBinding,
          DbExpression lambda);
      }

      private sealed class AnyPredicateTranslator : ExpressionConverter.MethodCallTranslator.OneLambdaTranslator
      {
        internal AnyPredicateTranslator()
          : base(SequenceMethod.AnyPredicate)
        {
        }

        protected override DbExpression TranslateOneLambda(
          ExpressionConverter parent,
          DbExpressionBinding sourceBinding,
          DbExpression lambda)
        {
          return (DbExpression) sourceBinding.Any(lambda);
        }
      }

      private sealed class AllTranslator : ExpressionConverter.MethodCallTranslator.OneLambdaTranslator
      {
        internal AllTranslator()
          : base(SequenceMethod.All)
        {
        }

        protected override DbExpression TranslateOneLambda(
          ExpressionConverter parent,
          DbExpressionBinding sourceBinding,
          DbExpression lambda)
        {
          return (DbExpression) sourceBinding.All(lambda);
        }
      }

      private sealed class WhereTranslator : ExpressionConverter.MethodCallTranslator.OneLambdaTranslator
      {
        internal WhereTranslator()
          : base(new SequenceMethod[1])
        {
        }

        protected override DbExpression TranslateOneLambda(
          ExpressionConverter parent,
          DbExpressionBinding sourceBinding,
          DbExpression lambda)
        {
          return parent.Filter(sourceBinding, lambda);
        }
      }

      private sealed class SelectTranslator : ExpressionConverter.MethodCallTranslator.OneLambdaTranslator
      {
        internal SelectTranslator()
          : base(SequenceMethod.Select)
        {
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          DbExpression source;
          DbExpressionBinding sourceBinding;
          DbExpression lambda;
          return this.Translate(parent, call, out source, out sourceBinding, out lambda);
        }

        protected override DbExpression TranslateOneLambda(
          ExpressionConverter parent,
          DbExpressionBinding sourceBinding,
          DbExpression lambda)
        {
          return parent.Project(sourceBinding, lambda);
        }
      }

      private sealed class DefaultIfEmptyTranslator : ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator
      {
        internal DefaultIfEmptyTranslator()
          : base(SequenceMethod.DefaultIfEmpty, SequenceMethod.DefaultIfEmptyValue)
        {
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          DbExpression dbExpression1 = parent.TranslateSet(call.Arguments[0]);
          DbExpression dbExpression2 = call.Arguments.Count == 2 ? parent.TranslateExpression(call.Arguments[1]) : ExpressionConverter.MethodCallTranslator.DefaultIfEmptyTranslator.GetDefaultValue(parent, call.Type);
          DbExpressionBinding left = DbExpressionBuilder.NewCollection((DbExpression) new byte?((byte) 1)).BindAs(parent.AliasGenerator.Next());
          bool flag = dbExpression2 != null && dbExpression2.ExpressionKind != DbExpressionKind.Null;
          if (flag)
          {
            DbExpressionBinding input = dbExpression1.BindAs(parent.AliasGenerator.Next());
            dbExpression1 = (DbExpression) input.Project((DbExpression) new Row((DbExpression) new byte?((byte) 1).As("sentinel"), new KeyValuePair<string, DbExpression>[1]
            {
              input.Variable.As("value")
            }));
          }
          DbExpressionBinding right = dbExpression1.BindAs(parent.AliasGenerator.Next());
          DbExpressionBinding input1 = left.LeftOuterJoin(right, (DbExpression) new bool?(true)).BindAs(parent.AliasGenerator.Next());
          DbExpression dbExpression3 = (DbExpression) input1.Variable.Property(right.VariableName);
          if (flag)
            dbExpression3 = (DbExpression) DbExpressionBuilder.Case((IEnumerable<DbExpression>) new DbIsNullExpression[1]
            {
              dbExpression3.Property("sentinel").IsNull()
            }, (IEnumerable<DbExpression>) new DbExpression[1]
            {
              dbExpression2
            }, (DbExpression) dbExpression3.Property("value"));
          DbExpression to = (DbExpression) input1.Project(dbExpression3);
          parent.ApplySpanMapping(dbExpression1, to);
          return to;
        }

        private static DbExpression GetDefaultValue(
          ExpressionConverter parent,
          Type resultType)
        {
          Type elementType = TypeSystem.GetElementType(resultType);
          object defaultValue = TypeSystem.GetDefaultValue(elementType);
          return defaultValue == null ? (DbExpression) null : parent.TranslateExpression((Expression) Expression.Constant(defaultValue, elementType));
        }
      }

      private sealed class ContainsTranslator : ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator
      {
        internal ContainsTranslator()
          : base(SequenceMethod.Contains)
        {
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          return ExpressionConverter.MethodCallTranslator.ContainsTranslator.TranslateContains(parent, call.Arguments[0], call.Arguments[1]);
        }

        private static DbExpression TranslateContainsHelper(
          ExpressionConverter parent,
          DbExpression left,
          IEnumerable<DbExpression> rightList,
          ExpressionConverter.EqualsPattern pattern,
          Type leftType,
          Type rightType)
        {
          return Helpers.BuildBalancedTreeInPlace<DbExpression>((IList<DbExpression>) new List<DbExpression>(rightList.Select<DbExpression, DbExpression>((Func<DbExpression, DbExpression>) (argument => parent.CreateEqualsExpression(left, argument, pattern, leftType, rightType)))), (Func<DbExpression, DbExpression, DbExpression>) ((prev, next) => (DbExpression) prev.Or(next)));
        }

        internal static DbExpression TranslateContains(
          ExpressionConverter parent,
          Expression sourceExpression,
          Expression valueExpression)
        {
          DbExpression input1 = parent.NormalizeSetSource(parent.TranslateExpression(sourceExpression));
          DbExpression dbExpression1 = parent.TranslateExpression(valueExpression);
          Type elementType = TypeSystem.GetElementType(sourceExpression.Type);
          if (input1.ExpressionKind == DbExpressionKind.NewInstance)
          {
            IList<DbExpression> arguments = ((DbNewInstanceExpression) input1).Arguments;
            if (arguments.Count <= 0)
              return (DbExpression) new bool?(false);
            bool comparisonBehavior = parent._funcletizer.RootContext.ContextOptions.UseCSharpNullComparisonBehavior;
            bool flag = parent.ProviderManifest.SupportsInExpression();
            if (!comparisonBehavior && !flag)
              return ExpressionConverter.MethodCallTranslator.ContainsTranslator.TranslateContainsHelper(parent, dbExpression1, (IEnumerable<DbExpression>) arguments, ExpressionConverter.EqualsPattern.Store, elementType, valueExpression.Type);
            List<DbExpression> dbExpressionList1 = new List<DbExpression>();
            List<DbExpression> dbExpressionList2 = new List<DbExpression>();
            foreach (DbExpression dbExpression2 in (IEnumerable<DbExpression>) arguments)
              (dbExpression2.ExpressionKind == DbExpressionKind.Constant ? dbExpressionList1 : dbExpressionList2).Add(dbExpression2);
            DbExpression left = (DbExpression) null;
            if (dbExpressionList1.Count > 0)
            {
              ExpressionConverter.EqualsPattern pattern = comparisonBehavior ? ExpressionConverter.EqualsPattern.PositiveNullEqualityNonComposable : ExpressionConverter.EqualsPattern.Store;
              left = flag ? (DbExpression) DbExpressionBuilder.CreateInExpression(dbExpression1, (IList<DbExpression>) dbExpressionList1) : ExpressionConverter.MethodCallTranslator.ContainsTranslator.TranslateContainsHelper(parent, dbExpression1, (IEnumerable<DbExpression>) dbExpressionList1, pattern, elementType, valueExpression.Type);
              if (comparisonBehavior)
                left = (DbExpression) left.And((DbExpression) dbExpression1.IsNull().Not());
            }
            DbExpression right = (DbExpression) null;
            if (dbExpressionList2.Count > 0)
            {
              ExpressionConverter.EqualsPattern pattern = comparisonBehavior ? ExpressionConverter.EqualsPattern.PositiveNullEqualityComposable : ExpressionConverter.EqualsPattern.Store;
              right = ExpressionConverter.MethodCallTranslator.ContainsTranslator.TranslateContainsHelper(parent, dbExpression1, (IEnumerable<DbExpression>) dbExpressionList2, pattern, elementType, valueExpression.Type);
            }
            if (left == null)
              return right;
            if (right == null)
              return left;
            return (DbExpression) left.Or(right);
          }
          DbExpressionBinding input2 = input1.BindAs(parent.AliasGenerator.Next());
          ExpressionConverter.EqualsPattern pattern1 = ExpressionConverter.EqualsPattern.Store;
          if (parent._funcletizer.RootContext.ContextOptions.UseCSharpNullComparisonBehavior)
            pattern1 = ExpressionConverter.EqualsPattern.PositiveNullEqualityComposable;
          return input2.Filter(parent.CreateEqualsExpression((DbExpression) input2.Variable, dbExpression1, pattern1, elementType, valueExpression.Type)).Exists();
        }
      }

      private abstract class FirstTranslatorBase : ExpressionConverter.MethodCallTranslator.UnarySequenceMethodTranslator
      {
        protected FirstTranslatorBase(params SequenceMethod[] methods)
          : base(methods)
        {
        }

        protected virtual DbExpression LimitResult(
          ExpressionConverter parent,
          DbExpression expression)
        {
          return parent.Limit(expression, (DbExpression) DbExpressionBuilder.Constant((object) 1));
        }

        protected override DbExpression TranslateUnary(
          ExpressionConverter parent,
          DbExpression operand,
          MethodCallExpression call)
        {
          DbExpression expression = this.LimitResult(parent, operand);
          if (!parent.IsQueryRoot((Expression) call))
            expression = ExpressionConverter.MethodCallTranslator.FirstTranslatorBase.AddDefaultCase((DbExpression) expression.Element(), call.Type);
          Span span = (Span) null;
          if (parent.TryGetSpan(operand, out span))
            parent.AddSpanMapping(expression, span);
          return expression;
        }

        internal static DbExpression AddDefaultCase(
          DbExpression element,
          Type elementType)
        {
          object defaultValue = TypeSystem.GetDefaultValue(elementType);
          if (defaultValue == null)
            return element;
          return (DbExpression) DbExpressionBuilder.Case((IEnumerable<DbExpression>) new List<DbExpression>(1)
          {
            ExpressionConverter.CreateIsNullExpression(element, elementType)
          }, (IEnumerable<DbExpression>) new List<DbExpression>(1)
          {
            (DbExpression) element.ResultType.Constant(defaultValue)
          }, element);
        }
      }

      private sealed class FirstTranslator : ExpressionConverter.MethodCallTranslator.FirstTranslatorBase
      {
        internal FirstTranslator()
          : base(SequenceMethod.First)
        {
        }

        protected override DbExpression TranslateUnary(
          ExpressionConverter parent,
          DbExpression operand,
          MethodCallExpression call)
        {
          if (!parent.IsQueryRoot((Expression) call))
            throw new NotSupportedException(Strings.ELinq_UnsupportedNestedFirst);
          return base.TranslateUnary(parent, operand, call);
        }
      }

      private sealed class FirstOrDefaultTranslator : ExpressionConverter.MethodCallTranslator.FirstTranslatorBase
      {
        internal FirstOrDefaultTranslator()
          : base(SequenceMethod.FirstOrDefault)
        {
        }
      }

      private abstract class SingleTranslatorBase : ExpressionConverter.MethodCallTranslator.FirstTranslatorBase
      {
        protected SingleTranslatorBase(params SequenceMethod[] methods)
          : base(methods)
        {
        }

        protected override DbExpression TranslateUnary(
          ExpressionConverter parent,
          DbExpression operand,
          MethodCallExpression call)
        {
          if (!parent.IsQueryRoot((Expression) call))
            throw new NotSupportedException(Strings.ELinq_UnsupportedNestedSingle);
          return base.TranslateUnary(parent, operand, call);
        }

        protected override DbExpression LimitResult(
          ExpressionConverter parent,
          DbExpression expression)
        {
          return parent.Limit(expression, (DbExpression) DbExpressionBuilder.Constant((object) 2));
        }
      }

      private sealed class SingleTranslator : ExpressionConverter.MethodCallTranslator.SingleTranslatorBase
      {
        internal SingleTranslator()
          : base(SequenceMethod.Single)
        {
        }
      }

      private sealed class SingleOrDefaultTranslator : ExpressionConverter.MethodCallTranslator.SingleTranslatorBase
      {
        internal SingleOrDefaultTranslator()
          : base(SequenceMethod.SingleOrDefault)
        {
        }
      }

      private abstract class FirstPredicateTranslatorBase : ExpressionConverter.MethodCallTranslator.OneLambdaTranslator
      {
        protected FirstPredicateTranslatorBase(params SequenceMethod[] methods)
          : base(methods)
        {
        }

        protected virtual DbExpression RestrictResult(
          ExpressionConverter parent,
          DbExpression expression)
        {
          return parent.Limit(expression, (DbExpression) DbExpressionBuilder.Constant((object) 1));
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          DbExpression expression1 = base.Translate(parent, call);
          if (parent.IsQueryRoot((Expression) call))
            return this.RestrictResult(parent, expression1);
          DbExpression expression2 = this.RestrictResult(parent, expression1);
          DbExpression expression3 = ExpressionConverter.MethodCallTranslator.FirstTranslatorBase.AddDefaultCase((DbExpression) expression2.Element(), call.Type);
          Span span = (Span) null;
          if (parent.TryGetSpan(expression2, out span))
            parent.AddSpanMapping(expression3, span);
          return expression3;
        }

        protected override DbExpression TranslateOneLambda(
          ExpressionConverter parent,
          DbExpressionBinding sourceBinding,
          DbExpression lambda)
        {
          return parent.Filter(sourceBinding, lambda);
        }
      }

      private sealed class FirstPredicateTranslator : ExpressionConverter.MethodCallTranslator.FirstPredicateTranslatorBase
      {
        internal FirstPredicateTranslator()
          : base(SequenceMethod.FirstPredicate)
        {
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          if (!parent.IsQueryRoot((Expression) call))
            throw new NotSupportedException(Strings.ELinq_UnsupportedNestedFirst);
          return base.Translate(parent, call);
        }
      }

      private sealed class FirstOrDefaultPredicateTranslator : ExpressionConverter.MethodCallTranslator.FirstPredicateTranslatorBase
      {
        internal FirstOrDefaultPredicateTranslator()
          : base(SequenceMethod.FirstOrDefaultPredicate)
        {
        }
      }

      private abstract class SinglePredicateTranslatorBase : ExpressionConverter.MethodCallTranslator.FirstPredicateTranslatorBase
      {
        protected SinglePredicateTranslatorBase(params SequenceMethod[] methods)
          : base(methods)
        {
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          if (!parent.IsQueryRoot((Expression) call))
            throw new NotSupportedException(Strings.ELinq_UnsupportedNestedSingle);
          return base.Translate(parent, call);
        }

        protected override DbExpression RestrictResult(
          ExpressionConverter parent,
          DbExpression expression)
        {
          return parent.Limit(expression, (DbExpression) DbExpressionBuilder.Constant((object) 2));
        }
      }

      private sealed class SinglePredicateTranslator : ExpressionConverter.MethodCallTranslator.SinglePredicateTranslatorBase
      {
        internal SinglePredicateTranslator()
          : base(SequenceMethod.SinglePredicate)
        {
        }
      }

      private sealed class SingleOrDefaultPredicateTranslator : ExpressionConverter.MethodCallTranslator.SinglePredicateTranslatorBase
      {
        internal SingleOrDefaultPredicateTranslator()
          : base(SequenceMethod.SingleOrDefaultPredicate)
        {
        }
      }

      private sealed class SelectManyTranslator : ExpressionConverter.MethodCallTranslator.OneLambdaTranslator
      {
        internal SelectManyTranslator()
          : base(SequenceMethod.SelectMany, SequenceMethod.SelectManyResultSelector)
        {
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          LambdaExpression selectorLambda = call.Arguments.Count == 3 ? parent.GetLambdaExpression(call, 2) : (LambdaExpression) null;
          DbExpression dbExpression = base.Translate(parent, call);
          DbExpressionBinding crossApplyInput;
          EdmProperty lojRightInput;
          if (ExpressionConverter.MethodCallTranslator.SelectManyTranslator.IsLeftOuterJoin(dbExpression, out crossApplyInput, out lojRightInput))
          {
            string leftName;
            string rightName;
            InitializerMetadata initializerMetadata;
            if (selectorLambda != null && ExpressionConverter.MethodCallTranslator.IsTrivialRename(selectorLambda, parent, out leftName, out rightName, out initializerMetadata))
            {
              DbExpressionBinding input = crossApplyInput.Expression.BindAs(leftName);
              DbExpressionBinding apply = input.Variable.Property(lojRightInput.Name).BindAs(rightName);
              return (DbExpression) new DbApplyExpression(DbExpressionKind.OuterApply, TypeUsage.Create((EdmType) TypeHelpers.CreateCollectionType(TypeUsage.Create((EdmType) TypeHelpers.CreateRowType((IEnumerable<KeyValuePair<string, TypeUsage>>) new List<KeyValuePair<string, TypeUsage>>()
              {
                new KeyValuePair<string, TypeUsage>(input.VariableName, input.VariableType),
                new KeyValuePair<string, TypeUsage>(apply.VariableName, apply.VariableType)
              }, initializerMetadata)))), input, apply);
            }
            dbExpression = (DbExpression) crossApplyInput.OuterApply(crossApplyInput.Variable.Property(lojRightInput).BindAs(parent.AliasGenerator.Next()));
          }
          DbExpressionBinding input1 = dbExpression.BindAs(parent.AliasGenerator.Next());
          RowType edmType = (RowType) input1.Variable.ResultType.EdmType;
          DbExpression cqtExpression1 = (DbExpression) input1.Variable.Property(edmType.Properties[1]);
          DbExpression projection;
          if (selectorLambda != null)
          {
            DbExpression cqtExpression2 = (DbExpression) input1.Variable.Property(edmType.Properties[0]);
            parent._bindingContext.PushBindingScope(new Binding((Expression) selectorLambda.Parameters[0], cqtExpression2));
            parent._bindingContext.PushBindingScope(new Binding((Expression) selectorLambda.Parameters[1], cqtExpression1));
            projection = parent.TranslateSet(selectorLambda.Body);
            parent._bindingContext.PopBindingScope();
            parent._bindingContext.PopBindingScope();
          }
          else
            projection = cqtExpression1;
          return (DbExpression) input1.Project(projection);
        }

        private static bool IsLeftOuterJoin(
          DbExpression cqtExpression,
          out DbExpressionBinding crossApplyInput,
          out EdmProperty lojRightInput)
        {
          crossApplyInput = (DbExpressionBinding) null;
          lojRightInput = (EdmProperty) null;
          if (cqtExpression.ExpressionKind != DbExpressionKind.CrossApply)
            return false;
          DbApplyExpression dbApplyExpression = (DbApplyExpression) cqtExpression;
          if (dbApplyExpression.Input.VariableType.EdmType.BuiltInTypeKind != BuiltInTypeKind.RowType)
            return false;
          RowType edmType = (RowType) dbApplyExpression.Input.VariableType.EdmType;
          if (dbApplyExpression.Apply.Expression.ExpressionKind != DbExpressionKind.Project)
            return false;
          DbProjectExpression expression1 = (DbProjectExpression) dbApplyExpression.Apply.Expression;
          if (expression1.Input.Expression.ExpressionKind != DbExpressionKind.LeftOuterJoin)
            return false;
          DbJoinExpression expression2 = (DbJoinExpression) expression1.Input.Expression;
          if (expression1.Projection.ExpressionKind != DbExpressionKind.Property)
            return false;
          DbPropertyExpression projection = (DbPropertyExpression) expression1.Projection;
          if (projection.Instance != expression1.Input.Variable || projection.Property.Name != expression2.Right.VariableName || expression2.JoinCondition.ExpressionKind != DbExpressionKind.Constant)
            return false;
          DbConstantExpression joinCondition = (DbConstantExpression) expression2.JoinCondition;
          if (!(joinCondition.Value is bool) || !(bool) joinCondition.Value || expression2.Left.Expression.ExpressionKind != DbExpressionKind.NewInstance)
            return false;
          DbNewInstanceExpression expression3 = (DbNewInstanceExpression) expression2.Left.Expression;
          if (expression3.Arguments.Count != 1 || expression3.Arguments[0].ExpressionKind != DbExpressionKind.Constant || expression2.Right.Expression.ExpressionKind != DbExpressionKind.Property)
            return false;
          DbPropertyExpression lojRight = (DbPropertyExpression) expression2.Right.Expression;
          if (lojRight.Instance != dbApplyExpression.Input.Variable)
            return false;
          EdmProperty edmProperty = edmType.Properties.SingleOrDefault<EdmProperty>((Func<EdmProperty, bool>) (p => p.Name == lojRight.Property.Name));
          if (edmProperty == null)
            return false;
          crossApplyInput = dbApplyExpression.Input;
          lojRightInput = edmProperty;
          return true;
        }

        protected override DbExpression TranslateOneLambda(
          ExpressionConverter parent,
          DbExpressionBinding sourceBinding,
          DbExpression lambda)
        {
          lambda = parent.NormalizeSetSource(lambda);
          DbExpressionBinding apply = lambda.BindAs(parent.AliasGenerator.Next());
          return (DbExpression) sourceBinding.CrossApply(apply);
        }
      }

      private sealed class CastMethodTranslator : ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator
      {
        internal CastMethodTranslator()
          : base(SequenceMethod.Cast)
        {
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          DbExpression input1 = parent.TranslateSet(call.Arguments[0]);
          Type elementType1 = TypeSystem.GetElementType(call.Type);
          Type elementType2 = TypeSystem.GetElementType(call.Arguments[0].Type);
          DbExpressionBinding input2 = input1.BindAs(parent.AliasGenerator.Next());
          DbExpression castExpression = parent.CreateCastExpression((DbExpression) input2.Variable, elementType1, elementType2);
          return parent.Project(input2, castExpression);
        }
      }

      private sealed class GroupByTranslator : ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator
      {
        internal GroupByTranslator()
          : base(SequenceMethod.GroupBy, SequenceMethod.GroupByElementSelector, SequenceMethod.GroupByElementSelectorResultSelector, SequenceMethod.GroupByResultSelector)
        {
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call,
          SequenceMethod sequenceMethod)
        {
          DbExpression input1 = parent.TranslateSet(call.Arguments[0]);
          LambdaExpression lambdaExpression1 = parent.GetLambdaExpression(call, 1);
          DbGroupExpressionBinding binding1;
          DbExpression dbExpression = parent.TranslateLambda(lambdaExpression1, input1, out binding1);
          if (!TypeSemantics.IsEqualComparable(dbExpression.ResultType))
            throw new NotSupportedException(Strings.ELinq_UnsupportedKeySelector((object) call.Method.Name));
          List<KeyValuePair<string, DbExpression>> keyValuePairList1 = new List<KeyValuePair<string, DbExpression>>();
          List<KeyValuePair<string, DbAggregate>> keyValuePairList2 = new List<KeyValuePair<string, DbAggregate>>();
          keyValuePairList1.Add(new KeyValuePair<string, DbExpression>("Key", dbExpression));
          keyValuePairList2.Add(new KeyValuePair<string, DbAggregate>("Group", (DbAggregate) binding1.GroupAggregate));
          DbExpressionBinding input2 = binding1.GroupBy((IEnumerable<KeyValuePair<string, DbExpression>>) keyValuePairList1, (IEnumerable<KeyValuePair<string, DbAggregate>>) keyValuePairList2).BindAs(parent.AliasGenerator.Next());
          DbExpression input3 = (DbExpression) input2.Variable.Property("Group");
          if (sequenceMethod == SequenceMethod.GroupByElementSelector || sequenceMethod == SequenceMethod.GroupByElementSelectorResultSelector)
          {
            LambdaExpression lambdaExpression2 = parent.GetLambdaExpression(call, 2);
            DbExpressionBinding binding2;
            DbExpression projection = parent.TranslateLambda(lambdaExpression2, input3, out binding2);
            input3 = (DbExpression) binding2.Project(projection);
          }
          DbExpression[] dbExpressionArray = new DbExpression[2]
          {
            (DbExpression) input2.Variable.Property("Key"),
            input3
          };
          List<EdmProperty> edmPropertyList = new List<EdmProperty>(2);
          edmPropertyList.Add(new EdmProperty("Key", dbExpressionArray[0].ResultType));
          edmPropertyList.Add(new EdmProperty("Group", dbExpressionArray[1].ResultType));
          InitializerMetadata groupingInitializer = InitializerMetadata.CreateGroupingInitializer(parent.EdmItemCollection, TypeSystem.GetElementType(call.Type));
          TypeUsage instanceType = TypeUsage.Create((EdmType) new RowType((IEnumerable<EdmProperty>) edmPropertyList, groupingInitializer));
          DbExpression topLevelProject = (DbExpression) input2.Project((DbExpression) instanceType.New(dbExpressionArray));
          DbExpression result = topLevelProject;
          return ExpressionConverter.MethodCallTranslator.GroupByTranslator.ProcessResultSelector(parent, call, sequenceMethod, topLevelProject, result);
        }

        private static DbExpression ProcessResultSelector(
          ExpressionConverter parent,
          MethodCallExpression call,
          SequenceMethod sequenceMethod,
          DbExpression topLevelProject,
          DbExpression result)
        {
          LambdaExpression lambdaExpression = (LambdaExpression) null;
          switch (sequenceMethod)
          {
            case SequenceMethod.GroupByResultSelector:
              lambdaExpression = parent.GetLambdaExpression(call, 2);
              break;
            case SequenceMethod.GroupByElementSelectorResultSelector:
              lambdaExpression = parent.GetLambdaExpression(call, 3);
              break;
          }
          if (lambdaExpression != null)
          {
            DbExpressionBinding input = topLevelProject.BindAs(parent.AliasGenerator.Next());
            DbPropertyExpression propertyExpression1 = input.Variable.Property("Key");
            DbPropertyExpression propertyExpression2 = input.Variable.Property("Group");
            parent._bindingContext.PushBindingScope(new Binding((Expression) lambdaExpression.Parameters[0], (DbExpression) propertyExpression1));
            parent._bindingContext.PushBindingScope(new Binding((Expression) lambdaExpression.Parameters[1], (DbExpression) propertyExpression2));
            DbExpression projection = parent.TranslateExpression(lambdaExpression.Body);
            result = (DbExpression) input.Project(projection);
            parent._bindingContext.PopBindingScope();
            parent._bindingContext.PopBindingScope();
          }
          return result;
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          return (DbExpression) null;
        }
      }

      private sealed class GroupJoinTranslator : ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator
      {
        internal GroupJoinTranslator()
          : base(SequenceMethod.GroupJoin)
        {
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          DbExpression input1 = parent.TranslateSet(call.Arguments[0]);
          DbExpression input2 = parent.TranslateSet(call.Arguments[1]);
          LambdaExpression lambdaExpression1 = parent.GetLambdaExpression(call, 2);
          LambdaExpression lambdaExpression2 = parent.GetLambdaExpression(call, 3);
          DbExpressionBinding binding1;
          DbExpression left = parent.TranslateLambda(lambdaExpression1, input1, out binding1);
          DbExpressionBinding binding2;
          DbExpression right = parent.TranslateLambda(lambdaExpression2, input2, out binding2);
          if (!TypeSemantics.IsEqualComparable(left.ResultType) || !TypeSemantics.IsEqualComparable(right.ResultType))
            throw new NotSupportedException(Strings.ELinq_UnsupportedKeySelector((object) call.Method.Name));
          DbExpression dbExpression = parent.Filter(binding2, parent.CreateEqualsExpression(left, right, ExpressionConverter.EqualsPattern.PositiveNullEqualityNonComposable, lambdaExpression1.Body.Type, lambdaExpression2.Body.Type));
          DbExpression projection1 = (DbExpression) DbExpressionBuilder.NewRow((IEnumerable<KeyValuePair<string, DbExpression>>) new List<KeyValuePair<string, DbExpression>>(2)
          {
            new KeyValuePair<string, DbExpression>("o", (DbExpression) binding1.Variable),
            new KeyValuePair<string, DbExpression>("i", dbExpression)
          });
          DbExpressionBinding input3 = binding1.Project(projection1).BindAs(parent.AliasGenerator.Next());
          DbExpression cqtExpression1 = (DbExpression) input3.Variable.Property("o");
          DbExpression cqtExpression2 = (DbExpression) input3.Variable.Property("i");
          LambdaExpression lambdaExpression3 = parent.GetLambdaExpression(call, 4);
          parent._bindingContext.PushBindingScope(new Binding((Expression) lambdaExpression3.Parameters[0], cqtExpression1));
          parent._bindingContext.PushBindingScope(new Binding((Expression) lambdaExpression3.Parameters[1], cqtExpression2));
          DbExpression projection2 = parent.TranslateExpression(lambdaExpression3.Body);
          parent._bindingContext.PopBindingScope();
          parent._bindingContext.PopBindingScope();
          return ExpressionConverter.MethodCallTranslator.GroupJoinTranslator.CollapseTrivialRenamingProjection((DbExpression) input3.Project(projection2));
        }

        private static DbExpression CollapseTrivialRenamingProjection(
          DbExpression cqtExpression)
        {
          if (cqtExpression.ExpressionKind != DbExpressionKind.Project)
            return cqtExpression;
          DbProjectExpression projectExpression = (DbProjectExpression) cqtExpression;
          if (projectExpression.Projection.ExpressionKind != DbExpressionKind.NewInstance || projectExpression.Projection.ResultType.EdmType.BuiltInTypeKind != BuiltInTypeKind.RowType)
            return cqtExpression;
          DbNewInstanceExpression projection1 = (DbNewInstanceExpression) projectExpression.Projection;
          RowType edmType1 = (RowType) projection1.ResultType.EdmType;
          List<Tuple<EdmProperty, string>> tupleList = new List<Tuple<EdmProperty, string>>();
          for (int index = 0; index < projection1.Arguments.Count; ++index)
          {
            if (projection1.Arguments[index].ExpressionKind != DbExpressionKind.Property)
              return cqtExpression;
            DbPropertyExpression propertyExpression = (DbPropertyExpression) projection1.Arguments[index];
            if (propertyExpression.Instance != projectExpression.Input.Variable)
              return cqtExpression;
            tupleList.Add(Tuple.Create<EdmProperty, string>((EdmProperty) propertyExpression.Property, edmType1.Properties[index].Name));
          }
          if (projectExpression.Input.Expression.ExpressionKind != DbExpressionKind.Project)
            return cqtExpression;
          DbProjectExpression expression = (DbProjectExpression) projectExpression.Input.Expression;
          if (expression.Projection.ExpressionKind != DbExpressionKind.NewInstance || expression.Projection.ResultType.EdmType.BuiltInTypeKind != BuiltInTypeKind.RowType)
            return cqtExpression;
          DbNewInstanceExpression projection2 = (DbNewInstanceExpression) expression.Projection;
          RowType edmType2 = (RowType) projection2.ResultType.EdmType;
          List<DbExpression> dbExpressionList = new List<DbExpression>();
          foreach (Tuple<EdmProperty, string> tuple in tupleList)
          {
            int index = edmType2.Properties.IndexOf(tuple.Item1);
            dbExpressionList.Add(projection2.Arguments[index]);
          }
          DbNewInstanceExpression instanceExpression = projection1.ResultType.New((IEnumerable<DbExpression>) dbExpressionList);
          return (DbExpression) expression.Input.Project((DbExpression) instanceExpression);
        }
      }

      private abstract class OrderByTranslatorBase : ExpressionConverter.MethodCallTranslator.OneLambdaTranslator
      {
        private readonly bool _ascending;

        protected OrderByTranslatorBase(bool ascending, params SequenceMethod[] methods)
          : base(methods)
        {
          this._ascending = ascending;
        }

        protected override DbExpression TranslateOneLambda(
          ExpressionConverter parent,
          DbExpressionBinding sourceBinding,
          DbExpression lambda)
        {
          return (DbExpression) parent.Sort(sourceBinding, (IList<DbSortClause>) new List<DbSortClause>(1)
          {
            this._ascending ? lambda.ToSortClause() : lambda.ToSortClauseDescending()
          });
        }
      }

      private sealed class OrderByTranslator : ExpressionConverter.MethodCallTranslator.OrderByTranslatorBase
      {
        internal OrderByTranslator()
          : base(true, SequenceMethod.OrderBy)
        {
        }
      }

      private sealed class OrderByDescendingTranslator : ExpressionConverter.MethodCallTranslator.OrderByTranslatorBase
      {
        internal OrderByDescendingTranslator()
          : base(false, SequenceMethod.OrderByDescending)
        {
        }
      }

      private abstract class ThenByTranslatorBase : ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator
      {
        private readonly bool _ascending;

        protected ThenByTranslatorBase(bool ascending, params SequenceMethod[] methods)
          : base(methods)
        {
          this._ascending = ascending;
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          DbExpression dbExpression = parent.TranslateSet(call.Arguments[0]);
          if (DbExpressionKind.Sort != dbExpression.ExpressionKind)
            throw new InvalidOperationException(Strings.ELinq_ThenByDoesNotFollowOrderBy);
          DbSortExpression dbSortExpression = (DbSortExpression) dbExpression;
          DbExpressionBinding input = dbSortExpression.Input;
          LambdaExpression lambdaExpression = parent.GetLambdaExpression(call, 1);
          ParameterExpression parameter = lambdaExpression.Parameters[0];
          parent._bindingContext.PushBindingScope(new Binding((Expression) parameter, (DbExpression) input.Variable));
          DbExpression key = parent.TranslateExpression(lambdaExpression.Body);
          parent._bindingContext.PopBindingScope();
          return (DbExpression) parent.Sort(input, (IList<DbSortClause>) new List<DbSortClause>((IEnumerable<DbSortClause>) dbSortExpression.SortOrder)
          {
            new DbSortClause(key, this._ascending, (string) null)
          });
        }
      }

      private sealed class ThenByTranslator : ExpressionConverter.MethodCallTranslator.ThenByTranslatorBase
      {
        internal ThenByTranslator()
          : base(true, SequenceMethod.ThenBy)
        {
        }
      }

      private sealed class ThenByDescendingTranslator : ExpressionConverter.MethodCallTranslator.ThenByTranslatorBase
      {
        internal ThenByDescendingTranslator()
          : base(false, SequenceMethod.ThenByDescending)
        {
        }
      }

      private sealed class SpatialMethodCallTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
      {
        private static readonly Dictionary<MethodInfo, string> _methodFunctionRenames = ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetRenamedMethodFunctions();

        internal SpatialMethodCallTranslator()
          : base(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetSupportedMethods())
        {
        }

        private static MethodInfo GetStaticMethod<TResult>(
          Expression<Func<TResult>> lambda)
        {
          return ((MethodCallExpression) lambda.Body).Method;
        }

        private static MethodInfo GetInstanceMethod<T, TResult>(
          Expression<Func<T, TResult>> lambda)
        {
          return ((MethodCallExpression) lambda.Body).Method;
        }

        private static IEnumerable<MethodInfo> GetSupportedMethods()
        {
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.FromText(default (string))));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.FromText(default (string), 0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.PointFromText(default (string), 0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.LineFromText(default (string), 0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.PolygonFromText(default (string), 0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.MultiPointFromText(default (string), 0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.MultiLineFromText(default (string), 0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.MultiPolygonFromText(default (string), 0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.GeographyCollectionFromText(default (string), 0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.FromBinary(default (byte[]), 0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.FromBinary(default (byte[]))));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.PointFromBinary(default (byte[]), 0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.LineFromBinary(default (byte[]), 0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.PolygonFromBinary(default (byte[]), 0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.MultiPointFromBinary(default (byte[]), 0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.MultiLineFromBinary(default (byte[]), 0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.MultiPolygonFromBinary(default (byte[]), 0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.GeographyCollectionFromBinary(default (byte[]), 0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.FromGml(default (string))));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.FromGml(default (string), 0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeography, byte[]>((Expression<Func<DbGeography, byte[]>>) (geo => geo.AsBinary()));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeography, string>((Expression<Func<DbGeography, string>>) (geo => geo.AsGml()));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeography, string>((Expression<Func<DbGeography, string>>) (geo => geo.AsText()));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeography, bool>((Expression<Func<DbGeography, bool>>) (geo => geo.SpatialEquals(default (DbGeography))));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeography, bool>((Expression<Func<DbGeography, bool>>) (geo => geo.Disjoint(default (DbGeography))));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeography, bool>((Expression<Func<DbGeography, bool>>) (geo => geo.Intersects(default (DbGeography))));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeography, DbGeography>((Expression<Func<DbGeography, DbGeography>>) (geo => geo.Buffer((double?) 0.0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeography, double?>((Expression<Func<DbGeography, double?>>) (geo => geo.Distance(default (DbGeography))));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeography, DbGeography>((Expression<Func<DbGeography, DbGeography>>) (geo => geo.Intersection(default (DbGeography))));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeography, DbGeography>((Expression<Func<DbGeography, DbGeography>>) (geo => geo.Union(default (DbGeography))));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeography, DbGeography>((Expression<Func<DbGeography, DbGeography>>) (geo => geo.Difference(default (DbGeography))));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeography, DbGeography>((Expression<Func<DbGeography, DbGeography>>) (geo => geo.SymmetricDifference(default (DbGeography))));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeography, DbGeography>((Expression<Func<DbGeography, DbGeography>>) (geo => geo.ElementAt(0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeography, DbGeography>((Expression<Func<DbGeography, DbGeography>>) (geo => geo.PointAt(0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.FromText(default (string))));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.FromText(default (string), 0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.PointFromText(default (string), 0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.LineFromText(default (string), 0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.PolygonFromText(default (string), 0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.MultiPointFromText(default (string), 0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.MultiLineFromText(default (string), 0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.MultiPolygonFromText(default (string), 0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.GeometryCollectionFromText(default (string), 0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.FromBinary(default (byte[]))));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.FromBinary(default (byte[]), 0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.PointFromBinary(default (byte[]), 0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.LineFromBinary(default (byte[]), 0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.PolygonFromBinary(default (byte[]), 0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.MultiPointFromBinary(default (byte[]), 0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.MultiLineFromBinary(default (byte[]), 0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.MultiPolygonFromBinary(default (byte[]), 0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.GeometryCollectionFromBinary(default (byte[]), 0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.FromGml(default (string))));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.FromGml(default (string), 0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, byte[]>((Expression<Func<DbGeometry, byte[]>>) (geo => geo.AsBinary()));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, string>((Expression<Func<DbGeometry, string>>) (geo => geo.AsGml()));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, string>((Expression<Func<DbGeometry, string>>) (geo => geo.AsText()));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, bool>((Expression<Func<DbGeometry, bool>>) (geo => geo.SpatialEquals(default (DbGeometry))));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, bool>((Expression<Func<DbGeometry, bool>>) (geo => geo.Disjoint(default (DbGeometry))));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, bool>((Expression<Func<DbGeometry, bool>>) (geo => geo.Intersects(default (DbGeometry))));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, bool>((Expression<Func<DbGeometry, bool>>) (geo => geo.Touches(default (DbGeometry))));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, bool>((Expression<Func<DbGeometry, bool>>) (geo => geo.Crosses(default (DbGeometry))));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, bool>((Expression<Func<DbGeometry, bool>>) (geo => geo.Within(default (DbGeometry))));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, bool>((Expression<Func<DbGeometry, bool>>) (geo => geo.Contains(default (DbGeometry))));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, bool>((Expression<Func<DbGeometry, bool>>) (geo => geo.Overlaps(default (DbGeometry))));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, bool>((Expression<Func<DbGeometry, bool>>) (geo => geo.Relate(default (DbGeometry), default (string))));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, DbGeometry>((Expression<Func<DbGeometry, DbGeometry>>) (geo => geo.Buffer((double?) 0.0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, double?>((Expression<Func<DbGeometry, double?>>) (geo => geo.Distance(default (DbGeometry))));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, DbGeometry>((Expression<Func<DbGeometry, DbGeometry>>) (geo => geo.Intersection(default (DbGeometry))));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, DbGeometry>((Expression<Func<DbGeometry, DbGeometry>>) (geo => geo.Union(default (DbGeometry))));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, DbGeometry>((Expression<Func<DbGeometry, DbGeometry>>) (geo => geo.Difference(default (DbGeometry))));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, DbGeometry>((Expression<Func<DbGeometry, DbGeometry>>) (geo => geo.SymmetricDifference(default (DbGeometry))));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, DbGeometry>((Expression<Func<DbGeometry, DbGeometry>>) (geo => geo.ElementAt(0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, DbGeometry>((Expression<Func<DbGeometry, DbGeometry>>) (geo => geo.PointAt(0)));
          yield return ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, DbGeometry>((Expression<Func<DbGeometry, DbGeometry>>) (geo => geo.InteriorRingAt(0)));
        }

        private static Dictionary<MethodInfo, string> GetRenamedMethodFunctions()
        {
          Dictionary<MethodInfo, string> dictionary = new Dictionary<MethodInfo, string>();
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.FromText(default (string)))), "GeographyFromText");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.FromText(default (string), 0))), "GeographyFromText");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.PointFromText(default (string), 0))), "GeographyPointFromText");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.LineFromText(default (string), 0))), "GeographyLineFromText");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.PolygonFromText(default (string), 0))), "GeographyPolygonFromText");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.MultiPointFromText(default (string), 0))), "GeographyMultiPointFromText");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.MultiLineFromText(default (string), 0))), "GeographyMultiLineFromText");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.MultiPolygonFromText(default (string), 0))), "GeographyMultiPolygonFromText");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.GeographyCollectionFromText(default (string), 0))), "GeographyCollectionFromText");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.FromBinary(default (byte[]), 0))), "GeographyFromBinary");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.FromBinary(default (byte[])))), "GeographyFromBinary");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.PointFromBinary(default (byte[]), 0))), "GeographyPointFromBinary");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.LineFromBinary(default (byte[]), 0))), "GeographyLineFromBinary");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.PolygonFromBinary(default (byte[]), 0))), "GeographyPolygonFromBinary");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.MultiPointFromBinary(default (byte[]), 0))), "GeographyMultiPointFromBinary");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.MultiLineFromBinary(default (byte[]), 0))), "GeographyMultiLineFromBinary");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.MultiPolygonFromBinary(default (byte[]), 0))), "GeographyMultiPolygonFromBinary");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.GeographyCollectionFromBinary(default (byte[]), 0))), "GeographyCollectionFromBinary");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.FromGml(default (string)))), "GeographyFromGml");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeography>((Expression<Func<DbGeography>>) (() => DbGeography.FromGml(default (string), 0))), "GeographyFromGml");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeography, byte[]>((Expression<Func<DbGeography, byte[]>>) (geo => geo.AsBinary())), "AsBinary");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeography, string>((Expression<Func<DbGeography, string>>) (geo => geo.AsGml())), "AsGml");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeography, string>((Expression<Func<DbGeography, string>>) (geo => geo.AsText())), "AsText");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeography, bool>((Expression<Func<DbGeography, bool>>) (geo => geo.SpatialEquals(default (DbGeography)))), "SpatialEquals");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeography, bool>((Expression<Func<DbGeography, bool>>) (geo => geo.Disjoint(default (DbGeography)))), "SpatialDisjoint");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeography, bool>((Expression<Func<DbGeography, bool>>) (geo => geo.Intersects(default (DbGeography)))), "SpatialIntersects");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeography, DbGeography>((Expression<Func<DbGeography, DbGeography>>) (geo => geo.Buffer((double?) 0.0))), "SpatialBuffer");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeography, double?>((Expression<Func<DbGeography, double?>>) (geo => geo.Distance(default (DbGeography)))), "Distance");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeography, DbGeography>((Expression<Func<DbGeography, DbGeography>>) (geo => geo.Intersection(default (DbGeography)))), "SpatialIntersection");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeography, DbGeography>((Expression<Func<DbGeography, DbGeography>>) (geo => geo.Union(default (DbGeography)))), "SpatialUnion");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeography, DbGeography>((Expression<Func<DbGeography, DbGeography>>) (geo => geo.Difference(default (DbGeography)))), "SpatialDifference");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeography, DbGeography>((Expression<Func<DbGeography, DbGeography>>) (geo => geo.SymmetricDifference(default (DbGeography)))), "SpatialSymmetricDifference");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeography, DbGeography>((Expression<Func<DbGeography, DbGeography>>) (geo => geo.ElementAt(0))), "SpatialElementAt");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeography, DbGeography>((Expression<Func<DbGeography, DbGeography>>) (geo => geo.PointAt(0))), "PointAt");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.FromText(default (string)))), "GeometryFromText");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.FromText(default (string), 0))), "GeometryFromText");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.PointFromText(default (string), 0))), "GeometryPointFromText");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.LineFromText(default (string), 0))), "GeometryLineFromText");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.PolygonFromText(default (string), 0))), "GeometryPolygonFromText");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.MultiPointFromText(default (string), 0))), "GeometryMultiPointFromText");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.MultiLineFromText(default (string), 0))), "GeometryMultiLineFromText");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.MultiPolygonFromText(default (string), 0))), "GeometryMultiPolygonFromText");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.GeometryCollectionFromText(default (string), 0))), "GeometryCollectionFromText");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.FromBinary(default (byte[])))), "GeometryFromBinary");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.FromBinary(default (byte[]), 0))), "GeometryFromBinary");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.PointFromBinary(default (byte[]), 0))), "GeometryPointFromBinary");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.LineFromBinary(default (byte[]), 0))), "GeometryLineFromBinary");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.PolygonFromBinary(default (byte[]), 0))), "GeometryPolygonFromBinary");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.MultiPointFromBinary(default (byte[]), 0))), "GeometryMultiPointFromBinary");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.MultiLineFromBinary(default (byte[]), 0))), "GeometryMultiLineFromBinary");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.MultiPolygonFromBinary(default (byte[]), 0))), "GeometryMultiPolygonFromBinary");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.GeometryCollectionFromBinary(default (byte[]), 0))), "GeometryCollectionFromBinary");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.FromGml(default (string)))), "GeometryFromGml");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetStaticMethod<DbGeometry>((Expression<Func<DbGeometry>>) (() => DbGeometry.FromGml(default (string), 0))), "GeometryFromGml");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, byte[]>((Expression<Func<DbGeometry, byte[]>>) (geo => geo.AsBinary())), "AsBinary");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, string>((Expression<Func<DbGeometry, string>>) (geo => geo.AsGml())), "AsGml");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, string>((Expression<Func<DbGeometry, string>>) (geo => geo.AsText())), "AsText");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, bool>((Expression<Func<DbGeometry, bool>>) (geo => geo.SpatialEquals(default (DbGeometry)))), "SpatialEquals");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, bool>((Expression<Func<DbGeometry, bool>>) (geo => geo.Disjoint(default (DbGeometry)))), "SpatialDisjoint");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, bool>((Expression<Func<DbGeometry, bool>>) (geo => geo.Intersects(default (DbGeometry)))), "SpatialIntersects");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, bool>((Expression<Func<DbGeometry, bool>>) (geo => geo.Touches(default (DbGeometry)))), "SpatialTouches");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, bool>((Expression<Func<DbGeometry, bool>>) (geo => geo.Crosses(default (DbGeometry)))), "SpatialCrosses");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, bool>((Expression<Func<DbGeometry, bool>>) (geo => geo.Within(default (DbGeometry)))), "SpatialWithin");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, bool>((Expression<Func<DbGeometry, bool>>) (geo => geo.Contains(default (DbGeometry)))), "SpatialContains");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, bool>((Expression<Func<DbGeometry, bool>>) (geo => geo.Overlaps(default (DbGeometry)))), "SpatialOverlaps");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, bool>((Expression<Func<DbGeometry, bool>>) (geo => geo.Relate(default (DbGeometry), default (string)))), "SpatialRelate");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, DbGeometry>((Expression<Func<DbGeometry, DbGeometry>>) (geo => geo.Buffer((double?) 0.0))), "SpatialBuffer");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, double?>((Expression<Func<DbGeometry, double?>>) (geo => geo.Distance(default (DbGeometry)))), "Distance");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, DbGeometry>((Expression<Func<DbGeometry, DbGeometry>>) (geo => geo.Intersection(default (DbGeometry)))), "SpatialIntersection");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, DbGeometry>((Expression<Func<DbGeometry, DbGeometry>>) (geo => geo.Union(default (DbGeometry)))), "SpatialUnion");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, DbGeometry>((Expression<Func<DbGeometry, DbGeometry>>) (geo => geo.Difference(default (DbGeometry)))), "SpatialDifference");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, DbGeometry>((Expression<Func<DbGeometry, DbGeometry>>) (geo => geo.SymmetricDifference(default (DbGeometry)))), "SpatialSymmetricDifference");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, DbGeometry>((Expression<Func<DbGeometry, DbGeometry>>) (geo => geo.ElementAt(0))), "SpatialElementAt");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, DbGeometry>((Expression<Func<DbGeometry, DbGeometry>>) (geo => geo.PointAt(0))), "PointAt");
          dictionary.Add(ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator.GetInstanceMethod<DbGeometry, DbGeometry>((Expression<Func<DbGeometry, DbGeometry>>) (geo => geo.InteriorRingAt(0))), "InteriorRingAt");
          return dictionary;
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MethodCallExpression call)
        {
          MethodInfo method = call.Method;
          string functionName;
          if (!ExpressionConverter.MethodCallTranslator.SpatialMethodCallTranslator._methodFunctionRenames.TryGetValue(method, out functionName))
            functionName = "ST" + method.Name;
          Expression[] array;
          if (method.IsStatic)
            array = call.Arguments.ToArray<Expression>();
          else
            array = ((IEnumerable<Expression>) new Expression[1]
            {
              call.Object
            }).Concat<Expression>((IEnumerable<Expression>) call.Arguments).ToArray<Expression>();
          return (DbExpression) parent.TranslateIntoCanonicalFunction(functionName, (Expression) call, array);
        }
      }
    }

    private sealed class OrderByLifter
    {
      private readonly AliasGenerator _aliasGenerator;

      internal OrderByLifter(AliasGenerator aliasGenerator)
      {
        this._aliasGenerator = aliasGenerator;
      }

      internal DbExpression Project(DbExpressionBinding input, DbExpression projection)
      {
        return this.GetLifter(input.Expression).Project(input.Project(projection));
      }

      internal DbExpression Filter(DbExpressionBinding input, DbExpression predicate)
      {
        return this.GetLifter(input.Expression).Filter(input.Filter(predicate));
      }

      internal DbExpression OfType(DbExpression argument, TypeUsage type)
      {
        return this.GetLifter(argument).OfType(type);
      }

      internal DbExpression Skip(DbExpressionBinding input, DbExpression skipCount)
      {
        return this.GetLifter(input.Expression).Skip(skipCount);
      }

      internal DbExpression Limit(DbExpression argument, DbExpression limit)
      {
        return this.GetLifter(argument).Limit(limit);
      }

      private ExpressionConverter.OrderByLifter.OrderByLifterBase GetLifter(
        DbExpression root)
      {
        return ExpressionConverter.OrderByLifter.OrderByLifterBase.GetLifter(root, this._aliasGenerator);
      }

      private abstract class OrderByLifterBase
      {
        protected readonly DbExpression _root;
        protected readonly AliasGenerator _aliasGenerator;

        protected OrderByLifterBase(DbExpression root, AliasGenerator aliasGenerator)
        {
          this._root = root;
          this._aliasGenerator = aliasGenerator;
        }

        internal static ExpressionConverter.OrderByLifter.OrderByLifterBase GetLifter(
          DbExpression source,
          AliasGenerator aliasGenerator)
        {
          if (source.ExpressionKind == DbExpressionKind.Sort)
            return (ExpressionConverter.OrderByLifter.OrderByLifterBase) new ExpressionConverter.OrderByLifter.SortLifter((DbSortExpression) source, aliasGenerator);
          if (source.ExpressionKind == DbExpressionKind.Project)
          {
            DbProjectExpression project = (DbProjectExpression) source;
            DbExpression expression = project.Input.Expression;
            if (expression.ExpressionKind == DbExpressionKind.Sort)
              return (ExpressionConverter.OrderByLifter.OrderByLifterBase) new ExpressionConverter.OrderByLifter.ProjectSortLifter(project, (DbSortExpression) expression, aliasGenerator);
            if (expression.ExpressionKind == DbExpressionKind.Skip)
              return (ExpressionConverter.OrderByLifter.OrderByLifterBase) new ExpressionConverter.OrderByLifter.ProjectSkipLifter(project, (DbSkipExpression) expression, aliasGenerator);
            if (expression.ExpressionKind == DbExpressionKind.Limit)
            {
              DbLimitExpression limit = (DbLimitExpression) expression;
              DbExpression dbExpression = limit.Argument;
              if (dbExpression.ExpressionKind == DbExpressionKind.Sort)
                return (ExpressionConverter.OrderByLifter.OrderByLifterBase) new ExpressionConverter.OrderByLifter.ProjectLimitSortLifter(project, limit, (DbSortExpression) dbExpression, aliasGenerator);
              if (dbExpression.ExpressionKind == DbExpressionKind.Skip)
                return (ExpressionConverter.OrderByLifter.OrderByLifterBase) new ExpressionConverter.OrderByLifter.ProjectLimitSkipLifter(project, limit, (DbSkipExpression) dbExpression, aliasGenerator);
            }
          }
          if (source.ExpressionKind == DbExpressionKind.Skip)
            return (ExpressionConverter.OrderByLifter.OrderByLifterBase) new ExpressionConverter.OrderByLifter.SkipLifter((DbSkipExpression) source, aliasGenerator);
          if (source.ExpressionKind == DbExpressionKind.Limit)
          {
            DbLimitExpression limit = (DbLimitExpression) source;
            DbExpression dbExpression = limit.Argument;
            if (dbExpression.ExpressionKind == DbExpressionKind.Sort)
              return (ExpressionConverter.OrderByLifter.OrderByLifterBase) new ExpressionConverter.OrderByLifter.LimitSortLifter(limit, (DbSortExpression) dbExpression, aliasGenerator);
            if (dbExpression.ExpressionKind == DbExpressionKind.Skip)
              return (ExpressionConverter.OrderByLifter.OrderByLifterBase) new ExpressionConverter.OrderByLifter.LimitSkipLifter(limit, (DbSkipExpression) dbExpression, aliasGenerator);
            if (dbExpression.ExpressionKind == DbExpressionKind.Project)
            {
              DbProjectExpression project = (DbProjectExpression) dbExpression;
              DbExpression expression = project.Input.Expression;
              if (expression.ExpressionKind == DbExpressionKind.Sort)
                return (ExpressionConverter.OrderByLifter.OrderByLifterBase) new ExpressionConverter.OrderByLifter.ProjectLimitSortLifter(project, limit, (DbSortExpression) expression, aliasGenerator);
              if (expression.ExpressionKind == DbExpressionKind.Skip)
                return (ExpressionConverter.OrderByLifter.OrderByLifterBase) new ExpressionConverter.OrderByLifter.ProjectLimitSkipLifter(project, limit, (DbSkipExpression) expression, aliasGenerator);
            }
          }
          return (ExpressionConverter.OrderByLifter.OrderByLifterBase) new ExpressionConverter.OrderByLifter.PassthroughOrderByLifter(source, aliasGenerator);
        }

        internal abstract DbExpression Project(DbProjectExpression project);

        internal abstract DbExpression Filter(DbFilterExpression filter);

        internal virtual DbExpression OfType(TypeUsage type)
        {
          DbExpressionBinding input1 = this._root.BindAs(this._aliasGenerator.Next());
          DbExpression dbExpression = this.Filter(input1.Filter((DbExpression) input1.Variable.IsOf(type)));
          ExpressionConverter.OrderByLifter.OrderByLifterBase lifter = ExpressionConverter.OrderByLifter.OrderByLifterBase.GetLifter(dbExpression, this._aliasGenerator);
          DbExpressionBinding input2 = dbExpression.BindAs(this._aliasGenerator.Next());
          return lifter.Project(input2.Project((DbExpression) input2.Variable.TreatAs(type)));
        }

        internal abstract DbExpression Limit(DbExpression k);

        internal abstract DbExpression Skip(DbExpression k);

        protected static DbProjectExpression ComposeProject(
          DbExpression input,
          DbProjectExpression first,
          DbProjectExpression second)
        {
          DbLambda lambda = DbExpressionBuilder.Lambda(second.Projection, second.Input.Variable);
          DbProjectExpression project = first.Input.Project((DbExpression) lambda.Invoke(first.Projection));
          return ExpressionConverter.OrderByLifter.OrderByLifterBase.RebindProject(input, project);
        }

        protected static DbFilterExpression ComposeFilter(
          DbExpression input,
          DbProjectExpression first,
          DbFilterExpression second)
        {
          DbLambda lambda = DbExpressionBuilder.Lambda(second.Predicate, second.Input.Variable);
          DbFilterExpression filter = first.Input.Filter((DbExpression) lambda.Invoke(first.Projection));
          return ExpressionConverter.OrderByLifter.OrderByLifterBase.RebindFilter(input, filter);
        }

        protected static DbSkipExpression AddToSkip(
          DbExpression input,
          DbSkipExpression skip,
          DbExpression plusK)
        {
          DbExpression k = ExpressionConverter.OrderByLifter.OrderByLifterBase.CombineIntegers(skip.Count, plusK, (Func<int, int, int>) ((l, r) => l + r));
          return ExpressionConverter.OrderByLifter.OrderByLifterBase.RebindSkip(input, skip, k);
        }

        protected static DbLimitExpression SubtractFromLimit(
          DbExpression input,
          DbLimitExpression limit,
          DbExpression minusK)
        {
          DbExpression count = ExpressionConverter.OrderByLifter.OrderByLifterBase.CombineIntegers(limit.Limit, minusK, (Func<int, int, int>) ((l, r) =>
          {
            if (r <= l)
              return l - r;
            return 0;
          }));
          return input.Limit(count);
        }

        protected static DbLimitExpression MinimumLimit(
          DbExpression input,
          DbLimitExpression limit,
          DbExpression k)
        {
          DbExpression count = ExpressionConverter.OrderByLifter.OrderByLifterBase.CombineIntegers(limit.Limit, k, new Func<int, int, int>(Math.Min));
          return input.Limit(count);
        }

        private static DbExpression CombineIntegers(
          DbExpression left,
          DbExpression right,
          Func<int, int, int> combineConstants)
        {
          if (left.ExpressionKind == DbExpressionKind.Constant && right.ExpressionKind == DbExpressionKind.Constant)
          {
            object obj1 = ((DbConstantExpression) left).Value;
            object obj2 = ((DbConstantExpression) right).Value;
            if (obj1 is int && obj2 is int)
              return (DbExpression) left.ResultType.Constant((object) combineConstants((int) obj1, (int) obj2));
          }
          throw new InvalidOperationException(Strings.ADP_InternalProviderError((object) 1025));
        }

        protected static DbProjectExpression RebindProject(
          DbExpression input,
          DbProjectExpression project)
        {
          return input.BindAs(project.Input.VariableName).Project(project.Projection);
        }

        protected static DbFilterExpression RebindFilter(
          DbExpression input,
          DbFilterExpression filter)
        {
          return input.BindAs(filter.Input.VariableName).Filter(filter.Predicate);
        }

        protected static DbSortExpression RebindSort(
          DbExpression input,
          DbSortExpression sort)
        {
          return input.BindAs(sort.Input.VariableName).Sort((IEnumerable<DbSortClause>) sort.SortOrder);
        }

        protected static DbSortExpression ApplySkipOrderToSort(
          DbExpression input,
          DbSkipExpression sortSpec)
        {
          return input.BindAs(sortSpec.Input.VariableName).Sort((IEnumerable<DbSortClause>) sortSpec.SortOrder);
        }

        protected static DbSkipExpression ApplySortOrderToSkip(
          DbExpression input,
          DbSortExpression sort,
          DbExpression k)
        {
          return input.BindAs(sort.Input.VariableName).Skip((IEnumerable<DbSortClause>) sort.SortOrder, k);
        }

        protected static DbSkipExpression RebindSkip(
          DbExpression input,
          DbSkipExpression skip,
          DbExpression k)
        {
          return input.BindAs(skip.Input.VariableName).Skip((IEnumerable<DbSortClause>) skip.SortOrder, k);
        }
      }

      private class LimitSkipLifter : ExpressionConverter.OrderByLifter.OrderByLifterBase
      {
        private readonly DbLimitExpression _limit;
        private readonly DbSkipExpression _skip;

        internal LimitSkipLifter(
          DbLimitExpression limit,
          DbSkipExpression skip,
          AliasGenerator aliasGenerator)
          : base((DbExpression) limit, aliasGenerator)
        {
          this._limit = limit;
          this._skip = skip;
        }

        internal override DbExpression Filter(DbFilterExpression filter)
        {
          return (DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.ApplySkipOrderToSort((DbExpression) filter, this._skip);
        }

        internal override DbExpression Project(DbProjectExpression project)
        {
          return (DbExpression) project;
        }

        internal override DbExpression Limit(DbExpression k)
        {
          if (this._limit.Limit.ExpressionKind == DbExpressionKind.Constant && k.ExpressionKind == DbExpressionKind.Constant)
            return (DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.MinimumLimit((DbExpression) this._skip, this._limit, k);
          return (DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.ApplySkipOrderToSort((DbExpression) this._limit, this._skip).Limit(k);
        }

        internal override DbExpression Skip(DbExpression k)
        {
          return (DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.RebindSkip((DbExpression) this._limit, this._skip, k);
        }
      }

      private class LimitSortLifter : ExpressionConverter.OrderByLifter.OrderByLifterBase
      {
        private readonly DbLimitExpression _limit;
        private readonly DbSortExpression _sort;

        internal LimitSortLifter(
          DbLimitExpression limit,
          DbSortExpression sort,
          AliasGenerator aliasGenerator)
          : base((DbExpression) limit, aliasGenerator)
        {
          this._limit = limit;
          this._sort = sort;
        }

        internal override DbExpression Filter(DbFilterExpression filter)
        {
          return (DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.RebindSort((DbExpression) filter, this._sort);
        }

        internal override DbExpression Project(DbProjectExpression project)
        {
          return (DbExpression) project;
        }

        internal override DbExpression Limit(DbExpression k)
        {
          if (this._limit.Limit.ExpressionKind == DbExpressionKind.Constant && k.ExpressionKind == DbExpressionKind.Constant)
            return (DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.MinimumLimit((DbExpression) this._sort, this._limit, k);
          return (DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.RebindSort((DbExpression) this._limit, this._sort).Limit(k);
        }

        internal override DbExpression Skip(DbExpression k)
        {
          return (DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.ApplySortOrderToSkip((DbExpression) this._limit, this._sort, k);
        }
      }

      private class ProjectLimitSkipLifter : ExpressionConverter.OrderByLifter.OrderByLifterBase
      {
        private readonly DbProjectExpression _project;
        private readonly DbLimitExpression _limit;
        private readonly DbSkipExpression _skip;
        private readonly DbExpression _source;

        internal ProjectLimitSkipLifter(
          DbProjectExpression project,
          DbLimitExpression limit,
          DbSkipExpression skip,
          AliasGenerator aliasGenerator)
          : base((DbExpression) project, aliasGenerator)
        {
          this._project = project;
          this._limit = limit;
          this._skip = skip;
          this._source = skip.Input.Expression;
        }

        internal override DbExpression Filter(DbFilterExpression filter)
        {
          return (DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.RebindProject((DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.ApplySkipOrderToSort((DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.ComposeFilter((DbExpression) this._skip.Limit(this._limit.Limit), this._project, filter), this._skip), this._project);
        }

        internal override DbExpression Project(DbProjectExpression project)
        {
          return (DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.ComposeProject((DbExpression) this._skip.Limit(this._limit.Limit), this._project, project);
        }

        internal override DbExpression Limit(DbExpression k)
        {
          if (this._limit.Limit.ExpressionKind == DbExpressionKind.Constant && k.ExpressionKind == DbExpressionKind.Constant)
            return (DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.RebindProject((DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.MinimumLimit((DbExpression) this._skip, this._limit, k), this._project);
          return (DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.RebindProject((DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.ApplySkipOrderToSort((DbExpression) this._skip.Limit(this._limit.Limit), this._skip).Limit(k), this._project);
        }

        internal override DbExpression Skip(DbExpression k)
        {
          if (this._skip.Count.ExpressionKind == DbExpressionKind.Constant && this._limit.Limit.ExpressionKind == DbExpressionKind.Constant && k.ExpressionKind == DbExpressionKind.Constant)
            return (DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.RebindProject((DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.SubtractFromLimit((DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.AddToSkip(this._source, this._skip, k), this._limit, k), this._project);
          return (DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.RebindProject((DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.RebindSkip((DbExpression) this._skip.Limit(this._limit.Limit), this._skip, k), this._project);
        }
      }

      private class ProjectLimitSortLifter : ExpressionConverter.OrderByLifter.OrderByLifterBase
      {
        private readonly DbProjectExpression _project;
        private readonly DbLimitExpression _limit;
        private readonly DbSortExpression _sort;

        internal ProjectLimitSortLifter(
          DbProjectExpression project,
          DbLimitExpression limit,
          DbSortExpression sort,
          AliasGenerator aliasGenerator)
          : base((DbExpression) project, aliasGenerator)
        {
          this._project = project;
          this._limit = limit;
          this._sort = sort;
        }

        internal override DbExpression Filter(DbFilterExpression filter)
        {
          return (DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.RebindProject((DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.RebindSort((DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.ComposeFilter((DbExpression) this._sort.Limit(this._limit.Limit), this._project, filter), this._sort), this._project);
        }

        internal override DbExpression Project(DbProjectExpression project)
        {
          return (DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.ComposeProject((DbExpression) this._sort.Limit(this._limit.Limit), this._project, project);
        }

        internal override DbExpression Limit(DbExpression k)
        {
          if (this._limit.Limit.ExpressionKind == DbExpressionKind.Constant && k.ExpressionKind == DbExpressionKind.Constant)
            return (DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.RebindProject((DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.MinimumLimit((DbExpression) this._sort, this._limit, k), this._project);
          return (DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.RebindProject((DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.RebindSort((DbExpression) this._sort.Limit(this._limit.Limit), this._sort).Limit(k), this._project);
        }

        internal override DbExpression Skip(DbExpression k)
        {
          return (DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.RebindProject((DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.ApplySortOrderToSkip((DbExpression) this._sort.Limit(this._limit.Limit), this._sort, k), this._project);
        }
      }

      private class ProjectSkipLifter : ExpressionConverter.OrderByLifter.OrderByLifterBase
      {
        private readonly DbProjectExpression _project;
        private readonly DbSkipExpression _skip;
        private readonly DbExpression _source;

        internal ProjectSkipLifter(
          DbProjectExpression project,
          DbSkipExpression skip,
          AliasGenerator aliasGenerator)
          : base((DbExpression) project, aliasGenerator)
        {
          this._project = project;
          this._skip = skip;
          this._source = this._skip.Input.Expression;
        }

        internal override DbExpression Filter(DbFilterExpression filter)
        {
          return (DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.RebindProject((DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.ApplySkipOrderToSort((DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.ComposeFilter((DbExpression) this._skip, this._project, filter), this._skip), this._project);
        }

        internal override DbExpression Limit(DbExpression k)
        {
          return (DbExpression) this._root.Limit(k);
        }

        internal override DbExpression Project(DbProjectExpression project)
        {
          return (DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.ComposeProject((DbExpression) this._skip, this._project, project);
        }

        internal override DbExpression Skip(DbExpression k)
        {
          if (this._skip.Count.ExpressionKind == DbExpressionKind.Constant && k.ExpressionKind == DbExpressionKind.Constant)
            return (DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.RebindProject((DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.AddToSkip(this._source, this._skip, k), this._project);
          return (DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.RebindProject((DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.RebindSkip((DbExpression) this._skip, this._skip, k), this._project);
        }
      }

      private class SkipLifter : ExpressionConverter.OrderByLifter.OrderByLifterBase
      {
        private readonly DbSkipExpression _skip;
        private readonly DbExpression _source;

        internal SkipLifter(DbSkipExpression skip, AliasGenerator aliasGenerator)
          : base((DbExpression) skip, aliasGenerator)
        {
          this._skip = skip;
          this._source = skip.Input.Expression;
        }

        internal override DbExpression Filter(DbFilterExpression filter)
        {
          return (DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.ApplySkipOrderToSort((DbExpression) filter, this._skip);
        }

        internal override DbExpression Project(DbProjectExpression project)
        {
          return (DbExpression) project;
        }

        internal override DbExpression Limit(DbExpression k)
        {
          return (DbExpression) this._root.Limit(k);
        }

        internal override DbExpression Skip(DbExpression k)
        {
          if (this._skip.Count.ExpressionKind == DbExpressionKind.Constant && k.ExpressionKind == DbExpressionKind.Constant)
            return (DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.AddToSkip(this._source, this._skip, k);
          return (DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.RebindSkip((DbExpression) this._skip, this._skip, k);
        }
      }

      private class ProjectSortLifter : ExpressionConverter.OrderByLifter.OrderByLifterBase
      {
        private readonly DbProjectExpression _project;
        private readonly DbSortExpression _sort;
        private readonly DbExpression _source;

        internal ProjectSortLifter(
          DbProjectExpression project,
          DbSortExpression sort,
          AliasGenerator aliasGenerator)
          : base((DbExpression) project, aliasGenerator)
        {
          this._project = project;
          this._sort = sort;
          this._source = sort.Input.Expression;
        }

        internal override DbExpression Project(DbProjectExpression project)
        {
          return (DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.ComposeProject((DbExpression) this._sort, this._project, project);
        }

        internal override DbExpression Filter(DbFilterExpression filter)
        {
          return (DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.RebindProject((DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.RebindSort((DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.ComposeFilter(this._source, this._project, filter), this._sort), this._project);
        }

        internal override DbExpression Limit(DbExpression k)
        {
          return (DbExpression) this._root.Limit(k);
        }

        internal override DbExpression Skip(DbExpression k)
        {
          return (DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.RebindProject((DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.ApplySortOrderToSkip(this._source, this._sort, k), this._project);
        }
      }

      private class SortLifter : ExpressionConverter.OrderByLifter.OrderByLifterBase
      {
        private readonly DbSortExpression _sort;
        private readonly DbExpression _source;

        internal SortLifter(DbSortExpression sort, AliasGenerator aliasGenerator)
          : base((DbExpression) sort, aliasGenerator)
        {
          this._sort = sort;
          this._source = sort.Input.Expression;
        }

        internal override DbExpression Project(DbProjectExpression project)
        {
          return (DbExpression) project;
        }

        internal override DbExpression Filter(DbFilterExpression filter)
        {
          return (DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.RebindSort((DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.RebindFilter(this._source, filter), this._sort);
        }

        internal override DbExpression Limit(DbExpression k)
        {
          return (DbExpression) this._root.Limit(k);
        }

        internal override DbExpression Skip(DbExpression k)
        {
          return (DbExpression) ExpressionConverter.OrderByLifter.OrderByLifterBase.ApplySortOrderToSkip(this._source, this._sort, k);
        }
      }

      private class PassthroughOrderByLifter : ExpressionConverter.OrderByLifter.OrderByLifterBase
      {
        internal PassthroughOrderByLifter(DbExpression source, AliasGenerator aliasGenerator)
          : base(source, aliasGenerator)
        {
        }

        internal override DbExpression Project(DbProjectExpression project)
        {
          return (DbExpression) project;
        }

        internal override DbExpression Filter(DbFilterExpression filter)
        {
          return (DbExpression) filter;
        }

        internal override DbExpression OfType(TypeUsage type)
        {
          return (DbExpression) this._root.OfType(type);
        }

        internal override DbExpression Limit(DbExpression k)
        {
          return (DbExpression) this._root.Limit(k);
        }

        internal override DbExpression Skip(DbExpression k)
        {
          throw new NotSupportedException(Strings.ELinq_SkipWithoutOrder);
        }
      }
    }

    internal sealed class MemberAccessTranslator : ExpressionConverter.TypedTranslator<MemberExpression>
    {
      private static readonly object _vbInitializerLock = new object();
      private static readonly Dictionary<PropertyInfo, ExpressionConverter.MemberAccessTranslator.PropertyTranslator> _propertyTranslators = new Dictionary<PropertyInfo, ExpressionConverter.MemberAccessTranslator.PropertyTranslator>();
      private static bool _vbPropertiesInitialized;

      internal MemberAccessTranslator()
        : base(ExpressionType.MemberAccess)
      {
      }

      protected override DbExpression TypedTranslate(
        ExpressionConverter parent,
        MemberExpression linq)
      {
        string name;
        Type type;
        MemberInfo clrMember = TypeSystem.PropertyOrField(linq.Member, out name, out type);
        if (linq.Expression != null)
        {
          if (ExpressionType.Constant == linq.Expression.NodeType && ((IEnumerable<object>) linq.Expression.Type.GetCustomAttributes(typeof (CompilerGeneratedAttribute), false)).FirstOrDefault<object>() != null)
          {
            Delegate @delegate = Expression.Lambda((Expression) linq, new ParameterExpression[0]).Compile();
            return parent.TranslateExpression((Expression) Expression.Constant(@delegate.DynamicInvoke()));
          }
          DbExpression instance = parent.TranslateExpression(linq.Expression);
          DbExpression propertyExpression;
          if (ExpressionConverter.MemberAccessTranslator.TryResolveAsProperty(parent, clrMember, instance.ResultType, instance, out propertyExpression))
            return propertyExpression;
        }
        ExpressionConverter.MemberAccessTranslator.PropertyTranslator propertyTranslator;
        if (clrMember.MemberType == MemberTypes.Property && ExpressionConverter.MemberAccessTranslator.TryGetTranslator((PropertyInfo) clrMember, out propertyTranslator))
          return propertyTranslator.Translate(parent, linq);
        throw new NotSupportedException(Strings.ELinq_UnrecognizedMember((object) linq.Member.Name));
      }

      [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Scope = "member", Target = "System.Data.Entity.Core.Objects.ELinq.ExpressionConverter+MemberAccessTranslator.#.cctor()")]
      static MemberAccessTranslator()
      {
        foreach (ExpressionConverter.MemberAccessTranslator.PropertyTranslator propertyTranslator in ExpressionConverter.MemberAccessTranslator.GetPropertyTranslators())
        {
          foreach (PropertyInfo property in propertyTranslator.Properties)
            ExpressionConverter.MemberAccessTranslator._propertyTranslators.Add(property, propertyTranslator);
        }
      }

      private static bool TryGetTranslator(
        PropertyInfo propertyInfo,
        out ExpressionConverter.MemberAccessTranslator.PropertyTranslator propertyTranslator)
      {
        PropertyInfo propertyInfo1 = propertyInfo;
        if (propertyInfo.DeclaringType.IsGenericType())
        {
          try
          {
            propertyInfo = propertyInfo.DeclaringType.GetGenericTypeDefinition().GetDeclaredProperty(propertyInfo.Name);
          }
          catch (AmbiguousMatchException ex)
          {
            propertyTranslator = (ExpressionConverter.MemberAccessTranslator.PropertyTranslator) null;
            return false;
          }
          if (propertyInfo == (PropertyInfo) null)
          {
            propertyTranslator = (ExpressionConverter.MemberAccessTranslator.PropertyTranslator) null;
            return false;
          }
        }
        ExpressionConverter.MemberAccessTranslator.PropertyTranslator propertyTranslator1;
        if (ExpressionConverter.MemberAccessTranslator._propertyTranslators.TryGetValue(propertyInfo, out propertyTranslator1))
        {
          propertyTranslator = propertyTranslator1;
          return true;
        }
        if ("Microsoft.VisualBasic, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" == propertyInfo.DeclaringType.Assembly().FullName)
        {
          lock (ExpressionConverter.MemberAccessTranslator._vbInitializerLock)
          {
            if (!ExpressionConverter.MemberAccessTranslator._vbPropertiesInitialized)
            {
              ExpressionConverter.MemberAccessTranslator.InitializeVBProperties(propertyInfo.DeclaringType.Assembly());
              ExpressionConverter.MemberAccessTranslator._vbPropertiesInitialized = true;
            }
            if (ExpressionConverter.MemberAccessTranslator._propertyTranslators.TryGetValue(propertyInfo, out propertyTranslator1))
            {
              propertyTranslator = propertyTranslator1;
              return true;
            }
            propertyTranslator = (ExpressionConverter.MemberAccessTranslator.PropertyTranslator) null;
            return false;
          }
        }
        else
        {
          if (ExpressionConverter.MemberAccessTranslator.GenericICollectionTranslator.TryGetPropertyTranslator(propertyInfo1, out propertyTranslator))
            return true;
          propertyTranslator = (ExpressionConverter.MemberAccessTranslator.PropertyTranslator) null;
          return false;
        }
      }

      private static bool TryResolveAsProperty(
        ExpressionConverter parent,
        MemberInfo clrMember,
        TypeUsage definingType,
        DbExpression instance,
        out DbExpression propertyExpression)
      {
        RowType edmType1 = definingType.EdmType as RowType;
        string name = clrMember.Name;
        if (edmType1 != null)
        {
          EdmMember edmMember;
          if (edmType1.Members.TryGetValue(name, false, out edmMember))
          {
            propertyExpression = (DbExpression) instance.Property(name);
            return true;
          }
          propertyExpression = (DbExpression) null;
          return false;
        }
        StructuralType edmType2 = definingType.EdmType as StructuralType;
        if (edmType2 != null)
        {
          EdmMember outMember = (EdmMember) null;
          if (parent._perspective.TryGetMember(edmType2, name, false, out outMember) && outMember != null)
          {
            if (outMember.BuiltInTypeKind == BuiltInTypeKind.NavigationProperty)
            {
              NavigationProperty navProp = (NavigationProperty) outMember;
              propertyExpression = ExpressionConverter.MemberAccessTranslator.TranslateNavigationProperty(parent, clrMember, instance, navProp);
              return true;
            }
            propertyExpression = (DbExpression) instance.Property(name);
            return true;
          }
        }
        if (name == "Key" && DbExpressionKind.Property == instance.ExpressionKind)
        {
          DbPropertyExpression propertyExpression1 = (DbPropertyExpression) instance;
          InitializerMetadata initializerMetadata;
          if (propertyExpression1.Property.Name == "Group" && InitializerMetadata.TryGetInitializerMetadata(propertyExpression1.Instance.ResultType, out initializerMetadata) && initializerMetadata.Kind == InitializerMetadataKind.Grouping)
          {
            propertyExpression = (DbExpression) propertyExpression1.Instance.Property("Key");
            return true;
          }
        }
        propertyExpression = (DbExpression) null;
        return false;
      }

      private static DbExpression TranslateNavigationProperty(
        ExpressionConverter parent,
        MemberInfo clrMember,
        DbExpression instance,
        NavigationProperty navProp)
      {
        DbExpression dbExpression = (DbExpression) instance.Property(navProp);
        if (BuiltInTypeKind.CollectionType == dbExpression.ResultType.EdmType.BuiltInTypeKind)
        {
          Type propertyType = ((PropertyInfo) clrMember).PropertyType;
          if (propertyType.IsGenericType() && propertyType.GetGenericTypeDefinition() == typeof (EntityCollection<>))
            dbExpression = (DbExpression) ExpressionConverter.CreateNewRowExpression(new List<KeyValuePair<string, DbExpression>>(2)
            {
              new KeyValuePair<string, DbExpression>("Owner", instance),
              new KeyValuePair<string, DbExpression>("Elements", dbExpression)
            }, InitializerMetadata.CreateEntityCollectionInitializer(parent.EdmItemCollection, propertyType, navProp));
        }
        return dbExpression;
      }

      private static DbExpression TranslateCount(
        ExpressionConverter parent,
        Type sequenceElementType,
        Expression sequence)
      {
        MethodInfo method;
        ReflectionUtil.TryLookupMethod(SequenceMethod.Count, out method);
        Expression linq = (Expression) Expression.Call(method.MakeGenericMethod(sequenceElementType), sequence);
        return parent.TranslateExpression(linq);
      }

      private static void InitializeVBProperties(Assembly vbAssembly)
      {
        foreach (ExpressionConverter.MemberAccessTranslator.PropertyTranslator propertyTranslator in ExpressionConverter.MemberAccessTranslator.GetVisualBasicPropertyTranslators(vbAssembly))
        {
          foreach (PropertyInfo property in propertyTranslator.Properties)
            ExpressionConverter.MemberAccessTranslator._propertyTranslators.Add(property, propertyTranslator);
        }
      }

      private static IEnumerable<ExpressionConverter.MemberAccessTranslator.PropertyTranslator> GetVisualBasicPropertyTranslators(
        Assembly vbAssembly)
      {
        return (IEnumerable<ExpressionConverter.MemberAccessTranslator.PropertyTranslator>) new ExpressionConverter.MemberAccessTranslator.PropertyTranslator[1]
        {
          (ExpressionConverter.MemberAccessTranslator.PropertyTranslator) new ExpressionConverter.MemberAccessTranslator.VBDateAndTimeNowTranslator(vbAssembly)
        };
      }

      private static IEnumerable<ExpressionConverter.MemberAccessTranslator.PropertyTranslator> GetPropertyTranslators()
      {
        return (IEnumerable<ExpressionConverter.MemberAccessTranslator.PropertyTranslator>) new ExpressionConverter.MemberAccessTranslator.PropertyTranslator[6]
        {
          (ExpressionConverter.MemberAccessTranslator.PropertyTranslator) new ExpressionConverter.MemberAccessTranslator.DefaultCanonicalFunctionPropertyTranslator(),
          (ExpressionConverter.MemberAccessTranslator.PropertyTranslator) new ExpressionConverter.MemberAccessTranslator.RenameCanonicalFunctionPropertyTranslator(),
          (ExpressionConverter.MemberAccessTranslator.PropertyTranslator) new ExpressionConverter.MemberAccessTranslator.EntityCollectionCountTranslator(),
          (ExpressionConverter.MemberAccessTranslator.PropertyTranslator) new ExpressionConverter.MemberAccessTranslator.NullableHasValueTranslator(),
          (ExpressionConverter.MemberAccessTranslator.PropertyTranslator) new ExpressionConverter.MemberAccessTranslator.NullableValueTranslator(),
          (ExpressionConverter.MemberAccessTranslator.PropertyTranslator) new ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator()
        };
      }

      internal static bool CanFuncletizePropertyInfo(PropertyInfo propertyInfo)
      {
        ExpressionConverter.MemberAccessTranslator.PropertyTranslator propertyTranslator;
        if (!ExpressionConverter.MemberAccessTranslator.GenericICollectionTranslator.TryGetPropertyTranslator(propertyInfo, out propertyTranslator))
          return !ExpressionConverter.MemberAccessTranslator.TryGetTranslator(propertyInfo, out propertyTranslator);
        return true;
      }

      internal abstract class PropertyTranslator
      {
        private readonly IEnumerable<PropertyInfo> _properties;

        protected PropertyTranslator(params PropertyInfo[] properties)
        {
          this._properties = (IEnumerable<PropertyInfo>) properties;
        }

        protected PropertyTranslator(IEnumerable<PropertyInfo> properties)
        {
          this._properties = properties;
        }

        internal IEnumerable<PropertyInfo> Properties
        {
          get
          {
            return this._properties;
          }
        }

        internal abstract DbExpression Translate(
          ExpressionConverter parent,
          MemberExpression call);

        public override string ToString()
        {
          return this.GetType().Name;
        }
      }

      private sealed class SpatialPropertyTranslator : ExpressionConverter.MemberAccessTranslator.PropertyTranslator
      {
        private readonly Dictionary<PropertyInfo, string> propertyFunctionRenames = ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetRenamedPropertyFunctions();

        internal SpatialPropertyTranslator()
          : base(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetSupportedProperties())
        {
        }

        private static PropertyInfo GetProperty<T, TResult>(
          Expression<Func<T, TResult>> lambda)
        {
          return (PropertyInfo) ((MemberExpression) lambda.Body).Member;
        }

        private static IEnumerable<PropertyInfo> GetSupportedProperties()
        {
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeography, int>((Expression<Func<DbGeography, int>>) (geo => geo.CoordinateSystemId));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeography, string>((Expression<Func<DbGeography, string>>) (geo => geo.SpatialTypeName));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeography, int>((Expression<Func<DbGeography, int>>) (geo => geo.Dimension));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeography, bool>((Expression<Func<DbGeography, bool>>) (geo => geo.IsEmpty));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeography, int?>((Expression<Func<DbGeography, int?>>) (geo => geo.ElementCount));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeography, double?>((Expression<Func<DbGeography, double?>>) (geo => geo.Latitude));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeography, double?>((Expression<Func<DbGeography, double?>>) (geo => geo.Longitude));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeography, double?>((Expression<Func<DbGeography, double?>>) (geo => geo.Elevation));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeography, double?>((Expression<Func<DbGeography, double?>>) (geo => geo.Measure));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeography, double?>((Expression<Func<DbGeography, double?>>) (geo => geo.Length));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeography, DbGeography>((Expression<Func<DbGeography, DbGeography>>) (geo => geo.StartPoint));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeography, DbGeography>((Expression<Func<DbGeography, DbGeography>>) (geo => geo.EndPoint));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeography, bool?>((Expression<Func<DbGeography, bool?>>) (geo => geo.IsClosed));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeography, int?>((Expression<Func<DbGeography, int?>>) (geo => geo.PointCount));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeography, double?>((Expression<Func<DbGeography, double?>>) (geo => geo.Area));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, int>((Expression<Func<DbGeometry, int>>) (geo => geo.CoordinateSystemId));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, string>((Expression<Func<DbGeometry, string>>) (geo => geo.SpatialTypeName));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, int>((Expression<Func<DbGeometry, int>>) (geo => geo.Dimension));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, DbGeometry>((Expression<Func<DbGeometry, DbGeometry>>) (geo => geo.Envelope));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, bool>((Expression<Func<DbGeometry, bool>>) (geo => geo.IsEmpty));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, bool>((Expression<Func<DbGeometry, bool>>) (geo => geo.IsSimple));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, DbGeometry>((Expression<Func<DbGeometry, DbGeometry>>) (geo => geo.Boundary));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, bool>((Expression<Func<DbGeometry, bool>>) (geo => geo.IsValid));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, DbGeometry>((Expression<Func<DbGeometry, DbGeometry>>) (geo => geo.ConvexHull));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, int?>((Expression<Func<DbGeometry, int?>>) (geo => geo.ElementCount));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, double?>((Expression<Func<DbGeometry, double?>>) (geo => geo.XCoordinate));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, double?>((Expression<Func<DbGeometry, double?>>) (geo => geo.YCoordinate));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, double?>((Expression<Func<DbGeometry, double?>>) (geo => geo.Elevation));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, double?>((Expression<Func<DbGeometry, double?>>) (geo => geo.Measure));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, double?>((Expression<Func<DbGeometry, double?>>) (geo => geo.Length));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, DbGeometry>((Expression<Func<DbGeometry, DbGeometry>>) (geo => geo.StartPoint));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, DbGeometry>((Expression<Func<DbGeometry, DbGeometry>>) (geo => geo.EndPoint));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, bool?>((Expression<Func<DbGeometry, bool?>>) (geo => geo.IsClosed));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, bool?>((Expression<Func<DbGeometry, bool?>>) (geo => geo.IsRing));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, int?>((Expression<Func<DbGeometry, int?>>) (geo => geo.PointCount));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, double?>((Expression<Func<DbGeometry, double?>>) (geo => geo.Area));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, DbGeometry>((Expression<Func<DbGeometry, DbGeometry>>) (geo => geo.Centroid));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, DbGeometry>((Expression<Func<DbGeometry, DbGeometry>>) (geo => geo.PointOnSurface));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, DbGeometry>((Expression<Func<DbGeometry, DbGeometry>>) (geo => geo.ExteriorRing));
          yield return ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, int?>((Expression<Func<DbGeometry, int?>>) (geo => geo.InteriorRingCount));
        }

        private static Dictionary<PropertyInfo, string> GetRenamedPropertyFunctions()
        {
          Dictionary<PropertyInfo, string> dictionary = new Dictionary<PropertyInfo, string>();
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeography, int>((Expression<Func<DbGeography, int>>) (geo => geo.CoordinateSystemId)), "CoordinateSystemId");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeography, string>((Expression<Func<DbGeography, string>>) (geo => geo.SpatialTypeName)), "SpatialTypeName");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeography, int>((Expression<Func<DbGeography, int>>) (geo => geo.Dimension)), "SpatialDimension");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeography, bool>((Expression<Func<DbGeography, bool>>) (geo => geo.IsEmpty)), "IsEmptySpatial");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeography, int?>((Expression<Func<DbGeography, int?>>) (geo => geo.ElementCount)), "SpatialElementCount");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeography, double?>((Expression<Func<DbGeography, double?>>) (geo => geo.Latitude)), "Latitude");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeography, double?>((Expression<Func<DbGeography, double?>>) (geo => geo.Longitude)), "Longitude");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeography, double?>((Expression<Func<DbGeography, double?>>) (geo => geo.Elevation)), "Elevation");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeography, double?>((Expression<Func<DbGeography, double?>>) (geo => geo.Measure)), "Measure");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeography, double?>((Expression<Func<DbGeography, double?>>) (geo => geo.Length)), "SpatialLength");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeography, DbGeography>((Expression<Func<DbGeography, DbGeography>>) (geo => geo.StartPoint)), "StartPoint");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeography, DbGeography>((Expression<Func<DbGeography, DbGeography>>) (geo => geo.EndPoint)), "EndPoint");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeography, bool?>((Expression<Func<DbGeography, bool?>>) (geo => geo.IsClosed)), "IsClosedSpatial");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeography, int?>((Expression<Func<DbGeography, int?>>) (geo => geo.PointCount)), "PointCount");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeography, double?>((Expression<Func<DbGeography, double?>>) (geo => geo.Area)), "Area");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, int>((Expression<Func<DbGeometry, int>>) (geo => geo.CoordinateSystemId)), "CoordinateSystemId");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, string>((Expression<Func<DbGeometry, string>>) (geo => geo.SpatialTypeName)), "SpatialTypeName");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, int>((Expression<Func<DbGeometry, int>>) (geo => geo.Dimension)), "SpatialDimension");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, DbGeometry>((Expression<Func<DbGeometry, DbGeometry>>) (geo => geo.Envelope)), "SpatialEnvelope");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, bool>((Expression<Func<DbGeometry, bool>>) (geo => geo.IsEmpty)), "IsEmptySpatial");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, bool>((Expression<Func<DbGeometry, bool>>) (geo => geo.IsSimple)), "IsSimpleGeometry");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, DbGeometry>((Expression<Func<DbGeometry, DbGeometry>>) (geo => geo.Boundary)), "SpatialBoundary");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, bool>((Expression<Func<DbGeometry, bool>>) (geo => geo.IsValid)), "IsValidGeometry");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, DbGeometry>((Expression<Func<DbGeometry, DbGeometry>>) (geo => geo.ConvexHull)), "SpatialConvexHull");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, int?>((Expression<Func<DbGeometry, int?>>) (geo => geo.ElementCount)), "SpatialElementCount");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, double?>((Expression<Func<DbGeometry, double?>>) (geo => geo.XCoordinate)), "XCoordinate");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, double?>((Expression<Func<DbGeometry, double?>>) (geo => geo.YCoordinate)), "YCoordinate");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, double?>((Expression<Func<DbGeometry, double?>>) (geo => geo.Elevation)), "Elevation");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, double?>((Expression<Func<DbGeometry, double?>>) (geo => geo.Measure)), "Measure");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, double?>((Expression<Func<DbGeometry, double?>>) (geo => geo.Length)), "SpatialLength");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, DbGeometry>((Expression<Func<DbGeometry, DbGeometry>>) (geo => geo.StartPoint)), "StartPoint");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, DbGeometry>((Expression<Func<DbGeometry, DbGeometry>>) (geo => geo.EndPoint)), "EndPoint");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, bool?>((Expression<Func<DbGeometry, bool?>>) (geo => geo.IsClosed)), "IsClosedSpatial");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, bool?>((Expression<Func<DbGeometry, bool?>>) (geo => geo.IsRing)), "IsRing");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, int?>((Expression<Func<DbGeometry, int?>>) (geo => geo.PointCount)), "PointCount");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, double?>((Expression<Func<DbGeometry, double?>>) (geo => geo.Area)), "Area");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, DbGeometry>((Expression<Func<DbGeometry, DbGeometry>>) (geo => geo.Centroid)), "Centroid");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, DbGeometry>((Expression<Func<DbGeometry, DbGeometry>>) (geo => geo.PointOnSurface)), "PointOnSurface");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, DbGeometry>((Expression<Func<DbGeometry, DbGeometry>>) (geo => geo.ExteriorRing)), "ExteriorRing");
          dictionary.Add(ExpressionConverter.MemberAccessTranslator.SpatialPropertyTranslator.GetProperty<DbGeometry, int?>((Expression<Func<DbGeometry, int?>>) (geo => geo.InteriorRingCount)), "InteriorRingCount");
          return dictionary;
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MemberExpression call)
        {
          PropertyInfo member = (PropertyInfo) call.Member;
          string functionName;
          if (!this.propertyFunctionRenames.TryGetValue(member, out functionName))
            functionName = "ST" + member.Name;
          return (DbExpression) parent.TranslateIntoCanonicalFunction(functionName, (Expression) call, call.Expression);
        }
      }

      private sealed class GenericICollectionTranslator : ExpressionConverter.MemberAccessTranslator.PropertyTranslator
      {
        private readonly Type _elementType;

        private GenericICollectionTranslator(Type elementType)
          : base(Enumerable.Empty<PropertyInfo>())
        {
          this._elementType = elementType;
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MemberExpression call)
        {
          return ExpressionConverter.MemberAccessTranslator.TranslateCount(parent, this._elementType, call.Expression);
        }

        internal static bool TryGetPropertyTranslator(
          PropertyInfo propertyInfo,
          out ExpressionConverter.MemberAccessTranslator.PropertyTranslator propertyTranslator)
        {
          if (propertyInfo.Name == "Count" && propertyInfo.PropertyType.Equals(typeof (int)))
          {
            foreach (KeyValuePair<Type, Type> implementedIcollection in ExpressionConverter.MemberAccessTranslator.GenericICollectionTranslator.GetImplementedICollections(propertyInfo.DeclaringType))
            {
              Type key = implementedIcollection.Key;
              Type elementType = implementedIcollection.Value;
              if (propertyInfo.IsImplementationOf(key))
              {
                propertyTranslator = (ExpressionConverter.MemberAccessTranslator.PropertyTranslator) new ExpressionConverter.MemberAccessTranslator.GenericICollectionTranslator(elementType);
                return true;
              }
            }
          }
          propertyTranslator = (ExpressionConverter.MemberAccessTranslator.PropertyTranslator) null;
          return false;
        }

        private static bool IsICollection(Type candidateType, out Type elementType)
        {
          if (candidateType.IsGenericType() && candidateType.GetGenericTypeDefinition().Equals(typeof (ICollection<>)))
          {
            elementType = candidateType.GetGenericArguments()[0];
            return true;
          }
          elementType = (Type) null;
          return false;
        }

        private static IEnumerable<KeyValuePair<Type, Type>> GetImplementedICollections(
          Type type)
        {
          Type collectionElementType;
          if (ExpressionConverter.MemberAccessTranslator.GenericICollectionTranslator.IsICollection(type, out collectionElementType))
          {
            yield return new KeyValuePair<Type, Type>(type, collectionElementType);
          }
          else
          {
            foreach (Type type1 in type.GetInterfaces())
            {
              if (ExpressionConverter.MemberAccessTranslator.GenericICollectionTranslator.IsICollection(type1, out collectionElementType))
                yield return new KeyValuePair<Type, Type>(type1, collectionElementType);
            }
          }
        }
      }

      internal sealed class DefaultCanonicalFunctionPropertyTranslator : ExpressionConverter.MemberAccessTranslator.PropertyTranslator
      {
        internal DefaultCanonicalFunctionPropertyTranslator()
          : base(ExpressionConverter.MemberAccessTranslator.DefaultCanonicalFunctionPropertyTranslator.GetProperties())
        {
        }

        private static IEnumerable<PropertyInfo> GetProperties()
        {
          return (IEnumerable<PropertyInfo>) new PropertyInfo[15]
          {
            typeof (string).GetDeclaredProperty("Length"),
            typeof (DateTime).GetDeclaredProperty("Year"),
            typeof (DateTime).GetDeclaredProperty("Month"),
            typeof (DateTime).GetDeclaredProperty("Day"),
            typeof (DateTime).GetDeclaredProperty("Hour"),
            typeof (DateTime).GetDeclaredProperty("Minute"),
            typeof (DateTime).GetDeclaredProperty("Second"),
            typeof (DateTime).GetDeclaredProperty("Millisecond"),
            typeof (DateTimeOffset).GetDeclaredProperty("Year"),
            typeof (DateTimeOffset).GetDeclaredProperty("Month"),
            typeof (DateTimeOffset).GetDeclaredProperty("Day"),
            typeof (DateTimeOffset).GetDeclaredProperty("Hour"),
            typeof (DateTimeOffset).GetDeclaredProperty("Minute"),
            typeof (DateTimeOffset).GetDeclaredProperty("Second"),
            typeof (DateTimeOffset).GetDeclaredProperty("Millisecond")
          };
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MemberExpression call)
        {
          return (DbExpression) parent.TranslateIntoCanonicalFunction(call.Member.Name, (Expression) call, call.Expression);
        }
      }

      internal sealed class RenameCanonicalFunctionPropertyTranslator : ExpressionConverter.MemberAccessTranslator.PropertyTranslator
      {
        private static readonly Dictionary<PropertyInfo, string> _propertyRenameMap = new Dictionary<PropertyInfo, string>(2);

        internal RenameCanonicalFunctionPropertyTranslator()
          : base(ExpressionConverter.MemberAccessTranslator.RenameCanonicalFunctionPropertyTranslator.GetProperties())
        {
        }

        private static IEnumerable<PropertyInfo> GetProperties()
        {
          return (IEnumerable<PropertyInfo>) new PropertyInfo[7]
          {
            ExpressionConverter.MemberAccessTranslator.RenameCanonicalFunctionPropertyTranslator.GetProperty(typeof (DateTime), "Now", "CurrentDateTime"),
            ExpressionConverter.MemberAccessTranslator.RenameCanonicalFunctionPropertyTranslator.GetProperty(typeof (DateTime), "UtcNow", "CurrentUtcDateTime"),
            ExpressionConverter.MemberAccessTranslator.RenameCanonicalFunctionPropertyTranslator.GetProperty(typeof (DateTimeOffset), "Now", "CurrentDateTimeOffset"),
            ExpressionConverter.MemberAccessTranslator.RenameCanonicalFunctionPropertyTranslator.GetProperty(typeof (TimeSpan), "Hours", "Hour"),
            ExpressionConverter.MemberAccessTranslator.RenameCanonicalFunctionPropertyTranslator.GetProperty(typeof (TimeSpan), "Minutes", "Minute"),
            ExpressionConverter.MemberAccessTranslator.RenameCanonicalFunctionPropertyTranslator.GetProperty(typeof (TimeSpan), "Seconds", "Second"),
            ExpressionConverter.MemberAccessTranslator.RenameCanonicalFunctionPropertyTranslator.GetProperty(typeof (TimeSpan), "Milliseconds", "Millisecond")
          };
        }

        private static PropertyInfo GetProperty(
          Type declaringType,
          string propertyName,
          string canonicalFunctionName)
        {
          PropertyInfo declaredProperty = declaringType.GetDeclaredProperty(propertyName);
          ExpressionConverter.MemberAccessTranslator.RenameCanonicalFunctionPropertyTranslator._propertyRenameMap[declaredProperty] = canonicalFunctionName;
          return declaredProperty;
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MemberExpression call)
        {
          PropertyInfo member = (PropertyInfo) call.Member;
          string propertyRename = ExpressionConverter.MemberAccessTranslator.RenameCanonicalFunctionPropertyTranslator._propertyRenameMap[member];
          DbExpression dbExpression;
          if (call.Expression == null)
            dbExpression = (DbExpression) parent.TranslateIntoCanonicalFunction(propertyRename, (Expression) call);
          else
            dbExpression = (DbExpression) parent.TranslateIntoCanonicalFunction(propertyRename, (Expression) call, call.Expression);
          return dbExpression;
        }
      }

      internal sealed class VBDateAndTimeNowTranslator : ExpressionConverter.MemberAccessTranslator.PropertyTranslator
      {
        private const string s_dateAndTimeTypeFullName = "Microsoft.VisualBasic.DateAndTime";

        internal VBDateAndTimeNowTranslator(Assembly vbAssembly)
          : base(ExpressionConverter.MemberAccessTranslator.VBDateAndTimeNowTranslator.GetProperty(vbAssembly))
        {
        }

        private static PropertyInfo GetProperty(Assembly vbAssembly)
        {
          return vbAssembly.GetType("Microsoft.VisualBasic.DateAndTime").GetDeclaredProperty("Now");
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MemberExpression call)
        {
          return (DbExpression) parent.TranslateIntoCanonicalFunction("CurrentDateTime", (Expression) call);
        }
      }

      internal sealed class EntityCollectionCountTranslator : ExpressionConverter.MemberAccessTranslator.PropertyTranslator
      {
        internal EntityCollectionCountTranslator()
          : base(ExpressionConverter.MemberAccessTranslator.EntityCollectionCountTranslator.GetProperty())
        {
        }

        private static PropertyInfo GetProperty()
        {
          return typeof (EntityCollection<>).GetDeclaredProperty("Count");
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MemberExpression call)
        {
          return ExpressionConverter.MemberAccessTranslator.TranslateCount(parent, call.Member.DeclaringType.GetGenericArguments()[0], call.Expression);
        }
      }

      internal sealed class NullableHasValueTranslator : ExpressionConverter.MemberAccessTranslator.PropertyTranslator
      {
        internal NullableHasValueTranslator()
          : base(ExpressionConverter.MemberAccessTranslator.NullableHasValueTranslator.GetProperty())
        {
        }

        private static PropertyInfo GetProperty()
        {
          return typeof (Nullable<>).GetDeclaredProperty("HasValue");
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MemberExpression call)
        {
          return (DbExpression) ExpressionConverter.CreateIsNullExpression(parent.TranslateExpression(call.Expression), call.Expression.Type).Not();
        }
      }

      internal sealed class NullableValueTranslator : ExpressionConverter.MemberAccessTranslator.PropertyTranslator
      {
        internal NullableValueTranslator()
          : base(ExpressionConverter.MemberAccessTranslator.NullableValueTranslator.GetProperty())
        {
        }

        private static PropertyInfo GetProperty()
        {
          return typeof (Nullable<>).GetDeclaredProperty("Value");
        }

        internal override DbExpression Translate(
          ExpressionConverter parent,
          MemberExpression call)
        {
          return parent.TranslateExpression(call.Expression);
        }
      }
    }

    private sealed class ConstantTranslator : ExpressionConverter.TypedTranslator<ConstantExpression>
    {
      internal ConstantTranslator()
        : base(ExpressionType.Constant)
      {
      }

      protected override DbExpression TypedTranslate(
        ExpressionConverter parent,
        ConstantExpression linq)
      {
        if (linq == parent._funcletizer.RootContextExpression)
          throw new InvalidOperationException(Strings.ELinq_UnsupportedUseOfContextParameter((object) parent._funcletizer.RootContextParameter.Name));
        ObjectQuery objectQuery = (linq.Value as IQueryable).TryGetObjectQuery();
        if (objectQuery != null)
          return parent.TranslateInlineQueryOfT(objectQuery);
        IEnumerable enumerable = linq.Value as IEnumerable;
        if (enumerable != null)
        {
          Type elementType = TypeSystem.GetElementType(linq.Type);
          if (elementType != (Type) null && elementType != linq.Type)
          {
            List<Expression> expressionList = new List<Expression>();
            foreach (object obj in enumerable)
              expressionList.Add((Expression) Expression.Constant(obj, elementType));
            parent._recompileRequired = (Func<bool>) (() => true);
            return parent.TranslateExpression((Expression) Expression.NewArrayInit(elementType, (IEnumerable<Expression>) expressionList));
          }
        }
        bool flag1 = null == linq.Value;
        bool flag2 = false;
        Type type1 = linq.Type;
        if (type1 == typeof (Enum))
          type1 = linq.Value.GetType();
        TypeUsage type2;
        if (parent.TryGetValueLayerType(type1, out type2) && (Helper.IsScalarType(type2.EdmType) || flag1 && Helper.IsEntityType(type2.EdmType)))
          flag2 = true;
        if (!flag2)
        {
          if (flag1)
            throw new NotSupportedException(Strings.ELinq_UnsupportedNullConstant((object) ExpressionConverter.DescribeClrType(linq.Type)));
          throw new NotSupportedException(Strings.ELinq_UnsupportedConstant((object) ExpressionConverter.DescribeClrType(linq.Type)));
        }
        if (flag1)
          return (DbExpression) type2.Null();
        object obj1 = linq.Value;
        if (Helper.IsPrimitiveType(type2.EdmType))
        {
          Type nonNullableType = TypeSystem.GetNonNullableType(type1);
          if (nonNullableType.IsEnum())
            obj1 = Convert.ChangeType(linq.Value, nonNullableType.GetEnumUnderlyingType(), (IFormatProvider) CultureInfo.InvariantCulture);
        }
        return (DbExpression) type2.Constant(obj1);
      }
    }

    private sealed class ParameterTranslator : ExpressionConverter.TypedTranslator<ParameterExpression>
    {
      internal ParameterTranslator()
        : base(ExpressionType.Parameter)
      {
      }

      protected override DbExpression TypedTranslate(
        ExpressionConverter parent,
        ParameterExpression linq)
      {
        throw new InvalidOperationException(Strings.ELinq_UnboundParameterExpression((object) linq.Name));
      }
    }

    private sealed class NewTranslator : ExpressionConverter.TypedTranslator<NewExpression>
    {
      internal NewTranslator()
        : base(ExpressionType.New)
      {
      }

      protected override DbExpression TypedTranslate(
        ExpressionConverter parent,
        NewExpression linq)
      {
        int num = linq.Members == null ? 0 : linq.Members.Count;
        if ((ConstructorInfo) null == linq.Constructor || linq.Arguments.Count != num)
          throw new NotSupportedException(Strings.ELinq_UnsupportedConstructor);
        parent.CheckInitializerType(linq.Type);
        List<KeyValuePair<string, DbExpression>> columns = new List<KeyValuePair<string, DbExpression>>(num + 1);
        HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.Ordinal);
        for (int index = 0; index < num; ++index)
        {
          string name;
          Type type;
          TypeSystem.PropertyOrField(linq.Members[index], out name, out type);
          DbExpression dbExpression = parent.TranslateExpression(linq.Arguments[index]);
          stringSet.Add(name);
          columns.Add(new KeyValuePair<string, DbExpression>(name, dbExpression));
        }
        InitializerMetadata projectionInitializer;
        if (num == 0)
        {
          columns.Add(DbExpressionBuilder.True.As("Key"));
          projectionInitializer = InitializerMetadata.CreateEmptyProjectionInitializer(parent.EdmItemCollection, linq);
        }
        else
          projectionInitializer = InitializerMetadata.CreateProjectionInitializer(parent.EdmItemCollection, linq);
        parent.ValidateInitializerMetadata(projectionInitializer);
        return (DbExpression) ExpressionConverter.CreateNewRowExpression(columns, projectionInitializer);
      }
    }

    private sealed class NewArrayInitTranslator : ExpressionConverter.TypedTranslator<NewArrayExpression>
    {
      internal NewArrayInitTranslator()
        : base(ExpressionType.NewArrayInit)
      {
      }

      protected override DbExpression TypedTranslate(
        ExpressionConverter parent,
        NewArrayExpression linq)
      {
        if (linq.Expressions.Count > 0)
          return (DbExpression) DbExpressionBuilder.NewCollection(linq.Expressions.Select<Expression, DbExpression>((Func<Expression, DbExpression>) (e => parent.TranslateExpression(e))));
        if (typeof (byte[]) == linq.Type)
        {
          TypeUsage type;
          if (parent.TryGetValueLayerType(typeof (byte), out type))
            return (DbExpression) TypeHelpers.CreateCollectionTypeUsage(type).NewEmptyCollection();
        }
        else
        {
          TypeUsage type;
          if (parent.TryGetValueLayerType(linq.Type, out type))
            return (DbExpression) type.NewEmptyCollection();
        }
        throw new NotSupportedException(Strings.ELinq_UnsupportedType((object) ExpressionConverter.DescribeClrType(linq.Type)));
      }
    }

    private sealed class ListInitTranslator : ExpressionConverter.TypedTranslator<ListInitExpression>
    {
      internal ListInitTranslator()
        : base(ExpressionType.ListInit)
      {
      }

      protected override DbExpression TypedTranslate(
        ExpressionConverter parent,
        ListInitExpression linq)
      {
        if (linq.NewExpression.Constructor != (ConstructorInfo) null && linq.NewExpression.Constructor.GetParameters().Length != 0)
          throw new NotSupportedException(Strings.ELinq_UnsupportedConstructor);
        if (linq.Initializers.Any<ElementInit>((Func<ElementInit, bool>) (i => i.Arguments.Count != 1)))
          throw new NotSupportedException(Strings.ELinq_UnsupportedInitializers);
        return (DbExpression) DbExpressionBuilder.NewCollection(linq.Initializers.Select<ElementInit, DbExpression>((Func<ElementInit, DbExpression>) (i => parent.TranslateExpression(i.Arguments[0]))));
      }
    }

    private sealed class MemberInitTranslator : ExpressionConverter.TypedTranslator<MemberInitExpression>
    {
      internal MemberInitTranslator()
        : base(ExpressionType.MemberInit)
      {
      }

      protected override DbExpression TypedTranslate(
        ExpressionConverter parent,
        MemberInitExpression linq)
      {
        if ((ConstructorInfo) null == linq.NewExpression.Constructor || linq.NewExpression.Constructor.GetParameters().Length != 0)
          throw new NotSupportedException(Strings.ELinq_UnsupportedConstructor);
        parent.CheckInitializerType(linq.Type);
        List<KeyValuePair<string, DbExpression>> columns = new List<KeyValuePair<string, DbExpression>>(linq.Bindings.Count + 1);
        MemberInfo[] memberInfoArray = new MemberInfo[linq.Bindings.Count];
        HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.Ordinal);
        for (int index = 0; index < linq.Bindings.Count; ++index)
        {
          MemberAssignment binding = linq.Bindings[index] as MemberAssignment;
          if (binding == null)
            throw new NotSupportedException(Strings.ELinq_UnsupportedBinding);
          string name;
          Type type;
          MemberInfo memberInfo = TypeSystem.PropertyOrField(binding.Member, out name, out type);
          DbExpression dbExpression = parent.TranslateExpression(binding.Expression);
          stringSet.Add(name);
          memberInfoArray[index] = memberInfo;
          columns.Add(new KeyValuePair<string, DbExpression>(name, dbExpression));
        }
        InitializerMetadata projectionInitializer;
        if (columns.Count == 0)
        {
          columns.Add(DbExpressionBuilder.Constant((object) true).As("Key"));
          projectionInitializer = InitializerMetadata.CreateEmptyProjectionInitializer(parent.EdmItemCollection, linq.NewExpression);
        }
        else
          projectionInitializer = InitializerMetadata.CreateProjectionInitializer(parent.EdmItemCollection, linq);
        parent.ValidateInitializerMetadata(projectionInitializer);
        return (DbExpression) ExpressionConverter.CreateNewRowExpression(columns, projectionInitializer);
      }
    }

    private sealed class ConditionalTranslator : ExpressionConverter.TypedTranslator<ConditionalExpression>
    {
      internal ConditionalTranslator()
        : base(ExpressionType.Conditional)
      {
      }

      protected override DbExpression TypedTranslate(
        ExpressionConverter parent,
        ConditionalExpression linq)
      {
        DbExpression dbExpression1 = parent.TranslateExpression(linq.Test);
        DbExpression dbExpression2;
        DbExpression elseExpression;
        if (!linq.IfTrue.IsNullConstant())
        {
          dbExpression2 = parent.TranslateExpression(linq.IfTrue);
          elseExpression = !linq.IfFalse.IsNullConstant() ? parent.TranslateExpression(linq.IfFalse) : (DbExpression) dbExpression2.ResultType.Null();
        }
        else
        {
          if (linq.IfFalse.IsNullConstant())
            throw new NotSupportedException(Strings.ELinq_UnsupportedNullConstant((object) ExpressionConverter.DescribeClrType(linq.Type)));
          elseExpression = parent.TranslateExpression(linq.IfFalse);
          dbExpression2 = (DbExpression) elseExpression.ResultType.Null();
        }
        return (DbExpression) DbExpressionBuilder.Case((IEnumerable<DbExpression>) new List<DbExpression>()
        {
          dbExpression1
        }, (IEnumerable<DbExpression>) new List<DbExpression>()
        {
          dbExpression2
        }, elseExpression);
      }
    }

    private sealed class NotSupportedTranslator : ExpressionConverter.Translator
    {
      internal NotSupportedTranslator(params ExpressionType[] nodeTypes)
        : base(nodeTypes)
      {
      }

      internal override DbExpression Translate(
        ExpressionConverter parent,
        Expression linq)
      {
        throw new NotSupportedException(Strings.ELinq_UnsupportedExpressionType((object) linq.NodeType));
      }
    }

    private sealed class ExtensionTranslator : ExpressionConverter.Translator
    {
      internal ExtensionTranslator()
        : base(~ExpressionType.Add)
      {
      }

      internal override DbExpression Translate(
        ExpressionConverter parent,
        Expression linq)
      {
        QueryParameterExpression queryParameter = linq as QueryParameterExpression;
        if (queryParameter == null)
          throw new NotSupportedException(Strings.ELinq_UnsupportedExpressionType((object) linq.NodeType));
        parent.AddParameter(queryParameter);
        return (DbExpression) queryParameter.ParameterReference;
      }
    }

    private abstract class BinaryTranslator : ExpressionConverter.TypedTranslator<BinaryExpression>
    {
      protected BinaryTranslator(params ExpressionType[] nodeTypes)
        : base(nodeTypes)
      {
      }

      protected override DbExpression TypedTranslate(
        ExpressionConverter parent,
        BinaryExpression linq)
      {
        return this.TranslateBinary(parent, parent.TranslateExpression(linq.Left), parent.TranslateExpression(linq.Right), linq);
      }

      protected abstract DbExpression TranslateBinary(
        ExpressionConverter parent,
        DbExpression left,
        DbExpression right,
        BinaryExpression linq);
    }

    private sealed class CoalesceTranslator : ExpressionConverter.BinaryTranslator
    {
      internal CoalesceTranslator()
        : base(ExpressionType.Coalesce)
      {
      }

      protected override DbExpression TranslateBinary(
        ExpressionConverter parent,
        DbExpression left,
        DbExpression right,
        BinaryExpression linq)
      {
        return (DbExpression) DbExpressionBuilder.Case((IEnumerable<DbExpression>) new List<DbExpression>(1)
        {
          ExpressionConverter.CreateIsNullExpression(left, linq.Left.Type)
        }, (IEnumerable<DbExpression>) new List<DbExpression>(1)
        {
          right
        }, left);
      }
    }

    private sealed class AndAlsoTranslator : ExpressionConverter.BinaryTranslator
    {
      internal AndAlsoTranslator()
        : base(ExpressionType.AndAlso)
      {
      }

      protected override DbExpression TranslateBinary(
        ExpressionConverter parent,
        DbExpression left,
        DbExpression right,
        BinaryExpression linq)
      {
        return (DbExpression) left.And(right);
      }
    }

    private sealed class OrElseTranslator : ExpressionConverter.BinaryTranslator
    {
      internal OrElseTranslator()
        : base(ExpressionType.OrElse)
      {
      }

      protected override DbExpression TranslateBinary(
        ExpressionConverter parent,
        DbExpression left,
        DbExpression right,
        BinaryExpression linq)
      {
        return (DbExpression) left.Or(right);
      }
    }

    private sealed class LessThanTranslator : ExpressionConverter.BinaryTranslator
    {
      internal LessThanTranslator()
        : base(ExpressionType.LessThan)
      {
      }

      protected override DbExpression TranslateBinary(
        ExpressionConverter parent,
        DbExpression left,
        DbExpression right,
        BinaryExpression linq)
      {
        return (DbExpression) left.LessThan(right);
      }
    }

    private sealed class LessThanOrEqualsTranslator : ExpressionConverter.BinaryTranslator
    {
      internal LessThanOrEqualsTranslator()
        : base(ExpressionType.LessThanOrEqual)
      {
      }

      protected override DbExpression TranslateBinary(
        ExpressionConverter parent,
        DbExpression left,
        DbExpression right,
        BinaryExpression linq)
      {
        return (DbExpression) left.LessThanOrEqual(right);
      }
    }

    private sealed class GreaterThanTranslator : ExpressionConverter.BinaryTranslator
    {
      internal GreaterThanTranslator()
        : base(ExpressionType.GreaterThan)
      {
      }

      protected override DbExpression TranslateBinary(
        ExpressionConverter parent,
        DbExpression left,
        DbExpression right,
        BinaryExpression linq)
      {
        return (DbExpression) left.GreaterThan(right);
      }
    }

    private sealed class GreaterThanOrEqualsTranslator : ExpressionConverter.BinaryTranslator
    {
      internal GreaterThanOrEqualsTranslator()
        : base(ExpressionType.GreaterThanOrEqual)
      {
      }

      protected override DbExpression TranslateBinary(
        ExpressionConverter parent,
        DbExpression left,
        DbExpression right,
        BinaryExpression linq)
      {
        return (DbExpression) left.GreaterThanOrEqual(right);
      }
    }

    private sealed class EqualsTranslator : ExpressionConverter.TypedTranslator<BinaryExpression>
    {
      internal EqualsTranslator()
        : base(ExpressionType.Equal)
      {
      }

      protected override DbExpression TypedTranslate(
        ExpressionConverter parent,
        BinaryExpression linq)
      {
        Expression left1 = linq.Left;
        Expression right1 = linq.Right;
        bool flag1 = left1.IsNullConstant();
        bool flag2 = right1.IsNullConstant();
        if (flag1 && flag2)
          return (DbExpression) DbExpressionBuilder.True;
        if (flag1)
          return ExpressionConverter.EqualsTranslator.CreateIsNullExpression(parent, right1);
        if (flag2)
          return ExpressionConverter.EqualsTranslator.CreateIsNullExpression(parent, left1);
        DbExpression left2 = parent.TranslateExpression(left1);
        DbExpression right2 = parent.TranslateExpression(right1);
        ExpressionConverter.EqualsPattern pattern = ExpressionConverter.EqualsPattern.Store;
        if (parent._funcletizer.RootContext.ContextOptions.UseCSharpNullComparisonBehavior)
          pattern = ExpressionConverter.EqualsPattern.PositiveNullEqualityComposable;
        return parent.CreateEqualsExpression(left2, right2, pattern, left1.Type, right1.Type);
      }

      private static DbExpression CreateIsNullExpression(
        ExpressionConverter parent,
        Expression input)
      {
        input = input.RemoveConvert();
        return ExpressionConverter.CreateIsNullExpression(parent.TranslateExpression(input), input.Type);
      }
    }

    private sealed class NotEqualsTranslator : ExpressionConverter.TypedTranslator<BinaryExpression>
    {
      internal NotEqualsTranslator()
        : base(ExpressionType.NotEqual)
      {
      }

      protected override DbExpression TypedTranslate(
        ExpressionConverter parent,
        BinaryExpression linq)
      {
        Expression linq1 = (Expression) Expression.Not((Expression) Expression.Equal(linq.Left, linq.Right));
        return parent.TranslateExpression(linq1);
      }
    }

    private sealed class IsTranslator : ExpressionConverter.TypedTranslator<TypeBinaryExpression>
    {
      internal IsTranslator()
        : base(ExpressionType.TypeIs)
      {
      }

      protected override DbExpression TypedTranslate(
        ExpressionConverter parent,
        TypeBinaryExpression linq)
      {
        DbExpression dbExpression = parent.TranslateExpression(linq.Expression);
        TypeUsage resultType = dbExpression.ResultType;
        TypeUsage isOrAsTargetType = parent.GetIsOrAsTargetType(ExpressionType.TypeIs, linq.TypeOperand, linq.Expression.Type);
        return (DbExpression) dbExpression.IsOf(isOrAsTargetType);
      }
    }

    private sealed class AddTranslator : ExpressionConverter.BinaryTranslator
    {
      internal AddTranslator()
        : base(ExpressionType.Add, ExpressionType.AddChecked)
      {
      }

      protected override DbExpression TypedTranslate(
        ExpressionConverter parent,
        BinaryExpression linq)
      {
        if (linq.IsStringAddExpression())
          return ExpressionConverter.StringTranslatorUtil.ConcatArgs(parent, linq);
        return this.TranslateBinary(parent, parent.TranslateExpression(linq.Left), parent.TranslateExpression(linq.Right), linq);
      }

      protected override DbExpression TranslateBinary(
        ExpressionConverter parent,
        DbExpression left,
        DbExpression right,
        BinaryExpression linq)
      {
        return (DbExpression) left.Plus(right);
      }
    }

    private sealed class DivideTranslator : ExpressionConverter.BinaryTranslator
    {
      internal DivideTranslator()
        : base(ExpressionType.Divide)
      {
      }

      protected override DbExpression TranslateBinary(
        ExpressionConverter parent,
        DbExpression left,
        DbExpression right,
        BinaryExpression linq)
      {
        return (DbExpression) left.Divide(right);
      }
    }

    private sealed class ModuloTranslator : ExpressionConverter.BinaryTranslator
    {
      internal ModuloTranslator()
        : base(ExpressionType.Modulo)
      {
      }

      protected override DbExpression TranslateBinary(
        ExpressionConverter parent,
        DbExpression left,
        DbExpression right,
        BinaryExpression linq)
      {
        return (DbExpression) left.Modulo(right);
      }
    }

    private sealed class MultiplyTranslator : ExpressionConverter.BinaryTranslator
    {
      internal MultiplyTranslator()
        : base(ExpressionType.Multiply, ExpressionType.MultiplyChecked)
      {
      }

      protected override DbExpression TranslateBinary(
        ExpressionConverter parent,
        DbExpression left,
        DbExpression right,
        BinaryExpression linq)
      {
        return (DbExpression) left.Multiply(right);
      }
    }

    private sealed class PowerTranslator : ExpressionConverter.BinaryTranslator
    {
      internal PowerTranslator()
        : base(ExpressionType.Power)
      {
      }

      protected override DbExpression TranslateBinary(
        ExpressionConverter parent,
        DbExpression left,
        DbExpression right,
        BinaryExpression linq)
      {
        return (DbExpression) left.Power(right);
      }
    }

    private sealed class SubtractTranslator : ExpressionConverter.BinaryTranslator
    {
      internal SubtractTranslator()
        : base(ExpressionType.Subtract, ExpressionType.SubtractChecked)
      {
      }

      protected override DbExpression TranslateBinary(
        ExpressionConverter parent,
        DbExpression left,
        DbExpression right,
        BinaryExpression linq)
      {
        return (DbExpression) left.Minus(right);
      }
    }

    private abstract class UnaryTranslator : ExpressionConverter.TypedTranslator<UnaryExpression>
    {
      protected UnaryTranslator(params ExpressionType[] nodeTypes)
        : base(nodeTypes)
      {
      }

      protected override DbExpression TypedTranslate(
        ExpressionConverter parent,
        UnaryExpression linq)
      {
        return this.TranslateUnary(parent, linq, parent.TranslateExpression(linq.Operand));
      }

      protected abstract DbExpression TranslateUnary(
        ExpressionConverter parent,
        UnaryExpression unary,
        DbExpression operand);
    }

    private sealed class NegateTranslator : ExpressionConverter.UnaryTranslator
    {
      internal NegateTranslator()
        : base(ExpressionType.Negate, ExpressionType.NegateChecked)
      {
      }

      protected override DbExpression TranslateUnary(
        ExpressionConverter parent,
        UnaryExpression unary,
        DbExpression operand)
      {
        return (DbExpression) operand.UnaryMinus();
      }
    }

    private sealed class UnaryPlusTranslator : ExpressionConverter.UnaryTranslator
    {
      internal UnaryPlusTranslator()
        : base(ExpressionType.UnaryPlus)
      {
      }

      protected override DbExpression TranslateUnary(
        ExpressionConverter parent,
        UnaryExpression unary,
        DbExpression operand)
      {
        return operand;
      }
    }

    private abstract class BitwiseBinaryTranslator : ExpressionConverter.TypedTranslator<BinaryExpression>
    {
      private readonly string _canonicalFunctionName;

      protected BitwiseBinaryTranslator(ExpressionType nodeType, string canonicalFunctionName)
        : base(nodeType)
      {
        this._canonicalFunctionName = canonicalFunctionName;
      }

      protected override DbExpression TypedTranslate(
        ExpressionConverter parent,
        BinaryExpression linq)
      {
        DbExpression left = parent.TranslateExpression(linq.Left);
        DbExpression right = parent.TranslateExpression(linq.Right);
        if (TypeSemantics.IsBooleanType(left.ResultType))
          return this.TranslateIntoLogicExpression(parent, linq, left, right);
        return (DbExpression) parent.CreateCanonicalFunction(this._canonicalFunctionName, (Expression) linq, left, right);
      }

      protected abstract DbExpression TranslateIntoLogicExpression(
        ExpressionConverter parent,
        BinaryExpression linq,
        DbExpression left,
        DbExpression right);
    }

    private sealed class AndTranslator : ExpressionConverter.BitwiseBinaryTranslator
    {
      internal AndTranslator()
        : base(ExpressionType.And, "BitwiseAnd")
      {
      }

      protected override DbExpression TranslateIntoLogicExpression(
        ExpressionConverter parent,
        BinaryExpression linq,
        DbExpression left,
        DbExpression right)
      {
        return (DbExpression) left.And(right);
      }
    }

    private sealed class OrTranslator : ExpressionConverter.BitwiseBinaryTranslator
    {
      internal OrTranslator()
        : base(ExpressionType.Or, "BitwiseOr")
      {
      }

      protected override DbExpression TranslateIntoLogicExpression(
        ExpressionConverter parent,
        BinaryExpression linq,
        DbExpression left,
        DbExpression right)
      {
        return (DbExpression) left.Or(right);
      }
    }

    private sealed class ExclusiveOrTranslator : ExpressionConverter.BitwiseBinaryTranslator
    {
      internal ExclusiveOrTranslator()
        : base(ExpressionType.ExclusiveOr, "BitwiseXor")
      {
      }

      protected override DbExpression TranslateIntoLogicExpression(
        ExpressionConverter parent,
        BinaryExpression linq,
        DbExpression left,
        DbExpression right)
      {
        return (DbExpression) left.And((DbExpression) right.Not()).Or((DbExpression) left.Not().And(right));
      }
    }

    private sealed class NotTranslator : ExpressionConverter.TypedTranslator<UnaryExpression>
    {
      internal NotTranslator()
        : base(ExpressionType.Not)
      {
      }

      protected override DbExpression TypedTranslate(
        ExpressionConverter parent,
        UnaryExpression linq)
      {
        DbExpression dbExpression = parent.TranslateExpression(linq.Operand);
        if (TypeSemantics.IsBooleanType(dbExpression.ResultType))
          return (DbExpression) dbExpression.Not();
        return (DbExpression) parent.CreateCanonicalFunction("BitwiseNot", (Expression) linq, dbExpression);
      }
    }

    private sealed class QuoteTranslator : ExpressionConverter.UnaryTranslator
    {
      internal QuoteTranslator()
        : base(ExpressionType.Quote)
      {
      }

      protected override DbExpression TranslateUnary(
        ExpressionConverter parent,
        UnaryExpression unary,
        DbExpression operand)
      {
        return operand;
      }
    }

    private sealed class ConvertTranslator : ExpressionConverter.UnaryTranslator
    {
      internal ConvertTranslator()
        : base(ExpressionType.Convert, ExpressionType.ConvertChecked)
      {
      }

      protected override DbExpression TranslateUnary(
        ExpressionConverter parent,
        UnaryExpression unary,
        DbExpression operand)
      {
        Type type1 = unary.Type;
        Type type2 = unary.Operand.Type;
        return parent.CreateCastExpression(operand, type1, type2);
      }
    }

    private sealed class AsTranslator : ExpressionConverter.UnaryTranslator
    {
      internal AsTranslator()
        : base(ExpressionType.TypeAs)
      {
      }

      protected override DbExpression TranslateUnary(
        ExpressionConverter parent,
        UnaryExpression unary,
        DbExpression operand)
      {
        TypeUsage resultType = operand.ResultType;
        TypeUsage isOrAsTargetType = parent.GetIsOrAsTargetType(ExpressionType.TypeAs, unary.Type, unary.Operand.Type);
        return (DbExpression) operand.TreatAs(isOrAsTargetType);
      }
    }
  }
}
