// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.DocumentationElement
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Resources;
using System.Xml;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal sealed class DocumentationElement : SchemaElement
  {
    private readonly Documentation _metdataDocumentation = new Documentation();

    public DocumentationElement(SchemaElement parentElement)
      : base(parentElement, (IDbDependencyResolver) null)
    {
    }

    public Documentation MetadataDocumentation
    {
      get
      {
        this._metdataDocumentation.SetReadOnly();
        return this._metdataDocumentation;
      }
    }

    protected override bool HandleElement(XmlReader reader)
    {
      if (base.HandleElement(reader))
        return true;
      if (this.CanHandleElement(reader, "Summary"))
      {
        this.HandleSummaryElement(reader);
        return true;
      }
      if (!this.CanHandleElement(reader, "LongDescription"))
        return false;
      this.HandleLongDescriptionElement(reader);
      return true;
    }

    protected override bool HandleText(XmlReader reader)
    {
      if (!string.IsNullOrWhiteSpace(reader.Value))
        this.AddError(ErrorCode.UnexpectedXmlElement, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidDocumentationBothTextAndStructure);
      return true;
    }

    private void HandleSummaryElement(XmlReader reader)
    {
      TextElement textElement = new TextElement((SchemaElement) this);
      textElement.Parse(reader);
      this._metdataDocumentation.Summary = textElement.Value;
    }

    private void HandleLongDescriptionElement(XmlReader reader)
    {
      TextElement textElement = new TextElement((SchemaElement) this);
      textElement.Parse(reader);
      this._metdataDocumentation.LongDescription = textElement.Value;
    }
  }
}
