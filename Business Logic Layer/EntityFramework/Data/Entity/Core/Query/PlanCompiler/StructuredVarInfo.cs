// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.StructuredVarInfo
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Query.InternalTrees;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal class StructuredVarInfo : VarInfo
  {
    private Dictionary<EdmProperty, Var> m_propertyToVarMap;
    private readonly List<Var> m_newVars;
    private readonly bool m_newVarsIncludeNullSentinelVar;
    private readonly List<EdmProperty> m_newProperties;
    private readonly RowType m_newType;
    private readonly TypeUsage m_newTypeUsage;

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    internal StructuredVarInfo(
      RowType newType,
      List<Var> newVars,
      List<EdmProperty> newTypeProperties,
      bool newVarsIncludeNullSentinelVar)
    {
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(newVars.Count == newTypeProperties.Count, "count mismatch");
      this.m_newVars = newVars;
      this.m_newProperties = newTypeProperties;
      this.m_newType = newType;
      this.m_newVarsIncludeNullSentinelVar = newVarsIncludeNullSentinelVar;
      this.m_newTypeUsage = TypeUsage.Create((EdmType) newType);
    }

    internal override VarInfoKind Kind
    {
      get
      {
        return VarInfoKind.StructuredTypeVarInfo;
      }
    }

    internal override List<Var> NewVars
    {
      get
      {
        return this.m_newVars;
      }
    }

    internal List<EdmProperty> Fields
    {
      get
      {
        return this.m_newProperties;
      }
    }

    internal bool NewVarsIncludeNullSentinelVar
    {
      get
      {
        return this.m_newVarsIncludeNullSentinelVar;
      }
    }

    internal bool TryGetVar(EdmProperty p, out Var v)
    {
      if (this.m_propertyToVarMap == null)
        this.InitPropertyToVarMap();
      return this.m_propertyToVarMap.TryGetValue(p, out v);
    }

    internal RowType NewType
    {
      get
      {
        return this.m_newType;
      }
    }

    internal TypeUsage NewTypeUsage
    {
      get
      {
        return this.m_newTypeUsage;
      }
    }

    private void InitPropertyToVarMap()
    {
      if (this.m_propertyToVarMap != null)
        return;
      this.m_propertyToVarMap = new Dictionary<EdmProperty, Var>();
      IEnumerator<Var> enumerator = (IEnumerator<Var>) this.m_newVars.GetEnumerator();
      foreach (EdmProperty newProperty in this.m_newProperties)
      {
        enumerator.MoveNext();
        this.m_propertyToVarMap.Add(newProperty, enumerator.Current);
      }
      enumerator.Dispose();
    }
  }
}
