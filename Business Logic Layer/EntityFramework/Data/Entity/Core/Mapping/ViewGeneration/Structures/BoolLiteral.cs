// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Structures.BoolLiteral
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Common.Utils.Boolean;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Structures
{
  internal abstract class BoolLiteral : InternalBase
  {
    internal static readonly IEqualityComparer<BoolLiteral> EqualityComparer = (IEqualityComparer<BoolLiteral>) new BoolLiteral.BoolLiteralComparer();
    internal static readonly IEqualityComparer<BoolLiteral> EqualityIdentifierComparer = (IEqualityComparer<BoolLiteral>) new BoolLiteral.IdentifierComparer();

    internal static TermExpr<DomainConstraint<BoolLiteral, Constant>> MakeTermExpression(
      BoolLiteral literal,
      IEnumerable<Constant> domain,
      IEnumerable<Constant> range)
    {
      Set<Constant> domain1 = new Set<Constant>(domain, Constant.EqualityComparer);
      Set<Constant> range1 = new Set<Constant>(range, Constant.EqualityComparer);
      return BoolLiteral.MakeTermExpression(literal, domain1, range1);
    }

    internal static TermExpr<DomainConstraint<BoolLiteral, Constant>> MakeTermExpression(
      BoolLiteral literal,
      Set<Constant> domain,
      Set<Constant> range)
    {
      domain.MakeReadOnly();
      range.MakeReadOnly();
      return new TermExpr<DomainConstraint<BoolLiteral, Constant>>((IEqualityComparer<DomainConstraint<BoolLiteral, Constant>>) System.Collections.Generic.EqualityComparer<DomainConstraint<BoolLiteral, Constant>>.Default, new DomainConstraint<BoolLiteral, Constant>(new DomainVariable<BoolLiteral, Constant>(literal, domain, BoolLiteral.EqualityIdentifierComparer), range));
    }

    internal abstract BoolExpr<DomainConstraint<BoolLiteral, Constant>> FixRange(
      Set<Constant> range,
      MemberDomainMap memberDomainMap);

    internal abstract BoolExpr<DomainConstraint<BoolLiteral, Constant>> GetDomainBoolExpression(
      MemberDomainMap domainMap);

    internal abstract BoolLiteral RemapBool(Dictionary<MemberPath, MemberPath> remap);

    internal abstract void GetRequiredSlots(
      MemberProjectionIndex projectedSlotMap,
      bool[] requiredSlots);

    internal abstract StringBuilder AsEsql(
      StringBuilder builder,
      string blockAlias,
      bool skipIsNotNull);

    internal abstract DbExpression AsCqt(DbExpression row, bool skipIsNotNull);

    internal abstract StringBuilder AsUserString(
      StringBuilder builder,
      string blockAlias,
      bool skipIsNotNull);

    internal abstract StringBuilder AsNegatedUserString(
      StringBuilder builder,
      string blockAlias,
      bool skipIsNotNull);

    protected virtual bool IsIdentifierEqualTo(BoolLiteral right)
    {
      return this.IsEqualTo(right);
    }

    protected abstract bool IsEqualTo(BoolLiteral right);

    protected virtual int GetIdentifierHash()
    {
      return this.GetHashCode();
    }

    private sealed class BoolLiteralComparer : IEqualityComparer<BoolLiteral>
    {
      public bool Equals(BoolLiteral left, BoolLiteral right)
      {
        if (object.ReferenceEquals((object) left, (object) right))
          return true;
        if (left == null || right == null)
          return false;
        return left.IsEqualTo(right);
      }

      public int GetHashCode(BoolLiteral literal)
      {
        return literal.GetHashCode();
      }
    }

    private sealed class IdentifierComparer : IEqualityComparer<BoolLiteral>
    {
      public bool Equals(BoolLiteral left, BoolLiteral right)
      {
        if (object.ReferenceEquals((object) left, (object) right))
          return true;
        if (left == null || right == null)
          return false;
        return left.IsIdentifierEqualTo(right);
      }

      public int GetHashCode(BoolLiteral literal)
      {
        return literal.GetIdentifierHash();
      }
    }
  }
}
