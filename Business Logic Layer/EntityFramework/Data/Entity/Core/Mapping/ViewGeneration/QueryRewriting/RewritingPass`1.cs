// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting.RewritingPass`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Linq;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting
{
  internal class RewritingPass<T_Tile> where T_Tile : class
  {
    private readonly Dictionary<T_Tile, TileOpKind> m_usedViews = new Dictionary<T_Tile, TileOpKind>();
    private readonly T_Tile m_toFill;
    private readonly T_Tile m_toAvoid;
    private readonly List<T_Tile> m_views;
    private readonly RewritingProcessor<T_Tile> m_qp;

    public RewritingPass(
      T_Tile toFill,
      T_Tile toAvoid,
      List<T_Tile> views,
      RewritingProcessor<T_Tile> qp)
    {
      this.m_toFill = toFill;
      this.m_toAvoid = toAvoid;
      this.m_views = views;
      this.m_qp = qp;
    }

    public static bool RewriteQuery(
      T_Tile toFill,
      T_Tile toAvoid,
      out T_Tile rewriting,
      List<T_Tile> views,
      RewritingProcessor<T_Tile> qp)
    {
      if (!new RewritingPass<T_Tile>(toFill, toAvoid, views, qp).RewriteQuery(out rewriting))
        return false;
      RewritingSimplifier<T_Tile>.TrySimplifyUnionRewriting(ref rewriting, toFill, toAvoid, qp);
      return true;
    }

    private static bool RewriteQueryInternal(
      T_Tile toFill,
      T_Tile toAvoid,
      out T_Tile rewriting,
      List<T_Tile> views,
      RewritingProcessor<T_Tile> qp)
    {
      return new RewritingPass<T_Tile>(toFill, toAvoid, views, qp).RewriteQuery(out rewriting);
    }

    private bool RewriteQuery(out T_Tile rewriting)
    {
      rewriting = this.m_toFill;
      T_Tile rewriting1;
      if (!this.FindRewritingByIncludedAndDisjoint(out rewriting1) && !this.FindContributingView(out rewriting1))
        return false;
      bool flag = !this.m_qp.IsDisjointFrom(rewriting1, this.m_toAvoid);
      if (flag)
      {
        foreach (T_Tile availableView in this.AvailableViews)
        {
          if (this.TryJoin(availableView, ref rewriting1))
          {
            flag = false;
            break;
          }
        }
      }
      if (flag)
      {
        foreach (T_Tile availableView in this.AvailableViews)
        {
          if (this.TryAntiSemiJoin(availableView, ref rewriting1))
          {
            flag = false;
            break;
          }
        }
      }
      if (flag)
        return false;
      RewritingSimplifier<T_Tile>.TrySimplifyJoinRewriting(ref rewriting1, this.m_toAvoid, this.m_usedViews, this.m_qp);
      T_Tile tile = this.m_qp.AntiSemiJoin(this.m_toFill, rewriting1);
      if (!this.m_qp.IsEmpty(tile))
      {
        T_Tile rewriting2;
        if (!RewritingPass<T_Tile>.RewriteQueryInternal(tile, this.m_toAvoid, out rewriting2, this.m_views, this.m_qp))
        {
          rewriting = rewriting2;
          return false;
        }
        rewriting1 = !this.m_qp.IsContainedIn(rewriting1, rewriting2) ? this.m_qp.Union(rewriting1, rewriting2) : rewriting2;
      }
      rewriting = rewriting1;
      return true;
    }

    private bool TryJoin(T_Tile view, ref T_Tile rewriting)
    {
      T_Tile tile = this.m_qp.Join(rewriting, view);
      if (this.m_qp.IsEmpty(tile))
        return false;
      this.m_usedViews[view] = TileOpKind.Join;
      rewriting = tile;
      return this.m_qp.IsDisjointFrom(rewriting, this.m_toAvoid);
    }

    private bool TryAntiSemiJoin(T_Tile view, ref T_Tile rewriting)
    {
      T_Tile tile = this.m_qp.AntiSemiJoin(rewriting, view);
      if (this.m_qp.IsEmpty(tile))
        return false;
      this.m_usedViews[view] = TileOpKind.AntiSemiJoin;
      rewriting = tile;
      return this.m_qp.IsDisjointFrom(rewriting, this.m_toAvoid);
    }

    private bool FindRewritingByIncludedAndDisjoint(out T_Tile rewritingSoFar)
    {
      rewritingSoFar = default (T_Tile);
      foreach (T_Tile availableView in this.AvailableViews)
      {
        if (this.m_qp.IsContainedIn(this.m_toFill, availableView))
        {
          if ((object) rewritingSoFar == null)
          {
            rewritingSoFar = availableView;
            this.m_usedViews[availableView] = TileOpKind.Join;
          }
          else
          {
            T_Tile b = this.m_qp.Join(rewritingSoFar, availableView);
            if (!this.m_qp.IsContainedIn(rewritingSoFar, b))
            {
              rewritingSoFar = b;
              this.m_usedViews[availableView] = TileOpKind.Join;
            }
            else
              continue;
          }
          if (this.m_qp.IsContainedIn(rewritingSoFar, this.m_toFill))
            return true;
        }
      }
      if ((object) rewritingSoFar != null)
      {
        foreach (T_Tile availableView in this.AvailableViews)
        {
          if (this.m_qp.IsDisjointFrom(this.m_toFill, availableView) && !this.m_qp.IsDisjointFrom(rewritingSoFar, availableView))
          {
            rewritingSoFar = this.m_qp.AntiSemiJoin(rewritingSoFar, availableView);
            this.m_usedViews[availableView] = TileOpKind.AntiSemiJoin;
            if (this.m_qp.IsContainedIn(rewritingSoFar, this.m_toFill))
              return true;
          }
        }
      }
      return (object) rewritingSoFar != null;
    }

    private bool FindContributingView(out T_Tile rewriting)
    {
      foreach (T_Tile availableView in this.AvailableViews)
      {
        if (!this.m_qp.IsDisjointFrom(availableView, this.m_toFill))
        {
          rewriting = availableView;
          this.m_usedViews[availableView] = TileOpKind.Join;
          return true;
        }
      }
      rewriting = default (T_Tile);
      return false;
    }

    private IEnumerable<T_Tile> AvailableViews
    {
      get
      {
        return this.m_views.Where<T_Tile>((Func<T_Tile, bool>) (view => !this.m_usedViews.ContainsKey(view)));
      }
    }
  }
}
