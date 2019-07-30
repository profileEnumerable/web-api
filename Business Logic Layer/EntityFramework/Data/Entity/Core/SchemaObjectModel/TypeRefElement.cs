// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.TypeRefElement
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Text;
using System.Xml;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal class TypeRefElement : ModelFunctionTypeElement
  {
    internal TypeRefElement(SchemaElement parentElement)
      : base(parentElement)
    {
    }

    protected override bool HandleAttribute(XmlReader reader)
    {
      if (base.HandleAttribute(reader))
        return true;
      if (!SchemaElement.CanHandleAttribute(reader, "Type"))
        return false;
      this.HandleTypeAttribute(reader);
      return true;
    }

    protected void HandleTypeAttribute(XmlReader reader)
    {
      string name;
      if (!Utils.GetString(this.Schema, reader, out name) || !Utils.ValidateDottedName(this.Schema, reader, name))
        return;
      this._unresolvedType = name;
    }

    internal override bool ResolveNameAndSetTypeUsage(
      Converter.ConversionCache convertedItemCache,
      Dictionary<SchemaElement, GlobalItem> newGlobalItems)
    {
      if (this._type is ScalarType)
      {
        this._typeUsageBuilder.ValidateAndSetTypeUsage(this._type as ScalarType, false);
        this._typeUsage = this._typeUsageBuilder.TypeUsage;
        return true;
      }
      EdmType edmType = (EdmType) Converter.LoadSchemaElement(this._type, this._type.Schema.ProviderManifest, convertedItemCache, newGlobalItems);
      if (edmType != null)
      {
        this._typeUsageBuilder.ValidateAndSetTypeUsage(edmType, false);
        this._typeUsage = this._typeUsageBuilder.TypeUsage;
      }
      return this._typeUsage != null;
    }

    internal override void WriteIdentity(StringBuilder builder)
    {
      builder.Append(this.UnresolvedType);
    }

    internal override TypeUsage GetTypeUsage()
    {
      return this._typeUsage;
    }

    internal override void Validate()
    {
      base.Validate();
      ValidationHelper.ValidateFacets((SchemaElement) this, this._type, this._typeUsageBuilder);
    }
  }
}
