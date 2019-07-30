// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.DbContextTypesInitializersPair
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;

namespace System.Data.Entity.Internal
{
  internal class DbContextTypesInitializersPair : Tuple<Dictionary<Type, List<string>>, Action<DbContext>>
  {
    public DbContextTypesInitializersPair(
      Dictionary<Type, List<string>> entityTypeToPropertyNameMap,
      Action<DbContext> setsInitializer)
      : base(entityTypeToPropertyNameMap, setsInitializer)
    {
    }

    public Dictionary<Type, List<string>> EntityTypeToPropertyNameMap
    {
      get
      {
        return this.Item1;
      }
    }

    public Action<DbContext> SetsInitializer
    {
      get
      {
        return this.Item2;
      }
    }
  }
}
