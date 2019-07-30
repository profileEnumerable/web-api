// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.CollectionTypeElement
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Text;
using System.Xml;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal class CollectionTypeElement : ModelFunctionTypeElement
  {
    private ModelFunctionTypeElement _typeSubElement;

    internal CollectionTypeElement(SchemaElement parentElement)
      : base(parentElement)
    {
    }

    internal ModelFunctionTypeElement SubElement
    {
      get
      {
        return this._typeSubElement;
      }
    }

    protected override bool HandleAttribute(XmlReader reader)
    {
      if (base.HandleAttribute(reader))
        return true;
      if (!SchemaElement.CanHandleAttribute(reader, "ElementType"))
        return false;
      this.HandleElementTypeAttribute(reader);
      return true;
    }

    protected void HandleElementTypeAttribute(XmlReader reader)
    {
      string name;
      if (!Utils.GetString(this.Schema, reader, out name) || !Utils.ValidateDottedName(this.Schema, reader, name))
        return;
      this._unresolvedType = name;
    }

    protected override bool HandleElement(XmlReader reader)
    {
      if (this.CanHandleElement(reader, "CollectionType"))
      {
        this.HandleCollectionTypeElement(reader);
        return true;
      }
      if (this.CanHandleElement(reader, "ReferenceType"))
      {
        this.HandleReferenceTypeElement(reader);
        return true;
      }
      if (this.CanHandleElement(reader, "TypeRef"))
      {
        this.HandleTypeRefElement(reader);
        return true;
      }
      if (!this.CanHandleElement(reader, "RowType"))
        return false;
      this.HandleRowTypeElement(reader);
      return true;
    }

    protected void HandleCollectionTypeElement(XmlReader reader)
    {
      CollectionTypeElement collectionTypeElement = new CollectionTypeElement((SchemaElement) this);
      collectionTypeElement.Parse(reader);
      this._typeSubElement = (ModelFunctionTypeElement) collectionTypeElement;
    }

    protected void HandleReferenceTypeElement(XmlReader reader)
    {
      ReferenceTypeElement referenceTypeElement = new ReferenceTypeElement((SchemaElement) this);
      referenceTypeElement.Parse(reader);
      this._typeSubElement = (ModelFunctionTypeElement) referenceTypeElement;
    }

    protected void HandleTypeRefElement(XmlReader reader)
    {
      TypeRefElement typeRefElement = new TypeRefElement((SchemaElement) this);
      typeRefElement.Parse(reader);
      this._typeSubElement = (ModelFunctionTypeElement) typeRefElement;
    }

    protected void HandleRowTypeElement(XmlReader reader)
    {
      RowTypeElement rowTypeElement = new RowTypeElement((SchemaElement) this);
      rowTypeElement.Parse(reader);
      this._typeSubElement = (ModelFunctionTypeElement) rowTypeElement;
    }

    internal override void ResolveTopLevelNames()
    {
      if (this._typeSubElement != null)
        this._typeSubElement.ResolveTopLevelNames();
      if (this._unresolvedType == null)
        return;
      base.ResolveTopLevelNames();
    }

    internal override void WriteIdentity(StringBuilder builder)
    {
      if (!string.IsNullOrWhiteSpace(this.UnresolvedType))
      {
        builder.Append("Collection(" + this.UnresolvedType + ")");
      }
      else
      {
        builder.Append("Collection(");
        this._typeSubElement.WriteIdentity(builder);
        builder.Append(")");
      }
    }

    internal override TypeUsage GetTypeUsage()
    {
      if (this._typeUsage != null)
        return this._typeUsage;
      if (this._typeSubElement != null)
      {
        CollectionType collectionType = new CollectionType(this._typeSubElement.GetTypeUsage());
        collectionType.AddMetadataProperties(this.OtherContent);
        this._typeUsage = TypeUsage.Create((EdmType) collectionType);
      }
      return this._typeUsage;
    }

    internal override bool ResolveNameAndSetTypeUsage(
      Converter.ConversionCache convertedItemCache,
      Dictionary<SchemaElement, GlobalItem> newGlobalItems)
    {
      if (this._typeUsage != null)
        return true;
      if (this._typeSubElement != null)
        return this._typeSubElement.ResolveNameAndSetTypeUsage(convertedItemCache, newGlobalItems);
      if (this._type is ScalarType)
      {
        this._typeUsageBuilder.ValidateAndSetTypeUsage(this._type as ScalarType, false);
        this._typeUsage = TypeUsage.Create((EdmType) new CollectionType(this._typeUsageBuilder.TypeUsage));
        return true;
      }
      EdmType edmType = (EdmType) Converter.LoadSchemaElement(this._type, this._type.Schema.ProviderManifest, convertedItemCache, newGlobalItems);
      if (edmType != null)
      {
        this._typeUsageBuilder.ValidateAndSetTypeUsage(edmType, false);
        this._typeUsage = TypeUsage.Create((EdmType) new CollectionType(this._typeUsageBuilder.TypeUsage));
      }
      return this._typeUsage != null;
    }

    internal override void Validate()
    {
      base.Validate();
      ValidationHelper.ValidateFacets((SchemaElement) this, this._type, this._typeUsageBuilder);
      ValidationHelper.ValidateTypeDeclaration((SchemaElement) this, this._type, (SchemaElement) this._typeSubElement);
      if (this._typeSubElement == null)
        return;
      this._typeSubElement.Validate();
    }
  }
}
