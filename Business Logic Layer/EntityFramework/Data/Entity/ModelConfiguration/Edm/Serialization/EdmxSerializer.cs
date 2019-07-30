// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Edm.Serialization.EdmxSerializer
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Globalization;
using System.Xml;

namespace System.Data.Entity.ModelConfiguration.Edm.Serialization
{
  internal sealed class EdmxSerializer
  {
    private const string EdmXmlNamespaceV1 = "http://schemas.microsoft.com/ado/2007/06/edmx";
    private const string EdmXmlNamespaceV2 = "http://schemas.microsoft.com/ado/2008/10/edmx";
    private const string EdmXmlNamespaceV3 = "http://schemas.microsoft.com/ado/2009/11/edmx";
    private DbDatabaseMapping _databaseMapping;
    private double _version;
    private XmlWriter _xmlWriter;
    private string _namespace;

    public void Serialize(DbDatabaseMapping databaseMapping, XmlWriter xmlWriter)
    {
      this._xmlWriter = xmlWriter;
      this._databaseMapping = databaseMapping;
      this._version = databaseMapping.Model.SchemaVersion;
      this._namespace = object.Equals((object) this._version, (object) 3.0) ? "http://schemas.microsoft.com/ado/2009/11/edmx" : (object.Equals((object) this._version, (object) 2.0) ? "http://schemas.microsoft.com/ado/2008/10/edmx" : "http://schemas.microsoft.com/ado/2007/06/edmx");
      this._xmlWriter.WriteStartDocument();
      using (this.Element("Edmx", "Version", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:F1}", (object) this._version)))
      {
        this.WriteEdmxRuntime();
        this.WriteEdmxDesigner();
      }
      this._xmlWriter.WriteEndDocument();
      this._xmlWriter.Flush();
    }

    private void WriteEdmxRuntime()
    {
      using (this.Element("Runtime"))
      {
        using (this.Element("ConceptualModels"))
          this._databaseMapping.Model.ValidateAndSerializeCsdl(this._xmlWriter);
        using (this.Element("Mappings"))
          new MslSerializer().Serialize(this._databaseMapping, this._xmlWriter);
        using (this.Element("StorageModels"))
          new SsdlSerializer().Serialize(this._databaseMapping.Database, this._databaseMapping.ProviderInfo.ProviderInvariantName, this._databaseMapping.ProviderInfo.ProviderManifestToken, this._xmlWriter, true);
      }
    }

    private void WriteEdmxDesigner()
    {
      using (this.Element("Designer"))
      {
        this.WriteEdmxConnection();
        this.WriteEdmxOptions();
        this.WriteEdmxDiagrams();
      }
    }

    private void WriteEdmxConnection()
    {
      using (this.Element("Connection"))
      {
        using (this.Element("DesignerInfoPropertySet"))
          this.WriteDesignerPropertyElement("MetadataArtifactProcessing", "EmbedInOutputAssembly");
      }
    }

    private void WriteEdmxOptions()
    {
      using (this.Element("Options"))
      {
        using (this.Element("DesignerInfoPropertySet"))
        {
          this.WriteDesignerPropertyElement("ValidateOnBuild", "False");
          this.WriteDesignerPropertyElement("CodeGenerationStrategy", "None");
          this.WriteDesignerPropertyElement("ProcessDependentTemplatesOnSave", "False");
          this.WriteDesignerPropertyElement("UseLegacyProvider", "False");
        }
      }
    }

    private void WriteDesignerPropertyElement(string name, string value)
    {
      using (this.Element("DesignerProperty", "Name", name, "Value", value))
        ;
    }

    private void WriteEdmxDiagrams()
    {
      using (this.Element("Diagrams"))
        ;
    }

    private IDisposable Element(string elementName, params string[] attributes)
    {
      this._xmlWriter.WriteStartElement(elementName, this._namespace);
      for (int index = 0; index < attributes.Length - 1; index += 2)
        this._xmlWriter.WriteAttributeString(attributes[index], attributes[index + 1]);
      return (IDisposable) new EdmxSerializer.EndElement(this._xmlWriter);
    }

    private class EndElement : IDisposable
    {
      private readonly XmlWriter _xmlWriter;

      public EndElement(XmlWriter xmlWriter)
      {
        this._xmlWriter = xmlWriter;
      }

      public void Dispose()
      {
        this._xmlWriter.WriteEndElement();
      }
    }
  }
}
