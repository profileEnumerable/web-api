// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.EdmXmlSchemaWriter
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal class EdmXmlSchemaWriter : XmlSchemaWriter
  {
    private static readonly string[] _syndicationItemToTargetPath = new string[21]
    {
      string.Empty,
      "SyndicationAuthorEmail",
      "SyndicationAuthorName",
      "SyndicationAuthorUri",
      "SyndicationContributorEmail",
      "SyndicationContributorName",
      "SyndicationContributorUri",
      "SyndicationUpdated",
      "SyndicationPublished",
      "SyndicationRights",
      "SyndicationSummary",
      "SyndicationTitle",
      "SyndicationCategoryLabel",
      "SyndicationCategoryScheme",
      "SyndicationCategoryTerm",
      "SyndicationLinkHref",
      "SyndicationLinkHrefLang",
      "SyndicationLinkLength",
      "SyndicationLinkRel",
      "SyndicationLinkTitle",
      "SyndicationLinkType"
    };
    private static readonly string[] _syndicationTextContentKindToString = new string[3]
    {
      "text",
      "html",
      "xhtml"
    };
    private const string AnnotationNamespacePrefix = "annotation";
    private const string CustomAnnotationNamespacePrefix = "customannotation";
    private const string StoreSchemaGenNamespacePrefix = "store";
    private const string DataServicesPrefix = "m";
    private const string DataServicesNamespace = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";
    private const string DataServicesMimeTypeAttribute = "System.Data.Services.MimeTypeAttribute";
    private const string DataServicesHasStreamAttribute = "System.Data.Services.Common.HasStreamAttribute";
    private const string DataServicesEntityPropertyMappingAttribute = "System.Data.Services.Common.EntityPropertyMappingAttribute";
    private readonly bool _serializeDefaultNullability;
    private readonly IDbDependencyResolver _resolver;

    private static string SyndicationItemPropertyToString(object value)
    {
      return EdmXmlSchemaWriter._syndicationItemToTargetPath[(int) value];
    }

    private static string SyndicationTextContentKindToString(object value)
    {
      return EdmXmlSchemaWriter._syndicationTextContentKindToString[(int) value];
    }

    public EdmXmlSchemaWriter()
    {
      this._resolver = DbConfiguration.DependencyResolver;
    }

    internal EdmXmlSchemaWriter(
      XmlWriter xmlWriter,
      double edmVersion,
      bool serializeDefaultNullability,
      IDbDependencyResolver resolver = null)
    {
      this._resolver = resolver ?? DbConfiguration.DependencyResolver;
      this._serializeDefaultNullability = serializeDefaultNullability;
      this._xmlWriter = xmlWriter;
      this._version = edmVersion;
    }

    internal virtual void WriteSchemaElementHeader(string schemaNamespace)
    {
      this._xmlWriter.WriteStartElement("Schema", XmlConstants.GetCsdlNamespace(this._version));
      this._xmlWriter.WriteAttributeString("Namespace", schemaNamespace);
      this._xmlWriter.WriteAttributeString("Alias", "Self");
      if (this._version == 3.0)
        this._xmlWriter.WriteAttributeString("annotation", "UseStrongSpatialTypes", "http://schemas.microsoft.com/ado/2009/02/edm/annotation", "false");
      this._xmlWriter.WriteAttributeString("xmlns", "annotation", (string) null, "http://schemas.microsoft.com/ado/2009/02/edm/annotation");
      this._xmlWriter.WriteAttributeString("xmlns", "customannotation", (string) null, "http://schemas.microsoft.com/ado/2013/11/edm/customannotation");
    }

    internal virtual void WriteSchemaElementHeader(
      string schemaNamespace,
      string provider,
      string providerManifestToken,
      bool writeStoreSchemaGenNamespace)
    {
      this._xmlWriter.WriteStartElement("Schema", XmlConstants.GetSsdlNamespace(this._version));
      this._xmlWriter.WriteAttributeString("Namespace", schemaNamespace);
      this._xmlWriter.WriteAttributeString("Provider", provider);
      this._xmlWriter.WriteAttributeString("ProviderManifestToken", providerManifestToken);
      this._xmlWriter.WriteAttributeString("Alias", "Self");
      if (writeStoreSchemaGenNamespace)
        this._xmlWriter.WriteAttributeString("xmlns", "store", (string) null, "http://schemas.microsoft.com/ado/2007/12/edm/EntityStoreSchemaGenerator");
      this._xmlWriter.WriteAttributeString("xmlns", "customannotation", (string) null, "http://schemas.microsoft.com/ado/2013/11/edm/customannotation");
    }

    private void WritePolymorphicTypeAttributes(EdmType edmType)
    {
      if (edmType.BaseType != null)
        this._xmlWriter.WriteAttributeString("BaseType", XmlSchemaWriter.GetQualifiedTypeName("Self", edmType.BaseType.Name));
      if (!edmType.Abstract)
        return;
      this._xmlWriter.WriteAttributeString("Abstract", "true");
    }

    public virtual void WriteFunctionElementHeader(EdmFunction function)
    {
      this._xmlWriter.WriteStartElement("Function");
      this._xmlWriter.WriteAttributeString("Name", function.Name);
      this._xmlWriter.WriteAttributeString("Aggregate", XmlSchemaWriter.GetLowerCaseStringFromBoolValue(function.AggregateAttribute));
      this._xmlWriter.WriteAttributeString("BuiltIn", XmlSchemaWriter.GetLowerCaseStringFromBoolValue(function.BuiltInAttribute));
      this._xmlWriter.WriteAttributeString("NiladicFunction", XmlSchemaWriter.GetLowerCaseStringFromBoolValue(function.NiladicFunctionAttribute));
      this._xmlWriter.WriteAttributeString("IsComposable", XmlSchemaWriter.GetLowerCaseStringFromBoolValue(function.IsComposableAttribute));
      this._xmlWriter.WriteAttributeString("ParameterTypeSemantics", function.ParameterTypeSemanticsAttribute.ToString());
      this._xmlWriter.WriteAttributeString("Schema", function.Schema);
      if (function.StoreFunctionNameAttribute != null && function.StoreFunctionNameAttribute != function.Name)
        this._xmlWriter.WriteAttributeString("StoreFunctionName", function.StoreFunctionNameAttribute);
      if (function.ReturnParameters == null || !function.ReturnParameters.Any<FunctionParameter>())
        return;
      EdmType edmType = function.ReturnParameters.First<FunctionParameter>().TypeUsage.EdmType;
      if (edmType.BuiltInTypeKind != BuiltInTypeKind.PrimitiveType)
        return;
      this._xmlWriter.WriteAttributeString("ReturnType", EdmXmlSchemaWriter.GetTypeName(edmType));
    }

    public virtual void WriteFunctionParameterHeader(FunctionParameter functionParameter)
    {
      this._xmlWriter.WriteStartElement("Parameter");
      this._xmlWriter.WriteAttributeString("Name", functionParameter.Name);
      this._xmlWriter.WriteAttributeString("Type", functionParameter.TypeName);
      this._xmlWriter.WriteAttributeString("Mode", functionParameter.Mode.ToString());
      if (functionParameter.IsMaxLength)
        this._xmlWriter.WriteAttributeString("MaxLength", "Max");
      else if (!functionParameter.IsMaxLengthConstant && functionParameter.MaxLength.HasValue)
        this._xmlWriter.WriteAttributeString("MaxLength", functionParameter.MaxLength.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!functionParameter.IsPrecisionConstant && functionParameter.Precision.HasValue)
        this._xmlWriter.WriteAttributeString("Precision", functionParameter.Precision.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (functionParameter.IsScaleConstant || !functionParameter.Scale.HasValue)
        return;
      this._xmlWriter.WriteAttributeString("Scale", functionParameter.Scale.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    internal virtual void WriteFunctionReturnTypeElementHeader()
    {
      this._xmlWriter.WriteStartElement("ReturnType");
    }

    internal void WriteEntityTypeElementHeader(EntityType entityType)
    {
      this._xmlWriter.WriteStartElement("EntityType");
      this._xmlWriter.WriteAttributeString("Name", entityType.Name);
      this.WriteExtendedProperties((MetadataItem) entityType);
      if (entityType.Annotations.GetClrAttributes() != null)
      {
        foreach (Attribute clrAttribute in (IEnumerable<Attribute>) entityType.Annotations.GetClrAttributes())
        {
          if (clrAttribute.GetType().FullName.Equals("System.Data.Services.Common.HasStreamAttribute", StringComparison.Ordinal))
            this._xmlWriter.WriteAttributeString("m", "HasStream", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata", "true");
          else if (clrAttribute.GetType().FullName.Equals("System.Data.Services.MimeTypeAttribute", StringComparison.Ordinal))
          {
            string propertyName = clrAttribute.GetType().GetDeclaredProperty("MemberName").GetValue((object) clrAttribute, (object[]) null) as string;
            EdmXmlSchemaWriter.AddAttributeAnnotation(entityType.Properties.SingleOrDefault<EdmProperty>((Func<EdmProperty, bool>) (p => p.Name.Equals(propertyName, StringComparison.Ordinal))), clrAttribute);
          }
          else if (clrAttribute.GetType().FullName.Equals("System.Data.Services.Common.EntityPropertyMappingAttribute", StringComparison.Ordinal))
          {
            string str = clrAttribute.GetType().GetDeclaredProperty("SourcePath").GetValue((object) clrAttribute, (object[]) null) as string;
            int length = str.IndexOf("/", StringComparison.Ordinal);
            string propertyName = length != -1 ? str.Substring(0, length) : str;
            EdmXmlSchemaWriter.AddAttributeAnnotation(entityType.Properties.SingleOrDefault<EdmProperty>((Func<EdmProperty, bool>) (p => p.Name.Equals(propertyName, StringComparison.Ordinal))), clrAttribute);
          }
        }
      }
      this.WritePolymorphicTypeAttributes((EdmType) entityType);
    }

    internal void WriteEnumTypeElementHeader(EnumType enumType)
    {
      this._xmlWriter.WriteStartElement("EnumType");
      this._xmlWriter.WriteAttributeString("Name", enumType.Name);
      this._xmlWriter.WriteAttributeString("IsFlags", XmlSchemaWriter.GetLowerCaseStringFromBoolValue(enumType.IsFlags));
      this.WriteExtendedProperties((MetadataItem) enumType);
      if (enumType.UnderlyingType == null)
        return;
      this._xmlWriter.WriteAttributeString("UnderlyingType", enumType.UnderlyingType.PrimitiveTypeKind.ToString());
    }

    internal void WriteEnumTypeMemberElementHeader(EnumMember enumTypeMember)
    {
      this._xmlWriter.WriteStartElement("Member");
      this._xmlWriter.WriteAttributeString("Name", enumTypeMember.Name);
      this._xmlWriter.WriteAttributeString("Value", enumTypeMember.Value.ToString());
    }

    private static void AddAttributeAnnotation(EdmProperty property, Attribute a)
    {
      if (property == null)
        return;
      IList<Attribute> clrAttributes = property.Annotations.GetClrAttributes();
      if (clrAttributes != null)
      {
        if (clrAttributes.Contains(a))
          return;
        clrAttributes.Add(a);
      }
      else
        property.GetMetadataProperties().SetClrAttributes((IList<Attribute>) new List<Attribute>()
        {
          a
        });
    }

    internal void WriteComplexTypeElementHeader(ComplexType complexType)
    {
      this._xmlWriter.WriteStartElement("ComplexType");
      this._xmlWriter.WriteAttributeString("Name", complexType.Name);
      this.WriteExtendedProperties((MetadataItem) complexType);
      this.WritePolymorphicTypeAttributes((EdmType) complexType);
    }

    internal virtual void WriteCollectionTypeElementHeader()
    {
      this._xmlWriter.WriteStartElement("CollectionType");
    }

    internal virtual void WriteRowTypeElementHeader()
    {
      this._xmlWriter.WriteStartElement("RowType");
    }

    internal void WriteAssociationTypeElementHeader(AssociationType associationType)
    {
      this._xmlWriter.WriteStartElement("Association");
      this._xmlWriter.WriteAttributeString("Name", associationType.Name);
    }

    internal void WriteAssociationEndElementHeader(RelationshipEndMember associationEnd)
    {
      this._xmlWriter.WriteStartElement("End");
      this._xmlWriter.WriteAttributeString("Role", associationEnd.Name);
      this._xmlWriter.WriteAttributeString("Type", XmlSchemaWriter.GetQualifiedTypeName("Self", associationEnd.GetEntityType().Name));
      this._xmlWriter.WriteAttributeString("Multiplicity", RelationshipMultiplicityConverter.MultiplicityToString(associationEnd.RelationshipMultiplicity));
    }

    internal void WriteOperationActionElement(string elementName, OperationAction operationAction)
    {
      this._xmlWriter.WriteStartElement(elementName);
      this._xmlWriter.WriteAttributeString("Action", operationAction.ToString());
      this._xmlWriter.WriteEndElement();
    }

    internal void WriteReferentialConstraintElementHeader()
    {
      this._xmlWriter.WriteStartElement("ReferentialConstraint");
    }

    internal void WriteDelaredKeyPropertiesElementHeader()
    {
      this._xmlWriter.WriteStartElement("Key");
    }

    internal void WriteDelaredKeyPropertyRefElement(EdmProperty property)
    {
      this._xmlWriter.WriteStartElement("PropertyRef");
      this._xmlWriter.WriteAttributeString("Name", property.Name);
      this._xmlWriter.WriteEndElement();
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    internal void WritePropertyElementHeader(EdmProperty property)
    {
      this._xmlWriter.WriteStartElement("Property");
      this._xmlWriter.WriteAttributeString("Name", property.Name);
      this._xmlWriter.WriteAttributeString("Type", EdmXmlSchemaWriter.GetTypeReferenceName(property));
      if (property.CollectionKind != CollectionKind.None)
        this._xmlWriter.WriteAttributeString("CollectionKind", property.CollectionKind.ToString());
      if (property.ConcurrencyMode == ConcurrencyMode.Fixed)
        this._xmlWriter.WriteAttributeString("ConcurrencyMode", "Fixed");
      this.WriteExtendedProperties((MetadataItem) property);
      if (property.Annotations.GetClrAttributes() != null)
      {
        int num1 = 0;
        foreach (Attribute clrAttribute in (IEnumerable<Attribute>) property.Annotations.GetClrAttributes())
        {
          if (clrAttribute.GetType().FullName.Equals("System.Data.Services.MimeTypeAttribute", StringComparison.Ordinal))
            this._xmlWriter.WriteAttributeString("m", "MimeType", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata", clrAttribute.GetType().GetDeclaredProperty("MimeType").GetValue((object) clrAttribute, (object[]) null) as string);
          else if (clrAttribute.GetType().FullName.Equals("System.Data.Services.Common.EntityPropertyMappingAttribute", StringComparison.Ordinal))
          {
            string str1;
            if (num1 != 0)
              str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "_{0}", (object) num1);
            else
              str1 = string.Empty;
            string str2 = str1;
            string str3 = clrAttribute.GetType().GetDeclaredProperty("SourcePath").GetValue((object) clrAttribute, (object[]) null) as string;
            int num2 = str3.IndexOf("/", StringComparison.Ordinal);
            if (num2 != -1 && num2 + 1 < str3.Length)
              this._xmlWriter.WriteAttributeString("m", "FC_SourcePath" + str2, "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata", str3.Substring(num2 + 1));
            object obj1 = clrAttribute.GetType().GetDeclaredProperty("TargetSyndicationItem").GetValue((object) clrAttribute, (object[]) null);
            string str4 = clrAttribute.GetType().GetDeclaredProperty("KeepInContent").GetValue((object) clrAttribute, (object[]) null).ToString();
            PropertyInfo declaredProperty = clrAttribute.GetType().GetDeclaredProperty("CriteriaValue");
            string str5 = (string) null;
            if (declaredProperty != (PropertyInfo) null)
              str5 = declaredProperty.GetValue((object) clrAttribute, (object[]) null) as string;
            if (str5 != null)
            {
              this._xmlWriter.WriteAttributeString("m", "FC_TargetPath" + str2, "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata", EdmXmlSchemaWriter.SyndicationItemPropertyToString(obj1));
              this._xmlWriter.WriteAttributeString("m", "FC_KeepInContent" + str2, "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata", str4);
              this._xmlWriter.WriteAttributeString("m", "FC_CriteriaValue" + str2, "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata", str5);
            }
            else if (string.Equals(obj1.ToString(), "CustomProperty", StringComparison.Ordinal))
            {
              string str6 = clrAttribute.GetType().GetDeclaredProperty("TargetPath").GetValue((object) clrAttribute, (object[]) null).ToString();
              string str7 = clrAttribute.GetType().GetDeclaredProperty("TargetNamespacePrefix").GetValue((object) clrAttribute, (object[]) null).ToString();
              string str8 = clrAttribute.GetType().GetDeclaredProperty("TargetNamespaceUri").GetValue((object) clrAttribute, (object[]) null).ToString();
              this._xmlWriter.WriteAttributeString("m", "FC_TargetPath" + str2, "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata", str6);
              this._xmlWriter.WriteAttributeString("m", "FC_NsUri" + str2, "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata", str8);
              this._xmlWriter.WriteAttributeString("m", "FC_NsPrefix" + str2, "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata", str7);
              this._xmlWriter.WriteAttributeString("m", "FC_KeepInContent" + str2, "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata", str4);
            }
            else
            {
              object obj2 = clrAttribute.GetType().GetDeclaredProperty("TargetTextContentKind").GetValue((object) clrAttribute, (object[]) null);
              this._xmlWriter.WriteAttributeString("m", "FC_TargetPath" + str2, "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata", EdmXmlSchemaWriter.SyndicationItemPropertyToString(obj1));
              this._xmlWriter.WriteAttributeString("m", "FC_ContentKind" + str2, "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata", EdmXmlSchemaWriter.SyndicationTextContentKindToString(obj2));
              this._xmlWriter.WriteAttributeString("m", "FC_KeepInContent" + str2, "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata", str4);
            }
            ++num1;
          }
        }
      }
      if (property.IsMaxLength)
        this._xmlWriter.WriteAttributeString("MaxLength", "Max");
      else if (!property.IsMaxLengthConstant && property.MaxLength.HasValue)
        this._xmlWriter.WriteAttributeString("MaxLength", property.MaxLength.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!property.IsFixedLengthConstant && property.IsFixedLength.HasValue)
        this._xmlWriter.WriteAttributeString("FixedLength", XmlSchemaWriter.GetLowerCaseStringFromBoolValue(property.IsFixedLength.Value));
      if (!property.IsUnicodeConstant && property.IsUnicode.HasValue)
        this._xmlWriter.WriteAttributeString("Unicode", XmlSchemaWriter.GetLowerCaseStringFromBoolValue(property.IsUnicode.Value));
      if (!property.IsPrecisionConstant && property.Precision.HasValue)
        this._xmlWriter.WriteAttributeString("Precision", property.Precision.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!property.IsScaleConstant && property.Scale.HasValue)
        this._xmlWriter.WriteAttributeString("Scale", property.Scale.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (property.StoreGeneratedPattern != StoreGeneratedPattern.None)
        this._xmlWriter.WriteAttributeString("StoreGeneratedPattern", property.StoreGeneratedPattern == StoreGeneratedPattern.Computed ? "Computed" : "Identity");
      if (this._serializeDefaultNullability || !property.Nullable)
        this._xmlWriter.WriteAttributeString("Nullable", XmlSchemaWriter.GetLowerCaseStringFromBoolValue(property.Nullable));
      MetadataProperty metadataProperty;
      if (!property.MetadataProperties.TryGetValue("http://schemas.microsoft.com/ado/2009/02/edm/annotation:StoreGeneratedPattern", false, out metadataProperty))
        return;
      this._xmlWriter.WriteAttributeString("StoreGeneratedPattern", "http://schemas.microsoft.com/ado/2009/02/edm/annotation", metadataProperty.Value.ToString());
    }

    private static string GetTypeReferenceName(EdmProperty property)
    {
      if (property.IsPrimitiveType)
        return property.TypeName;
      if (property.IsComplexType)
        return XmlSchemaWriter.GetQualifiedTypeName("Self", property.ComplexType.Name);
      return XmlSchemaWriter.GetQualifiedTypeName("Self", property.EnumType.Name);
    }

    internal void WriteNavigationPropertyElementHeader(NavigationProperty member)
    {
      this._xmlWriter.WriteStartElement("NavigationProperty");
      this._xmlWriter.WriteAttributeString("Name", member.Name);
      this._xmlWriter.WriteAttributeString("Relationship", XmlSchemaWriter.GetQualifiedTypeName("Self", member.Association.Name));
      this._xmlWriter.WriteAttributeString("FromRole", member.GetFromEnd().Name);
      this._xmlWriter.WriteAttributeString("ToRole", member.ToEndMember.Name);
    }

    internal void WriteReferentialConstraintRoleElement(
      string roleName,
      RelationshipEndMember edmAssociationEnd,
      IEnumerable<EdmProperty> properties)
    {
      this._xmlWriter.WriteStartElement(roleName);
      this._xmlWriter.WriteAttributeString("Role", edmAssociationEnd.Name);
      foreach (EdmProperty property in properties)
      {
        this._xmlWriter.WriteStartElement("PropertyRef");
        this._xmlWriter.WriteAttributeString("Name", property.Name);
        this._xmlWriter.WriteEndElement();
      }
      this._xmlWriter.WriteEndElement();
    }

    internal virtual void WriteEntityContainerElementHeader(EntityContainer container)
    {
      this._xmlWriter.WriteStartElement("EntityContainer");
      this._xmlWriter.WriteAttributeString("Name", container.Name);
      this.WriteExtendedProperties((MetadataItem) container);
    }

    internal void WriteAssociationSetElementHeader(AssociationSet associationSet)
    {
      this._xmlWriter.WriteStartElement("AssociationSet");
      this._xmlWriter.WriteAttributeString("Name", associationSet.Name);
      this._xmlWriter.WriteAttributeString("Association", XmlSchemaWriter.GetQualifiedTypeName("Self", associationSet.ElementType.Name));
    }

    internal void WriteAssociationSetEndElement(EntitySet end, string roleName)
    {
      this._xmlWriter.WriteStartElement("End");
      this._xmlWriter.WriteAttributeString("Role", roleName);
      this._xmlWriter.WriteAttributeString("EntitySet", end.Name);
      this._xmlWriter.WriteEndElement();
    }

    internal virtual void WriteEntitySetElementHeader(EntitySet entitySet)
    {
      this._xmlWriter.WriteStartElement("EntitySet");
      this._xmlWriter.WriteAttributeString("Name", entitySet.Name);
      this._xmlWriter.WriteAttributeString("EntityType", XmlSchemaWriter.GetQualifiedTypeName("Self", entitySet.ElementType.Name));
      if (!string.IsNullOrWhiteSpace(entitySet.Schema))
        this._xmlWriter.WriteAttributeString("Schema", entitySet.Schema);
      if (!string.IsNullOrWhiteSpace(entitySet.Table))
        this._xmlWriter.WriteAttributeString("Table", entitySet.Table);
      this.WriteExtendedProperties((MetadataItem) entitySet);
    }

    internal virtual void WriteFunctionImportElementHeader(EdmFunction functionImport)
    {
      this._xmlWriter.WriteStartElement("FunctionImport");
      this._xmlWriter.WriteAttributeString("Name", functionImport.Name);
      if (!functionImport.IsComposableAttribute)
        return;
      this._xmlWriter.WriteAttributeString("IsComposable", "true");
    }

    internal virtual void WriteFunctionImportReturnTypeAttributes(
      FunctionParameter returnParameter,
      EntitySet entitySet,
      bool inline)
    {
      this._xmlWriter.WriteAttributeString(inline ? "ReturnType" : "Type", EdmXmlSchemaWriter.GetTypeName(returnParameter.TypeUsage.EdmType));
      if (entitySet == null)
        return;
      this._xmlWriter.WriteAttributeString("EntitySet", entitySet.Name);
    }

    internal virtual void WriteFunctionImportParameterElementHeader(FunctionParameter parameter)
    {
      this._xmlWriter.WriteStartElement("Parameter");
      this._xmlWriter.WriteAttributeString("Name", parameter.Name);
      this._xmlWriter.WriteAttributeString("Mode", parameter.Mode.ToString());
      this._xmlWriter.WriteAttributeString("Type", EdmXmlSchemaWriter.GetTypeName(parameter.TypeUsage.EdmType));
    }

    internal void WriteDefiningQuery(EntitySet entitySet)
    {
      if (string.IsNullOrWhiteSpace(entitySet.DefiningQuery))
        return;
      this._xmlWriter.WriteElementString("DefiningQuery", entitySet.DefiningQuery);
    }

    internal EdmXmlSchemaWriter Replicate(XmlWriter xmlWriter)
    {
      return new EdmXmlSchemaWriter(xmlWriter, this._version, this._serializeDefaultNullability, (IDbDependencyResolver) null);
    }

    internal void WriteExtendedProperties(MetadataItem item)
    {
      foreach (MetadataProperty metadataProperty in item.MetadataProperties.Where<MetadataProperty>((Func<MetadataProperty, bool>) (p => p.PropertyKind == PropertyKind.Extended)))
      {
        string xmlNamespaceUri;
        string attributeName;
        if (EdmXmlSchemaWriter.TrySplitExtendedMetadataPropertyName(metadataProperty.Name, out xmlNamespaceUri, out attributeName) && metadataProperty.Name != "http://schemas.microsoft.com/ado/2009/02/edm/annotation:StoreGeneratedPattern")
        {
          Func<IMetadataAnnotationSerializer> service = this._resolver.GetService<Func<IMetadataAnnotationSerializer>>((object) attributeName);
          string str = service == null ? metadataProperty.Value.ToString() : service().Serialize(attributeName, metadataProperty.Value);
          this._xmlWriter.WriteAttributeString(attributeName, xmlNamespaceUri, str);
        }
      }
    }

    private static bool TrySplitExtendedMetadataPropertyName(
      string name,
      out string xmlNamespaceUri,
      out string attributeName)
    {
      int length = name.LastIndexOf(':');
      if (length < 1 || name.Length <= length + 1)
      {
        xmlNamespaceUri = (string) null;
        attributeName = (string) null;
        return false;
      }
      xmlNamespaceUri = name.Substring(0, length);
      attributeName = name.Substring(length + 1, name.Length - 1 - length);
      return true;
    }

    private static string GetTypeName(EdmType type)
    {
      if (type.BuiltInTypeKind == BuiltInTypeKind.CollectionType)
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Collection({0})", (object) EdmXmlSchemaWriter.GetTypeName(((CollectionType) type).TypeUsage.EdmType));
      if (type.BuiltInTypeKind != BuiltInTypeKind.PrimitiveType)
        return type.FullName;
      return type.Name;
    }

    internal static class SyndicationXmlConstants
    {
      internal const string SyndAuthorEmail = "SyndicationAuthorEmail";
      internal const string SyndAuthorName = "SyndicationAuthorName";
      internal const string SyndAuthorUri = "SyndicationAuthorUri";
      internal const string SyndPublished = "SyndicationPublished";
      internal const string SyndRights = "SyndicationRights";
      internal const string SyndSummary = "SyndicationSummary";
      internal const string SyndTitle = "SyndicationTitle";
      internal const string SyndContributorEmail = "SyndicationContributorEmail";
      internal const string SyndContributorName = "SyndicationContributorName";
      internal const string SyndContributorUri = "SyndicationContributorUri";
      internal const string SyndCategoryLabel = "SyndicationCategoryLabel";
      internal const string SyndContentKindPlaintext = "text";
      internal const string SyndContentKindHtml = "html";
      internal const string SyndContentKindXHtml = "xhtml";
      internal const string SyndUpdated = "SyndicationUpdated";
      internal const string SyndLinkHref = "SyndicationLinkHref";
      internal const string SyndLinkRel = "SyndicationLinkRel";
      internal const string SyndLinkType = "SyndicationLinkType";
      internal const string SyndLinkHrefLang = "SyndicationLinkHrefLang";
      internal const string SyndLinkTitle = "SyndicationLinkTitle";
      internal const string SyndLinkLength = "SyndicationLinkLength";
      internal const string SyndCategoryTerm = "SyndicationCategoryTerm";
      internal const string SyndCategoryScheme = "SyndicationCategoryScheme";
    }
  }
}
