// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.TextElement
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Xml;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal sealed class TextElement : SchemaElement
  {
    public TextElement(SchemaElement parentElement)
      : base(parentElement, (IDbDependencyResolver) null)
    {
    }

    public string Value { get; private set; }

    protected override bool HandleText(XmlReader reader)
    {
      this.TextElementTextHandler(reader);
      return true;
    }

    private void TextElementTextHandler(XmlReader reader)
    {
      string str = reader.Value;
      if (string.IsNullOrEmpty(str))
        return;
      if (string.IsNullOrEmpty(this.Value))
        this.Value = str;
      else
        this.Value += str;
    }
  }
}
