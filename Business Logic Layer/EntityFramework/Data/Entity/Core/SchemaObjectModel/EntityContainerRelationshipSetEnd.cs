// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.EntityContainerRelationshipSetEnd
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Resources;
using System.Xml;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal class EntityContainerRelationshipSetEnd : SchemaElement
  {
    private IRelationshipEnd _relationshipEnd;
    private string _unresolvedEntitySetName;
    private EntityContainerEntitySet _entitySet;

    public EntityContainerRelationshipSetEnd(EntityContainerRelationshipSet parentElement)
      : base((SchemaElement) parentElement, (IDbDependencyResolver) null)
    {
    }

    public IRelationshipEnd RelationshipEnd
    {
      get
      {
        return this._relationshipEnd;
      }
      internal set
      {
        this._relationshipEnd = value;
      }
    }

    public EntityContainerEntitySet EntitySet
    {
      get
      {
        return this._entitySet;
      }
      internal set
      {
        this._entitySet = value;
      }
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
      if (!SchemaElement.CanHandleAttribute(reader, "EntitySet"))
        return false;
      this.HandleEntitySetAttribute(reader);
      return true;
    }

    private void HandleEntitySetAttribute(XmlReader reader)
    {
      if (this.Schema.DataModel == SchemaDataModelOption.ProviderDataModel)
        this._unresolvedEntitySetName = reader.Value;
      else
        this._unresolvedEntitySetName = this.HandleUndottedNameAttribute(reader, this._unresolvedEntitySetName);
    }

    internal override void ResolveTopLevelNames()
    {
      base.ResolveTopLevelNames();
      if (this._entitySet != null)
        return;
      this._entitySet = this.ParentElement.ParentElement.FindEntitySet(this._unresolvedEntitySetName);
      if (this._entitySet != null)
        return;
      this.AddError(ErrorCode.InvalidEndEntitySet, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidEntitySetNameReference((object) this._unresolvedEntitySetName, (object) this.Name));
    }

    internal override void Validate()
    {
      base.Validate();
      if (this._relationshipEnd == null || this._entitySet == null || (this._relationshipEnd.Type.IsOfType((StructuredType) this._entitySet.EntityType) || this._entitySet.EntityType.IsOfType((StructuredType) this._relationshipEnd.Type)))
        return;
      this.AddError(ErrorCode.InvalidEndEntitySet, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidEndEntitySetTypeMismatch((object) this._relationshipEnd.Name));
    }

    internal EntityContainerRelationshipSet ParentElement
    {
      get
      {
        return (EntityContainerRelationshipSet) base.ParentElement;
      }
    }
  }
}
