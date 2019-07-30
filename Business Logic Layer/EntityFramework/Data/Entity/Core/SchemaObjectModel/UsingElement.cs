// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.UsingElement
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Xml;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal class UsingElement : SchemaElement
  {
    internal UsingElement(Schema parentElement)
      : base((SchemaElement) parentElement, (IDbDependencyResolver) null)
    {
    }

    public virtual string Alias { get; private set; }

    public virtual string NamespaceName { get; private set; }

    public override string FQName
    {
      get
      {
        return (string) null;
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
      if (SchemaElement.CanHandleAttribute(reader, "Namespace"))
      {
        this.HandleNamespaceAttribute(reader);
        return true;
      }
      if (!SchemaElement.CanHandleAttribute(reader, "Alias"))
        return false;
      this.HandleAliasAttribute(reader);
      return true;
    }

    private void HandleNamespaceAttribute(XmlReader reader)
    {
      ReturnValue<string> returnValue = this.HandleDottedNameAttribute(reader, this.NamespaceName);
      if (!returnValue.Succeeded)
        return;
      this.NamespaceName = returnValue.Value;
    }

    private void HandleAliasAttribute(XmlReader reader)
    {
      this.Alias = this.HandleUndottedNameAttribute(reader, this.Alias);
    }
  }
}
