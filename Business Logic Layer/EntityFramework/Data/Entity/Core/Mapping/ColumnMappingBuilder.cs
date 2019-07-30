// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ColumnMappingBuilder
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.Mapping
{
  internal class ColumnMappingBuilder
  {
    private EdmProperty _columnProperty;
    private readonly IList<EdmProperty> _propertyPath;
    private ScalarPropertyMapping _scalarPropertyMapping;

    public ColumnMappingBuilder(EdmProperty columnProperty, IList<EdmProperty> propertyPath)
    {
      Check.NotNull<EdmProperty>(columnProperty, nameof (columnProperty));
      Check.NotNull<IList<EdmProperty>>(propertyPath, nameof (propertyPath));
      this._columnProperty = columnProperty;
      this._propertyPath = propertyPath;
    }

    public IList<EdmProperty> PropertyPath
    {
      get
      {
        return this._propertyPath;
      }
    }

    public EdmProperty ColumnProperty
    {
      get
      {
        return this._columnProperty;
      }
      internal set
      {
        this._columnProperty = value;
        if (this._scalarPropertyMapping == null)
          return;
        this._scalarPropertyMapping.Column = this._columnProperty;
      }
    }

    internal void SetTarget(ScalarPropertyMapping scalarPropertyMapping)
    {
      this._scalarPropertyMapping = scalarPropertyMapping;
    }
  }
}
