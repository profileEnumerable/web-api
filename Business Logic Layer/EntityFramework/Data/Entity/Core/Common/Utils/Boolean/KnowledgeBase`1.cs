// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.Boolean.KnowledgeBase`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Text;

namespace System.Data.Entity.Core.Common.Utils.Boolean
{
  internal class KnowledgeBase<T_Identifier>
  {
    private readonly List<BoolExpr<T_Identifier>> _facts;
    private Vertex _knowledge;
    private readonly ConversionContext<T_Identifier> _context;

    internal KnowledgeBase()
    {
      this._facts = new List<BoolExpr<T_Identifier>>();
      this._knowledge = Vertex.One;
      this._context = IdentifierService<T_Identifier>.Instance.CreateConversionContext();
    }

    protected IEnumerable<BoolExpr<T_Identifier>> Facts
    {
      get
      {
        return (IEnumerable<BoolExpr<T_Identifier>>) this._facts;
      }
    }

    internal void AddKnowledgeBase(KnowledgeBase<T_Identifier> kb)
    {
      foreach (BoolExpr<T_Identifier> fact in kb._facts)
        this.AddFact(fact);
    }

    internal virtual void AddFact(BoolExpr<T_Identifier> fact)
    {
      this._facts.Add(fact);
      this._knowledge = this._context.Solver.And(this._knowledge, new Converter<T_Identifier>(fact, this._context).Vertex);
    }

    internal void AddImplication(BoolExpr<T_Identifier> condition, BoolExpr<T_Identifier> implies)
    {
      this.AddFact((BoolExpr<T_Identifier>) new KnowledgeBase<T_Identifier>.Implication(condition, implies));
    }

    internal void AddEquivalence(BoolExpr<T_Identifier> left, BoolExpr<T_Identifier> right)
    {
      this.AddFact((BoolExpr<T_Identifier>) new KnowledgeBase<T_Identifier>.Equivalence(left, right));
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("Facts:");
      foreach (BoolExpr<T_Identifier> fact in this._facts)
        stringBuilder.Append("\t").AppendLine(fact.ToString());
      return stringBuilder.ToString();
    }

    protected class Implication : OrExpr<T_Identifier>
    {
      private readonly BoolExpr<T_Identifier> _condition;
      private readonly BoolExpr<T_Identifier> _implies;

      internal BoolExpr<T_Identifier> Condition
      {
        get
        {
          return this._condition;
        }
      }

      internal BoolExpr<T_Identifier> Implies
      {
        get
        {
          return this._implies;
        }
      }

      internal Implication(BoolExpr<T_Identifier> condition, BoolExpr<T_Identifier> implies)
        : base(condition.MakeNegated(), implies)
      {
        this._condition = condition;
        this._implies = implies;
      }

      public override string ToString()
      {
        return StringUtil.FormatInvariant("{0} --> {1}", (object) this._condition, (object) this._implies);
      }
    }

    protected class Equivalence : AndExpr<T_Identifier>
    {
      private readonly BoolExpr<T_Identifier> _left;
      private readonly BoolExpr<T_Identifier> _right;

      internal BoolExpr<T_Identifier> Left
      {
        get
        {
          return this._left;
        }
      }

      internal BoolExpr<T_Identifier> Right
      {
        get
        {
          return this._right;
        }
      }

      internal Equivalence(BoolExpr<T_Identifier> left, BoolExpr<T_Identifier> right)
        : base((BoolExpr<T_Identifier>) new KnowledgeBase<T_Identifier>.Implication(left, right), (BoolExpr<T_Identifier>) new KnowledgeBase<T_Identifier>.Implication(right, left))
      {
        this._left = left;
        this._right = right;
      }

      public override string ToString()
      {
        return StringUtil.FormatInvariant("{0} <--> {1}", (object) this._left, (object) this._right);
      }
    }
  }
}
