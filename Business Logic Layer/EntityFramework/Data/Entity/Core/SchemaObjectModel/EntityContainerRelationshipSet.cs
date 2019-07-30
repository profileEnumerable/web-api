// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.EntityContainerRelationshipSet
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
  internal abstract class EntityContainerRelationshipSet : SchemaElement
  {
    private IRelationship _relationship;
    private string _unresolvedRelationshipTypeName;

    public EntityContainerRelationshipSet(EntityContainer parentElement)
      : base((SchemaElement) parentElement, (IDbDependencyResolver) null)
    {
    }

    public override string FQName
    {
      get
      {
        return this.ParentElement.Name + "." + this.Name;
      }
    }

    internal IRelationship Relationship
    {
      get
      {
        return this._relationship;
      }
      set
      {
        this._relationship = value;
      }
    }

    protected abstract bool HasEnd(string role);

    protected abstract void AddEnd(
      IRelationshipEnd relationshipEnd,
      EntityContainerEntitySet entitySet);

    internal abstract IEnumerable<EntityContainerRelationshipSetEnd> Ends { get; }

    protected void HandleRelationshipTypeNameAttribute(XmlReader reader)
    {
      ReturnValue<string> returnValue = this.HandleDottedNameAttribute(reader, this._unresolvedRelationshipTypeName);
      if (!returnValue.Succeeded)
        return;
      this._unresolvedRelationshipTypeName = returnValue.Value;
    }

    internal override void ResolveTopLevelNames()
    {
      base.ResolveTopLevelNames();
      if (this._relationship == null)
      {
        SchemaType type;
        if (!this.Schema.ResolveTypeName((SchemaElement) this, this._unresolvedRelationshipTypeName, out type))
          return;
        this._relationship = type as IRelationship;
        if (this._relationship == null)
        {
          this.AddError(ErrorCode.InvalidPropertyType, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidRelationshipSetType((object) type.Name));
          return;
        }
      }
      foreach (SchemaElement end in this.Ends)
        end.ResolveTopLevelNames();
    }

    internal override void ResolveSecondLevelNames()
    {
      base.ResolveSecondLevelNames();
      foreach (SchemaElement end in this.Ends)
        end.ResolveSecondLevelNames();
    }

    internal override void Validate()
    {
      base.Validate();
      this.InferEnds();
      foreach (SchemaElement end in this.Ends)
        end.Validate();
    }

    private void InferEnds()
    {
      foreach (IRelationshipEnd end in (IEnumerable<IRelationshipEnd>) this.Relationship.Ends)
      {
        if (!this.HasEnd(end.Name))
        {
          EntityContainerEntitySet entitySet = this.InferEntitySet(end);
          if (entitySet != null)
            this.AddEnd(end, entitySet);
        }
      }
    }

    private EntityContainerEntitySet InferEntitySet(
      IRelationshipEnd relationshipEnd)
    {
      List<EntityContainerEntitySet> containerEntitySetList = new List<EntityContainerEntitySet>();
      foreach (EntityContainerEntitySet entitySet in this.ParentElement.EntitySets)
      {
        if (relationshipEnd.Type.IsOfType((StructuredType) entitySet.EntityType))
          containerEntitySetList.Add(entitySet);
      }
      if (containerEntitySetList.Count == 1)
        return containerEntitySetList[0];
      if (containerEntitySetList.Count == 0)
        this.AddError(ErrorCode.MissingExtentEntityContainerEnd, EdmSchemaErrorSeverity.Error, (object) Strings.MissingEntityContainerEnd((object) relationshipEnd.Name, (object) this.FQName));
      else
        this.AddError(ErrorCode.AmbiguousEntityContainerEnd, EdmSchemaErrorSeverity.Error, (object) Strings.AmbiguousEntityContainerEnd((object) relationshipEnd.Name, (object) this.FQName));
      return (EntityContainerEntitySet) null;
    }

    internal EntityContainer ParentElement
    {
      get
      {
        return (EntityContainer) base.ParentElement;
      }
    }
  }
}
