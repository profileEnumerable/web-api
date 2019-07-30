// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.CodeGen
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Query.InternalTrees;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal class CodeGen
  {
    private readonly System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler m_compilerState;
    private List<System.Data.Entity.Core.Query.InternalTrees.Node> m_subCommands;

    internal static void Process(
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler compilerState,
      out List<ProviderCommandInfo> childCommands,
      out ColumnMap resultColumnMap,
      out int columnCount)
    {
      new CodeGen(compilerState).Process(out childCommands, out resultColumnMap, out columnCount);
    }

    private CodeGen(System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler compilerState)
    {
      this.m_compilerState = compilerState;
    }

    private void Process(
      out List<ProviderCommandInfo> childCommands,
      out ColumnMap resultColumnMap,
      out int columnCount)
    {
      PhysicalProjectOp op = (PhysicalProjectOp) this.Command.Root.Op;
      this.m_subCommands = new List<System.Data.Entity.Core.Query.InternalTrees.Node>((IEnumerable<System.Data.Entity.Core.Query.InternalTrees.Node>) new System.Data.Entity.Core.Query.InternalTrees.Node[1]
      {
        this.Command.Root
      });
      childCommands = new List<ProviderCommandInfo>((IEnumerable<ProviderCommandInfo>) new ProviderCommandInfo[1]
      {
        ProviderCommandInfoUtils.Create(this.Command, this.Command.Root)
      });
      resultColumnMap = this.BuildResultColumnMap(op);
      columnCount = op.Outputs.Count;
    }

    private ColumnMap BuildResultColumnMap(PhysicalProjectOp projectOp)
    {
      Dictionary<Var, KeyValuePair<int, int>> varToCommandColumnMap = this.BuildVarMap();
      return ColumnMapTranslator.Translate((ColumnMap) projectOp.ColumnMap, varToCommandColumnMap);
    }

    private Dictionary<Var, KeyValuePair<int, int>> BuildVarMap()
    {
      Dictionary<Var, KeyValuePair<int, int>> dictionary = new Dictionary<Var, KeyValuePair<int, int>>();
      int key = 0;
      foreach (System.Data.Entity.Core.Query.InternalTrees.Node subCommand in this.m_subCommands)
      {
        PhysicalProjectOp op = (PhysicalProjectOp) subCommand.Op;
        int num = 0;
        foreach (Var output in (List<Var>) op.Outputs)
        {
          KeyValuePair<int, int> keyValuePair = new KeyValuePair<int, int>(key, num);
          dictionary[output] = keyValuePair;
          ++num;
        }
        ++key;
      }
      return dictionary;
    }

    private Command Command
    {
      get
      {
        return this.m_compilerState.Command;
      }
    }
  }
}
