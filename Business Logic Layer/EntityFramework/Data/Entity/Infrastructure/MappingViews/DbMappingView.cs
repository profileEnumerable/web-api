// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.MappingViews.DbMappingView
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;

namespace System.Data.Entity.Infrastructure.MappingViews
{
  /// <summary>Represents a mapping view.</summary>
  public class DbMappingView
  {
    private readonly string _entitySql;

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Infrastructure.MappingViews.DbMappingView" /> instance having the specified entity SQL.
    /// </summary>
    /// <param name="entitySql">A string that specifies the entity SQL.</param>
    public DbMappingView(string entitySql)
    {
      Check.NotEmpty(entitySql, nameof (entitySql));
      this._entitySql = entitySql;
    }

    /// <summary>Gets the entity SQL.</summary>
    public string EntitySql
    {
      get
      {
        return this._entitySql;
      }
    }
  }
}
