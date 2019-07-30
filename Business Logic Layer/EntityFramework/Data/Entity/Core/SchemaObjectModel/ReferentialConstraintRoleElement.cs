// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.ReferentialConstraintRoleElement
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
  internal sealed class ReferentialConstraintRoleElement : SchemaElement
  {
    private List<PropertyRefElement> _roleProperties;
    private IRelationshipEnd _end;

    public ReferentialConstraintRoleElement(ReferentialConstraint parentElement)
      : base((SchemaElement) parentElement, (IDbDependencyResolver) null)
    {
    }

    public IList<PropertyRefElement> RoleProperties
    {
      get
      {
        if (this._roleProperties == null)
          this._roleProperties = new List<PropertyRefElement>();
        return (IList<PropertyRefElement>) this._roleProperties;
      }
    }

    public IRelationshipEnd End
    {
      get
      {
        return this._end;
      }
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

    protected override bool HandleAttribute(XmlReader reader)
    {
      if (!SchemaElement.CanHandleAttribute(reader, "Role"))
        return false;
      this.HandleRoleAttribute(reader);
      return true;
    }

    private void HandlePropertyRefElement(XmlReader reader)
    {
      PropertyRefElement propertyRefElement = new PropertyRefElement(this.ParentElement);
      propertyRefElement.Parse(reader);
      this.RoleProperties.Add(propertyRefElement);
    }

    private void HandleRoleAttribute(XmlReader reader)
    {
      string str;
      Utils.GetString(this.Schema, reader, out str);
      this.Name = str;
    }

    internal override void ResolveTopLevelNames()
    {
      IRelationship parentElement = (IRelationship) this.ParentElement.ParentElement;
      if (!parentElement.TryGetEnd(this.Name, out this._end))
      {
        this.AddError(ErrorCode.InvalidRoleInRelationshipConstraint, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidEndRoleInRelationshipConstraint((object) this.Name, (object) parentElement.Name));
      }
      else
      {
        SchemaEntityType type = this._end.Type;
      }
    }

    internal override void Validate()
    {
      base.Validate();
      foreach (PropertyRefElement roleProperty in this._roleProperties)
      {
        if (!roleProperty.ResolveNames(this._end.Type))
          this.AddError(ErrorCode.InvalidPropertyInRelationshipConstraint, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidPropertyInRelationshipConstraint((object) roleProperty.Name, (object) this.Name));
      }
    }
  }
}
