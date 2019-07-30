// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.EntityContainerAssociationSetEnd
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Xml;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal sealed class EntityContainerAssociationSetEnd : EntityContainerRelationshipSetEnd
  {
    private string _unresolvedRelationshipEndRole;

    public EntityContainerAssociationSetEnd(EntityContainerAssociationSet parentElement)
      : base((EntityContainerRelationshipSet) parentElement)
    {
    }

    public string Role
    {
      get
      {
        return this._unresolvedRelationshipEndRole;
      }
      set
      {
        this._unresolvedRelationshipEndRole = value;
      }
    }

    public override string Name
    {
      get
      {
        return this.Role;
      }
    }

    protected override bool HandleAttribute(XmlReader reader)
    {
      if (base.HandleAttribute(reader))
        return true;
      if (!SchemaElement.CanHandleAttribute(reader, "Role"))
        return false;
      this.HandleRoleAttribute(reader);
      return true;
    }

    private void HandleRoleAttribute(XmlReader reader)
    {
      this._unresolvedRelationshipEndRole = this.HandleUndottedNameAttribute(reader, this._unresolvedRelationshipEndRole);
    }

    internal override void ResolveTopLevelNames()
    {
      base.ResolveTopLevelNames();
      IRelationship relationship = this.ParentElement.Relationship;
    }

    internal override void ResolveSecondLevelNames()
    {
      base.ResolveSecondLevelNames();
      if (this._unresolvedRelationshipEndRole == null && this.EntitySet != null)
      {
        this.RelationshipEnd = this.InferRelationshipEnd(this.EntitySet);
        if (this.RelationshipEnd == null)
          return;
        this._unresolvedRelationshipEndRole = this.RelationshipEnd.Name;
      }
      else
      {
        if (this._unresolvedRelationshipEndRole == null)
          return;
        IRelationship relationship = this.ParentElement.Relationship;
        IRelationshipEnd end;
        if (relationship.TryGetEnd(this._unresolvedRelationshipEndRole, out end))
          this.RelationshipEnd = end;
        else
          this.AddError(ErrorCode.InvalidContainerTypeForEnd, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidEntityEndName((object) this.Role, (object) relationship.FQName));
      }
    }

    private IRelationshipEnd InferRelationshipEnd(EntityContainerEntitySet set)
    {
      if (this.ParentElement.Relationship == null)
        return (IRelationshipEnd) null;
      List<IRelationshipEnd> relationshipEndList = new List<IRelationshipEnd>();
      foreach (IRelationshipEnd end in (IEnumerable<IRelationshipEnd>) this.ParentElement.Relationship.Ends)
      {
        if (set.EntityType.IsOfType((StructuredType) end.Type))
          relationshipEndList.Add(end);
      }
      if (relationshipEndList.Count == 1)
        return relationshipEndList[0];
      if (relationshipEndList.Count == 0)
        this.AddError(ErrorCode.FailedInference, EdmSchemaErrorSeverity.Error, (object) Strings.InferRelationshipEndFailedNoEntitySetMatch((object) set.Name, (object) this.ParentElement.Name, (object) this.ParentElement.Relationship.FQName, (object) set.EntityType.FQName, (object) this.ParentElement.ParentElement.FQName));
      else
        this.AddError(ErrorCode.FailedInference, EdmSchemaErrorSeverity.Error, (object) Strings.InferRelationshipEndAmbiguous((object) set.Name, (object) this.ParentElement.Name, (object) this.ParentElement.Relationship.FQName, (object) set.EntityType.FQName, (object) this.ParentElement.ParentElement.FQName));
      return (IRelationshipEnd) null;
    }

    internal override SchemaElement Clone(SchemaElement parentElement)
    {
      EntityContainerAssociationSetEnd associationSetEnd = new EntityContainerAssociationSetEnd((EntityContainerAssociationSet) parentElement);
      associationSetEnd._unresolvedRelationshipEndRole = this._unresolvedRelationshipEndRole;
      associationSetEnd.EntitySet = this.EntitySet;
      return (SchemaElement) associationSetEnd;
    }
  }
}
