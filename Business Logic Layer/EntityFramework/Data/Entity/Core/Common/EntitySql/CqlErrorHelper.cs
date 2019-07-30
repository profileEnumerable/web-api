// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.CqlErrorHelper
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.EntitySql.AST;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace System.Data.Entity.Core.Common.EntitySql
{
  internal static class CqlErrorHelper
  {
    internal static void ReportFunctionOverloadError(
      MethodExpr functionExpr,
      EdmFunction functionType,
      List<TypeUsage> argTypes)
    {
      string str = "";
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(functionType.Name).Append("(");
      for (int index = 0; index < argTypes.Count; ++index)
      {
        stringBuilder.Append(str);
        stringBuilder.Append(argTypes[index] != null ? argTypes[index].EdmType.FullName : "NULL");
        str = ", ";
      }
      stringBuilder.Append(")");
      Func<object, object, object, string> func = !TypeSemantics.IsAggregateFunction(functionType) ? (TypeHelpers.IsCanonicalFunction(functionType) ? new Func<object, object, object, string>(Strings.NoCanonicalFunctionOverloadMatch) : new Func<object, object, object, string>(Strings.NoFunctionOverloadMatch)) : (TypeHelpers.IsCanonicalFunction(functionType) ? new Func<object, object, object, string>(Strings.NoCanonicalAggrFunctionOverloadMatch) : new Func<object, object, object, string>(Strings.NoAggrFunctionOverloadMatch));
      throw EntitySqlException.Create(functionExpr.ErrCtx.CommandText, func((object) functionType.NamespaceName, (object) functionType.Name, (object) stringBuilder.ToString()), functionExpr.ErrCtx.InputPosition, Strings.CtxFunction((object) functionType.Name), false, (Exception) null);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.EntityUtil.EntitySqlError(System.Data.Entity.Core.Common.EntitySql.ErrorContext,System.String)")]
    internal static void ReportAliasAlreadyUsedError(
      string aliasName,
      ErrorContext errCtx,
      string contextMessage)
    {
      throw EntitySqlException.Create(errCtx, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) Strings.AliasNameAlreadyUsed((object) aliasName), (object) contextMessage), (Exception) null);
    }

    internal static void ReportIncompatibleCommonType(
      ErrorContext errCtx,
      TypeUsage leftType,
      TypeUsage rightType)
    {
      CqlErrorHelper.ReportIncompatibleCommonType(errCtx, leftType, rightType, leftType, rightType);
      throw EntitySqlException.Create(errCtx, Strings.ArgumentTypesAreIncompatible((object) leftType.Identity, (object) rightType.Identity), (Exception) null);
    }

    private static void ReportIncompatibleCommonType(
      ErrorContext errCtx,
      TypeUsage rootLeftType,
      TypeUsage rootRightType,
      TypeUsage leftType,
      TypeUsage rightType)
    {
      TypeUsage commonType = (TypeUsage) null;
      bool flag = rootLeftType == leftType;
      string empty = string.Empty;
      if (leftType.EdmType.BuiltInTypeKind != rightType.EdmType.BuiltInTypeKind)
        throw EntitySqlException.Create(errCtx, Strings.TypeKindMismatch((object) CqlErrorHelper.GetReadableTypeKind(leftType), (object) CqlErrorHelper.GetReadableTypeName(leftType), (object) CqlErrorHelper.GetReadableTypeKind(rightType), (object) CqlErrorHelper.GetReadableTypeName(rightType)), (Exception) null);
      switch (leftType.EdmType.BuiltInTypeKind)
      {
        case BuiltInTypeKind.CollectionType:
        case BuiltInTypeKind.RefType:
          CqlErrorHelper.ReportIncompatibleCommonType(errCtx, rootLeftType, rootRightType, TypeHelpers.GetElementTypeUsage(leftType), TypeHelpers.GetElementTypeUsage(rightType));
          break;
        case BuiltInTypeKind.ComplexType:
          ComplexType edmType1 = (ComplexType) leftType.EdmType;
          ComplexType edmType2 = (ComplexType) rightType.EdmType;
          if (edmType1.Members.Count != edmType2.Members.Count)
          {
            string errorMessage = !flag ? Strings.InvalidComplexType((object) CqlErrorHelper.GetReadableTypeName((EdmType) edmType1), (object) CqlErrorHelper.GetReadableTypeName(rootLeftType), (object) CqlErrorHelper.GetReadableTypeName((EdmType) edmType2), (object) CqlErrorHelper.GetReadableTypeName(rootRightType)) : Strings.InvalidRootComplexType((object) CqlErrorHelper.GetReadableTypeName((EdmType) edmType1), (object) CqlErrorHelper.GetReadableTypeName((EdmType) edmType2));
            throw EntitySqlException.Create(errCtx, errorMessage, (Exception) null);
          }
          for (int index = 0; index < edmType1.Members.Count; ++index)
            CqlErrorHelper.ReportIncompatibleCommonType(errCtx, rootLeftType, rootRightType, edmType1.Members[index].TypeUsage, edmType2.Members[index].TypeUsage);
          break;
        case BuiltInTypeKind.EntityType:
          if (TypeSemantics.TryGetCommonType(leftType, rightType, out commonType))
            break;
          string errorMessage1 = !flag ? Strings.InvalidEntityTypeArgument((object) CqlErrorHelper.GetReadableTypeName(leftType), (object) CqlErrorHelper.GetReadableTypeName(rootLeftType), (object) CqlErrorHelper.GetReadableTypeName(rightType), (object) CqlErrorHelper.GetReadableTypeName(rootRightType)) : Strings.InvalidEntityRootTypeArgument((object) CqlErrorHelper.GetReadableTypeName(leftType), (object) CqlErrorHelper.GetReadableTypeName(rightType));
          throw EntitySqlException.Create(errCtx, errorMessage1, (Exception) null);
        case BuiltInTypeKind.RowType:
          RowType edmType3 = (RowType) leftType.EdmType;
          RowType edmType4 = (RowType) rightType.EdmType;
          if (edmType3.Members.Count != edmType4.Members.Count)
          {
            string errorMessage2 = !flag ? Strings.InvalidRowType((object) CqlErrorHelper.GetReadableTypeName((EdmType) edmType3), (object) CqlErrorHelper.GetReadableTypeName(rootLeftType), (object) CqlErrorHelper.GetReadableTypeName((EdmType) edmType4), (object) CqlErrorHelper.GetReadableTypeName(rootRightType)) : Strings.InvalidRootRowType((object) CqlErrorHelper.GetReadableTypeName((EdmType) edmType3), (object) CqlErrorHelper.GetReadableTypeName((EdmType) edmType4));
            throw EntitySqlException.Create(errCtx, errorMessage2, (Exception) null);
          }
          for (int index = 0; index < edmType3.Members.Count; ++index)
            CqlErrorHelper.ReportIncompatibleCommonType(errCtx, rootLeftType, rootRightType, edmType3.Members[index].TypeUsage, edmType4.Members[index].TypeUsage);
          break;
        default:
          if (TypeSemantics.TryGetCommonType(leftType, rightType, out commonType))
            break;
          string errorMessage3 = !flag ? Strings.InvalidPlaceholderTypeArgument((object) CqlErrorHelper.GetReadableTypeKind(leftType), (object) CqlErrorHelper.GetReadableTypeName(leftType), (object) CqlErrorHelper.GetReadableTypeName(rootLeftType), (object) CqlErrorHelper.GetReadableTypeKind(rightType), (object) CqlErrorHelper.GetReadableTypeName(rightType), (object) CqlErrorHelper.GetReadableTypeName(rootRightType)) : Strings.InvalidPlaceholderRootTypeArgument((object) CqlErrorHelper.GetReadableTypeKind(leftType), (object) CqlErrorHelper.GetReadableTypeName(leftType), (object) CqlErrorHelper.GetReadableTypeKind(rightType), (object) CqlErrorHelper.GetReadableTypeName(rightType));
          throw EntitySqlException.Create(errCtx, errorMessage3, (Exception) null);
      }
    }

    private static string GetReadableTypeName(TypeUsage type)
    {
      return CqlErrorHelper.GetReadableTypeName(type.EdmType);
    }

    private static string GetReadableTypeName(EdmType type)
    {
      if (type.BuiltInTypeKind == BuiltInTypeKind.RowType || type.BuiltInTypeKind == BuiltInTypeKind.CollectionType || type.BuiltInTypeKind == BuiltInTypeKind.RefType)
        return type.Name;
      return type.FullName;
    }

    private static string GetReadableTypeKind(TypeUsage type)
    {
      return CqlErrorHelper.GetReadableTypeKind(type.EdmType);
    }

    private static string GetReadableTypeKind(EdmType type)
    {
      string empty = string.Empty;
      string str;
      switch (type.BuiltInTypeKind)
      {
        case BuiltInTypeKind.CollectionType:
          str = Strings.LocalizedCollection;
          break;
        case BuiltInTypeKind.ComplexType:
          str = Strings.LocalizedComplex;
          break;
        case BuiltInTypeKind.EntityType:
          str = Strings.LocalizedEntity;
          break;
        case BuiltInTypeKind.PrimitiveType:
          str = Strings.LocalizedPrimitive;
          break;
        case BuiltInTypeKind.RefType:
          str = Strings.LocalizedReference;
          break;
        case BuiltInTypeKind.RowType:
          str = Strings.LocalizedRow;
          break;
        default:
          str = type.BuiltInTypeKind.ToString();
          break;
      }
      return str + " " + Strings.LocalizedType;
    }
  }
}
