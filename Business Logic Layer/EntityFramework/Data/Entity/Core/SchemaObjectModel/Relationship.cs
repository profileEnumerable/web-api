// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.Relationship
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Resources;
using System.Xml;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal sealed class Relationship : SchemaType, IRelationship
  {
    private RelationshipEndCollection _ends;
    private List<ReferentialConstraint> _constraints;
    private bool _isForeignKey;

    public Relationship(Schema parent, RelationshipKind kind)
      : base(parent)
    {
      this.RelationshipKind = kind;
      if (this.Schema.DataModel == SchemaDataModelOption.EntityDataModel)
      {
        this._isForeignKey = false;
        this.OtherContent.Add(this.Schema.SchemaSource);
      }
      else
      {
        if (this.Schema.DataModel != SchemaDataModelOption.ProviderDataModel)
          return;
        this._isForeignKey = true;
      }
    }

    public IList<IRelationshipEnd> Ends
    {
      get
      {
        if (this._ends == null)
          this._ends = new RelationshipEndCollection();
        return (IList<IRelationshipEnd>) this._ends;
      }
    }

    public IList<ReferentialConstraint> Constraints
    {
      get
      {
        if (this._constraints == null)
          this._constraints = new List<ReferentialConstraint>();
        return (IList<ReferentialConstraint>) this._constraints;
      }
    }

    public bool TryGetEnd(string roleName, out IRelationshipEnd end)
    {
      return this._ends.TryGetEnd(roleName, out end);
    }

    public RelationshipKind RelationshipKind { get; private set; }

    public bool IsForeignKey
    {
      get
      {
        return this._isForeignKey;
      }
    }

    internal override void Validate()
    {
      base.Validate();
      bool flag = false;
      foreach (RelationshipEnd end in (IEnumerable<IRelationshipEnd>) this.Ends)
      {
        end.Validate();
        if (this.RelationshipKind == RelationshipKind.Association && end.Operations.Count > 0)
        {
          if (flag)
            end.AddError(ErrorCode.InvalidOperation, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidOperationMultipleEndsInAssociation);
          flag = true;
        }
      }
      if (this.Constraints.Count == 0)
      {
        if (this.Schema.DataModel != SchemaDataModelOption.ProviderDataModel)
          return;
        this.AddError(ErrorCode.MissingConstraintOnRelationshipType, EdmSchemaErrorSeverity.Error, (object) Strings.MissingConstraintOnRelationshipType((object) this.FQName));
      }
      else
      {
        foreach (SchemaElement constraint in (IEnumerable<ReferentialConstraint>) this.Constraints)
          constraint.Validate();
      }
    }

    internal override void ResolveTopLevelNames()
    {
      base.ResolveTopLevelNames();
      foreach (SchemaElement end in (IEnumerable<IRelationshipEnd>) this.Ends)
        end.ResolveTopLevelNames();
      foreach (SchemaElement constraint in (IEnumerable<ReferentialConstraint>) this.Constraints)
        constraint.ResolveTopLevelNames();
    }

    protected override bool HandleElement(XmlReader reader)
    {
      if (base.HandleElement(reader))
        return true;
      if (this.CanHandleElement(reader, "End"))
      {
        this.HandleEndElement(reader);
        return true;
      }
      if (!this.CanHandleElement(reader, "ReferentialConstraint"))
        return false;
      this.HandleConstraintElement(reader);
      return true;
    }

    private void HandleEndElement(XmlReader reader)
    {
      RelationshipEnd relationshipEnd = new RelationshipEnd(this);
      relationshipEnd.Parse(reader);
      if (this.Ends.Count == 2)
        this.AddError(ErrorCode.InvalidAssociation, EdmSchemaErrorSeverity.Error, (object) Strings.TooManyAssociationEnds((object) this.FQName));
      else
        this.Ends.Add((IRelationshipEnd) relationshipEnd);
    }

    private void HandleConstraintElement(XmlReader reader)
    {
      ReferentialConstraint referentialConstraint = new ReferentialConstraint(this);
      referentialConstraint.Parse(reader);
      this.Constraints.Add(referentialConstraint);
      if (this.Schema.DataModel != SchemaDataModelOption.EntityDataModel || this.Schema.SchemaVersion < 2.0)
        return;
      this._isForeignKey = true;
    }
  }
}
