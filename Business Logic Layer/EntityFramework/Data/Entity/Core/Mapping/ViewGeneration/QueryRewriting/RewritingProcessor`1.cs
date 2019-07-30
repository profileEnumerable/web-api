// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting.RewritingProcessor`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting
{
  internal class RewritingProcessor<T_Tile> : System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting.TileProcessor<T_Tile>
    where T_Tile : class
  {
    private static Random rnd = new Random(1507);
    public const double PermuteFraction = 0.0;
    public const int MinPermutations = 0;
    public const int MaxPermutations = 0;
    private int m_numSATChecks;
    private int m_numIntersection;
    private int m_numDifference;
    private int m_numUnion;
    private int m_numErrors;
    private readonly System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting.TileProcessor<T_Tile> m_tileProcessor;

    public RewritingProcessor(System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting.TileProcessor<T_Tile> tileProcessor)
    {
      this.m_tileProcessor = tileProcessor;
    }

    internal System.Data.Entity.Core.Mapping.ViewGeneration.QueryRewriting.TileProcessor<T_Tile> TileProcessor
    {
      get
      {
        return this.m_tileProcessor;
      }
    }

    public void GetStatistics(
      out int numSATChecks,
      out int numIntersection,
      out int numUnion,
      out int numDifference,
      out int numErrors)
    {
      numSATChecks = this.m_numSATChecks;
      numIntersection = this.m_numIntersection;
      numUnion = this.m_numUnion;
      numDifference = this.m_numDifference;
      numErrors = this.m_numErrors;
    }

    internal override T_Tile GetArg1(T_Tile tile)
    {
      return this.m_tileProcessor.GetArg1(tile);
    }

    internal override T_Tile GetArg2(T_Tile tile)
    {
      return this.m_tileProcessor.GetArg2(tile);
    }

    internal override TileOpKind GetOpKind(T_Tile tile)
    {
      return this.m_tileProcessor.GetOpKind(tile);
    }

    internal override bool IsEmpty(T_Tile a)
    {
      ++this.m_numSATChecks;
      return this.m_tileProcessor.IsEmpty(a);
    }

    public bool IsDisjointFrom(T_Tile a, T_Tile b)
    {
      return this.m_tileProcessor.IsEmpty(this.Join(a, b));
    }

    internal bool IsContainedIn(T_Tile a, T_Tile b)
    {
      return this.IsEmpty(this.AntiSemiJoin(a, b));
    }

    internal bool IsEquivalentTo(T_Tile a, T_Tile b)
    {
      bool flag1 = this.IsContainedIn(a, b);
      bool flag2 = this.IsContainedIn(b, a);
      if (flag1)
        return flag2;
      return false;
    }

    internal override T_Tile Union(T_Tile a, T_Tile b)
    {
      ++this.m_numUnion;
      return this.m_tileProcessor.Union(a, b);
    }

    internal override T_Tile Join(T_Tile a, T_Tile b)
    {
      if ((object) a == null)
        return b;
      ++this.m_numIntersection;
      return this.m_tileProcessor.Join(a, b);
    }

    internal override T_Tile AntiSemiJoin(T_Tile a, T_Tile b)
    {
      ++this.m_numDifference;
      return this.m_tileProcessor.AntiSemiJoin(a, b);
    }

    public void AddError()
    {
      ++this.m_numErrors;
    }

    public int CountOperators(T_Tile query)
    {
      int num = 0;
      if ((object) query != null && this.GetOpKind(query) != TileOpKind.Named)
        num = num + 1 + this.CountOperators(this.GetArg1(query)) + this.CountOperators(this.GetArg2(query));
      return num;
    }

    public int CountViews(T_Tile query)
    {
      HashSet<T_Tile> views = new HashSet<T_Tile>();
      this.GatherViews(query, views);
      return views.Count;
    }

    public void GatherViews(T_Tile rewriting, HashSet<T_Tile> views)
    {
      if ((object) rewriting == null)
        return;
      if (this.GetOpKind(rewriting) == TileOpKind.Named)
      {
        views.Add(rewriting);
      }
      else
      {
        this.GatherViews(this.GetArg1(rewriting), views);
        this.GatherViews(this.GetArg2(rewriting), views);
      }
    }

    public static IEnumerable<T> AllButOne<T>(IEnumerable<T> list, int toSkipPosition)
    {
      int valuePosition = 0;
      foreach (T obj in list)
      {
        if (valuePosition++ != toSkipPosition)
          yield return obj;
      }
    }

    public static IEnumerable<T> Concat<T>(T value, IEnumerable<T> rest)
    {
      yield return value;
      foreach (T obj in rest)
        yield return obj;
    }

    public static IEnumerable<IEnumerable<T>> Permute<T>(IEnumerable<T> list)
    {
      IEnumerable<T> rest = (IEnumerable<T>) null;
      int valuePosition = 0;
      foreach (T obj in list)
      {
        rest = RewritingProcessor<T_Tile>.AllButOne<T>(list, valuePosition++);
        foreach (IEnumerable<T> rest1 in RewritingProcessor<T_Tile>.Permute<T>(rest))
          yield return RewritingProcessor<T_Tile>.Concat<T>(obj, rest1);
      }
      if (rest == null)
        yield return list;
    }

    public static List<T> RandomPermutation<T>(IEnumerable<T> input)
    {
      List<T> objList = new List<T>(input);
      for (int index1 = 0; index1 < objList.Count; ++index1)
      {
        int index2 = RewritingProcessor<T_Tile>.rnd.Next(objList.Count);
        T obj = objList[index1];
        objList[index1] = objList[index2];
        objList[index2] = obj;
      }
      return objList;
    }

    public static IEnumerable<T> Reverse<T>(IEnumerable<T> input, HashSet<T> filter)
    {
      List<T> output = new List<T>(input);
      output.Reverse();
      foreach (T obj in output)
      {
        if (filter.Contains(obj))
          yield return obj;
      }
    }

    public bool RewriteQuery(
      T_Tile toFill,
      T_Tile toAvoid,
      IEnumerable<T_Tile> views,
      out T_Tile rewriting)
    {
      if (!this.RewriteQueryOnce(toFill, toAvoid, views, out rewriting))
        return false;
      HashSet<T_Tile> tileSet = new HashSet<T_Tile>();
      this.GatherViews(rewriting, tileSet);
      int num1 = this.CountOperators(rewriting);
      int num2 = 0;
      int num3 = Math.Min(0, Math.Max(0, (int) ((double) tileSet.Count * 0.0)));
      while (num2++ < num3)
      {
        IEnumerable<T_Tile> views1 = num2 != 1 ? (IEnumerable<T_Tile>) RewritingProcessor<T_Tile>.RandomPermutation<T_Tile>((IEnumerable<T_Tile>) tileSet) : RewritingProcessor<T_Tile>.Reverse<T_Tile>(views, tileSet);
        T_Tile rewriting1;
        this.RewriteQueryOnce(toFill, toAvoid, views1, out rewriting1);
        int num4 = this.CountOperators(rewriting1);
        if (num4 < num1)
        {
          num1 = num4;
          rewriting = rewriting1;
        }
        HashSet<T_Tile> views2 = new HashSet<T_Tile>();
        this.GatherViews(rewriting1, views2);
        tileSet = views2;
      }
      return true;
    }

    public bool RewriteQueryOnce(
      T_Tile toFill,
      T_Tile toAvoid,
      IEnumerable<T_Tile> views,
      out T_Tile rewriting)
    {
      List<T_Tile> views1 = new List<T_Tile>(views);
      return RewritingPass<T_Tile>.RewriteQuery(toFill, toAvoid, out rewriting, views1, this);
    }
  }
}
