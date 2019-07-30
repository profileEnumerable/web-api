// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.OnOperation
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Resources;
using System.Xml;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal sealed class OnOperation : SchemaElement
  {
    public OnOperation(RelationshipEnd parentElement, Operation operation)
      : base((SchemaElement) parentElement, (IDbDependencyResolver) null)
    {
      this.Operation = operation;
    }

    public Operation Operation { get; private set; }

    public Action Action { get; private set; }

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
      if (!SchemaElement.CanHandleAttribute(reader, "Action"))
        return false;
      this.HandleActionAttribute(reader);
      return true;
    }

    private void HandleActionAttribute(XmlReader reader)
    {
      int relationshipKind = (int) this.ParentElement.ParentElement.RelationshipKind;
      switch (reader.Value.Trim())
      {
        case "None":
          this.Action = Action.None;
          break;
        case "Cascade":
          this.Action = Action.Cascade;
          break;
        default:
          this.AddError(ErrorCode.InvalidAction, EdmSchemaErrorSeverity.Error, reader, (object) Strings.InvalidAction((object) reader.Value, (object) this.ParentElement.FQName));
          break;
      }
    }

    private RelationshipEnd ParentElement
    {
      get
      {
        return (RelationshipEnd) base.ParentElement;
      }
    }
  }
}
