// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.Var
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Globalization;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal abstract class Var
  {
    private readonly int _id;
    private readonly VarType _varType;
    private readonly TypeUsage _type;

    internal Var(int id, VarType varType, TypeUsage type)
    {
      this._id = id;
      this._varType = varType;
      this._type = type;
    }

    internal int Id
    {
      get
      {
        return this._id;
      }
    }

    internal VarType VarType
    {
      get
      {
        return this._varType;
      }
    }

    internal TypeUsage Type
    {
      get
      {
        return this._type;
      }
    }

    internal virtual bool TryGetName(out string name)
    {
      name = (string) null;
      return false;
    }

    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) this.Id);
    }
  }
}
