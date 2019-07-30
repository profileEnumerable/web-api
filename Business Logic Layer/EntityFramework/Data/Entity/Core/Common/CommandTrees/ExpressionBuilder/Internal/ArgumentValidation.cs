// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder.Internal.ArgumentValidation
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.CommandTrees.Internal;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Metadata.Edm.Provider;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder.Internal
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
  internal static class ArgumentValidation
  {
    internal static ReadOnlyCollection<TElement> NewReadOnlyCollection<TElement>(
      IList<TElement> list)
    {
      return new ReadOnlyCollection<TElement>(list);
    }

    internal static void RequirePolymorphicType(TypeUsage type)
    {
      if (!TypeSemantics.IsPolymorphicType(type))
        throw new ArgumentException(Strings.Cqt_General_PolymorphicTypeRequired((object) type.ToString()), nameof (type));
    }

    internal static void RequireCompatibleType(
      DbExpression expression,
      TypeUsage requiredResultType,
      string argumentName)
    {
      ArgumentValidation.RequireCompatibleType(expression, requiredResultType, argumentName, -1);
    }

    private static void RequireCompatibleType(
      DbExpression expression,
      TypeUsage requiredResultType,
      string argumentName,
      int argumentIndex)
    {
      if (!TypeSemantics.IsStructurallyEqualOrPromotableTo(expression.ResultType, requiredResultType))
      {
        if (argumentIndex != -1)
          argumentName = StringUtil.FormatIndex(argumentName, argumentIndex);
        throw new ArgumentException(Strings.Cqt_ExpressionLink_TypeMismatch((object) expression.ResultType.ToString(), (object) requiredResultType.ToString()), argumentName);
      }
    }

    internal static void RequireCompatibleType(
      DbExpression expression,
      PrimitiveTypeKind requiredResultType,
      string argumentName)
    {
      ArgumentValidation.RequireCompatibleType(expression, requiredResultType, argumentName, -1);
    }

    private static void RequireCompatibleType(
      DbExpression expression,
      PrimitiveTypeKind requiredResultType,
      string argumentName,
      int index)
    {
      PrimitiveTypeKind typeKind;
      bool primitiveTypeKind = TypeHelpers.TryGetPrimitiveTypeKind(expression.ResultType, out typeKind);
      if (!primitiveTypeKind || typeKind != requiredResultType)
      {
        if (index != -1)
          argumentName = StringUtil.FormatIndex(argumentName, index);
        throw new ArgumentException(Strings.Cqt_ExpressionLink_TypeMismatch(primitiveTypeKind ? (object) Enum.GetName(typeof (PrimitiveTypeKind), (object) typeKind) : (object) expression.ResultType.ToString(), (object) Enum.GetName(typeof (PrimitiveTypeKind), (object) requiredResultType)), argumentName);
      }
    }

    private static void RequireCompatibleType(
      DbExpression from,
      RelationshipEndMember end,
      bool allowAllRelationshipsInSameTypeHierarchy)
    {
      TypeUsage typeUsage = end.TypeUsage;
      if (!TypeSemantics.IsReferenceType(typeUsage))
        typeUsage = TypeHelpers.CreateReferenceTypeUsage(TypeHelpers.GetEdmType<EntityType>(typeUsage));
      if (allowAllRelationshipsInSameTypeHierarchy)
      {
        if (TypeHelpers.GetCommonTypeUsage(typeUsage, from.ResultType) == null)
          throw new ArgumentException(Strings.Cqt_RelNav_WrongSourceType((object) typeUsage.ToString()), nameof (from));
      }
      else if (!TypeSemantics.IsStructurallyEqualOrPromotableTo(from.ResultType.EdmType, typeUsage.EdmType))
        throw new ArgumentException(Strings.Cqt_RelNav_WrongSourceType((object) typeUsage.ToString()), nameof (from));
    }

    internal static void RequireCollectionArgument<TExpressionType>(DbExpression argument)
    {
      if (!TypeSemantics.IsCollectionType(argument.ResultType))
        throw new ArgumentException(Strings.Cqt_Unary_CollectionRequired((object) typeof (TExpressionType).Name), nameof (argument));
    }

    internal static TypeUsage RequireCollectionArguments<TExpressionType>(
      DbExpression left,
      DbExpression right)
    {
      if (!TypeSemantics.IsCollectionType(left.ResultType) || !TypeSemantics.IsCollectionType(right.ResultType))
        throw new ArgumentException(Strings.Cqt_Binary_CollectionsRequired((object) typeof (TExpressionType).Name));
      TypeUsage commonTypeUsage = TypeHelpers.GetCommonTypeUsage(left.ResultType, right.ResultType);
      if (commonTypeUsage == null)
        throw new ArgumentException(Strings.Cqt_Binary_CollectionsRequired((object) typeof (TExpressionType).Name));
      return commonTypeUsage;
    }

    internal static TypeUsage RequireComparableCollectionArguments<TExpressionType>(
      DbExpression left,
      DbExpression right)
    {
      TypeUsage typeUsage = ArgumentValidation.RequireCollectionArguments<TExpressionType>(left, right);
      if (!TypeHelpers.IsSetComparableOpType(TypeHelpers.GetElementTypeUsage(left.ResultType)))
        throw new ArgumentException(Strings.Cqt_InvalidTypeForSetOperation((object) TypeHelpers.GetElementTypeUsage(left.ResultType).Identity, (object) typeof (TExpressionType).Name), nameof (left));
      if (!TypeHelpers.IsSetComparableOpType(TypeHelpers.GetElementTypeUsage(right.ResultType)))
        throw new ArgumentException(Strings.Cqt_InvalidTypeForSetOperation((object) TypeHelpers.GetElementTypeUsage(right.ResultType).Identity, (object) typeof (TExpressionType).Name), nameof (right));
      return typeUsage;
    }

    private static EnumerableValidator<TElementIn, TElementOut, TResult> CreateValidator<TElementIn, TElementOut, TResult>(
      IEnumerable<TElementIn> argument,
      string argumentName,
      Func<TElementIn, int, TElementOut> convertElement,
      Func<List<TElementOut>, TResult> createResult)
    {
      return new EnumerableValidator<TElementIn, TElementOut, TResult>(argument, argumentName)
      {
        ConvertElement = convertElement,
        CreateResult = createResult
      };
    }

    internal static DbExpressionList CreateExpressionList(
      IEnumerable<DbExpression> arguments,
      string argumentName,
      Action<DbExpression, int> validationCallback)
    {
      return ArgumentValidation.CreateExpressionList(arguments, argumentName, false, validationCallback);
    }

    private static DbExpressionList CreateExpressionList(
      IEnumerable<DbExpression> arguments,
      string argumentName,
      bool allowEmpty,
      Action<DbExpression, int> validationCallback)
    {
      EnumerableValidator<DbExpression, DbExpression, DbExpressionList> validator = ArgumentValidation.CreateValidator<DbExpression, DbExpression, DbExpressionList>(arguments, argumentName, (Func<DbExpression, int, DbExpression>) ((exp, idx) =>
      {
        if (validationCallback != null)
          validationCallback(exp, idx);
        return exp;
      }), (Func<List<DbExpression>, DbExpressionList>) (expList => new DbExpressionList((IList<DbExpression>) expList)));
      validator.AllowEmpty = allowEmpty;
      return validator.Validate();
    }

    private static DbExpressionList CreateExpressionList(
      IEnumerable<DbExpression> arguments,
      string argumentName,
      int expectedElementCount,
      Action<DbExpression, int> validationCallback)
    {
      EnumerableValidator<DbExpression, DbExpression, DbExpressionList> validator = ArgumentValidation.CreateValidator<DbExpression, DbExpression, DbExpressionList>(arguments, argumentName, (Func<DbExpression, int, DbExpression>) ((exp, idx) =>
      {
        if (validationCallback != null)
          validationCallback(exp, idx);
        return exp;
      }), (Func<List<DbExpression>, DbExpressionList>) (expList => new DbExpressionList((IList<DbExpression>) expList)));
      validator.ExpectedElementCount = expectedElementCount;
      validator.AllowEmpty = false;
      return validator.Validate();
    }

    private static FunctionParameter[] GetExpectedParameters(EdmFunction function)
    {
      return function.Parameters.Where<FunctionParameter>((Func<FunctionParameter, bool>) (p =>
      {
        if (p.Mode != ParameterMode.In)
          return p.Mode == ParameterMode.InOut;
        return true;
      })).ToArray<FunctionParameter>();
    }

    internal static DbExpressionList ValidateFunctionAggregate(
      EdmFunction function,
      IEnumerable<DbExpression> args)
    {
      ArgumentValidation.CheckFunction(function);
      if (!TypeSemantics.IsAggregateFunction(function) || function.ReturnParameter == null)
        throw new ArgumentException(Strings.Cqt_Aggregate_InvalidFunction, nameof (function));
      FunctionParameter[] expectedParams = ArgumentValidation.GetExpectedParameters(function);
      return ArgumentValidation.CreateExpressionList(args, "argument", expectedParams.Length, (Action<DbExpression, int>) ((exp, idx) =>
      {
        TypeUsage typeUsage = expectedParams[idx].TypeUsage;
        TypeUsage elementType = (TypeUsage) null;
        if (TypeHelpers.TryGetCollectionElementType(typeUsage, out elementType))
          typeUsage = elementType;
        ArgumentValidation.RequireCompatibleType(exp, typeUsage, "argument");
      }));
    }

    internal static void ValidateSortClause(DbExpression key)
    {
      if (!TypeHelpers.IsValidSortOpKeyType(key.ResultType))
        throw new ArgumentException(Strings.Cqt_Sort_OrderComparable, nameof (key));
    }

    internal static void ValidateSortClause(DbExpression key, string collation)
    {
      ArgumentValidation.ValidateSortClause(key);
      Check.NotEmpty(collation, nameof (collation));
      if (!TypeSemantics.IsPrimitiveType(key.ResultType, PrimitiveTypeKind.String))
        throw new ArgumentException(Strings.Cqt_Sort_NonStringCollationInvalid, nameof (collation));
    }

    internal static ReadOnlyCollection<DbVariableReferenceExpression> ValidateLambda(
      IEnumerable<DbVariableReferenceExpression> variables)
    {
      EnumerableValidator<DbVariableReferenceExpression, DbVariableReferenceExpression, ReadOnlyCollection<DbVariableReferenceExpression>> validator = ArgumentValidation.CreateValidator<DbVariableReferenceExpression, DbVariableReferenceExpression, ReadOnlyCollection<DbVariableReferenceExpression>>(variables, nameof (variables), (Func<DbVariableReferenceExpression, int, DbVariableReferenceExpression>) ((varExp, idx) =>
      {
        if (varExp == null)
          throw new ArgumentNullException(StringUtil.FormatIndex(nameof (variables), idx));
        return varExp;
      }), (Func<List<DbVariableReferenceExpression>, ReadOnlyCollection<DbVariableReferenceExpression>>) (varList => new ReadOnlyCollection<DbVariableReferenceExpression>((IList<DbVariableReferenceExpression>) varList)));
      validator.AllowEmpty = true;
      validator.GetName = (Func<DbVariableReferenceExpression, int, string>) ((varDef, idx) => varDef.VariableName);
      return validator.Validate();
    }

    internal static TypeUsage ValidateQuantifier(DbExpression predicate)
    {
      ArgumentValidation.RequireCompatibleType(predicate, PrimitiveTypeKind.Boolean, nameof (predicate));
      return predicate.ResultType;
    }

    internal static TypeUsage ValidateApply(
      DbExpressionBinding input,
      DbExpressionBinding apply)
    {
      if (input.VariableName.Equals(apply.VariableName, StringComparison.Ordinal))
        throw new ArgumentException(Strings.Cqt_Apply_DuplicateVariableNames);
      return ArgumentValidation.CreateCollectionOfRowResultType(new List<KeyValuePair<string, TypeUsage>>()
      {
        new KeyValuePair<string, TypeUsage>(input.VariableName, input.VariableType),
        new KeyValuePair<string, TypeUsage>(apply.VariableName, apply.VariableType)
      });
    }

    internal static ReadOnlyCollection<DbExpressionBinding> ValidateCrossJoin(
      IEnumerable<DbExpressionBinding> inputs,
      out TypeUsage resultType)
    {
      List<DbExpressionBinding> expressionBindingList = new List<DbExpressionBinding>();
      List<KeyValuePair<string, TypeUsage>> columns = new List<KeyValuePair<string, TypeUsage>>();
      Dictionary<string, int> dictionary = new Dictionary<string, int>();
      IEnumerator<DbExpressionBinding> enumerator = inputs.GetEnumerator();
      int index = 0;
      while (enumerator.MoveNext())
      {
        DbExpressionBinding current = enumerator.Current;
        string paramName = StringUtil.FormatIndex(nameof (inputs), index);
        if (current == null)
          throw new ArgumentNullException(paramName);
        int num = -1;
        if (dictionary.TryGetValue(current.VariableName, out num))
          throw new ArgumentException(Strings.Cqt_CrossJoin_DuplicateVariableNames((object) num, (object) index, (object) current.VariableName));
        expressionBindingList.Add(current);
        dictionary.Add(current.VariableName, index);
        columns.Add(new KeyValuePair<string, TypeUsage>(current.VariableName, current.VariableType));
        ++index;
      }
      if (expressionBindingList.Count < 2)
        throw new ArgumentException(Strings.Cqt_CrossJoin_AtLeastTwoInputs, nameof (inputs));
      resultType = ArgumentValidation.CreateCollectionOfRowResultType(columns);
      return new ReadOnlyCollection<DbExpressionBinding>((IList<DbExpressionBinding>) expressionBindingList);
    }

    internal static TypeUsage ValidateJoin(
      DbExpressionBinding left,
      DbExpressionBinding right,
      DbExpression joinCondition)
    {
      if (left.VariableName.Equals(right.VariableName, StringComparison.Ordinal))
        throw new ArgumentException(Strings.Cqt_Join_DuplicateVariableNames);
      ArgumentValidation.RequireCompatibleType(joinCondition, PrimitiveTypeKind.Boolean, nameof (joinCondition));
      return ArgumentValidation.CreateCollectionOfRowResultType(new List<KeyValuePair<string, TypeUsage>>(2)
      {
        new KeyValuePair<string, TypeUsage>(left.VariableName, left.VariableType),
        new KeyValuePair<string, TypeUsage>(right.VariableName, right.VariableType)
      });
    }

    internal static TypeUsage ValidateFilter(
      DbExpressionBinding input,
      DbExpression predicate)
    {
      ArgumentValidation.RequireCompatibleType(predicate, PrimitiveTypeKind.Boolean, nameof (predicate));
      return input.Expression.ResultType;
    }

    internal static TypeUsage ValidateGroupBy(
      IEnumerable<KeyValuePair<string, DbExpression>> keys,
      IEnumerable<KeyValuePair<string, DbAggregate>> aggregates,
      out DbExpressionList validKeys,
      out ReadOnlyCollection<DbAggregate> validAggregates)
    {
      List<KeyValuePair<string, TypeUsage>> columns = new List<KeyValuePair<string, TypeUsage>>();
      HashSet<string> keyNames = new HashSet<string>();
      EnumerableValidator<KeyValuePair<string, DbExpression>, DbExpression, DbExpressionList> validator1 = ArgumentValidation.CreateValidator<KeyValuePair<string, DbExpression>, DbExpression, DbExpressionList>(keys, nameof (keys), (Func<KeyValuePair<string, DbExpression>, int, DbExpression>) ((keyInfo, index) =>
      {
        ArgumentValidation.CheckNamed<DbExpression>(keyInfo, nameof (keys), index);
        if (!TypeHelpers.IsValidGroupKeyType(keyInfo.Value.ResultType))
          throw new ArgumentException(Strings.Cqt_GroupBy_KeyNotEqualityComparable((object) keyInfo.Key));
        keyNames.Add(keyInfo.Key);
        columns.Add(new KeyValuePair<string, TypeUsage>(keyInfo.Key, keyInfo.Value.ResultType));
        return keyInfo.Value;
      }), (Func<List<DbExpression>, DbExpressionList>) (expList => new DbExpressionList((IList<DbExpression>) expList)));
      validator1.AllowEmpty = true;
      validator1.GetName = (Func<KeyValuePair<string, DbExpression>, int, string>) ((keyInfo, idx) => keyInfo.Key);
      validKeys = validator1.Validate();
      bool hasGroupAggregate = false;
      EnumerableValidator<KeyValuePair<string, DbAggregate>, DbAggregate, ReadOnlyCollection<DbAggregate>> validator2 = ArgumentValidation.CreateValidator<KeyValuePair<string, DbAggregate>, DbAggregate, ReadOnlyCollection<DbAggregate>>(aggregates, nameof (aggregates), (Func<KeyValuePair<string, DbAggregate>, int, DbAggregate>) ((aggInfo, idx) =>
      {
        ArgumentValidation.CheckNamed<DbAggregate>(aggInfo, nameof (aggregates), idx);
        if (keyNames.Contains(aggInfo.Key))
          throw new ArgumentException(Strings.Cqt_GroupBy_AggregateColumnExistsAsGroupColumn((object) aggInfo.Key));
        if (aggInfo.Value is DbGroupAggregate)
        {
          if (hasGroupAggregate)
            throw new ArgumentException(Strings.Cqt_GroupBy_MoreThanOneGroupAggregate);
          hasGroupAggregate = true;
        }
        columns.Add(new KeyValuePair<string, TypeUsage>(aggInfo.Key, aggInfo.Value.ResultType));
        return aggInfo.Value;
      }), (Func<List<DbAggregate>, ReadOnlyCollection<DbAggregate>>) (aggList => ArgumentValidation.NewReadOnlyCollection<DbAggregate>((IList<DbAggregate>) aggList)));
      validator2.AllowEmpty = true;
      validator2.GetName = (Func<KeyValuePair<string, DbAggregate>, int, string>) ((aggInfo, idx) => aggInfo.Key);
      validAggregates = validator2.Validate();
      if (validKeys.Count == 0 && validAggregates.Count == 0)
        throw new ArgumentException(Strings.Cqt_GroupBy_AtLeastOneKeyOrAggregate);
      return ArgumentValidation.CreateCollectionOfRowResultType(columns);
    }

    internal static ReadOnlyCollection<DbSortClause> ValidateSortArguments(
      IEnumerable<DbSortClause> sortOrder)
    {
      EnumerableValidator<DbSortClause, DbSortClause, ReadOnlyCollection<DbSortClause>> validator = ArgumentValidation.CreateValidator<DbSortClause, DbSortClause, ReadOnlyCollection<DbSortClause>>(sortOrder, nameof (sortOrder), (Func<DbSortClause, int, DbSortClause>) ((key, idx) => key), (Func<List<DbSortClause>, ReadOnlyCollection<DbSortClause>>) (keyList => ArgumentValidation.NewReadOnlyCollection<DbSortClause>((IList<DbSortClause>) keyList)));
      validator.AllowEmpty = false;
      return validator.Validate();
    }

    internal static ReadOnlyCollection<DbSortClause> ValidateSort(
      IEnumerable<DbSortClause> sortOrder)
    {
      return ArgumentValidation.ValidateSortArguments(sortOrder);
    }

    internal static TypeUsage ValidateConstant(Type type)
    {
      PrimitiveTypeKind primitiveTypeKind;
      if (!ArgumentValidation.TryGetPrimitiveTypeKind(type, out primitiveTypeKind))
        throw new ArgumentException(Strings.Cqt_Constant_InvalidType, nameof (type));
      return TypeHelpers.GetLiteralTypeUsage(primitiveTypeKind);
    }

    internal static TypeUsage ValidateConstant(object value)
    {
      return ArgumentValidation.ValidateConstant(value.GetType());
    }

    internal static void ValidateConstant(TypeUsage constantType, object value)
    {
      ArgumentValidation.CheckType(constantType, nameof (constantType));
      EnumType type1;
      if (TypeHelpers.TryGetEdmType<EnumType>(constantType, out type1))
      {
        Type clrEquivalentType = type1.UnderlyingType.ClrEquivalentType;
        if (clrEquivalentType != value.GetType() && (!value.GetType().IsEnum() || !ArgumentValidation.ClrEdmEnumTypesMatch(type1, value.GetType())))
          throw new ArgumentException(Strings.Cqt_Constant_ClrEnumTypeDoesNotMatchEdmEnumType((object) value.GetType().Name, (object) type1.Name, (object) clrEquivalentType.Name), nameof (value));
      }
      else
      {
        PrimitiveType type2;
        if (!TypeHelpers.TryGetEdmType<PrimitiveType>(constantType, out type2))
          throw new ArgumentException(Strings.Cqt_Constant_InvalidConstantType((object) constantType.ToString()), nameof (constantType));
        PrimitiveTypeKind primitiveTypeKind;
        if ((!ArgumentValidation.TryGetPrimitiveTypeKind(value.GetType(), out primitiveTypeKind) || type2.PrimitiveTypeKind != primitiveTypeKind) && (!Helper.IsGeographicType(type2) || primitiveTypeKind != PrimitiveTypeKind.Geography) && (!Helper.IsGeometricType(type2) || primitiveTypeKind != PrimitiveTypeKind.Geometry))
          throw new ArgumentException(Strings.Cqt_Constant_InvalidValueForType((object) constantType.ToString()), nameof (value));
      }
    }

    internal static TypeUsage ValidateCreateRef(
      EntitySet entitySet,
      EntityType entityType,
      IEnumerable<DbExpression> keyValues,
      out DbExpression keyConstructor)
    {
      ArgumentValidation.CheckEntitySet((EntitySetBase) entitySet, nameof (entitySet));
      ArgumentValidation.CheckType((EdmType) entityType, nameof (entityType));
      if (!TypeSemantics.IsValidPolymorphicCast((EdmType) entitySet.ElementType, (EdmType) entityType))
        throw new ArgumentException(Strings.Cqt_Ref_PolymorphicArgRequired);
      IList<EdmMember> keyMembers = (IList<EdmMember>) entityType.KeyMembers;
      EnumerableValidator<DbExpression, KeyValuePair<string, DbExpression>, List<KeyValuePair<string, DbExpression>>> validator = ArgumentValidation.CreateValidator<DbExpression, KeyValuePair<string, DbExpression>, List<KeyValuePair<string, DbExpression>>>(keyValues, nameof (keyValues), (Func<DbExpression, int, KeyValuePair<string, DbExpression>>) ((valueExp, idx) =>
      {
        ArgumentValidation.RequireCompatibleType(valueExp, keyMembers[idx].TypeUsage, nameof (keyValues), idx);
        return new KeyValuePair<string, DbExpression>(keyMembers[idx].Name, valueExp);
      }), (Func<List<KeyValuePair<string, DbExpression>>, List<KeyValuePair<string, DbExpression>>>) (columnList => columnList));
      validator.ExpectedElementCount = keyMembers.Count;
      List<KeyValuePair<string, DbExpression>> keyValuePairList = validator.Validate();
      keyConstructor = (DbExpression) DbExpressionBuilder.NewRow((IEnumerable<KeyValuePair<string, DbExpression>>) keyValuePairList);
      return ArgumentValidation.CreateReferenceResultType((EntityTypeBase) entityType);
    }

    internal static TypeUsage ValidateRefFromKey(
      EntitySet entitySet,
      DbExpression keyValues,
      EntityType entityType)
    {
      ArgumentValidation.CheckEntitySet((EntitySetBase) entitySet, nameof (entitySet));
      ArgumentValidation.CheckType((EdmType) entityType);
      if (!TypeSemantics.IsValidPolymorphicCast((EdmType) entitySet.ElementType, (EdmType) entityType))
        throw new ArgumentException(Strings.Cqt_Ref_PolymorphicArgRequired);
      TypeUsage resultType = ArgumentValidation.CreateResultType((EdmType) TypeHelpers.CreateKeyRowType((EntityTypeBase) entitySet.ElementType));
      ArgumentValidation.RequireCompatibleType(keyValues, resultType, nameof (keyValues));
      return ArgumentValidation.CreateReferenceResultType((EntityTypeBase) entityType);
    }

    internal static TypeUsage ValidateNavigate(
      DbExpression navigateFrom,
      RelationshipType type,
      string fromEndName,
      string toEndName,
      out RelationshipEndMember fromEnd,
      out RelationshipEndMember toEnd)
    {
      ArgumentValidation.CheckType((EdmType) type);
      if (!type.RelationshipEndMembers.TryGetValue(fromEndName, false, out fromEnd))
        throw new ArgumentOutOfRangeException(fromEndName, Strings.Cqt_Factory_NoSuchRelationEnd);
      if (!type.RelationshipEndMembers.TryGetValue(toEndName, false, out toEnd))
        throw new ArgumentOutOfRangeException(toEndName, Strings.Cqt_Factory_NoSuchRelationEnd);
      ArgumentValidation.RequireCompatibleType(navigateFrom, fromEnd, false);
      return ArgumentValidation.CreateResultType(toEnd);
    }

    internal static TypeUsage ValidateNavigate(
      DbExpression navigateFrom,
      RelationshipEndMember fromEnd,
      RelationshipEndMember toEnd,
      out RelationshipType relType,
      bool allowAllRelationshipsInSameTypeHierarchy)
    {
      ArgumentValidation.CheckMember((EdmMember) fromEnd, nameof (fromEnd));
      ArgumentValidation.CheckMember((EdmMember) toEnd, nameof (toEnd));
      relType = fromEnd.DeclaringType as RelationshipType;
      ArgumentValidation.CheckType((EdmType) relType);
      if (!relType.Equals((object) toEnd.DeclaringType))
        throw new ArgumentException(Strings.Cqt_Factory_IncompatibleRelationEnds, nameof (toEnd));
      ArgumentValidation.RequireCompatibleType(navigateFrom, fromEnd, allowAllRelationshipsInSameTypeHierarchy);
      return ArgumentValidation.CreateResultType(toEnd);
    }

    internal static TypeUsage ValidateElement(DbExpression argument)
    {
      ArgumentValidation.RequireCollectionArgument<DbElementExpression>(argument);
      return TypeHelpers.GetEdmType<CollectionType>(argument.ResultType).TypeUsage;
    }

    internal static TypeUsage ValidateCase(
      IEnumerable<DbExpression> whenExpressions,
      IEnumerable<DbExpression> thenExpressions,
      DbExpression elseExpression,
      out DbExpressionList validWhens,
      out DbExpressionList validThens)
    {
      validWhens = ArgumentValidation.CreateExpressionList(whenExpressions, nameof (whenExpressions), (Action<DbExpression, int>) ((exp, idx) => ArgumentValidation.RequireCompatibleType(exp, PrimitiveTypeKind.Boolean, nameof (whenExpressions), idx)));
      TypeUsage commonResultType = (TypeUsage) null;
      validThens = ArgumentValidation.CreateExpressionList(thenExpressions, nameof (thenExpressions), (Action<DbExpression, int>) ((exp, idx) =>
      {
        if (commonResultType == null)
        {
          commonResultType = exp.ResultType;
        }
        else
        {
          commonResultType = TypeHelpers.GetCommonTypeUsage(exp.ResultType, commonResultType);
          if (commonResultType == null)
            throw new ArgumentException(Strings.Cqt_Case_InvalidResultType);
        }
      }));
      commonResultType = TypeHelpers.GetCommonTypeUsage(elseExpression.ResultType, commonResultType);
      if (commonResultType == null)
        throw new ArgumentException(Strings.Cqt_Case_InvalidResultType);
      if (validWhens.Count != validThens.Count)
        throw new ArgumentException(Strings.Cqt_Case_WhensMustEqualThens);
      return commonResultType;
    }

    internal static TypeUsage ValidateFunction(
      EdmFunction function,
      IEnumerable<DbExpression> arguments,
      out DbExpressionList validArgs)
    {
      ArgumentValidation.CheckFunction(function);
      if (!function.IsComposableAttribute)
        throw new ArgumentException(Strings.Cqt_Function_NonComposableInExpression, nameof (function));
      if (!string.IsNullOrEmpty(function.CommandTextAttribute) && !function.HasUserDefinedBody)
        throw new ArgumentException(Strings.Cqt_Function_CommandTextInExpression, nameof (function));
      if (function.ReturnParameter == null)
        throw new ArgumentException(Strings.Cqt_Function_VoidResultInvalid, nameof (function));
      FunctionParameter[] expectedParams = ArgumentValidation.GetExpectedParameters(function);
      validArgs = ArgumentValidation.CreateExpressionList(arguments, nameof (arguments), expectedParams.Length, (Action<DbExpression, int>) ((exp, idx) => ArgumentValidation.RequireCompatibleType(exp, expectedParams[idx].TypeUsage, nameof (arguments), idx)));
      return function.ReturnParameter.TypeUsage;
    }

    internal static TypeUsage ValidateInvoke(
      DbLambda lambda,
      IEnumerable<DbExpression> arguments,
      out DbExpressionList validArguments)
    {
      validArguments = (DbExpressionList) null;
      EnumerableValidator<DbExpression, DbExpression, DbExpressionList> validator = ArgumentValidation.CreateValidator<DbExpression, DbExpression, DbExpressionList>(arguments, nameof (arguments), (Func<DbExpression, int, DbExpression>) ((exp, idx) =>
      {
        ArgumentValidation.RequireCompatibleType(exp, lambda.Variables[idx].ResultType, nameof (arguments), idx);
        return exp;
      }), (Func<List<DbExpression>, DbExpressionList>) (expList => new DbExpressionList((IList<DbExpression>) expList)));
      validator.ExpectedElementCount = lambda.Variables.Count;
      validArguments = validator.Validate();
      return lambda.Body.ResultType;
    }

    internal static TypeUsage ValidateNewEmptyCollection(
      TypeUsage collectionType,
      out DbExpressionList validElements)
    {
      ArgumentValidation.CheckType(collectionType, nameof (collectionType));
      if (!TypeSemantics.IsCollectionType(collectionType))
        throw new ArgumentException(Strings.Cqt_NewInstance_CollectionTypeRequired, nameof (collectionType));
      validElements = new DbExpressionList((IList<DbExpression>) new DbExpression[0]);
      return collectionType;
    }

    internal static TypeUsage ValidateNewRow(
      IEnumerable<KeyValuePair<string, DbExpression>> columnValues,
      out DbExpressionList validElements)
    {
      List<KeyValuePair<string, TypeUsage>> columnTypes = new List<KeyValuePair<string, TypeUsage>>();
      EnumerableValidator<KeyValuePair<string, DbExpression>, DbExpression, DbExpressionList> validator = ArgumentValidation.CreateValidator<KeyValuePair<string, DbExpression>, DbExpression, DbExpressionList>(columnValues, nameof (columnValues), (Func<KeyValuePair<string, DbExpression>, int, DbExpression>) ((columnValue, idx) =>
      {
        ArgumentValidation.CheckNamed<DbExpression>(columnValue, nameof (columnValues), idx);
        columnTypes.Add(new KeyValuePair<string, TypeUsage>(columnValue.Key, columnValue.Value.ResultType));
        return columnValue.Value;
      }), (Func<List<DbExpression>, DbExpressionList>) (expList => new DbExpressionList((IList<DbExpression>) expList)));
      validator.GetName = (Func<KeyValuePair<string, DbExpression>, int, string>) ((columnValue, idx) => columnValue.Key);
      validElements = validator.Validate();
      return ArgumentValidation.CreateResultType((EdmType) TypeHelpers.CreateRowType((IEnumerable<KeyValuePair<string, TypeUsage>>) columnTypes));
    }

    internal static TypeUsage ValidateNew(
      TypeUsage instanceType,
      IEnumerable<DbExpression> arguments,
      out DbExpressionList validArguments)
    {
      ArgumentValidation.CheckType(instanceType, nameof (instanceType));
      CollectionType type = (CollectionType) null;
      if (TypeHelpers.TryGetEdmType<CollectionType>(instanceType, out type) && type != null)
      {
        TypeUsage elementType = type.TypeUsage;
        validArguments = ArgumentValidation.CreateExpressionList(arguments, nameof (arguments), true, (Action<DbExpression, int>) ((exp, idx) => ArgumentValidation.RequireCompatibleType(exp, elementType, nameof (arguments), idx)));
      }
      else
      {
        List<TypeUsage> expectedTypes = ArgumentValidation.GetStructuralMemberTypes(instanceType);
        int pos = 0;
        validArguments = ArgumentValidation.CreateExpressionList(arguments, nameof (arguments), expectedTypes.Count, (Action<DbExpression, int>) ((exp, idx) => ArgumentValidation.RequireCompatibleType(exp, expectedTypes[pos++], nameof (arguments), idx)));
      }
      return instanceType;
    }

    private static List<TypeUsage> GetStructuralMemberTypes(TypeUsage instanceType)
    {
      StructuralType edmType = instanceType.EdmType as StructuralType;
      if (edmType == null)
        throw new ArgumentException(Strings.Cqt_NewInstance_StructuralTypeRequired, nameof (instanceType));
      if (edmType.Abstract)
        throw new ArgumentException(Strings.Cqt_NewInstance_CannotInstantiateAbstractType((object) instanceType.ToString()), nameof (instanceType));
      IBaseList<EdmMember> structuralMembers = TypeHelpers.GetAllStructuralMembers((EdmType) edmType);
      if (structuralMembers == null || structuralMembers.Count < 1)
        throw new ArgumentException(Strings.Cqt_NewInstance_CannotInstantiateMemberlessType((object) instanceType.ToString()), nameof (instanceType));
      List<TypeUsage> typeUsageList = new List<TypeUsage>(structuralMembers.Count);
      for (int index = 0; index < structuralMembers.Count; ++index)
        typeUsageList.Add(Helper.GetModelTypeUsage(structuralMembers[index]));
      return typeUsageList;
    }

    internal static TypeUsage ValidateNewEntityWithRelationships(
      EntityType entityType,
      IEnumerable<DbExpression> attributeValues,
      IList<DbRelatedEntityRef> relationships,
      out DbExpressionList validArguments,
      out ReadOnlyCollection<DbRelatedEntityRef> validRelatedRefs)
    {
      TypeUsage typeUsage = ArgumentValidation.ValidateNew(ArgumentValidation.CreateResultType((EdmType) entityType), attributeValues, out validArguments);
      if (relationships.Count > 0)
      {
        List<DbRelatedEntityRef> relatedEntityRefList = new List<DbRelatedEntityRef>(relationships.Count);
        for (int index = 0; index < relationships.Count; ++index)
        {
          DbRelatedEntityRef relationship = relationships[index];
          EntityTypeBase elementType = TypeHelpers.GetEdmType<RefType>(relationship.SourceEnd.TypeUsage).ElementType;
          if (!entityType.EdmEquals((MetadataItem) elementType) && !entityType.IsSubtypeOf((EdmType) elementType))
            throw new ArgumentException(Strings.Cqt_NewInstance_IncompatibleRelatedEntity_SourceTypeNotValid, StringUtil.FormatIndex(nameof (relationships), index));
          relatedEntityRefList.Add(relationship);
        }
        validRelatedRefs = new ReadOnlyCollection<DbRelatedEntityRef>((IList<DbRelatedEntityRef>) relatedEntityRefList);
      }
      else
        validRelatedRefs = new ReadOnlyCollection<DbRelatedEntityRef>((IList<DbRelatedEntityRef>) new DbRelatedEntityRef[0]);
      return typeUsage;
    }

    internal static TypeUsage ValidateProperty(
      DbExpression instance,
      string propertyName,
      bool ignoreCase,
      out EdmMember foundMember)
    {
      StructuralType type;
      if (TypeHelpers.TryGetEdmType<StructuralType>(instance.ResultType, out type) && type.Members.TryGetValue(propertyName, ignoreCase, out foundMember) && foundMember != null && (Helper.IsRelationshipEndMember(foundMember) || Helper.IsEdmProperty(foundMember) || Helper.IsNavigationProperty(foundMember)))
        return Helper.GetModelTypeUsage(foundMember);
      throw new ArgumentOutOfRangeException(nameof (propertyName), Strings.NoSuchProperty((object) propertyName, (object) instance.ResultType.ToString()));
    }

    private static void CheckNamed<T>(
      KeyValuePair<string, T> element,
      string argumentName,
      int index)
    {
      if (string.IsNullOrEmpty(element.Key))
      {
        if (index != -1)
          argumentName = StringUtil.FormatIndex(argumentName, index);
        throw new ArgumentNullException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.Key", (object) argumentName));
      }
      if ((object) element.Value == null)
      {
        if (index != -1)
          argumentName = StringUtil.FormatIndex(argumentName, index);
        throw new ArgumentNullException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.Value", (object) argumentName));
      }
    }

    private static void CheckReadOnly(GlobalItem item, string varName)
    {
      if (!item.IsReadOnly)
        throw new ArgumentException(Strings.Cqt_General_MetadataNotReadOnly, varName);
    }

    private static void CheckReadOnly(TypeUsage item, string varName)
    {
      if (!item.IsReadOnly)
        throw new ArgumentException(Strings.Cqt_General_MetadataNotReadOnly, varName);
    }

    private static void CheckReadOnly(EntitySetBase item, string varName)
    {
      if (!item.IsReadOnly)
        throw new ArgumentException(Strings.Cqt_General_MetadataNotReadOnly, varName);
    }

    private static void CheckType(EdmType type)
    {
      ArgumentValidation.CheckType(type, nameof (type));
    }

    private static void CheckType(EdmType type, string argumentName)
    {
      ArgumentValidation.CheckReadOnly((GlobalItem) type, argumentName);
    }

    internal static void CheckType(TypeUsage type)
    {
      ArgumentValidation.CheckType(type, nameof (type));
    }

    internal static void CheckType(TypeUsage type, string varName)
    {
      ArgumentValidation.CheckReadOnly(type, varName);
      if (!ArgumentValidation.CheckDataSpace(type))
        throw new ArgumentException(Strings.Cqt_Metadata_TypeUsageIncorrectSpace, nameof (type));
    }

    internal static void CheckMember(EdmMember memberMeta, string varName)
    {
      ArgumentValidation.CheckReadOnly((GlobalItem) memberMeta.DeclaringType, varName);
      if (!ArgumentValidation.CheckDataSpace(memberMeta.TypeUsage) || !ArgumentValidation.CheckDataSpace((GlobalItem) memberMeta.DeclaringType))
        throw new ArgumentException(Strings.Cqt_Metadata_EdmMemberIncorrectSpace, varName);
    }

    private static void CheckParameter(FunctionParameter paramMeta, string varName)
    {
      ArgumentValidation.CheckReadOnly((GlobalItem) paramMeta.DeclaringFunction, varName);
      if (!ArgumentValidation.CheckDataSpace(paramMeta.TypeUsage))
        throw new ArgumentException(Strings.Cqt_Metadata_FunctionParameterIncorrectSpace, varName);
    }

    [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
    private static void CheckFunction(EdmFunction function)
    {
      ArgumentValidation.CheckReadOnly((GlobalItem) function, nameof (function));
      if (!ArgumentValidation.CheckDataSpace((GlobalItem) function))
        throw new ArgumentException(Strings.Cqt_Metadata_FunctionIncorrectSpace, nameof (function));
      if (function.IsComposableAttribute && function.ReturnParameter == null)
        throw new ArgumentException(Strings.Cqt_Metadata_FunctionReturnParameterNull, nameof (function));
      if (function.ReturnParameter != null && !ArgumentValidation.CheckDataSpace(function.ReturnParameter.TypeUsage))
        throw new ArgumentException(Strings.Cqt_Metadata_FunctionParameterIncorrectSpace, "function.ReturnParameter");
      IList<FunctionParameter> parameters = (IList<FunctionParameter>) function.Parameters;
      for (int index = 0; index < parameters.Count; ++index)
        ArgumentValidation.CheckParameter(parameters[index], StringUtil.FormatIndex("function.Parameters", index));
    }

    internal static void CheckEntitySet(EntitySetBase entitySet, string varName)
    {
      ArgumentValidation.CheckReadOnly(entitySet, varName);
      if (entitySet.EntityContainer == null)
        throw new ArgumentException(Strings.Cqt_Metadata_EntitySetEntityContainerNull, varName);
      if (!ArgumentValidation.CheckDataSpace((GlobalItem) entitySet.EntityContainer))
        throw new ArgumentException(Strings.Cqt_Metadata_EntitySetIncorrectSpace, varName);
      if (!ArgumentValidation.CheckDataSpace((GlobalItem) entitySet.ElementType))
        throw new ArgumentException(Strings.Cqt_Metadata_EntitySetIncorrectSpace, varName);
    }

    private static bool CheckDataSpace(TypeUsage type)
    {
      return ArgumentValidation.CheckDataSpace((GlobalItem) type.EdmType);
    }

    private static bool CheckDataSpace(GlobalItem item)
    {
      if (BuiltInTypeKind.PrimitiveType == item.BuiltInTypeKind || BuiltInTypeKind.EdmFunction == item.BuiltInTypeKind && DataSpace.CSpace == item.DataSpace)
        return true;
      if (Helper.IsRowType(item))
      {
        foreach (EdmMember property in ((RowType) item).Properties)
        {
          if (!ArgumentValidation.CheckDataSpace(property.TypeUsage))
            return false;
        }
        return true;
      }
      if (Helper.IsCollectionType(item))
        return ArgumentValidation.CheckDataSpace(((CollectionType) item).TypeUsage);
      if (Helper.IsRefType(item))
        return ArgumentValidation.CheckDataSpace((GlobalItem) ((RefType) item).ElementType);
      if (item.DataSpace != DataSpace.SSpace)
        return item.DataSpace == DataSpace.CSpace;
      return true;
    }

    internal static TypeUsage CreateCollectionOfRowResultType(
      List<KeyValuePair<string, TypeUsage>> columns)
    {
      return TypeUsage.Create((EdmType) TypeHelpers.CreateCollectionType(TypeUsage.Create((EdmType) TypeHelpers.CreateRowType((IEnumerable<KeyValuePair<string, TypeUsage>>) columns))));
    }

    private static TypeUsage CreateResultType(EdmType resultType)
    {
      return TypeUsage.Create(resultType);
    }

    private static TypeUsage CreateResultType(RelationshipEndMember end)
    {
      TypeUsage typeUsage = end.TypeUsage;
      if (!TypeSemantics.IsReferenceType(typeUsage))
        typeUsage = TypeHelpers.CreateReferenceTypeUsage(TypeHelpers.GetEdmType<EntityType>(typeUsage));
      if (RelationshipMultiplicity.Many == end.RelationshipMultiplicity)
        typeUsage = TypeHelpers.CreateCollectionTypeUsage(typeUsage);
      return typeUsage;
    }

    internal static TypeUsage CreateReferenceResultType(EntityTypeBase referencedEntityType)
    {
      return TypeUsage.Create((EdmType) TypeHelpers.CreateReferenceType(referencedEntityType));
    }

    private static bool TryGetPrimitiveTypeKind(
      Type clrType,
      out PrimitiveTypeKind primitiveTypeKind)
    {
      return ClrProviderManifest.TryGetPrimitiveTypeKind(clrType, out primitiveTypeKind);
    }

    private static bool ClrEdmEnumTypesMatch(EnumType edmEnumType, Type clrEnumType)
    {
      PrimitiveTypeKind primitiveTypeKind;
      if (clrEnumType.Name != edmEnumType.Name || clrEnumType.GetEnumNames().Length < edmEnumType.Members.Count || (!ArgumentValidation.TryGetPrimitiveTypeKind(clrEnumType.GetEnumUnderlyingType(), out primitiveTypeKind) || primitiveTypeKind != edmEnumType.UnderlyingType.PrimitiveTypeKind))
        return false;
      foreach (EnumMember member in edmEnumType.Members)
      {
        if (!((IEnumerable<string>) clrEnumType.GetEnumNames()).Contains<string>(member.Name) || !member.Value.Equals(Convert.ChangeType(Enum.Parse(clrEnumType, member.Name), clrEnumType.GetEnumUnderlyingType(), (IFormatProvider) CultureInfo.InvariantCulture)))
          return false;
      }
      return true;
    }
  }
}
