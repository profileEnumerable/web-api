// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting.TileBinaryOperator`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Globalization;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting
{
  internal class TileBinaryOperator<T_Query> : Tile<T_Query> where T_Query : ITileQuery
  {
    private readonly Tile<T_Query> m_arg1;
    private readonly Tile<T_Query> m_arg2;

    public TileBinaryOperator(
      Tile<T_Query> arg1,
      Tile<T_Query> arg2,
      TileOpKind opKind,
      T_Query query)
      : base(opKind, query)
    {
      this.m_arg1 = arg1;
      this.m_arg2 = arg2;
    }

    public override Tile<T_Query> Arg1
    {
      get
      {
        return this.m_arg1;
      }
    }

    public override Tile<T_Query> Arg2
    {
      get
      {
        return this.m_arg2;
      }
    }

    public override string Description
    {
      get
      {
        string format = (string) null;
        switch (this.OpKind)
        {
          case TileOpKind.Union:
            format = "({0} | {1})";
            break;
          case TileOpKind.Join:
            format = "({0} & {1})";
            break;
          case TileOpKind.AntiSemiJoin:
            format = "({0} - {1})";
            break;
        }
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, (object) this.Arg1.Description, (object) this.Arg2.Description);
      }
    }

    internal override Tile<T_Query> Replace(Tile<T_Query> oldTile, Tile<T_Query> newTile)
    {
      Tile<T_Query> tile1 = this.Arg1.Replace(oldTile, newTile);
      Tile<T_Query> tile2 = this.Arg2.Replace(oldTile, newTile);
      if (tile1 != this.Arg1 || tile2 != this.Arg2)
        return (Tile<T_Query>) new TileBinaryOperator<T_Query>(tile1, tile2, this.OpKind, this.Query);
      return (Tile<T_Query>) this;
    }
  }
}
