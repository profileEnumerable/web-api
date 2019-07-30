// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.SchemaManager
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Metadata.Edm.Provider;
using System.Data.Entity.Resources;
using System.Diagnostics;
using System.Linq;
using System.Xml;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  [DebuggerDisplay("DataModel={DataModel}")]
  internal class SchemaManager
  {
    private readonly HashSet<string> _namespaceLookUpTable = new HashSet<string>((IEqualityComparer<string>) StringComparer.Ordinal);
    private readonly SchemaElementLookUpTable<SchemaType> _schemaTypes = new SchemaElementLookUpTable<SchemaType>();
    private const int MaxErrorCount = 100;
    private DbProviderManifest _providerManifest;
    private PrimitiveSchema _primitiveSchema;
    private double effectiveSchemaVersion;
    private readonly SchemaDataModelOption _dataModel;
    private readonly ProviderManifestNeeded _providerManifestNeeded;
    private readonly AttributeValueNotification _providerNotification;
    private readonly AttributeValueNotification _providerManifestTokenNotification;

    private SchemaManager(
      SchemaDataModelOption dataModel,
      AttributeValueNotification providerNotification,
      AttributeValueNotification providerManifestTokenNotification,
      ProviderManifestNeeded providerManifestNeeded)
    {
      this._dataModel = dataModel;
      this._providerNotification = providerNotification;
      this._providerManifestTokenNotification = providerManifestTokenNotification;
      this._providerManifestNeeded = providerManifestNeeded;
    }

    public static IList<EdmSchemaError> LoadProviderManifest(
      XmlReader xmlReader,
      string location,
      bool checkForSystemNamespace,
      out Schema schema)
    {
      IList<Schema> schemaCollection = (IList<Schema>) new List<Schema>(1);
      DbProviderManifest providerManifest = checkForSystemNamespace ? (DbProviderManifest) EdmProviderManifest.Instance : (DbProviderManifest) null;
      IList<EdmSchemaError> andValidate = SchemaManager.ParseAndValidate((IEnumerable<XmlReader>) new XmlReader[1]
      {
        xmlReader
      }, (IEnumerable<string>) new string[1]
      {
        location
      }, SchemaDataModelOption.ProviderManifestModel, providerManifest, out schemaCollection);
      schema = schemaCollection.Count == 0 ? (Schema) null : schemaCollection[0];
      return andValidate;
    }

    public static void NoOpAttributeValueNotification(
      string attributeValue,
      System.Action<string, ErrorCode, EdmSchemaErrorSeverity> addError)
    {
    }

    public static IList<EdmSchemaError> ParseAndValidate(
      IEnumerable<XmlReader> xmlReaders,
      IEnumerable<string> sourceFilePaths,
      SchemaDataModelOption dataModel,
      DbProviderManifest providerManifest,
      out IList<Schema> schemaCollection)
    {
      return SchemaManager.ParseAndValidate(xmlReaders, sourceFilePaths, dataModel, new AttributeValueNotification(SchemaManager.NoOpAttributeValueNotification), new AttributeValueNotification(SchemaManager.NoOpAttributeValueNotification), (ProviderManifestNeeded) (error => providerManifest ?? (DbProviderManifest) MetadataItem.EdmProviderManifest), out schemaCollection);
    }

    public static IList<EdmSchemaError> ParseAndValidate(
      IEnumerable<XmlReader> xmlReaders,
      IEnumerable<string> sourceFilePaths,
      SchemaDataModelOption dataModel,
      AttributeValueNotification providerNotification,
      AttributeValueNotification providerManifestTokenNotification,
      ProviderManifestNeeded providerManifestNeeded,
      out IList<Schema> schemaCollection)
    {
      SchemaManager schemaManager = new SchemaManager(dataModel, providerNotification, providerManifestTokenNotification, providerManifestNeeded);
      List<EdmSchemaError> errorCollection = new List<EdmSchemaError>();
      schemaCollection = (IList<Schema>) new List<Schema>();
      bool errorEncountered = false;
      List<string> stringList = sourceFilePaths == null ? new List<string>() : new List<string>(sourceFilePaths);
      int index = 0;
      foreach (XmlReader xmlReader in xmlReaders)
      {
        string location = (string) null;
        if (stringList.Count <= index)
          SchemaManager.TryGetBaseUri(xmlReader, out location);
        else
          location = stringList[index];
        Schema schema = new Schema(schemaManager);
        IList<EdmSchemaError> newErrors = schema.Parse(xmlReader, location);
        SchemaManager.CheckIsSameVersion(schema, (IEnumerable<Schema>) schemaCollection, errorCollection);
        if (SchemaManager.UpdateErrorCollectionAndCheckForMaxErrors(errorCollection, newErrors, ref errorEncountered))
          return (IList<EdmSchemaError>) errorCollection;
        if (!errorEncountered)
        {
          schemaCollection.Add(schema);
          schemaManager.AddSchema(schema);
        }
        ++index;
      }
      if (!errorEncountered)
      {
        foreach (Schema schema in (IEnumerable<Schema>) schemaCollection)
        {
          if (SchemaManager.UpdateErrorCollectionAndCheckForMaxErrors(errorCollection, schema.Resolve(), ref errorEncountered))
            return (IList<EdmSchemaError>) errorCollection;
        }
        if (!errorEncountered)
        {
          foreach (Schema schema in (IEnumerable<Schema>) schemaCollection)
          {
            if (SchemaManager.UpdateErrorCollectionAndCheckForMaxErrors(errorCollection, schema.ValidateSchema(), ref errorEncountered))
              return (IList<EdmSchemaError>) errorCollection;
          }
        }
      }
      return (IList<EdmSchemaError>) errorCollection;
    }

    internal static bool TryGetSchemaVersion(
      XmlReader reader,
      out double version,
      out DataSpace dataSpace)
    {
      if (!reader.EOF && reader.NodeType != XmlNodeType.Element)
      {
        while (reader.Read() && reader.NodeType != XmlNodeType.Element)
          ;
      }
      if (!reader.EOF && (reader.LocalName == "Schema" || reader.LocalName == "Mapping"))
        return SchemaManager.TryGetSchemaVersion(reader.NamespaceURI, out version, out dataSpace);
      version = 0.0;
      dataSpace = DataSpace.OSpace;
      return false;
    }

    internal static bool TryGetSchemaVersion(
      string xmlNamespaceName,
      out double version,
      out DataSpace dataSpace)
    {
      switch (xmlNamespaceName)
      {
        case "http://schemas.microsoft.com/ado/2006/04/edm":
          version = 1.0;
          dataSpace = DataSpace.CSpace;
          return true;
        case "http://schemas.microsoft.com/ado/2007/05/edm":
          version = 1.1;
          dataSpace = DataSpace.CSpace;
          return true;
        case "http://schemas.microsoft.com/ado/2008/09/edm":
          version = 2.0;
          dataSpace = DataSpace.CSpace;
          return true;
        case "http://schemas.microsoft.com/ado/2009/11/edm":
          version = 3.0;
          dataSpace = DataSpace.CSpace;
          return true;
        case "http://schemas.microsoft.com/ado/2006/04/edm/ssdl":
          version = 1.0;
          dataSpace = DataSpace.SSpace;
          return true;
        case "http://schemas.microsoft.com/ado/2009/02/edm/ssdl":
          version = 2.0;
          dataSpace = DataSpace.SSpace;
          return true;
        case "http://schemas.microsoft.com/ado/2009/11/edm/ssdl":
          version = 3.0;
          dataSpace = DataSpace.SSpace;
          return true;
        case "urn:schemas-microsoft-com:windows:storage:mapping:CS":
          version = 1.0;
          dataSpace = DataSpace.CSSpace;
          return true;
        case "http://schemas.microsoft.com/ado/2008/09/mapping/cs":
          version = 2.0;
          dataSpace = DataSpace.CSSpace;
          return true;
        case "http://schemas.microsoft.com/ado/2009/11/mapping/cs":
          version = 3.0;
          dataSpace = DataSpace.CSSpace;
          return true;
        default:
          version = 0.0;
          dataSpace = DataSpace.OSpace;
          return false;
      }
    }

    private static bool CheckIsSameVersion(
      Schema schemaToBeAdded,
      IEnumerable<Schema> schemaCollection,
      List<EdmSchemaError> errorCollection)
    {
      if (schemaToBeAdded.SchemaVersion != 0.0 && schemaCollection.Count<Schema>() > 0 && schemaCollection.Any<Schema>((Func<Schema, bool>) (s =>
      {
        if (s.SchemaVersion != 0.0)
          return s.SchemaVersion != schemaToBeAdded.SchemaVersion;
        return false;
      })))
        errorCollection.Add(new EdmSchemaError(Strings.CannotLoadDifferentVersionOfSchemaInTheSameItemCollection, 194, EdmSchemaErrorSeverity.Error));
      return true;
    }

    public double SchemaVersion
    {
      get
      {
        return this.effectiveSchemaVersion;
      }
    }

    public void AddSchema(Schema schema)
    {
      if (this._namespaceLookUpTable.Count == 0 && schema.DataModel != SchemaDataModelOption.ProviderManifestModel && this.PrimitiveSchema.Namespace != null)
        this._namespaceLookUpTable.Add(this.PrimitiveSchema.Namespace);
      this._namespaceLookUpTable.Add(schema.Namespace);
    }

    public bool TryResolveType(string namespaceName, string typeName, out SchemaType schemaType)
    {
      string key = string.IsNullOrEmpty(namespaceName) ? typeName : namespaceName + "." + typeName;
      schemaType = this.SchemaTypes.LookUpEquivalentKey(key);
      return schemaType != null;
    }

    public bool IsValidNamespaceName(string namespaceName)
    {
      return this._namespaceLookUpTable.Contains(namespaceName);
    }

    internal static bool TryGetBaseUri(XmlReader xmlReader, out string location)
    {
      string baseUri = xmlReader.BaseURI;
      Uri result = (Uri) null;
      if (!string.IsNullOrEmpty(baseUri) && Uri.TryCreate(baseUri, UriKind.Absolute, out result) && result.Scheme == "file")
      {
        location = Helper.GetFileNameFromUri(result);
        return true;
      }
      location = (string) null;
      return false;
    }

    private static bool UpdateErrorCollectionAndCheckForMaxErrors(
      List<EdmSchemaError> errorCollection,
      IList<EdmSchemaError> newErrors,
      ref bool errorEncountered)
    {
      if (!errorEncountered && !MetadataHelper.CheckIfAllErrorsAreWarnings(newErrors))
        errorEncountered = true;
      errorCollection.AddRange((IEnumerable<EdmSchemaError>) newErrors);
      return errorEncountered && errorCollection.Where<EdmSchemaError>((Func<EdmSchemaError, bool>) (e => e.Severity == EdmSchemaErrorSeverity.Error)).Count<EdmSchemaError>() > 100;
    }

    internal SchemaElementLookUpTable<SchemaType> SchemaTypes
    {
      get
      {
        return this._schemaTypes;
      }
    }

    internal DbProviderManifest GetProviderManifest(
      System.Action<string, ErrorCode, EdmSchemaErrorSeverity> addError)
    {
      if (this._providerManifest == null)
        this._providerManifest = this._providerManifestNeeded(addError);
      return this._providerManifest;
    }

    internal SchemaDataModelOption DataModel
    {
      get
      {
        return this._dataModel;
      }
    }

    internal void EnsurePrimitiveSchemaIsLoaded(double forSchemaVersion)
    {
      if (this._primitiveSchema != null)
        return;
      this.effectiveSchemaVersion = forSchemaVersion;
      this._primitiveSchema = new PrimitiveSchema(this);
    }

    internal PrimitiveSchema PrimitiveSchema
    {
      get
      {
        return this._primitiveSchema;
      }
    }

    internal AttributeValueNotification ProviderNotification
    {
      get
      {
        return this._providerNotification;
      }
    }

    internal AttributeValueNotification ProviderManifestTokenNotification
    {
      get
      {
        return this._providerManifestTokenNotification;
      }
    }
  }
}
