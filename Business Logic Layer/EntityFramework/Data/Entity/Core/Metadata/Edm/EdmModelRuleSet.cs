// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.EdmModelRuleSet
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal abstract class EdmModelRuleSet : DataModelValidationRuleSet
  {
    public static EdmModelRuleSet CreateEdmModelRuleSet(
      double version,
      bool validateSyntax)
    {
      if (object.Equals((object) version, (object) 1.0))
        return (EdmModelRuleSet) new EdmModelRuleSet.V1RuleSet(validateSyntax);
      if (object.Equals((object) version, (object) 1.1))
        return (EdmModelRuleSet) new EdmModelRuleSet.V1_1RuleSet(validateSyntax);
      if (object.Equals((object) version, (object) 2.0))
        return (EdmModelRuleSet) new EdmModelRuleSet.V2RuleSet(validateSyntax);
      if (object.Equals((object) version, (object) 3.0))
        return (EdmModelRuleSet) new EdmModelRuleSet.V3RuleSet(validateSyntax);
      return (EdmModelRuleSet) null;
    }

    private EdmModelRuleSet(bool validateSyntax)
    {
      if (validateSyntax)
      {
        this.AddRule((DataModelValidationRule) EdmModelSyntacticValidationRules.EdmAssociationConstraint_DependentEndMustNotBeNull);
        this.AddRule((DataModelValidationRule) EdmModelSyntacticValidationRules.EdmAssociationConstraint_DependentPropertiesMustNotBeEmpty);
        this.AddRule((DataModelValidationRule) EdmModelSyntacticValidationRules.EdmAssociationEnd_EntityTypeMustNotBeNull);
        this.AddRule((DataModelValidationRule) EdmModelSyntacticValidationRules.EdmAssociationSet_ElementTypeMustNotBeNull);
        this.AddRule((DataModelValidationRule) EdmModelSyntacticValidationRules.EdmAssociationSet_SourceSetMustNotBeNull);
        this.AddRule((DataModelValidationRule) EdmModelSyntacticValidationRules.EdmAssociationSet_TargetSetMustNotBeNull);
        this.AddRule((DataModelValidationRule) EdmModelSyntacticValidationRules.EdmAssociationType_AssocationEndMustNotBeNull);
        this.AddRule((DataModelValidationRule) EdmModelSyntacticValidationRules.EdmEntitySet_ElementTypeMustNotBeNull);
        this.AddRule((DataModelValidationRule) EdmModelSyntacticValidationRules.EdmModel_NameMustNotBeEmptyOrWhiteSpace);
        this.AddRule((DataModelValidationRule) EdmModelSyntacticValidationRules.EdmModel_NameIsTooLong);
        this.AddRule((DataModelValidationRule) EdmModelSyntacticValidationRules.EdmModel_NameIsNotAllowed);
        this.AddRule((DataModelValidationRule) EdmModelSyntacticValidationRules.EdmNavigationProperty_AssocationMustNotBeNull);
        this.AddRule((DataModelValidationRule) EdmModelSyntacticValidationRules.EdmNavigationProperty_ResultEndMustNotBeNull);
        this.AddRule((DataModelValidationRule) EdmModelSyntacticValidationRules.EdmTypeReference_TypeNotValid);
      }
      this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmType_SystemNamespaceEncountered);
      this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmEntityContainer_SimilarRelationshipEnd);
      this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmEntityContainer_InvalidEntitySetNameReference);
      this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmEntityContainer_ConcurrencyRedefinedOnSubTypeOfEntitySetType);
      this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmEntityContainer_DuplicateEntityContainerMemberName);
      this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmEntityContainer_DuplicateEntitySetTable);
      this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmEntitySet_EntitySetTypeHasNoKeys);
      this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmAssociationSet_DuplicateEndName);
      this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmEntityType_EntityKeyMustBeScalar);
      this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmEntityType_DuplicatePropertyNameSpecifiedInEntityKey);
      this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmEntityType_InvalidKeyNullablePart);
      this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmEntityType_InvalidKeyKeyDefinedInBaseClass);
      this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmEntityType_KeyMissingOnEntityType);
      this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmEntityType_InvalidMemberNameMatchesTypeName);
      this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmEntityType_PropertyNameAlreadyDefinedDuplicate);
      this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmEntityType_CycleInTypeHierarchy);
      this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmNavigationProperty_BadNavigationPropertyUndefinedRole);
      this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmNavigationProperty_BadNavigationPropertyRolesCannotBeTheSame);
      this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmNavigationProperty_BadNavigationPropertyBadFromRoleType);
      this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmAssociationType_InvalidOperationMultipleEndsInAssociation);
      this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmAssociationType_EndWithManyMultiplicityCannotHaveOperationsSpecified);
      this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmAssociationType_EndNameAlreadyDefinedDuplicate);
      this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmAssociationType_InvalidPropertyInRelationshipConstraint);
      this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmAssociationType_SameRoleReferredInReferentialConstraint);
      this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmAssociationType_ValidateReferentialConstraint);
      this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmComplexType_InvalidMemberNameMatchesTypeName);
      this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmNamespace_TypeNameAlreadyDefinedDuplicate);
      this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmFunction_DuplicateParameterName);
    }

    private abstract class NonV1_1RuleSet : EdmModelRuleSet
    {
      protected NonV1_1RuleSet(bool validateSyntax)
        : base(validateSyntax)
      {
        this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmProperty_NullableComplexType);
        this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmProperty_InvalidCollectionKind);
        this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmComplexType_PropertyNameAlreadyDefinedDuplicate);
        this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmComplexType_InvalidIsAbstract);
        this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmComplexType_InvalidIsPolymorphic);
        this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmFunction_ComposableFunctionImportsNotAllowed_V1_V2);
      }
    }

    private sealed class V1RuleSet : EdmModelRuleSet.NonV1_1RuleSet
    {
      internal V1RuleSet(bool validateSyntax)
        : base(validateSyntax)
      {
        this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmProperty_InvalidPropertyType);
      }
    }

    private sealed class V1_1RuleSet : EdmModelRuleSet
    {
      internal V1_1RuleSet(bool validateSyntax)
        : base(validateSyntax)
      {
        this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmComplexType_PropertyNameAlreadyDefinedDuplicate_V1_1);
        this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmComplexType_CycleInTypeHierarchy_V1_1);
        this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmProperty_InvalidCollectionKind_V1_1);
        this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmProperty_InvalidPropertyType_V1_1);
      }
    }

    private class V2RuleSet : EdmModelRuleSet.NonV1_1RuleSet
    {
      internal V2RuleSet(bool validateSyntax)
        : base(validateSyntax)
      {
        this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmProperty_InvalidPropertyType);
      }
    }

    private sealed class V3RuleSet : EdmModelRuleSet.V2RuleSet
    {
      internal V3RuleSet(bool validateSyntax)
        : base(validateSyntax)
      {
        this.RemoveRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmProperty_InvalidPropertyType);
        this.AddRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmProperty_InvalidPropertyType_V3);
        this.RemoveRule((DataModelValidationRule) EdmModelSemanticValidationRules.EdmFunction_ComposableFunctionImportsNotAllowed_V1_V2);
      }
    }
  }
}
