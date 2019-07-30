// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.FacetEnabledSchemaElement
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Xml;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal abstract class FacetEnabledSchemaElement : SchemaElement
  {
    protected SchemaType _type;
    protected string _unresolvedType;
    protected TypeUsageBuilder _typeUsageBuilder;

    internal Function ParentElement
    {
      get
      {
        return base.ParentElement as Function;
      }
    }

    internal SchemaType Type
    {
      get
      {
        return this._type;
      }
    }

    internal virtual TypeUsage TypeUsage
    {
      get
      {
        return this._typeUsageBuilder.TypeUsage;
      }
    }

    internal TypeUsageBuilder TypeUsageBuilder
    {
      get
      {
        return this._typeUsageBuilder;
      }
    }

    internal bool HasUserDefinedFacets
    {
      get
      {
        return this._typeUsageBuilder.HasUserDefinedFacets;
      }
    }

    internal string UnresolvedType
    {
      get
      {
        return this._unresolvedType;
      }
      set
      {
        this._unresolvedType = value;
      }
    }

    internal FacetEnabledSchemaElement(Function parentElement)
      : base((SchemaElement) parentElement, (IDbDependencyResolver) null)
    {
    }

    internal FacetEnabledSchemaElement(SchemaElement parentElement)
      : base(parentElement, (IDbDependencyResolver) null)
    {
    }

    internal override void ResolveTopLevelNames()
    {
      base.ResolveTopLevelNames();
      if (!this.Schema.ResolveTypeName((SchemaElement) this, this.UnresolvedType, out this._type) || this.Schema.DataModel != SchemaDataModelOption.ProviderManifestModel || !this._typeUsageBuilder.HasUserDefinedFacets)
        return;
      this._typeUsageBuilder.ValidateAndSetTypeUsage((ScalarType) this._type, this.Schema.DataModel != SchemaDataModelOption.ProviderManifestModel);
    }

    internal void ValidateAndSetTypeUsage(ScalarType scalar)
    {
      this._typeUsageBuilder.ValidateAndSetTypeUsage(scalar, false);
    }

    internal void ValidateAndSetTypeUsage(EdmType edmType)
    {
      this._typeUsageBuilder.ValidateAndSetTypeUsage(edmType, false);
    }

    protected override bool HandleAttribute(XmlReader reader)
    {
      return base.HandleAttribute(reader) || this._typeUsageBuilder.HandleAttribute(reader);
    }
  }
}
