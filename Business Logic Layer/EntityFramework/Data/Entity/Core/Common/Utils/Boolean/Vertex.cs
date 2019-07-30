// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.Boolean.Vertex
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace System.Data.Entity.Core.Common.Utils.Boolean
{
  internal sealed class Vertex : IEquatable<Vertex>
  {
    internal static readonly Vertex One = new Vertex();
    internal static readonly Vertex Zero = new Vertex();
    internal readonly int Variable;
    internal readonly Vertex[] Children;

    private Vertex()
    {
      this.Variable = int.MaxValue;
      this.Children = new Vertex[0];
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.EntityUtil.BoolExprAssert(System.Boolean,System.String)")]
    internal Vertex(int variable, Vertex[] children)
    {
      if (variable >= int.MaxValue)
        throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.BoolExprAssert, 0, (object) "exceeded number of supported variables");
      this.Variable = variable;
      this.Children = children;
    }

    [Conditional("DEBUG")]
    private static void AssertConstructorArgumentsValid(int variable, Vertex[] children)
    {
      foreach (Vertex child in children)
        ;
    }

    internal bool IsOne()
    {
      return object.ReferenceEquals((object) Vertex.One, (object) this);
    }

    internal bool IsZero()
    {
      return object.ReferenceEquals((object) Vertex.Zero, (object) this);
    }

    internal bool IsSink()
    {
      return this.Variable == int.MaxValue;
    }

    public bool Equals(Vertex other)
    {
      return object.ReferenceEquals((object) this, (object) other);
    }

    public override bool Equals(object obj)
    {
      return base.Equals(obj);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    public override string ToString()
    {
      if (this.IsOne())
        return "_1_";
      if (this.IsZero())
        return "_0_";
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<{0}, {1}>", (object) this.Variable, (object) StringUtil.ToCommaSeparatedString((IEnumerable) this.Children));
    }
  }
}
