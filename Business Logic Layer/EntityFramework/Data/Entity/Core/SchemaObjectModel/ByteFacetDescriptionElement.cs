// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.ByteFacetDescriptionElement
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Xml;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal sealed class ByteFacetDescriptionElement : FacetDescriptionElement
  {
    public ByteFacetDescriptionElement(TypeElement type, string name)
      : base(type, name)
    {
    }

    public override EdmType FacetType
    {
      get
      {
        return (EdmType) MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.Byte);
      }
    }

    protected override void HandleDefaultAttribute(XmlReader reader)
    {
      byte field = 0;
      if (!this.HandleByteAttribute(reader, ref field))
        return;
      this.DefaultValue = (object) field;
    }
  }
}
