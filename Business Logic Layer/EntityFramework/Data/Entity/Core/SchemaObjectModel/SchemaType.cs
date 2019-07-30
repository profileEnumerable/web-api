// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.SchemaType
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Infrastructure.DependencyResolution;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal abstract class SchemaType : SchemaElement
  {
    public string Namespace
    {
      get
      {
        return this.Schema.Namespace;
      }
    }

    public override string Identity
    {
      get
      {
        return this.Namespace + "." + this.Name;
      }
    }

    public override string FQName
    {
      get
      {
        return this.Namespace + "." + this.Name;
      }
    }

    internal SchemaType(Schema parentElement)
      : base((SchemaElement) parentElement, (IDbDependencyResolver) null)
    {
    }
  }
}
