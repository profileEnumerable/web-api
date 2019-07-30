// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.ClrEnumType
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Metadata.Edm
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance")]
  internal sealed class ClrEnumType : EnumType
  {
    private readonly Type _type;
    private readonly string _cspaceTypeName;

    internal ClrEnumType(Type clrType, string cspaceNamespaceName, string cspaceTypeName)
      : base(clrType)
    {
      this._type = clrType;
      this._cspaceTypeName = cspaceNamespaceName + "." + cspaceTypeName;
    }

    internal override Type ClrType
    {
      get
      {
        return this._type;
      }
    }

    internal string CSpaceTypeName
    {
      get
      {
        return this._cspaceTypeName;
      }
    }
  }
}
