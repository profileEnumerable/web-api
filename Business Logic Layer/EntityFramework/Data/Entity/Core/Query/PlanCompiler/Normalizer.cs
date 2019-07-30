// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.Normalizer
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Query.InternalTrees;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal class Normalizer : SubqueryTrackingVisitor
  {
    private Normalizer(System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler planCompilerState)
      : base(planCompilerState)
    {
    }

    internal static void Process(System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler planCompilerState)
    {
      new Normalizer(planCompilerState).Process();
    }

    private void Process()
    {
      this.m_command.Root = this.VisitNode(this.m_command.Root);
    }

    public override Node Visit(ExistsOp op, Node n)
    {
      this.VisitChildren(n);
      n.Child0 = this.BuildDummyProjectForExists(n.Child0);
      return n;
    }

    private Node BuildDummyProjectForExists(Node child)
    {
      Var projectVar;
      return this.m_command.BuildProject(child, this.m_command.CreateNode((Op) this.m_command.CreateInternalConstantOp(this.m_command.IntegerType, (object) 1)), out projectVar);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private Node BuildUnnest(Node collectionNode)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(collectionNode.Op.IsScalarOp, "non-scalar usage of Un-nest?");
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(TypeSemantics.IsCollectionType(collectionNode.Op.Type), "non-collection usage for Un-nest?");
      Var computedVar;
      Node varDefNode = this.m_command.CreateVarDefNode(collectionNode, out computedVar);
      return this.m_command.CreateNode((Op) this.m_command.CreateUnnestOp(computedVar), varDefNode);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private Node VisitCollectionFunction(FunctionOp op, Node n)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(TypeSemantics.IsCollectionType(op.Type), "non-TVF function?");
      Node node1 = this.BuildUnnest(n);
      Node node2 = this.m_command.CreateNode((Op) this.m_command.CreatePhysicalProjectOp((node1.Op as UnnestOp).Table.Columns[0]), node1);
      return this.m_command.CreateNode((Op) this.m_command.CreateCollectOp(n.Op.Type), node2);
    }

    private Node VisitCollectionAggregateFunction(FunctionOp op, Node n)
    {
      TypeUsage type = (TypeUsage) null;
      Node child0 = n.Child0;
      if (OpType.SoftCast == child0.Op.OpType)
      {
        type = TypeHelpers.GetEdmType<CollectionType>(child0.Op.Type).TypeUsage;
        child0 = child0.Child0;
        while (OpType.SoftCast == child0.Op.OpType)
          child0 = child0.Child0;
      }
      Node node1 = this.BuildUnnest(child0);
      Var column = (node1.Op as UnnestOp).Table.Columns[0];
      AggregateOp aggregateOp = this.m_command.CreateAggregateOp(op.Function, false);
      Node node2 = this.m_command.CreateNode((Op) this.m_command.CreateVarRefOp(column));
      if (type != null)
        node2 = this.m_command.CreateNode((Op) this.m_command.CreateSoftCastOp(type), node2);
      Node node3 = this.m_command.CreateNode((Op) aggregateOp, node2);
      VarVec varVec1 = this.m_command.CreateVarVec();
      Node node4 = this.m_command.CreateNode((Op) this.m_command.CreateVarDefListOp());
      VarVec varVec2 = this.m_command.CreateVarVec();
      Var computedVar;
      Node varDefListNode = this.m_command.CreateVarDefListNode(node3, out computedVar);
      varVec2.Set(computedVar);
      Node node5 = this.m_command.CreateNode((Op) this.m_command.CreateGroupByOp(varVec1, varVec2), node1, node4, varDefListNode);
      return this.AddSubqueryToParentRelOp(computedVar, node5);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "functionOp")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    public override Node Visit(FunctionOp op, Node n)
    {
      this.VisitScalarOpDefault((ScalarOp) op, n);
      Node node = !TypeSemantics.IsCollectionType(op.Type) ? (!PlanCompilerUtil.IsCollectionAggregateFunction(op, n) ? n : this.VisitCollectionAggregateFunction(op, n)) : this.VisitCollectionFunction(op, n);
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(node != null, "failure to construct a functionOp?");
      return node;
    }

    protected override Node VisitJoinOp(JoinBaseOp op, Node n)
    {
      if (this.ProcessJoinOp(n))
        n.Child2.Child0 = this.BuildDummyProjectForExists(n.Child2.Child0);
      return n;
    }
  }
}
