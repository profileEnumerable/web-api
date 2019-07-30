// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Internal.Materialization.CodeGenEmitter
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Core.Objects.ELinq;
using System.Data.Entity.Core.Objects.Internal;
using System.Data.Entity.Core.Query.InternalTrees;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Data.Entity.Core.Common.Internal.Materialization
{
  internal static class CodeGenEmitter
  {
    internal static readonly MethodInfo CodeGenEmitter_BinaryEquals = typeof (CodeGenEmitter).GetOnlyDeclaredMethod("BinaryEquals");
    internal static readonly MethodInfo CodeGenEmitter_CheckedConvert = typeof (CodeGenEmitter).GetOnlyDeclaredMethod("CheckedConvert");
    internal static readonly MethodInfo CodeGenEmitter_Compile = typeof (CodeGenEmitter).GetDeclaredMethod("Compile", typeof (Expression));
    internal static readonly MethodInfo DbDataReader_GetValue = typeof (DbDataReader).GetOnlyDeclaredMethod("GetValue");
    internal static readonly MethodInfo DbDataReader_GetString = typeof (DbDataReader).GetOnlyDeclaredMethod("GetString");
    internal static readonly MethodInfo DbDataReader_GetInt16 = typeof (DbDataReader).GetOnlyDeclaredMethod("GetInt16");
    internal static readonly MethodInfo DbDataReader_GetInt32 = typeof (DbDataReader).GetOnlyDeclaredMethod("GetInt32");
    internal static readonly MethodInfo DbDataReader_GetInt64 = typeof (DbDataReader).GetOnlyDeclaredMethod("GetInt64");
    internal static readonly MethodInfo DbDataReader_GetBoolean = typeof (DbDataReader).GetOnlyDeclaredMethod("GetBoolean");
    internal static readonly MethodInfo DbDataReader_GetDecimal = typeof (DbDataReader).GetOnlyDeclaredMethod("GetDecimal");
    internal static readonly MethodInfo DbDataReader_GetFloat = typeof (DbDataReader).GetOnlyDeclaredMethod("GetFloat");
    internal static readonly MethodInfo DbDataReader_GetDouble = typeof (DbDataReader).GetOnlyDeclaredMethod("GetDouble");
    internal static readonly MethodInfo DbDataReader_GetDateTime = typeof (DbDataReader).GetOnlyDeclaredMethod("GetDateTime");
    internal static readonly MethodInfo DbDataReader_GetGuid = typeof (DbDataReader).GetOnlyDeclaredMethod("GetGuid");
    internal static readonly MethodInfo DbDataReader_GetByte = typeof (DbDataReader).GetOnlyDeclaredMethod("GetByte");
    internal static readonly MethodInfo DbDataReader_IsDBNull = typeof (DbDataReader).GetOnlyDeclaredMethod("IsDBNull");
    internal static readonly ConstructorInfo EntityKey_ctor_SingleKey = typeof (EntityKey).GetDeclaredConstructor(typeof (EntitySetBase), typeof (object));
    internal static readonly ConstructorInfo EntityKey_ctor_CompositeKey = typeof (EntityKey).GetDeclaredConstructor(typeof (EntitySetBase), typeof (object[]));
    internal static readonly MethodInfo EntityWrapperFactory_GetEntityWithChangeTrackerStrategyFunc = typeof (EntityWrapperFactory).GetOnlyDeclaredMethod("GetEntityWithChangeTrackerStrategyFunc");
    internal static readonly MethodInfo EntityWrapperFactory_GetEntityWithKeyStrategyStrategyFunc = typeof (EntityWrapperFactory).GetOnlyDeclaredMethod("GetEntityWithKeyStrategyStrategyFunc");
    internal static readonly MethodInfo EntityProxyTypeInfo_SetEntityWrapper = typeof (EntityProxyTypeInfo).GetOnlyDeclaredMethod("SetEntityWrapper");
    internal static readonly MethodInfo EntityWrapperFactory_GetNullPropertyAccessorStrategyFunc = typeof (EntityWrapperFactory).GetOnlyDeclaredMethod("GetNullPropertyAccessorStrategyFunc");
    internal static readonly MethodInfo EntityWrapperFactory_GetPocoEntityKeyStrategyFunc = typeof (EntityWrapperFactory).GetOnlyDeclaredMethod("GetPocoEntityKeyStrategyFunc");
    internal static readonly MethodInfo EntityWrapperFactory_GetPocoPropertyAccessorStrategyFunc = typeof (EntityWrapperFactory).GetOnlyDeclaredMethod("GetPocoPropertyAccessorStrategyFunc");
    internal static readonly MethodInfo EntityWrapperFactory_GetSnapshotChangeTrackingStrategyFunc = typeof (EntityWrapperFactory).GetOnlyDeclaredMethod("GetSnapshotChangeTrackingStrategyFunc");
    internal static readonly PropertyInfo EntityWrapperFactory_NullWrapper = typeof (NullEntityWrapper).GetDeclaredProperty("NullWrapper");
    internal static readonly PropertyInfo IEntityWrapper_Entity = typeof (IEntityWrapper).GetDeclaredProperty("Entity");
    internal static readonly MethodInfo IEqualityComparerOfString_Equals = typeof (IEqualityComparer<string>).GetDeclaredMethod("Equals", typeof (string), typeof (string));
    internal static readonly ConstructorInfo MaterializedDataRecord_ctor = typeof (MaterializedDataRecord).GetDeclaredConstructor(typeof (MetadataWorkspace), typeof (TypeUsage), typeof (object[]));
    internal static readonly MethodInfo RecordState_GatherData = typeof (RecordState).GetOnlyDeclaredMethod("GatherData");
    internal static readonly MethodInfo RecordState_SetNullRecord = typeof (RecordState).GetOnlyDeclaredMethod("SetNullRecord");
    internal static readonly MethodInfo Shaper_Discriminate = typeof (Shaper).GetOnlyDeclaredMethod("Discriminate");
    internal static readonly MethodInfo Shaper_GetPropertyValueWithErrorHandling = typeof (Shaper).GetOnlyDeclaredMethod("GetPropertyValueWithErrorHandling");
    internal static readonly MethodInfo Shaper_GetColumnValueWithErrorHandling = typeof (Shaper).GetOnlyDeclaredMethod("GetColumnValueWithErrorHandling");
    internal static readonly MethodInfo Shaper_GetGeographyColumnValue = typeof (Shaper).GetOnlyDeclaredMethod("GetGeographyColumnValue");
    internal static readonly MethodInfo Shaper_GetGeometryColumnValue = typeof (Shaper).GetOnlyDeclaredMethod("GetGeometryColumnValue");
    internal static readonly MethodInfo Shaper_GetSpatialColumnValueWithErrorHandling = typeof (Shaper).GetOnlyDeclaredMethod("GetSpatialColumnValueWithErrorHandling");
    internal static readonly MethodInfo Shaper_GetSpatialPropertyValueWithErrorHandling = typeof (Shaper).GetOnlyDeclaredMethod("GetSpatialPropertyValueWithErrorHandling");
    internal static readonly MethodInfo Shaper_HandleEntity = typeof (Shaper).GetOnlyDeclaredMethod("HandleEntity");
    internal static readonly MethodInfo Shaper_HandleEntityAppendOnly = typeof (Shaper).GetOnlyDeclaredMethod("HandleEntityAppendOnly");
    internal static readonly MethodInfo Shaper_HandleEntityNoTracking = typeof (Shaper).GetOnlyDeclaredMethod("HandleEntityNoTracking");
    internal static readonly MethodInfo Shaper_HandleFullSpanCollection = typeof (Shaper).GetOnlyDeclaredMethod("HandleFullSpanCollection");
    internal static readonly MethodInfo Shaper_HandleFullSpanElement = typeof (Shaper).GetOnlyDeclaredMethod("HandleFullSpanElement");
    internal static readonly MethodInfo Shaper_HandleIEntityWithKey = typeof (Shaper).GetOnlyDeclaredMethod("HandleIEntityWithKey");
    internal static readonly MethodInfo Shaper_HandleRelationshipSpan = typeof (Shaper).GetOnlyDeclaredMethod("HandleRelationshipSpan");
    internal static readonly MethodInfo Shaper_SetColumnValue = typeof (Shaper).GetOnlyDeclaredMethod("SetColumnValue");
    internal static readonly MethodInfo Shaper_SetEntityRecordInfo = typeof (Shaper).GetOnlyDeclaredMethod("SetEntityRecordInfo");
    internal static readonly MethodInfo Shaper_SetState = typeof (Shaper).GetOnlyDeclaredMethod("SetState");
    internal static readonly MethodInfo Shaper_SetStatePassthrough = typeof (Shaper).GetOnlyDeclaredMethod("SetStatePassthrough");
    internal static readonly Expression DBNull_Value = (Expression) Expression.Constant((object) DBNull.Value, typeof (object));
    internal static readonly ParameterExpression Shaper_Parameter = Expression.Parameter(typeof (Shaper), "shaper");
    internal static readonly Expression Shaper_Reader = (Expression) Expression.Field((Expression) CodeGenEmitter.Shaper_Parameter, typeof (Shaper).GetField("Reader"));
    internal static readonly Expression Shaper_Workspace = (Expression) Expression.Field((Expression) CodeGenEmitter.Shaper_Parameter, typeof (Shaper).GetField("Workspace"));
    internal static readonly Expression Shaper_State = (Expression) Expression.Field((Expression) CodeGenEmitter.Shaper_Parameter, typeof (Shaper).GetField("State"));
    internal static readonly Expression Shaper_Context = (Expression) Expression.Field((Expression) CodeGenEmitter.Shaper_Parameter, typeof (Shaper).GetField("Context"));
    internal static readonly Expression Shaper_Context_Options = (Expression) Expression.Property(CodeGenEmitter.Shaper_Context, typeof (ObjectContext).GetDeclaredProperty("ContextOptions"));
    internal static readonly Expression Shaper_ProxyCreationEnabled = (Expression) Expression.Property(CodeGenEmitter.Shaper_Context_Options, typeof (ObjectContextOptions).GetDeclaredProperty("ProxyCreationEnabled"));

    internal static bool BinaryEquals(byte[] left, byte[] right)
    {
      if (left == null)
        return null == right;
      if (right == null || left.Length != right.Length)
        return false;
      for (int index = 0; index < left.Length; ++index)
      {
        if ((int) left[index] != (int) right[index])
          return false;
      }
      return true;
    }

    internal static Func<Shaper, TResult> Compile<TResult>(Expression body)
    {
      return CodeGenEmitter.BuildShaperLambda<TResult>(body).Compile();
    }

    internal static Expression<Func<Shaper, TResult>> BuildShaperLambda<TResult>(
      Expression body)
    {
      if (body == null)
        return (Expression<Func<Shaper, TResult>>) null;
      return Expression.Lambda<Func<Shaper, TResult>>(body, CodeGenEmitter.Shaper_Parameter);
    }

    internal static object Compile(Type resultType, Expression body)
    {
      return CodeGenEmitter.CodeGenEmitter_Compile.MakeGenericMethod(resultType).Invoke((object) null, new object[1]
      {
        (object) body
      });
    }

    internal static Expression Emit_AndAlso(IEnumerable<Expression> operands)
    {
      Expression left = (Expression) null;
      foreach (Expression operand in operands)
        left = left != null ? (Expression) Expression.AndAlso(left, operand) : operand;
      return left;
    }

    internal static Expression Emit_BitwiseOr(IEnumerable<Expression> operands)
    {
      Expression left = (Expression) null;
      foreach (Expression operand in operands)
        left = left != null ? (Expression) Expression.Or(left, operand) : operand;
      return left;
    }

    internal static Expression Emit_NullConstant(Type type)
    {
      return !type.IsNullable() ? CodeGenEmitter.Emit_EnsureType((Expression) Expression.Constant((object) null, typeof (object)), type) : (Expression) Expression.Constant((object) null, type);
    }

    internal static Expression Emit_WrappedNullConstant()
    {
      return (Expression) Expression.Property((Expression) null, CodeGenEmitter.EntityWrapperFactory_NullWrapper);
    }

    internal static Expression Emit_EnsureType(Expression input, Type type)
    {
      Expression expression = input;
      if (input.Type != type && !typeof (IEntityWrapper).IsAssignableFrom(input.Type))
      {
        if (type.IsAssignableFrom(input.Type))
          expression = (Expression) Expression.Convert(input, type);
        else
          expression = (Expression) Expression.Call(CodeGenEmitter.CodeGenEmitter_CheckedConvert.MakeGenericMethod(input.Type, type), input);
      }
      return expression;
    }

    internal static Expression Emit_EnsureTypeAndWrap(
      Expression input,
      Expression keyReader,
      Expression entitySetReader,
      Type requestedType,
      Type identityType,
      Type actualType,
      MergeOption mergeOption,
      bool isProxy)
    {
      Expression input1 = CodeGenEmitter.Emit_EnsureType(input, requestedType);
      if (!requestedType.IsClass())
        input1 = CodeGenEmitter.Emit_EnsureType(input, typeof (object));
      return CodeGenEmitter.CreateEntityWrapper(CodeGenEmitter.Emit_EnsureType(input1, actualType), keyReader, entitySetReader, actualType, identityType, mergeOption, isProxy);
    }

    internal static Expression CreateEntityWrapper(
      Expression input,
      Expression keyReader,
      Expression entitySetReader,
      Type actualType,
      Type identityType,
      MergeOption mergeOption,
      bool isProxy)
    {
      bool hashCode = actualType.OverridesEqualsOrGetHashCode();
      bool flag1 = typeof (IEntityWithKey).IsAssignableFrom(actualType);
      bool flag2 = typeof (IEntityWithRelationships).IsAssignableFrom(actualType);
      bool flag3 = typeof (IEntityWithChangeTracker).IsAssignableFrom(actualType);
      Expression expression1;
      if (flag2 && flag3 && (flag1 && !isProxy))
      {
        expression1 = (Expression) Expression.New(typeof (LightweightEntityWrapper<>).MakeGenericType(actualType).GetDeclaredConstructor(actualType, typeof (EntityKey), typeof (EntitySet), typeof (ObjectContext), typeof (MergeOption), typeof (Type), typeof (bool)), input, keyReader, entitySetReader, CodeGenEmitter.Shaper_Context, (Expression) Expression.Constant((object) mergeOption, typeof (MergeOption)), (Expression) Expression.Constant((object) identityType, typeof (Type)), (Expression) Expression.Constant((object) hashCode, typeof (bool)));
      }
      else
      {
        Expression expression2 = !flag2 || isProxy ? (Expression) Expression.Call(CodeGenEmitter.EntityWrapperFactory_GetPocoPropertyAccessorStrategyFunc) : (Expression) Expression.Call(CodeGenEmitter.EntityWrapperFactory_GetNullPropertyAccessorStrategyFunc);
        Expression expression3 = flag1 ? (Expression) Expression.Call(CodeGenEmitter.EntityWrapperFactory_GetEntityWithKeyStrategyStrategyFunc) : (Expression) Expression.Call(CodeGenEmitter.EntityWrapperFactory_GetPocoEntityKeyStrategyFunc);
        Expression expression4 = flag3 ? (Expression) Expression.Call(CodeGenEmitter.EntityWrapperFactory_GetEntityWithChangeTrackerStrategyFunc) : (Expression) Expression.Call(CodeGenEmitter.EntityWrapperFactory_GetSnapshotChangeTrackingStrategyFunc);
        Type type;
        if (!flag2)
          type = typeof (EntityWrapperWithoutRelationships<>).MakeGenericType(actualType);
        else
          type = typeof (EntityWrapperWithRelationships<>).MakeGenericType(actualType);
        expression1 = (Expression) Expression.New(type.GetDeclaredConstructor(actualType, typeof (EntityKey), typeof (EntitySet), typeof (ObjectContext), typeof (MergeOption), typeof (Type), typeof (Func<object, IPropertyAccessorStrategy>), typeof (Func<object, IChangeTrackingStrategy>), typeof (Func<object, IEntityKeyStrategy>), typeof (bool)), input, keyReader, entitySetReader, CodeGenEmitter.Shaper_Context, (Expression) Expression.Constant((object) mergeOption, typeof (MergeOption)), (Expression) Expression.Constant((object) identityType, typeof (Type)), expression2, expression4, expression3, (Expression) Expression.Constant((object) hashCode, typeof (bool)));
      }
      return (Expression) Expression.Convert(expression1, typeof (IEntityWrapper));
    }

    internal static Expression Emit_UnwrapAndEnsureType(Expression input, Type type)
    {
      return CodeGenEmitter.Emit_EnsureType((Expression) Expression.Property(input, CodeGenEmitter.IEntityWrapper_Entity), type);
    }

    internal static TTarget CheckedConvert<TSource, TTarget>(TSource value)
    {
      try
      {
        return (TTarget) (object) value;
      }
      catch (InvalidCastException ex)
      {
        Type type = value.GetType();
        if (type.IsGenericType() && type.GetGenericTypeDefinition() == typeof (CompensatingCollection<>))
          type = typeof (IEnumerable<>).MakeGenericType(type.GetGenericArguments());
        throw EntityUtil.ValueInvalidCast(type, typeof (TTarget));
      }
      catch (NullReferenceException ex)
      {
        throw new InvalidOperationException(Strings.Materializer_NullReferenceCast((object) typeof (TTarget).Name));
      }
    }

    internal static Expression Emit_Equal(Expression left, Expression right)
    {
      return !(typeof (byte[]) == left.Type) ? (Expression) Expression.Equal(left, right) : (Expression) Expression.Call(CodeGenEmitter.CodeGenEmitter_BinaryEquals, left, right);
    }

    internal static Expression Emit_EntityKey_HasValue(SimpleColumnMap[] keyColumns)
    {
      return (Expression) Expression.Not(CodeGenEmitter.Emit_Reader_IsDBNull((ColumnMap) keyColumns[0]));
    }

    internal static Expression Emit_Reader_GetValue(int ordinal, Type type)
    {
      return CodeGenEmitter.Emit_EnsureType((Expression) Expression.Call(CodeGenEmitter.Shaper_Reader, CodeGenEmitter.DbDataReader_GetValue, (Expression) Expression.Constant((object) ordinal)), type);
    }

    internal static Expression Emit_Reader_IsDBNull(int ordinal)
    {
      return (Expression) Expression.Call(CodeGenEmitter.Shaper_Reader, CodeGenEmitter.DbDataReader_IsDBNull, (Expression) Expression.Constant((object) ordinal));
    }

    internal static Expression Emit_Reader_IsDBNull(ColumnMap columnMap)
    {
      return CodeGenEmitter.Emit_Reader_IsDBNull(((ScalarColumnMap) columnMap).ColumnPos);
    }

    internal static Expression Emit_Conditional_NotDBNull(
      Expression result,
      int ordinal,
      Type columnType)
    {
      result = (Expression) Expression.Condition(CodeGenEmitter.Emit_Reader_IsDBNull(ordinal), (Expression) Expression.Constant(TypeSystem.GetDefaultValue(columnType), columnType), result);
      return result;
    }

    internal static MethodInfo GetReaderMethod(Type type, out bool isNullable)
    {
      isNullable = false;
      Type underlyingType = Nullable.GetUnderlyingType(type);
      if ((Type) null != underlyingType)
      {
        isNullable = true;
        type = underlyingType;
      }
      MethodInfo methodInfo;
      switch (Type.GetTypeCode(type))
      {
        case TypeCode.Boolean:
          methodInfo = CodeGenEmitter.DbDataReader_GetBoolean;
          break;
        case TypeCode.Byte:
          methodInfo = CodeGenEmitter.DbDataReader_GetByte;
          break;
        case TypeCode.Int16:
          methodInfo = CodeGenEmitter.DbDataReader_GetInt16;
          break;
        case TypeCode.Int32:
          methodInfo = CodeGenEmitter.DbDataReader_GetInt32;
          break;
        case TypeCode.Int64:
          methodInfo = CodeGenEmitter.DbDataReader_GetInt64;
          break;
        case TypeCode.Single:
          methodInfo = CodeGenEmitter.DbDataReader_GetFloat;
          break;
        case TypeCode.Double:
          methodInfo = CodeGenEmitter.DbDataReader_GetDouble;
          break;
        case TypeCode.Decimal:
          methodInfo = CodeGenEmitter.DbDataReader_GetDecimal;
          break;
        case TypeCode.DateTime:
          methodInfo = CodeGenEmitter.DbDataReader_GetDateTime;
          break;
        case TypeCode.String:
          methodInfo = CodeGenEmitter.DbDataReader_GetString;
          isNullable = true;
          break;
        default:
          if (typeof (Guid) == type)
          {
            methodInfo = CodeGenEmitter.DbDataReader_GetGuid;
            break;
          }
          if (typeof (TimeSpan) == type || typeof (DateTimeOffset) == type)
          {
            methodInfo = CodeGenEmitter.DbDataReader_GetValue;
            break;
          }
          if (typeof (object) == type)
          {
            methodInfo = CodeGenEmitter.DbDataReader_GetValue;
            break;
          }
          methodInfo = CodeGenEmitter.DbDataReader_GetValue;
          isNullable = true;
          break;
      }
      return methodInfo;
    }

    internal static Expression Emit_Shaper_GetPropertyValueWithErrorHandling(
      Type propertyType,
      int ordinal,
      string propertyName,
      string typeName,
      TypeUsage columnType)
    {
      PrimitiveTypeKind spatialType;
      Expression expression;
      if (Helper.IsSpatialType(columnType, out spatialType))
        expression = (Expression) Expression.Call((Expression) CodeGenEmitter.Shaper_Parameter, CodeGenEmitter.Shaper_GetSpatialPropertyValueWithErrorHandling.MakeGenericMethod(propertyType), (Expression) Expression.Constant((object) ordinal), (Expression) Expression.Constant((object) propertyName), (Expression) Expression.Constant((object) typeName), (Expression) Expression.Constant((object) spatialType, typeof (PrimitiveTypeKind)));
      else
        expression = (Expression) Expression.Call((Expression) CodeGenEmitter.Shaper_Parameter, CodeGenEmitter.Shaper_GetPropertyValueWithErrorHandling.MakeGenericMethod(propertyType), (Expression) Expression.Constant((object) ordinal), (Expression) Expression.Constant((object) propertyName), (Expression) Expression.Constant((object) typeName));
      return expression;
    }

    internal static Expression Emit_Shaper_GetColumnValueWithErrorHandling(
      Type resultType,
      int ordinal,
      TypeUsage columnType)
    {
      PrimitiveTypeKind spatialType;
      Expression expression;
      if (Helper.IsSpatialType(columnType, out spatialType))
      {
        PrimitiveTypeKind primitiveTypeKind = Helper.IsGeographicType((PrimitiveType) columnType.EdmType) ? PrimitiveTypeKind.Geography : PrimitiveTypeKind.Geometry;
        expression = (Expression) Expression.Call((Expression) CodeGenEmitter.Shaper_Parameter, CodeGenEmitter.Shaper_GetSpatialColumnValueWithErrorHandling.MakeGenericMethod(resultType), (Expression) Expression.Constant((object) ordinal), (Expression) Expression.Constant((object) primitiveTypeKind, typeof (PrimitiveTypeKind)));
      }
      else
        expression = (Expression) Expression.Call((Expression) CodeGenEmitter.Shaper_Parameter, CodeGenEmitter.Shaper_GetColumnValueWithErrorHandling.MakeGenericMethod(resultType), (Expression) Expression.Constant((object) ordinal));
      return expression;
    }

    internal static Expression Emit_Shaper_GetGeographyColumnValue(int ordinal)
    {
      return (Expression) Expression.Call((Expression) CodeGenEmitter.Shaper_Parameter, CodeGenEmitter.Shaper_GetGeographyColumnValue, (Expression) Expression.Constant((object) ordinal));
    }

    internal static Expression Emit_Shaper_GetGeometryColumnValue(int ordinal)
    {
      return (Expression) Expression.Call((Expression) CodeGenEmitter.Shaper_Parameter, CodeGenEmitter.Shaper_GetGeometryColumnValue, (Expression) Expression.Constant((object) ordinal));
    }

    internal static Expression Emit_Shaper_GetState(int stateSlotNumber, Type type)
    {
      return CodeGenEmitter.Emit_EnsureType((Expression) Expression.ArrayIndex(CodeGenEmitter.Shaper_State, (Expression) Expression.Constant((object) stateSlotNumber)), type);
    }

    internal static Expression Emit_Shaper_SetState(int stateSlotNumber, Expression value)
    {
      return (Expression) Expression.Call((Expression) CodeGenEmitter.Shaper_Parameter, CodeGenEmitter.Shaper_SetState.MakeGenericMethod(value.Type), (Expression) Expression.Constant((object) stateSlotNumber), value);
    }

    internal static Expression Emit_Shaper_SetStatePassthrough(
      int stateSlotNumber,
      Expression value)
    {
      return (Expression) Expression.Call((Expression) CodeGenEmitter.Shaper_Parameter, CodeGenEmitter.Shaper_SetStatePassthrough.MakeGenericMethod(value.Type), (Expression) Expression.Constant((object) stateSlotNumber), value);
    }
  }
}
