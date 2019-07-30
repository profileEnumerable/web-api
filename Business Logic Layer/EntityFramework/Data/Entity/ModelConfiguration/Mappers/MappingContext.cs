// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Mappers.MappingContext
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Utilities;

namespace System.Data.Entity.ModelConfiguration.Mappers
{
  internal sealed class MappingContext
  {
    private readonly System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration _modelConfiguration;
    private readonly ConventionsConfiguration _conventionsConfiguration;
    private readonly EdmModel _model;
    private readonly AttributeProvider _attributeProvider;
    private readonly DbModelBuilderVersion _modelBuilderVersion;

    public MappingContext(
      System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration modelConfiguration,
      ConventionsConfiguration conventionsConfiguration,
      EdmModel model,
      DbModelBuilderVersion modelBuilderVersion = DbModelBuilderVersion.Latest,
      AttributeProvider attributeProvider = null)
    {
      this._modelConfiguration = modelConfiguration;
      this._conventionsConfiguration = conventionsConfiguration;
      this._model = model;
      this._modelBuilderVersion = modelBuilderVersion;
      this._attributeProvider = attributeProvider ?? new AttributeProvider();
    }

    public System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration ModelConfiguration
    {
      get
      {
        return this._modelConfiguration;
      }
    }

    public ConventionsConfiguration ConventionsConfiguration
    {
      get
      {
        return this._conventionsConfiguration;
      }
    }

    public EdmModel Model
    {
      get
      {
        return this._model;
      }
    }

    public AttributeProvider AttributeProvider
    {
      get
      {
        return this._attributeProvider;
      }
    }

    public DbModelBuilderVersion ModelBuilderVersion
    {
      get
      {
        return this._modelBuilderVersion;
      }
    }
  }
}
