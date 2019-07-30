// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Utilities.TypeFinder
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.Utilities
{
  internal class TypeFinder
  {
    private readonly Assembly _assembly;

    public TypeFinder(Assembly assembly)
    {
      this._assembly = assembly;
    }

    public Type FindType(
      Type baseType,
      string typeName,
      Func<IEnumerable<Type>, IEnumerable<Type>> filter,
      Func<string, Exception> noType = null,
      Func<string, IEnumerable<Type>, Exception> multipleTypes = null,
      Func<string, string, Exception> noTypeWithName = null,
      Func<string, string, Exception> multipleTypesWithName = null)
    {
      bool flag = !string.IsNullOrWhiteSpace(typeName);
      Type type = (Type) null;
      if (flag)
        type = this._assembly.GetType(typeName);
      if (type == (Type) null)
      {
        string name = this._assembly.GetName().Name;
        IEnumerable<Type> source1 = this._assembly.GetAccessibleTypes().Where<Type>((Func<Type, bool>) (t => baseType.IsAssignableFrom(t)));
        IEnumerable<Type> source2;
        if (flag)
        {
          source2 = (IEnumerable<Type>) source1.Where<Type>((Func<Type, bool>) (t => string.Equals(t.Name, typeName, StringComparison.OrdinalIgnoreCase))).ToList<Type>();
          if (source2.Count<Type>() > 1)
            source2 = (IEnumerable<Type>) source2.Where<Type>((Func<Type, bool>) (t => string.Equals(t.Name, typeName, StringComparison.Ordinal))).ToList<Type>();
          if (!source2.Any<Type>())
          {
            if (noTypeWithName != null)
              throw noTypeWithName(typeName, name);
            return (Type) null;
          }
          if (source2.Count<Type>() > 1)
          {
            if (multipleTypesWithName != null)
              throw multipleTypesWithName(typeName, name);
            return (Type) null;
          }
        }
        else
        {
          source2 = filter(source1);
          if (!source2.Any<Type>())
          {
            if (noType != null)
              throw noType(name);
            return (Type) null;
          }
          if (source2.Count<Type>() > 1)
          {
            if (multipleTypes != null)
              throw multipleTypes(name, source2);
            return (Type) null;
          }
        }
        type = source2.Single<Type>();
      }
      return type;
    }
  }
}
