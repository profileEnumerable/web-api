// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.Boolean.Converter`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Linq;

namespace System.Data.Entity.Core.Common.Utils.Boolean
{
  internal sealed class Converter<T_Identifier>
  {
    private readonly Vertex _vertex;
    private readonly ConversionContext<T_Identifier> _context;
    private DnfSentence<T_Identifier> _dnf;
    private CnfSentence<T_Identifier> _cnf;

    internal Converter(BoolExpr<T_Identifier> expr, ConversionContext<T_Identifier> context)
    {
      this._context = context ?? IdentifierService<T_Identifier>.Instance.CreateConversionContext();
      this._vertex = ToDecisionDiagramConverter<T_Identifier>.TranslateToRobdd(expr, this._context);
    }

    internal Vertex Vertex
    {
      get
      {
        return this._vertex;
      }
    }

    internal DnfSentence<T_Identifier> Dnf
    {
      get
      {
        this.InitializeNormalForms();
        return this._dnf;
      }
    }

    internal CnfSentence<T_Identifier> Cnf
    {
      get
      {
        this.InitializeNormalForms();
        return this._cnf;
      }
    }

    private void InitializeNormalForms()
    {
      if (this._cnf != null)
        return;
      if (this._vertex.IsOne())
      {
        this._cnf = new CnfSentence<T_Identifier>(Set<CnfClause<T_Identifier>>.Empty);
        this._dnf = new DnfSentence<T_Identifier>(new Set<DnfClause<T_Identifier>>()
        {
          new DnfClause<T_Identifier>(Set<Literal<T_Identifier>>.Empty)
        }.MakeReadOnly());
      }
      else if (this._vertex.IsZero())
      {
        this._cnf = new CnfSentence<T_Identifier>(new Set<CnfClause<T_Identifier>>()
        {
          new CnfClause<T_Identifier>(Set<Literal<T_Identifier>>.Empty)
        }.MakeReadOnly());
        this._dnf = new DnfSentence<T_Identifier>(Set<DnfClause<T_Identifier>>.Empty);
      }
      else
      {
        Set<DnfClause<T_Identifier>> dnfClauses = new Set<DnfClause<T_Identifier>>();
        Set<CnfClause<T_Identifier>> cnfClauses = new Set<CnfClause<T_Identifier>>();
        Set<Literal<T_Identifier>> path = new Set<Literal<T_Identifier>>();
        this.FindAllPaths(this._vertex, cnfClauses, dnfClauses, path);
        this._cnf = new CnfSentence<T_Identifier>(cnfClauses.MakeReadOnly());
        this._dnf = new DnfSentence<T_Identifier>(dnfClauses.MakeReadOnly());
      }
    }

    private void FindAllPaths(
      Vertex vertex,
      Set<CnfClause<T_Identifier>> cnfClauses,
      Set<DnfClause<T_Identifier>> dnfClauses,
      Set<Literal<T_Identifier>> path)
    {
      if (vertex.IsOne())
      {
        DnfClause<T_Identifier> element = new DnfClause<T_Identifier>(path);
        dnfClauses.Add(element);
      }
      else if (vertex.IsZero())
      {
        CnfClause<T_Identifier> element = new CnfClause<T_Identifier>(new Set<Literal<T_Identifier>>(path.Select<Literal<T_Identifier>, Literal<T_Identifier>>((Func<Literal<T_Identifier>, Literal<T_Identifier>>) (l => l.MakeNegated()))));
        cnfClauses.Add(element);
      }
      else
      {
        foreach (LiteralVertexPair<T_Identifier> successor in this._context.GetSuccessors(vertex))
        {
          path.Add(successor.Literal);
          this.FindAllPaths(successor.Vertex, cnfClauses, dnfClauses, path);
          path.Remove(successor.Literal);
        }
      }
    }
  }
}
