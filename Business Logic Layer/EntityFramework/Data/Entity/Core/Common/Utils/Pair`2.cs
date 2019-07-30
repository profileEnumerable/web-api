// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.Pair`2
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Text;

namespace System.Data.Entity.Core.Common.Utils
{
  internal class Pair<TFirst, TSecond> : InternalBase
  {
    private readonly TFirst first;
    private readonly TSecond second;

    internal Pair(TFirst first, TSecond second)
    {
      this.first = first;
      this.second = second;
    }

    internal TFirst First
    {
      get
      {
        return this.first;
      }
    }

    internal TSecond Second
    {
      get
      {
        return this.second;
      }
    }

    public override int GetHashCode()
    {
      return this.first.GetHashCode() << 5 ^ this.second.GetHashCode();
    }

    public bool Equals(Pair<TFirst, TSecond> other)
    {
      if (this.first.Equals((object) other.first))
        return this.second.Equals((object) other.second);
      return false;
    }

    public override bool Equals(object other)
    {
      Pair<TFirst, TSecond> other1 = other as Pair<TFirst, TSecond>;
      if (other1 != null)
        return this.Equals(other1);
      return false;
    }

    internal override void ToCompactString(StringBuilder builder)
    {
      builder.Append("<");
      builder.Append((object) this.first);
      builder.Append(", " + (object) this.second);
      builder.Append(">");
    }

    internal class PairComparer : IEqualityComparer<Pair<TFirst, TSecond>>
    {
      internal static readonly Pair<TFirst, TSecond>.PairComparer Instance = new Pair<TFirst, TSecond>.PairComparer();
      private static readonly EqualityComparer<TFirst> _firstComparer = EqualityComparer<TFirst>.Default;
      private static readonly EqualityComparer<TSecond> _secondComparer = EqualityComparer<TSecond>.Default;

      private PairComparer()
      {
      }

      public bool Equals(Pair<TFirst, TSecond> x, Pair<TFirst, TSecond> y)
      {
        if (Pair<TFirst, TSecond>.PairComparer._firstComparer.Equals(x.First, y.First))
          return Pair<TFirst, TSecond>.PairComparer._secondComparer.Equals(x.Second, y.Second);
        return false;
      }

      public int GetHashCode(Pair<TFirst, TSecond> source)
      {
        return source.GetHashCode();
      }
    }
  }
}
