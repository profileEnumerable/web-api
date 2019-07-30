// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.SchemaEntityType
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Diagnostics;
using System.Xml;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  [DebuggerDisplay("Name={Name}, BaseType={BaseType.FQName}, HasKeys={HasKeys}")]
  internal sealed class SchemaEntityType : StructuredType
  {
    private static readonly List<PropertyRefElement> _emptyKeyProperties = new List<PropertyRefElement>(0);
    private const char KEY_DELIMITER = ' ';
    private ISchemaElementLookUpTable<NavigationProperty> _navigationProperties;
    private EntityKeyElement _keyElement;

    public SchemaEntityType(Schema parentElement)
      : base(parentElement)
    {
      if (this.Schema.DataModel != SchemaDataModelOption.EntityDataModel)
        return;
      this.OtherContent.Add(this.Schema.SchemaSource);
    }

    internal override void ResolveTopLevelNames()
    {
      base.ResolveTopLevelNames();
      if (this.BaseType != null)
      {
        if (!(this.BaseType is SchemaEntityType))
        {
          this.AddError(ErrorCode.InvalidBaseType, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidBaseTypeForItemType((object) this.BaseType.FQName, (object) this.FQName));
        }
        else
        {
          if (this._keyElement == null || this.BaseType == null)
            return;
          this.AddError(ErrorCode.InvalidKey, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidKeyKeyDefinedInBaseClass((object) this.FQName, (object) this.BaseType.FQName));
        }
      }
      else if (this._keyElement == null)
      {
        this.AddError(ErrorCode.KeyMissingOnEntityType, EdmSchemaErrorSeverity.Error, (object) Strings.KeyMissingOnEntityType((object) this.FQName));
      }
      else
      {
        if (this.BaseType == null && this.UnresolvedBaseType != null)
          return;
        this._keyElement.ResolveTopLevelNames();
      }
    }

    protected override bool HandleAttribute(XmlReader reader)
    {
      return base.HandleAttribute(reader) || SchemaElement.CanHandleAttribute(reader, "OpenType") && this.Schema.DataModel == SchemaDataModelOption.EntityDataModel;
    }

    public EntityKeyElement KeyElement
    {
      get
      {
        return this._keyElement;
      }
    }

    public IList<PropertyRefElement> DeclaredKeyProperties
    {
      get
      {
        if (this.KeyElement == null)
          return (IList<PropertyRefElement>) SchemaEntityType._emptyKeyProperties;
        return this.KeyElement.KeyProperties;
      }
    }

    public IList<PropertyRefElement> KeyProperties
    {
      get
      {
        if (this.KeyElement != null)
          return this.KeyElement.KeyProperties;
        if (this.BaseType != null)
          return (this.BaseType as SchemaEntityType).KeyProperties;
        return (IList<PropertyRefElement>) SchemaEntityType._emptyKeyProperties;
      }
    }

    public ISchemaElementLookUpTable<NavigationProperty> NavigationProperties
    {
      get
      {
        if (this._navigationProperties == null)
          this._navigationProperties = (ISchemaElementLookUpTable<NavigationProperty>) new FilteredSchemaElementLookUpTable<NavigationProperty, SchemaElement>(this.NamedMembers);
        return this._navigationProperties;
      }
    }

    internal override void Validate()
    {
      base.Validate();
      if (this.KeyElement == null)
        return;
      this.KeyElement.Validate();
    }

    protected override bool HandleElement(XmlReader reader)
    {
      if (base.HandleElement(reader))
        return true;
      if (this.CanHandleElement(reader, "Key"))
      {
        this.HandleKeyElement(reader);
        return true;
      }
      if (this.CanHandleElement(reader, "NavigationProperty"))
      {
        this.HandleNavigationPropertyElement(reader);
        return true;
      }
      if (this.CanHandleElement(reader, "ValueAnnotation") && this.Schema.DataModel == SchemaDataModelOption.EntityDataModel)
      {
        this.SkipElement(reader);
        return true;
      }
      if (!this.CanHandleElement(reader, "TypeAnnotation") || this.Schema.DataModel != SchemaDataModelOption.EntityDataModel)
        return false;
      this.SkipElement(reader);
      return true;
    }

    private void HandleNavigationPropertyElement(XmlReader reader)
    {
      NavigationProperty navigationProperty = new NavigationProperty(this);
      navigationProperty.Parse(reader);
      this.AddMember((SchemaElement) navigationProperty);
    }

    private void HandleKeyElement(XmlReader reader)
    {
      this._keyElement = new EntityKeyElement(this);
      this._keyElement.Parse(reader);
    }
  }
}
