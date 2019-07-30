// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.NavigationProperty
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Diagnostics;
using System.Xml;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  [DebuggerDisplay("Name={Name}, Relationship={_unresolvedRelationshipName}, FromRole={_unresolvedFromEndRole}, ToRole={_unresolvedToEndRole}")]
  internal sealed class NavigationProperty : Property
  {
    private string _unresolvedFromEndRole;
    private string _unresolvedToEndRole;
    private string _unresolvedRelationshipName;
    private IRelationshipEnd _fromEnd;
    private IRelationshipEnd _toEnd;
    private IRelationship _relationship;

    public NavigationProperty(SchemaEntityType parent)
      : base((StructuredType) parent)
    {
    }

    public SchemaEntityType ParentElement
    {
      get
      {
        return base.ParentElement as SchemaEntityType;
      }
    }

    internal IRelationship Relationship
    {
      get
      {
        return this._relationship;
      }
    }

    internal IRelationshipEnd ToEnd
    {
      get
      {
        return this._toEnd;
      }
    }

    internal IRelationshipEnd FromEnd
    {
      get
      {
        return this._fromEnd;
      }
    }

    public override SchemaType Type
    {
      get
      {
        if (this._toEnd == null || this._toEnd.Type == null)
          return (SchemaType) null;
        return (SchemaType) this._toEnd.Type;
      }
    }

    protected override bool HandleAttribute(XmlReader reader)
    {
      if (base.HandleAttribute(reader))
        return true;
      if (SchemaElement.CanHandleAttribute(reader, "Relationship"))
      {
        this.HandleAssociationAttribute(reader);
        return true;
      }
      if (SchemaElement.CanHandleAttribute(reader, "FromRole"))
      {
        this.HandleFromRoleAttribute(reader);
        return true;
      }
      if (SchemaElement.CanHandleAttribute(reader, "ToRole"))
      {
        this.HandleToRoleAttribute(reader);
        return true;
      }
      return SchemaElement.CanHandleAttribute(reader, "ContainsTarget");
    }

    internal override void ResolveTopLevelNames()
    {
      base.ResolveTopLevelNames();
      SchemaType type;
      if (!this.Schema.ResolveTypeName((SchemaElement) this, this._unresolvedRelationshipName, out type))
        return;
      this._relationship = type as IRelationship;
      if (this._relationship == null)
      {
        this.AddError(ErrorCode.BadNavigationProperty, EdmSchemaErrorSeverity.Error, (object) Strings.BadNavigationPropertyRelationshipNotRelationship((object) this._unresolvedRelationshipName));
      }
      else
      {
        bool flag = true;
        if (!this._relationship.TryGetEnd(this._unresolvedFromEndRole, out this._fromEnd))
        {
          this.AddError(ErrorCode.BadNavigationProperty, EdmSchemaErrorSeverity.Error, (object) Strings.BadNavigationPropertyUndefinedRole((object) this._unresolvedFromEndRole, (object) this._relationship.FQName));
          flag = false;
        }
        if (!this._relationship.TryGetEnd(this._unresolvedToEndRole, out this._toEnd))
        {
          this.AddError(ErrorCode.BadNavigationProperty, EdmSchemaErrorSeverity.Error, (object) Strings.BadNavigationPropertyUndefinedRole((object) this._unresolvedToEndRole, (object) this._relationship.FQName));
          flag = false;
        }
        if (!flag || this._fromEnd != this._toEnd)
          return;
        this.AddError(ErrorCode.BadNavigationProperty, EdmSchemaErrorSeverity.Error, (object) Strings.BadNavigationPropertyRolesCannotBeTheSame);
      }
    }

    internal override void Validate()
    {
      base.Validate();
      if (this._fromEnd.Type != this.ParentElement)
        this.AddError(ErrorCode.BadNavigationProperty, EdmSchemaErrorSeverity.Error, (object) Strings.BadNavigationPropertyBadFromRoleType((object) this.Name, (object) this._fromEnd.Type.FQName, (object) this._fromEnd.Name, (object) this._relationship.FQName, (object) this.ParentElement.FQName));
      SchemaEntityType type = this._toEnd.Type;
    }

    private void HandleToRoleAttribute(XmlReader reader)
    {
      this._unresolvedToEndRole = this.HandleUndottedNameAttribute(reader, this._unresolvedToEndRole);
    }

    private void HandleFromRoleAttribute(XmlReader reader)
    {
      this._unresolvedFromEndRole = this.HandleUndottedNameAttribute(reader, this._unresolvedFromEndRole);
    }

    private void HandleAssociationAttribute(XmlReader reader)
    {
      string name;
      if (!Utils.GetDottedName(this.Schema, reader, out name))
        return;
      this._unresolvedRelationshipName = name;
    }
  }
}
