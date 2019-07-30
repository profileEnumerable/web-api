// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.ColumnMapFactory
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects.ELinq;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal class ColumnMapFactory
  {
    internal virtual CollectionColumnMap CreateFunctionImportStructuralTypeColumnMap(
      DbDataReader storeDataReader,
      FunctionImportMappingNonComposable mapping,
      int resultSetIndex,
      EntitySet entitySet,
      StructuralType baseStructuralType)
    {
      FunctionImportStructuralTypeMappingKB resultMapping = mapping.GetResultMapping(resultSetIndex);
      if (resultMapping.NormalizedEntityTypeMappings.Count == 0)
        return this.CreateColumnMapFromReaderAndType(storeDataReader, (EdmType) baseStructuralType, entitySet, resultMapping.ReturnTypeColumnsRenameMapping);
      EntityType entityType = baseStructuralType as EntityType;
      ScalarColumnMap[] discriminatorColumnMaps = ColumnMapFactory.CreateDiscriminatorColumnMaps(storeDataReader, mapping, resultSetIndex);
      HashSet<EntityType> entityTypeSet = new HashSet<EntityType>((IEnumerable<EntityType>) resultMapping.MappedEntityTypes);
      entityTypeSet.Add(entityType);
      Dictionary<EntityType, TypedColumnMap> typeChoices = new Dictionary<EntityType, TypedColumnMap>(entityTypeSet.Count);
      ColumnMap[] baseTypeColumns = (ColumnMap[]) null;
      foreach (EntityType key in entityTypeSet)
      {
        ColumnMap[] columnMapsForType = ColumnMapFactory.GetColumnMapsForType(storeDataReader, (EdmType) key, resultMapping.ReturnTypeColumnsRenameMapping);
        EntityColumnMap elementColumnMap = ColumnMapFactory.CreateEntityTypeElementColumnMap(storeDataReader, (EdmType) key, entitySet, columnMapsForType, resultMapping.ReturnTypeColumnsRenameMapping);
        if (!key.Abstract)
          typeChoices.Add(key, (TypedColumnMap) elementColumnMap);
        if (key == baseStructuralType)
          baseTypeColumns = columnMapsForType;
      }
      MultipleDiscriminatorPolymorphicColumnMap polymorphicColumnMap = new MultipleDiscriminatorPolymorphicColumnMap(TypeUsage.Create((EdmType) baseStructuralType), baseStructuralType.Name, baseTypeColumns, (SimpleColumnMap[]) discriminatorColumnMaps, typeChoices, (Func<object[], EntityType>) (discriminatorValues => mapping.Discriminate(discriminatorValues, resultSetIndex)));
      return (CollectionColumnMap) new SimpleCollectionColumnMap(baseStructuralType.GetCollectionType().TypeUsage, baseStructuralType.Name, (ColumnMap) polymorphicColumnMap, (SimpleColumnMap[]) null, (SimpleColumnMap[]) null);
    }

    internal virtual CollectionColumnMap CreateColumnMapFromReaderAndType(
      DbDataReader storeDataReader,
      EdmType edmType,
      EntitySet entitySet,
      Dictionary<string, FunctionImportReturnTypeStructuralTypeColumnRenameMapping> renameList)
    {
      ColumnMap[] columnMapsForType = ColumnMapFactory.GetColumnMapsForType(storeDataReader, edmType, renameList);
      ColumnMap elementMap = (ColumnMap) null;
      if (Helper.IsRowType((GlobalItem) edmType))
        elementMap = (ColumnMap) new RecordColumnMap(TypeUsage.Create(edmType), edmType.Name, columnMapsForType, (SimpleColumnMap) null);
      else if (Helper.IsComplexType(edmType))
        elementMap = (ColumnMap) new ComplexTypeColumnMap(TypeUsage.Create(edmType), edmType.Name, columnMapsForType, (SimpleColumnMap) null);
      else if (Helper.IsScalarType(edmType))
      {
        if (storeDataReader.FieldCount != 1)
          throw new EntityCommandExecutionException(Strings.ADP_InvalidDataReaderFieldCountForScalarType);
        elementMap = (ColumnMap) new ScalarColumnMap(TypeUsage.Create(edmType), edmType.Name, 0, 0);
      }
      else if (Helper.IsEntityType(edmType))
        elementMap = (ColumnMap) ColumnMapFactory.CreateEntityTypeElementColumnMap(storeDataReader, edmType, entitySet, columnMapsForType, (Dictionary<string, FunctionImportReturnTypeStructuralTypeColumnRenameMapping>) null);
      return (CollectionColumnMap) new SimpleCollectionColumnMap(edmType.GetCollectionType().TypeUsage, edmType.Name, elementMap, (SimpleColumnMap[]) null, (SimpleColumnMap[]) null);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    internal virtual CollectionColumnMap CreateColumnMapFromReaderAndClrType(
      DbDataReader reader,
      Type type,
      MetadataWorkspace workspace)
    {
      ConstructorInfo declaredConstructor = type.GetDeclaredConstructor();
      if (type.IsAbstract() || (ConstructorInfo) null == declaredConstructor && !type.IsValueType())
        throw new InvalidOperationException(Strings.ObjectContext_InvalidTypeForStoreQuery((object) type));
      List<Tuple<MemberAssignment, int, EdmProperty>> source1 = new List<Tuple<MemberAssignment, int, EdmProperty>>();
      foreach (PropertyInfo propertyInfo in type.GetInstanceProperties().Select<PropertyInfo, PropertyInfo>((Func<PropertyInfo, PropertyInfo>) (p => p.GetPropertyInfoForSet())))
      {
        Type type1 = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
        if ((object) type1 == null)
          type1 = propertyInfo.PropertyType;
        Type type2 = type1;
        Type type3 = type2.IsEnum() ? type2.GetEnumUnderlyingType() : propertyInfo.PropertyType;
        int ordinal;
        EdmType modelEdmType;
        if (ColumnMapFactory.TryGetColumnOrdinalFromReader(reader, propertyInfo.Name, out ordinal) && workspace.TryDetermineCSpaceModelType(type3, out modelEdmType) && (Helper.IsScalarType(modelEdmType) && propertyInfo.CanWriteExtended()) && (propertyInfo.GetIndexParameters().Length == 0 && (MethodInfo) null != propertyInfo.Setter()))
          source1.Add(Tuple.Create<MemberAssignment, int, EdmProperty>(Expression.Bind((MemberInfo) propertyInfo, (Expression) Expression.Parameter(propertyInfo.PropertyType, "placeholder")), ordinal, new EdmProperty(propertyInfo.Name, TypeUsage.Create(modelEdmType))));
      }
      MemberInfo[] memberInfoArray = new MemberInfo[source1.Count];
      MemberBinding[] memberBindingArray = new MemberBinding[source1.Count];
      ColumnMap[] properties = new ColumnMap[source1.Count];
      EdmProperty[] edmPropertyArray = new EdmProperty[source1.Count];
      int index = 0;
      foreach (IGrouping<int, Tuple<MemberAssignment, int, EdmProperty>> source2 in (IEnumerable<IGrouping<int, Tuple<MemberAssignment, int, EdmProperty>>>) source1.GroupBy<Tuple<MemberAssignment, int, EdmProperty>, int>((Func<Tuple<MemberAssignment, int, EdmProperty>, int>) (tuple => tuple.Item2)).OrderBy<IGrouping<int, Tuple<MemberAssignment, int, EdmProperty>>, int>((Func<IGrouping<int, Tuple<MemberAssignment, int, EdmProperty>>, int>) (tuple => tuple.Key)))
      {
        if (source2.Count<Tuple<MemberAssignment, int, EdmProperty>>() != 1)
          throw new InvalidOperationException(Strings.ObjectContext_TwoPropertiesMappedToSameColumn((object) reader.GetName(source2.Key), (object) string.Join(", ", source2.Select<Tuple<MemberAssignment, int, EdmProperty>, string>((Func<Tuple<MemberAssignment, int, EdmProperty>, string>) (tuple => tuple.Item3.Name)).ToArray<string>())));
        Tuple<MemberAssignment, int, EdmProperty> tuple1 = source2.Single<Tuple<MemberAssignment, int, EdmProperty>>();
        MemberAssignment memberAssignment = tuple1.Item1;
        int columnPos = tuple1.Item2;
        EdmProperty edmProperty = tuple1.Item3;
        memberInfoArray[index] = memberAssignment.Member;
        memberBindingArray[index] = (MemberBinding) memberAssignment;
        properties[index] = (ColumnMap) new ScalarColumnMap(edmProperty.TypeUsage, edmProperty.Name, 0, columnPos);
        edmPropertyArray[index] = edmProperty;
        ++index;
      }
      MemberInitExpression initExpression = Expression.MemberInit((ConstructorInfo) null == declaredConstructor ? Expression.New(type) : Expression.New(declaredConstructor), memberBindingArray);
      InitializerMetadata projectionInitializer = InitializerMetadata.CreateProjectionInitializer((EdmItemCollection) workspace.GetItemCollection(DataSpace.CSpace), initExpression);
      RowType rowType = new RowType((IEnumerable<EdmProperty>) edmPropertyArray, projectionInitializer);
      RecordColumnMap recordColumnMap = new RecordColumnMap(TypeUsage.Create((EdmType) rowType), "DefaultTypeProjection", properties, (SimpleColumnMap) null);
      return (CollectionColumnMap) new SimpleCollectionColumnMap(rowType.GetCollectionType().TypeUsage, rowType.Name, (ColumnMap) recordColumnMap, (SimpleColumnMap[]) null, (SimpleColumnMap[]) null);
    }

    private static EntityColumnMap CreateEntityTypeElementColumnMap(
      DbDataReader storeDataReader,
      EdmType edmType,
      EntitySet entitySet,
      ColumnMap[] propertyColumnMaps,
      Dictionary<string, FunctionImportReturnTypeStructuralTypeColumnRenameMapping> renameList)
    {
      EntityType entityType = (EntityType) edmType;
      ColumnMap[] columnMapArray = new ColumnMap[storeDataReader.FieldCount];
      foreach (ColumnMap propertyColumnMap in propertyColumnMaps)
      {
        int columnPos = ((ScalarColumnMap) propertyColumnMap).ColumnPos;
        columnMapArray[columnPos] = propertyColumnMap;
      }
      IList<EdmMember> keyMembers = (IList<EdmMember>) entityType.KeyMembers;
      SimpleColumnMap[] keyColumns = new SimpleColumnMap[keyMembers.Count];
      int index = 0;
      foreach (EdmMember member in (IEnumerable<EdmMember>) keyMembers)
      {
        int ordinalFromReader = ColumnMapFactory.GetMemberOrdinalFromReader(storeDataReader, member, edmType, renameList);
        ColumnMap columnMap = columnMapArray[ordinalFromReader];
        keyColumns[index] = (SimpleColumnMap) columnMap;
        ++index;
      }
      SimpleEntityIdentity simpleEntityIdentity = new SimpleEntityIdentity(entitySet, keyColumns);
      return new EntityColumnMap(TypeUsage.Create(edmType), edmType.Name, propertyColumnMaps, (EntityIdentity) simpleEntityIdentity);
    }

    private static ColumnMap[] GetColumnMapsForType(
      DbDataReader storeDataReader,
      EdmType edmType,
      Dictionary<string, FunctionImportReturnTypeStructuralTypeColumnRenameMapping> renameList)
    {
      IBaseList<EdmMember> structuralMembers = TypeHelpers.GetAllStructuralMembers(edmType);
      ColumnMap[] columnMapArray = new ColumnMap[structuralMembers.Count];
      int index = 0;
      foreach (EdmMember member in (IEnumerable) structuralMembers)
      {
        if (!Helper.IsScalarType(member.TypeUsage.EdmType))
          throw new InvalidOperationException(Strings.ADP_InvalidDataReaderUnableToMaterializeNonScalarType((object) member.Name, (object) member.TypeUsage.EdmType.FullName));
        int ordinalFromReader = ColumnMapFactory.GetMemberOrdinalFromReader(storeDataReader, member, edmType, renameList);
        columnMapArray[index] = (ColumnMap) new ScalarColumnMap(member.TypeUsage, member.Name, 0, ordinalFromReader);
        ++index;
      }
      return columnMapArray;
    }

    private static ScalarColumnMap[] CreateDiscriminatorColumnMaps(
      DbDataReader storeDataReader,
      FunctionImportMappingNonComposable mapping,
      int resultIndex)
    {
      TypeUsage type = TypeUsage.Create((EdmType) MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.String));
      IList<string> discriminatorColumns = mapping.GetDiscriminatorColumns(resultIndex);
      ScalarColumnMap[] scalarColumnMapArray = new ScalarColumnMap[discriminatorColumns.Count];
      for (int index = 0; index < scalarColumnMapArray.Length; ++index)
      {
        string str = discriminatorColumns[index];
        ScalarColumnMap scalarColumnMap = new ScalarColumnMap(type, str, 0, ColumnMapFactory.GetDiscriminatorOrdinalFromReader(storeDataReader, str, mapping.FunctionImport));
        scalarColumnMapArray[index] = scalarColumnMap;
      }
      return scalarColumnMapArray;
    }

    private static int GetMemberOrdinalFromReader(
      DbDataReader storeDataReader,
      EdmMember member,
      EdmType currentType,
      Dictionary<string, FunctionImportReturnTypeStructuralTypeColumnRenameMapping> renameList)
    {
      string renameForMember = ColumnMapFactory.GetRenameForMember(member, currentType, renameList);
      int ordinal;
      if (!ColumnMapFactory.TryGetColumnOrdinalFromReader(storeDataReader, renameForMember, out ordinal))
        throw new EntityCommandExecutionException(Strings.ADP_InvalidDataReaderMissingColumnForType((object) currentType.FullName, (object) member.Name));
      return ordinal;
    }

    private static string GetRenameForMember(
      EdmMember member,
      EdmType currentType,
      Dictionary<string, FunctionImportReturnTypeStructuralTypeColumnRenameMapping> renameList)
    {
      if (renameList != null && renameList.Count != 0 && renameList.Any<KeyValuePair<string, FunctionImportReturnTypeStructuralTypeColumnRenameMapping>>((Func<KeyValuePair<string, FunctionImportReturnTypeStructuralTypeColumnRenameMapping>, bool>) (m => m.Key == member.Name)))
        return renameList[member.Name].GetRename(currentType);
      return member.Name;
    }

    private static int GetDiscriminatorOrdinalFromReader(
      DbDataReader storeDataReader,
      string columnName,
      EdmFunction functionImport)
    {
      int ordinal;
      if (!ColumnMapFactory.TryGetColumnOrdinalFromReader(storeDataReader, columnName, out ordinal))
        throw new EntityCommandExecutionException(Strings.ADP_InvalidDataReaderMissingDiscriminatorColumn((object) columnName, (object) functionImport.FullName));
      return ordinal;
    }

    private static bool TryGetColumnOrdinalFromReader(
      DbDataReader storeDataReader,
      string columnName,
      out int ordinal)
    {
      if (storeDataReader.FieldCount == 0)
      {
        ordinal = 0;
        return false;
      }
      try
      {
        ordinal = storeDataReader.GetOrdinal(columnName);
        return true;
      }
      catch (IndexOutOfRangeException ex)
      {
        ordinal = 0;
        return false;
      }
    }
  }
}
