// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.CollectionInfo
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal class CollectionInfo
  {
    private readonly Var m_collectionVar;
    private readonly ColumnMap m_columnMap;
    private readonly VarList m_flattenedElementVars;
    private readonly VarVec m_keys;
    private readonly List<SortKey> m_sortKeys;
    private readonly object m_discriminatorValue;

    internal Var CollectionVar
    {
      get
      {
        return this.m_collectionVar;
      }
    }

    internal ColumnMap ColumnMap
    {
      get
      {
        return this.m_columnMap;
      }
    }

    internal VarList FlattenedElementVars
    {
      get
      {
        return this.m_flattenedElementVars;
      }
    }

    internal VarVec Keys
    {
      get
      {
        return this.m_keys;
      }
    }

    internal List<SortKey> SortKeys
    {
      get
      {
        return this.m_sortKeys;
      }
    }

    internal object DiscriminatorValue
    {
      get
      {
        return this.m_discriminatorValue;
      }
    }

    internal CollectionInfo(
      Var collectionVar,
      ColumnMap columnMap,
      VarList flattenedElementVars,
      VarVec keys,
      List<SortKey> sortKeys,
      object discriminatorValue)
    {
      this.m_collectionVar = collectionVar;
      this.m_columnMap = columnMap;
      this.m_flattenedElementVars = flattenedElementVars;
      this.m_keys = keys;
      this.m_sortKeys = sortKeys;
      this.m_discriminatorValue = discriminatorValue;
    }
  }
}
