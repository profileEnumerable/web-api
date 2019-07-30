// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.FacetDescriptionElement
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Xml;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal abstract class FacetDescriptionElement : SchemaElement
  {
    private int? _minValue;
    private int? _maxValue;
    private bool _isConstant;
    private FacetDescription _facetDescription;

    public FacetDescriptionElement(TypeElement type, string name)
      : base((SchemaElement) type, name, (IDbDependencyResolver) null)
    {
    }

    protected override bool ProhibitAttribute(string namespaceUri, string localName)
    {
      if (base.ProhibitAttribute(namespaceUri, localName))
        return true;
      return namespaceUri == null && localName == "Name" ? false : false;
    }

    protected override bool HandleAttribute(XmlReader reader)
    {
      if (base.HandleAttribute(reader))
        return true;
      if (SchemaElement.CanHandleAttribute(reader, "Minimum"))
      {
        this.HandleMinimumAttribute(reader);
        return true;
      }
      if (SchemaElement.CanHandleAttribute(reader, "Maximum"))
      {
        this.HandleMaximumAttribute(reader);
        return true;
      }
      if (SchemaElement.CanHandleAttribute(reader, "DefaultValue"))
      {
        this.HandleDefaultAttribute(reader);
        return true;
      }
      if (!SchemaElement.CanHandleAttribute(reader, "Constant"))
        return false;
      this.HandleConstantAttribute(reader);
      return true;
    }

    protected void HandleMinimumAttribute(XmlReader reader)
    {
      int field = -1;
      if (!this.HandleIntAttribute(reader, ref field))
        return;
      this._minValue = new int?(field);
    }

    protected void HandleMaximumAttribute(XmlReader reader)
    {
      int field = -1;
      if (!this.HandleIntAttribute(reader, ref field))
        return;
      this._maxValue = new int?(field);
    }

    protected abstract void HandleDefaultAttribute(XmlReader reader);

    protected void HandleConstantAttribute(XmlReader reader)
    {
      bool field = false;
      if (!this.HandleBoolAttribute(reader, ref field))
        return;
      this._isConstant = field;
    }

    public abstract EdmType FacetType { get; }

    public int? MinValue
    {
      get
      {
        return this._minValue;
      }
    }

    public int? MaxValue
    {
      get
      {
        return this._maxValue;
      }
    }

    public object DefaultValue { get; set; }

    public FacetDescription FacetDescription
    {
      get
      {
        return this._facetDescription;
      }
    }

    internal void CreateAndValidateFacetDescription(string declaringTypeName)
    {
      this._facetDescription = new FacetDescription(this.Name, this.FacetType, this.MinValue, this.MaxValue, this.DefaultValue, this._isConstant, declaringTypeName);
    }
  }
}
