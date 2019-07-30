// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.Parameter
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Text;
using System.Xml;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal class Parameter : FacetEnabledSchemaElement
  {
    private ParameterDirection _parameterDirection = ParameterDirection.Input;
    private CollectionKind _collectionKind;
    private ModelFunctionTypeElement _typeSubElement;
    private bool _isRefType;

    internal Parameter(Function parentElement)
      : base(parentElement)
    {
      this._typeUsageBuilder = new TypeUsageBuilder((SchemaElement) this);
    }

    internal ParameterDirection ParameterDirection
    {
      get
      {
        return this._parameterDirection;
      }
    }

    internal CollectionKind CollectionKind
    {
      get
      {
        return this._collectionKind;
      }
      set
      {
        this._collectionKind = value;
      }
    }

    internal bool IsRefType
    {
      get
      {
        return this._isRefType;
      }
    }

    internal override TypeUsage TypeUsage
    {
      get
      {
        if (this._typeSubElement != null)
          return this._typeSubElement.GetTypeUsage();
        if (base.TypeUsage == null)
          return (TypeUsage) null;
        if (this.CollectionKind != CollectionKind.None)
          return TypeUsage.Create((EdmType) new CollectionType(base.TypeUsage));
        return base.TypeUsage;
      }
    }

    internal new SchemaType Type
    {
      get
      {
        return this._type;
      }
    }

    internal void WriteIdentity(StringBuilder builder)
    {
      builder.Append("Parameter(");
      if (!string.IsNullOrWhiteSpace(this.UnresolvedType))
      {
        if (this._collectionKind != CollectionKind.None)
          builder.Append("Collection(" + this.UnresolvedType + ")");
        else if (this._isRefType)
          builder.Append("Ref(" + this.UnresolvedType + ")");
        else
          builder.Append(this.UnresolvedType);
      }
      else if (this._typeSubElement != null)
        this._typeSubElement.WriteIdentity(builder);
      builder.Append(")");
    }

    internal override SchemaElement Clone(SchemaElement parentElement)
    {
      Parameter parameter = new Parameter((Function) parentElement);
      parameter._collectionKind = this._collectionKind;
      parameter._parameterDirection = this._parameterDirection;
      parameter._type = this._type;
      parameter.Name = this.Name;
      parameter._typeUsageBuilder = this._typeUsageBuilder;
      return (SchemaElement) parameter;
    }

    internal bool ResolveNestedTypeNames(
      Converter.ConversionCache convertedItemCache,
      Dictionary<SchemaElement, GlobalItem> newGlobalItems)
    {
      if (this._typeSubElement == null)
        return false;
      return this._typeSubElement.ResolveNameAndSetTypeUsage(convertedItemCache, newGlobalItems);
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
      if (SchemaElement.CanHandleAttribute(reader, "Mode"))
      {
        this.HandleModeAttribute(reader);
        return true;
      }
      return this._typeUsageBuilder.HandleAttribute(reader);
    }

    private void HandleTypeAttribute(XmlReader reader)
    {
      string type;
      if (!Utils.GetString(this.Schema, reader, out type))
        return;
      TypeModifier typeModifier;
      Function.RemoveTypeModifier(ref type, out typeModifier, out this._isRefType);
      if (typeModifier == TypeModifier.Array)
        this.CollectionKind = CollectionKind.Bag;
      if (!Utils.ValidateDottedName(this.Schema, reader, type))
        return;
      this.UnresolvedType = type;
    }

    private void HandleModeAttribute(XmlReader reader)
    {
      string str1 = reader.Value;
      if (string.IsNullOrEmpty(str1))
        return;
      string str2 = str1.Trim();
      if (string.IsNullOrEmpty(str2))
        return;
      switch (str2)
      {
        case "In":
          this._parameterDirection = ParameterDirection.Input;
          break;
        case "Out":
          this._parameterDirection = ParameterDirection.Output;
          if (!this.ParentElement.IsComposable || !this.ParentElement.IsFunctionImport)
            break;
          this.AddErrorBadParameterDirection(str2, reader, new Func<object, object, object, object, string>(Strings.BadParameterDirectionForComposableFunctions));
          break;
        case "InOut":
          this._parameterDirection = ParameterDirection.InputOutput;
          if (!this.ParentElement.IsComposable || !this.ParentElement.IsFunctionImport)
            break;
          this.AddErrorBadParameterDirection(str2, reader, new Func<object, object, object, object, string>(Strings.BadParameterDirectionForComposableFunctions));
          break;
        default:
          this.AddErrorBadParameterDirection(str2, reader, new Func<object, object, object, object, string>(Strings.BadParameterDirection));
          break;
      }
    }

    private void AddErrorBadParameterDirection(
      string value,
      XmlReader reader,
      Func<object, object, object, object, string> errorFunc)
    {
      this.AddError(ErrorCode.BadParameterDirection, EdmSchemaErrorSeverity.Error, reader, (object) errorFunc((object) this.ParentElement.Parameters.Count, (object) this.ParentElement.Name, (object) this.ParentElement.ParentElement.FQName, (object) value));
    }

    protected override bool HandleElement(XmlReader reader)
    {
      if (base.HandleElement(reader))
        return true;
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
      if (this.CanHandleElement(reader, "RowType"))
      {
        this.HandleRowTypeElement(reader);
        return true;
      }
      if (this.Schema.DataModel == SchemaDataModelOption.EntityDataModel)
      {
        if (this.CanHandleElement(reader, "ValueAnnotation"))
        {
          this.SkipElement(reader);
          return true;
        }
        if (this.CanHandleElement(reader, "TypeAnnotation"))
        {
          this.SkipElement(reader);
          return true;
        }
      }
      return false;
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
      if (this._unresolvedType != null)
        base.ResolveTopLevelNames();
      if (this._typeSubElement == null)
        return;
      this._typeSubElement.ResolveTopLevelNames();
    }

    internal override void Validate()
    {
      base.Validate();
      ValidationHelper.ValidateTypeDeclaration((SchemaElement) this, this._type, (SchemaElement) this._typeSubElement);
      if (this.Schema.DataModel != SchemaDataModelOption.EntityDataModel)
      {
        bool isAggregate = this.ParentElement.IsAggregate;
        if (this._type != null && (!(this._type is ScalarType) || !isAggregate && this._collectionKind != CollectionKind.None))
        {
          string str = "";
          if (this._type != null)
            str = Function.GetTypeNameForErrorMessage(this._type, this._collectionKind, this._isRefType);
          else if (this._typeSubElement != null)
            str = this._typeSubElement.FQName;
          if (this.Schema.DataModel == SchemaDataModelOption.ProviderManifestModel)
          {
            this.AddError(ErrorCode.FunctionWithNonEdmTypeNotSupported, EdmSchemaErrorSeverity.Error, (SchemaElement) this, (object) Strings.FunctionWithNonEdmPrimitiveTypeNotSupported((object) str, (object) this.ParentElement.FQName));
            return;
          }
          this.AddError(ErrorCode.FunctionWithNonPrimitiveTypeNotSupported, EdmSchemaErrorSeverity.Error, (SchemaElement) this, (object) Strings.FunctionWithNonPrimitiveTypeNotSupported((object) str, (object) this.ParentElement.FQName));
          return;
        }
      }
      ValidationHelper.ValidateFacets((SchemaElement) this, this._type, this._typeUsageBuilder);
      if (this._isRefType)
        ValidationHelper.ValidateRefType((SchemaElement) this, this._type);
      if (this._typeSubElement == null)
        return;
      this._typeSubElement.Validate();
    }
  }
}
