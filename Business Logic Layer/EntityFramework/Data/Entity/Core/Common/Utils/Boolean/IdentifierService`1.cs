// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.Boolean.IdentifierService`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.Common.Utils.Boolean
{
  internal abstract class IdentifierService<T_Identifier>
  {
    internal static readonly IdentifierService<T_Identifier> Instance = IdentifierService<T_Identifier>.GetIdentifierService();

    private static IdentifierService<T_Identifier> GetIdentifierService()
    {
      Type type1 = typeof (T_Identifier);
      if (!type1.IsGenericType() || !(type1.GetGenericTypeDefinition() == typeof (DomainConstraint<,>)))
        return (IdentifierService<T_Identifier>) new IdentifierService<T_Identifier>.GenericIdentifierService();
      Type[] genericArguments = type1.GetGenericArguments();
      Type type2 = genericArguments[0];
      Type type3 = genericArguments[1];
      return (IdentifierService<T_Identifier>) Activator.CreateInstance(typeof (IdentifierService<>.DomainConstraintIdentifierService<,>).MakeGenericType(type1, type2, type3));
    }

    private IdentifierService()
    {
    }

    internal abstract Literal<T_Identifier> NegateLiteral(Literal<T_Identifier> literal);

    internal abstract ConversionContext<T_Identifier> CreateConversionContext();

    internal abstract BoolExpr<T_Identifier> LocalSimplify(BoolExpr<T_Identifier> expression);

    private class GenericIdentifierService : IdentifierService<T_Identifier>
    {
      internal override Literal<T_Identifier> NegateLiteral(Literal<T_Identifier> literal)
      {
        return new Literal<T_Identifier>(literal.Term, !literal.IsTermPositive);
      }

      internal override ConversionContext<T_Identifier> CreateConversionContext()
      {
        return (ConversionContext<T_Identifier>) new GenericConversionContext<T_Identifier>();
      }

      internal override BoolExpr<T_Identifier> LocalSimplify(
        BoolExpr<T_Identifier> expression)
      {
        return expression.Accept<BoolExpr<T_Identifier>>((Visitor<T_Identifier, BoolExpr<T_Identifier>>) Simplifier<T_Identifier>.Instance);
      }
    }

    private class DomainConstraintIdentifierService<T_Variable, T_Element> : IdentifierService<DomainConstraint<T_Variable, T_Element>>
    {
      internal override Literal<DomainConstraint<T_Variable, T_Element>> NegateLiteral(
        Literal<DomainConstraint<T_Variable, T_Element>> literal)
      {
        return new Literal<DomainConstraint<T_Variable, T_Element>>(new TermExpr<DomainConstraint<T_Variable, T_Element>>(literal.Term.Identifier.InvertDomainConstraint()), literal.IsTermPositive);
      }

      internal override ConversionContext<DomainConstraint<T_Variable, T_Element>> CreateConversionContext()
      {
        return (ConversionContext<DomainConstraint<T_Variable, T_Element>>) new DomainConstraintConversionContext<T_Variable, T_Element>();
      }

      internal override BoolExpr<DomainConstraint<T_Variable, T_Element>> LocalSimplify(
        BoolExpr<DomainConstraint<T_Variable, T_Element>> expression)
      {
        expression = NegationPusher.EliminateNot<T_Variable, T_Element>(expression);
        return expression.Accept<BoolExpr<DomainConstraint<T_Variable, T_Element>>>((Visitor<DomainConstraint<T_Variable, T_Element>, BoolExpr<DomainConstraint<T_Variable, T_Element>>>) Simplifier<DomainConstraint<T_Variable, T_Element>>.Instance);
      }
    }
  }
}
