// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting.RewritingSimplifier`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Linq;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting
{
  internal class RewritingSimplifier<T_Tile> where T_Tile : class
  {
    private readonly Dictionary<T_Tile, TileOpKind> m_usedViews = new Dictionary<T_Tile, TileOpKind>();
    private readonly T_Tile m_originalRewriting;
    private readonly T_Tile m_toAvoid;
    private readonly RewritingProcessor<T_Tile> m_qp;

    private RewritingSimplifier(
      T_Tile originalRewriting,
      T_Tile toAvoid,
      Dictionary<T_Tile, TileOpKind> usedViews,
      RewritingProcessor<T_Tile> qp)
    {
      this.m_originalRewriting = originalRewriting;
      this.m_toAvoid = toAvoid;
      this.m_qp = qp;
      this.m_usedViews = usedViews;
    }

    private RewritingSimplifier(
      T_Tile rewriting,
      T_Tile toFill,
      T_Tile toAvoid,
      RewritingProcessor<T_Tile> qp)
    {
      this.m_originalRewriting = toFill;
      this.m_toAvoid = toAvoid;
      this.m_qp = qp;
      this.m_usedViews = new Dictionary<T_Tile, TileOpKind>();
      this.GatherUnionedSubqueriesInUsedViews(rewriting);
    }

    internal static bool TrySimplifyUnionRewriting(
      ref T_Tile rewriting,
      T_Tile toFill,
      T_Tile toAvoid,
      RewritingProcessor<T_Tile> qp)
    {
      T_Tile simplifiedRewriting;
      if (!new RewritingSimplifier<T_Tile>(rewriting, toFill, toAvoid, qp).SimplifyRewriting(out simplifiedRewriting))
        return false;
      rewriting = simplifiedRewriting;
      return true;
    }

    internal static bool TrySimplifyJoinRewriting(
      ref T_Tile rewriting,
      T_Tile toAvoid,
      Dictionary<T_Tile, TileOpKind> usedViews,
      RewritingProcessor<T_Tile> qp)
    {
      T_Tile simplifiedRewriting;
      if (!new RewritingSimplifier<T_Tile>(rewriting, toAvoid, usedViews, qp).SimplifyRewriting(out simplifiedRewriting))
        return false;
      rewriting = simplifiedRewriting;
      return true;
    }

    private void GatherUnionedSubqueriesInUsedViews(T_Tile query)
    {
      if ((object) query == null)
        return;
      if (this.m_qp.GetOpKind(query) != TileOpKind.Union)
      {
        this.m_usedViews[query] = TileOpKind.Union;
      }
      else
      {
        this.GatherUnionedSubqueriesInUsedViews(this.m_qp.GetArg1(query));
        this.GatherUnionedSubqueriesInUsedViews(this.m_qp.GetArg2(query));
      }
    }

    private bool SimplifyRewriting(out T_Tile simplifiedRewriting)
    {
      bool flag = false;
      simplifiedRewriting = default (T_Tile);
      T_Tile simplifiedRewriting1;
      while (this.SimplifyRewritingOnce(out simplifiedRewriting1))
      {
        flag = true;
        simplifiedRewriting = simplifiedRewriting1;
      }
      return flag;
    }

    private bool SimplifyRewritingOnce(out T_Tile simplifiedRewriting)
    {
      HashSet<T_Tile> remainingViews = new HashSet<T_Tile>((IEnumerable<T_Tile>) this.m_usedViews.Keys);
      foreach (T_Tile key in this.m_usedViews.Keys)
      {
        switch (this.m_usedViews[key])
        {
          case TileOpKind.Union:
          case TileOpKind.Join:
            remainingViews.Remove(key);
            if (this.SimplifyRewritingOnce(key, remainingViews, out simplifiedRewriting))
              return true;
            remainingViews.Add(key);
            continue;
          default:
            continue;
        }
      }
      simplifiedRewriting = default (T_Tile);
      return false;
    }

    private bool SimplifyRewritingOnce(
      T_Tile newRewriting,
      HashSet<T_Tile> remainingViews,
      out T_Tile simplifiedRewriting)
    {
      simplifiedRewriting = default (T_Tile);
      if (remainingViews.Count == 0)
        return false;
      if (remainingViews.Count == 1)
      {
        T_Tile key = remainingViews.First<T_Tile>();
        if (!(this.m_usedViews[key] != TileOpKind.Union ? this.m_qp.IsContainedIn(this.m_originalRewriting, newRewriting) && this.m_qp.IsDisjointFrom(this.m_toAvoid, newRewriting) : this.m_qp.IsContainedIn(this.m_originalRewriting, newRewriting)))
          return false;
        simplifiedRewriting = newRewriting;
        this.m_usedViews.Remove(key);
        return true;
      }
      int num1 = remainingViews.Count / 2;
      int num2 = 0;
      T_Tile tile1 = newRewriting;
      T_Tile tile2 = newRewriting;
      HashSet<T_Tile> remainingViews1 = new HashSet<T_Tile>();
      HashSet<T_Tile> remainingViews2 = new HashSet<T_Tile>();
      foreach (T_Tile remainingView in remainingViews)
      {
        TileOpKind usedView = this.m_usedViews[remainingView];
        if (num2++ < num1)
        {
          remainingViews1.Add(remainingView);
          tile1 = this.GetRewritingHalf(tile1, remainingView, usedView);
        }
        else
        {
          remainingViews2.Add(remainingView);
          tile2 = this.GetRewritingHalf(tile2, remainingView, usedView);
        }
      }
      if (!this.SimplifyRewritingOnce(tile1, remainingViews2, out simplifiedRewriting))
        return this.SimplifyRewritingOnce(tile2, remainingViews1, out simplifiedRewriting);
      return true;
    }

    private T_Tile GetRewritingHalf(
      T_Tile halfRewriting,
      T_Tile remainingView,
      TileOpKind viewKind)
    {
      switch (viewKind)
      {
        case TileOpKind.Union:
          halfRewriting = this.m_qp.Union(halfRewriting, remainingView);
          break;
        case TileOpKind.Join:
          halfRewriting = this.m_qp.Join(halfRewriting, remainingView);
          break;
        case TileOpKind.AntiSemiJoin:
          halfRewriting = this.m_qp.AntiSemiJoin(halfRewriting, remainingView);
          break;
      }
      return halfRewriting;
    }
  }
}
