// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting.Tile`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Globalization;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting
{
  internal abstract class Tile<T_Query> where T_Query : ITileQuery
  {
    private readonly T_Query m_query;
    private readonly TileOpKind m_opKind;

    protected Tile(TileOpKind opKind, T_Query query)
    {
      this.m_opKind = opKind;
      this.m_query = query;
    }

    public T_Query Query
    {
      get
      {
        return this.m_query;
      }
    }

    public abstract string Description { get; }

    public IEnumerable<T_Query> GetNamedQueries()
    {
      return Tile<T_Query>.GetNamedQueries(this);
    }

    private static IEnumerable<T_Query> GetNamedQueries(Tile<T_Query> rewriting)
    {
      if (rewriting != null)
      {
        if (rewriting.OpKind == TileOpKind.Named)
        {
          yield return ((TileNamed<T_Query>) rewriting).NamedQuery;
        }
        else
        {
          foreach (T_Query namedQuery in Tile<T_Query>.GetNamedQueries(rewriting.Arg1))
            yield return namedQuery;
          foreach (T_Query namedQuery in Tile<T_Query>.GetNamedQueries(rewriting.Arg2))
            yield return namedQuery;
        }
      }
    }

    public override string ToString()
    {
      if (this.Description != null)
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}: [{1}]", (object) this.Description, (object) this.Query);
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}]", (object) this.Query);
    }

    public abstract Tile<T_Query> Arg1 { get; }

    public abstract Tile<T_Query> Arg2 { get; }

    public TileOpKind OpKind
    {
      get
      {
        return this.m_opKind;
      }
    }

    internal abstract Tile<T_Query> Replace(Tile<T_Query> oldTile, Tile<T_Query> newTile);
  }
}
