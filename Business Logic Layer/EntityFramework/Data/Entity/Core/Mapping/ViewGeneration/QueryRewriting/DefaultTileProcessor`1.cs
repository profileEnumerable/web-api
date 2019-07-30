// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting.DefaultTileProcessor`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting
{
  internal class DefaultTileProcessor<T_Query> : TileProcessor<Tile<T_Query>> where T_Query : ITileQuery
  {
    private readonly TileQueryProcessor<T_Query> _tileQueryProcessor;

    internal DefaultTileProcessor(TileQueryProcessor<T_Query> tileQueryProcessor)
    {
      this._tileQueryProcessor = tileQueryProcessor;
    }

    internal TileQueryProcessor<T_Query> QueryProcessor
    {
      get
      {
        return this._tileQueryProcessor;
      }
    }

    internal override bool IsEmpty(Tile<T_Query> tile)
    {
      return !this._tileQueryProcessor.IsSatisfiable(tile.Query);
    }

    internal override Tile<T_Query> Union(Tile<T_Query> arg1, Tile<T_Query> arg2)
    {
      return (Tile<T_Query>) new TileBinaryOperator<T_Query>(arg1, arg2, TileOpKind.Union, this._tileQueryProcessor.Union(arg1.Query, arg2.Query));
    }

    internal override Tile<T_Query> Join(Tile<T_Query> arg1, Tile<T_Query> arg2)
    {
      return (Tile<T_Query>) new TileBinaryOperator<T_Query>(arg1, arg2, TileOpKind.Join, this._tileQueryProcessor.Intersect(arg1.Query, arg2.Query));
    }

    internal override Tile<T_Query> AntiSemiJoin(Tile<T_Query> arg1, Tile<T_Query> arg2)
    {
      return (Tile<T_Query>) new TileBinaryOperator<T_Query>(arg1, arg2, TileOpKind.AntiSemiJoin, this._tileQueryProcessor.Difference(arg1.Query, arg2.Query));
    }

    internal override Tile<T_Query> GetArg1(Tile<T_Query> tile)
    {
      return tile.Arg1;
    }

    internal override Tile<T_Query> GetArg2(Tile<T_Query> tile)
    {
      return tile.Arg2;
    }

    internal override TileOpKind GetOpKind(Tile<T_Query> tile)
    {
      return tile.OpKind;
    }

    internal bool IsContainedIn(Tile<T_Query> arg1, Tile<T_Query> arg2)
    {
      return this.IsEmpty(this.AntiSemiJoin(arg1, arg2));
    }

    internal bool IsEquivalentTo(Tile<T_Query> arg1, Tile<T_Query> arg2)
    {
      if (this.IsContainedIn(arg1, arg2))
        return this.IsContainedIn(arg2, arg1);
      return false;
    }
  }
}
