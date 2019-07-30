// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.EntityUtil
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.Internal;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Data.Entity.Core
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
  internal static class EntityUtil
  {
    internal static Dictionary<string, string> COMPILER_VERSION = new Dictionary<string, string>()
    {
      {
        "CompilerVersion",
        "V3.5"
      }
    };
    internal const int AssemblyQualifiedNameIndex = 3;
    internal const int InvariantNameIndex = 2;
    internal const string Parameter = "Parameter";
    internal const CompareOptions StringCompareOptions = CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth;

    internal static IEnumerable<KeyValuePair<T1, T2>> Zip<T1, T2>(
      this IEnumerable<T1> first,
      IEnumerable<T2> second)
    {
      if (first != null && second != null)
      {
        using (IEnumerator<T1> firstEnumerator = first.GetEnumerator())
        {
          using (IEnumerator<T2> secondEnumerator = second.GetEnumerator())
          {
            while (firstEnumerator.MoveNext() && secondEnumerator.MoveNext())
              yield return new KeyValuePair<T1, T2>(firstEnumerator.Current, secondEnumerator.Current);
          }
        }
      }
    }

    internal static bool IsAnICollection(Type type)
    {
      if (!typeof (ICollection<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
        return type.GetInterface(typeof (ICollection<>).FullName) != (Type) null;
      return true;
    }

    internal static Type GetCollectionElementType(Type propertyType)
    {
      Type elementType = propertyType.TryGetElementType(typeof (ICollection<>));
      if (elementType == (Type) null)
        throw new InvalidOperationException(Strings.PocoEntityWrapper_UnexpectedTypeForNavigationProperty((object) propertyType.FullName, (object) typeof (ICollection<>)));
      return elementType;
    }

    internal static Type DetermineCollectionType(Type requestedType)
    {
      Type collectionElementType = EntityUtil.GetCollectionElementType(requestedType);
      if (requestedType.IsArray)
        throw new InvalidOperationException(Strings.ObjectQuery_UnableToMaterializeArray((object) requestedType, (object) typeof (List<>).MakeGenericType(collectionElementType)));
      if (!requestedType.IsAbstract() && requestedType.GetPublicConstructor() != (ConstructorInfo) null)
        return requestedType;
      Type c1 = typeof (HashSet<>).MakeGenericType(collectionElementType);
      if (requestedType.IsAssignableFrom(c1))
        return c1;
      Type c2 = typeof (List<>).MakeGenericType(collectionElementType);
      if (requestedType.IsAssignableFrom(c2))
        return c2;
      return (Type) null;
    }

    internal static Type GetEntityIdentityType(Type entityType)
    {
      if (!EntityProxyFactory.IsProxyType(entityType))
        return entityType;
      return entityType.BaseType();
    }

    internal static string QuoteIdentifier(string identifier)
    {
      return "[" + identifier.Replace("]", "]]") + "]";
    }

    internal static MetadataException InvalidSchemaEncountered(string errors)
    {
      return new MetadataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, EntityRes.GetString(nameof (InvalidSchemaEncountered)), (object) errors));
    }

    internal static Exception InternalError(
      EntityUtil.InternalErrorCode internalError,
      int location,
      object additionalInfo)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat("{0}, {1}", (object) (int) internalError, (object) location);
      if (additionalInfo != null)
        stringBuilder.AppendFormat(", {0}", additionalInfo);
      return (Exception) new InvalidOperationException(Strings.ADP_InternalProviderError((object) stringBuilder.ToString()));
    }

    internal static void CheckValidStateForChangeEntityState(EntityState state)
    {
      switch (state)
      {
        case EntityState.Detached:
          break;
        case EntityState.Unchanged:
          break;
        case EntityState.Added:
          break;
        case EntityState.Deleted:
          break;
        case EntityState.Modified:
          break;
        default:
          throw new ArgumentException(Strings.ObjectContext_InvalidEntityState, nameof (state));
      }
    }

    internal static void CheckValidStateForChangeRelationshipState(
      EntityState state,
      string paramName)
    {
      switch (state)
      {
        case EntityState.Detached:
          break;
        case EntityState.Unchanged:
          break;
        case EntityState.Added:
          break;
        case EntityState.Deleted:
          break;
        default:
          throw new ArgumentException(Strings.ObjectContext_InvalidRelationshipState, paramName);
      }
    }

    internal static void ThrowPropertyIsNotNullable(string propertyName)
    {
      if (string.IsNullOrEmpty(propertyName))
        throw new ConstraintException(Strings.Materializer_PropertyIsNotNullable);
      throw new PropertyConstraintException(Strings.Materializer_PropertyIsNotNullableWithName((object) propertyName), propertyName);
    }

    internal static void ThrowSetInvalidValue(
      object value,
      Type destinationType,
      string className,
      string propertyName)
    {
      if (value == null)
      {
        Type type = Nullable.GetUnderlyingType(destinationType);
        if ((object) type == null)
          type = destinationType;
        throw new ConstraintException(Strings.Materializer_SetInvalidValue((object) type.Name, (object) className, (object) propertyName, (object) "null"));
      }
      Type type1 = Nullable.GetUnderlyingType(destinationType);
      if ((object) type1 == null)
        type1 = destinationType;
      throw new InvalidOperationException(Strings.Materializer_SetInvalidValue((object) type1.Name, (object) className, (object) propertyName, (object) value.GetType().Name));
    }

    internal static InvalidOperationException ValueInvalidCast(
      Type valueType,
      Type destinationType)
    {
      if (destinationType.IsValueType() && destinationType.IsGenericType() && typeof (Nullable<>) == destinationType.GetGenericTypeDefinition())
        return new InvalidOperationException(Strings.Materializer_InvalidCastNullable((object) valueType, (object) destinationType.GetGenericArguments()[0]));
      return new InvalidOperationException(Strings.Materializer_InvalidCastReference((object) valueType, (object) destinationType));
    }

    internal static void CheckArgumentMergeOption(MergeOption mergeOption)
    {
      switch (mergeOption)
      {
        case MergeOption.AppendOnly:
          break;
        case MergeOption.OverwriteChanges:
          break;
        case MergeOption.PreserveChanges:
          break;
        case MergeOption.NoTracking:
          break;
        default:
          throw new ArgumentOutOfRangeException(typeof (MergeOption).Name, Strings.ADP_InvalidEnumerationValue((object) typeof (MergeOption).Name, (object) ((int) mergeOption).ToString((IFormatProvider) CultureInfo.InvariantCulture)));
      }
    }

    internal static void CheckArgumentRefreshMode(RefreshMode refreshMode)
    {
      if (refreshMode != RefreshMode.ClientWins && refreshMode != RefreshMode.StoreWins)
        throw new ArgumentOutOfRangeException(typeof (RefreshMode).Name, Strings.ADP_InvalidEnumerationValue((object) typeof (RefreshMode).Name, (object) ((int) refreshMode).ToString((IFormatProvider) CultureInfo.InvariantCulture)));
    }

    internal static InvalidOperationException ExecuteFunctionCalledWithNonReaderFunction(
      EdmFunction functionImport)
    {
      return new InvalidOperationException(functionImport.ReturnParameter != null ? Strings.ObjectContext_ExecuteFunctionCalledWithScalarFunction((object) functionImport.ReturnParameter.TypeUsage.EdmType.FullName, (object) functionImport.Name) : Strings.ObjectContext_ExecuteFunctionCalledWithNonQueryFunction((object) functionImport.Name));
    }

    internal static void ValidateEntitySetInKey(EntityKey key, EntitySet entitySet)
    {
      EntityUtil.ValidateEntitySetInKey(key, entitySet, (string) null);
    }

    internal static void ValidateEntitySetInKey(
      EntityKey key,
      EntitySet entitySet,
      string argument)
    {
      string entityContainerName = key.EntityContainerName;
      string entitySetName = key.EntitySetName;
      string name1 = entitySet.EntityContainer.Name;
      string name2 = entitySet.Name;
      if (StringComparer.Ordinal.Equals(entityContainerName, name1) && StringComparer.Ordinal.Equals(entitySetName, name2))
        return;
      if (string.IsNullOrEmpty(argument))
        throw new InvalidOperationException(Strings.ObjectContext_InvalidEntitySetInKey((object) entityContainerName, (object) entitySetName, (object) name1, (object) name2));
      throw new InvalidOperationException(Strings.ObjectContext_InvalidEntitySetInKeyFromName((object) entityContainerName, (object) entitySetName, (object) name1, (object) name2, (object) argument));
    }

    internal static void ValidateNecessaryModificationFunctionMapping(
      ModificationFunctionMapping mapping,
      string currentState,
      IEntityStateEntry stateEntry,
      string type,
      string typeName)
    {
      if (mapping == null)
        throw new UpdateException(Strings.Update_MissingFunctionMapping((object) currentState, (object) type, (object) typeName), (Exception) null, new List<IEntityStateEntry>()
        {
          stateEntry
        }.Cast<ObjectStateEntry>().Distinct<ObjectStateEntry>());
    }

    internal static UpdateException Update(
      string message,
      Exception innerException,
      params IEntityStateEntry[] stateEntries)
    {
      return new UpdateException(message, innerException, stateEntries.Cast<ObjectStateEntry>().Distinct<ObjectStateEntry>());
    }

    internal static UpdateException UpdateRelationshipCardinalityConstraintViolation(
      string relationshipSetName,
      int minimumCount,
      int? maximumCount,
      string entitySetName,
      int actualCount,
      string otherEndPluralName,
      IEntityStateEntry stateEntry)
    {
      string str1 = EntityUtil.ConvertCardinalityToString(new int?(minimumCount));
      string str2 = EntityUtil.ConvertCardinalityToString(maximumCount);
      string str3 = EntityUtil.ConvertCardinalityToString(new int?(actualCount));
      if (minimumCount == 1 && str1 == str2)
        return EntityUtil.Update(Strings.Update_RelationshipCardinalityConstraintViolationSingleValue((object) entitySetName, (object) relationshipSetName, (object) str3, (object) otherEndPluralName, (object) str1), (Exception) null, stateEntry);
      return EntityUtil.Update(Strings.Update_RelationshipCardinalityConstraintViolation((object) entitySetName, (object) relationshipSetName, (object) str3, (object) otherEndPluralName, (object) str1, (object) str2), (Exception) null, stateEntry);
    }

    private static string ConvertCardinalityToString(int? cardinality)
    {
      if (cardinality.HasValue)
        return cardinality.Value.ToString((IFormatProvider) CultureInfo.CurrentCulture);
      return "*";
    }

    internal static T CheckArgumentOutOfRange<T>(T[] values, int index, string parameterName)
    {
      if ((uint) values.Length <= (uint) index)
        throw new ArgumentOutOfRangeException(parameterName);
      return values[index];
    }

    internal static IEnumerable<T> CheckArgumentContainsNull<T>(
      ref IEnumerable<T> enumerableArgument,
      string argumentName)
      where T : class
    {
      EntityUtil.GetCheapestSafeEnumerableAsCollection<T>(ref enumerableArgument);
      foreach (T obj in enumerableArgument)
      {
        if ((object) obj == null)
          throw new ArgumentException(Strings.CheckArgumentContainsNullFailed((object) argumentName));
      }
      return enumerableArgument;
    }

    internal static IEnumerable<T> CheckArgumentEmpty<T>(
      ref IEnumerable<T> enumerableArgument,
      Func<string, string> errorMessage,
      string argumentName)
    {
      int count;
      EntityUtil.GetCheapestSafeCountOfEnumerable<T>(ref enumerableArgument, out count);
      if (count <= 0)
        throw new ArgumentException(errorMessage(argumentName));
      return enumerableArgument;
    }

    private static void GetCheapestSafeCountOfEnumerable<T>(
      ref IEnumerable<T> enumerable,
      out int count)
    {
      ICollection<T> enumerableAsCollection = EntityUtil.GetCheapestSafeEnumerableAsCollection<T>(ref enumerable);
      count = enumerableAsCollection.Count;
    }

    private static ICollection<T> GetCheapestSafeEnumerableAsCollection<T>(
      ref IEnumerable<T> enumerable)
    {
      ICollection<T> objs = enumerable as ICollection<T>;
      if (objs != null)
        return objs;
      enumerable = (IEnumerable<T>) new List<T>(enumerable);
      return enumerable as ICollection<T>;
    }

    internal static bool IsNull(object value)
    {
      if (value == null || DBNull.Value == value)
        return true;
      INullable nullable = value as INullable;
      if (nullable != null)
        return nullable.IsNull;
      return false;
    }

    internal static int SrcCompare(string strA, string strB)
    {
      return !(strA == strB) ? 1 : 0;
    }

    internal static int DstCompare(string strA, string strB)
    {
      return CultureInfo.CurrentCulture.CompareInfo.Compare(strA, strB, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth);
    }

    internal enum InternalErrorCode
    {
      WrongNumberOfKeys = 1000, // 0x000003E8
      UnknownColumnMapKind = 1001, // 0x000003E9
      NestOverNest = 1002, // 0x000003EA
      ColumnCountMismatch = 1003, // 0x000003EB
      AssertionFailed = 1004, // 0x000003EC
      UnknownVar = 1005, // 0x000003ED
      WrongVarType = 1006, // 0x000003EE
      ExtentWithoutEntity = 1007, // 0x000003EF
      UnnestWithoutInput = 1008, // 0x000003F0
      UnnestMultipleCollections = 1009, // 0x000003F1
      CodeGen_NoSuchProperty = 1011, // 0x000003F3
      JoinOverSingleStreamNest = 1012, // 0x000003F4
      InvalidInternalTree = 1013, // 0x000003F5
      NameValuePairNext = 1014, // 0x000003F6
      InvalidParserState1 = 1015, // 0x000003F7
      InvalidParserState2 = 1016, // 0x000003F8
      SqlGenParametersNotPermitted = 1017, // 0x000003F9
      EntityKeyMissingKeyValue = 1018, // 0x000003FA
      UpdatePipelineResultRequestInvalid = 1019, // 0x000003FB
      InvalidStateEntry = 1020, // 0x000003FC
      InvalidPrimitiveTypeKind = 1021, // 0x000003FD
      UnknownLinqNodeType = 1023, // 0x000003FF
      CollectionWithNoColumns = 1024, // 0x00000400
      UnexpectedLinqLambdaExpressionFormat = 1025, // 0x00000401
      CommandTreeOnStoredProcedureEntityCommand = 1026, // 0x00000402
      BoolExprAssert = 1027, // 0x00000403
      FailedToGeneratePromotionRank = 1029, // 0x00000405
    }
  }
}
