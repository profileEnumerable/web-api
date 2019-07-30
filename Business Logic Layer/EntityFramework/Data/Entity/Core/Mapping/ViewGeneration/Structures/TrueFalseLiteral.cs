// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Structures.TrueFalseLiteral
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Common.Utils.Boolean;
using System.Linq;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Structures
{
  internal abstract class TrueFalseLiteral : BoolLiteral
  {
    internal override BoolExpr<DomainConstraint<BoolLiteral, Constant>> GetDomainBoolExpression(
      MemberDomainMap domainMap)
    {
      IEnumerable<Constant> elements = (IEnumerable<Constant>) new Constant[1]
      {
        (Constant) new ScalarConstant((object) true)
      };
      return (BoolExpr<DomainConstraint<BoolLiteral, Constant>>) BoolLiteral.MakeTermExpression((BoolLiteral) this, new Set<Constant>((IEnumerable<Constant>) new Constant[2]
      {
        (Constant) new ScalarConstant((object) true),
        (Constant) new ScalarConstant((object) false)
      }, Constant.EqualityComparer).MakeReadOnly(), new Set<Constant>(elements, Constant.EqualityComparer).MakeReadOnly());
    }

    internal override BoolExpr<DomainConstraint<BoolLiteral, Constant>> FixRange(
      Set<Constant> range,
      MemberDomainMap memberDomainMap)
    {
      ScalarConstant scalarConstant = (ScalarConstant) range.First<Constant>();
      BoolExpr<DomainConstraint<BoolLiteral, Constant>> child = this.GetDomainBoolExpression(memberDomainMap);
      if (!(bool) scalarConstant.Value)
        child = (BoolExpr<DomainConstraint<BoolLiteral, Constant>>) new NotExpr<DomainConstraint<BoolLiteral, Constant>>(child);
      return child;
    }
  }
}
