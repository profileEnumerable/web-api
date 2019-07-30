// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.EntityKeyElement
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Resources;
using System.Xml;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal sealed class EntityKeyElement : SchemaElement
  {
    private List<PropertyRefElement> _keyProperties;

    public EntityKeyElement(SchemaEntityType parentElement)
      : base((SchemaElement) parentElement, (IDbDependencyResolver) null)
    {
    }

    public IList<PropertyRefElement> KeyProperties
    {
      get
      {
        if (this._keyProperties == null)
          this._keyProperties = new List<PropertyRefElement>();
        return (IList<PropertyRefElement>) this._keyProperties;
      }
    }

    protected override bool HandleAttribute(XmlReader reader)
    {
      return false;
    }

    protected override bool HandleElement(XmlReader reader)
    {
      if (base.HandleElement(reader))
        return true;
      if (!this.CanHandleElement(reader, "PropertyRef"))
        return false;
      this.HandlePropertyRefElement(reader);
      return true;
    }

    private void HandlePropertyRefElement(XmlReader reader)
    {
      PropertyRefElement propertyRefElement = new PropertyRefElement(this.ParentElement);
      propertyRefElement.Parse(reader);
      this.KeyProperties.Add(propertyRefElement);
    }

    internal override void ResolveTopLevelNames()
    {
      foreach (PropertyRefElement keyProperty in this._keyProperties)
      {
        if (!keyProperty.ResolveNames((SchemaEntityType) this.ParentElement))
          this.AddError(ErrorCode.InvalidKey, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidKeyNoProperty((object) this.ParentElement.FQName, (object) keyProperty.Name));
      }
    }

    internal override void Validate()
    {
      Dictionary<string, PropertyRefElement> dictionary = new Dictionary<string, PropertyRefElement>((IEqualityComparer<string>) StringComparer.Ordinal);
      foreach (PropertyRefElement keyProperty in this._keyProperties)
      {
        StructuredProperty property = keyProperty.Property;
        if (dictionary.ContainsKey(property.Name))
        {
          this.AddError(ErrorCode.DuplicatePropertySpecifiedInEntityKey, EdmSchemaErrorSeverity.Error, (object) Strings.DuplicatePropertyNameSpecifiedInEntityKey((object) this.ParentElement.FQName, (object) property.Name));
        }
        else
        {
          dictionary.Add(property.Name, keyProperty);
          if (property.Nullable)
            this.AddError(ErrorCode.InvalidKey, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidKeyNullablePart((object) property.Name, (object) this.ParentElement.Name));
          if (!(property.Type is ScalarType) && !(property.Type is SchemaEnumType) || property.CollectionKind != CollectionKind.None)
            this.AddError(ErrorCode.EntityKeyMustBeScalar, EdmSchemaErrorSeverity.Error, (object) Strings.EntityKeyMustBeScalar((object) property.Name, (object) this.ParentElement.Name));
          else if (!(property.Type is SchemaEnumType))
          {
            PrimitiveType edmType = (PrimitiveType) property.TypeUsage.EdmType;
            if (this.Schema.DataModel == SchemaDataModelOption.EntityDataModel)
            {
              if (edmType.PrimitiveTypeKind == PrimitiveTypeKind.Binary && this.Schema.SchemaVersion < 2.0 || Helper.IsSpatialType(edmType))
                this.AddError(ErrorCode.EntityKeyTypeCurrentlyNotSupported, EdmSchemaErrorSeverity.Error, (object) Strings.EntityKeyTypeCurrentlyNotSupported((object) property.Name, (object) this.ParentElement.FQName, (object) edmType.PrimitiveTypeKind));
            }
            else if (edmType.PrimitiveTypeKind == PrimitiveTypeKind.Binary && this.Schema.SchemaVersion < 2.0 || Helper.IsSpatialType(edmType))
              this.AddError(ErrorCode.EntityKeyTypeCurrentlyNotSupported, EdmSchemaErrorSeverity.Error, (object) Strings.EntityKeyTypeCurrentlyNotSupportedInSSDL((object) property.Name, (object) this.ParentElement.FQName, (object) property.TypeUsage.EdmType.Name, (object) property.TypeUsage.EdmType.BaseType.FullName, (object) edmType.PrimitiveTypeKind));
          }
        }
      }
    }
  }
}
