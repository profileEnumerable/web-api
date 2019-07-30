// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.Span
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Internal;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Text;

namespace System.Data.Entity.Core.Objects
{
  internal sealed class Span
  {
    private readonly List<Span.SpanPath> _spanList;
    private string _cacheKey;

    internal Span()
    {
      this._spanList = new List<Span.SpanPath>();
    }

    internal List<Span.SpanPath> SpanList
    {
      get
      {
        return this._spanList;
      }
    }

    internal static bool RequiresRelationshipSpan(MergeOption mergeOption)
    {
      return mergeOption != MergeOption.NoTracking;
    }

    internal static Span IncludeIn(Span spanToIncludeIn, string pathToInclude)
    {
      if (spanToIncludeIn == null)
        spanToIncludeIn = new Span();
      spanToIncludeIn.Include(pathToInclude);
      return spanToIncludeIn;
    }

    internal static Span CopyUnion(Span span1, Span span2)
    {
      if (span1 == null)
        return span2;
      if (span2 == null)
        return span1;
      Span span3 = span1.Clone();
      foreach (Span.SpanPath span4 in span2.SpanList)
        span3.AddSpanPath(span4);
      return span3;
    }

    internal string GetCacheKey()
    {
      if (this._cacheKey == null && this._spanList.Count > 0)
      {
        if (this._spanList.Count == 1 && this._spanList[0].Navigations.Count == 1)
        {
          this._cacheKey = this._spanList[0].Navigations[0];
        }
        else
        {
          StringBuilder stringBuilder = new StringBuilder();
          for (int index1 = 0; index1 < this._spanList.Count; ++index1)
          {
            if (index1 > 0)
              stringBuilder.Append(";");
            Span.SpanPath span = this._spanList[index1];
            stringBuilder.Append(span.Navigations[0]);
            for (int index2 = 1; index2 < span.Navigations.Count; ++index2)
            {
              stringBuilder.Append(".");
              stringBuilder.Append(span.Navigations[index2]);
            }
          }
          this._cacheKey = stringBuilder.ToString();
        }
      }
      return this._cacheKey;
    }

    public void Include(string path)
    {
      Check.NotEmpty(path, nameof (path));
      this.AddSpanPath(new Span.SpanPath(Span.ParsePath(path)));
      this._cacheKey = (string) null;
    }

    internal Span Clone()
    {
      Span span = new Span();
      span.SpanList.AddRange((IEnumerable<Span.SpanPath>) this._spanList);
      span._cacheKey = this._cacheKey;
      return span;
    }

    internal void AddSpanPath(Span.SpanPath spanPath)
    {
      if (!this.ValidateSpanPath(spanPath))
        return;
      this.RemoveExistingSubPaths(spanPath);
      this._spanList.Add(spanPath);
    }

    private bool ValidateSpanPath(Span.SpanPath spanPath)
    {
      for (int index = 0; index < this._spanList.Count; ++index)
      {
        if (spanPath.IsSubPath(this._spanList[index]))
          return false;
      }
      return true;
    }

    private void RemoveExistingSubPaths(Span.SpanPath spanPath)
    {
      List<Span.SpanPath> spanPathList = new List<Span.SpanPath>();
      for (int index = 0; index < this._spanList.Count; ++index)
      {
        if (this._spanList[index].IsSubPath(spanPath))
          spanPathList.Add(this._spanList[index]);
      }
      foreach (Span.SpanPath spanPath1 in spanPathList)
        this._spanList.Remove(spanPath1);
    }

    private static List<string> ParsePath(string path)
    {
      List<string> multipartIdentifier = MultipartIdentifier.ParseMultipartIdentifier(path, "[", "]", '.');
      for (int index = multipartIdentifier.Count - 1; index >= 0; --index)
      {
        if (multipartIdentifier[index] == null)
          multipartIdentifier.RemoveAt(index);
        else if (multipartIdentifier[index].Length == 0)
          throw new ArgumentException(Strings.ObjectQuery_Span_SpanPathSyntaxError);
      }
      return multipartIdentifier;
    }

    internal class SpanPath
    {
      public readonly List<string> Navigations;

      public SpanPath(List<string> navigations)
      {
        this.Navigations = navigations;
      }

      public bool IsSubPath(Span.SpanPath rhs)
      {
        if (this.Navigations.Count > rhs.Navigations.Count)
          return false;
        for (int index = 0; index < this.Navigations.Count; ++index)
        {
          if (!this.Navigations[index].Equals(rhs.Navigations[index], StringComparison.OrdinalIgnoreCase))
            return false;
        }
        return true;
      }
    }
  }
}
