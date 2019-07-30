// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Infrastructure.VersionedModel
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Xml.Linq;

namespace System.Data.Entity.Migrations.Infrastructure
{
  internal class VersionedModel
  {
    private readonly XDocument _model;
    private readonly string _version;

    public VersionedModel(XDocument model, string version = null)
    {
      this._model = model;
      this._version = version;
    }

    public XDocument Model
    {
      get
      {
        return this._model;
      }
    }

    public string Version
    {
      get
      {
        return this._version;
      }
    }
  }
}
