// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.OpCopier
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
  internal class OpCopier : BasicOpVisitorOfNode
  {
    private readonly Command m_srcCmd;
    protected Command m_destCmd;
    protected VarMap m_varMap;

    internal static Node Copy(Command cmd, Node n)
    {
      VarMap varMap;
      return OpCopier.Copy(cmd, n, out varMap);
    }

    internal static Node Copy(Command cmd, Node node, VarList varList, out VarList newVarList)
    {
      VarMap varMap;
      Node node1 = OpCopier.Copy(cmd, node, out varMap);
      newVarList = Command.CreateVarList();
      foreach (Var var1 in (List<Var>) varList)
      {
        Var var2 = varMap[var1];
        newVarList.Add(var2);
      }
      return node1;
    }

    internal static Node Copy(Command cmd, Node n, out VarMap varMap)
    {
      OpCopier opCopier = new OpCopier(cmd);
      Node node = opCopier.CopyNode(n);
      varMap = opCopier.m_varMap;
      return node;
    }

    internal static List<SortKey> Copy(Command cmd, List<SortKey> sortKeys)
    {
      return new OpCopier(cmd).Copy(sortKeys);
    }

    protected OpCopier(Command cmd)
      : this(cmd, cmd)
    {
    }

    private OpCopier(Command destCommand, Command sourceCommand)
    {
      this.m_srcCmd = sourceCommand;
      this.m_destCmd = destCommand;
      this.m_varMap = new VarMap();
    }

    private Var GetMappedVar(Var v)
    {
      Var var;
      if (this.m_varMap.TryGetValue(v, out var))
        return var;
      if (this.m_destCmd != this.m_srcCmd)
        throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.UnknownVar, 6, (object) null);
      return v;
    }

    private void SetMappedVar(Var v, Var mappedVar)
    {
      this.m_varMap.Add(v, mappedVar);
    }

    private void MapTable(Table newTable, Table oldTable)
    {
      for (int index = 0; index < oldTable.Columns.Count; ++index)
        this.SetMappedVar(oldTable.Columns[index], newTable.Columns[index]);
    }

    private IEnumerable<Var> MapVars(IEnumerable<Var> vars)
    {
      foreach (Var var in vars)
      {
        Var mappedVar = this.GetMappedVar(var);
        yield return mappedVar;
      }
    }

    private VarVec Copy(VarVec vars)
    {
      return this.m_destCmd.CreateVarVec(this.MapVars((IEnumerable<Var>) vars));
    }

    private VarList Copy(VarList varList)
    {
      return Command.CreateVarList(this.MapVars((IEnumerable<Var>) varList));
    }

    private SortKey Copy(SortKey sortKey)
    {
      return Command.CreateSortKey(this.GetMappedVar(sortKey.Var), sortKey.AscendingSort, sortKey.Collation);
    }

    private List<SortKey> Copy(List<SortKey> sortKeys)
    {
      List<SortKey> sortKeyList = new List<SortKey>();
      foreach (SortKey sortKey in sortKeys)
        sortKeyList.Add(this.Copy(sortKey));
      return sortKeyList;
    }

    protected Node CopyNode(Node n)
    {
      return n.Op.Accept<Node>((BasicOpVisitorOfT<Node>) this, n);
    }

    private List<Node> ProcessChildren(Node n)
    {
      List<Node> nodeList = new List<Node>();
      foreach (Node child in n.Children)
        nodeList.Add(this.CopyNode(child));
      return nodeList;
    }

    private Node CopyDefault(Op op, Node original)
    {
      return this.m_destCmd.CreateNode(op, this.ProcessChildren(original));
    }

    public override Node Visit(Op op, Node n)
    {
      throw new NotSupportedException(Strings.Iqt_General_UnsupportedOp((object) op.GetType().FullName));
    }

    public override Node Visit(ConstantOp op, Node n)
    {
      return this.m_destCmd.CreateNode((Op) this.m_destCmd.CreateConstantOp(op.Type, op.Value));
    }

    public override Node Visit(NullOp op, Node n)
    {
      return this.m_destCmd.CreateNode((Op) this.m_destCmd.CreateNullOp(op.Type));
    }

    public override Node Visit(ConstantPredicateOp op, Node n)
    {
      return this.m_destCmd.CreateNode((Op) this.m_destCmd.CreateConstantPredicateOp(op.Value));
    }

    public override Node Visit(InternalConstantOp op, Node n)
    {
      return this.m_destCmd.CreateNode((Op) this.m_destCmd.CreateInternalConstantOp(op.Type, op.Value));
    }

    public override Node Visit(NullSentinelOp op, Node n)
    {
      return this.m_destCmd.CreateNode((Op) this.m_destCmd.CreateNullSentinelOp());
    }

    public override Node Visit(FunctionOp op, Node n)
    {
      return this.CopyDefault((Op) this.m_destCmd.CreateFunctionOp(op.Function), n);
    }

    public override Node Visit(PropertyOp op, Node n)
    {
      return this.CopyDefault((Op) this.m_destCmd.CreatePropertyOp(op.PropertyInfo), n);
    }

    public override Node Visit(RelPropertyOp op, Node n)
    {
      return this.CopyDefault((Op) this.m_destCmd.CreateRelPropertyOp(op.PropertyInfo), n);
    }

    public override Node Visit(CaseOp op, Node n)
    {
      return this.CopyDefault((Op) this.m_destCmd.CreateCaseOp(op.Type), n);
    }

    public override Node Visit(ComparisonOp op, Node n)
    {
      return this.CopyDefault((Op) this.m_destCmd.CreateComparisonOp(op.OpType, op.UseDatabaseNullSemantics), n);
    }

    public override Node Visit(LikeOp op, Node n)
    {
      return this.CopyDefault((Op) this.m_destCmd.CreateLikeOp(), n);
    }

    public override Node Visit(AggregateOp op, Node n)
    {
      return this.CopyDefault((Op) this.m_destCmd.CreateAggregateOp(op.AggFunc, op.IsDistinctAggregate), n);
    }

    public override Node Visit(NewInstanceOp op, Node n)
    {
      return this.CopyDefault((Op) this.m_destCmd.CreateNewInstanceOp(op.Type), n);
    }

    public override Node Visit(NewEntityOp op, Node n)
    {
      return this.CopyDefault(!op.Scoped ? (Op) this.m_destCmd.CreateNewEntityOp(op.Type, op.RelationshipProperties) : (Op) this.m_destCmd.CreateScopedNewEntityOp(op.Type, op.RelationshipProperties, op.EntitySet), n);
    }

    public override Node Visit(DiscriminatedNewEntityOp op, Node n)
    {
      return this.CopyDefault((Op) this.m_destCmd.CreateDiscriminatedNewEntityOp(op.Type, op.DiscriminatorMap, op.EntitySet, op.RelationshipProperties), n);
    }

    public override Node Visit(NewMultisetOp op, Node n)
    {
      return this.CopyDefault((Op) this.m_destCmd.CreateNewMultisetOp(op.Type), n);
    }

    public override Node Visit(NewRecordOp op, Node n)
    {
      return this.CopyDefault((Op) this.m_destCmd.CreateNewRecordOp(op.Type), n);
    }

    public override Node Visit(RefOp op, Node n)
    {
      return this.CopyDefault((Op) this.m_destCmd.CreateRefOp(op.EntitySet, op.Type), n);
    }

    public override Node Visit(VarRefOp op, Node n)
    {
      Var var;
      if (!this.m_varMap.TryGetValue(op.Var, out var))
        var = op.Var;
      return this.m_destCmd.CreateNode((Op) this.m_destCmd.CreateVarRefOp(var));
    }

    public override Node Visit(ConditionalOp op, Node n)
    {
      return this.CopyDefault((Op) this.m_destCmd.CreateConditionalOp(op.OpType), n);
    }

    public override Node Visit(ArithmeticOp op, Node n)
    {
      return this.CopyDefault((Op) this.m_destCmd.CreateArithmeticOp(op.OpType, op.Type), n);
    }

    public override Node Visit(TreatOp op, Node n)
    {
      return this.CopyDefault(op.IsFakeTreat ? (Op) this.m_destCmd.CreateFakeTreatOp(op.Type) : (Op) this.m_destCmd.CreateTreatOp(op.Type), n);
    }

    public override Node Visit(CastOp op, Node n)
    {
      return this.CopyDefault((Op) this.m_destCmd.CreateCastOp(op.Type), n);
    }

    public override Node Visit(SoftCastOp op, Node n)
    {
      return this.CopyDefault((Op) this.m_destCmd.CreateSoftCastOp(op.Type), n);
    }

    public override Node Visit(DerefOp op, Node n)
    {
      return this.CopyDefault((Op) this.m_destCmd.CreateDerefOp(op.Type), n);
    }

    public override Node Visit(NavigateOp op, Node n)
    {
      return this.CopyDefault((Op) this.m_destCmd.CreateNavigateOp(op.Type, op.RelProperty), n);
    }

    public override Node Visit(IsOfOp op, Node n)
    {
      if (op.IsOfOnly)
        return this.CopyDefault((Op) this.m_destCmd.CreateIsOfOnlyOp(op.IsOfType), n);
      return this.CopyDefault((Op) this.m_destCmd.CreateIsOfOp(op.IsOfType), n);
    }

    public override Node Visit(ExistsOp op, Node n)
    {
      return this.CopyDefault((Op) this.m_destCmd.CreateExistsOp(), n);
    }

    public override Node Visit(ElementOp op, Node n)
    {
      return this.CopyDefault((Op) this.m_destCmd.CreateElementOp(op.Type), n);
    }

    public override Node Visit(GetRefKeyOp op, Node n)
    {
      return this.CopyDefault((Op) this.m_destCmd.CreateGetRefKeyOp(op.Type), n);
    }

    public override Node Visit(GetEntityRefOp op, Node n)
    {
      return this.CopyDefault((Op) this.m_destCmd.CreateGetEntityRefOp(op.Type), n);
    }

    public override Node Visit(CollectOp op, Node n)
    {
      return this.CopyDefault((Op) this.m_destCmd.CreateCollectOp(op.Type), n);
    }

    public override Node Visit(ScanTableOp op, Node n)
    {
      ScanTableOp scanTableOp = this.m_destCmd.CreateScanTableOp(op.Table.TableMetadata);
      this.MapTable(scanTableOp.Table, op.Table);
      return this.m_destCmd.CreateNode((Op) scanTableOp);
    }

    public override Node Visit(ScanViewOp op, Node n)
    {
      ScanViewOp scanViewOp = this.m_destCmd.CreateScanViewOp(op.Table.TableMetadata);
      this.MapTable(scanViewOp.Table, op.Table);
      List<Node> args = this.ProcessChildren(n);
      return this.m_destCmd.CreateNode((Op) scanViewOp, args);
    }

    public override Node Visit(UnnestOp op, Node n)
    {
      List<Node> args = this.ProcessChildren(n);
      UnnestOp unnestOp = this.m_destCmd.CreateUnnestOp(this.GetMappedVar(op.Var), this.m_destCmd.CreateTableInstance(op.Table.TableMetadata));
      this.MapTable(unnestOp.Table, op.Table);
      return this.m_destCmd.CreateNode((Op) unnestOp, args);
    }

    public override Node Visit(ProjectOp op, Node n)
    {
      List<Node> args = this.ProcessChildren(n);
      return this.m_destCmd.CreateNode((Op) this.m_destCmd.CreateProjectOp(this.Copy(op.Outputs)), args);
    }

    public override Node Visit(FilterOp op, Node n)
    {
      return this.CopyDefault((Op) this.m_destCmd.CreateFilterOp(), n);
    }

    public override Node Visit(SortOp op, Node n)
    {
      List<Node> args = this.ProcessChildren(n);
      return this.m_destCmd.CreateNode((Op) this.m_destCmd.CreateSortOp(this.Copy(op.Keys)), args);
    }

    public override Node Visit(ConstrainedSortOp op, Node n)
    {
      List<Node> args = this.ProcessChildren(n);
      return this.m_destCmd.CreateNode((Op) this.m_destCmd.CreateConstrainedSortOp(this.Copy(op.Keys), op.WithTies), args);
    }

    public override Node Visit(GroupByOp op, Node n)
    {
      List<Node> args = this.ProcessChildren(n);
      return this.m_destCmd.CreateNode((Op) this.m_destCmd.CreateGroupByOp(this.Copy(op.Keys), this.Copy(op.Outputs)), args);
    }

    public override Node Visit(GroupByIntoOp op, Node n)
    {
      List<Node> args = this.ProcessChildren(n);
      return this.m_destCmd.CreateNode((Op) this.m_destCmd.CreateGroupByIntoOp(this.Copy(op.Keys), this.Copy(op.Inputs), this.Copy(op.Outputs)), args);
    }

    public override Node Visit(CrossJoinOp op, Node n)
    {
      return this.CopyDefault((Op) this.m_destCmd.CreateCrossJoinOp(), n);
    }

    public override Node Visit(InnerJoinOp op, Node n)
    {
      return this.CopyDefault((Op) this.m_destCmd.CreateInnerJoinOp(), n);
    }

    public override Node Visit(LeftOuterJoinOp op, Node n)
    {
      return this.CopyDefault((Op) this.m_destCmd.CreateLeftOuterJoinOp(), n);
    }

    public override Node Visit(FullOuterJoinOp op, Node n)
    {
      return this.CopyDefault((Op) this.m_destCmd.CreateFullOuterJoinOp(), n);
    }

    public override Node Visit(CrossApplyOp op, Node n)
    {
      return this.CopyDefault((Op) this.m_destCmd.CreateCrossApplyOp(), n);
    }

    public override Node Visit(OuterApplyOp op, Node n)
    {
      return this.CopyDefault((Op) this.m_destCmd.CreateOuterApplyOp(), n);
    }

    private Node CopySetOp(SetOp op, Node n)
    {
      List<Node> args = this.ProcessChildren(n);
      VarMap leftMap = new VarMap();
      VarMap rightMap = new VarMap();
      foreach (KeyValuePair<Var, Var> keyValuePair in (Dictionary<Var, Var>) op.VarMap[0])
      {
        Var setOpVar = (Var) this.m_destCmd.CreateSetOpVar(keyValuePair.Key.Type);
        this.SetMappedVar(keyValuePair.Key, setOpVar);
        leftMap.Add(setOpVar, this.GetMappedVar(keyValuePair.Value));
        rightMap.Add(setOpVar, this.GetMappedVar(op.VarMap[1][keyValuePair.Key]));
      }
      SetOp setOp = (SetOp) null;
      switch (op.OpType)
      {
        case OpType.UnionAll:
          Var var = ((UnionAllOp) op).BranchDiscriminator;
          if (var != null)
            var = this.GetMappedVar(var);
          setOp = (SetOp) this.m_destCmd.CreateUnionAllOp(leftMap, rightMap, var);
          break;
        case OpType.Intersect:
          setOp = (SetOp) this.m_destCmd.CreateIntersectOp(leftMap, rightMap);
          break;
        case OpType.Except:
          setOp = (SetOp) this.m_destCmd.CreateExceptOp(leftMap, rightMap);
          break;
      }
      return this.m_destCmd.CreateNode((Op) setOp, args);
    }

    public override Node Visit(UnionAllOp op, Node n)
    {
      return this.CopySetOp((SetOp) op, n);
    }

    public override Node Visit(IntersectOp op, Node n)
    {
      return this.CopySetOp((SetOp) op, n);
    }

    public override Node Visit(ExceptOp op, Node n)
    {
      return this.CopySetOp((SetOp) op, n);
    }

    public override Node Visit(DistinctOp op, Node n)
    {
      List<Node> args = this.ProcessChildren(n);
      return this.m_destCmd.CreateNode((Op) this.m_destCmd.CreateDistinctOp(this.Copy(op.Keys)), args);
    }

    public override Node Visit(SingleRowOp op, Node n)
    {
      return this.CopyDefault((Op) this.m_destCmd.CreateSingleRowOp(), n);
    }

    public override Node Visit(SingleRowTableOp op, Node n)
    {
      return this.CopyDefault((Op) this.m_destCmd.CreateSingleRowTableOp(), n);
    }

    public override Node Visit(VarDefOp op, Node n)
    {
      List<Node> args = this.ProcessChildren(n);
      Var computedVar = (Var) this.m_destCmd.CreateComputedVar(op.Var.Type);
      this.SetMappedVar(op.Var, computedVar);
      return this.m_destCmd.CreateNode((Op) this.m_destCmd.CreateVarDefOp(computedVar), args);
    }

    public override Node Visit(VarDefListOp op, Node n)
    {
      return this.CopyDefault((Op) this.m_destCmd.CreateVarDefListOp(), n);
    }

    private ColumnMap Copy(ColumnMap columnMap)
    {
      return ColumnMapCopier.Copy(columnMap, this.m_varMap);
    }

    public override Node Visit(PhysicalProjectOp op, Node n)
    {
      List<Node> args = this.ProcessChildren(n);
      return this.m_destCmd.CreateNode((Op) this.m_destCmd.CreatePhysicalProjectOp(this.Copy(op.Outputs), this.Copy((ColumnMap) op.ColumnMap) as SimpleCollectionColumnMap), args);
    }

    private Node VisitNestOp(Node n)
    {
      NestBaseOp op = n.Op as NestBaseOp;
      SingleStreamNestOp singleStreamNestOp = op as SingleStreamNestOp;
      List<Node> args = this.ProcessChildren(n);
      Var discriminatorVar = (Var) null;
      if (singleStreamNestOp != null)
        discriminatorVar = this.GetMappedVar(singleStreamNestOp.Discriminator);
      List<CollectionInfo> collectionInfoList = new List<CollectionInfo>();
      foreach (CollectionInfo collectionInfo1 in op.CollectionInfo)
      {
        ColumnMap columnMap = this.Copy(collectionInfo1.ColumnMap);
        Var computedVar = (Var) this.m_destCmd.CreateComputedVar(collectionInfo1.CollectionVar.Type);
        this.SetMappedVar(collectionInfo1.CollectionVar, computedVar);
        VarList flattenedElementVars = this.Copy(collectionInfo1.FlattenedElementVars);
        VarVec keys = this.Copy(collectionInfo1.Keys);
        List<SortKey> sortKeys = this.Copy(collectionInfo1.SortKeys);
        CollectionInfo collectionInfo2 = Command.CreateCollectionInfo(computedVar, columnMap, flattenedElementVars, keys, sortKeys, collectionInfo1.DiscriminatorValue);
        collectionInfoList.Add(collectionInfo2);
      }
      VarVec outputVars = this.Copy(op.Outputs);
      List<SortKey> prefixSortKeys = this.Copy(op.PrefixSortKeys);
      NestBaseOp nestBaseOp;
      if (singleStreamNestOp != null)
      {
        VarVec keys = this.Copy(singleStreamNestOp.Keys);
        List<SortKey> postfixSortKeys = this.Copy(singleStreamNestOp.PostfixSortKeys);
        nestBaseOp = (NestBaseOp) this.m_destCmd.CreateSingleStreamNestOp(keys, prefixSortKeys, postfixSortKeys, outputVars, collectionInfoList, discriminatorVar);
      }
      else
        nestBaseOp = (NestBaseOp) this.m_destCmd.CreateMultiStreamNestOp(prefixSortKeys, outputVars, collectionInfoList);
      return this.m_destCmd.CreateNode((Op) nestBaseOp, args);
    }

    public override Node Visit(SingleStreamNestOp op, Node n)
    {
      return this.VisitNestOp(n);
    }

    public override Node Visit(MultiStreamNestOp op, Node n)
    {
      return this.VisitNestOp(n);
    }
  }
}
