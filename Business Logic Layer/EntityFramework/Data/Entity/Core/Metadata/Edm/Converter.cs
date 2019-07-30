// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.Converter
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Metadata.Edm.Provider;
using System.Data.Entity.Core.SchemaObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace System.Data.Entity.Core.Metadata.Edm
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
  internal static class Converter
  {
    internal static readonly FacetDescription ConcurrencyModeFacet;
    internal static readonly FacetDescription StoreGeneratedPatternFacet;
    internal static readonly FacetDescription CollationFacet;

    [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
    static Converter()
    {
      EnumType enumType1 = new EnumType("ConcurrencyMode", "Edm", PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.Int32), false, DataSpace.CSpace);
      foreach (string name in Enum.GetNames(typeof (ConcurrencyMode)))
        enumType1.AddMember(new EnumMember(name, (object) (int) Enum.Parse(typeof (ConcurrencyMode), name, false)));
      EnumType enumType2 = new EnumType("StoreGeneratedPattern", "Edm", PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.Int32), false, DataSpace.CSpace);
      foreach (string name in Enum.GetNames(typeof (StoreGeneratedPattern)))
        enumType2.AddMember(new EnumMember(name, (object) (int) Enum.Parse(typeof (StoreGeneratedPattern), name, false)));
      Converter.ConcurrencyModeFacet = new FacetDescription("ConcurrencyMode", (EdmType) enumType1, new int?(), new int?(), (object) ConcurrencyMode.None);
      Converter.StoreGeneratedPatternFacet = new FacetDescription("StoreGeneratedPattern", (EdmType) enumType2, new int?(), new int?(), (object) StoreGeneratedPattern.None);
      Converter.CollationFacet = new FacetDescription("Collation", (EdmType) MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.String), new int?(), new int?(), (object) string.Empty);
    }

    internal static IEnumerable<GlobalItem> ConvertSchema(
      Schema somSchema,
      DbProviderManifest providerManifest,
      ItemCollection itemCollection)
    {
      Dictionary<SchemaElement, GlobalItem> newGlobalItems = new Dictionary<SchemaElement, GlobalItem>();
      Converter.ConvertSchema(somSchema, providerManifest, new Converter.ConversionCache(itemCollection), newGlobalItems);
      return (IEnumerable<GlobalItem>) newGlobalItems.Values;
    }

    internal static IEnumerable<GlobalItem> ConvertSchema(
      IList<Schema> somSchemas,
      DbProviderManifest providerManifest,
      ItemCollection itemCollection)
    {
      Dictionary<SchemaElement, GlobalItem> newGlobalItems = new Dictionary<SchemaElement, GlobalItem>();
      Converter.ConversionCache convertedItemCache = new Converter.ConversionCache(itemCollection);
      foreach (Schema somSchema in (IEnumerable<Schema>) somSchemas)
        Converter.ConvertSchema(somSchema, providerManifest, convertedItemCache, newGlobalItems);
      return (IEnumerable<GlobalItem>) newGlobalItems.Values;
    }

    private static void ConvertSchema(
      Schema somSchema,
      DbProviderManifest providerManifest,
      Converter.ConversionCache convertedItemCache,
      Dictionary<SchemaElement, GlobalItem> newGlobalItems)
    {
      List<System.Data.Entity.Core.SchemaObjectModel.Function> functionList = new List<System.Data.Entity.Core.SchemaObjectModel.Function>();
      foreach (System.Data.Entity.Core.SchemaObjectModel.SchemaType schemaType in somSchema.SchemaTypes)
      {
        if (Converter.LoadSchemaElement(schemaType, providerManifest, convertedItemCache, newGlobalItems) == null)
        {
          System.Data.Entity.Core.SchemaObjectModel.Function function = schemaType as System.Data.Entity.Core.SchemaObjectModel.Function;
          if (function != null)
            functionList.Add(function);
        }
      }
      foreach (SchemaEntityType element in somSchema.SchemaTypes.OfType<SchemaEntityType>())
        Converter.LoadEntityTypePhase2(element, providerManifest, convertedItemCache, newGlobalItems);
      foreach (System.Data.Entity.Core.SchemaObjectModel.SchemaType element in functionList)
        Converter.LoadSchemaElement(element, providerManifest, convertedItemCache, newGlobalItems);
      if (convertedItemCache.ItemCollection.DataSpace == DataSpace.CSpace)
      {
        ((EdmItemCollection) convertedItemCache.ItemCollection).EdmVersion = somSchema.SchemaVersion;
      }
      else
      {
        StoreItemCollection itemCollection = convertedItemCache.ItemCollection as StoreItemCollection;
        if (itemCollection == null)
          return;
        itemCollection.StoreSchemaVersion = somSchema.SchemaVersion;
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
    internal static MetadataItem LoadSchemaElement(
      System.Data.Entity.Core.SchemaObjectModel.SchemaType element,
      DbProviderManifest providerManifest,
      Converter.ConversionCache convertedItemCache,
      Dictionary<SchemaElement, GlobalItem> newGlobalItems)
    {
      GlobalItem globalItem;
      if (newGlobalItems.TryGetValue((SchemaElement) element, out globalItem))
        return (MetadataItem) globalItem;
      System.Data.Entity.Core.SchemaObjectModel.EntityContainer element1 = element as System.Data.Entity.Core.SchemaObjectModel.EntityContainer;
      if (element1 != null)
        return (MetadataItem) Converter.ConvertToEntityContainer(element1, providerManifest, convertedItemCache, newGlobalItems);
      if (element is SchemaEntityType)
        return (MetadataItem) Converter.ConvertToEntityType((SchemaEntityType) element, providerManifest, convertedItemCache, newGlobalItems);
      if (element is Relationship)
        return (MetadataItem) Converter.ConvertToAssociationType((Relationship) element, providerManifest, convertedItemCache, newGlobalItems);
      if (element is SchemaComplexType)
        return (MetadataItem) Converter.ConvertToComplexType((SchemaComplexType) element, providerManifest, convertedItemCache, newGlobalItems);
      if (element is System.Data.Entity.Core.SchemaObjectModel.Function)
        return (MetadataItem) Converter.ConvertToFunction((System.Data.Entity.Core.SchemaObjectModel.Function) element, providerManifest, convertedItemCache, (EntityContainer) null, newGlobalItems);
      if (element is SchemaEnumType)
        return (MetadataItem) Converter.ConvertToEnumType((SchemaEnumType) element, newGlobalItems);
      return (MetadataItem) null;
    }

    private static EntityContainer ConvertToEntityContainer(
      System.Data.Entity.Core.SchemaObjectModel.EntityContainer element,
      DbProviderManifest providerManifest,
      Converter.ConversionCache convertedItemCache,
      Dictionary<SchemaElement, GlobalItem> newGlobalItems)
    {
      EntityContainer entityContainer = new EntityContainer(element.Name, Converter.GetDataSpace(providerManifest));
      newGlobalItems.Add((SchemaElement) element, (GlobalItem) entityContainer);
      foreach (EntityContainerEntitySet entitySet in element.EntitySets)
        entityContainer.AddEntitySetBase((EntitySetBase) Converter.ConvertToEntitySet(entitySet, providerManifest, convertedItemCache, newGlobalItems));
      foreach (EntityContainerRelationshipSet relationshipSet in element.RelationshipSets)
        entityContainer.AddEntitySetBase((EntitySetBase) Converter.ConvertToAssociationSet(relationshipSet, providerManifest, convertedItemCache, entityContainer, newGlobalItems));
      foreach (System.Data.Entity.Core.SchemaObjectModel.Function functionImport in element.FunctionImports)
        entityContainer.AddFunctionImport(Converter.ConvertToFunction(functionImport, providerManifest, convertedItemCache, entityContainer, newGlobalItems));
      if (element.Documentation != null)
        entityContainer.Documentation = Converter.ConvertToDocumentation(element.Documentation);
      Converter.AddOtherContent((SchemaElement) element, (MetadataItem) entityContainer);
      return entityContainer;
    }

    private static EntityType ConvertToEntityType(
      SchemaEntityType element,
      DbProviderManifest providerManifest,
      Converter.ConversionCache convertedItemCache,
      Dictionary<SchemaElement, GlobalItem> newGlobalItems)
    {
      string[] strArray = (string[]) null;
      if (element.DeclaredKeyProperties.Count != 0)
      {
        strArray = new string[element.DeclaredKeyProperties.Count];
        for (int index = 0; index < strArray.Length; ++index)
          strArray[index] = element.DeclaredKeyProperties[index].Property.Name;
      }
      EdmProperty[] edmPropertyArray = new EdmProperty[element.Properties.Count];
      int num = 0;
      foreach (StructuredProperty property in element.Properties)
        edmPropertyArray[num++] = Converter.ConvertToProperty(property, providerManifest, convertedItemCache, newGlobalItems);
      EntityType entityType = new EntityType(element.Name, element.Namespace, Converter.GetDataSpace(providerManifest), (IEnumerable<string>) strArray, (IEnumerable<EdmMember>) edmPropertyArray);
      if (element.BaseType != null)
        entityType.BaseType = (EdmType) Converter.LoadSchemaElement((System.Data.Entity.Core.SchemaObjectModel.SchemaType) element.BaseType, providerManifest, convertedItemCache, newGlobalItems);
      entityType.Abstract = element.IsAbstract;
      if (element.Documentation != null)
        entityType.Documentation = Converter.ConvertToDocumentation(element.Documentation);
      Converter.AddOtherContent((SchemaElement) element, (MetadataItem) entityType);
      newGlobalItems.Add((SchemaElement) element, (GlobalItem) entityType);
      return entityType;
    }

    private static void LoadEntityTypePhase2(
      SchemaEntityType element,
      DbProviderManifest providerManifest,
      Converter.ConversionCache convertedItemCache,
      Dictionary<SchemaElement, GlobalItem> newGlobalItems)
    {
      EntityType newGlobalItem = (EntityType) newGlobalItems[(SchemaElement) element];
      foreach (System.Data.Entity.Core.SchemaObjectModel.NavigationProperty navigationProperty in element.NavigationProperties)
        newGlobalItem.AddMember((EdmMember) Converter.ConvertToNavigationProperty(newGlobalItem, navigationProperty, providerManifest, convertedItemCache, newGlobalItems));
    }

    private static ComplexType ConvertToComplexType(
      SchemaComplexType element,
      DbProviderManifest providerManifest,
      Converter.ConversionCache convertedItemCache,
      Dictionary<SchemaElement, GlobalItem> newGlobalItems)
    {
      ComplexType complexType = new ComplexType(element.Name, element.Namespace, Converter.GetDataSpace(providerManifest));
      newGlobalItems.Add((SchemaElement) element, (GlobalItem) complexType);
      foreach (StructuredProperty property in element.Properties)
        complexType.AddMember((EdmMember) Converter.ConvertToProperty(property, providerManifest, convertedItemCache, newGlobalItems));
      complexType.Abstract = element.IsAbstract;
      if (element.BaseType != null)
        complexType.BaseType = (EdmType) Converter.LoadSchemaElement((System.Data.Entity.Core.SchemaObjectModel.SchemaType) element.BaseType, providerManifest, convertedItemCache, newGlobalItems);
      if (element.Documentation != null)
        complexType.Documentation = Converter.ConvertToDocumentation(element.Documentation);
      Converter.AddOtherContent((SchemaElement) element, (MetadataItem) complexType);
      return complexType;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    private static AssociationType ConvertToAssociationType(
      Relationship element,
      DbProviderManifest providerManifest,
      Converter.ConversionCache convertedItemCache,
      Dictionary<SchemaElement, GlobalItem> newGlobalItems)
    {
      AssociationType associationType = new AssociationType(element.Name, element.Namespace, element.IsForeignKey, Converter.GetDataSpace(providerManifest));
      newGlobalItems.Add((SchemaElement) element, (GlobalItem) associationType);
      foreach (RelationshipEnd end in (IEnumerable<IRelationshipEnd>) element.Ends)
      {
        EntityType endMemberType = (EntityType) Converter.LoadSchemaElement((System.Data.Entity.Core.SchemaObjectModel.SchemaType) end.Type, providerManifest, convertedItemCache, newGlobalItems);
        AssociationEndMember associationEndMember = Converter.InitializeAssociationEndMember(associationType, (IRelationshipEnd) end, endMemberType);
        Converter.AddOtherContent((SchemaElement) end, (MetadataItem) associationEndMember);
        foreach (OnOperation operation in (IEnumerable<OnOperation>) end.Operations)
        {
          if (operation.Operation == Operation.Delete)
          {
            OperationAction operationAction = OperationAction.None;
            switch (operation.Action)
            {
              case System.Data.Entity.Core.SchemaObjectModel.Action.None:
                operationAction = OperationAction.None;
                break;
              case System.Data.Entity.Core.SchemaObjectModel.Action.Cascade:
                operationAction = OperationAction.Cascade;
                break;
            }
            associationEndMember.DeleteBehavior = operationAction;
          }
        }
        if (end.Documentation != null)
          associationEndMember.Documentation = Converter.ConvertToDocumentation(end.Documentation);
      }
      for (int index = 0; index < element.Constraints.Count; ++index)
      {
        System.Data.Entity.Core.SchemaObjectModel.ReferentialConstraint constraint = element.Constraints[index];
        AssociationEndMember member1 = (AssociationEndMember) associationType.Members[constraint.PrincipalRole.Name];
        AssociationEndMember member2 = (AssociationEndMember) associationType.Members[constraint.DependentRole.Name];
        EntityTypeBase elementType1 = ((RefType) member1.TypeUsage.EdmType).ElementType;
        EntityTypeBase elementType2 = ((RefType) member2.TypeUsage.EdmType).ElementType;
        ReferentialConstraint referentialConstraint = new ReferentialConstraint((RelationshipEndMember) member1, (RelationshipEndMember) member2, (IEnumerable<EdmProperty>) Converter.GetProperties(elementType1, constraint.PrincipalRole.RoleProperties), (IEnumerable<EdmProperty>) Converter.GetProperties(elementType2, constraint.DependentRole.RoleProperties));
        if (constraint.Documentation != null)
          referentialConstraint.Documentation = Converter.ConvertToDocumentation(constraint.Documentation);
        if (constraint.PrincipalRole.Documentation != null)
          referentialConstraint.FromRole.Documentation = Converter.ConvertToDocumentation(constraint.PrincipalRole.Documentation);
        if (constraint.DependentRole.Documentation != null)
          referentialConstraint.ToRole.Documentation = Converter.ConvertToDocumentation(constraint.DependentRole.Documentation);
        associationType.AddReferentialConstraint(referentialConstraint);
        Converter.AddOtherContent((SchemaElement) element.Constraints[index], (MetadataItem) referentialConstraint);
      }
      if (element.Documentation != null)
        associationType.Documentation = Converter.ConvertToDocumentation(element.Documentation);
      Converter.AddOtherContent((SchemaElement) element, (MetadataItem) associationType);
      return associationType;
    }

    private static AssociationEndMember InitializeAssociationEndMember(
      AssociationType associationType,
      IRelationshipEnd end,
      EntityType endMemberType)
    {
      EdmMember edmMember;
      AssociationEndMember associationEndMember;
      if (!associationType.Members.TryGetValue(end.Name, false, out edmMember))
      {
        associationEndMember = new AssociationEndMember(end.Name, endMemberType.GetReferenceType(), end.Multiplicity.Value);
        associationType.AddKeyMember((EdmMember) associationEndMember);
      }
      else
        associationEndMember = (AssociationEndMember) edmMember;
      RelationshipEnd relationshipEnd = end as RelationshipEnd;
      if (relationshipEnd != null && relationshipEnd.Documentation != null)
        associationEndMember.Documentation = Converter.ConvertToDocumentation(relationshipEnd.Documentation);
      return associationEndMember;
    }

    private static EdmProperty[] GetProperties(
      EntityTypeBase entityType,
      IList<PropertyRefElement> properties)
    {
      EdmProperty[] edmPropertyArray = new EdmProperty[properties.Count];
      for (int index = 0; index < properties.Count; ++index)
        edmPropertyArray[index] = (EdmProperty) entityType.Members[properties[index].Name];
      return edmPropertyArray;
    }

    private static void AddOtherContent(SchemaElement element, MetadataItem item)
    {
      if (element.OtherContent.Count <= 0)
        return;
      item.AddMetadataProperties(element.OtherContent);
    }

    private static EntitySet ConvertToEntitySet(
      EntityContainerEntitySet set,
      DbProviderManifest providerManifest,
      Converter.ConversionCache convertedItemCache,
      Dictionary<SchemaElement, GlobalItem> newGlobalItems)
    {
      EntitySet entitySet = new EntitySet(set.Name, set.DbSchema, set.Table, set.DefiningQuery, (EntityType) Converter.LoadSchemaElement((System.Data.Entity.Core.SchemaObjectModel.SchemaType) set.EntityType, providerManifest, convertedItemCache, newGlobalItems));
      if (set.Documentation != null)
        entitySet.Documentation = Converter.ConvertToDocumentation(set.Documentation);
      Converter.AddOtherContent((SchemaElement) set, (MetadataItem) entitySet);
      return entitySet;
    }

    private static EntitySet GetEntitySet(
      EntityContainerEntitySet set,
      EntityContainer container)
    {
      return container.GetEntitySetByName(set.Name, false);
    }

    private static AssociationSet ConvertToAssociationSet(
      EntityContainerRelationshipSet relationshipSet,
      DbProviderManifest providerManifest,
      Converter.ConversionCache convertedItemCache,
      EntityContainer container,
      Dictionary<SchemaElement, GlobalItem> newGlobalItems)
    {
      AssociationType associationType = (AssociationType) Converter.LoadSchemaElement((System.Data.Entity.Core.SchemaObjectModel.SchemaType) relationshipSet.Relationship, providerManifest, convertedItemCache, newGlobalItems);
      AssociationSet parentSet = new AssociationSet(relationshipSet.Name, associationType);
      foreach (EntityContainerRelationshipSetEnd end in relationshipSet.Ends)
      {
        EntityType entityType = (EntityType) Converter.LoadSchemaElement((System.Data.Entity.Core.SchemaObjectModel.SchemaType) end.EntitySet.EntityType, providerManifest, convertedItemCache, newGlobalItems);
        AssociationEndMember member = (AssociationEndMember) associationType.Members[end.Name];
        AssociationSetEnd associationSetEnd = new AssociationSetEnd(Converter.GetEntitySet(end.EntitySet, container), parentSet, member);
        Converter.AddOtherContent((SchemaElement) end, (MetadataItem) associationSetEnd);
        parentSet.AddAssociationSetEnd(associationSetEnd);
        if (end.Documentation != null)
          associationSetEnd.Documentation = Converter.ConvertToDocumentation(end.Documentation);
      }
      if (relationshipSet.Documentation != null)
        parentSet.Documentation = Converter.ConvertToDocumentation(relationshipSet.Documentation);
      Converter.AddOtherContent((SchemaElement) relationshipSet, (MetadataItem) parentSet);
      return parentSet;
    }

    private static EdmProperty ConvertToProperty(
      StructuredProperty somProperty,
      DbProviderManifest providerManifest,
      Converter.ConversionCache convertedItemCache,
      Dictionary<SchemaElement, GlobalItem> newGlobalItems)
    {
      ScalarType type = somProperty.Type as ScalarType;
      TypeUsage typeUsage;
      if (type != null && somProperty.Schema.DataModel != SchemaDataModelOption.EntityDataModel)
      {
        typeUsage = somProperty.TypeUsage;
        Converter.UpdateSentinelValuesInFacets(ref typeUsage);
      }
      else
      {
        EdmType edmType = type == null ? (EdmType) Converter.LoadSchemaElement(somProperty.Type, providerManifest, convertedItemCache, newGlobalItems) : (EdmType) convertedItemCache.ItemCollection.GetItem<PrimitiveType>(somProperty.TypeUsage.EdmType.FullName);
        if (somProperty.CollectionKind != CollectionKind.None)
        {
          typeUsage = TypeUsage.Create((EdmType) new CollectionType(edmType));
        }
        else
        {
          SchemaEnumType schemaEnumType = type == null ? somProperty.Type as SchemaEnumType : (SchemaEnumType) null;
          typeUsage = TypeUsage.Create(edmType);
          if (schemaEnumType != null)
            somProperty.EnsureEnumTypeFacets(convertedItemCache, newGlobalItems);
          if (somProperty.TypeUsage != null)
            Converter.ApplyTypePropertyFacets(somProperty.TypeUsage, ref typeUsage);
        }
      }
      Converter.PopulateGeneralFacets(somProperty, ref typeUsage);
      EdmProperty edmProperty = new EdmProperty(somProperty.Name, typeUsage);
      if (somProperty.Documentation != null)
        edmProperty.Documentation = Converter.ConvertToDocumentation(somProperty.Documentation);
      Converter.AddOtherContent((SchemaElement) somProperty, (MetadataItem) edmProperty);
      return edmProperty;
    }

    private static NavigationProperty ConvertToNavigationProperty(
      EntityType declaringEntityType,
      System.Data.Entity.Core.SchemaObjectModel.NavigationProperty somNavigationProperty,
      DbProviderManifest providerManifest,
      Converter.ConversionCache convertedItemCache,
      Dictionary<SchemaElement, GlobalItem> newGlobalItems)
    {
      EntityType endMemberType = (EntityType) Converter.LoadSchemaElement(somNavigationProperty.Type, providerManifest, convertedItemCache, newGlobalItems);
      AssociationType associationType = (AssociationType) Converter.LoadSchemaElement((System.Data.Entity.Core.SchemaObjectModel.SchemaType) somNavigationProperty.Relationship, providerManifest, convertedItemCache, newGlobalItems);
      IRelationshipEnd end = (IRelationshipEnd) null;
      somNavigationProperty.Relationship.TryGetEnd(somNavigationProperty.ToEnd.Name, out end);
      RelationshipMultiplicity? multiplicity1 = end.Multiplicity;
      EdmType edmType = (multiplicity1.GetValueOrDefault() != RelationshipMultiplicity.Many ? 0 : (multiplicity1.HasValue ? 1 : 0)) == 0 ? (EdmType) endMemberType : (EdmType) endMemberType.GetCollectionType();
      RelationshipMultiplicity? multiplicity2 = end.Multiplicity;
      TypeUsage typeUsage;
      if ((multiplicity2.GetValueOrDefault() != RelationshipMultiplicity.One ? 0 : (multiplicity2.HasValue ? 1 : 0)) != 0)
        typeUsage = TypeUsage.Create(edmType, new FacetValues()
        {
          Nullable = (FacetValueContainer<bool?>) new bool?(false)
        });
      else
        typeUsage = TypeUsage.Create(edmType);
      Converter.InitializeAssociationEndMember(associationType, somNavigationProperty.ToEnd, endMemberType);
      Converter.InitializeAssociationEndMember(associationType, somNavigationProperty.FromEnd, declaringEntityType);
      NavigationProperty navigationProperty = new NavigationProperty(somNavigationProperty.Name, typeUsage);
      navigationProperty.RelationshipType = (RelationshipType) associationType;
      navigationProperty.ToEndMember = (RelationshipEndMember) associationType.Members[somNavigationProperty.ToEnd.Name];
      navigationProperty.FromEndMember = (RelationshipEndMember) associationType.Members[somNavigationProperty.FromEnd.Name];
      if (somNavigationProperty.Documentation != null)
        navigationProperty.Documentation = Converter.ConvertToDocumentation(somNavigationProperty.Documentation);
      Converter.AddOtherContent((SchemaElement) somNavigationProperty, (MetadataItem) navigationProperty);
      return navigationProperty;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
    private static EdmFunction ConvertToFunction(
      System.Data.Entity.Core.SchemaObjectModel.Function somFunction,
      DbProviderManifest providerManifest,
      Converter.ConversionCache convertedItemCache,
      EntityContainer functionImportEntityContainer,
      Dictionary<SchemaElement, GlobalItem> newGlobalItems)
    {
      GlobalItem globalItem = (GlobalItem) null;
      if (!somFunction.IsFunctionImport && newGlobalItems.TryGetValue((SchemaElement) somFunction, out globalItem))
        return (EdmFunction) globalItem;
      bool areConvertingForProviderManifest = somFunction.Schema.DataModel == SchemaDataModelOption.ProviderManifestModel;
      List<FunctionParameter> functionParameterList1 = new List<FunctionParameter>();
      if (somFunction.ReturnTypeList != null)
      {
        int num = 0;
        foreach (ReturnType returnType in (IEnumerable<ReturnType>) somFunction.ReturnTypeList)
        {
          TypeUsage functionTypeUsage = Converter.GetFunctionTypeUsage(somFunction is ModelFunction, somFunction, (FacetEnabledSchemaElement) returnType, providerManifest, areConvertingForProviderManifest, returnType.Type, returnType.CollectionKind, returnType.IsRefType, convertedItemCache, newGlobalItems);
          if (functionTypeUsage == null)
            return (EdmFunction) null;
          string str = num == 0 ? string.Empty : num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          ++num;
          FunctionParameter functionParameter = new FunctionParameter("ReturnType" + str, functionTypeUsage, ParameterMode.ReturnValue);
          Converter.AddOtherContent((SchemaElement) returnType, (MetadataItem) functionParameter);
          functionParameterList1.Add(functionParameter);
        }
      }
      else if (somFunction.Type != null)
      {
        TypeUsage functionTypeUsage = Converter.GetFunctionTypeUsage(somFunction is ModelFunction, somFunction, (FacetEnabledSchemaElement) null, providerManifest, areConvertingForProviderManifest, somFunction.Type, somFunction.CollectionKind, somFunction.IsReturnAttributeReftype, convertedItemCache, newGlobalItems);
        if (functionTypeUsage == null)
          return (EdmFunction) null;
        functionParameterList1.Add(new FunctionParameter("ReturnType", functionTypeUsage, ParameterMode.ReturnValue));
      }
      EntitySet[] entitySetArray = (EntitySet[]) null;
      string name;
      if (somFunction.IsFunctionImport)
      {
        FunctionImportElement functionImportElement = (FunctionImportElement) somFunction;
        name = functionImportElement.Container.Name;
        if (functionImportElement.EntitySet != null)
        {
          EntityContainer container = functionImportEntityContainer;
          entitySetArray = new EntitySet[1]
          {
            Converter.GetEntitySet(functionImportElement.EntitySet, container)
          };
        }
        else if (functionImportElement.ReturnTypeList != null)
        {
          EntityContainer entityContainer = functionImportEntityContainer;
          entitySetArray = functionImportElement.ReturnTypeList.Select<ReturnType, EntitySet>((Func<ReturnType, EntitySet>) (returnType =>
          {
            if (returnType.EntitySet == null)
              return (EntitySet) null;
            return Converter.GetEntitySet(returnType.EntitySet, functionImportEntityContainer);
          })).ToArray<EntitySet>();
        }
      }
      else
        name = somFunction.Namespace;
      List<FunctionParameter> functionParameterList2 = new List<FunctionParameter>();
      foreach (Parameter parameter in somFunction.Parameters)
      {
        TypeUsage functionTypeUsage = Converter.GetFunctionTypeUsage(somFunction is ModelFunction, somFunction, (FacetEnabledSchemaElement) parameter, providerManifest, areConvertingForProviderManifest, parameter.Type, parameter.CollectionKind, parameter.IsRefType, convertedItemCache, newGlobalItems);
        if (functionTypeUsage == null)
          return (EdmFunction) null;
        FunctionParameter functionParameter = new FunctionParameter(parameter.Name, functionTypeUsage, Converter.GetParameterMode(parameter.ParameterDirection));
        Converter.AddOtherContent((SchemaElement) parameter, (MetadataItem) functionParameter);
        if (parameter.Documentation != null)
          functionParameter.Documentation = Converter.ConvertToDocumentation(parameter.Documentation);
        functionParameterList2.Add(functionParameter);
      }
      EdmFunction edmFunction = new EdmFunction(somFunction.Name, name, Converter.GetDataSpace(providerManifest), new EdmFunctionPayload()
      {
        Schema = somFunction.DbSchema,
        StoreFunctionName = somFunction.StoreFunctionName,
        CommandText = somFunction.CommandText,
        EntitySets = (IList<EntitySet>) entitySetArray,
        IsAggregate = new bool?(somFunction.IsAggregate),
        IsBuiltIn = new bool?(somFunction.IsBuiltIn),
        IsNiladic = new bool?(somFunction.IsNiladicFunction),
        IsComposable = new bool?(somFunction.IsComposable),
        IsFromProviderManifest = new bool?(areConvertingForProviderManifest),
        IsFunctionImport = new bool?(somFunction.IsFunctionImport),
        ReturnParameters = (IList<FunctionParameter>) functionParameterList1.ToArray(),
        Parameters = (IList<FunctionParameter>) functionParameterList2.ToArray(),
        ParameterTypeSemantics = new ParameterTypeSemantics?(somFunction.ParameterTypeSemantics)
      });
      if (!somFunction.IsFunctionImport)
        newGlobalItems.Add((SchemaElement) somFunction, (GlobalItem) edmFunction);
      if (somFunction.Documentation != null)
        edmFunction.Documentation = Converter.ConvertToDocumentation(somFunction.Documentation);
      Converter.AddOtherContent((SchemaElement) somFunction, (MetadataItem) edmFunction);
      return edmFunction;
    }

    private static EnumType ConvertToEnumType(
      SchemaEnumType somEnumType,
      Dictionary<SchemaElement, GlobalItem> newGlobalItems)
    {
      ScalarType underlyingType = (ScalarType) somEnumType.UnderlyingType;
      EnumType enumType = new EnumType(somEnumType.Name, somEnumType.Namespace, underlyingType.Type, somEnumType.IsFlags, DataSpace.CSpace);
      Type clrEquivalentType = underlyingType.Type.ClrEquivalentType;
      foreach (SchemaEnumMember enumMember1 in somEnumType.EnumMembers)
      {
        EnumMember enumMember2 = new EnumMember(enumMember1.Name, Convert.ChangeType((object) enumMember1.Value, clrEquivalentType, (IFormatProvider) CultureInfo.InvariantCulture));
        if (enumMember1.Documentation != null)
          enumMember2.Documentation = Converter.ConvertToDocumentation(enumMember1.Documentation);
        Converter.AddOtherContent((SchemaElement) enumMember1, (MetadataItem) enumMember2);
        enumType.AddMember(enumMember2);
      }
      if (somEnumType.Documentation != null)
        enumType.Documentation = Converter.ConvertToDocumentation(somEnumType.Documentation);
      Converter.AddOtherContent((SchemaElement) somEnumType, (MetadataItem) enumType);
      newGlobalItems.Add((SchemaElement) somEnumType, (GlobalItem) enumType);
      return enumType;
    }

    private static Documentation ConvertToDocumentation(DocumentationElement element)
    {
      return element.MetadataDocumentation;
    }

    [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private static TypeUsage GetFunctionTypeUsage(
      bool isModelFunction,
      System.Data.Entity.Core.SchemaObjectModel.Function somFunction,
      FacetEnabledSchemaElement somParameter,
      DbProviderManifest providerManifest,
      bool areConvertingForProviderManifest,
      System.Data.Entity.Core.SchemaObjectModel.SchemaType type,
      CollectionKind collectionKind,
      bool isRefType,
      Converter.ConversionCache convertedItemCache,
      Dictionary<SchemaElement, GlobalItem> newGlobalItems)
    {
      if (somParameter != null && areConvertingForProviderManifest && somParameter.HasUserDefinedFacets)
        return somParameter.TypeUsage;
      if (type == null)
      {
        if (isModelFunction && somParameter != null && somParameter is Parameter)
        {
          ((Parameter) somParameter).ResolveNestedTypeNames(convertedItemCache, newGlobalItems);
          return somParameter.TypeUsage;
        }
        if (somParameter == null || !(somParameter is ReturnType))
          return (TypeUsage) null;
        ((ReturnType) somParameter).ResolveNestedTypeNames(convertedItemCache, newGlobalItems);
        return somParameter.TypeUsage;
      }
      EdmType edmType;
      if (!areConvertingForProviderManifest)
      {
        ScalarType scalarType = type as ScalarType;
        if (scalarType != null)
        {
          if (isModelFunction && somParameter != null)
          {
            if (somParameter.TypeUsage == null)
              somParameter.ValidateAndSetTypeUsage(scalarType);
            return somParameter.TypeUsage;
          }
          if (isModelFunction)
          {
            ModelFunction modelFunction = somFunction as ModelFunction;
            if (modelFunction.TypeUsage == null)
              modelFunction.ValidateAndSetTypeUsage(scalarType);
            return modelFunction.TypeUsage;
          }
          if (somParameter != null && somParameter.HasUserDefinedFacets && somFunction.Schema.DataModel == SchemaDataModelOption.ProviderDataModel)
          {
            somParameter.ValidateAndSetTypeUsage(scalarType);
            return somParameter.TypeUsage;
          }
          edmType = (EdmType) Converter.GetPrimitiveType(scalarType, providerManifest);
        }
        else
        {
          edmType = (EdmType) Converter.LoadSchemaElement(type, providerManifest, convertedItemCache, newGlobalItems);
          if (isModelFunction && type is SchemaEnumType)
          {
            if (somParameter != null)
            {
              somParameter.ValidateAndSetTypeUsage(edmType);
              return somParameter.TypeUsage;
            }
            if (somFunction != null)
            {
              ModelFunction modelFunction = (ModelFunction) somFunction;
              modelFunction.ValidateAndSetTypeUsage(edmType);
              return modelFunction.TypeUsage;
            }
          }
        }
      }
      else
        edmType = !(type is TypeElement) ? (EdmType) (type as ScalarType).Type : (EdmType) (type as TypeElement).PrimitiveType;
      TypeUsage typeUsage;
      if (collectionKind != CollectionKind.None)
      {
        typeUsage = convertedItemCache.GetCollectionTypeUsageWithNullFacets(edmType);
      }
      else
      {
        EntityType entityType = edmType as EntityType;
        typeUsage = entityType == null || !isRefType ? convertedItemCache.GetTypeUsageWithNullFacets(edmType) : TypeUsage.Create((EdmType) new RefType(entityType));
      }
      return typeUsage;
    }

    private static ParameterMode GetParameterMode(ParameterDirection parameterDirection)
    {
      switch (parameterDirection)
      {
        case ParameterDirection.Input:
          return ParameterMode.In;
        case ParameterDirection.Output:
          return ParameterMode.Out;
        default:
          return ParameterMode.InOut;
      }
    }

    private static void ApplyTypePropertyFacets(TypeUsage sourceType, ref TypeUsage targetType)
    {
      Dictionary<string, Facet> dictionary = targetType.Facets.ToDictionary<Facet, string>((Func<Facet, string>) (f => f.Name));
      bool flag = false;
      foreach (Facet facet1 in sourceType.Facets)
      {
        Facet facet2;
        if (dictionary.TryGetValue(facet1.Name, out facet2))
        {
          if (!facet2.Description.IsConstant)
          {
            flag = true;
            dictionary[facet2.Name] = Facet.Create(facet2.Description, facet1.Value);
          }
        }
        else
        {
          flag = true;
          dictionary.Add(facet1.Name, facet1);
        }
      }
      if (!flag)
        return;
      targetType = TypeUsage.Create(targetType.EdmType, (IEnumerable<Facet>) dictionary.Values);
    }

    private static void PopulateGeneralFacets(
      StructuredProperty somProperty,
      ref TypeUsage propertyTypeUsage)
    {
      bool flag = false;
      Dictionary<string, Facet> dictionary = propertyTypeUsage.Facets.ToDictionary<Facet, string>((Func<Facet, string>) (f => f.Name));
      if (!somProperty.Nullable)
      {
        dictionary["Nullable"] = Facet.Create(MetadataItem.NullableFacetDescription, (object) false);
        flag = true;
      }
      if (somProperty.Default != null)
      {
        dictionary["DefaultValue"] = Facet.Create(MetadataItem.DefaultValueFacetDescription, somProperty.DefaultAsObject);
        flag = true;
      }
      if (somProperty.Schema.SchemaVersion == 1.1)
      {
        Facet facet = Facet.Create(MetadataItem.CollectionKindFacetDescription, (object) somProperty.CollectionKind);
        dictionary.Add(facet.Name, facet);
        flag = true;
      }
      if (!flag)
        return;
      propertyTypeUsage = TypeUsage.Create(propertyTypeUsage.EdmType, (IEnumerable<Facet>) dictionary.Values);
    }

    private static DataSpace GetDataSpace(DbProviderManifest providerManifest)
    {
      return providerManifest is EdmProviderManifest ? DataSpace.CSpace : DataSpace.SSpace;
    }

    private static PrimitiveType GetPrimitiveType(
      ScalarType scalarType,
      DbProviderManifest providerManifest)
    {
      PrimitiveType primitiveType = (PrimitiveType) null;
      string name = scalarType.Name;
      foreach (PrimitiveType storeType in providerManifest.GetStoreTypes())
      {
        if (storeType.Name == name)
        {
          primitiveType = storeType;
          break;
        }
      }
      return primitiveType;
    }

    private static void UpdateSentinelValuesInFacets(ref TypeUsage typeUsage)
    {
      PrimitiveType edmType = (PrimitiveType) typeUsage.EdmType;
      if (edmType.PrimitiveTypeKind != PrimitiveTypeKind.String && edmType.PrimitiveTypeKind != PrimitiveTypeKind.Binary || !Helper.IsUnboundedFacetValue(typeUsage.Facets["MaxLength"]))
        return;
      typeUsage = typeUsage.ShallowCopy(new FacetValues()
      {
        MaxLength = (FacetValueContainer<int?>) Helper.GetFacet((IEnumerable<FacetDescription>) edmType.FacetDescriptions, "MaxLength").MaxValue
      });
    }

    internal class ConversionCache
    {
      internal readonly ItemCollection ItemCollection;
      private readonly Dictionary<EdmType, TypeUsage> _nullFacetsTypeUsage;
      private readonly Dictionary<EdmType, TypeUsage> _nullFacetsCollectionTypeUsage;

      internal ConversionCache(ItemCollection itemCollection)
      {
        this.ItemCollection = itemCollection;
        this._nullFacetsTypeUsage = new Dictionary<EdmType, TypeUsage>();
        this._nullFacetsCollectionTypeUsage = new Dictionary<EdmType, TypeUsage>();
      }

      internal TypeUsage GetTypeUsageWithNullFacets(EdmType edmType)
      {
        TypeUsage typeUsage1;
        if (this._nullFacetsTypeUsage.TryGetValue(edmType, out typeUsage1))
          return typeUsage1;
        TypeUsage typeUsage2 = TypeUsage.Create(edmType, FacetValues.NullFacetValues);
        this._nullFacetsTypeUsage.Add(edmType, typeUsage2);
        return typeUsage2;
      }

      internal TypeUsage GetCollectionTypeUsageWithNullFacets(EdmType edmType)
      {
        TypeUsage typeUsage1;
        if (this._nullFacetsCollectionTypeUsage.TryGetValue(edmType, out typeUsage1))
          return typeUsage1;
        TypeUsage typeUsage2 = TypeUsage.Create((EdmType) new CollectionType(this.GetTypeUsageWithNullFacets(edmType)), FacetValues.NullFacetValues);
        this._nullFacetsCollectionTypeUsage.Add(edmType, typeUsage2);
        return typeUsage2;
      }
    }
  }
}
