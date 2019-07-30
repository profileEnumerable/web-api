// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.ClrComplexType
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace System.Data.Entity.Core.Metadata.Edm
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance")]
  internal sealed class ClrComplexType : ComplexType
  {
    private readonly Type _type;
    private Func<object> _constructor;
    private readonly string _cspaceTypeName;

    internal ClrComplexType(Type clrType, string cspaceNamespaceName, string cspaceTypeName)
      : base(Check.NotNull<Type>(clrType, nameof (clrType)).Name, clrType.NestingNamespace() ?? string.Empty, DataSpace.OSpace)
    {
      this._type = clrType;
      this._cspaceTypeName = cspaceNamespaceName + "." + cspaceTypeName;
      this.Abstract = clrType.IsAbstract();
    }

    internal static ClrComplexType CreateReadonlyClrComplexType(
      Type clrType,
      string cspaceNamespaceName,
      string cspaceTypeName)
    {
      ClrComplexType clrComplexType = new ClrComplexType(clrType, cspaceNamespaceName, cspaceTypeName);
      clrComplexType.SetReadOnly();
      return clrComplexType;
    }

    internal Func<object> Constructor
    {
      get
      {
        return this._constructor;
      }
      set
      {
        Interlocked.CompareExchange<Func<object>>(ref this._constructor, value, (Func<object>) null);
      }
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
