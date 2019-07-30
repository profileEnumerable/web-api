// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.ParameterVar
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal sealed class ParameterVar : Var
  {
    private readonly string m_paramName;

    internal ParameterVar(int id, TypeUsage type, string paramName)
      : base(id, VarType.Parameter, type)
    {
      this.m_paramName = paramName;
    }

    internal string ParameterName
    {
      get
      {
        return this.m_paramName;
      }
    }

    internal override bool TryGetName(out string name)
    {
      name = this.ParameterName;
      return true;
    }
  }
}
