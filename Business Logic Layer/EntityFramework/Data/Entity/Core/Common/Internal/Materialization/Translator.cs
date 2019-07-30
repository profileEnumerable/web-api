// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Internal.Materialization.Translator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Common.QueryCache;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Core.Objects.ELinq;
using System.Data.Entity.Core.Objects.Internal;
using System.Data.Entity.Core.Query.InternalTrees;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Data.Entity.Core.Common.Internal.Materialization
{
  internal class Translator
  {
    public static readonly MethodInfo GenericTranslateColumnMap = typeof (Translator).GetDeclaredMethod("TranslateColumnMap", typeof (ColumnMap), typeof (MetadataWorkspace), typeof (SpanIndex), typeof (MergeOption), typeof (bool), typeof (bool));

    internal virtual ShaperFactory<T> TranslateColumnMap<T>(
      ColumnMap columnMap,
      MetadataWorkspace workspace,
      SpanIndex spanIndex,
      MergeOption mergeOption,
      bool streaming,
      bool valueLayer)
    {
      ShaperFactoryQueryCacheKey<T> key = new ShaperFactoryQueryCacheKey<T>(ColumnMapKeyBuilder.GetColumnMapKey(columnMap, spanIndex), mergeOption, streaming, valueLayer);
      QueryCacheManager queryCacheManager = workspace.GetQueryCacheManager();
      ShaperFactory<T> shaperFactory1;
      if (queryCacheManager.TryCacheLookup<ShaperFactoryQueryCacheKey<T>, ShaperFactory<T>>(key, out shaperFactory1))
        return shaperFactory1;
      Translator.TranslatorVisitor translatorVisitor = new Translator.TranslatorVisitor(workspace, spanIndex, mergeOption, streaming, valueLayer);
      columnMap.Accept<TranslatorResult, TranslatorArg>((ColumnMapVisitorWithResults<TranslatorResult, TranslatorArg>) translatorVisitor, new TranslatorArg(typeof (IEnumerable<>).MakeGenericType(typeof (T))));
      CoordinatorFactory<T> rootCoordinatorFactory = (CoordinatorFactory<T>) translatorVisitor.RootCoordinatorScratchpad.Compile();
      Type[] columnTypes = (Type[]) null;
      bool[] nullableColumns = (bool[]) null;
      if (!streaming)
      {
        int num = Math.Max(translatorVisitor.ColumnTypes.Any<KeyValuePair<int, Type>>() ? translatorVisitor.ColumnTypes.Keys.Max() : 0, translatorVisitor.NullableColumns.Any<int>() ? translatorVisitor.NullableColumns.Max() : 0);
        columnTypes = new Type[num + 1];
        foreach (KeyValuePair<int, Type> columnType in translatorVisitor.ColumnTypes)
          columnTypes[columnType.Key] = columnType.Value;
        nullableColumns = new bool[num + 1];
        foreach (int nullableColumn in translatorVisitor.NullableColumns)
          nullableColumns[nullableColumn] = true;
      }
      ShaperFactory<T> shaperFactory2 = new ShaperFactory<T>(translatorVisitor.StateSlotCount, rootCoordinatorFactory, columnTypes, nullableColumns, mergeOption);
      QueryCacheEntry outQueryCacheEntry = new QueryCacheEntry((QueryCacheKey) key, (object) shaperFactory2);
      if (queryCacheManager.TryLookupAndAdd(outQueryCacheEntry, out outQueryCacheEntry))
        shaperFactory2 = (ShaperFactory<T>) outQueryCacheEntry.GetTarget();
      return shaperFactory2;
    }

    internal static ShaperFactory TranslateColumnMap(
      Translator translator,
      Type elementType,
      ColumnMap columnMap,
      MetadataWorkspace workspace,
      SpanIndex spanIndex,
      MergeOption mergeOption,
      bool streaming,
      bool valueLayer)
    {
      return (ShaperFactory) Translator.GenericTranslateColumnMap.MakeGenericMethod(elementType).Invoke((object) translator, new object[6]
      {
        (object) columnMap,
        (object) workspace,
        (object) spanIndex,
        (object) mergeOption,
        (object) streaming,
        (object) valueLayer
      });
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    internal class TranslatorVisitor : ColumnMapVisitorWithResults<TranslatorResult, TranslatorArg>
    {
      public static readonly MethodInfo Translator_MultipleDiscriminatorPolymorphicColumnMapHelper = typeof (Translator.TranslatorVisitor).GetOnlyDeclaredMethod("MultipleDiscriminatorPolymorphicColumnMapHelper");
      public static readonly MethodInfo Translator_TypedCreateInlineDelegate = typeof (Translator.TranslatorVisitor).GetOnlyDeclaredMethod("TypedCreateInlineDelegate");
      private readonly Dictionary<EdmType, ObjectTypeMapping> _objectTypeMappings = new Dictionary<EdmType, ObjectTypeMapping>();
      private readonly MetadataWorkspace _workspace;
      private readonly SpanIndex _spanIndex;
      private readonly MergeOption _mergeOption;
      private readonly bool _streaming;
      private readonly bool IsValueLayer;
      private CoordinatorScratchpad _currentCoordinatorScratchpad;
      private bool _inNullableType;

      public TranslatorVisitor(
        MetadataWorkspace workspace,
        SpanIndex spanIndex,
        MergeOption mergeOption,
        bool streaming,
        bool valueLayer)
      {
        this._workspace = workspace;
        this._spanIndex = spanIndex;
        this._mergeOption = mergeOption;
        this._streaming = streaming;
        this.ColumnTypes = new Dictionary<int, Type>();
        this.NullableColumns = new Set<int>();
        this.IsValueLayer = valueLayer;
      }

      public CoordinatorScratchpad RootCoordinatorScratchpad { get; private set; }

      public int StateSlotCount { get; private set; }

      public Dictionary<int, Type> ColumnTypes { get; private set; }

      public Set<int> NullableColumns { get; private set; }

      private static TranslatorResult AcceptWithMappedType(
        Translator.TranslatorVisitor translatorVisitor,
        ColumnMap columnMap)
      {
        Type clrType = translatorVisitor.DetermineClrType(columnMap.Type);
        return columnMap.Accept<TranslatorResult, TranslatorArg>((ColumnMapVisitorWithResults<TranslatorResult, TranslatorArg>) translatorVisitor, new TranslatorArg(clrType));
      }

      internal override TranslatorResult Visit(
        ComplexTypeColumnMap columnMap,
        TranslatorArg arg)
      {
        Expression expression1 = (Expression) null;
        bool inNullableType = this._inNullableType;
        if (columnMap.NullSentinel != null)
        {
          expression1 = CodeGenEmitter.Emit_Reader_IsDBNull((ColumnMap) columnMap.NullSentinel);
          this._inNullableType = true;
          int columnPos = ((ScalarColumnMap) columnMap.NullSentinel).ColumnPos;
          if (!this._streaming && !this.NullableColumns.Contains(columnPos))
            this.NullableColumns.Add(columnPos);
        }
        Expression expression2;
        if (this.IsValueLayer)
        {
          expression2 = this.BuildExpressionToGetRecordState((StructuredColumnMap) columnMap, (Expression) null, (Expression) null, expression1);
        }
        else
        {
          ComplexType edmType = (ComplexType) columnMap.Type.EdmType;
          expression2 = (Expression) Expression.MemberInit(Expression.New(DelegateFactory.GetConstructorForType(this.DetermineClrType((EdmType) edmType))), (IEnumerable<MemberBinding>) this.CreatePropertyBindings((StructuredColumnMap) columnMap, edmType.Properties));
          if (expression1 != null)
            expression2 = (Expression) Expression.Condition(expression1, CodeGenEmitter.Emit_NullConstant(expression2.Type), expression2);
        }
        this._inNullableType = inNullableType;
        return new TranslatorResult(expression2, arg.RequestedType);
      }

      internal override TranslatorResult Visit(
        EntityColumnMap columnMap,
        TranslatorArg arg)
      {
        EntityIdentity entityIdentity = columnMap.EntityIdentity;
        Expression entitySetReader = (Expression) null;
        Expression entityKeyReader = this.Emit_EntityKey_ctor(this, entityIdentity, columnMap.Type.EdmType, false, out entitySetReader);
        Expression returnedExpression;
        if (this.IsValueLayer)
        {
          Expression nullCheckExpression = (Expression) Expression.Not(CodeGenEmitter.Emit_EntityKey_HasValue(entityIdentity.Keys));
          returnedExpression = this.BuildExpressionToGetRecordState((StructuredColumnMap) columnMap, entityKeyReader, entitySetReader, nullCheckExpression);
        }
        else
        {
          EntityType edmType = (EntityType) columnMap.Type.EdmType;
          ClrEntityType clrType1 = (ClrEntityType) this.LookupObjectMapping((EdmType) edmType).ClrType;
          Type clrType2 = clrType1.ClrType;
          List<MemberBinding> propertyBindings = this.CreatePropertyBindings((StructuredColumnMap) columnMap, edmType.Properties);
          EntityProxyTypeInfo proxyType = EntityProxyFactory.GetProxyType(clrType1, this._workspace);
          Expression ifFalse = this.Emit_ConstructEntity((EntityType) clrType1, (IEnumerable<MemberBinding>) propertyBindings, entityKeyReader, entitySetReader, arg, (EntityProxyTypeInfo) null);
          Expression body;
          if (proxyType == null)
          {
            body = ifFalse;
          }
          else
          {
            Expression ifTrue = this.Emit_ConstructEntity((EntityType) clrType1, (IEnumerable<MemberBinding>) propertyBindings, entityKeyReader, entitySetReader, arg, proxyType);
            body = (Expression) Expression.Condition(CodeGenEmitter.Shaper_ProxyCreationEnabled, ifTrue, ifFalse);
          }
          Expression ifTrue1;
          if (MergeOption.NoTracking != this._mergeOption)
          {
            if (typeof (IEntityWithKey).IsAssignableFrom(proxyType == null ? clrType2 : proxyType.ProxyType) && this._mergeOption != MergeOption.AppendOnly)
              ifTrue1 = (Expression) Expression.Call((Expression) CodeGenEmitter.Shaper_Parameter, CodeGenEmitter.Shaper_HandleIEntityWithKey.MakeGenericMethod(clrType2), body, entitySetReader);
            else if (this._mergeOption == MergeOption.AppendOnly)
            {
              LambdaExpression inlineDelegate = this.CreateInlineDelegate(body);
              ifTrue1 = (Expression) Expression.Call((Expression) CodeGenEmitter.Shaper_Parameter, CodeGenEmitter.Shaper_HandleEntityAppendOnly.MakeGenericMethod(clrType2), (Expression) inlineDelegate, entityKeyReader, entitySetReader);
            }
            else
              ifTrue1 = (Expression) Expression.Call((Expression) CodeGenEmitter.Shaper_Parameter, CodeGenEmitter.Shaper_HandleEntity.MakeGenericMethod(clrType2), body, entityKeyReader, entitySetReader);
          }
          else
            ifTrue1 = (Expression) Expression.Call((Expression) CodeGenEmitter.Shaper_Parameter, CodeGenEmitter.Shaper_HandleEntityNoTracking.MakeGenericMethod(clrType2), body);
          returnedExpression = (Expression) Expression.Condition(CodeGenEmitter.Emit_EntityKey_HasValue(entityIdentity.Keys), ifTrue1, CodeGenEmitter.Emit_WrappedNullConstant());
        }
        int columnPos = ((ScalarColumnMap) entityIdentity.Keys[0]).ColumnPos;
        if (!this._streaming && !this.NullableColumns.Contains(columnPos))
          this.NullableColumns.Add(columnPos);
        return new TranslatorResult(returnedExpression, arg.RequestedType);
      }

      private Expression Emit_ConstructEntity(
        EntityType oSpaceType,
        IEnumerable<MemberBinding> propertyBindings,
        Expression entityKeyReader,
        Expression entitySetReader,
        TranslatorArg arg,
        EntityProxyTypeInfo proxyTypeInfo)
      {
        bool isProxy = proxyTypeInfo != null;
        Type clrType = oSpaceType.ClrType;
        Expression input;
        Type actualType;
        if (isProxy)
        {
          input = (Expression) Expression.MemberInit(Expression.New(proxyTypeInfo.ProxyType), propertyBindings);
          actualType = proxyTypeInfo.ProxyType;
        }
        else
        {
          input = (Expression) Expression.MemberInit(Expression.New(DelegateFactory.GetConstructorForType(clrType)), propertyBindings);
          actualType = clrType;
        }
        Expression expression = CodeGenEmitter.Emit_EnsureTypeAndWrap(input, entityKeyReader, entitySetReader, arg.RequestedType, clrType, actualType, this._mergeOption == MergeOption.NoTracking ? MergeOption.NoTracking : MergeOption.AppendOnly, isProxy);
        if (isProxy)
        {
          expression = (Expression) Expression.Call((Expression) Expression.Constant((object) proxyTypeInfo), CodeGenEmitter.EntityProxyTypeInfo_SetEntityWrapper, expression);
          if ((MethodInfo) proxyTypeInfo.InitializeEntityCollections != (MethodInfo) null)
            expression = (Expression) Expression.Call((MethodInfo) proxyTypeInfo.InitializeEntityCollections, expression);
        }
        return expression;
      }

      private List<MemberBinding> CreatePropertyBindings(
        StructuredColumnMap columnMap,
        ReadOnlyMetadataCollection<EdmProperty> properties)
      {
        List<MemberBinding> memberBindingList = new List<MemberBinding>(columnMap.Properties.Length);
        ObjectTypeMapping objectTypeMapping = this.LookupObjectMapping(columnMap.Type.EdmType);
        for (int index = 0; index < columnMap.Properties.Length; ++index)
        {
          PropertyInfo property1 = DelegateFactory.ValidateSetterProperty(objectTypeMapping.GetPropertyMap(properties[index].Name).ClrProperty.PropertyInfo);
          MethodInfo methodInfo = property1.Setter();
          Type propertyType = property1.PropertyType;
          Expression expression = columnMap.Properties[index].Accept<TranslatorResult, TranslatorArg>((ColumnMapVisitorWithResults<TranslatorResult, TranslatorArg>) this, new TranslatorArg(propertyType)).Expression;
          ScalarColumnMap property2 = columnMap.Properties[index] as ScalarColumnMap;
          if (property2 != null)
          {
            string propertyName = methodInfo.Name.Substring(4);
            Expression withErrorHandling = CodeGenEmitter.Emit_Shaper_GetPropertyValueWithErrorHandling(propertyType, property2.ColumnPos, propertyName, methodInfo.DeclaringType.Name, property2.Type);
            this._currentCoordinatorScratchpad.AddExpressionWithErrorHandling(expression, withErrorHandling);
          }
          memberBindingList.Add((MemberBinding) Expression.Bind((MemberInfo) property1, expression));
        }
        return memberBindingList;
      }

      internal override TranslatorResult Visit(
        SimplePolymorphicColumnMap columnMap,
        TranslatorArg arg)
      {
        Expression expression1 = Translator.TranslatorVisitor.AcceptWithMappedType(this, (ColumnMap) columnMap.TypeDiscriminator).Expression;
        Expression expression2 = !this.IsValueLayer ? CodeGenEmitter.Emit_WrappedNullConstant() : CodeGenEmitter.Emit_EnsureType(this.BuildExpressionToGetRecordState((StructuredColumnMap) columnMap, (Expression) null, (Expression) null, (Expression) Expression.Constant((object) true)), arg.RequestedType);
        foreach (KeyValuePair<object, TypedColumnMap> typeChoice in columnMap.TypeChoices)
        {
          if (!this.DetermineClrType(typeChoice.Value.Type).IsAbstract())
          {
            Expression left = (Expression) Expression.Constant(typeChoice.Key, expression1.Type);
            Expression test = !(expression1.Type == typeof (string)) ? CodeGenEmitter.Emit_Equal(left, expression1) : (Expression) Expression.Call((Expression) Expression.Constant((object) TrailingSpaceStringComparer.Instance), CodeGenEmitter.IEqualityComparerOfString_Equals, left, expression1);
            bool inNullableType = this._inNullableType;
            this._inNullableType = true;
            expression2 = (Expression) Expression.Condition(test, typeChoice.Value.Accept<TranslatorResult, TranslatorArg>((ColumnMapVisitorWithResults<TranslatorResult, TranslatorArg>) this, arg).Expression, expression2);
            this._inNullableType = inNullableType;
          }
        }
        return new TranslatorResult(expression2, arg.RequestedType);
      }

      internal override TranslatorResult Visit(
        MultipleDiscriminatorPolymorphicColumnMap columnMap,
        TranslatorArg arg)
      {
        return new TranslatorResult((Expression) Translator.TranslatorVisitor.Translator_MultipleDiscriminatorPolymorphicColumnMapHelper.MakeGenericMethod(arg.RequestedType).Invoke((object) this, new object[1]
        {
          (object) columnMap
        }), arg.RequestedType);
      }

      [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called via reflection by the Visit method")]
      private Expression MultipleDiscriminatorPolymorphicColumnMapHelper<TElement>(
        MultipleDiscriminatorPolymorphicColumnMap columnMap)
      {
        Expression[] expressionArray = new Expression[columnMap.TypeDiscriminators.Length];
        for (int index = 0; index < expressionArray.Length; ++index)
          expressionArray[index] = columnMap.TypeDiscriminators[index].Accept<TranslatorResult, TranslatorArg>((ColumnMapVisitorWithResults<TranslatorResult, TranslatorArg>) this, new TranslatorArg(typeof (object))).Expression;
        Expression expression1 = (Expression) Expression.NewArrayInit(typeof (object), expressionArray);
        List<Expression> expressionList = new List<Expression>();
        Type type = typeof (KeyValuePair<EntityType, Func<Shaper, TElement>>);
        ConstructorInfo declaredConstructor = type.GetDeclaredConstructor(typeof (EntityType), typeof (Func<Shaper, TElement>));
        foreach (KeyValuePair<EntityType, TypedColumnMap> typeChoice in columnMap.TypeChoices)
        {
          LambdaExpression inlineDelegate = this.CreateInlineDelegate(CodeGenEmitter.Emit_EnsureType(Translator.TranslatorVisitor.AcceptWithMappedType(this, (ColumnMap) typeChoice.Value).UnwrappedExpression, typeof (TElement)));
          Expression expression2 = (Expression) Expression.New(declaredConstructor, (Expression) Expression.Constant((object) typeChoice.Key), (Expression) inlineDelegate);
          expressionList.Add(expression2);
        }
        MethodInfo method = CodeGenEmitter.Shaper_Discriminate.MakeGenericMethod(typeof (TElement));
        return (Expression) Expression.Call((Expression) CodeGenEmitter.Shaper_Parameter, method, expression1, (Expression) Expression.Constant((object) columnMap.Discriminate), (Expression) Expression.NewArrayInit(type, (IEnumerable<Expression>) expressionList));
      }

      internal override TranslatorResult Visit(
        RecordColumnMap columnMap,
        TranslatorArg arg)
      {
        Expression expression1 = (Expression) null;
        bool inNullableType = this._inNullableType;
        if (columnMap.NullSentinel != null)
        {
          expression1 = CodeGenEmitter.Emit_Reader_IsDBNull((ColumnMap) columnMap.NullSentinel);
          this._inNullableType = true;
          int columnPos = ((ScalarColumnMap) columnMap.NullSentinel).ColumnPos;
          if (!this._streaming && !this.NullableColumns.Contains(columnPos))
            this.NullableColumns.Add(columnPos);
        }
        Expression expression2;
        if (this.IsValueLayer)
        {
          expression2 = this.BuildExpressionToGetRecordState((StructuredColumnMap) columnMap, (Expression) null, (Expression) null, expression1);
        }
        else
        {
          InitializerMetadata initializerMetadata;
          Expression ifTrue;
          if (InitializerMetadata.TryGetInitializerMetadata(columnMap.Type, out initializerMetadata))
          {
            expression2 = this.HandleLinqRecord(columnMap, initializerMetadata);
            ifTrue = CodeGenEmitter.Emit_NullConstant(expression2.Type);
          }
          else
          {
            RowType edmType = (RowType) columnMap.Type.EdmType;
            if (this._spanIndex != null && this._spanIndex.HasSpanMap(edmType))
            {
              expression2 = this.HandleSpandexRecord(columnMap, arg, edmType);
              ifTrue = CodeGenEmitter.Emit_WrappedNullConstant();
            }
            else
            {
              expression2 = this.HandleRegularRecord(columnMap, arg, edmType);
              ifTrue = CodeGenEmitter.Emit_NullConstant(expression2.Type);
            }
          }
          if (expression1 != null)
            expression2 = (Expression) Expression.Condition(expression1, ifTrue, expression2);
        }
        this._inNullableType = inNullableType;
        return new TranslatorResult(expression2, arg.RequestedType);
      }

      private Expression BuildExpressionToGetRecordState(
        StructuredColumnMap columnMap,
        Expression entityKeyReader,
        Expression entitySetReader,
        Expression nullCheckExpression)
      {
        RecordStateScratchpad recordStateScratchpad = this._currentCoordinatorScratchpad.CreateRecordStateScratchpad();
        int stateSlotNumber = this.AllocateStateSlot();
        recordStateScratchpad.StateSlotNumber = stateSlotNumber;
        int length1 = columnMap.Properties.Length;
        int length2 = entityKeyReader != null ? length1 + 1 : length1;
        recordStateScratchpad.ColumnCount = length1;
        EntityType type = (EntityType) null;
        if (TypeHelpers.TryGetEdmType<EntityType>(columnMap.Type, out type))
        {
          recordStateScratchpad.DataRecordInfo = (DataRecordInfo) new EntityRecordInfo(type, EntityKey.EntityNotValidKey, (EntitySet) null);
        }
        else
        {
          TypeUsage modelTypeUsage = Helper.GetModelTypeUsage(columnMap.Type);
          recordStateScratchpad.DataRecordInfo = new DataRecordInfo(modelTypeUsage);
        }
        Expression[] expressionArray = new Expression[length2];
        string[] strArray = new string[recordStateScratchpad.ColumnCount];
        TypeUsage[] typeUsageArray = new TypeUsage[recordStateScratchpad.ColumnCount];
        for (int index = 0; index < length1; ++index)
        {
          Expression expression = columnMap.Properties[index].Accept<TranslatorResult, TranslatorArg>((ColumnMapVisitorWithResults<TranslatorResult, TranslatorArg>) this, new TranslatorArg(typeof (object))).Expression;
          expressionArray[index] = (Expression) Expression.Call((Expression) CodeGenEmitter.Shaper_Parameter, CodeGenEmitter.Shaper_SetColumnValue, (Expression) Expression.Constant((object) stateSlotNumber), (Expression) Expression.Constant((object) index), (Expression) Expression.Coalesce(expression, CodeGenEmitter.DBNull_Value));
          strArray[index] = columnMap.Properties[index].Name;
          typeUsageArray[index] = columnMap.Properties[index].Type;
        }
        if (entityKeyReader != null)
          expressionArray[length2 - 1] = (Expression) Expression.Call((Expression) CodeGenEmitter.Shaper_Parameter, CodeGenEmitter.Shaper_SetEntityRecordInfo, (Expression) Expression.Constant((object) stateSlotNumber), entityKeyReader, entitySetReader);
        recordStateScratchpad.GatherData = CodeGenEmitter.Emit_BitwiseOr((IEnumerable<Expression>) expressionArray);
        recordStateScratchpad.PropertyNames = strArray;
        recordStateScratchpad.TypeUsages = typeUsageArray;
        Expression ifFalse = (Expression) Expression.Call(CodeGenEmitter.Emit_Shaper_GetState(stateSlotNumber, typeof (RecordState)), CodeGenEmitter.RecordState_GatherData, (Expression) CodeGenEmitter.Shaper_Parameter);
        if (nullCheckExpression != null)
        {
          Expression ifTrue = (Expression) Expression.Call(CodeGenEmitter.Emit_Shaper_GetState(stateSlotNumber, typeof (RecordState)), CodeGenEmitter.RecordState_SetNullRecord);
          ifFalse = (Expression) Expression.Condition(nullCheckExpression, ifTrue, ifFalse);
        }
        return ifFalse;
      }

      private Expression HandleLinqRecord(
        RecordColumnMap columnMap,
        InitializerMetadata initializerMetadata)
      {
        List<TranslatorResult> propertyTranslatorResults = new List<TranslatorResult>(columnMap.Properties.Length);
        foreach (KeyValuePair<ColumnMap, Type> keyValuePair in ((IEnumerable<ColumnMap>) columnMap.Properties).Zip<ColumnMap, Type>(initializerMetadata.GetChildTypes()))
        {
          ColumnMap key = keyValuePair.Key;
          Type clrType = keyValuePair.Value;
          if ((Type) null == clrType)
            clrType = this.DetermineClrType(key.Type);
          TranslatorResult translatorResult = key.Accept<TranslatorResult, TranslatorArg>((ColumnMapVisitorWithResults<TranslatorResult, TranslatorArg>) this, new TranslatorArg(clrType));
          propertyTranslatorResults.Add(translatorResult);
        }
        return initializerMetadata.Emit(propertyTranslatorResults);
      }

      private Expression HandleRegularRecord(
        RecordColumnMap columnMap,
        TranslatorArg arg,
        RowType spanRowType)
      {
        Expression[] expressionArray = new Expression[columnMap.Properties.Length];
        for (int index = 0; index < expressionArray.Length; ++index)
        {
          Expression unwrappedExpression = Translator.TranslatorVisitor.AcceptWithMappedType(this, columnMap.Properties[index]).UnwrappedExpression;
          expressionArray[index] = (Expression) Expression.Coalesce(CodeGenEmitter.Emit_EnsureType(unwrappedExpression, typeof (object)), CodeGenEmitter.DBNull_Value);
        }
        Expression expression1 = (Expression) Expression.NewArrayInit(typeof (object), expressionArray);
        TypeUsage typeUsage = columnMap.Type;
        if (this._spanIndex != null)
          typeUsage = this._spanIndex.GetSpannedRowType(spanRowType) ?? typeUsage;
        Expression expression2 = (Expression) Expression.Constant((object) typeUsage, typeof (TypeUsage));
        return CodeGenEmitter.Emit_EnsureType((Expression) Expression.New(CodeGenEmitter.MaterializedDataRecord_ctor, CodeGenEmitter.Shaper_Workspace, expression2, expression1), arg.RequestedType);
      }

      private Expression HandleSpandexRecord(
        RecordColumnMap columnMap,
        TranslatorArg arg,
        RowType spanRowType)
      {
        Dictionary<int, AssociationEndMember> spanMap = this._spanIndex.GetSpanMap(spanRowType);
        Expression expression1 = columnMap.Properties[0].Accept<TranslatorResult, TranslatorArg>((ColumnMapVisitorWithResults<TranslatorResult, TranslatorArg>) this, arg).Expression;
        for (int index = 1; index < columnMap.Properties.Length; ++index)
        {
          AssociationEndMember associationEndMember = spanMap[index];
          TranslatorResult translatorResult1 = Translator.TranslatorVisitor.AcceptWithMappedType(this, columnMap.Properties[index]);
          Expression expression2 = translatorResult1.Expression;
          CollectionTranslatorResult translatorResult2 = translatorResult1 as CollectionTranslatorResult;
          if (translatorResult2 != null)
          {
            Expression toGetCoordinator = translatorResult2.ExpressionToGetCoordinator;
            Type genericArgument = expression2.Type.GetGenericArguments()[0];
            MethodInfo method = CodeGenEmitter.Shaper_HandleFullSpanCollection.MakeGenericMethod(genericArgument);
            expression1 = (Expression) Expression.Call((Expression) CodeGenEmitter.Shaper_Parameter, method, expression1, toGetCoordinator, (Expression) Expression.Constant((object) associationEndMember));
          }
          else if (typeof (EntityKey) == expression2.Type)
          {
            MethodInfo relationshipSpan = CodeGenEmitter.Shaper_HandleRelationshipSpan;
            expression1 = (Expression) Expression.Call((Expression) CodeGenEmitter.Shaper_Parameter, relationshipSpan, expression1, expression2, (Expression) Expression.Constant((object) associationEndMember));
          }
          else
          {
            MethodInfo handleFullSpanElement = CodeGenEmitter.Shaper_HandleFullSpanElement;
            expression1 = (Expression) Expression.Call((Expression) CodeGenEmitter.Shaper_Parameter, handleFullSpanElement, expression1, expression2, (Expression) Expression.Constant((object) associationEndMember));
          }
        }
        return expression1;
      }

      internal override TranslatorResult Visit(
        SimpleCollectionColumnMap columnMap,
        TranslatorArg arg)
      {
        return this.ProcessCollectionColumnMap((CollectionColumnMap) columnMap, arg);
      }

      internal override TranslatorResult Visit(
        DiscriminatedCollectionColumnMap columnMap,
        TranslatorArg arg)
      {
        return this.ProcessCollectionColumnMap((CollectionColumnMap) columnMap, arg, (ColumnMap) columnMap.Discriminator, columnMap.DiscriminatorValue);
      }

      private TranslatorResult ProcessCollectionColumnMap(
        CollectionColumnMap columnMap,
        TranslatorArg arg)
      {
        return this.ProcessCollectionColumnMap(columnMap, arg, (ColumnMap) null, (object) null);
      }

      private TranslatorResult ProcessCollectionColumnMap(
        CollectionColumnMap columnMap,
        TranslatorArg arg,
        ColumnMap discriminatorColumnMap,
        object discriminatorValue)
      {
        Type elementType1 = this.DetermineElementType(arg.RequestedType, columnMap);
        CoordinatorScratchpad coordinatorScratchpad = new CoordinatorScratchpad(elementType1);
        this.EnterCoordinatorTranslateScope(coordinatorScratchpad);
        ColumnMap columnMap1 = columnMap.Element;
        if (this.IsValueLayer && !(columnMap1 is StructuredColumnMap))
        {
          ColumnMap[] properties = new ColumnMap[1]
          {
            columnMap.Element
          };
          columnMap1 = (ColumnMap) new RecordColumnMap(columnMap.Element.Type, columnMap.Element.Name, properties, (SimpleColumnMap) null);
        }
        bool inNullableType = this._inNullableType;
        if (discriminatorColumnMap != null)
          this._inNullableType = true;
        Expression unconvertedExpression = columnMap1.Accept<TranslatorResult, TranslatorArg>((ColumnMapVisitorWithResults<TranslatorResult, TranslatorArg>) this, new TranslatorArg(elementType1)).UnconvertedExpression;
        Expression[] keyReaders;
        if (columnMap.Keys != null)
        {
          keyReaders = new Expression[columnMap.Keys.Length];
          for (int index = 0; index < keyReaders.Length; ++index)
          {
            Expression expression = Translator.TranslatorVisitor.AcceptWithMappedType(this, (ColumnMap) columnMap.Keys[index]).Expression;
            keyReaders[index] = expression;
          }
        }
        else
          keyReaders = new Expression[0];
        Expression discriminator = (Expression) null;
        if (discriminatorColumnMap != null)
        {
          discriminator = Translator.TranslatorVisitor.AcceptWithMappedType(this, discriminatorColumnMap).Expression;
          this._inNullableType = inNullableType;
        }
        Expression getCoordinator = this.BuildExpressionToGetCoordinator(elementType1, unconvertedExpression, keyReaders, discriminator, discriminatorValue, coordinatorScratchpad);
        MethodInfo genericElementsMethod = Translator.TranslatorVisitor.GetGenericElementsMethod(elementType1);
        Expression expression1;
        if (this.IsValueLayer)
        {
          expression1 = getCoordinator;
        }
        else
        {
          expression1 = (Expression) Expression.Call(getCoordinator, genericElementsMethod);
          coordinatorScratchpad.Element = CodeGenEmitter.Emit_EnsureType(coordinatorScratchpad.Element, elementType1);
          Type elementType2 = arg.RequestedType.TryGetElementType(typeof (ICollection<>));
          if (elementType2 != (Type) null)
          {
            Type collectionType = EntityUtil.DetermineCollectionType(arg.RequestedType);
            if (collectionType == (Type) null)
              throw new InvalidOperationException(Strings.ObjectQuery_UnableToMaterializeArbitaryProjectionType((object) arg.RequestedType));
            Type type = typeof (List<>).MakeGenericType(elementType2);
            if (collectionType != type)
              coordinatorScratchpad.InitializeCollection = CodeGenEmitter.Emit_EnsureType((Expression) DelegateFactory.GetNewExpressionForCollectionType(collectionType), typeof (ICollection<>).MakeGenericType(elementType2));
            expression1 = CodeGenEmitter.Emit_EnsureType(expression1, arg.RequestedType);
          }
          else if (!arg.RequestedType.IsAssignableFrom(expression1.Type))
          {
            Type type = typeof (CompensatingCollection<>).MakeGenericType(elementType1);
            expression1 = CodeGenEmitter.Emit_EnsureType((Expression) Expression.New(type.GetConstructors()[0], expression1), type);
          }
        }
        this.ExitCoordinatorTranslateScope();
        return (TranslatorResult) new CollectionTranslatorResult(expression1, arg.RequestedType, getCoordinator);
      }

      public static MethodInfo GetGenericElementsMethod(Type elementType)
      {
        return typeof (Coordinator<>).MakeGenericType(elementType).GetOnlyDeclaredMethod("GetElements");
      }

      private Type DetermineElementType(Type collectionType, CollectionColumnMap columnMap)
      {
        Type type;
        if (this.IsValueLayer)
        {
          type = typeof (RecordState);
        }
        else
        {
          type = TypeSystem.GetElementType(collectionType);
          if (type == collectionType)
            type = this.DetermineClrType(((CollectionType) columnMap.Type.EdmType).TypeUsage);
        }
        return type;
      }

      private void EnterCoordinatorTranslateScope(CoordinatorScratchpad coordinatorScratchpad)
      {
        if (this.RootCoordinatorScratchpad == null)
        {
          coordinatorScratchpad.Depth = 0;
          this.RootCoordinatorScratchpad = coordinatorScratchpad;
          this._currentCoordinatorScratchpad = coordinatorScratchpad;
        }
        else
        {
          coordinatorScratchpad.Depth = this._currentCoordinatorScratchpad.Depth + 1;
          this._currentCoordinatorScratchpad.AddNestedCoordinator(coordinatorScratchpad);
          this._currentCoordinatorScratchpad = coordinatorScratchpad;
        }
      }

      private void ExitCoordinatorTranslateScope()
      {
        this._currentCoordinatorScratchpad = this._currentCoordinatorScratchpad.Parent;
      }

      private Expression BuildExpressionToGetCoordinator(
        Type elementType,
        Expression element,
        Expression[] keyReaders,
        Expression discriminator,
        object discriminatorValue,
        CoordinatorScratchpad coordinatorScratchpad)
      {
        int stateSlotNumber1 = this.AllocateStateSlot();
        coordinatorScratchpad.StateSlotNumber = stateSlotNumber1;
        coordinatorScratchpad.Element = element;
        List<Expression> expressionList1 = new List<Expression>(keyReaders.Length);
        List<Expression> expressionList2 = new List<Expression>(keyReaders.Length);
        foreach (Expression keyReader in keyReaders)
        {
          int stateSlotNumber2 = this.AllocateStateSlot();
          expressionList1.Add(CodeGenEmitter.Emit_Shaper_SetState(stateSlotNumber2, keyReader));
          expressionList2.Add(CodeGenEmitter.Emit_Equal(CodeGenEmitter.Emit_Shaper_GetState(stateSlotNumber2, keyReader.Type), keyReader));
        }
        coordinatorScratchpad.SetKeys = CodeGenEmitter.Emit_BitwiseOr((IEnumerable<Expression>) expressionList1);
        coordinatorScratchpad.CheckKeys = CodeGenEmitter.Emit_AndAlso((IEnumerable<Expression>) expressionList2);
        if (discriminator != null)
          coordinatorScratchpad.HasData = CodeGenEmitter.Emit_Equal((Expression) Expression.Constant(discriminatorValue, discriminator.Type), discriminator);
        return CodeGenEmitter.Emit_Shaper_GetState(stateSlotNumber1, typeof (Coordinator<>).MakeGenericType(elementType));
      }

      internal override TranslatorResult Visit(
        RefColumnMap columnMap,
        TranslatorArg arg)
      {
        EntityIdentity entityIdentity = columnMap.EntityIdentity;
        Expression entitySetReader;
        Expression returnedExpression = (Expression) Expression.Condition(CodeGenEmitter.Emit_EntityKey_HasValue(entityIdentity.Keys), this.Emit_EntityKey_ctor(this, entityIdentity, (EdmType) ((RefType) columnMap.Type.EdmType).ElementType, true, out entitySetReader), (Expression) Expression.Constant((object) null, typeof (EntityKey)));
        int columnPos = ((ScalarColumnMap) entityIdentity.Keys[0]).ColumnPos;
        if (!this._streaming && !this.NullableColumns.Contains(columnPos))
          this.NullableColumns.Add(columnPos);
        return new TranslatorResult(returnedExpression, arg.RequestedType);
      }

      [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
      internal override TranslatorResult Visit(
        ScalarColumnMap columnMap,
        TranslatorArg arg)
      {
        Type requestedType = arg.RequestedType;
        TypeUsage type1 = columnMap.Type;
        int columnPos = columnMap.ColumnPos;
        Type type2 = (Type) null;
        PrimitiveTypeKind spatialType;
        Expression expression1;
        if (Helper.IsSpatialType(type1, out spatialType))
        {
          expression1 = CodeGenEmitter.Emit_Conditional_NotDBNull(Helper.IsGeographicType((PrimitiveType) type1.EdmType) ? CodeGenEmitter.Emit_EnsureType(CodeGenEmitter.Emit_Shaper_GetGeographyColumnValue(columnPos), requestedType) : CodeGenEmitter.Emit_EnsureType(CodeGenEmitter.Emit_Shaper_GetGeometryColumnValue(columnPos), requestedType), columnPos, requestedType);
          if (!this._streaming && !this.NullableColumns.Contains(columnPos))
            this.NullableColumns.Add(columnPos);
        }
        else
        {
          bool isNullable;
          MethodInfo readerMethod = CodeGenEmitter.GetReaderMethod(requestedType, out isNullable);
          Expression expression2 = (Expression) Expression.Call(CodeGenEmitter.Shaper_Reader, readerMethod, (Expression) Expression.Constant((object) columnPos));
          type2 = TypeSystem.GetNonNullableType(requestedType);
          if (type2.IsEnum() && type2 != requestedType)
            expression2 = (Expression) Expression.Convert(expression2, type2);
          else if (requestedType == typeof (object) && !this.IsValueLayer && TypeSemantics.IsEnumerationType(type1))
          {
            expression2 = (Expression) Expression.Condition(CodeGenEmitter.Emit_Reader_IsDBNull(columnPos), expression2, (Expression) Expression.Convert((Expression) Expression.Convert(expression2, TypeSystem.GetNonNullableType(this.DetermineClrType(type1.EdmType))), typeof (object)));
            if (!this._streaming && !this.NullableColumns.Contains(columnPos))
              this.NullableColumns.Add(columnPos);
          }
          expression1 = CodeGenEmitter.Emit_EnsureType(expression2, requestedType);
          if (isNullable)
          {
            expression1 = CodeGenEmitter.Emit_Conditional_NotDBNull(expression1, columnPos, requestedType);
            if (!this._streaming && !this.NullableColumns.Contains(columnPos))
              this.NullableColumns.Add(columnPos);
          }
        }
        if (!this._streaming)
        {
          Type type3 = type2;
          if ((object) type3 == null)
            type3 = requestedType;
          Type type4 = type3;
          Type type5 = type4.IsEnum() ? type4.GetEnumUnderlyingType() : type4;
          Type type6;
          if (this.ColumnTypes.TryGetValue(columnPos, out type6))
          {
            if (type6 == typeof (object) && type5 != typeof (object))
              this.ColumnTypes[columnPos] = type5;
          }
          else
          {
            this.ColumnTypes.Add(columnPos, type5);
            if (this._inNullableType && !this.NullableColumns.Contains(columnPos))
              this.NullableColumns.Add(columnPos);
          }
        }
        Expression withErrorHandling = CodeGenEmitter.Emit_Shaper_GetColumnValueWithErrorHandling(arg.RequestedType, columnPos, type1);
        this._currentCoordinatorScratchpad.AddExpressionWithErrorHandling(expression1, withErrorHandling);
        return new TranslatorResult(expression1, requestedType);
      }

      internal override TranslatorResult Visit(
        VarRefColumnMap columnMap,
        TranslatorArg arg)
      {
        throw new InvalidOperationException(string.Empty);
      }

      private int AllocateStateSlot()
      {
        return this.StateSlotCount++;
      }

      private Type DetermineClrType(TypeUsage typeUsage)
      {
        return this.DetermineClrType(typeUsage.EdmType);
      }

      private Type DetermineClrType(EdmType edmType)
      {
        Type type = (Type) null;
        edmType = this.ResolveSpanType(edmType);
        switch (edmType.BuiltInTypeKind)
        {
          case BuiltInTypeKind.CollectionType:
            if (this.IsValueLayer)
            {
              type = typeof (Coordinator<RecordState>);
              break;
            }
            type = typeof (IEnumerable<>).MakeGenericType(this.DetermineClrType(((CollectionType) edmType).TypeUsage.EdmType));
            break;
          case BuiltInTypeKind.ComplexType:
          case BuiltInTypeKind.EntityType:
            type = !this.IsValueLayer ? this.LookupObjectMapping(edmType).ClrType.ClrType : typeof (RecordState);
            break;
          case BuiltInTypeKind.EnumType:
            if (this.IsValueLayer)
            {
              type = this.DetermineClrType((EdmType) ((EnumType) edmType).UnderlyingType);
              break;
            }
            type = typeof (Nullable<>).MakeGenericType(this.LookupObjectMapping(edmType).ClrType.ClrType);
            break;
          case BuiltInTypeKind.PrimitiveType:
            type = ((PrimitiveType) edmType).ClrEquivalentType;
            if (type.IsValueType())
            {
              type = typeof (Nullable<>).MakeGenericType(type);
              break;
            }
            break;
          case BuiltInTypeKind.RefType:
            type = typeof (EntityKey);
            break;
          case BuiltInTypeKind.RowType:
            if (this.IsValueLayer)
            {
              type = typeof (RecordState);
              break;
            }
            InitializerMetadata initializerMetadata = ((RowType) edmType).InitializerMetadata;
            type = initializerMetadata == null ? typeof (DbDataRecord) : initializerMetadata.ClrType;
            break;
        }
        return type;
      }

      private static ConstructorInfo GetConstructor(Type type)
      {
        if (!type.IsAbstract())
          return DelegateFactory.GetConstructorForType(type);
        return (ConstructorInfo) null;
      }

      private ObjectTypeMapping LookupObjectMapping(EdmType edmType)
      {
        EdmType edmType1 = this.ResolveSpanType(edmType) ?? edmType;
        ObjectTypeMapping objectMapping;
        if (!this._objectTypeMappings.TryGetValue(edmType1, out objectMapping))
        {
          objectMapping = Util.GetObjectMapping(edmType1, this._workspace);
          this._objectTypeMappings.Add(edmType1, objectMapping);
        }
        return objectMapping;
      }

      private EdmType ResolveSpanType(EdmType edmType)
      {
        EdmType elementType = edmType;
        switch (elementType.BuiltInTypeKind)
        {
          case BuiltInTypeKind.CollectionType:
            elementType = this.ResolveSpanType(((CollectionType) elementType).TypeUsage.EdmType);
            if (elementType != null)
            {
              elementType = (EdmType) new CollectionType(elementType);
              break;
            }
            break;
          case BuiltInTypeKind.RowType:
            RowType spanRowType = (RowType) elementType;
            if (this._spanIndex != null && this._spanIndex.HasSpanMap(spanRowType))
            {
              elementType = spanRowType.Members[0].TypeUsage.EdmType;
              break;
            }
            break;
        }
        return elementType;
      }

      private LambdaExpression CreateInlineDelegate(Expression body)
      {
        Type type = body.Type;
        return (LambdaExpression) Translator.TranslatorVisitor.Translator_TypedCreateInlineDelegate.MakeGenericMethod(type).Invoke((object) this, new object[1]
        {
          (object) body
        });
      }

      [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called via reflection by the non-generic overload")]
      private Expression<Func<Shaper, T>> TypedCreateInlineDelegate<T>(
        Expression body)
      {
        Expression<Func<Shaper, T>> expression = Expression.Lambda<Func<Shaper, T>>(body, CodeGenEmitter.Shaper_Parameter);
        this._currentCoordinatorScratchpad.AddInlineDelegate((LambdaExpression) expression);
        return expression;
      }

      private Expression Emit_EntityKey_ctor(
        Translator.TranslatorVisitor translatorVisitor,
        EntityIdentity entityIdentity,
        EdmType type,
        bool isForColumnValue,
        out Expression entitySetReader)
      {
        Expression left = (Expression) null;
        List<Expression> expressionList = new List<Expression>(entityIdentity.Keys.Length);
        if (this.IsValueLayer)
        {
          for (int index = 0; index < entityIdentity.Keys.Length; ++index)
          {
            Expression expression = entityIdentity.Keys[index].Accept<TranslatorResult, TranslatorArg>((ColumnMapVisitorWithResults<TranslatorResult, TranslatorArg>) translatorVisitor, new TranslatorArg(typeof (object))).Expression;
            expressionList.Add(expression);
          }
        }
        else
        {
          ObjectTypeMapping objectTypeMapping = this.LookupObjectMapping(type);
          for (int index = 0; index < entityIdentity.Keys.Length; ++index)
          {
            Type propertyType = DelegateFactory.ValidateSetterProperty(objectTypeMapping.GetPropertyMap(entityIdentity.Keys[index].Name).ClrProperty.PropertyInfo).PropertyType;
            Expression expression = entityIdentity.Keys[index].Accept<TranslatorResult, TranslatorArg>((ColumnMapVisitorWithResults<TranslatorResult, TranslatorArg>) translatorVisitor, new TranslatorArg(propertyType)).Expression;
            expressionList.Add(CodeGenEmitter.Emit_EnsureType(expression, typeof (object)));
          }
        }
        SimpleEntityIdentity simpleEntityIdentity = entityIdentity as SimpleEntityIdentity;
        if (simpleEntityIdentity != null)
        {
          if (simpleEntityIdentity.EntitySet == null)
          {
            entitySetReader = (Expression) Expression.Constant((object) null, typeof (EntitySet));
            return (Expression) Expression.Constant((object) null, typeof (EntityKey));
          }
          entitySetReader = (Expression) Expression.Constant((object) simpleEntityIdentity.EntitySet, typeof (EntitySet));
        }
        else
        {
          DiscriminatedEntityIdentity discriminatedEntityIdentity = (DiscriminatedEntityIdentity) entityIdentity;
          Expression expression = discriminatedEntityIdentity.EntitySetColumnMap.Accept<TranslatorResult, TranslatorArg>((ColumnMapVisitorWithResults<TranslatorResult, TranslatorArg>) translatorVisitor, new TranslatorArg(typeof (int?))).Expression;
          EntitySet[] entitySetMap = discriminatedEntityIdentity.EntitySetMap;
          entitySetReader = (Expression) Expression.Constant((object) null, typeof (EntitySet));
          for (int index = 0; index < entitySetMap.Length; ++index)
            entitySetReader = (Expression) Expression.Condition((Expression) Expression.Equal(expression, (Expression) Expression.Constant((object) index, typeof (int?))), (Expression) Expression.Constant((object) entitySetMap[index], typeof (EntitySet)), entitySetReader);
          int stateSlotNumber = translatorVisitor.AllocateStateSlot();
          left = CodeGenEmitter.Emit_Shaper_SetStatePassthrough(stateSlotNumber, entitySetReader);
          entitySetReader = CodeGenEmitter.Emit_Shaper_GetState(stateSlotNumber, typeof (EntitySet));
        }
        Expression ifFalse;
        if (1 == entityIdentity.Keys.Length)
          ifFalse = (Expression) Expression.New(CodeGenEmitter.EntityKey_ctor_SingleKey, entitySetReader, expressionList[0]);
        else
          ifFalse = (Expression) Expression.New(CodeGenEmitter.EntityKey_ctor_CompositeKey, entitySetReader, (Expression) Expression.NewArrayInit(typeof (object), (IEnumerable<Expression>) expressionList));
        if (left != null)
        {
          Expression ifTrue = !translatorVisitor.IsValueLayer || isForColumnValue ? (Expression) Expression.Constant((object) null, typeof (EntityKey)) : (Expression) Expression.Constant((object) EntityKey.NoEntitySetKey, typeof (EntityKey));
          ifFalse = (Expression) Expression.Condition((Expression) Expression.Equal(left, (Expression) Expression.Constant((object) null, typeof (EntitySet))), ifTrue, ifFalse);
        }
        return ifFalse;
      }
    }
  }
}
