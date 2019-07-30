// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Utilities.AssemblyExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.Utilities
{
  internal static class AssemblyExtensions
  {
    public static string GetInformationalVersion(this Assembly assembly)
    {
      return assembly.GetCustomAttributes<AssemblyInformationalVersionAttribute>().Single<AssemblyInformationalVersionAttribute>().InformationalVersion;
    }

    public static IEnumerable<Type> GetAccessibleTypes(this Assembly assembly)
    {
      try
      {
        return assembly.DefinedTypes.Select<TypeInfo, Type>((Func<TypeInfo, Type>) (t => t.AsType()));
      }
      catch (ReflectionTypeLoadException ex)
      {
        return ((IEnumerable<Type>) ex.Types).Where<Type>((Func<Type, bool>) (t => t != (Type) null));
      }
    }
  }
}
