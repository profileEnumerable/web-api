// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.VarInfoMap
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Query.InternalTrees;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal class VarInfoMap
  {
    private readonly Dictionary<Var, VarInfo> m_map;

    internal VarInfoMap()
    {
      this.m_map = new Dictionary<Var, VarInfo>();
    }

    internal VarInfo CreateStructuredVarInfo(
      Var v,
      RowType newType,
      List<Var> newVars,
      List<EdmProperty> newProperties,
      bool newVarsIncludeNullSentinelVar)
    {
      VarInfo varInfo = (VarInfo) new StructuredVarInfo(newType, newVars, newProperties, newVarsIncludeNullSentinelVar);
      this.m_map.Add(v, varInfo);
      return varInfo;
    }

    internal VarInfo CreateStructuredVarInfo(
      Var v,
      RowType newType,
      List<Var> newVars,
      List<EdmProperty> newProperties)
    {
      return this.CreateStructuredVarInfo(v, newType, newVars, newProperties, false);
    }

    internal VarInfo CreateCollectionVarInfo(Var v, Var newVar)
    {
      VarInfo varInfo = (VarInfo) new CollectionVarInfo(newVar);
      this.m_map.Add(v, varInfo);
      return varInfo;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    internal VarInfo CreatePrimitiveTypeVarInfo(Var v, Var newVar)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(TypeSemantics.IsScalarType(v.Type), "The current variable should be of primitive or enum type.");
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(TypeSemantics.IsScalarType(newVar.Type), "The new variable should be of primitive or enum type.");
      VarInfo varInfo = (VarInfo) new PrimitiveTypeVarInfo(newVar);
      this.m_map.Add(v, varInfo);
      return varInfo;
    }

    internal bool TryGetVarInfo(Var v, out VarInfo varInfo)
    {
      return this.m_map.TryGetValue(v, out varInfo);
    }
  }
}
