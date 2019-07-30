// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Edm.EdmModelVisitor
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;

namespace System.Data.Entity.Edm
{
  internal abstract class EdmModelVisitor
  {
    protected static void VisitCollection<T>(IEnumerable<T> collection, Action<T> visitMethod)
    {
      if (collection == null)
        return;
      foreach (T obj in collection)
        visitMethod(obj);
    }

    protected internal virtual void VisitEdmModel(EdmModel item)
    {
      if (item == null)
        return;
      this.VisitComplexTypes(item.ComplexTypes);
      this.VisitEntityTypes(item.EntityTypes);
      this.VisitEnumTypes(item.EnumTypes);
      this.VisitAssociationTypes(item.AssociationTypes);
      this.VisitFunctions(item.Functions);
      this.VisitEntityContainers(item.Containers);
    }

    protected virtual void VisitAnnotations(
      MetadataItem item,
      IEnumerable<MetadataProperty> annotations)
    {
      EdmModelVisitor.VisitCollection<MetadataProperty>(annotations, new Action<MetadataProperty>(this.VisitAnnotation));
    }

    protected virtual void VisitAnnotation(MetadataProperty item)
    {
    }

    protected internal virtual void VisitMetadataItem(MetadataItem item)
    {
      if (item == null || !item.Annotations.Any<MetadataProperty>())
        return;
      this.VisitAnnotations(item, item.Annotations);
    }

    protected virtual void VisitEntityContainers(IEnumerable<EntityContainer> entityContainers)
    {
      EdmModelVisitor.VisitCollection<EntityContainer>(entityContainers, new Action<EntityContainer>(this.VisitEdmEntityContainer));
    }

    protected virtual void VisitEdmEntityContainer(EntityContainer item)
    {
      this.VisitMetadataItem((MetadataItem) item);
      if (item == null)
        return;
      if (item.EntitySets.Count > 0)
        this.VisitEntitySets(item, (IEnumerable<EntitySet>) item.EntitySets);
      if (item.AssociationSets.Count > 0)
        this.VisitAssociationSets(item, (IEnumerable<AssociationSet>) item.AssociationSets);
      if (item.FunctionImports.Count <= 0)
        return;
      this.VisitFunctionImports(item, (IEnumerable<EdmFunction>) item.FunctionImports);
    }

    protected internal virtual void VisitEdmFunction(EdmFunction function)
    {
      this.VisitMetadataItem((MetadataItem) function);
      if (function == null)
        return;
      if (function.Parameters != null)
        this.VisitFunctionParameters((IEnumerable<FunctionParameter>) function.Parameters);
      if (function.ReturnParameters == null)
        return;
      this.VisitFunctionReturnParameters((IEnumerable<FunctionParameter>) function.ReturnParameters);
    }

    protected virtual void VisitEntitySets(
      EntityContainer container,
      IEnumerable<EntitySet> entitySets)
    {
      EdmModelVisitor.VisitCollection<EntitySet>(entitySets, new Action<EntitySet>(this.VisitEdmEntitySet));
    }

    protected internal virtual void VisitEdmEntitySet(EntitySet item)
    {
      this.VisitMetadataItem((MetadataItem) item);
    }

    protected virtual void VisitAssociationSets(
      EntityContainer container,
      IEnumerable<AssociationSet> associationSets)
    {
      EdmModelVisitor.VisitCollection<AssociationSet>(associationSets, new Action<AssociationSet>(this.VisitEdmAssociationSet));
    }

    protected virtual void VisitEdmAssociationSet(AssociationSet item)
    {
      this.VisitMetadataItem((MetadataItem) item);
      if (item.SourceSet != null)
        this.VisitEdmAssociationSetEnd(item.SourceSet);
      if (item.TargetSet == null)
        return;
      this.VisitEdmAssociationSetEnd(item.TargetSet);
    }

    protected virtual void VisitEdmAssociationSetEnd(EntitySet item)
    {
      this.VisitMetadataItem((MetadataItem) item);
    }

    protected internal virtual void VisitFunctionImports(
      EntityContainer container,
      IEnumerable<EdmFunction> functionImports)
    {
      EdmModelVisitor.VisitCollection<EdmFunction>(functionImports, new Action<EdmFunction>(this.VisitFunctionImport));
    }

    protected internal virtual void VisitFunctionImport(EdmFunction functionImport)
    {
      this.VisitMetadataItem((MetadataItem) functionImport);
      if (functionImport.Parameters != null)
        this.VisitFunctionImportParameters((IEnumerable<FunctionParameter>) functionImport.Parameters);
      if (functionImport.ReturnParameters == null)
        return;
      this.VisitFunctionImportReturnParameters((IEnumerable<FunctionParameter>) functionImport.ReturnParameters);
    }

    protected internal virtual void VisitFunctionImportParameters(
      IEnumerable<FunctionParameter> parameters)
    {
      EdmModelVisitor.VisitCollection<FunctionParameter>(parameters, new Action<FunctionParameter>(this.VisitFunctionImportParameter));
    }

    protected internal virtual void VisitFunctionImportParameter(FunctionParameter parameter)
    {
      this.VisitMetadataItem((MetadataItem) parameter);
    }

    protected internal virtual void VisitFunctionImportReturnParameters(
      IEnumerable<FunctionParameter> parameters)
    {
      EdmModelVisitor.VisitCollection<FunctionParameter>(parameters, new Action<FunctionParameter>(this.VisitFunctionImportReturnParameter));
    }

    protected internal virtual void VisitFunctionImportReturnParameter(FunctionParameter parameter)
    {
      this.VisitMetadataItem((MetadataItem) parameter);
    }

    protected virtual void VisitComplexTypes(IEnumerable<ComplexType> complexTypes)
    {
      EdmModelVisitor.VisitCollection<ComplexType>(complexTypes, new Action<ComplexType>(this.VisitComplexType));
    }

    protected virtual void VisitComplexType(ComplexType item)
    {
      this.VisitMetadataItem((MetadataItem) item);
      if (item.Properties.Count <= 0)
        return;
      EdmModelVisitor.VisitCollection<EdmProperty>((IEnumerable<EdmProperty>) item.Properties, new Action<EdmProperty>(this.VisitEdmProperty));
    }

    protected virtual void VisitDeclaredProperties(
      ComplexType complexType,
      IEnumerable<EdmProperty> properties)
    {
      EdmModelVisitor.VisitCollection<EdmProperty>(properties, new Action<EdmProperty>(this.VisitEdmProperty));
    }

    protected virtual void VisitEntityTypes(IEnumerable<EntityType> entityTypes)
    {
      EdmModelVisitor.VisitCollection<EntityType>(entityTypes, new Action<EntityType>(this.VisitEdmEntityType));
    }

    protected virtual void VisitEnumTypes(IEnumerable<EnumType> enumTypes)
    {
      EdmModelVisitor.VisitCollection<EnumType>(enumTypes, new Action<EnumType>(this.VisitEdmEnumType));
    }

    protected internal virtual void VisitFunctions(IEnumerable<EdmFunction> functions)
    {
      EdmModelVisitor.VisitCollection<EdmFunction>(functions, new Action<EdmFunction>(this.VisitEdmFunction));
    }

    protected virtual void VisitFunctionParameters(IEnumerable<FunctionParameter> parameters)
    {
      EdmModelVisitor.VisitCollection<FunctionParameter>(parameters, new Action<FunctionParameter>(this.VisitFunctionParameter));
    }

    protected internal virtual void VisitFunctionParameter(FunctionParameter functionParameter)
    {
      this.VisitMetadataItem((MetadataItem) functionParameter);
    }

    protected internal virtual void VisitFunctionReturnParameters(
      IEnumerable<FunctionParameter> returnParameters)
    {
      EdmModelVisitor.VisitCollection<FunctionParameter>(returnParameters, new Action<FunctionParameter>(this.VisitFunctionReturnParameter));
    }

    protected internal virtual void VisitFunctionReturnParameter(FunctionParameter returnParameter)
    {
      this.VisitMetadataItem((MetadataItem) returnParameter);
      this.VisitEdmType(returnParameter.TypeUsage.EdmType);
    }

    protected internal virtual void VisitEdmType(EdmType edmType)
    {
      switch (edmType.BuiltInTypeKind)
      {
        case BuiltInTypeKind.CollectionType:
          this.VisitCollectionType((CollectionType) edmType);
          break;
        case BuiltInTypeKind.PrimitiveType:
          this.VisitPrimitiveType((PrimitiveType) edmType);
          break;
        case BuiltInTypeKind.RowType:
          this.VisitRowType((RowType) edmType);
          break;
      }
    }

    protected internal virtual void VisitCollectionType(CollectionType collectionType)
    {
      this.VisitMetadataItem((MetadataItem) collectionType);
      this.VisitEdmType(collectionType.TypeUsage.EdmType);
    }

    protected internal virtual void VisitRowType(RowType rowType)
    {
      this.VisitMetadataItem((MetadataItem) rowType);
      if (rowType.DeclaredProperties.Count <= 0)
        return;
      EdmModelVisitor.VisitCollection<EdmProperty>((IEnumerable<EdmProperty>) rowType.DeclaredProperties, new Action<EdmProperty>(this.VisitEdmProperty));
    }

    protected internal virtual void VisitPrimitiveType(PrimitiveType primitiveType)
    {
      this.VisitMetadataItem((MetadataItem) primitiveType);
    }

    protected virtual void VisitEdmEnumType(EnumType item)
    {
      this.VisitMetadataItem((MetadataItem) item);
      if (item == null || item.Members.Count <= 0)
        return;
      this.VisitEnumMembers(item, (IEnumerable<EnumMember>) item.Members);
    }

    protected virtual void VisitEnumMembers(EnumType enumType, IEnumerable<EnumMember> members)
    {
      EdmModelVisitor.VisitCollection<EnumMember>(members, new Action<EnumMember>(this.VisitEdmEnumTypeMember));
    }

    protected internal virtual void VisitEdmEntityType(EntityType item)
    {
      this.VisitMetadataItem((MetadataItem) item);
      if (item == null)
        return;
      if (item.BaseType == null && item.KeyProperties.Count > 0)
        this.VisitKeyProperties(item, (IList<EdmProperty>) item.KeyProperties);
      if (item.DeclaredProperties.Count > 0)
        this.VisitDeclaredProperties(item, (IList<EdmProperty>) item.DeclaredProperties);
      if (item.DeclaredNavigationProperties.Count <= 0)
        return;
      this.VisitDeclaredNavigationProperties(item, (IEnumerable<NavigationProperty>) item.DeclaredNavigationProperties);
    }

    protected virtual void VisitKeyProperties(EntityType entityType, IList<EdmProperty> properties)
    {
      EdmModelVisitor.VisitCollection<EdmProperty>((IEnumerable<EdmProperty>) properties, new Action<EdmProperty>(this.VisitEdmProperty));
    }

    protected virtual void VisitDeclaredProperties(
      EntityType entityType,
      IList<EdmProperty> properties)
    {
      EdmModelVisitor.VisitCollection<EdmProperty>((IEnumerable<EdmProperty>) properties, new Action<EdmProperty>(this.VisitEdmProperty));
    }

    protected virtual void VisitDeclaredNavigationProperties(
      EntityType entityType,
      IEnumerable<NavigationProperty> navigationProperties)
    {
      EdmModelVisitor.VisitCollection<NavigationProperty>(navigationProperties, new Action<NavigationProperty>(this.VisitEdmNavigationProperty));
    }

    protected virtual void VisitAssociationTypes(IEnumerable<AssociationType> associationTypes)
    {
      EdmModelVisitor.VisitCollection<AssociationType>(associationTypes, new Action<AssociationType>(this.VisitEdmAssociationType));
    }

    protected internal virtual void VisitEdmAssociationType(AssociationType item)
    {
      this.VisitMetadataItem((MetadataItem) item);
      if (item != null)
      {
        if (item.SourceEnd != null)
          this.VisitEdmAssociationEnd((RelationshipEndMember) item.SourceEnd);
        if (item.TargetEnd != null)
          this.VisitEdmAssociationEnd((RelationshipEndMember) item.TargetEnd);
      }
      if (item.Constraint == null)
        return;
      this.VisitEdmAssociationConstraint(item.Constraint);
    }

    protected internal virtual void VisitEdmProperty(EdmProperty item)
    {
      this.VisitMetadataItem((MetadataItem) item);
    }

    protected virtual void VisitEdmEnumTypeMember(EnumMember item)
    {
      this.VisitMetadataItem((MetadataItem) item);
    }

    protected virtual void VisitEdmAssociationEnd(RelationshipEndMember item)
    {
      this.VisitMetadataItem((MetadataItem) item);
    }

    protected virtual void VisitEdmAssociationConstraint(ReferentialConstraint item)
    {
      if (item == null)
        return;
      this.VisitMetadataItem((MetadataItem) item);
      if (item.ToRole != null)
        this.VisitEdmAssociationEnd(item.ToRole);
      EdmModelVisitor.VisitCollection<EdmProperty>((IEnumerable<EdmProperty>) item.ToProperties, new Action<EdmProperty>(this.VisitEdmProperty));
    }

    protected virtual void VisitEdmNavigationProperty(NavigationProperty item)
    {
      this.VisitMetadataItem((MetadataItem) item);
    }
  }
}
