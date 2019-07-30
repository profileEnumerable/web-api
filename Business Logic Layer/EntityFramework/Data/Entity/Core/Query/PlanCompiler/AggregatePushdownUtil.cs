// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.AggregatePushdownUtil
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Query.InternalTrees;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal static class AggregatePushdownUtil
  {
    internal static bool IsVarRefOverGivenVar(Node node, Var var)
    {
      if (node.Op.OpType != OpType.VarRef)
        return false;
      return ((VarRefOp) node.Op).Var == var;
    }
  }
}
