// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.PrimitiveSchema
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Linq;
using System.Xml;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal class PrimitiveSchema : Schema
  {
    public PrimitiveSchema(SchemaManager schemaManager)
      : base(schemaManager)
    {
      this.Schema = (Schema) this;
      DbProviderManifest providerManifest = this.ProviderManifest;
      if (providerManifest == null)
      {
        this.AddError(new EdmSchemaError(Strings.FailedToRetrieveProviderManifest, 168, EdmSchemaErrorSeverity.Error));
      }
      else
      {
        IList<PrimitiveType> source = (IList<PrimitiveType>) providerManifest.GetStoreTypes();
        if (schemaManager.DataModel == SchemaDataModelOption.EntityDataModel && schemaManager.SchemaVersion < 3.0)
          source = (IList<PrimitiveType>) source.Where<PrimitiveType>((Func<PrimitiveType, bool>) (t => !Helper.IsSpatialType(t))).ToList<PrimitiveType>();
        foreach (PrimitiveType primitiveType in (IEnumerable<PrimitiveType>) source)
          this.TryAddType((SchemaType) new ScalarType((Schema) this, primitiveType.Name, primitiveType), false);
      }
    }

    internal override string Alias
    {
      get
      {
        return this.ProviderManifest.NamespaceName;
      }
    }

    internal override string Namespace
    {
      get
      {
        if (this.ProviderManifest != null)
          return this.ProviderManifest.NamespaceName;
        return string.Empty;
      }
    }

    protected override bool HandleAttribute(XmlReader reader)
    {
      return false;
    }
  }
}
