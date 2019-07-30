// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.EntityContainerAssociationSet
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Xml;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal sealed class EntityContainerAssociationSet : EntityContainerRelationshipSet
  {
    private readonly Dictionary<string, EntityContainerAssociationSetEnd> _relationshipEnds = new Dictionary<string, EntityContainerAssociationSetEnd>();
    private readonly List<EntityContainerAssociationSetEnd> _rolelessEnds = new List<EntityContainerAssociationSetEnd>();

    public EntityContainerAssociationSet(EntityContainer parentElement)
      : base(parentElement)
    {
    }

    internal override IEnumerable<EntityContainerRelationshipSetEnd> Ends
    {
      get
      {
        foreach (EntityContainerAssociationSetEnd associationSetEnd in this._relationshipEnds.Values)
          yield return (EntityContainerRelationshipSetEnd) associationSetEnd;
        foreach (EntityContainerAssociationSetEnd rolelessEnd in this._rolelessEnds)
          yield return (EntityContainerRelationshipSetEnd) rolelessEnd;
      }
    }

    protected override bool HandleAttribute(XmlReader reader)
    {
      if (base.HandleAttribute(reader))
        return true;
      if (!SchemaElement.CanHandleAttribute(reader, "Association"))
        return false;
      this.HandleRelationshipTypeNameAttribute(reader);
      return true;
    }

    protected override bool HandleElement(XmlReader reader)
    {
      if (base.HandleElement(reader))
        return true;
      if (!this.CanHandleElement(reader, "End"))
        return false;
      this.HandleEndElement(reader);
      return true;
    }

    private void HandleEndElement(XmlReader reader)
    {
      EntityContainerAssociationSetEnd associationSetEnd = new EntityContainerAssociationSetEnd(this);
      associationSetEnd.Parse(reader);
      if (associationSetEnd.Role == null)
        this._rolelessEnds.Add(associationSetEnd);
      else if (this.HasEnd(associationSetEnd.Role))
        associationSetEnd.AddError(ErrorCode.InvalidName, EdmSchemaErrorSeverity.Error, reader, (object) Strings.DuplicateEndName((object) associationSetEnd.Name));
      else
        this._relationshipEnds.Add(associationSetEnd.Role, associationSetEnd);
    }

    internal override void ResolveTopLevelNames()
    {
      base.ResolveTopLevelNames();
    }

    internal override void ResolveSecondLevelNames()
    {
      base.ResolveSecondLevelNames();
      foreach (EntityContainerAssociationSetEnd rolelessEnd in this._rolelessEnds)
      {
        if (rolelessEnd.Role != null)
        {
          if (this.HasEnd(rolelessEnd.Role))
            rolelessEnd.AddError(ErrorCode.InvalidName, EdmSchemaErrorSeverity.Error, (object) Strings.InferRelationshipEndGivesAlreadyDefinedEnd((object) rolelessEnd.EntitySet.FQName, (object) this.Name));
          else
            this._relationshipEnds.Add(rolelessEnd.Role, rolelessEnd);
        }
      }
      this._rolelessEnds.Clear();
    }

    protected override void AddEnd(
      IRelationshipEnd relationshipEnd,
      EntityContainerEntitySet entitySet)
    {
      EntityContainerAssociationSetEnd associationSetEnd = new EntityContainerAssociationSetEnd(this);
      associationSetEnd.Role = relationshipEnd.Name;
      associationSetEnd.RelationshipEnd = relationshipEnd;
      associationSetEnd.EntitySet = entitySet;
      if (associationSetEnd.EntitySet == null)
        return;
      this._relationshipEnds.Add(associationSetEnd.Role, associationSetEnd);
    }

    protected override bool HasEnd(string role)
    {
      return this._relationshipEnds.ContainsKey(role);
    }

    internal override SchemaElement Clone(SchemaElement parentElement)
    {
      EntityContainerAssociationSet containerAssociationSet = new EntityContainerAssociationSet((EntityContainer) parentElement);
      containerAssociationSet.Name = this.Name;
      containerAssociationSet.Relationship = this.Relationship;
      foreach (SchemaElement end in this.Ends)
      {
        EntityContainerAssociationSetEnd associationSetEnd = (EntityContainerAssociationSetEnd) end.Clone((SchemaElement) containerAssociationSet);
        containerAssociationSet._relationshipEnds.Add(associationSetEnd.Role, associationSetEnd);
      }
      return (SchemaElement) containerAssociationSet;
    }
  }
}
