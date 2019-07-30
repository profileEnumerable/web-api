// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.EdmModelSemanticValidationRules
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace System.Data.Entity.Core.Metadata.Edm
{
  [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode")]
  internal static class EdmModelSemanticValidationRules
  {
    internal static readonly EdmModelValidationRule<EdmFunction> EdmFunction_ComposableFunctionImportsNotAllowed_V1_V2 = new EdmModelValidationRule<EdmFunction>((Action<EdmModelValidationContext, EdmFunction>) ((context, function) =>
    {
      if (!function.IsFunctionImport || !function.IsComposableAttribute)
        return;
      context.AddError((MetadataItem) function, (string) null, Strings.EdmModel_Validator_Semantic_ComposableFunctionImportsNotSupportedForSchemaVersion);
    }));
    internal static readonly EdmModelValidationRule<EdmFunction> EdmFunction_DuplicateParameterName = new EdmModelValidationRule<EdmFunction>((Action<EdmModelValidationContext, EdmFunction>) ((context, function) =>
    {
      HashSet<string> memberNameList = new HashSet<string>();
      foreach (FunctionParameter parameter in function.Parameters)
      {
        if (parameter != null && !string.IsNullOrWhiteSpace(parameter.Name))
          EdmModelSemanticValidationRules.AddMemberNameToHashSet((INamedDataModelItem) parameter, memberNameList, context, new Func<string, string>(Strings.ParameterNameAlreadyDefinedDuplicate));
      }
    }));
    internal static readonly EdmModelValidationRule<EdmType> EdmType_SystemNamespaceEncountered = new EdmModelValidationRule<EdmType>((Action<EdmModelValidationContext, EdmType>) ((context, edmType) =>
    {
      if (!EdmModelSemanticValidationRules.IsEdmSystemNamespace(edmType.NamespaceName) || edmType.BuiltInTypeKind == BuiltInTypeKind.RowType || (edmType.BuiltInTypeKind == BuiltInTypeKind.CollectionType || edmType.BuiltInTypeKind == BuiltInTypeKind.PrimitiveType))
        return;
      context.AddError((MetadataItem) edmType, (string) null, Strings.EdmModel_Validator_Semantic_SystemNamespaceEncountered((object) edmType.Name));
    }));
    internal static readonly EdmModelValidationRule<EntityContainer> EdmEntityContainer_SimilarRelationshipEnd = new EdmModelValidationRule<EntityContainer>((Action<EdmModelValidationContext, EntityContainer>) ((context, edmEntityContainer) =>
    {
      List<KeyValuePair<AssociationSet, EntitySet>> source1 = new List<KeyValuePair<AssociationSet, EntitySet>>();
      List<KeyValuePair<AssociationSet, EntitySet>> source2 = new List<KeyValuePair<AssociationSet, EntitySet>>();
      foreach (AssociationSet associationSet in edmEntityContainer.AssociationSets)
      {
        KeyValuePair<AssociationSet, EntitySet> sourceEnd = new KeyValuePair<AssociationSet, EntitySet>(associationSet, associationSet.SourceSet);
        KeyValuePair<AssociationSet, EntitySet> targetEnd = new KeyValuePair<AssociationSet, EntitySet>(associationSet, associationSet.TargetSet);
        KeyValuePair<AssociationSet, EntitySet> keyValuePair1 = source1.FirstOrDefault<KeyValuePair<AssociationSet, EntitySet>>((Func<KeyValuePair<AssociationSet, EntitySet>, bool>) (e => EdmModelSemanticValidationRules.AreRelationshipEndsEqual(e, sourceEnd)));
        KeyValuePair<AssociationSet, EntitySet> keyValuePair2 = source2.FirstOrDefault<KeyValuePair<AssociationSet, EntitySet>>((Func<KeyValuePair<AssociationSet, EntitySet>, bool>) (e => EdmModelSemanticValidationRules.AreRelationshipEndsEqual(e, targetEnd)));
        if (!keyValuePair1.Equals((object) new KeyValuePair<AssociationSet, EntitySet>()))
          context.AddError((MetadataItem) edmEntityContainer, (string) null, Strings.EdmModel_Validator_Semantic_SimilarRelationshipEnd((object) keyValuePair1.Key.ElementType.SourceEnd.Name, (object) keyValuePair1.Key.Name, (object) associationSet.Name, (object) keyValuePair1.Value.Name, (object) edmEntityContainer.Name));
        else
          source1.Add(sourceEnd);
        if (!keyValuePair2.Equals((object) new KeyValuePair<AssociationSet, EntitySet>()))
          context.AddError((MetadataItem) edmEntityContainer, (string) null, Strings.EdmModel_Validator_Semantic_SimilarRelationshipEnd((object) keyValuePair2.Key.ElementType.TargetEnd.Name, (object) keyValuePair2.Key.Name, (object) associationSet.Name, (object) keyValuePair2.Value.Name, (object) edmEntityContainer.Name));
        else
          source2.Add(targetEnd);
      }
    }));
    internal static readonly EdmModelValidationRule<EntityContainer> EdmEntityContainer_InvalidEntitySetNameReference = new EdmModelValidationRule<EntityContainer>((Action<EdmModelValidationContext, EntityContainer>) ((context, edmEntityContainer) =>
    {
      if (edmEntityContainer.AssociationSets == null)
        return;
      foreach (AssociationSet associationSet in edmEntityContainer.AssociationSets)
      {
        if (associationSet.SourceSet != null && associationSet.ElementType != null && (associationSet.ElementType.SourceEnd != null && !edmEntityContainer.EntitySets.Contains(associationSet.SourceSet)))
          context.AddError((MetadataItem) associationSet.SourceSet, (string) null, Strings.EdmModel_Validator_Semantic_InvalidEntitySetNameReference((object) associationSet.SourceSet.Name, (object) associationSet.ElementType.SourceEnd.Name));
        if (associationSet.TargetSet != null && associationSet.ElementType != null && (associationSet.ElementType.TargetEnd != null && !edmEntityContainer.EntitySets.Contains(associationSet.TargetSet)))
          context.AddError((MetadataItem) associationSet.TargetSet, (string) null, Strings.EdmModel_Validator_Semantic_InvalidEntitySetNameReference((object) associationSet.TargetSet.Name, (object) associationSet.ElementType.TargetEnd.Name));
      }
    }));
    internal static readonly EdmModelValidationRule<EntityContainer> EdmEntityContainer_ConcurrencyRedefinedOnSubTypeOfEntitySetType = new EdmModelValidationRule<EntityContainer>((Action<EdmModelValidationContext, EntityContainer>) ((context, edmEntityContainer) =>
    {
      Dictionary<EntityType, EntitySet> baseEntitySetTypes = new Dictionary<EntityType, EntitySet>();
      foreach (EntitySet entitySet in edmEntityContainer.EntitySets)
      {
        if (entitySet != null && entitySet.ElementType != null && !baseEntitySetTypes.ContainsKey(entitySet.ElementType))
          baseEntitySetTypes.Add(entitySet.ElementType, entitySet);
      }
      foreach (EntityType entityType in context.Model.EntityTypes)
      {
        EntitySet set;
        if (EdmModelSemanticValidationRules.TypeIsSubTypeOf(entityType, baseEntitySetTypes, out set) && EdmModelSemanticValidationRules.IsTypeDefinesNewConcurrencyProperties(entityType))
          context.AddError((MetadataItem) entityType, (string) null, Strings.EdmModel_Validator_Semantic_ConcurrencyRedefinedOnSubTypeOfEntitySetType((object) EdmModelSemanticValidationRules.GetQualifiedName((INamedDataModelItem) entityType, entityType.NamespaceName), (object) EdmModelSemanticValidationRules.GetQualifiedName((INamedDataModelItem) set.ElementType, set.ElementType.NamespaceName), (object) EdmModelSemanticValidationRules.GetQualifiedName((INamedDataModelItem) set, set.EntityContainer.Name)));
      }
    }));
    internal static readonly EdmModelValidationRule<EntityContainer> EdmEntityContainer_DuplicateEntityContainerMemberName = new EdmModelValidationRule<EntityContainer>((Action<EdmModelValidationContext, EntityContainer>) ((context, edmEntityContainer) =>
    {
      HashSet<string> memberNameList = new HashSet<string>();
      foreach (INamedDataModelItem baseEntitySet in edmEntityContainer.BaseEntitySets)
        EdmModelSemanticValidationRules.AddMemberNameToHashSet(baseEntitySet, memberNameList, context, new Func<string, string>(Strings.EdmModel_Validator_Semantic_DuplicateEntityContainerMemberName));
    }));
    internal static readonly EdmModelValidationRule<EntityContainer> EdmEntityContainer_DuplicateEntitySetTable = new EdmModelValidationRule<EntityContainer>((Action<EdmModelValidationContext, EntityContainer>) ((context, edmEntityContainer) =>
    {
      HashSet<string> stringSet = new HashSet<string>();
      foreach (EntitySetBase baseEntitySet in edmEntityContainer.BaseEntitySets)
      {
        if (!string.IsNullOrWhiteSpace(baseEntitySet.Table))
        {
          if (!stringSet.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) baseEntitySet.Schema, (object) baseEntitySet.Table)))
            context.AddError((MetadataItem) baseEntitySet, "Name", Strings.DuplicateEntitySetTable((object) baseEntitySet.Name, (object) baseEntitySet.Schema, (object) baseEntitySet.Table));
        }
      }
    }));
    internal static readonly EdmModelValidationRule<EntitySet> EdmEntitySet_EntitySetTypeHasNoKeys = new EdmModelValidationRule<EntitySet>((Action<EdmModelValidationContext, EntitySet>) ((context, edmEntitySet) =>
    {
      if (edmEntitySet.ElementType == null || edmEntitySet.ElementType.GetValidKey().Any<EdmProperty>())
        return;
      context.AddError((MetadataItem) edmEntitySet, "EntityType", Strings.EdmModel_Validator_Semantic_EntitySetTypeHasNoKeys((object) edmEntitySet.Name, (object) edmEntitySet.ElementType.Name));
    }));
    internal static readonly EdmModelValidationRule<AssociationSet> EdmAssociationSet_DuplicateEndName = new EdmModelValidationRule<AssociationSet>((Action<EdmModelValidationContext, AssociationSet>) ((context, edmAssociationSet) =>
    {
      if (edmAssociationSet.ElementType == null || edmAssociationSet.ElementType.SourceEnd == null || (edmAssociationSet.ElementType.TargetEnd == null || !(edmAssociationSet.ElementType.SourceEnd.Name == edmAssociationSet.ElementType.TargetEnd.Name)))
        return;
      context.AddError((MetadataItem) edmAssociationSet.SourceSet, "Name", Strings.EdmModel_Validator_Semantic_DuplicateEndName((object) edmAssociationSet.ElementType.SourceEnd.Name));
    }));
    internal static readonly EdmModelValidationRule<EntityType> EdmEntityType_DuplicatePropertyNameSpecifiedInEntityKey = new EdmModelValidationRule<EntityType>((Action<EdmModelValidationContext, EntityType>) ((context, edmEntityType) =>
    {
      List<EdmProperty> list = edmEntityType.GetKeyProperties().ToList<EdmProperty>();
      if (list.Count <= 0)
        return;
      List<EdmProperty> edmPropertyList = new List<EdmProperty>();
      foreach (EdmProperty edmProperty in list)
      {
        EdmProperty key = edmProperty;
        if (key != null && !edmPropertyList.Contains(key))
        {
          if (list.Count<EdmProperty>((Func<EdmProperty, bool>) (p => key.Equals((object) p))) > 1)
            context.AddError((MetadataItem) key, (string) null, Strings.EdmModel_Validator_Semantic_DuplicatePropertyNameSpecifiedInEntityKey((object) edmEntityType.Name, (object) key.Name));
          edmPropertyList.Add(key);
        }
      }
    }));
    internal static readonly EdmModelValidationRule<EntityType> EdmEntityType_InvalidKeyNullablePart = new EdmModelValidationRule<EntityType>((Action<EdmModelValidationContext, EntityType>) ((context, edmEntityType) =>
    {
      foreach (EdmProperty edmProperty in edmEntityType.GetValidKey())
      {
        if (edmProperty.IsPrimitiveType && edmProperty.Nullable)
          context.AddError((MetadataItem) edmProperty, "Nullable", Strings.EdmModel_Validator_Semantic_InvalidKeyNullablePart((object) edmProperty.Name, (object) edmEntityType.Name));
      }
    }));
    internal static readonly EdmModelValidationRule<EntityType> EdmEntityType_EntityKeyMustBeScalar = new EdmModelValidationRule<EntityType>((Action<EdmModelValidationContext, EntityType>) ((context, edmEntityType) =>
    {
      foreach (EdmProperty edmProperty in edmEntityType.GetValidKey())
      {
        if (!edmProperty.IsUnderlyingPrimitiveType)
          context.AddError((MetadataItem) edmProperty, (string) null, Strings.EdmModel_Validator_Semantic_EntityKeyMustBeScalar((object) edmEntityType.Name, (object) edmProperty.Name));
      }
    }));
    internal static readonly EdmModelValidationRule<EntityType> EdmEntityType_InvalidKeyKeyDefinedInBaseClass = new EdmModelValidationRule<EntityType>((Action<EdmModelValidationContext, EntityType>) ((context, edmEntityType) =>
    {
      if (edmEntityType.BaseType == null || !edmEntityType.KeyProperties.Where<EdmProperty>((Func<EdmProperty, bool>) (key => edmEntityType.DeclaredMembers.Contains((EdmMember) key))).Any<EdmProperty>())
        return;
      context.AddError((MetadataItem) edmEntityType.BaseType, (string) null, Strings.EdmModel_Validator_Semantic_InvalidKeyKeyDefinedInBaseClass((object) edmEntityType.Name, (object) edmEntityType.BaseType.Name));
    }));
    internal static readonly EdmModelValidationRule<EntityType> EdmEntityType_KeyMissingOnEntityType = new EdmModelValidationRule<EntityType>((Action<EdmModelValidationContext, EntityType>) ((context, edmEntityType) =>
    {
      if (edmEntityType.BaseType != null || edmEntityType.KeyProperties.Count != 0)
        return;
      context.AddError((MetadataItem) edmEntityType, (string) null, Strings.EdmModel_Validator_Semantic_KeyMissingOnEntityType((object) edmEntityType.Name));
    }));
    internal static readonly EdmModelValidationRule<EntityType> EdmEntityType_InvalidMemberNameMatchesTypeName = new EdmModelValidationRule<EntityType>((Action<EdmModelValidationContext, EntityType>) ((context, edmEntityType) =>
    {
      List<EdmProperty> list = edmEntityType.Properties.ToList<EdmProperty>();
      if (string.IsNullOrWhiteSpace(edmEntityType.Name) || list.Count <= 0)
        return;
      foreach (EdmProperty edmProperty in list)
      {
        if (edmProperty != null && context.IsCSpace && edmProperty.Name.EqualsOrdinal(edmEntityType.Name))
          context.AddError((MetadataItem) edmProperty, "Name", Strings.EdmModel_Validator_Semantic_InvalidMemberNameMatchesTypeName((object) edmProperty.Name, (object) EdmModelSemanticValidationRules.GetQualifiedName((INamedDataModelItem) edmEntityType, edmEntityType.NamespaceName)));
      }
      if (!edmEntityType.DeclaredNavigationProperties.Any<NavigationProperty>())
        return;
      foreach (NavigationProperty navigationProperty in edmEntityType.DeclaredNavigationProperties)
      {
        if (navigationProperty != null && navigationProperty.Name.EqualsOrdinal(edmEntityType.Name))
          context.AddError((MetadataItem) navigationProperty, "Name", Strings.EdmModel_Validator_Semantic_InvalidMemberNameMatchesTypeName((object) navigationProperty.Name, (object) EdmModelSemanticValidationRules.GetQualifiedName((INamedDataModelItem) edmEntityType, edmEntityType.NamespaceName)));
      }
    }));
    internal static readonly EdmModelValidationRule<EntityType> EdmEntityType_PropertyNameAlreadyDefinedDuplicate = new EdmModelValidationRule<EntityType>((Action<EdmModelValidationContext, EntityType>) ((context, edmEntityType) =>
    {
      HashSet<string> memberNameList = new HashSet<string>();
      foreach (EdmProperty property in edmEntityType.Properties)
      {
        if (property != null && !string.IsNullOrWhiteSpace(property.Name))
          EdmModelSemanticValidationRules.AddMemberNameToHashSet((INamedDataModelItem) property, memberNameList, context, new Func<string, string>(Strings.EdmModel_Validator_Semantic_PropertyNameAlreadyDefinedDuplicate));
      }
      if (!edmEntityType.DeclaredNavigationProperties.Any<NavigationProperty>())
        return;
      foreach (NavigationProperty navigationProperty in edmEntityType.DeclaredNavigationProperties)
      {
        if (navigationProperty != null && !string.IsNullOrWhiteSpace(navigationProperty.Name))
          EdmModelSemanticValidationRules.AddMemberNameToHashSet((INamedDataModelItem) navigationProperty, memberNameList, context, new Func<string, string>(Strings.EdmModel_Validator_Semantic_PropertyNameAlreadyDefinedDuplicate));
      }
    }));
    internal static readonly EdmModelValidationRule<EntityType> EdmEntityType_CycleInTypeHierarchy = new EdmModelValidationRule<EntityType>((Action<EdmModelValidationContext, EntityType>) ((context, edmEntityType) =>
    {
      if (!EdmModelSemanticValidationRules.CheckForInheritanceCycle<EntityType>(edmEntityType, (Func<EntityType, EntityType>) (et => (EntityType) et.BaseType)))
        return;
      context.AddError((MetadataItem) edmEntityType, "BaseType", Strings.EdmModel_Validator_Semantic_CycleInTypeHierarchy((object) EdmModelSemanticValidationRules.GetQualifiedName((INamedDataModelItem) edmEntityType, edmEntityType.NamespaceName)));
    }));
    internal static readonly EdmModelValidationRule<NavigationProperty> EdmNavigationProperty_BadNavigationPropertyUndefinedRole = new EdmModelValidationRule<NavigationProperty>((Action<EdmModelValidationContext, NavigationProperty>) ((context, edmNavigationProperty) =>
    {
      if (edmNavigationProperty.Association == null || edmNavigationProperty.Association.SourceEnd == null || (edmNavigationProperty.Association.TargetEnd == null || edmNavigationProperty.Association.SourceEnd.Name == null) || (edmNavigationProperty.Association.TargetEnd.Name == null || edmNavigationProperty.ToEndMember == edmNavigationProperty.Association.SourceEnd || edmNavigationProperty.ToEndMember == edmNavigationProperty.Association.TargetEnd))
        return;
      context.AddError((MetadataItem) edmNavigationProperty, (string) null, Strings.EdmModel_Validator_Semantic_BadNavigationPropertyUndefinedRole((object) edmNavigationProperty.Association.SourceEnd.Name, (object) edmNavigationProperty.Association.TargetEnd.Name, (object) edmNavigationProperty.Association.Name));
    }));
    internal static readonly EdmModelValidationRule<NavigationProperty> EdmNavigationProperty_BadNavigationPropertyRolesCannotBeTheSame = new EdmModelValidationRule<NavigationProperty>((Action<EdmModelValidationContext, NavigationProperty>) ((context, edmNavigationProperty) =>
    {
      if (edmNavigationProperty.Association == null || edmNavigationProperty.Association.SourceEnd == null || (edmNavigationProperty.Association.TargetEnd == null || edmNavigationProperty.ToEndMember != edmNavigationProperty.GetFromEnd()))
        return;
      context.AddError((MetadataItem) edmNavigationProperty, "ToRole", Strings.EdmModel_Validator_Semantic_BadNavigationPropertyRolesCannotBeTheSame);
    }));
    internal static readonly EdmModelValidationRule<NavigationProperty> EdmNavigationProperty_BadNavigationPropertyBadFromRoleType = new EdmModelValidationRule<NavigationProperty>((Action<EdmModelValidationContext, NavigationProperty>) ((context, edmNavigationProperty) =>
    {
      AssociationEndMember fromEnd;
      if (edmNavigationProperty.Association == null || (fromEnd = edmNavigationProperty.GetFromEnd()) == null)
        return;
      EntityType entityType1 = (EntityType) null;
      IList<EntityType> entityTypeList = context.Model.EntityTypes as IList<EntityType> ?? (IList<EntityType>) context.Model.EntityTypes.ToList<EntityType>();
      for (int index = 0; index < entityTypeList.Count; ++index)
      {
        EntityType entityType2 = entityTypeList[index];
        if (entityType2.DeclaredNavigationProperties.Contains(edmNavigationProperty))
        {
          entityType1 = entityType2;
          break;
        }
      }
      EntityType entityType3 = fromEnd.GetEntityType();
      if (entityType1 == entityType3)
        return;
      context.AddError((MetadataItem) edmNavigationProperty, "FromRole", Strings.BadNavigationPropertyBadFromRoleType((object) edmNavigationProperty.Name, (object) entityType3.Name, (object) fromEnd.Name, (object) edmNavigationProperty.Association.Name, (object) entityType1.Name));
    }));
    internal static readonly EdmModelValidationRule<AssociationType> EdmAssociationType_InvalidOperationMultipleEndsInAssociation = new EdmModelValidationRule<AssociationType>((Action<EdmModelValidationContext, AssociationType>) ((context, edmAssociationType) =>
    {
      if (edmAssociationType.SourceEnd == null || edmAssociationType.SourceEnd.DeleteBehavior == OperationAction.None || (edmAssociationType.TargetEnd == null || edmAssociationType.TargetEnd.DeleteBehavior == OperationAction.None))
        return;
      context.AddError((MetadataItem) edmAssociationType, (string) null, Strings.EdmModel_Validator_Semantic_InvalidOperationMultipleEndsInAssociation);
    }));
    internal static readonly EdmModelValidationRule<AssociationType> EdmAssociationType_EndWithManyMultiplicityCannotHaveOperationsSpecified = new EdmModelValidationRule<AssociationType>((Action<EdmModelValidationContext, AssociationType>) ((context, edmAssociationType) =>
    {
      if (edmAssociationType.SourceEnd != null && edmAssociationType.SourceEnd.RelationshipMultiplicity == RelationshipMultiplicity.Many && edmAssociationType.SourceEnd.DeleteBehavior != OperationAction.None)
        context.AddError((MetadataItem) edmAssociationType.SourceEnd, "OnDelete", Strings.EdmModel_Validator_Semantic_EndWithManyMultiplicityCannotHaveOperationsSpecified((object) edmAssociationType.SourceEnd.Name, (object) edmAssociationType.Name));
      if (edmAssociationType.TargetEnd == null || edmAssociationType.TargetEnd.RelationshipMultiplicity != RelationshipMultiplicity.Many || edmAssociationType.TargetEnd.DeleteBehavior == OperationAction.None)
        return;
      context.AddError((MetadataItem) edmAssociationType.TargetEnd, "OnDelete", Strings.EdmModel_Validator_Semantic_EndWithManyMultiplicityCannotHaveOperationsSpecified((object) edmAssociationType.TargetEnd.Name, (object) edmAssociationType.Name));
    }));
    internal static readonly EdmModelValidationRule<AssociationType> EdmAssociationType_EndNameAlreadyDefinedDuplicate = new EdmModelValidationRule<AssociationType>((Action<EdmModelValidationContext, AssociationType>) ((context, edmAssociationType) =>
    {
      if (edmAssociationType.SourceEnd == null || edmAssociationType.TargetEnd == null || !(edmAssociationType.SourceEnd.Name == edmAssociationType.TargetEnd.Name))
        return;
      context.AddError((MetadataItem) edmAssociationType.SourceEnd, "Name", Strings.EdmModel_Validator_Semantic_EndNameAlreadyDefinedDuplicate((object) edmAssociationType.SourceEnd.Name));
    }));
    internal static readonly EdmModelValidationRule<AssociationType> EdmAssociationType_SameRoleReferredInReferentialConstraint = new EdmModelValidationRule<AssociationType>((Action<EdmModelValidationContext, AssociationType>) ((context, edmAssociationType) =>
    {
      if (!EdmModelSemanticValidationRules.IsReferentialConstraintReadyForValidation(edmAssociationType) || !(edmAssociationType.Constraint.FromRole.Name == edmAssociationType.Constraint.ToRole.Name))
        return;
      context.AddError((MetadataItem) edmAssociationType.Constraint.ToRole, (string) null, Strings.EdmModel_Validator_Semantic_SameRoleReferredInReferentialConstraint((object) edmAssociationType.Name));
    }));
    internal static readonly EdmModelValidationRule<AssociationType> EdmAssociationType_ValidateReferentialConstraint = new EdmModelValidationRule<AssociationType>((Action<EdmModelValidationContext, AssociationType>) ((context, edmAssociationType) =>
    {
      if (!EdmModelSemanticValidationRules.IsReferentialConstraintReadyForValidation(edmAssociationType))
        return;
      ReferentialConstraint constraint = edmAssociationType.Constraint;
      RelationshipEndMember fromRole = constraint.FromRole;
      RelationshipEndMember toRole = constraint.ToRole;
      bool isKeyProperty1;
      bool areAllPropertiesNullable1;
      bool isAnyPropertyNullable1;
      bool isSubsetOfKeyProperties1;
      EdmModelSemanticValidationRules.IsKeyProperty(constraint.ToProperties.ToList<EdmProperty>(), toRole, out isKeyProperty1, out areAllPropertiesNullable1, out isAnyPropertyNullable1, out isSubsetOfKeyProperties1);
      bool isKeyProperty2;
      bool areAllPropertiesNullable2;
      bool isAnyPropertyNullable2;
      bool isSubsetOfKeyProperties2;
      EdmModelSemanticValidationRules.IsKeyProperty(constraint.FromRole.GetEntityType().GetValidKey().ToList<EdmProperty>(), fromRole, out isKeyProperty2, out areAllPropertiesNullable2, out isAnyPropertyNullable2, out isSubsetOfKeyProperties2);
      bool flag = context.Model.SchemaVersion <= 1.1;
      if (fromRole.RelationshipMultiplicity == RelationshipMultiplicity.Many)
        context.AddError((MetadataItem) fromRole, (string) null, Strings.EdmModel_Validator_Semantic_InvalidMultiplicityFromRoleUpperBoundMustBeOne((object) fromRole.Name, (object) edmAssociationType.Name));
      else if (areAllPropertiesNullable1 && fromRole.RelationshipMultiplicity == RelationshipMultiplicity.One)
      {
        string propertyNullableV1 = Strings.EdmModel_Validator_Semantic_InvalidMultiplicityFromRoleToPropertyNullableV1((object) fromRole.Name, (object) edmAssociationType.Name);
        context.AddError((MetadataItem) edmAssociationType, (string) null, propertyNullableV1);
      }
      else if ((flag && !areAllPropertiesNullable1 || !flag && !isAnyPropertyNullable1) && fromRole.RelationshipMultiplicity != RelationshipMultiplicity.One)
      {
        string errorMessage = !flag ? Strings.EdmModel_Validator_Semantic_InvalidMultiplicityFromRoleToPropertyNonNullableV2((object) fromRole.Name, (object) edmAssociationType.Name) : Strings.EdmModel_Validator_Semantic_InvalidMultiplicityFromRoleToPropertyNonNullableV1((object) fromRole.Name, (object) edmAssociationType.Name);
        context.AddError((MetadataItem) edmAssociationType, (string) null, errorMessage);
      }
      if (!isSubsetOfKeyProperties1 && !edmAssociationType.IsForeignKey(context.Model.SchemaVersion) && context.IsCSpace)
        context.AddError((MetadataItem) toRole, (string) null, Strings.EdmModel_Validator_Semantic_InvalidToPropertyInRelationshipConstraint((object) toRole.Name, (object) EdmModelSemanticValidationRules.GetQualifiedName((INamedDataModelItem) toRole.GetEntityType(), toRole.GetEntityType().NamespaceName), (object) EdmModelSemanticValidationRules.GetQualifiedName((INamedDataModelItem) edmAssociationType, edmAssociationType.NamespaceName)));
      if (isKeyProperty1)
      {
        if (toRole.RelationshipMultiplicity == RelationshipMultiplicity.Many)
          context.AddError((MetadataItem) toRole, (string) null, Strings.EdmModel_Validator_Semantic_InvalidMultiplicityToRoleUpperBoundMustBeOne((object) toRole.Name, (object) edmAssociationType.Name));
      }
      else if (toRole.RelationshipMultiplicity != RelationshipMultiplicity.Many)
        context.AddError((MetadataItem) toRole, (string) null, Strings.EdmModel_Validator_Semantic_InvalidMultiplicityToRoleUpperBoundMustBeMany((object) toRole.Name, (object) edmAssociationType.Name));
      List<EdmProperty> list1 = fromRole.GetEntityType().GetValidKey().ToList<EdmProperty>();
      List<EdmProperty> list2 = constraint.ToProperties.ToList<EdmProperty>();
      if (list2.Count != list1.Count)
      {
        context.AddError((MetadataItem) constraint, (string) null, Strings.EdmModel_Validator_Semantic_MismatchNumberOfPropertiesinRelationshipConstraint);
      }
      else
      {
        constraint.FromProperties.ToList<EdmProperty>();
        int count = list2.Count;
        for (int i = 0; i < count; ++i)
        {
          EdmProperty primitiveType1 = list2[i];
          EdmProperty primitiveType2 = list1.SingleOrDefault<EdmProperty>((Func<EdmProperty, bool>) (p => p.Name == principalProperties[i].Name));
          if (primitiveType2 != null && primitiveType1 != null && (primitiveType2.TypeUsage != null && primitiveType1.TypeUsage != null) && (primitiveType2.IsPrimitiveType && primitiveType1.IsPrimitiveType && !EdmModelSemanticValidationRules.IsPrimitiveTypesEqual(primitiveType1, primitiveType2)))
            context.AddError((MetadataItem) constraint, (string) null, Strings.EdmModel_Validator_Semantic_TypeMismatchRelationshipConstraint((object) constraint.ToProperties.ToList<EdmProperty>()[i].Name, (object) toRole.GetEntityType().Name, (object) primitiveType2.Name, (object) fromRole.GetEntityType().Name, (object) edmAssociationType.Name));
        }
      }
    }));
    internal static readonly EdmModelValidationRule<AssociationType> EdmAssociationType_InvalidPropertyInRelationshipConstraint = new EdmModelValidationRule<AssociationType>((Action<EdmModelValidationContext, AssociationType>) ((context, edmAssociationType) =>
    {
      if (edmAssociationType.Constraint == null || edmAssociationType.Constraint.ToRole == null || edmAssociationType.Constraint.ToRole.GetEntityType() == null)
        return;
      List<EdmProperty> list = edmAssociationType.Constraint.ToRole.GetEntityType().Properties.ToList<EdmProperty>();
      foreach (EdmProperty toProperty in edmAssociationType.Constraint.ToProperties)
      {
        if (toProperty != null && !list.Contains(toProperty))
          context.AddError((MetadataItem) toProperty, (string) null, Strings.EdmModel_Validator_Semantic_InvalidPropertyInRelationshipConstraint((object) toProperty.Name, (object) edmAssociationType.Constraint.ToRole.Name));
      }
    }));
    internal static readonly EdmModelValidationRule<ComplexType> EdmComplexType_InvalidIsAbstract = new EdmModelValidationRule<ComplexType>((Action<EdmModelValidationContext, ComplexType>) ((context, edmComplexType) =>
    {
      if (!edmComplexType.Abstract)
        return;
      context.AddError((MetadataItem) edmComplexType, "Abstract", Strings.EdmModel_Validator_Semantic_InvalidComplexTypeAbstract((object) EdmModelSemanticValidationRules.GetQualifiedName((INamedDataModelItem) edmComplexType, edmComplexType.NamespaceName)));
    }));
    internal static readonly EdmModelValidationRule<ComplexType> EdmComplexType_InvalidIsPolymorphic = new EdmModelValidationRule<ComplexType>((Action<EdmModelValidationContext, ComplexType>) ((context, edmComplexType) =>
    {
      if (edmComplexType.BaseType == null)
        return;
      context.AddError((MetadataItem) edmComplexType, "BaseType", Strings.EdmModel_Validator_Semantic_InvalidComplexTypePolymorphic((object) EdmModelSemanticValidationRules.GetQualifiedName((INamedDataModelItem) edmComplexType, edmComplexType.NamespaceName)));
    }));
    internal static readonly EdmModelValidationRule<ComplexType> EdmComplexType_InvalidMemberNameMatchesTypeName = new EdmModelValidationRule<ComplexType>((Action<EdmModelValidationContext, ComplexType>) ((context, edmComplexType) =>
    {
      if (string.IsNullOrWhiteSpace(edmComplexType.Name) || !edmComplexType.Properties.Any<EdmProperty>())
        return;
      foreach (EdmProperty property in edmComplexType.Properties)
      {
        if (property != null && property.Name.EqualsOrdinal(edmComplexType.Name))
          context.AddError((MetadataItem) property, "Name", Strings.EdmModel_Validator_Semantic_InvalidMemberNameMatchesTypeName((object) property.Name, (object) EdmModelSemanticValidationRules.GetQualifiedName((INamedDataModelItem) edmComplexType, edmComplexType.NamespaceName)));
      }
    }));
    internal static readonly EdmModelValidationRule<ComplexType> EdmComplexType_PropertyNameAlreadyDefinedDuplicate = new EdmModelValidationRule<ComplexType>((Action<EdmModelValidationContext, ComplexType>) ((context, edmComplexType) =>
    {
      if (!edmComplexType.Properties.Any<EdmProperty>())
        return;
      HashSet<string> memberNameList = new HashSet<string>();
      foreach (EdmProperty property in edmComplexType.Properties)
      {
        if (!string.IsNullOrWhiteSpace(property.Name))
          EdmModelSemanticValidationRules.AddMemberNameToHashSet((INamedDataModelItem) property, memberNameList, context, new Func<string, string>(Strings.EdmModel_Validator_Semantic_PropertyNameAlreadyDefinedDuplicate));
      }
    }));
    internal static readonly EdmModelValidationRule<ComplexType> EdmComplexType_PropertyNameAlreadyDefinedDuplicate_V1_1 = new EdmModelValidationRule<ComplexType>((Action<EdmModelValidationContext, ComplexType>) ((context, edmComplexType) =>
    {
      if (!edmComplexType.Properties.Any<EdmProperty>())
        return;
      HashSet<string> memberNameList = new HashSet<string>();
      foreach (EdmProperty property in edmComplexType.Properties)
      {
        if (property != null && !string.IsNullOrWhiteSpace(property.Name))
          EdmModelSemanticValidationRules.AddMemberNameToHashSet((INamedDataModelItem) property, memberNameList, context, new Func<string, string>(Strings.EdmModel_Validator_Semantic_PropertyNameAlreadyDefinedDuplicate));
      }
    }));
    internal static readonly EdmModelValidationRule<ComplexType> EdmComplexType_CycleInTypeHierarchy_V1_1 = new EdmModelValidationRule<ComplexType>((Action<EdmModelValidationContext, ComplexType>) ((context, edmComplexType) =>
    {
      if (!EdmModelSemanticValidationRules.CheckForInheritanceCycle<ComplexType>(edmComplexType, (Func<ComplexType, ComplexType>) (ct => (ComplexType) ct.BaseType)))
        return;
      context.AddError((MetadataItem) edmComplexType, "BaseType", Strings.EdmModel_Validator_Semantic_CycleInTypeHierarchy((object) EdmModelSemanticValidationRules.GetQualifiedName((INamedDataModelItem) edmComplexType, edmComplexType.NamespaceName)));
    }));
    internal static readonly EdmModelValidationRule<EdmProperty> EdmProperty_InvalidCollectionKind = new EdmModelValidationRule<EdmProperty>((Action<EdmModelValidationContext, EdmProperty>) ((context, edmProperty) =>
    {
      if (edmProperty.CollectionKind == CollectionKind.None)
        return;
      context.AddError((MetadataItem) edmProperty, "CollectionKind", Strings.EdmModel_Validator_Semantic_InvalidCollectionKindNotV1_1((object) edmProperty.Name));
    }));
    internal static readonly EdmModelValidationRule<EdmProperty> EdmProperty_InvalidCollectionKind_V1_1 = new EdmModelValidationRule<EdmProperty>((Action<EdmModelValidationContext, EdmProperty>) ((context, edmProperty) =>
    {
      if (edmProperty.CollectionKind == CollectionKind.None || edmProperty.TypeUsage == null || edmProperty.IsCollectionType)
        return;
      context.AddError((MetadataItem) edmProperty, "CollectionKind", Strings.EdmModel_Validator_Semantic_InvalidCollectionKindNotCollection((object) edmProperty.Name));
    }));
    internal static readonly EdmModelValidationRule<EdmProperty> EdmProperty_NullableComplexType = new EdmModelValidationRule<EdmProperty>((Action<EdmModelValidationContext, EdmProperty>) ((context, edmProperty) =>
    {
      if (edmProperty.TypeUsage == null || edmProperty.ComplexType == null || !edmProperty.Nullable)
        return;
      context.AddError((MetadataItem) edmProperty, "Nullable", Strings.EdmModel_Validator_Semantic_NullableComplexType((object) edmProperty.Name));
    }));
    internal static readonly EdmModelValidationRule<EdmProperty> EdmProperty_InvalidPropertyType = new EdmModelValidationRule<EdmProperty>((Action<EdmModelValidationContext, EdmProperty>) ((context, edmProperty) =>
    {
      if (edmProperty.TypeUsage.EdmType == null || edmProperty.IsPrimitiveType || edmProperty.IsComplexType)
        return;
      context.AddError((MetadataItem) edmProperty, "Type", Strings.EdmModel_Validator_Semantic_InvalidPropertyType(edmProperty.IsCollectionType ? (object) "CollectionType" : (object) edmProperty.TypeUsage.EdmType.BuiltInTypeKind.ToString()));
    }));
    internal static readonly EdmModelValidationRule<EdmProperty> EdmProperty_InvalidPropertyType_V1_1 = new EdmModelValidationRule<EdmProperty>((Action<EdmModelValidationContext, EdmProperty>) ((context, edmProperty) =>
    {
      if (edmProperty.TypeUsage == null || edmProperty.TypeUsage.EdmType == null || (edmProperty.IsPrimitiveType || edmProperty.IsComplexType) || edmProperty.IsCollectionType)
        return;
      context.AddError((MetadataItem) edmProperty, "Type", Strings.EdmModel_Validator_Semantic_InvalidPropertyType_V1_1((object) edmProperty.TypeUsage.EdmType.BuiltInTypeKind.ToString()));
    }));
    internal static readonly EdmModelValidationRule<EdmProperty> EdmProperty_InvalidPropertyType_V3 = new EdmModelValidationRule<EdmProperty>((Action<EdmModelValidationContext, EdmProperty>) ((context, edmProperty) =>
    {
      if (edmProperty.TypeUsage == null || edmProperty.TypeUsage.EdmType == null || (edmProperty.IsPrimitiveType || edmProperty.IsComplexType) || edmProperty.IsEnumType)
        return;
      context.AddError((MetadataItem) edmProperty, "Type", Strings.EdmModel_Validator_Semantic_InvalidPropertyType_V3((object) edmProperty.TypeUsage.EdmType.BuiltInTypeKind.ToString()));
    }));
    internal static readonly EdmModelValidationRule<EdmModel> EdmNamespace_TypeNameAlreadyDefinedDuplicate = new EdmModelValidationRule<EdmModel>((Action<EdmModelValidationContext, EdmModel>) ((context, model) =>
    {
      HashSet<string> memberNameList = new HashSet<string>();
      foreach (INamedDataModelItem namespaceItem in model.NamespaceItems)
        EdmModelSemanticValidationRules.AddMemberNameToHashSet(namespaceItem, memberNameList, context, new Func<string, string>(Strings.EdmModel_Validator_Semantic_TypeNameAlreadyDefinedDuplicate));
    }));

    private static string GetQualifiedName(INamedDataModelItem item, string qualifiedPrefix)
    {
      return qualifiedPrefix + "." + item.Name;
    }

    private static bool AreRelationshipEndsEqual(
      KeyValuePair<AssociationSet, EntitySet> left,
      KeyValuePair<AssociationSet, EntitySet> right)
    {
      return object.ReferenceEquals((object) left.Value, (object) right.Value) && object.ReferenceEquals((object) left.Key.ElementType, (object) right.Key.ElementType);
    }

    private static bool IsReferentialConstraintReadyForValidation(AssociationType association)
    {
      ReferentialConstraint constraint = association.Constraint;
      if (constraint == null || constraint.FromRole == null || (constraint.ToRole == null || constraint.FromRole.GetEntityType() == null) || (constraint.ToRole.GetEntityType() == null || !constraint.ToProperties.Any<EdmProperty>()))
        return false;
      foreach (EdmProperty toProperty in constraint.ToProperties)
      {
        if (toProperty == null || (toProperty.TypeUsage == null || toProperty.TypeUsage.EdmType == null))
          return false;
      }
      IEnumerable<EdmProperty> validKey = constraint.FromRole.GetEntityType().GetValidKey();
      if (validKey.Any<EdmProperty>())
        return validKey.All<EdmProperty>((Func<EdmProperty, bool>) (propRef =>
        {
          if (propRef != null && propRef.TypeUsage != null)
            return propRef.TypeUsage.EdmType != null;
          return false;
        }));
      return false;
    }

    private static void IsKeyProperty(
      List<EdmProperty> roleProperties,
      RelationshipEndMember roleElement,
      out bool isKeyProperty,
      out bool areAllPropertiesNullable,
      out bool isAnyPropertyNullable,
      out bool isSubsetOfKeyProperties)
    {
      isKeyProperty = true;
      areAllPropertiesNullable = true;
      isAnyPropertyNullable = false;
      isSubsetOfKeyProperties = true;
      if (roleElement.GetEntityType().GetValidKey().Count<EdmProperty>() != roleProperties.Count<EdmProperty>())
        isKeyProperty = false;
      for (int index = 0; index < roleProperties.Count<EdmProperty>(); ++index)
      {
        if (isSubsetOfKeyProperties && !roleElement.GetEntityType().GetValidKey().ToList<EdmProperty>().Contains(roleProperties[index]))
        {
          isKeyProperty = false;
          isSubsetOfKeyProperties = false;
        }
        bool nullable = roleProperties[index].Nullable;
        areAllPropertiesNullable &= nullable;
        isAnyPropertyNullable |= nullable;
      }
    }

    private static void AddMemberNameToHashSet(
      INamedDataModelItem item,
      HashSet<string> memberNameList,
      EdmModelValidationContext context,
      Func<string, string> getErrorString)
    {
      if (string.IsNullOrWhiteSpace(item.Name) || memberNameList.Add(item.Name))
        return;
      context.AddError((MetadataItem) item, "Name", getErrorString(item.Name));
    }

    private static bool CheckForInheritanceCycle<T>(T type, Func<T, T> getBaseType) where T : class
    {
      T obj1 = getBaseType(type);
      if ((object) obj1 != null)
      {
        T obj2 = obj1;
        T obj3 = obj1;
        do
        {
          obj3 = getBaseType(obj3);
          if (object.ReferenceEquals((object) obj2, (object) obj3))
            return true;
          if ((object) obj2 == null)
            return false;
          obj2 = getBaseType(obj2);
          if ((object) obj3 != null)
            obj3 = getBaseType(obj3);
        }
        while ((object) obj3 != null);
      }
      return false;
    }

    private static bool IsPrimitiveTypesEqual(
      EdmProperty primitiveType1,
      EdmProperty primitiveType2)
    {
      return primitiveType1.PrimitiveType.PrimitiveTypeKind == primitiveType2.PrimitiveType.PrimitiveTypeKind;
    }

    private static bool IsEdmSystemNamespace(string namespaceName)
    {
      if (!(namespaceName == "Transient") && !(namespaceName == "Edm"))
        return namespaceName == "System";
      return true;
    }

    private static bool IsTypeDefinesNewConcurrencyProperties(EntityType entityType)
    {
      return entityType.DeclaredProperties.Where<EdmProperty>((Func<EdmProperty, bool>) (property => property.TypeUsage != null)).Any<EdmProperty>((Func<EdmProperty, bool>) (property =>
      {
        if (property.PrimitiveType != null)
          return property.ConcurrencyMode != ConcurrencyMode.None;
        return false;
      }));
    }

    private static bool TypeIsSubTypeOf(
      EntityType entityType,
      Dictionary<EntityType, EntitySet> baseEntitySetTypes,
      out EntitySet set)
    {
      if (entityType.IsTypeHierarchyRoot())
      {
        set = (EntitySet) null;
        return false;
      }
      foreach (EntityType key in entityType.ToHierarchy())
      {
        if (baseEntitySetTypes.ContainsKey(key))
        {
          set = baseEntitySetTypes[key];
          return true;
        }
      }
      set = (EntitySet) null;
      return false;
    }

    private static bool IsTypeHierarchyRoot(this EntityType entityType)
    {
      return entityType.BaseType == null;
    }

    private static bool IsForeignKey(this AssociationType association, double version)
    {
      return version >= 2.0 && association.Constraint != null;
    }
  }
}
