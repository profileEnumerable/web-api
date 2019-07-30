// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Utilities.ProviderRowFinder
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace System.Data.Entity.Utilities
{
  internal class ProviderRowFinder
  {
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    public virtual DataRow FindRow(
      Type hintType,
      Func<DataRow, bool> selector,
      IEnumerable<DataRow> dataRows)
    {
      AssemblyName assemblyName = hintType == (Type) null ? (AssemblyName) null : new AssemblyName(hintType.Assembly().FullName);
      foreach (DataRow dataRow in dataRows)
      {
        string typeName = (string) dataRow[3];
        AssemblyName rowProviderFactoryAssemblyName = (AssemblyName) null;
        Type.GetType(typeName, (Func<AssemblyName, Assembly>) (a =>
        {
          rowProviderFactoryAssemblyName = a;
          return (Assembly) null;
        }), (Func<Assembly, string, bool, Type>) ((_, __, ___) => (Type) null));
        if (rowProviderFactoryAssemblyName != null)
        {
          if (!(hintType == (Type) null))
          {
            if (!string.Equals(assemblyName.Name, rowProviderFactoryAssemblyName.Name, StringComparison.OrdinalIgnoreCase))
              continue;
          }
          try
          {
            if (selector(dataRow))
              return dataRow;
          }
          catch (Exception ex)
          {
          }
        }
      }
      return (DataRow) null;
    }
  }
}
