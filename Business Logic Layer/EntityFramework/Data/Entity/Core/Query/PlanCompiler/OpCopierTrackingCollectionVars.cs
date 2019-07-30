// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.OpCopierTrackingCollectionVars
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Query.InternalTrees;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal class OpCopierTrackingCollectionVars : OpCopier
  {
    private readonly Dictionary<Var, System.Data.Entity.Core.Query.InternalTrees.Node> m_newCollectionVarDefinitions = new Dictionary<Var, System.Data.Entity.Core.Query.InternalTrees.Node>();

    private OpCopierTrackingCollectionVars(Command cmd)
      : base(cmd)
    {
    }

    internal static System.Data.Entity.Core.Query.InternalTrees.Node Copy(
      Command cmd,
      System.Data.Entity.Core.Query.InternalTrees.Node n,
      out VarMap varMap,
      out Dictionary<Var, System.Data.Entity.Core.Query.InternalTrees.Node> newCollectionVarDefinitions)
    {
      OpCopierTrackingCollectionVars trackingCollectionVars = new OpCopierTrackingCollectionVars(cmd);
      System.Data.Entity.Core.Query.InternalTrees.Node node = trackingCollectionVars.CopyNode(n);
      varMap = trackingCollectionVars.m_varMap;
      newCollectionVarDefinitions = trackingCollectionVars.m_newCollectionVarDefinitions;
      return node;
    }

    public override System.Data.Entity.Core.Query.InternalTrees.Node Visit(
      MultiStreamNestOp op,
      System.Data.Entity.Core.Query.InternalTrees.Node n)
    {
      System.Data.Entity.Core.Query.InternalTrees.Node node = base.Visit(op, n);
      MultiStreamNestOp op1 = (MultiStreamNestOp) node.Op;
      for (int index = 0; index < op1.CollectionInfo.Count; ++index)
        this.m_newCollectionVarDefinitions.Add(op1.CollectionInfo[index].CollectionVar, node.Children[index + 1]);
      return node;
    }
  }
}
