// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.ProviderCommandInfoUtils
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Query.InternalTrees;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal static class ProviderCommandInfoUtils
  {
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "rowtype")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    internal static ProviderCommandInfo Create(Command command, System.Data.Entity.Core.Query.InternalTrees.Node node)
    {
      PhysicalProjectOp op = node.Op as PhysicalProjectOp;
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(op != null, "Expected root Op to be a physical Project");
      DbCommandTree commandTree = CTreeGenerator.Generate(command, node);
      DbQueryCommandTree queryCommandTree = commandTree as DbQueryCommandTree;
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(queryCommandTree != null, "null query command tree");
      CollectionType edmType = TypeHelpers.GetEdmType<CollectionType>(queryCommandTree.Query.ResultType);
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(TypeSemantics.IsRowType(edmType.TypeUsage), "command rowtype is not a record");
      ProviderCommandInfoUtils.BuildOutputVarMap(op, edmType.TypeUsage);
      return new ProviderCommandInfo(commandTree);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "RowType")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PhysicalProjectOp")]
    private static Dictionary<Var, EdmProperty> BuildOutputVarMap(
      PhysicalProjectOp projectOp,
      TypeUsage outputType)
    {
      Dictionary<Var, EdmProperty> dictionary = new Dictionary<Var, EdmProperty>();
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(TypeSemantics.IsRowType(outputType), "PhysicalProjectOp result type is not a RowType?");
      IEnumerator<EdmProperty> enumerator1 = (IEnumerator<EdmProperty>) TypeHelpers.GetEdmType<RowType>(outputType).Properties.GetEnumerator();
      IEnumerator<Var> enumerator2 = (IEnumerator<Var>) projectOp.Outputs.GetEnumerator();
      while (true)
      {
        bool flag1 = enumerator1.MoveNext();
        bool flag2 = enumerator2.MoveNext();
        if (flag1 == flag2)
        {
          if (flag1)
            dictionary[enumerator2.Current] = enumerator1.Current;
          else
            goto label_5;
        }
        else
          break;
      }
      throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.ColumnCountMismatch, 1, (object) null);
label_5:
      return dictionary;
    }
  }
}
