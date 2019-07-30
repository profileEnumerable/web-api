// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.SchemaElement
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Resources;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  [DebuggerDisplay("Name={Name}")]
  internal abstract class SchemaElement
  {
    internal const string XmlNamespaceNamespace = "http://www.w3.org/2000/xmlns/";
    protected const int MaxValueVersionComponent = 32767;
    private System.Data.Entity.Core.SchemaObjectModel.Schema _schema;
    private int _lineNumber;
    private int _linePosition;
    private string _name;
    private List<MetadataProperty> _otherContent;
    private readonly IDbDependencyResolver _resolver;

    internal int LineNumber
    {
      get
      {
        return this._lineNumber;
      }
    }

    internal int LinePosition
    {
      get
      {
        return this._linePosition;
      }
    }

    public virtual string Name
    {
      get
      {
        return this._name;
      }
      set
      {
        this._name = value;
      }
    }

    internal DocumentationElement Documentation { get; set; }

    internal SchemaElement ParentElement { get; private set; }

    internal System.Data.Entity.Core.SchemaObjectModel.Schema Schema
    {
      get
      {
        return this._schema;
      }
      set
      {
        this._schema = value;
      }
    }

    public virtual string FQName
    {
      get
      {
        return this.Name;
      }
    }

    public virtual string Identity
    {
      get
      {
        return this.Name;
      }
    }

    public List<MetadataProperty> OtherContent
    {
      get
      {
        if (this._otherContent == null)
          this._otherContent = new List<MetadataProperty>();
        return this._otherContent;
      }
    }

    internal virtual void Validate()
    {
    }

    internal void AddError(
      ErrorCode errorCode,
      EdmSchemaErrorSeverity severity,
      int lineNumber,
      int linePosition,
      object message)
    {
      this.AddError(errorCode, severity, this.SchemaLocation, lineNumber, linePosition, message);
    }

    internal void AddError(
      ErrorCode errorCode,
      EdmSchemaErrorSeverity severity,
      XmlReader reader,
      object message)
    {
      int lineNumber;
      int linePosition;
      SchemaElement.GetPositionInfo(reader, out lineNumber, out linePosition);
      this.AddError(errorCode, severity, this.SchemaLocation, lineNumber, linePosition, message);
    }

    internal void AddError(ErrorCode errorCode, EdmSchemaErrorSeverity severity, object message)
    {
      this.AddError(errorCode, severity, this.SchemaLocation, this.LineNumber, this.LinePosition, message);
    }

    internal void AddError(
      ErrorCode errorCode,
      EdmSchemaErrorSeverity severity,
      SchemaElement element,
      object message)
    {
      this.AddError(errorCode, severity, element.Schema.Location, element.LineNumber, element.LinePosition, message);
    }

    internal void Parse(XmlReader reader)
    {
      this.GetPositionInfo(reader);
      bool flag1 = !reader.IsEmptyElement;
      for (bool flag2 = reader.MoveToFirstAttribute(); flag2; flag2 = reader.MoveToNextAttribute())
        this.ParseAttribute(reader);
      this.HandleAttributesComplete();
      bool flag3 = !flag1;
      bool flag4 = false;
      while (!flag3)
      {
        if (flag4)
        {
          flag4 = false;
          reader.Skip();
          if (reader.EOF)
            break;
        }
        else if (!reader.Read())
          break;
        switch (reader.NodeType)
        {
          case XmlNodeType.Element:
            flag4 = this.ParseElement(reader);
            continue;
          case XmlNodeType.Text:
          case XmlNodeType.CDATA:
          case XmlNodeType.SignificantWhitespace:
            this.ParseText(reader);
            continue;
          case XmlNodeType.EntityReference:
          case XmlNodeType.DocumentType:
            flag4 = true;
            continue;
          case XmlNodeType.ProcessingInstruction:
          case XmlNodeType.Comment:
          case XmlNodeType.Notation:
          case XmlNodeType.Whitespace:
          case XmlNodeType.XmlDeclaration:
            continue;
          case XmlNodeType.EndElement:
            flag3 = true;
            continue;
          default:
            this.AddError(ErrorCode.UnexpectedXmlNodeType, EdmSchemaErrorSeverity.Error, reader, (object) Strings.UnexpectedXmlNodeType((object) reader.NodeType));
            flag4 = true;
            continue;
        }
      }
      this.HandleChildElementsComplete();
      if (!reader.EOF || reader.Depth <= 0)
        return;
      this.AddError(ErrorCode.MalformedXml, EdmSchemaErrorSeverity.Error, 0, 0, (object) Strings.MalformedXml((object) this.LineNumber, (object) this.LinePosition));
    }

    internal void GetPositionInfo(XmlReader reader)
    {
      SchemaElement.GetPositionInfo(reader, out this._lineNumber, out this._linePosition);
    }

    internal static void GetPositionInfo(
      XmlReader reader,
      out int lineNumber,
      out int linePosition)
    {
      IXmlLineInfo xmlLineInfo = reader as IXmlLineInfo;
      if (xmlLineInfo != null && xmlLineInfo.HasLineInfo())
      {
        lineNumber = xmlLineInfo.LineNumber;
        linePosition = xmlLineInfo.LinePosition;
      }
      else
      {
        lineNumber = 0;
        linePosition = 0;
      }
    }

    internal virtual void ResolveTopLevelNames()
    {
    }

    internal virtual void ResolveSecondLevelNames()
    {
    }

    internal SchemaElement(SchemaElement parentElement, IDbDependencyResolver resolver = null)
    {
      this._resolver = resolver ?? DbConfiguration.DependencyResolver;
      if (parentElement == null)
        return;
      this.ParentElement = parentElement;
      for (SchemaElement schemaElement = parentElement; schemaElement != null; schemaElement = schemaElement.ParentElement)
      {
        System.Data.Entity.Core.SchemaObjectModel.Schema schema = schemaElement as System.Data.Entity.Core.SchemaObjectModel.Schema;
        if (schema != null)
        {
          this.Schema = schema;
          break;
        }
      }
      if (this.Schema == null)
        throw new InvalidOperationException(Strings.AllElementsMustBeInSchema);
    }

    internal SchemaElement(
      SchemaElement parentElement,
      string name,
      IDbDependencyResolver resolver = null)
      : this(parentElement, resolver)
    {
      this._name = name;
    }

    protected virtual void HandleAttributesComplete()
    {
    }

    protected virtual void HandleChildElementsComplete()
    {
    }

    protected string HandleUndottedNameAttribute(XmlReader reader, string field)
    {
      string name = field;
      if (!Utils.GetUndottedName(this.Schema, reader, out name))
        return name;
      return name;
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "field")]
    protected ReturnValue<string> HandleDottedNameAttribute(
      XmlReader reader,
      string field)
    {
      ReturnValue<string> returnValue = new ReturnValue<string>();
      string name;
      if (!Utils.GetDottedName(this.Schema, reader, out name))
        return returnValue;
      returnValue.Value = name;
      return returnValue;
    }

    internal bool HandleIntAttribute(XmlReader reader, ref int field)
    {
      int num;
      if (!Utils.GetInt(this.Schema, reader, out num))
        return false;
      field = num;
      return true;
    }

    internal bool HandleByteAttribute(XmlReader reader, ref byte field)
    {
      byte num;
      if (!Utils.GetByte(this.Schema, reader, out num))
        return false;
      field = num;
      return true;
    }

    internal bool HandleBoolAttribute(XmlReader reader, ref bool field)
    {
      bool flag;
      if (!Utils.GetBool(this.Schema, reader, out flag))
        return false;
      field = flag;
      return true;
    }

    protected virtual void SkipThroughElement(XmlReader reader)
    {
      this.Parse(reader);
    }

    protected virtual void SkipElement(XmlReader reader)
    {
      using (XmlReader xmlReader = reader.ReadSubtree())
      {
        do
          ;
        while (xmlReader.Read());
      }
    }

    protected string SchemaLocation
    {
      get
      {
        if (this.Schema != null)
          return this.Schema.Location;
        return (string) null;
      }
    }

    protected virtual bool HandleText(XmlReader reader)
    {
      return false;
    }

    internal virtual SchemaElement Clone(SchemaElement parentElement)
    {
      throw Error.NotImplemented();
    }

    private void HandleDocumentationElement(XmlReader reader)
    {
      this.Documentation = new DocumentationElement(this);
      this.Documentation.Parse(reader);
    }

    protected virtual void HandleNameAttribute(XmlReader reader)
    {
      this.Name = this.HandleUndottedNameAttribute(reader, this.Name);
    }

    private void AddError(
      ErrorCode errorCode,
      EdmSchemaErrorSeverity severity,
      string sourceLocation,
      int lineNumber,
      int linePosition,
      object message)
    {
      string message1 = message as string;
      EdmSchemaError error;
      if (message1 != null)
      {
        error = new EdmSchemaError(message1, (int) errorCode, severity, sourceLocation, lineNumber, linePosition);
      }
      else
      {
        Exception exception = message as Exception;
        error = exception == null ? new EdmSchemaError(message.ToString(), (int) errorCode, severity, sourceLocation, lineNumber, linePosition) : new EdmSchemaError(exception.Message, (int) errorCode, severity, sourceLocation, lineNumber, linePosition, exception);
      }
      this.Schema.AddError(error);
    }

    private void ParseAttribute(XmlReader reader)
    {
      string namespaceUri = reader.NamespaceURI;
      if (namespaceUri == "http://schemas.microsoft.com/ado/2009/02/edm/annotation" && reader.LocalName == "UseStrongSpatialTypes" && (!this.ProhibitAttribute(namespaceUri, reader.LocalName) && this.HandleAttribute(reader)))
        return;
      if (!this.Schema.IsParseableXmlNamespace(namespaceUri, true))
      {
        this.AddOtherContent(reader);
      }
      else
      {
        if (!this.ProhibitAttribute(namespaceUri, reader.LocalName) && this.HandleAttribute(reader) || reader.SchemaInfo != null && reader.SchemaInfo.Validity == XmlSchemaValidity.Invalid || !string.IsNullOrEmpty(namespaceUri) && !this.Schema.IsParseableXmlNamespace(namespaceUri, true))
          return;
        this.AddError(ErrorCode.UnexpectedXmlAttribute, EdmSchemaErrorSeverity.Error, reader, (object) Strings.UnexpectedXmlAttribute((object) reader.Name));
      }
    }

    protected virtual bool ProhibitAttribute(string namespaceUri, string localName)
    {
      return false;
    }

    internal static bool CanHandleAttribute(XmlReader reader, string localName)
    {
      if (reader.NamespaceURI.Length == 0)
        return reader.LocalName == localName;
      return false;
    }

    protected virtual bool HandleAttribute(XmlReader reader)
    {
      if (!SchemaElement.CanHandleAttribute(reader, "Name"))
        return false;
      this.HandleNameAttribute(reader);
      return true;
    }

    private bool AddOtherContent(XmlReader reader)
    {
      int lineNumber;
      int linePosition;
      SchemaElement.GetPositionInfo(reader, out lineNumber, out linePosition);
      MetadataProperty property;
      if (reader.NodeType == XmlNodeType.Element)
      {
        if (this._schema.SchemaVersion == 1.0 || this._schema.SchemaVersion == 1.1)
          return true;
        if (this._schema.SchemaVersion >= 2.0 && reader.NamespaceURI == "http://schemas.microsoft.com/ado/2006/04/codegeneration")
        {
          this.AddError(ErrorCode.NoCodeGenNamespaceInStructuralAnnotation, EdmSchemaErrorSeverity.Error, lineNumber, linePosition, (object) Strings.NoCodeGenNamespaceInStructuralAnnotation((object) "http://schemas.microsoft.com/ado/2006/04/codegeneration"));
          return true;
        }
        using (XmlReader xmlReader = reader.ReadSubtree())
        {
          xmlReader.Read();
          using (StringReader stringReader = new StringReader(xmlReader.ReadOuterXml()))
          {
            XElement xelement = XElement.Load((TextReader) stringReader);
            property = SchemaElement.CreateMetadataPropertyFromXmlElement(xelement.Name.NamespaceName, xelement.Name.LocalName, xelement);
          }
        }
      }
      else
      {
        if (reader.NamespaceURI == "http://www.w3.org/2000/xmlns/")
          return true;
        property = this.CreateMetadataPropertyFromXmlAttribute(reader.NamespaceURI, reader.LocalName, reader.Value);
      }
      if (!this.OtherContent.Exists((Predicate<MetadataProperty>) (mp => mp.Identity == property.Identity)))
        this.OtherContent.Add(property);
      else
        this.AddError(ErrorCode.AlreadyDefined, EdmSchemaErrorSeverity.Error, lineNumber, linePosition, (object) Strings.DuplicateAnnotation((object) property.Identity, (object) this.FQName));
      return false;
    }

    internal static MetadataProperty CreateMetadataPropertyFromXmlElement(
      string xmlNamespaceUri,
      string elementName,
      XElement value)
    {
      return MetadataProperty.CreateAnnotation(xmlNamespaceUri + ":" + elementName, (object) value);
    }

    internal MetadataProperty CreateMetadataPropertyFromXmlAttribute(
      string xmlNamespaceUri,
      string attributeName,
      string value)
    {
      Func<IMetadataAnnotationSerializer> service = this._resolver.GetService<Func<IMetadataAnnotationSerializer>>((object) attributeName);
      object obj = service == null ? (object) value : service().Deserialize(attributeName, value);
      return MetadataProperty.CreateAnnotation(xmlNamespaceUri + ":" + attributeName, obj);
    }

    private bool ParseElement(XmlReader reader)
    {
      string namespaceUri = reader.NamespaceURI;
      if (!this.Schema.IsParseableXmlNamespace(namespaceUri, true) && this.ParentElement != null)
        return this.AddOtherContent(reader);
      if (this.HandleElement(reader))
        return false;
      if (string.IsNullOrEmpty(namespaceUri) || this.Schema.IsParseableXmlNamespace(reader.NamespaceURI, false))
        this.AddError(ErrorCode.UnexpectedXmlElement, EdmSchemaErrorSeverity.Error, reader, (object) Strings.UnexpectedXmlElement((object) reader.Name));
      return true;
    }

    protected bool CanHandleElement(XmlReader reader, string localName)
    {
      if (reader.NamespaceURI == this.Schema.SchemaXmlNamespace)
        return reader.LocalName == localName;
      return false;
    }

    protected virtual bool HandleElement(XmlReader reader)
    {
      if (!this.CanHandleElement(reader, "Documentation"))
        return false;
      this.HandleDocumentationElement(reader);
      return true;
    }

    private void ParseText(XmlReader reader)
    {
      if (this.HandleText(reader) || reader.Value != null && reader.Value.Trim().Length == 0)
        return;
      this.AddError(ErrorCode.TextNotAllowed, EdmSchemaErrorSeverity.Error, reader, (object) Strings.TextNotAllowed((object) reader.Value));
    }

    [Conditional("DEBUG")]
    internal static void AssertReaderConsidersSchemaInvalid(XmlReader reader)
    {
    }
  }
}
