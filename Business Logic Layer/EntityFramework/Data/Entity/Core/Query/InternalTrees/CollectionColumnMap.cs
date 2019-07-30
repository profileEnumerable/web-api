// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.CollectionColumnMap
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal abstract class CollectionColumnMap : ColumnMap
  {
    private readonly ColumnMap m_element;
    private readonly SimpleColumnMap[] m_foreignKeys;
    private readonly SimpleColumnMap[] m_keys;

    internal CollectionColumnMap(
      TypeUsage type,
      string name,
      ColumnMap elementMap,
      SimpleColumnMap[] keys,
      SimpleColumnMap[] foreignKeys)
      : base(type, name)
    {
      this.m_element = elementMap;
      this.m_keys = keys ?? new SimpleColumnMap[0];
      this.m_foreignKeys = foreignKeys ?? new SimpleColumnMap[0];
    }

    internal SimpleColumnMap[] ForeignKeys
    {
      get
      {
        return this.m_foreignKeys;
      }
    }

    internal SimpleColumnMap[] Keys
    {
      get
      {
        return this.m_keys;
      }
    }

    internal ColumnMap Element
    {
      get
      {
        return this.m_element;
      }
    }
  }
}
