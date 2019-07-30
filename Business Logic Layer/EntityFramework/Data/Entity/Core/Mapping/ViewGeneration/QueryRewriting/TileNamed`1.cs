// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting.TileNamed`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting
{
  internal class TileNamed<T_Query> : Tile<T_Query> where T_Query : ITileQuery
  {
    public TileNamed(T_Query namedQuery)
      : base(TileOpKind.Named, namedQuery)
    {
    }

    public T_Query NamedQuery
    {
      get
      {
        return this.Query;
      }
    }

    public override Tile<T_Query> Arg1
    {
      get
      {
        return (Tile<T_Query>) null;
      }
    }

    public override Tile<T_Query> Arg2
    {
      get
      {
        return (Tile<T_Query>) null;
      }
    }

    public override string Description
    {
      get
      {
        return this.Query.Description;
      }
    }

    public override string ToString()
    {
      return this.Query.ToString();
    }

    internal override Tile<T_Query> Replace(Tile<T_Query> oldTile, Tile<T_Query> newTile)
    {
      if (this != oldTile)
        return (Tile<T_Query>) this;
      return newTile;
    }
  }
}
