// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.RelationshipEnd
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
  internal sealed class RelationshipEnd : SchemaElement, IRelationshipEnd
  {
    private string _unresolvedType;
    private RelationshipMultiplicity? _multiplicity;
    private List<OnOperation> _operations;

    public RelationshipEnd(Relationship relationship)
      : base((SchemaElement) relationship, (IDbDependencyResolver) null)
    {
    }

    public SchemaEntityType Type { get; private set; }

    public RelationshipMultiplicity? Multiplicity
    {
      get
      {
        return this._multiplicity;
      }
      set
      {
        this._multiplicity = value;
      }
    }

    public ICollection<OnOperation> Operations
    {
      get
      {
        if (this._operations == null)
          this._operations = new List<OnOperation>();
        return (ICollection<OnOperation>) this._operations;
      }
    }

    internal override void ResolveTopLevelNames()
    {
      base.ResolveTopLevelNames();
      SchemaType type;
      if (this.Type != null || this._unresolvedType == null || !this.Schema.ResolveTypeName((SchemaElement) this, this._unresolvedType, out type))
        return;
      this.Type = type as SchemaEntityType;
      if (this.Type != null)
        return;
      this.AddError(ErrorCode.InvalidRelationshipEndType, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidRelationshipEndType((object) this.ParentElement.Name, (object) type.FQName));
    }

    internal override void Validate()
    {
      base.Validate();
      RelationshipMultiplicity? multiplicity = this.Multiplicity;
      if ((multiplicity.GetValueOrDefault() != RelationshipMultiplicity.Many ? 0 : (multiplicity.HasValue ? 1 : 0)) != 0 && this.Operations.Count != 0)
        this.AddError(ErrorCode.EndWithManyMultiplicityCannotHaveOperationsSpecified, EdmSchemaErrorSeverity.Error, (object) Strings.EndWithManyMultiplicityCannotHaveOperationsSpecified((object) this.Name, (object) this.ParentElement.FQName));
      if (this.ParentElement.Constraints.Count != 0 || this.Multiplicity.HasValue)
        return;
      this.AddError(ErrorCode.EndWithoutMultiplicity, EdmSchemaErrorSeverity.Error, (object) Strings.EndWithoutMultiplicity((object) this.Name, (object) this.ParentElement.FQName));
    }

    protected override void HandleAttributesComplete()
    {
      if (this.Name == null && this._unresolvedType != null)
        this.Name = Utils.ExtractTypeName(this._unresolvedType);
      base.HandleAttributesComplete();
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
      if (SchemaElement.CanHandleAttribute(reader, "Multiplicity"))
      {
        this.HandleMultiplicityAttribute(reader);
        return true;
      }
      if (SchemaElement.CanHandleAttribute(reader, "Role"))
      {
        this.HandleNameAttribute(reader);
        return true;
      }
      if (!SchemaElement.CanHandleAttribute(reader, "Type"))
        return false;
      this.HandleTypeAttribute(reader);
      return true;
    }

    protected override bool HandleElement(XmlReader reader)
    {
      if (base.HandleElement(reader))
        return true;
      if (!this.CanHandleElement(reader, "OnDelete"))
        return false;
      this.HandleOnDeleteElement(reader);
      return true;
    }

    private void HandleTypeAttribute(XmlReader reader)
    {
      string name;
      if (!Utils.GetDottedName(this.Schema, reader, out name))
        return;
      this._unresolvedType = name;
    }

    private void HandleMultiplicityAttribute(XmlReader reader)
    {
      RelationshipMultiplicity multiplicity;
      if (!RelationshipMultiplicityConverter.TryParseMultiplicity(reader.Value, out multiplicity))
        this.AddError(ErrorCode.InvalidMultiplicity, EdmSchemaErrorSeverity.Error, reader, (object) Strings.InvalidRelationshipEndMultiplicity((object) this.ParentElement.Name, (object) reader.Value));
      this._multiplicity = new RelationshipMultiplicity?(multiplicity);
    }

    private void HandleOnDeleteElement(XmlReader reader)
    {
      this.HandleOnOperationElement(reader, Operation.Delete);
    }

    private void HandleOnOperationElement(XmlReader reader, Operation operation)
    {
      foreach (OnOperation operation1 in (IEnumerable<OnOperation>) this.Operations)
      {
        if (operation1.Operation == operation)
          this.AddError(ErrorCode.InvalidOperation, EdmSchemaErrorSeverity.Error, reader, (object) Strings.DuplicationOperation((object) reader.Name));
      }
      OnOperation onOperation = new OnOperation(this, operation);
      onOperation.Parse(reader);
      this._operations.Add(onOperation);
    }

    internal IRelationship ParentElement
    {
      get
      {
        return (IRelationship) base.ParentElement;
      }
    }
  }
}
