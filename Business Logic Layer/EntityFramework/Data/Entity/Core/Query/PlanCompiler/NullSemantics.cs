// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.NullSemantics
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Query.InternalTrees;
using System.Linq;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal class NullSemantics : BasicOpVisitorOfNode
  {
    private NullSemantics.VariableNullabilityTable _variableNullabilityTable = new NullSemantics.VariableNullabilityTable(32);
    private Command _command;
    private bool _modified;
    private bool _negated;

    private NullSemantics(Command command)
    {
      this._command = command;
    }

    public static bool Process(Command command)
    {
      NullSemantics nullSemantics = new NullSemantics(command);
      command.Root = nullSemantics.VisitNode(command.Root);
      return nullSemantics._modified;
    }

    protected override Node VisitDefault(Node n)
    {
      bool negated = this._negated;
      switch (n.Op.OpType)
      {
        case OpType.EQ:
          this._negated = false;
          n = this.HandleEQ(n, negated);
          break;
        case OpType.NE:
          n = this.HandleNE(n);
          break;
        case OpType.And:
          n = base.VisitDefault(n);
          break;
        case OpType.Or:
          n = this.HandleOr(n);
          break;
        case OpType.Not:
          this._negated = !this._negated;
          n = base.VisitDefault(n);
          break;
        default:
          this._negated = false;
          n = base.VisitDefault(n);
          break;
      }
      this._negated = negated;
      return n;
    }

    private Node HandleOr(Node n)
    {
      Node node = n.Child0.Op.OpType == OpType.IsNull ? n.Child0 : (Node) null;
      if (node == null || node.Child0.Op.OpType != OpType.VarRef)
        return base.VisitDefault(n);
      Var var = ((VarRefOp) node.Child0.Op).Var;
      bool flag = this._variableNullabilityTable[var];
      this._variableNullabilityTable[var] = false;
      n.Child1 = this.VisitNode(n.Child1);
      this._variableNullabilityTable[var] = flag;
      return n;
    }

    private Node HandleEQ(Node n, bool negated)
    {
      NullSemantics nullSemantics = this;
      nullSemantics._modified = ((nullSemantics._modified ? 1 : 0) | (!object.ReferenceEquals((object) n.Child0, (object) (n.Child0 = this.VisitNode(n.Child0))) || !object.ReferenceEquals((object) n.Child1, (object) (n.Child1 = this.VisitNode(n.Child1))) ? 1 : (!object.ReferenceEquals((object) n, (object) (n = this.ImplementEquality(n, negated))) ? 1 : 0))) != 0;
      return n;
    }

    private Node HandleNE(Node n)
    {
      n = this._command.CreateNode((Op) this._command.CreateConditionalOp(OpType.Not), this._command.CreateNode((Op) this._command.CreateComparisonOp(OpType.EQ, ((ComparisonOp) n.Op).UseDatabaseNullSemantics), n.Child0, n.Child1));
      this._modified = true;
      return base.VisitDefault(n);
    }

    private bool IsNullableVarRef(Node n)
    {
      if (n.Op.OpType == OpType.VarRef)
        return this._variableNullabilityTable[((VarRefOp) n.Op).Var];
      return false;
    }

    private Node ImplementEquality(Node n, bool negated)
    {
      if (((ComparisonOp) n.Op).UseDatabaseNullSemantics)
        return n;
      Node child0 = n.Child0;
      Node child1 = n.Child1;
      switch (child0.Op.OpType)
      {
        case OpType.Constant:
        case OpType.InternalConstant:
        case OpType.NullSentinel:
          switch (child1.Op.OpType)
          {
            case OpType.Constant:
            case OpType.InternalConstant:
            case OpType.NullSentinel:
              return n;
            case OpType.Null:
              return this.False();
            default:
              if (!negated)
                return n;
              return this.And(n, this.Not(this.IsNull(this.Clone(child1))));
          }
        case OpType.Null:
          switch (child1.Op.OpType)
          {
            case OpType.Constant:
            case OpType.InternalConstant:
            case OpType.NullSentinel:
              return this.False();
            case OpType.Null:
              return this.True();
            default:
              return this.IsNull(child1);
          }
        default:
          switch (child1.Op.OpType)
          {
            case OpType.Constant:
            case OpType.InternalConstant:
            case OpType.NullSentinel:
              if (!negated || !this.IsNullableVarRef(n))
                return n;
              return this.And(n, this.Not(this.IsNull(this.Clone(child0))));
            case OpType.Null:
              return this.IsNull(child0);
            default:
              if (!negated)
                return this.Or(n, this.And(this.IsNull(this.Clone(child0)), this.IsNull(this.Clone(child1))));
              return this.And(n, this.NotXor(this.Clone(child0), this.Clone(child1)));
          }
      }
    }

    private Node Clone(Node x)
    {
      return OpCopier.Copy(this._command, x);
    }

    private Node False()
    {
      return this._command.CreateNode((Op) this._command.CreateFalseOp());
    }

    private Node True()
    {
      return this._command.CreateNode((Op) this._command.CreateTrueOp());
    }

    private Node IsNull(Node x)
    {
      return this._command.CreateNode((Op) this._command.CreateConditionalOp(OpType.IsNull), x);
    }

    private Node Not(Node x)
    {
      return this._command.CreateNode((Op) this._command.CreateConditionalOp(OpType.Not), x);
    }

    private Node And(Node x, Node y)
    {
      return this._command.CreateNode((Op) this._command.CreateConditionalOp(OpType.And), x, y);
    }

    private Node Or(Node x, Node y)
    {
      return this._command.CreateNode((Op) this._command.CreateConditionalOp(OpType.Or), x, y);
    }

    private Node Boolean(bool value)
    {
      return this._command.CreateNode((Op) this._command.CreateConstantOp(this._command.BooleanType, (object) value));
    }

    private Node NotXor(Node x, Node y)
    {
      return this._command.CreateNode((Op) this._command.CreateComparisonOp(OpType.EQ, false), this._command.CreateNode((Op) this._command.CreateCaseOp(this._command.BooleanType), this.IsNull(x), this.Boolean(true), this.Boolean(false)), this._command.CreateNode((Op) this._command.CreateCaseOp(this._command.BooleanType), this.IsNull(y), this.Boolean(true), this.Boolean(false)));
    }

    private struct VariableNullabilityTable
    {
      private bool[] _entries;

      public VariableNullabilityTable(int capacity)
      {
        this._entries = Enumerable.Repeat<bool>(true, capacity).ToArray<bool>();
      }

      public bool this[Var variable]
      {
        get
        {
          if (variable.Id < this._entries.Length)
            return this._entries[variable.Id];
          return true;
        }
        set
        {
          this.EnsureCapacity(variable.Id + 1);
          this._entries[variable.Id] = value;
        }
      }

      private void EnsureCapacity(int minimum)
      {
        if (this._entries.Length >= minimum)
          return;
        int count = this._entries.Length * 2;
        if (count < minimum)
          count = minimum;
        bool[] array = Enumerable.Repeat<bool>(true, count).ToArray<bool>();
        Array.Copy((Array) this._entries, 0, (Array) array, 0, this._entries.Length);
        this._entries = array;
      }
    }
  }
}
