// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.StructuredProperty
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Xml;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal class StructuredProperty : Property
  {
    private SchemaType _type;
    private readonly TypeUsageBuilder _typeUsageBuilder;
    private CollectionKind _collectionKind;

    internal StructuredProperty(StructuredType parentElement)
      : base(parentElement)
    {
      this._typeUsageBuilder = new TypeUsageBuilder((SchemaElement) this);
    }

    public override SchemaType Type
    {
      get
      {
        return this._type;
      }
    }

    public TypeUsage TypeUsage
    {
      get
      {
        return this._typeUsageBuilder.TypeUsage;
      }
    }

    public bool Nullable
    {
      get
      {
        return this._typeUsageBuilder.Nullable;
      }
    }

    public string Default
    {
      get
      {
        return this._typeUsageBuilder.Default;
      }
    }

    public object DefaultAsObject
    {
      get
      {
        return this._typeUsageBuilder.DefaultAsObject;
      }
    }

    public CollectionKind CollectionKind
    {
      get
      {
        return this._collectionKind;
      }
    }

    internal override void ResolveTopLevelNames()
    {
      base.ResolveTopLevelNames();
      if (this._type != null)
        return;
      this._type = this.ResolveType(this.UnresolvedType);
      this._typeUsageBuilder.ValidateDefaultValue(this._type);
      ScalarType type = this._type as ScalarType;
      if (type == null)
        return;
      this._typeUsageBuilder.ValidateAndSetTypeUsage(type, true);
    }

    internal void EnsureEnumTypeFacets(
      Converter.ConversionCache convertedItemCache,
      Dictionary<SchemaElement, GlobalItem> newGlobalItems)
    {
      this._typeUsageBuilder.ValidateAndSetTypeUsage((EdmType) Converter.LoadSchemaElement(this.Type, this.Type.Schema.ProviderManifest, convertedItemCache, newGlobalItems), false);
    }

    protected virtual SchemaType ResolveType(string typeName)
    {
      SchemaType type;
      if (!this.Schema.ResolveTypeName((SchemaElement) this, typeName, out type))
        return (SchemaType) null;
      if (type is SchemaComplexType || type is ScalarType || type is SchemaEnumType)
        return type;
      this.AddError(ErrorCode.InvalidPropertyType, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidPropertyType((object) this.UnresolvedType));
      return (SchemaType) null;
    }

    internal string UnresolvedType { get; set; }

    internal override void Validate()
    {
      base.Validate();
      if (this._collectionKind != CollectionKind.Bag)
      {
        int collectionKind = (int) this._collectionKind;
      }
      SchemaEnumType type = this._type as SchemaEnumType;
      if (type != null)
      {
        this._typeUsageBuilder.ValidateEnumFacets(type);
      }
      else
      {
        if (!this.Nullable || this.Schema.SchemaVersion == 1.1 || !(this._type is SchemaComplexType))
          return;
        this.AddError(ErrorCode.NullableComplexType, EdmSchemaErrorSeverity.Error, (object) Strings.ComplexObject_NullableComplexTypesNotSupported((object) this.FQName));
      }
    }

    protected override bool HandleAttribute(XmlReader reader)
    {
      if (base.HandleAttribute(reader))
        return true;
      if (SchemaElement.CanHandleAttribute(reader, "Type"))
      {
        this.HandleTypeAttribute(reader);
        return true;
      }
      if (SchemaElement.CanHandleAttribute(reader, "CollectionKind"))
      {
        this.HandleCollectionKindAttribute(reader);
        return true;
      }
      return this._typeUsageBuilder.HandleAttribute(reader);
    }

    private void HandleTypeAttribute(XmlReader reader)
    {
      if (this.UnresolvedType != null)
      {
        this.AddError(ErrorCode.AlreadyDefined, EdmSchemaErrorSeverity.Error, reader, (object) Strings.PropertyTypeAlreadyDefined((object) reader.Name));
      }
      else
      {
        string name;
        if (!Utils.GetDottedName(this.Schema, reader, out name))
          return;
        this.UnresolvedType = name;
      }
    }

    private void HandleCollectionKindAttribute(XmlReader reader)
    {
      string str = reader.Value;
      if (str == "None")
        this._collectionKind = CollectionKind.None;
      else if (str == "List")
      {
        this._collectionKind = CollectionKind.List;
      }
      else
      {
        if (!(str == "Bag"))
          return;
        this._collectionKind = CollectionKind.Bag;
      }
    }
  }
}
