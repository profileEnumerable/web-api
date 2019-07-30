// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.Schema
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Metadata.Edm.Provider;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security;
using System.Xml;
using System.Xml.Schema;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  [DebuggerDisplay("Namespace={Namespace}, PublicKeyToken={PublicKeyToken}, Version={Version}")]
  internal class Schema : SchemaElement
  {
    private List<EdmSchemaError> _errors = new List<EdmSchemaError>();
    private const int RootDepth = 2;
    private List<Function> _functions;
    private AliasResolver _aliasResolver;
    private string _location;
    protected string _namespaceName;
    private List<SchemaType> _schemaTypes;
    private int _depth;
    private double _schemaVersion;
    private readonly SchemaManager _schemaManager;
    private bool? _useStrongSpatialTypes;
    private HashSet<string> _validatableXmlNamespaces;
    private HashSet<string> _parseableXmlNamespaces;
    private MetadataProperty _schemaSourceProperty;

    public Schema(SchemaManager schemaManager)
      : base((SchemaElement) null, (IDbDependencyResolver) null)
    {
      this._schemaManager = schemaManager;
      this._errors = new List<EdmSchemaError>();
    }

    internal IList<EdmSchemaError> Resolve()
    {
      this.ResolveTopLevelNames();
      if (this._errors.Count != 0)
        return (IList<EdmSchemaError>) this.ResetErrors();
      this.ResolveSecondLevelNames();
      return (IList<EdmSchemaError>) this.ResetErrors();
    }

    internal IList<EdmSchemaError> ValidateSchema()
    {
      this.Validate();
      return (IList<EdmSchemaError>) this.ResetErrors();
    }

    internal void AddError(EdmSchemaError error)
    {
      this._errors.Add(error);
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    internal IList<EdmSchemaError> Parse(
      XmlReader sourceReader,
      string sourceLocation)
    {
      try
      {
        XmlReaderSettings xmlReaderSettings = this.CreateXmlReaderSettings();
        return this.InternalParse(XmlReader.Create(sourceReader, xmlReaderSettings), sourceLocation);
      }
      catch (IOException ex)
      {
        this.AddError(ErrorCode.IOException, EdmSchemaErrorSeverity.Error, sourceReader, (object) ex);
      }
      return (IList<EdmSchemaError>) this.ResetErrors();
    }

    private IList<EdmSchemaError> InternalParse(
      XmlReader sourceReader,
      string sourceLocation)
    {
      this.Schema = this;
      this.Location = sourceLocation;
      try
      {
        if (sourceReader.NodeType != XmlNodeType.Element)
        {
          while (sourceReader.Read() && sourceReader.NodeType != XmlNodeType.Element)
            ;
        }
        this.GetPositionInfo(sourceReader);
        List<string> schemaNamespaces = System.Data.Entity.Core.SchemaObjectModel.Schema.SomSchemaSetHelper.GetPrimarySchemaNamespaces(this.DataModel);
        if (sourceReader.EOF)
        {
          if (sourceLocation != null)
            this.AddError(ErrorCode.EmptyFile, EdmSchemaErrorSeverity.Error, (object) Strings.EmptyFile((object) sourceLocation));
          else
            this.AddError(ErrorCode.EmptyFile, EdmSchemaErrorSeverity.Error, (object) Strings.EmptySchemaTextReader);
        }
        else if (!schemaNamespaces.Contains(sourceReader.NamespaceURI))
        {
          Func<object, object, object, string> func = new Func<object, object, object, string>(Strings.UnexpectedRootElement);
          if (string.IsNullOrEmpty(sourceReader.NamespaceURI))
            func = new Func<object, object, object, string>(Strings.UnexpectedRootElementNoNamespace);
          string commaDelimitedString = Helper.GetCommaDelimitedString((IEnumerable<string>) schemaNamespaces);
          this.AddError(ErrorCode.UnexpectedXmlElement, EdmSchemaErrorSeverity.Error, (object) func((object) sourceReader.NamespaceURI, (object) sourceReader.LocalName, (object) commaDelimitedString));
        }
        else
        {
          this.SchemaXmlNamespace = sourceReader.NamespaceURI;
          if (this.DataModel == SchemaDataModelOption.EntityDataModel)
            this.SchemaVersion = !(this.SchemaXmlNamespace == "http://schemas.microsoft.com/ado/2006/04/edm") ? (!(this.SchemaXmlNamespace == "http://schemas.microsoft.com/ado/2007/05/edm") ? (!(this.SchemaXmlNamespace == "http://schemas.microsoft.com/ado/2008/09/edm") ? 3.0 : 2.0) : 1.1) : 1.0;
          else if (this.DataModel == SchemaDataModelOption.ProviderDataModel)
            this.SchemaVersion = !(this.SchemaXmlNamespace == "http://schemas.microsoft.com/ado/2006/04/edm/ssdl") ? (!(this.SchemaXmlNamespace == "http://schemas.microsoft.com/ado/2009/02/edm/ssdl") ? 3.0 : 2.0) : 1.0;
          switch (sourceReader.LocalName)
          {
            case nameof (Schema):
            case "ProviderManifest":
              this.HandleTopLevelSchemaElement(sourceReader);
              sourceReader.Read();
              break;
            default:
              this.AddError(ErrorCode.UnexpectedXmlElement, EdmSchemaErrorSeverity.Error, (object) Strings.UnexpectedRootElement((object) sourceReader.NamespaceURI, (object) sourceReader.LocalName, (object) this.SchemaXmlNamespace));
              break;
          }
        }
      }
      catch (InvalidOperationException ex)
      {
        this.AddError(ErrorCode.InternalError, EdmSchemaErrorSeverity.Error, (object) ex.Message);
      }
      catch (UnauthorizedAccessException ex)
      {
        this.AddError(ErrorCode.UnauthorizedAccessException, EdmSchemaErrorSeverity.Error, sourceReader, (object) ex);
      }
      catch (IOException ex)
      {
        this.AddError(ErrorCode.IOException, EdmSchemaErrorSeverity.Error, sourceReader, (object) ex);
      }
      catch (SecurityException ex)
      {
        this.AddError(ErrorCode.SecurityError, EdmSchemaErrorSeverity.Error, sourceReader, (object) ex);
      }
      catch (XmlException ex)
      {
        this.AddError(ErrorCode.XmlError, EdmSchemaErrorSeverity.Error, sourceReader, (object) ex);
      }
      return (IList<EdmSchemaError>) this.ResetErrors();
    }

    internal static XmlReaderSettings CreateEdmStandardXmlReaderSettings()
    {
      XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
      xmlReaderSettings.CheckCharacters = true;
      xmlReaderSettings.CloseInput = false;
      xmlReaderSettings.IgnoreWhitespace = true;
      xmlReaderSettings.ConformanceLevel = ConformanceLevel.Auto;
      xmlReaderSettings.IgnoreComments = true;
      xmlReaderSettings.IgnoreProcessingInstructions = true;
      xmlReaderSettings.DtdProcessing = DtdProcessing.Prohibit;
      xmlReaderSettings.ValidationFlags &= ~XmlSchemaValidationFlags.ProcessIdentityConstraints;
      xmlReaderSettings.ValidationFlags &= ~XmlSchemaValidationFlags.ProcessSchemaLocation;
      xmlReaderSettings.ValidationFlags &= ~XmlSchemaValidationFlags.ProcessInlineSchema;
      return xmlReaderSettings;
    }

    private XmlReaderSettings CreateXmlReaderSettings()
    {
      XmlReaderSettings xmlReaderSettings = System.Data.Entity.Core.SchemaObjectModel.Schema.CreateEdmStandardXmlReaderSettings();
      xmlReaderSettings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
      xmlReaderSettings.ValidationEventHandler += new ValidationEventHandler(this.OnSchemaValidationEvent);
      xmlReaderSettings.ValidationType = ValidationType.Schema;
      XmlSchemaSet schemaSet = System.Data.Entity.Core.SchemaObjectModel.Schema.SomSchemaSetHelper.GetSchemaSet(this.DataModel);
      xmlReaderSettings.Schemas = schemaSet;
      return xmlReaderSettings;
    }

    internal void OnSchemaValidationEvent(object sender, ValidationEventArgs e)
    {
      XmlReader xmlReader = sender as XmlReader;
      if (xmlReader != null && !this.IsValidateableXmlNamespace(xmlReader.NamespaceURI, xmlReader.NodeType == XmlNodeType.Attribute) && (this.SchemaVersion == 1.0 || this.SchemaVersion == 1.1 || (xmlReader.NodeType == XmlNodeType.Attribute || e.Severity == XmlSeverityType.Warning)) || this.SchemaVersion >= 2.0 && xmlReader.NodeType == XmlNodeType.Attribute && e.Severity == XmlSeverityType.Warning)
        return;
      EdmSchemaErrorSeverity severity = EdmSchemaErrorSeverity.Error;
      if (e.Severity == XmlSeverityType.Warning)
        severity = EdmSchemaErrorSeverity.Warning;
      this.AddError(ErrorCode.XmlError, severity, e.Exception.LineNumber, e.Exception.LinePosition, (object) e.Message);
    }

    public bool IsParseableXmlNamespace(string xmlNamespaceUri, bool isAttribute)
    {
      if (string.IsNullOrEmpty(xmlNamespaceUri) && isAttribute)
        return true;
      if (this._parseableXmlNamespaces == null)
      {
        this._parseableXmlNamespaces = new HashSet<string>();
        foreach (XmlSchemaResource xmlSchemaResource in XmlSchemaResource.GetMetadataSchemaResourceMap(this.SchemaVersion).Values)
          this._parseableXmlNamespaces.Add(xmlSchemaResource.NamespaceUri);
      }
      return this._parseableXmlNamespaces.Contains(xmlNamespaceUri);
    }

    public bool IsValidateableXmlNamespace(string xmlNamespaceUri, bool isAttribute)
    {
      if (string.IsNullOrEmpty(xmlNamespaceUri) && isAttribute)
        return true;
      if (this._validatableXmlNamespaces == null)
      {
        HashSet<string> hashSet = new HashSet<string>();
        foreach (XmlSchemaResource schemaResource in XmlSchemaResource.GetMetadataSchemaResourceMap(this.SchemaVersion == 0.0 ? 3.0 : this.SchemaVersion).Values)
          System.Data.Entity.Core.SchemaObjectModel.Schema.AddAllSchemaResourceNamespaceNames(hashSet, schemaResource);
        if (this.SchemaVersion == 0.0)
          return hashSet.Contains(xmlNamespaceUri);
        this._validatableXmlNamespaces = hashSet;
      }
      return this._validatableXmlNamespaces.Contains(xmlNamespaceUri);
    }

    private static void AddAllSchemaResourceNamespaceNames(
      HashSet<string> hashSet,
      XmlSchemaResource schemaResource)
    {
      hashSet.Add(schemaResource.NamespaceUri);
      foreach (XmlSchemaResource importedSchema in schemaResource.ImportedSchemas)
        System.Data.Entity.Core.SchemaObjectModel.Schema.AddAllSchemaResourceNamespaceNames(hashSet, importedSchema);
    }

    internal override void ResolveTopLevelNames()
    {
      base.ResolveTopLevelNames();
      this.AliasResolver.ResolveNamespaces();
      foreach (SchemaElement schemaType in this.SchemaTypes)
        schemaType.ResolveTopLevelNames();
      foreach (SchemaElement function in this.Functions)
        function.ResolveTopLevelNames();
    }

    internal override void ResolveSecondLevelNames()
    {
      base.ResolveSecondLevelNames();
      foreach (SchemaElement schemaType in this.SchemaTypes)
        schemaType.ResolveSecondLevelNames();
      foreach (SchemaElement function in this.Functions)
        function.ResolveSecondLevelNames();
    }

    internal override void Validate()
    {
      if (string.IsNullOrEmpty(this.Namespace))
      {
        this.AddError(ErrorCode.MissingNamespaceAttribute, EdmSchemaErrorSeverity.Error, (object) Strings.MissingNamespaceAttribute);
      }
      else
      {
        if (!string.IsNullOrEmpty(this.Alias) && EdmItemCollection.IsSystemNamespace(this.ProviderManifest, this.Alias))
          this.AddError(ErrorCode.CannotUseSystemNamespaceAsAlias, EdmSchemaErrorSeverity.Error, (object) Strings.CannotUseSystemNamespaceAsAlias((object) this.Alias));
        if (this.ProviderManifest != null && EdmItemCollection.IsSystemNamespace(this.ProviderManifest, this.Namespace))
          this.AddError(ErrorCode.SystemNamespace, EdmSchemaErrorSeverity.Error, (object) Strings.SystemNamespaceEncountered((object) this.Namespace));
        foreach (SchemaElement schemaType in this.SchemaTypes)
          schemaType.Validate();
        foreach (Function function in this.Functions)
        {
          this.AddFunctionType(function);
          function.Validate();
        }
      }
    }

    internal string SchemaXmlNamespace { get; private set; }

    internal DbProviderManifest ProviderManifest
    {
      get
      {
        return this._schemaManager.GetProviderManifest((System.Action<string, ErrorCode, EdmSchemaErrorSeverity>) ((message, code, severity) => this.AddError(code, severity, (object) message)));
      }
    }

    internal double SchemaVersion
    {
      get
      {
        return this._schemaVersion;
      }
      set
      {
        this._schemaVersion = value;
      }
    }

    internal virtual string Alias { get; private set; }

    internal virtual string Namespace
    {
      get
      {
        return this._namespaceName;
      }
      private set
      {
        this._namespaceName = value;
      }
    }

    internal string Location
    {
      get
      {
        return this._location;
      }
      private set
      {
        this._location = value;
      }
    }

    internal MetadataProperty SchemaSource
    {
      get
      {
        if (this._schemaSourceProperty == null)
          this._schemaSourceProperty = new MetadataProperty(nameof (SchemaSource), (EdmType) EdmProviderManifest.Instance.GetPrimitiveType(PrimitiveTypeKind.String), false, this._location != null ? (object) this._location : (object) string.Empty);
        return this._schemaSourceProperty;
      }
    }

    internal List<SchemaType> SchemaTypes
    {
      get
      {
        if (this._schemaTypes == null)
          this._schemaTypes = new List<SchemaType>();
        return this._schemaTypes;
      }
    }

    public override string FQName
    {
      get
      {
        return this.Namespace;
      }
    }

    private List<Function> Functions
    {
      get
      {
        if (this._functions == null)
          this._functions = new List<Function>();
        return this._functions;
      }
    }

    protected override bool HandleElement(XmlReader reader)
    {
      if (base.HandleElement(reader))
        return true;
      if (this.CanHandleElement(reader, "EntityType"))
      {
        this.HandleEntityTypeElement(reader);
        return true;
      }
      if (this.CanHandleElement(reader, "ComplexType"))
      {
        this.HandleInlineTypeElement(reader);
        return true;
      }
      if (this.CanHandleElement(reader, "Association"))
      {
        this.HandleAssociationElement(reader);
        return true;
      }
      if (this.DataModel == SchemaDataModelOption.EntityDataModel)
      {
        if (this.CanHandleElement(reader, "Using"))
        {
          this.HandleUsingElement(reader);
          return true;
        }
        if (this.CanHandleElement(reader, "Function"))
        {
          this.HandleModelFunctionElement(reader);
          return true;
        }
        if (this.CanHandleElement(reader, "EnumType"))
        {
          this.HandleEnumTypeElement(reader);
          return true;
        }
        if (this.CanHandleElement(reader, "ValueTerm"))
        {
          this.SkipElement(reader);
          return true;
        }
        if (this.CanHandleElement(reader, "Annotations"))
        {
          this.SkipElement(reader);
          return true;
        }
      }
      if (this.DataModel == SchemaDataModelOption.EntityDataModel || this.DataModel == SchemaDataModelOption.ProviderDataModel)
      {
        if (this.CanHandleElement(reader, "EntityContainer"))
        {
          this.HandleEntityContainerTypeElement(reader);
          return true;
        }
        if (this.DataModel == SchemaDataModelOption.ProviderDataModel && this.CanHandleElement(reader, "Function"))
        {
          this.HandleFunctionElement(reader);
          return true;
        }
      }
      else
      {
        if (this.CanHandleElement(reader, "Types"))
        {
          this.SkipThroughElement(reader);
          return true;
        }
        if (this.CanHandleElement(reader, "Functions"))
        {
          this.SkipThroughElement(reader);
          return true;
        }
        if (this.CanHandleElement(reader, "Function"))
        {
          this.HandleFunctionElement(reader);
          return true;
        }
        if (this.CanHandleElement(reader, "Type"))
        {
          this.HandleTypeInformationElement(reader);
          return true;
        }
      }
      return false;
    }

    protected override bool ProhibitAttribute(string namespaceUri, string localName)
    {
      if (base.ProhibitAttribute(namespaceUri, localName))
        return true;
      return namespaceUri == null && localName == "Name" ? false : false;
    }

    protected override bool HandleAttribute(XmlReader reader)
    {
      if (this._depth == 1)
        return false;
      if (base.HandleAttribute(reader))
        return true;
      if (SchemaElement.CanHandleAttribute(reader, "Alias"))
      {
        this.HandleAliasAttribute(reader);
        return true;
      }
      if (SchemaElement.CanHandleAttribute(reader, "Namespace"))
      {
        this.HandleNamespaceAttribute(reader);
        return true;
      }
      if (SchemaElement.CanHandleAttribute(reader, "Provider"))
      {
        this.HandleProviderAttribute(reader);
        return true;
      }
      if (SchemaElement.CanHandleAttribute(reader, "ProviderManifestToken"))
      {
        this.HandleProviderManifestTokenAttribute(reader);
        return true;
      }
      if (!(reader.NamespaceURI == "http://schemas.microsoft.com/ado/2009/02/edm/annotation") || !(reader.LocalName == "UseStrongSpatialTypes"))
        return false;
      this.HandleUseStrongSpatialTypesAnnotation(reader);
      return true;
    }

    protected override void HandleAttributesComplete()
    {
      if (this._depth < 2)
        return;
      if (this._depth == 2)
        this._schemaManager.EnsurePrimitiveSchemaIsLoaded(this.SchemaVersion);
      base.HandleAttributesComplete();
    }

    protected override void SkipThroughElement(XmlReader reader)
    {
      try
      {
        ++this._depth;
        base.SkipThroughElement(reader);
      }
      finally
      {
        --this._depth;
      }
    }

    internal bool ResolveTypeName(SchemaElement usingElement, string typeName, out SchemaType type)
    {
      type = (SchemaType) null;
      string namespaceName1;
      string name;
      System.Data.Entity.Core.SchemaObjectModel.Utils.ExtractNamespaceAndName(typeName, out namespaceName1, out name);
      string alias = namespaceName1 ?? (this.ProviderManifest == null ? this._namespaceName : this.ProviderManifest.NamespaceName);
      string namespaceName2;
      if (namespaceName1 == null || !this.AliasResolver.TryResolveAlias(alias, out namespaceName2))
        namespaceName2 = alias;
      if (!this.SchemaManager.TryResolveType(namespaceName2, name, out type))
      {
        if (namespaceName1 == null)
          usingElement.AddError(ErrorCode.NotInNamespace, EdmSchemaErrorSeverity.Error, (object) Strings.NotNamespaceQualified((object) typeName));
        else if (!this.SchemaManager.IsValidNamespaceName(namespaceName2))
          usingElement.AddError(ErrorCode.BadNamespace, EdmSchemaErrorSeverity.Error, (object) Strings.BadNamespaceOrAlias((object) namespaceName1));
        else if (namespaceName2 != alias)
          usingElement.AddError(ErrorCode.NotInNamespace, EdmSchemaErrorSeverity.Error, (object) Strings.NotInNamespaceAlias((object) name, (object) namespaceName2, (object) alias));
        else
          usingElement.AddError(ErrorCode.NotInNamespace, EdmSchemaErrorSeverity.Error, (object) Strings.NotInNamespaceNoAlias((object) name, (object) namespaceName2));
        return false;
      }
      if (this.DataModel == SchemaDataModelOption.EntityDataModel || type.Schema == this || type.Schema == this.SchemaManager.PrimitiveSchema)
        return true;
      usingElement.AddError(ErrorCode.InvalidNamespaceOrAliasSpecified, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidNamespaceOrAliasSpecified((object) namespaceName1));
      return false;
    }

    internal AliasResolver AliasResolver
    {
      get
      {
        if (this._aliasResolver == null)
          this._aliasResolver = new AliasResolver(this);
        return this._aliasResolver;
      }
    }

    internal SchemaDataModelOption DataModel
    {
      get
      {
        return this.SchemaManager.DataModel;
      }
    }

    internal SchemaManager SchemaManager
    {
      get
      {
        return this._schemaManager;
      }
    }

    internal bool UseStrongSpatialTypes
    {
      get
      {
        return this._useStrongSpatialTypes ?? true;
      }
    }

    private void HandleNamespaceAttribute(XmlReader reader)
    {
      ReturnValue<string> returnValue = this.HandleDottedNameAttribute(reader, this.Namespace);
      if (!returnValue.Succeeded)
        return;
      this.Namespace = returnValue.Value;
    }

    private void HandleAliasAttribute(XmlReader reader)
    {
      this.Alias = this.HandleUndottedNameAttribute(reader, this.Alias);
    }

    private void HandleProviderAttribute(XmlReader reader)
    {
      this._schemaManager.ProviderNotification(reader.Value, (System.Action<string, ErrorCode, EdmSchemaErrorSeverity>) ((message, code, severity) => this.AddError(code, severity, reader, (object) message)));
    }

    private void HandleProviderManifestTokenAttribute(XmlReader reader)
    {
      this._schemaManager.ProviderManifestTokenNotification(reader.Value, (System.Action<string, ErrorCode, EdmSchemaErrorSeverity>) ((message, code, severity) => this.AddError(code, severity, reader, (object) message)));
    }

    private void HandleUseStrongSpatialTypesAnnotation(XmlReader reader)
    {
      bool field = false;
      if (!this.HandleBoolAttribute(reader, ref field))
        return;
      this._useStrongSpatialTypes = new bool?(field);
    }

    private void HandleUsingElement(XmlReader reader)
    {
      UsingElement usingElement = new UsingElement(this);
      usingElement.Parse(reader);
      this.AliasResolver.Add(usingElement);
    }

    private void HandleEnumTypeElement(XmlReader reader)
    {
      SchemaEnumType schemaEnumType = new SchemaEnumType(this);
      schemaEnumType.Parse(reader);
      this.TryAddType((SchemaType) schemaEnumType, true);
    }

    private void HandleTopLevelSchemaElement(XmlReader reader)
    {
      try
      {
        this._depth += 2;
        this.Parse(reader);
      }
      finally
      {
        this._depth -= 2;
      }
    }

    private void HandleEntityTypeElement(XmlReader reader)
    {
      SchemaEntityType schemaEntityType = new SchemaEntityType(this);
      schemaEntityType.Parse(reader);
      this.TryAddType((SchemaType) schemaEntityType, true);
    }

    private void HandleTypeInformationElement(XmlReader reader)
    {
      TypeElement typeElement = new TypeElement(this);
      typeElement.Parse(reader);
      this.TryAddType((SchemaType) typeElement, true);
    }

    private void HandleFunctionElement(XmlReader reader)
    {
      Function function = new Function(this);
      function.Parse(reader);
      this.Functions.Add(function);
    }

    private void HandleModelFunctionElement(XmlReader reader)
    {
      ModelFunction modelFunction = new ModelFunction(this);
      modelFunction.Parse(reader);
      this.Functions.Add((Function) modelFunction);
    }

    private void HandleAssociationElement(XmlReader reader)
    {
      Relationship relationship = new Relationship(this, RelationshipKind.Association);
      relationship.Parse(reader);
      this.TryAddType((SchemaType) relationship, true);
    }

    private void HandleInlineTypeElement(XmlReader reader)
    {
      SchemaComplexType schemaComplexType = new SchemaComplexType(this);
      schemaComplexType.Parse(reader);
      this.TryAddType((SchemaType) schemaComplexType, true);
    }

    private void HandleEntityContainerTypeElement(XmlReader reader)
    {
      EntityContainer entityContainer = new EntityContainer(this);
      entityContainer.Parse(reader);
      this.TryAddContainer((SchemaType) entityContainer, true);
    }

    private List<EdmSchemaError> ResetErrors()
    {
      List<EdmSchemaError> errors = this._errors;
      this._errors = new List<EdmSchemaError>();
      return errors;
    }

    protected void TryAddType(SchemaType schemaType, bool doNotAddErrorForEmptyName)
    {
      this.SchemaManager.SchemaTypes.Add(schemaType, doNotAddErrorForEmptyName, new Func<object, string>(Strings.TypeNameAlreadyDefinedDuplicate));
      this.SchemaTypes.Add(schemaType);
    }

    protected void TryAddContainer(SchemaType schemaType, bool doNotAddErrorForEmptyName)
    {
      this.SchemaManager.SchemaTypes.Add(schemaType, doNotAddErrorForEmptyName, new Func<object, string>(Strings.EntityContainerAlreadyExists));
      this.SchemaTypes.Add(schemaType);
    }

    protected void AddFunctionType(Function function)
    {
      string str = this.DataModel == SchemaDataModelOption.EntityDataModel ? "Conceptual" : "Storage";
      if (this.SchemaVersion >= 2.0 && this.SchemaManager.SchemaTypes.ContainsKey(function.FQName))
        function.AddError(ErrorCode.AlreadyDefined, EdmSchemaErrorSeverity.Error, (object) Strings.AmbiguousFunctionAndType((object) function.FQName, (object) str));
      else if (this.SchemaManager.SchemaTypes.TryAdd((SchemaType) function) != AddErrorKind.Succeeded)
        function.AddError(ErrorCode.AlreadyDefined, EdmSchemaErrorSeverity.Error, (object) Strings.AmbiguousFunctionOverload((object) function.FQName, (object) str));
      else
        this.SchemaTypes.Add((SchemaType) function);
    }

    private static class SomSchemaSetHelper
    {
      private static readonly Memoizer<SchemaDataModelOption, XmlSchemaSet> _cachedSchemaSets = new Memoizer<SchemaDataModelOption, XmlSchemaSet>(new Func<SchemaDataModelOption, XmlSchemaSet>(System.Data.Entity.Core.SchemaObjectModel.Schema.SomSchemaSetHelper.ComputeSchemaSet), (IEqualityComparer<SchemaDataModelOption>) EqualityComparer<SchemaDataModelOption>.Default);

      internal static List<string> GetPrimarySchemaNamespaces(SchemaDataModelOption dataModel)
      {
        List<string> stringList = new List<string>();
        switch (dataModel)
        {
          case SchemaDataModelOption.EntityDataModel:
            stringList.Add("http://schemas.microsoft.com/ado/2006/04/edm");
            stringList.Add("http://schemas.microsoft.com/ado/2007/05/edm");
            stringList.Add("http://schemas.microsoft.com/ado/2008/09/edm");
            stringList.Add("http://schemas.microsoft.com/ado/2009/11/edm");
            break;
          case SchemaDataModelOption.ProviderDataModel:
            stringList.Add("http://schemas.microsoft.com/ado/2006/04/edm/ssdl");
            stringList.Add("http://schemas.microsoft.com/ado/2009/02/edm/ssdl");
            stringList.Add("http://schemas.microsoft.com/ado/2009/11/edm/ssdl");
            break;
          default:
            stringList.Add("http://schemas.microsoft.com/ado/2006/04/edm/providermanifest");
            break;
        }
        return stringList;
      }

      internal static XmlSchemaSet GetSchemaSet(SchemaDataModelOption dataModel)
      {
        return System.Data.Entity.Core.SchemaObjectModel.Schema.SomSchemaSetHelper._cachedSchemaSets.Evaluate(dataModel);
      }

      private static XmlSchemaSet ComputeSchemaSet(SchemaDataModelOption dataModel)
      {
        List<string> schemaNamespaces = System.Data.Entity.Core.SchemaObjectModel.Schema.SomSchemaSetHelper.GetPrimarySchemaNamespaces(dataModel);
        XmlSchemaSet schemaSet = new XmlSchemaSet();
        schemaSet.XmlResolver = (XmlResolver) null;
        Dictionary<string, XmlSchemaResource> schemaResourceMap = XmlSchemaResource.GetMetadataSchemaResourceMap(3.0);
        HashSet<string> schemasAlreadyAdded = new HashSet<string>();
        foreach (string index in schemaNamespaces)
        {
          XmlSchemaResource schemaResource = schemaResourceMap[index];
          System.Data.Entity.Core.SchemaObjectModel.Schema.SomSchemaSetHelper.AddXmlSchemaToSet(schemaSet, schemaResource, schemasAlreadyAdded);
        }
        schemaSet.Compile();
        return schemaSet;
      }

      private static void AddXmlSchemaToSet(
        XmlSchemaSet schemaSet,
        XmlSchemaResource schemaResource,
        HashSet<string> schemasAlreadyAdded)
      {
        foreach (XmlSchemaResource importedSchema in schemaResource.ImportedSchemas)
          System.Data.Entity.Core.SchemaObjectModel.Schema.SomSchemaSetHelper.AddXmlSchemaToSet(schemaSet, importedSchema, schemasAlreadyAdded);
        if (schemasAlreadyAdded.Contains(schemaResource.NamespaceUri))
          return;
        XmlSchema schema = XmlSchema.Read(System.Data.Entity.Core.SchemaObjectModel.Schema.SomSchemaSetHelper.GetResourceStream(schemaResource.ResourceName), (ValidationEventHandler) null);
        schemaSet.Add(schema);
        schemasAlreadyAdded.Add(schemaResource.NamespaceUri);
      }

      private static Stream GetResourceStream(string resourceName)
      {
        return typeof (System.Data.Entity.Core.SchemaObjectModel.Schema).Assembly().GetManifestResourceStream(resourceName);
      }
    }
  }
}
