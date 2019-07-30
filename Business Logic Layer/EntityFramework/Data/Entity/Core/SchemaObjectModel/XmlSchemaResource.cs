// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.XmlSchemaResource
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal struct XmlSchemaResource
  {
    private static readonly XmlSchemaResource[] _emptyImportList = new XmlSchemaResource[0];
    internal string NamespaceUri;
    internal string ResourceName;
    internal XmlSchemaResource[] ImportedSchemas;

    public XmlSchemaResource(
      string namespaceUri,
      string resourceName,
      XmlSchemaResource[] importedSchemas)
    {
      this.NamespaceUri = namespaceUri;
      this.ResourceName = resourceName;
      this.ImportedSchemas = importedSchemas;
    }

    public XmlSchemaResource(string namespaceUri, string resourceName)
    {
      this.NamespaceUri = namespaceUri;
      this.ResourceName = resourceName;
      this.ImportedSchemas = XmlSchemaResource._emptyImportList;
    }

    internal static Dictionary<string, XmlSchemaResource> GetMetadataSchemaResourceMap(
      double schemaVersion)
    {
      Dictionary<string, XmlSchemaResource> schemaResourceMap = new Dictionary<string, XmlSchemaResource>((IEqualityComparer<string>) StringComparer.Ordinal);
      XmlSchemaResource.AddEdmSchemaResourceMapEntries(schemaResourceMap, schemaVersion);
      XmlSchemaResource.AddStoreSchemaResourceMapEntries(schemaResourceMap, schemaVersion);
      return schemaResourceMap;
    }

    internal static void AddStoreSchemaResourceMapEntries(
      Dictionary<string, XmlSchemaResource> schemaResourceMap,
      double schemaVersion)
    {
      XmlSchemaResource[] importedSchemas = new XmlSchemaResource[1]
      {
        new XmlSchemaResource("http://schemas.microsoft.com/ado/2007/12/edm/EntityStoreSchemaGenerator", "System.Data.Resources.EntityStoreSchemaGenerator.xsd")
      };
      XmlSchemaResource xmlSchemaResource1 = new XmlSchemaResource("http://schemas.microsoft.com/ado/2006/04/edm/ssdl", "System.Data.Resources.SSDLSchema.xsd", importedSchemas);
      schemaResourceMap.Add(xmlSchemaResource1.NamespaceUri, xmlSchemaResource1);
      if (schemaVersion >= 2.0)
      {
        XmlSchemaResource xmlSchemaResource2 = new XmlSchemaResource("http://schemas.microsoft.com/ado/2009/02/edm/ssdl", "System.Data.Resources.SSDLSchema_2.xsd", importedSchemas);
        schemaResourceMap.Add(xmlSchemaResource2.NamespaceUri, xmlSchemaResource2);
      }
      if (schemaVersion >= 3.0)
      {
        XmlSchemaResource xmlSchemaResource2 = new XmlSchemaResource("http://schemas.microsoft.com/ado/2009/11/edm/ssdl", "System.Data.Resources.SSDLSchema_3.xsd", importedSchemas);
        schemaResourceMap.Add(xmlSchemaResource2.NamespaceUri, xmlSchemaResource2);
      }
      XmlSchemaResource xmlSchemaResource3 = new XmlSchemaResource("http://schemas.microsoft.com/ado/2006/04/edm/providermanifest", "System.Data.Resources.ProviderServices.ProviderManifest.xsd");
      schemaResourceMap.Add(xmlSchemaResource3.NamespaceUri, xmlSchemaResource3);
    }

    internal static void AddMappingSchemaResourceMapEntries(
      Dictionary<string, XmlSchemaResource> schemaResourceMap,
      double schemaVersion)
    {
      XmlSchemaResource xmlSchemaResource1 = new XmlSchemaResource("urn:schemas-microsoft-com:windows:storage:mapping:CS", "System.Data.Resources.CSMSL_1.xsd");
      schemaResourceMap.Add(xmlSchemaResource1.NamespaceUri, xmlSchemaResource1);
      if (schemaVersion >= 2.0)
      {
        XmlSchemaResource xmlSchemaResource2 = new XmlSchemaResource("http://schemas.microsoft.com/ado/2008/09/mapping/cs", "System.Data.Resources.CSMSL_2.xsd");
        schemaResourceMap.Add(xmlSchemaResource2.NamespaceUri, xmlSchemaResource2);
      }
      if (schemaVersion < 3.0)
        return;
      XmlSchemaResource xmlSchemaResource3 = new XmlSchemaResource("http://schemas.microsoft.com/ado/2009/11/mapping/cs", "System.Data.Resources.CSMSL_3.xsd");
      schemaResourceMap.Add(xmlSchemaResource3.NamespaceUri, xmlSchemaResource3);
    }

    internal static void AddEdmSchemaResourceMapEntries(
      Dictionary<string, XmlSchemaResource> schemaResourceMap,
      double schemaVersion)
    {
      XmlSchemaResource[] importedSchemas1 = new XmlSchemaResource[1]
      {
        new XmlSchemaResource("http://schemas.microsoft.com/ado/2006/04/codegeneration", "System.Data.Resources.CodeGenerationSchema.xsd")
      };
      XmlSchemaResource[] importedSchemas2 = new XmlSchemaResource[2]
      {
        new XmlSchemaResource("http://schemas.microsoft.com/ado/2006/04/codegeneration", "System.Data.Resources.CodeGenerationSchema.xsd"),
        new XmlSchemaResource("http://schemas.microsoft.com/ado/2009/02/edm/annotation", "System.Data.Resources.AnnotationSchema.xsd")
      };
      XmlSchemaResource[] importedSchemas3 = new XmlSchemaResource[2]
      {
        new XmlSchemaResource("http://schemas.microsoft.com/ado/2006/04/codegeneration", "System.Data.Resources.CodeGenerationSchema.xsd"),
        new XmlSchemaResource("http://schemas.microsoft.com/ado/2009/02/edm/annotation", "System.Data.Resources.AnnotationSchema.xsd")
      };
      XmlSchemaResource xmlSchemaResource1 = new XmlSchemaResource("http://schemas.microsoft.com/ado/2006/04/edm", "System.Data.Resources.CSDLSchema_1.xsd", importedSchemas1);
      schemaResourceMap.Add(xmlSchemaResource1.NamespaceUri, xmlSchemaResource1);
      XmlSchemaResource xmlSchemaResource2 = new XmlSchemaResource("http://schemas.microsoft.com/ado/2007/05/edm", "System.Data.Resources.CSDLSchema_1_1.xsd", importedSchemas1);
      schemaResourceMap.Add(xmlSchemaResource2.NamespaceUri, xmlSchemaResource2);
      if (schemaVersion >= 2.0)
      {
        XmlSchemaResource xmlSchemaResource3 = new XmlSchemaResource("http://schemas.microsoft.com/ado/2008/09/edm", "System.Data.Resources.CSDLSchema_2.xsd", importedSchemas2);
        schemaResourceMap.Add(xmlSchemaResource3.NamespaceUri, xmlSchemaResource3);
      }
      if (schemaVersion < 3.0)
        return;
      XmlSchemaResource xmlSchemaResource4 = new XmlSchemaResource("http://schemas.microsoft.com/ado/2009/11/edm", "System.Data.Resources.CSDLSchema_3.xsd", importedSchemas3);
      schemaResourceMap.Add(xmlSchemaResource4.NamespaceUri, xmlSchemaResource4);
    }
  }
}
