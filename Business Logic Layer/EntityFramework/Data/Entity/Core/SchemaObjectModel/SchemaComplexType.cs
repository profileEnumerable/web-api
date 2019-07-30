// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.SchemaComplexType
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Xml;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal sealed class SchemaComplexType : StructuredType
  {
    internal SchemaComplexType(Schema parentElement)
      : base(parentElement)
    {
      if (this.Schema.DataModel != SchemaDataModelOption.EntityDataModel)
        return;
      this.OtherContent.Add(this.Schema.SchemaSource);
    }

    internal override void ResolveTopLevelNames()
    {
      base.ResolveTopLevelNames();
      if (this.BaseType == null || this.BaseType is SchemaComplexType)
        return;
      this.AddError(ErrorCode.InvalidBaseType, EdmSchemaErrorSeverity.Error, (object) Strings.InvalidBaseTypeForNestedType((object) this.BaseType.FQName, (object) this.FQName));
    }

    protected override bool HandleElement(XmlReader reader)
    {
      if (base.HandleElement(reader))
        return true;
      if (this.CanHandleElement(reader, "ValueAnnotation"))
      {
        this.SkipElement(reader);
        return true;
      }
      if (!this.CanHandleElement(reader, "TypeAnnotation"))
        return false;
      this.SkipElement(reader);
      return true;
    }
  }
}
