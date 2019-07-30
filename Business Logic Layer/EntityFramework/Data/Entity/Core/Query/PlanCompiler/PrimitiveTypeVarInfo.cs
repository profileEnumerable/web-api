// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.PrimitiveTypeVarInfo
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Query.InternalTrees;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal class PrimitiveTypeVarInfo : VarInfo
  {
    private readonly List<Var> m_newVars;

    internal PrimitiveTypeVarInfo(Var newVar)
    {
      this.m_newVars = new List<Var>() { newVar };
    }

    internal Var NewVar
    {
      get
      {
        return this.m_newVars[0];
      }
    }

    internal override VarInfoKind Kind
    {
      get
      {
        return VarInfoKind.PrimitiveTypeVarInfo;
      }
    }

    internal override List<Var> NewVars
    {
      get
      {
        return this.m_newVars;
      }
    }
  }
}
