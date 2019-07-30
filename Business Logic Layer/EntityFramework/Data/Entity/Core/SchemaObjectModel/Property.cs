// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.Property
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Xml;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal abstract class Property : SchemaElement
  {
    internal Property(StructuredType parentElement)
      : base((SchemaElement) parentElement, (IDbDependencyResolver) null)
    {
    }

    public abstract SchemaType Type { get; }

    protected override bool HandleElement(XmlReader reader)
    {
      if (base.HandleElement(reader))
        return true;
      if (this.Schema.DataModel == SchemaDataModelOption.EntityDataModel)
      {
        if (this.CanHandleElement(reader, "ValueAnnotation"))
        {
          this.SkipElement(reader);
          return true;
        }
        if (this.CanHandleElement(reader, "TypeAnnotation"))
        {
          this.SkipElement(reader);
          return true;
        }
      }
      return false;
    }
  }
}
