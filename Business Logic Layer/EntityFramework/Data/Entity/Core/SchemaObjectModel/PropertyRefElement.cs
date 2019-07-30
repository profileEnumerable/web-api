// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.PropertyRefElement
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Infrastructure.DependencyResolution;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal sealed class PropertyRefElement : SchemaElement
  {
    private StructuredProperty _property;

    public PropertyRefElement(SchemaElement parentElement)
      : base(parentElement, (IDbDependencyResolver) null)
    {
    }

    public StructuredProperty Property
    {
      get
      {
        return this._property;
      }
    }

    internal override void ResolveTopLevelNames()
    {
    }

    internal bool ResolveNames(SchemaEntityType entityType)
    {
      if (string.IsNullOrEmpty(this.Name))
        return true;
      this._property = entityType.FindProperty(this.Name);
      return this._property != null;
    }
  }
}
