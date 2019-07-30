// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.FunctionOverloadResolver
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Metadata.Edm.Provider;
using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.Core.Common.EntitySql
{
  internal static class FunctionOverloadResolver
  {
    internal static EdmFunction ResolveFunctionOverloads(
      IList<EdmFunction> functionsMetadata,
      IList<TypeUsage> argTypes,
      bool isGroupAggregateFunction,
      out bool isAmbiguous)
    {
      return FunctionOverloadResolver.ResolveFunctionOverloads<EdmFunction, FunctionParameter>(functionsMetadata, argTypes, (Func<EdmFunction, IList<FunctionParameter>>) (edmFunction => (IList<FunctionParameter>) edmFunction.Parameters), (Func<FunctionParameter, TypeUsage>) (functionParameter => functionParameter.TypeUsage), (Func<FunctionParameter, ParameterMode>) (functionParameter => functionParameter.Mode), (Func<TypeUsage, IEnumerable<TypeUsage>>) (argType => TypeSemantics.FlattenType(argType)), (Func<TypeUsage, TypeUsage, IEnumerable<TypeUsage>>) ((paramType, argType) => TypeSemantics.FlattenType(paramType)), (Func<TypeUsage, TypeUsage, bool>) ((fromType, toType) => TypeSemantics.IsPromotableTo(fromType, toType)), (Func<TypeUsage, TypeUsage, bool>) ((fromType, toType) => TypeSemantics.IsStructurallyEqual(fromType, toType)), isGroupAggregateFunction, out isAmbiguous);
    }

    internal static EdmFunction ResolveFunctionOverloads(
      IList<EdmFunction> functionsMetadata,
      IList<TypeUsage> argTypes,
      Func<TypeUsage, IEnumerable<TypeUsage>> flattenArgumentType,
      Func<TypeUsage, TypeUsage, IEnumerable<TypeUsage>> flattenParameterType,
      Func<TypeUsage, TypeUsage, bool> isPromotableTo,
      Func<TypeUsage, TypeUsage, bool> isStructurallyEqual,
      bool isGroupAggregateFunction,
      out bool isAmbiguous)
    {
      return FunctionOverloadResolver.ResolveFunctionOverloads<EdmFunction, FunctionParameter>(functionsMetadata, argTypes, (Func<EdmFunction, IList<FunctionParameter>>) (edmFunction => (IList<FunctionParameter>) edmFunction.Parameters), (Func<FunctionParameter, TypeUsage>) (functionParameter => functionParameter.TypeUsage), (Func<FunctionParameter, ParameterMode>) (functionParameter => functionParameter.Mode), flattenArgumentType, flattenParameterType, isPromotableTo, isStructurallyEqual, isGroupAggregateFunction, out isAmbiguous);
    }

    internal static TFunctionMetadata ResolveFunctionOverloads<TFunctionMetadata, TFunctionParameterMetadata>(
      IList<TFunctionMetadata> functionsMetadata,
      IList<TypeUsage> argTypes,
      Func<TFunctionMetadata, IList<TFunctionParameterMetadata>> getSignatureParams,
      Func<TFunctionParameterMetadata, TypeUsage> getParameterTypeUsage,
      Func<TFunctionParameterMetadata, ParameterMode> getParameterMode,
      Func<TypeUsage, IEnumerable<TypeUsage>> flattenArgumentType,
      Func<TypeUsage, TypeUsage, IEnumerable<TypeUsage>> flattenParameterType,
      Func<TypeUsage, TypeUsage, bool> isPromotableTo,
      Func<TypeUsage, TypeUsage, bool> isStructurallyEqual,
      bool isGroupAggregateFunction,
      out bool isAmbiguous)
      where TFunctionMetadata : class
    {
      List<TypeUsage> typeUsageList = new List<TypeUsage>(argTypes.Count);
      foreach (TypeUsage argType in (IEnumerable<TypeUsage>) argTypes)
        typeUsageList.AddRange(flattenArgumentType(argType));
      TFunctionMetadata functionMetadata = default (TFunctionMetadata);
      isAmbiguous = false;
      List<int[]> source = new List<int[]>(functionsMetadata.Count);
      int[] bestCandidateRank = (int[]) null;
      int index1 = 0;
      int num = int.MinValue;
      for (; index1 < functionsMetadata.Count; ++index1)
      {
        int totalRank;
        int[] parameterRanks;
        if (FunctionOverloadResolver.TryRankFunctionParameters<TFunctionParameterMetadata>(argTypes, (IList<TypeUsage>) typeUsageList, getSignatureParams(functionsMetadata[index1]), getParameterTypeUsage, getParameterMode, flattenParameterType, isPromotableTo, isStructurallyEqual, isGroupAggregateFunction, out totalRank, out parameterRanks))
        {
          if (totalRank == num)
            isAmbiguous = true;
          else if (totalRank > num)
          {
            isAmbiguous = false;
            num = totalRank;
            functionMetadata = functionsMetadata[index1];
            bestCandidateRank = parameterRanks;
          }
          source.Add(parameterRanks);
        }
      }
      if ((object) functionMetadata != null && !isAmbiguous && (typeUsageList.Count > 1 && source.Count > 1))
        isAmbiguous = source.Any<int[]>((Func<int[], bool>) (rank =>
        {
          if (!object.ReferenceEquals((object) bestCandidateRank, (object) rank))
          {
            for (int index2 = 0; index2 < rank.Length; ++index2)
            {
              if (bestCandidateRank[index2] < rank[index2])
                return true;
            }
          }
          return false;
        }));
      if (!isAmbiguous)
        return functionMetadata;
      return default (TFunctionMetadata);
    }

    private static bool TryRankFunctionParameters<TFunctionParameterMetadata>(
      IList<TypeUsage> argumentList,
      IList<TypeUsage> flatArgumentList,
      IList<TFunctionParameterMetadata> overloadParamList,
      Func<TFunctionParameterMetadata, TypeUsage> getParameterTypeUsage,
      Func<TFunctionParameterMetadata, ParameterMode> getParameterMode,
      Func<TypeUsage, TypeUsage, IEnumerable<TypeUsage>> flattenParameterType,
      Func<TypeUsage, TypeUsage, bool> isPromotableTo,
      Func<TypeUsage, TypeUsage, bool> isStructurallyEqual,
      bool isGroupAggregateFunction,
      out int totalRank,
      out int[] parameterRanks)
    {
      totalRank = 0;
      parameterRanks = (int[]) null;
      if (argumentList.Count != overloadParamList.Count)
        return false;
      List<TypeUsage> typeUsageList = new List<TypeUsage>(flatArgumentList.Count);
      for (int index = 0; index < overloadParamList.Count; ++index)
      {
        TypeUsage typeUsage = argumentList[index];
        TypeUsage type = getParameterTypeUsage(overloadParamList[index]);
        switch (getParameterMode(overloadParamList[index]))
        {
          case ParameterMode.In:
          case ParameterMode.InOut:
            if (isGroupAggregateFunction)
            {
              if (!TypeSemantics.IsCollectionType(type))
                throw new EntitySqlException(Strings.InvalidArgumentTypeForAggregateFunction);
              type = TypeHelpers.GetElementTypeUsage(type);
            }
            if (!isPromotableTo(typeUsage, type))
              return false;
            typeUsageList.AddRange(flattenParameterType(type, typeUsage));
            continue;
          default:
            return false;
        }
      }
      parameterRanks = new int[typeUsageList.Count];
      for (int index = 0; index < parameterRanks.Length; ++index)
      {
        int promotionRank = FunctionOverloadResolver.GetPromotionRank(flatArgumentList[index], typeUsageList[index], isPromotableTo, isStructurallyEqual);
        totalRank += promotionRank;
        parameterRanks[index] = promotionRank;
      }
      return true;
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "isPromotableTo")]
    private static int GetPromotionRank(
      TypeUsage fromType,
      TypeUsage toType,
      Func<TypeUsage, TypeUsage, bool> isPromotableTo,
      Func<TypeUsage, TypeUsage, bool> isStructurallyEqual)
    {
      if (isStructurallyEqual(fromType, toType))
        return 0;
      PrimitiveType edmType1 = fromType.EdmType as PrimitiveType;
      PrimitiveType edmType2 = toType.EdmType as PrimitiveType;
      if (edmType1 != null && edmType2 != null)
      {
        if (Helper.AreSameSpatialUnionType(edmType1, edmType2))
          return 0;
        int num = EdmProviderManifest.Instance.GetPromotionTypes(edmType1).IndexOf(edmType2);
        if (num < 0)
          throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.FailedToGeneratePromotionRank, 1, (object) null);
        return -num;
      }
      EntityTypeBase edmType3 = fromType.EdmType as EntityTypeBase;
      EntityTypeBase edmType4 = toType.EdmType as EntityTypeBase;
      if (edmType3 == null || edmType4 == null)
        throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.FailedToGeneratePromotionRank, 3, (object) null);
      int num1 = 0;
      EdmType edmType5 = (EdmType) edmType3;
      while (edmType5 != edmType4 && edmType5 != null)
      {
        edmType5 = edmType5.BaseType;
        ++num1;
      }
      if (edmType5 == null)
        throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.FailedToGeneratePromotionRank, 2, (object) null);
      return -num1;
    }
  }
}
