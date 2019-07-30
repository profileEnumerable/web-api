// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.MslXmlSchemaWriter
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Linq;
using System.Xml;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal class MslXmlSchemaWriter : XmlSchemaWriter
  {
    private string _entityTypeNamespace;
    private string _dbSchemaName;

    internal MslXmlSchemaWriter(XmlWriter xmlWriter, double version)
    {
      this._xmlWriter = xmlWriter;
      this._version = version;
    }

    internal void WriteSchema(DbDatabaseMapping databaseMapping)
    {
      this.WriteSchemaElementHeader();
      this.WriteDbModelElement(databaseMapping);
      this.WriteEndElement();
    }

    private void WriteSchemaElementHeader()
    {
      this._xmlWriter.WriteStartElement("Mapping", MslConstructs.GetMslNamespace(this._version));
      this._xmlWriter.WriteAttributeString("Space", "C-S");
    }

    private void WriteDbModelElement(DbDatabaseMapping databaseMapping)
    {
      this._entityTypeNamespace = databaseMapping.Model.NamespaceNames.SingleOrDefault<string>();
      this._dbSchemaName = databaseMapping.Database.Containers.Single<EntityContainer>().Name;
      this.WriteEntityContainerMappingElement(databaseMapping.EntityContainerMappings.First<EntityContainerMapping>());
    }

    internal void WriteEntityContainerMappingElement(EntityContainerMapping containerMapping)
    {
      this._xmlWriter.WriteStartElement("EntityContainerMapping");
      this._xmlWriter.WriteAttributeString("StorageEntityContainer", this._dbSchemaName);
      this._xmlWriter.WriteAttributeString("CdmEntityContainer", containerMapping.EdmEntityContainer.Name);
      foreach (EntitySetMapping entitySetMapping in containerMapping.EntitySetMappings)
        this.WriteEntitySetMappingElement(entitySetMapping);
      foreach (AssociationSetMapping associationSetMapping in containerMapping.AssociationSetMappings)
        this.WriteAssociationSetMappingElement(associationSetMapping);
      foreach (FunctionImportMappingComposable functionImportMapping in containerMapping.FunctionImportMappings.OfType<FunctionImportMappingComposable>())
        this.WriteFunctionImportMappingElement(functionImportMapping);
      foreach (FunctionImportMappingNonComposable functionImportMapping in containerMapping.FunctionImportMappings.OfType<FunctionImportMappingNonComposable>())
        this.WriteFunctionImportMappingElement(functionImportMapping);
      this._xmlWriter.WriteEndElement();
    }

    public void WriteEntitySetMappingElement(EntitySetMapping entitySetMapping)
    {
      this._xmlWriter.WriteStartElement("EntitySetMapping");
      this._xmlWriter.WriteAttributeString("Name", entitySetMapping.EntitySet.Name);
      foreach (EntityTypeMapping entityTypeMapping in entitySetMapping.EntityTypeMappings)
        this.WriteEntityTypeMappingElement(entityTypeMapping);
      foreach (EntityTypeModificationFunctionMapping modificationFunctionMapping in entitySetMapping.ModificationFunctionMappings)
      {
        this._xmlWriter.WriteStartElement("EntityTypeMapping");
        this._xmlWriter.WriteAttributeString("TypeName", MslXmlSchemaWriter.GetEntityTypeName(this._entityTypeNamespace + "." + modificationFunctionMapping.EntityType.Name, false));
        this.WriteModificationFunctionMapping(modificationFunctionMapping);
        this._xmlWriter.WriteEndElement();
      }
      this._xmlWriter.WriteEndElement();
    }

    public void WriteAssociationSetMappingElement(AssociationSetMapping associationSetMapping)
    {
      this._xmlWriter.WriteStartElement("AssociationSetMapping");
      this._xmlWriter.WriteAttributeString("Name", associationSetMapping.AssociationSet.Name);
      this._xmlWriter.WriteAttributeString("TypeName", this._entityTypeNamespace + "." + associationSetMapping.AssociationSet.ElementType.Name);
      this._xmlWriter.WriteAttributeString("StoreEntitySet", associationSetMapping.Table.Name);
      this.WriteAssociationEndMappingElement(associationSetMapping.SourceEndMapping);
      this.WriteAssociationEndMappingElement(associationSetMapping.TargetEndMapping);
      if (associationSetMapping.ModificationFunctionMapping != null)
        this.WriteModificationFunctionMapping(associationSetMapping.ModificationFunctionMapping);
      foreach (ConditionPropertyMapping condition in associationSetMapping.Conditions)
        this.WriteConditionElement(condition);
      this._xmlWriter.WriteEndElement();
    }

    private void WriteAssociationEndMappingElement(EndPropertyMapping endMapping)
    {
      this._xmlWriter.WriteStartElement("EndProperty");
      this._xmlWriter.WriteAttributeString("Name", endMapping.AssociationEnd.Name);
      foreach (ScalarPropertyMapping propertyMapping in endMapping.PropertyMappings)
        this.WriteScalarPropertyElement(propertyMapping.Property.Name, propertyMapping.Column.Name);
      this._xmlWriter.WriteEndElement();
    }

    private void WriteEntityTypeMappingElement(EntityTypeMapping entityTypeMapping)
    {
      this._xmlWriter.WriteStartElement("EntityTypeMapping");
      this._xmlWriter.WriteAttributeString("TypeName", MslXmlSchemaWriter.GetEntityTypeName(this._entityTypeNamespace + "." + entityTypeMapping.EntityType.Name, entityTypeMapping.IsHierarchyMapping));
      foreach (MappingFragment mappingFragment in entityTypeMapping.MappingFragments)
        this.WriteMappingFragmentElement(mappingFragment);
      this._xmlWriter.WriteEndElement();
    }

    internal void WriteMappingFragmentElement(MappingFragment mappingFragment)
    {
      this._xmlWriter.WriteStartElement("MappingFragment");
      this._xmlWriter.WriteAttributeString("StoreEntitySet", mappingFragment.TableSet.Name);
      foreach (PropertyMapping propertyMapping in mappingFragment.PropertyMappings)
        this.WritePropertyMapping(propertyMapping);
      foreach (ConditionPropertyMapping columnCondition in mappingFragment.ColumnConditions)
        this.WriteConditionElement(columnCondition);
      this._xmlWriter.WriteEndElement();
    }

    public void WriteFunctionImportMappingElement(
      FunctionImportMappingComposable functionImportMapping)
    {
      this.WriteFunctionImportMappingStartElement((FunctionImportMapping) functionImportMapping);
      if (functionImportMapping.StructuralTypeMappings != null)
      {
        this._xmlWriter.WriteStartElement("ResultMapping");
        Tuple<StructuralType, List<ConditionPropertyMapping>, List<PropertyMapping>> tuple = functionImportMapping.StructuralTypeMappings.Single<Tuple<StructuralType, List<ConditionPropertyMapping>, List<PropertyMapping>>>();
        if (tuple.Item1.BuiltInTypeKind == BuiltInTypeKind.ComplexType)
        {
          this._xmlWriter.WriteStartElement("ComplexTypeMapping");
          this._xmlWriter.WriteAttributeString("TypeName", tuple.Item1.FullName);
        }
        else
        {
          this._xmlWriter.WriteStartElement("EntityTypeMapping");
          this._xmlWriter.WriteAttributeString("TypeName", tuple.Item1.FullName);
          foreach (ConditionPropertyMapping condition in tuple.Item2)
            this.WriteConditionElement(condition);
        }
        foreach (PropertyMapping propertyMapping in tuple.Item3)
          this.WritePropertyMapping(propertyMapping);
        this._xmlWriter.WriteEndElement();
        this._xmlWriter.WriteEndElement();
      }
      this.WriteFunctionImportEndElement();
    }

    public void WriteFunctionImportMappingElement(
      FunctionImportMappingNonComposable functionImportMapping)
    {
      this.WriteFunctionImportMappingStartElement((FunctionImportMapping) functionImportMapping);
      foreach (FunctionImportResultMapping resultMapping in functionImportMapping.ResultMappings)
        this.WriteFunctionImportResultMappingElement(resultMapping);
      this.WriteFunctionImportEndElement();
    }

    private void WriteFunctionImportMappingStartElement(FunctionImportMapping functionImportMapping)
    {
      this._xmlWriter.WriteStartElement("FunctionImportMapping");
      this._xmlWriter.WriteAttributeString("FunctionName", functionImportMapping.TargetFunction.FullName);
      this._xmlWriter.WriteAttributeString("FunctionImportName", functionImportMapping.FunctionImport.Name);
    }

    private void WriteFunctionImportResultMappingElement(FunctionImportResultMapping resultMapping)
    {
      this._xmlWriter.WriteStartElement("ResultMapping");
      foreach (FunctionImportStructuralTypeMapping typeMapping in resultMapping.TypeMappings)
      {
        FunctionImportEntityTypeMapping entityTypeMapping = typeMapping as FunctionImportEntityTypeMapping;
        if (entityTypeMapping != null)
          this.WriteFunctionImportEntityTypeMappingElement(entityTypeMapping);
        else
          this.WriteFunctionImportComplexTypeMappingElement((FunctionImportComplexTypeMapping) typeMapping);
      }
      this._xmlWriter.WriteEndElement();
    }

    private void WriteFunctionImportEntityTypeMappingElement(
      FunctionImportEntityTypeMapping entityTypeMapping)
    {
      this._xmlWriter.WriteStartElement("EntityTypeMapping");
      this._xmlWriter.WriteAttributeString("TypeName", MslXmlSchemaWriter.CreateFunctionImportEntityTypeMappingTypeName(entityTypeMapping));
      this.WriteFunctionImportPropertyMappingElements(entityTypeMapping.PropertyMappings.Cast<FunctionImportReturnTypeScalarPropertyMapping>());
      foreach (FunctionImportEntityTypeMappingCondition condition in entityTypeMapping.Conditions)
        this.WriteFunctionImportConditionElement(condition);
      this._xmlWriter.WriteEndElement();
    }

    internal static string CreateFunctionImportEntityTypeMappingTypeName(
      FunctionImportEntityTypeMapping entityTypeMapping)
    {
      return string.Join(";", entityTypeMapping.EntityTypes.Select<EntityType, string>((Func<EntityType, string>) (e => MslXmlSchemaWriter.GetEntityTypeName(e.FullName, false))).Concat<string>(entityTypeMapping.IsOfTypeEntityTypes.Select<EntityType, string>((Func<EntityType, string>) (e => MslXmlSchemaWriter.GetEntityTypeName(e.FullName, true)))));
    }

    private void WriteFunctionImportComplexTypeMappingElement(
      FunctionImportComplexTypeMapping complexTypeMapping)
    {
      this._xmlWriter.WriteStartElement("ComplexTypeMapping");
      this._xmlWriter.WriteAttributeString("TypeName", complexTypeMapping.ReturnType.FullName);
      this.WriteFunctionImportPropertyMappingElements(complexTypeMapping.PropertyMappings.Cast<FunctionImportReturnTypeScalarPropertyMapping>());
      this._xmlWriter.WriteEndElement();
    }

    private void WriteFunctionImportPropertyMappingElements(
      IEnumerable<FunctionImportReturnTypeScalarPropertyMapping> propertyMappings)
    {
      foreach (FunctionImportReturnTypeScalarPropertyMapping propertyMapping in propertyMappings)
        this.WriteScalarPropertyElement(propertyMapping.PropertyName, propertyMapping.ColumnName);
    }

    private void WriteFunctionImportConditionElement(
      FunctionImportEntityTypeMappingCondition condition)
    {
      this._xmlWriter.WriteStartElement("Condition");
      this._xmlWriter.WriteAttributeString("ColumnName", condition.ColumnName);
      FunctionImportEntityTypeMappingConditionIsNull mappingConditionIsNull = condition as FunctionImportEntityTypeMappingConditionIsNull;
      if (mappingConditionIsNull != null)
        this.WriteIsNullConditionAttribute(mappingConditionIsNull.IsNull);
      else
        this.WriteConditionValue(((FunctionImportEntityTypeMappingConditionValue) condition).Value);
      this._xmlWriter.WriteEndElement();
    }

    private void WriteFunctionImportEndElement()
    {
      this._xmlWriter.WriteEndElement();
    }

    private void WriteModificationFunctionMapping(
      EntityTypeModificationFunctionMapping modificationFunctionMapping)
    {
      this._xmlWriter.WriteStartElement("ModificationFunctionMapping");
      this.WriteFunctionMapping("InsertFunction", modificationFunctionMapping.InsertFunctionMapping, false);
      this.WriteFunctionMapping("UpdateFunction", modificationFunctionMapping.UpdateFunctionMapping, false);
      this.WriteFunctionMapping("DeleteFunction", modificationFunctionMapping.DeleteFunctionMapping, false);
      this._xmlWriter.WriteEndElement();
    }

    private void WriteModificationFunctionMapping(
      AssociationSetModificationFunctionMapping modificationFunctionMapping)
    {
      this._xmlWriter.WriteStartElement("ModificationFunctionMapping");
      this.WriteFunctionMapping("InsertFunction", modificationFunctionMapping.InsertFunctionMapping, true);
      this.WriteFunctionMapping("DeleteFunction", modificationFunctionMapping.DeleteFunctionMapping, true);
      this._xmlWriter.WriteEndElement();
    }

    public void WriteFunctionMapping(
      string functionElement,
      ModificationFunctionMapping functionMapping,
      bool associationSetMapping = false)
    {
      this._xmlWriter.WriteStartElement(functionElement);
      this._xmlWriter.WriteAttributeString("FunctionName", functionMapping.Function.FullName);
      if (functionMapping.RowsAffectedParameter != null)
        this._xmlWriter.WriteAttributeString("RowsAffectedParameter", functionMapping.RowsAffectedParameter.Name);
      if (!associationSetMapping)
      {
        this.WritePropertyParameterBindings((IEnumerable<ModificationFunctionParameterBinding>) functionMapping.ParameterBindings, 0);
        this.WriteAssociationParameterBindings((IEnumerable<ModificationFunctionParameterBinding>) functionMapping.ParameterBindings);
        if (functionMapping.ResultBindings != null)
          this.WriteResultBindings((IEnumerable<ModificationFunctionResultBinding>) functionMapping.ResultBindings);
      }
      else
        this.WriteAssociationSetMappingParameterBindings((IEnumerable<ModificationFunctionParameterBinding>) functionMapping.ParameterBindings);
      this._xmlWriter.WriteEndElement();
    }

    private void WriteAssociationSetMappingParameterBindings(
      IEnumerable<ModificationFunctionParameterBinding> parameterBindings)
    {
      foreach (IGrouping<AssociationSetEnd, ModificationFunctionParameterBinding> grouping in parameterBindings.Where<ModificationFunctionParameterBinding>((Func<ModificationFunctionParameterBinding, bool>) (pm => pm.MemberPath.AssociationSetEnd != null)).GroupBy<ModificationFunctionParameterBinding, AssociationSetEnd>((Func<ModificationFunctionParameterBinding, AssociationSetEnd>) (pm => pm.MemberPath.AssociationSetEnd)))
      {
        this._xmlWriter.WriteStartElement("EndProperty");
        this._xmlWriter.WriteAttributeString("Name", grouping.Key.Name);
        foreach (ModificationFunctionParameterBinding parameterBinding in (IEnumerable<ModificationFunctionParameterBinding>) grouping)
          this.WriteScalarParameterElement(parameterBinding.MemberPath.Members.First<EdmMember>(), parameterBinding);
        this._xmlWriter.WriteEndElement();
      }
    }

    private void WritePropertyParameterBindings(
      IEnumerable<ModificationFunctionParameterBinding> parameterBindings,
      int level = 0)
    {
      foreach (IGrouping<EdmMember, ModificationFunctionParameterBinding> grouping in parameterBindings.Where<ModificationFunctionParameterBinding>((Func<ModificationFunctionParameterBinding, bool>) (pm =>
      {
        if (pm.MemberPath.AssociationSetEnd == null)
          return pm.MemberPath.Members.Count<EdmMember>() > level;
        return false;
      })).GroupBy<ModificationFunctionParameterBinding, EdmMember>((Func<ModificationFunctionParameterBinding, EdmMember>) (pm => pm.MemberPath.Members.ElementAt<EdmMember>(level))))
      {
        EdmProperty key = (EdmProperty) grouping.Key;
        if (key.IsComplexType)
        {
          this._xmlWriter.WriteStartElement("ComplexProperty");
          this._xmlWriter.WriteAttributeString("Name", key.Name);
          this._xmlWriter.WriteAttributeString("TypeName", this._entityTypeNamespace + "." + key.ComplexType.Name);
          this.WritePropertyParameterBindings((IEnumerable<ModificationFunctionParameterBinding>) grouping, level + 1);
          this._xmlWriter.WriteEndElement();
        }
        else
        {
          foreach (ModificationFunctionParameterBinding parameterBinding in (IEnumerable<ModificationFunctionParameterBinding>) grouping)
            this.WriteScalarParameterElement((EdmMember) key, parameterBinding);
        }
      }
    }

    private void WriteAssociationParameterBindings(
      IEnumerable<ModificationFunctionParameterBinding> parameterBindings)
    {
      foreach (IGrouping<AssociationSetEnd, ModificationFunctionParameterBinding> grouping in parameterBindings.Where<ModificationFunctionParameterBinding>((Func<ModificationFunctionParameterBinding, bool>) (pm => pm.MemberPath.AssociationSetEnd != null)).GroupBy<ModificationFunctionParameterBinding, AssociationSetEnd>((Func<ModificationFunctionParameterBinding, AssociationSetEnd>) (pm => pm.MemberPath.AssociationSetEnd)))
      {
        IGrouping<AssociationSetEnd, ModificationFunctionParameterBinding> group = grouping;
        this._xmlWriter.WriteStartElement("AssociationEnd");
        AssociationSet parentAssociationSet = group.Key.ParentAssociationSet;
        this._xmlWriter.WriteAttributeString("AssociationSet", parentAssociationSet.Name);
        this._xmlWriter.WriteAttributeString("From", group.Key.Name);
        this._xmlWriter.WriteAttributeString("To", parentAssociationSet.AssociationSetEnds.Single<AssociationSetEnd>((Func<AssociationSetEnd, bool>) (ae => ae != group.Key)).Name);
        foreach (ModificationFunctionParameterBinding parameterBinding in (IEnumerable<ModificationFunctionParameterBinding>) group)
          this.WriteScalarParameterElement(parameterBinding.MemberPath.Members.First<EdmMember>(), parameterBinding);
        this._xmlWriter.WriteEndElement();
      }
    }

    private void WriteResultBindings(
      IEnumerable<ModificationFunctionResultBinding> resultBindings)
    {
      foreach (ModificationFunctionResultBinding resultBinding in resultBindings)
      {
        this._xmlWriter.WriteStartElement("ResultBinding");
        this._xmlWriter.WriteAttributeString("Name", resultBinding.Property.Name);
        this._xmlWriter.WriteAttributeString("ColumnName", resultBinding.ColumnName);
        this._xmlWriter.WriteEndElement();
      }
    }

    private void WriteScalarParameterElement(
      EdmMember member,
      ModificationFunctionParameterBinding parameterBinding)
    {
      this._xmlWriter.WriteStartElement("ScalarProperty");
      this._xmlWriter.WriteAttributeString("Name", member.Name);
      this._xmlWriter.WriteAttributeString("ParameterName", parameterBinding.Parameter.Name);
      this._xmlWriter.WriteAttributeString("Version", parameterBinding.IsCurrent ? "Current" : "Original");
      this._xmlWriter.WriteEndElement();
    }

    private void WritePropertyMapping(PropertyMapping propertyMapping)
    {
      ScalarPropertyMapping scalarPropertyMapping = propertyMapping as ScalarPropertyMapping;
      if (scalarPropertyMapping != null)
      {
        this.WritePropertyMapping(scalarPropertyMapping);
      }
      else
      {
        ComplexPropertyMapping complexPropertyMapping = propertyMapping as ComplexPropertyMapping;
        if (complexPropertyMapping == null)
          return;
        this.WritePropertyMapping(complexPropertyMapping);
      }
    }

    private void WritePropertyMapping(ScalarPropertyMapping scalarPropertyMapping)
    {
      this.WriteScalarPropertyElement(scalarPropertyMapping.Property.Name, scalarPropertyMapping.Column.Name);
    }

    private void WritePropertyMapping(ComplexPropertyMapping complexPropertyMapping)
    {
      this._xmlWriter.WriteStartElement("ComplexProperty");
      this._xmlWriter.WriteAttributeString("Name", complexPropertyMapping.Property.Name);
      this._xmlWriter.WriteAttributeString("TypeName", this._entityTypeNamespace + "." + complexPropertyMapping.Property.ComplexType.Name);
      foreach (PropertyMapping propertyMapping in complexPropertyMapping.TypeMappings.Single<ComplexTypeMapping>().PropertyMappings)
        this.WritePropertyMapping(propertyMapping);
      this._xmlWriter.WriteEndElement();
    }

    private static string GetEntityTypeName(
      string fullyQualifiedEntityTypeName,
      bool isHierarchyMapping)
    {
      if (isHierarchyMapping)
        return "IsTypeOf(" + fullyQualifiedEntityTypeName + ")";
      return fullyQualifiedEntityTypeName;
    }

    private void WriteConditionElement(ConditionPropertyMapping condition)
    {
      this._xmlWriter.WriteStartElement("Condition");
      if (condition.IsNull.HasValue)
        this.WriteIsNullConditionAttribute(condition.IsNull.Value);
      else
        this.WriteConditionValue(condition.Value);
      this._xmlWriter.WriteAttributeString("ColumnName", condition.Column.Name);
      this._xmlWriter.WriteEndElement();
    }

    private void WriteIsNullConditionAttribute(bool isNullValue)
    {
      this._xmlWriter.WriteAttributeString("IsNull", XmlSchemaWriter.GetLowerCaseStringFromBoolValue(isNullValue));
    }

    private void WriteConditionValue(object conditionValue)
    {
      if (conditionValue is bool)
        this._xmlWriter.WriteAttributeString("Value", (bool) conditionValue ? "1" : "0");
      else
        this._xmlWriter.WriteAttributeString("Value", conditionValue.ToString());
    }

    private void WriteScalarPropertyElement(string propertyName, string columnName)
    {
      this._xmlWriter.WriteStartElement("ScalarProperty");
      this._xmlWriter.WriteAttributeString("Name", propertyName);
      this._xmlWriter.WriteAttributeString("ColumnName", columnName);
      this._xmlWriter.WriteEndElement();
    }
  }
}
