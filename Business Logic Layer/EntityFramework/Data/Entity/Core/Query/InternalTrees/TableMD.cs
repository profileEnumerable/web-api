// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.TableMD
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Query.PlanCompiler;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal class TableMD
  {
    private readonly List<ColumnMD> m_columns;
    private readonly List<ColumnMD> m_keys;
    private readonly EntitySetBase m_extent;
    private readonly bool m_flattened;

    private TableMD(EntitySetBase extent)
    {
      this.m_columns = new List<ColumnMD>();
      this.m_keys = new List<ColumnMD>();
      this.m_extent = extent;
    }

    internal TableMD(TypeUsage type, EntitySetBase extent)
      : this(extent)
    {
      this.m_columns.Add(new ColumnMD("element", type));
      this.m_flattened = !TypeUtils.IsStructuredType(type);
    }

    internal TableMD(
      IEnumerable<EdmProperty> properties,
      IEnumerable<EdmMember> keyProperties,
      EntitySetBase extent)
      : this(extent)
    {
      Dictionary<string, ColumnMD> dictionary = new Dictionary<string, ColumnMD>();
      this.m_flattened = true;
      foreach (EdmProperty property in properties)
      {
        ColumnMD columnMd = new ColumnMD((EdmMember) property);
        this.m_columns.Add(columnMd);
        dictionary[property.Name] = columnMd;
      }
      foreach (EdmMember keyProperty in keyProperties)
      {
        ColumnMD columnMd;
        if (dictionary.TryGetValue(keyProperty.Name, out columnMd))
          this.m_keys.Add(columnMd);
      }
    }

    internal EntitySetBase Extent
    {
      get
      {
        return this.m_extent;
      }
    }

    internal List<ColumnMD> Columns
    {
      get
      {
        return this.m_columns;
      }
    }

    internal List<ColumnMD> Keys
    {
      get
      {
        return this.m_keys;
      }
    }

    internal bool Flattened
    {
      get
      {
        return this.m_flattened;
      }
    }

    public override string ToString()
    {
      if (this.m_extent == null)
        return "Transient";
      return this.m_extent.Name;
    }
  }
}
