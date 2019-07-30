// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.TypeElement
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Xml;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal class TypeElement : SchemaType
  {
    private readonly PrimitiveType _primitiveType = new PrimitiveType();
    private readonly List<FacetDescriptionElement> _facetDescriptions = new List<FacetDescriptionElement>();

    public TypeElement(Schema parent)
      : base(parent)
    {
      this._primitiveType.NamespaceName = this.Schema.Namespace;
    }

    protected override bool HandleElement(XmlReader reader)
    {
      if (base.HandleElement(reader))
        return true;
      if (this.CanHandleElement(reader, "FacetDescriptions"))
      {
        this.SkipThroughElement(reader);
        return true;
      }
      if (this.CanHandleElement(reader, "Precision"))
      {
        this.HandlePrecisionElement(reader);
        return true;
      }
      if (this.CanHandleElement(reader, "Scale"))
      {
        this.HandleScaleElement(reader);
        return true;
      }
      if (this.CanHandleElement(reader, "MaxLength"))
      {
        this.HandleMaxLengthElement(reader);
        return true;
      }
      if (this.CanHandleElement(reader, "Unicode"))
      {
        this.HandleUnicodeElement(reader);
        return true;
      }
      if (this.CanHandleElement(reader, "FixedLength"))
      {
        this.HandleFixedLengthElement(reader);
        return true;
      }
      if (this.CanHandleElement(reader, "SRID"))
      {
        this.HandleSridElement(reader);
        return true;
      }
      if (!this.CanHandleElement(reader, "IsStrict"))
        return false;
      this.HandleIsStrictElement(reader);
      return true;
    }

    protected override bool HandleAttribute(XmlReader reader)
    {
      if (base.HandleAttribute(reader))
        return true;
      if (!SchemaElement.CanHandleAttribute(reader, "PrimitiveTypeKind"))
        return false;
      this.HandlePrimitiveTypeKindAttribute(reader);
      return true;
    }

    private void HandlePrecisionElement(XmlReader reader)
    {
      ByteFacetDescriptionElement descriptionElement = new ByteFacetDescriptionElement(this, "Precision");
      descriptionElement.Parse(reader);
      this._facetDescriptions.Add((FacetDescriptionElement) descriptionElement);
    }

    private void HandleScaleElement(XmlReader reader)
    {
      ByteFacetDescriptionElement descriptionElement = new ByteFacetDescriptionElement(this, "Scale");
      descriptionElement.Parse(reader);
      this._facetDescriptions.Add((FacetDescriptionElement) descriptionElement);
    }

    private void HandleMaxLengthElement(XmlReader reader)
    {
      IntegerFacetDescriptionElement descriptionElement = new IntegerFacetDescriptionElement(this, "MaxLength");
      descriptionElement.Parse(reader);
      this._facetDescriptions.Add((FacetDescriptionElement) descriptionElement);
    }

    private void HandleUnicodeElement(XmlReader reader)
    {
      BooleanFacetDescriptionElement descriptionElement = new BooleanFacetDescriptionElement(this, "Unicode");
      descriptionElement.Parse(reader);
      this._facetDescriptions.Add((FacetDescriptionElement) descriptionElement);
    }

    private void HandleFixedLengthElement(XmlReader reader)
    {
      BooleanFacetDescriptionElement descriptionElement = new BooleanFacetDescriptionElement(this, "FixedLength");
      descriptionElement.Parse(reader);
      this._facetDescriptions.Add((FacetDescriptionElement) descriptionElement);
    }

    private void HandleSridElement(XmlReader reader)
    {
      SridFacetDescriptionElement descriptionElement = new SridFacetDescriptionElement(this, "SRID");
      descriptionElement.Parse(reader);
      this._facetDescriptions.Add((FacetDescriptionElement) descriptionElement);
    }

    private void HandleIsStrictElement(XmlReader reader)
    {
      BooleanFacetDescriptionElement descriptionElement = new BooleanFacetDescriptionElement(this, "IsStrict");
      descriptionElement.Parse(reader);
      this._facetDescriptions.Add((FacetDescriptionElement) descriptionElement);
    }

    private void HandlePrimitiveTypeKindAttribute(XmlReader reader)
    {
      string str = reader.Value;
      try
      {
        this._primitiveType.PrimitiveTypeKind = (PrimitiveTypeKind) Enum.Parse(typeof (PrimitiveTypeKind), str);
        this._primitiveType.BaseType = (EdmType) MetadataItem.EdmProviderManifest.GetPrimitiveType(this._primitiveType.PrimitiveTypeKind);
      }
      catch (ArgumentException ex)
      {
        this.AddError(ErrorCode.InvalidPrimitiveTypeKind, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidPrimitiveTypeKind((object) str));
      }
    }

    public override string Name
    {
      get
      {
        return this._primitiveType.Name;
      }
      set
      {
        this._primitiveType.Name = value;
      }
    }

    public PrimitiveType PrimitiveType
    {
      get
      {
        return this._primitiveType;
      }
    }

    public IEnumerable<FacetDescription> FacetDescriptions
    {
      get
      {
        foreach (FacetDescriptionElement facetDescription in this._facetDescriptions)
          yield return facetDescription.FacetDescription;
      }
    }

    internal override void ResolveTopLevelNames()
    {
      base.ResolveTopLevelNames();
      foreach (FacetDescriptionElement facetDescription in this._facetDescriptions)
      {
        try
        {
          facetDescription.CreateAndValidateFacetDescription(this.Name);
        }
        catch (ArgumentException ex)
        {
          this.AddError(ErrorCode.InvalidFacetInProviderManifest, EdmSchemaErrorSeverity.Error, (object) ex.Message);
        }
      }
    }

    internal override void Validate()
    {
      base.Validate();
      if (!this.ValidateSufficientFacets())
        return;
      this.ValidateInterFacetConsistency();
    }

    private bool ValidateInterFacetConsistency()
    {
      if (this.PrimitiveType.PrimitiveTypeKind == PrimitiveTypeKind.Decimal)
      {
        FacetDescription facet1 = Helper.GetFacet(this.FacetDescriptions, "Precision");
        FacetDescription facet2 = Helper.GetFacet(this.FacetDescriptions, "Scale");
        if (facet1.MaxValue.Value < facet2.MaxValue.Value)
        {
          this.AddError(ErrorCode.BadPrecisionAndScale, EdmSchemaErrorSeverity.Error, (object) Strings.BadPrecisionAndScale((object) facet1.MaxValue.Value, (object) facet2.MaxValue.Value));
          return false;
        }
      }
      return true;
    }

    private bool ValidateSufficientFacets()
    {
      PrimitiveType baseType = this._primitiveType.BaseType as PrimitiveType;
      if (baseType == null)
        return false;
      bool flag = false;
      foreach (FacetDescription facetDescription in baseType.FacetDescriptions)
      {
        if (Helper.GetFacet(this.FacetDescriptions, facetDescription.FacetName) == null)
        {
          this.AddError(ErrorCode.RequiredFacetMissing, EdmSchemaErrorSeverity.Error, (object) Strings.MissingFacetDescription((object) this.PrimitiveType.Name, (object) this.PrimitiveType.PrimitiveTypeKind, (object) facetDescription.FacetName));
          flag = true;
        }
      }
      return !flag;
    }
  }
}
