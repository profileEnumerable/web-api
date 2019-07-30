// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.EdmSerializationVisitor
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Edm;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Resources;
using System.Linq;
using System.Text;
using System.Xml;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal sealed class EdmSerializationVisitor : EdmModelVisitor
  {
    private readonly EdmXmlSchemaWriter _schemaWriter;

    public EdmSerializationVisitor(
      XmlWriter xmlWriter,
      double edmVersion,
      bool serializeDefaultNullability = false)
      : this(new EdmXmlSchemaWriter(xmlWriter, edmVersion, serializeDefaultNullability, (IDbDependencyResolver) null))
    {
    }

    public EdmSerializationVisitor(EdmXmlSchemaWriter schemaWriter)
    {
      this._schemaWriter = schemaWriter;
    }

    public void Visit(EdmModel edmModel, string modelNamespace)
    {
      this._schemaWriter.WriteSchemaElementHeader(modelNamespace ?? edmModel.NamespaceNames.DefaultIfEmpty<string>("Empty").Single<string>());
      this.VisitEdmModel(edmModel);
      this._schemaWriter.WriteEndElement();
    }

    public void Visit(EdmModel edmModel, string provider, string providerManifestToken)
    {
      this.Visit(edmModel, edmModel.Containers.Single<EntityContainer>().Name + "Schema", provider, providerManifestToken);
    }

    public void Visit(
      EdmModel edmModel,
      string namespaceName,
      string provider,
      string providerManifestToken)
    {
      bool writeStoreSchemaGenNamespace = edmModel.Container.BaseEntitySets.Any<EntitySetBase>((Func<EntitySetBase, bool>) (e => e.MetadataProperties.Any<MetadataProperty>((Func<MetadataProperty, bool>) (p => p.Name.StartsWith("http://schemas.microsoft.com/ado/2007/12/edm/EntityStoreSchemaGenerator", StringComparison.Ordinal)))));
      this._schemaWriter.WriteSchemaElementHeader(namespaceName, provider, providerManifestToken, writeStoreSchemaGenNamespace);
      this.VisitEdmModel(edmModel);
      this._schemaWriter.WriteEndElement();
    }

    protected override void VisitEdmEntityContainer(EntityContainer item)
    {
      this._schemaWriter.WriteEntityContainerElementHeader(item);
      base.VisitEdmEntityContainer(item);
      this._schemaWriter.WriteEndElement();
    }

    protected internal override void VisitEdmFunction(EdmFunction item)
    {
      this._schemaWriter.WriteFunctionElementHeader(item);
      base.VisitEdmFunction(item);
      this._schemaWriter.WriteEndElement();
    }

    protected internal override void VisitFunctionParameter(FunctionParameter functionParameter)
    {
      this._schemaWriter.WriteFunctionParameterHeader(functionParameter);
      base.VisitFunctionParameter(functionParameter);
      this._schemaWriter.WriteEndElement();
    }

    protected internal override void VisitFunctionReturnParameter(FunctionParameter returnParameter)
    {
      if (returnParameter.TypeUsage.EdmType.BuiltInTypeKind != BuiltInTypeKind.PrimitiveType)
      {
        this._schemaWriter.WriteFunctionReturnTypeElementHeader();
        base.VisitFunctionReturnParameter(returnParameter);
        this._schemaWriter.WriteEndElement();
      }
      else
        base.VisitFunctionReturnParameter(returnParameter);
    }

    protected internal override void VisitCollectionType(CollectionType collectionType)
    {
      this._schemaWriter.WriteCollectionTypeElementHeader();
      base.VisitCollectionType(collectionType);
      this._schemaWriter.WriteEndElement();
    }

    protected override void VisitEdmAssociationSet(AssociationSet item)
    {
      this._schemaWriter.WriteAssociationSetElementHeader(item);
      base.VisitEdmAssociationSet(item);
      if (item.SourceSet != null)
        this._schemaWriter.WriteAssociationSetEndElement(item.SourceSet, item.SourceEnd.Name);
      if (item.TargetSet != null)
        this._schemaWriter.WriteAssociationSetEndElement(item.TargetSet, item.TargetEnd.Name);
      this._schemaWriter.WriteEndElement();
    }

    protected internal override void VisitEdmEntitySet(EntitySet item)
    {
      this._schemaWriter.WriteEntitySetElementHeader(item);
      this._schemaWriter.WriteDefiningQuery(item);
      base.VisitEdmEntitySet(item);
      this._schemaWriter.WriteEndElement();
    }

    protected internal override void VisitFunctionImport(EdmFunction functionImport)
    {
      this._schemaWriter.WriteFunctionImportElementHeader(functionImport);
      if (functionImport.ReturnParameters.Count == 1)
      {
        this._schemaWriter.WriteFunctionImportReturnTypeAttributes(functionImport.ReturnParameter, functionImport.EntitySet, true);
        this.VisitFunctionImportReturnParameter(functionImport.ReturnParameter);
      }
      base.VisitFunctionImport(functionImport);
      if (functionImport.ReturnParameters.Count > 1)
        this.VisitFunctionImportReturnParameters(functionImport);
      this._schemaWriter.WriteEndElement();
    }

    protected internal override void VisitFunctionImportParameter(FunctionParameter parameter)
    {
      this._schemaWriter.WriteFunctionImportParameterElementHeader(parameter);
      base.VisitFunctionImportParameter(parameter);
      this._schemaWriter.WriteEndElement();
    }

    private void VisitFunctionImportReturnParameters(EdmFunction functionImport)
    {
      for (int index = 0; index < functionImport.ReturnParameters.Count; ++index)
      {
        this._schemaWriter.WriteFunctionReturnTypeElementHeader();
        this._schemaWriter.WriteFunctionImportReturnTypeAttributes(functionImport.ReturnParameters[index], functionImport.EntitySets[index], false);
        this.VisitFunctionImportReturnParameter(functionImport.ReturnParameter);
        this._schemaWriter.WriteEndElement();
      }
    }

    protected internal override void VisitRowType(RowType rowType)
    {
      this._schemaWriter.WriteRowTypeElementHeader();
      base.VisitRowType(rowType);
      this._schemaWriter.WriteEndElement();
    }

    protected internal override void VisitEdmEntityType(EntityType item)
    {
      StringBuilder builder = new StringBuilder();
      EdmSerializationVisitor.AppendSchemaErrors(builder, (MetadataItem) item);
      if (MetadataItemHelper.IsInvalid((MetadataItem) item))
      {
        this.AppendMetadataItem<EntityType>(builder, item, (Action<EdmSerializationVisitor, EntityType>) ((v, i) => v.InternalVisitEdmEntityType(i)));
        this.WriteComment(builder.ToString());
      }
      else
      {
        this.WriteComment(builder.ToString());
        this.InternalVisitEdmEntityType(item);
      }
    }

    protected override void VisitEdmEnumType(EnumType item)
    {
      this._schemaWriter.WriteEnumTypeElementHeader(item);
      base.VisitEdmEnumType(item);
      this._schemaWriter.WriteEndElement();
    }

    protected override void VisitEdmEnumTypeMember(EnumMember item)
    {
      this._schemaWriter.WriteEnumTypeMemberElementHeader(item);
      base.VisitEdmEnumTypeMember(item);
      this._schemaWriter.WriteEndElement();
    }

    protected override void VisitKeyProperties(EntityType entityType, IList<EdmProperty> properties)
    {
      if (!properties.Any<EdmProperty>())
        return;
      this._schemaWriter.WriteDelaredKeyPropertiesElementHeader();
      foreach (EdmProperty property in (IEnumerable<EdmProperty>) properties)
        this._schemaWriter.WriteDelaredKeyPropertyRefElement(property);
      this._schemaWriter.WriteEndElement();
    }

    protected internal override void VisitEdmProperty(EdmProperty item)
    {
      this._schemaWriter.WritePropertyElementHeader(item);
      base.VisitEdmProperty(item);
      this._schemaWriter.WriteEndElement();
    }

    protected override void VisitEdmNavigationProperty(NavigationProperty item)
    {
      this._schemaWriter.WriteNavigationPropertyElementHeader(item);
      base.VisitEdmNavigationProperty(item);
      this._schemaWriter.WriteEndElement();
    }

    protected override void VisitComplexType(ComplexType item)
    {
      this._schemaWriter.WriteComplexTypeElementHeader(item);
      base.VisitComplexType(item);
      this._schemaWriter.WriteEndElement();
    }

    protected internal override void VisitEdmAssociationType(AssociationType item)
    {
      StringBuilder builder = new StringBuilder();
      EdmSerializationVisitor.AppendSchemaErrors(builder, (MetadataItem) item);
      if (MetadataItemHelper.IsInvalid((MetadataItem) item))
      {
        this.AppendMetadataItem<AssociationType>(builder, item, (Action<EdmSerializationVisitor, AssociationType>) ((v, i) => v.InternalVisitEdmAssociationType(i)));
        this.WriteComment(builder.ToString());
      }
      else
      {
        this.WriteComment(builder.ToString());
        this.InternalVisitEdmAssociationType(item);
      }
    }

    protected override void VisitEdmAssociationEnd(RelationshipEndMember item)
    {
      this._schemaWriter.WriteAssociationEndElementHeader(item);
      if (item.DeleteBehavior != OperationAction.None)
        this._schemaWriter.WriteOperationActionElement("OnDelete", item.DeleteBehavior);
      this.VisitMetadataItem((MetadataItem) item);
      this._schemaWriter.WriteEndElement();
    }

    protected override void VisitEdmAssociationConstraint(ReferentialConstraint item)
    {
      this._schemaWriter.WriteReferentialConstraintElementHeader();
      this._schemaWriter.WriteReferentialConstraintRoleElement("Principal", item.FromRole, (IEnumerable<EdmProperty>) item.FromProperties);
      this._schemaWriter.WriteReferentialConstraintRoleElement("Dependent", item.ToRole, (IEnumerable<EdmProperty>) item.ToProperties);
      this.VisitMetadataItem((MetadataItem) item);
      this._schemaWriter.WriteEndElement();
    }

    private void InternalVisitEdmEntityType(EntityType item)
    {
      this._schemaWriter.WriteEntityTypeElementHeader(item);
      base.VisitEdmEntityType(item);
      this._schemaWriter.WriteEndElement();
    }

    private void InternalVisitEdmAssociationType(AssociationType item)
    {
      this._schemaWriter.WriteAssociationTypeElementHeader(item);
      base.VisitEdmAssociationType(item);
      this._schemaWriter.WriteEndElement();
    }

    private static void AppendSchemaErrors(StringBuilder builder, MetadataItem item)
    {
      if (!MetadataItemHelper.HasSchemaErrors(item))
        return;
      builder.Append(Strings.MetadataItemErrorsFoundDuringGeneration);
      foreach (EdmSchemaError schemaError in MetadataItemHelper.GetSchemaErrors(item))
      {
        builder.AppendLine();
        builder.Append(schemaError.ToString());
      }
    }

    private void AppendMetadataItem<T>(
      StringBuilder builder,
      T item,
      Action<EdmSerializationVisitor, T> visitAction)
      where T : MetadataItem
    {
      XmlWriterSettings settings = new XmlWriterSettings()
      {
        ConformanceLevel = ConformanceLevel.Fragment,
        Indent = true
      };
      settings.NewLineChars += "        ";
      builder.Append(settings.NewLineChars);
      using (XmlWriter xmlWriter = XmlWriter.Create(builder, settings))
      {
        EdmSerializationVisitor serializationVisitor = new EdmSerializationVisitor(this._schemaWriter.Replicate(xmlWriter));
        visitAction(serializationVisitor, item);
      }
    }

    private void WriteComment(string comment)
    {
      this._schemaWriter.WriteComment(comment.Replace("--", "- -"));
    }
  }
}
