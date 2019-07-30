// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DependencyResolution.DbConfigurationFinder
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.Infrastructure.DependencyResolution
{
  internal class DbConfigurationFinder
  {
    public virtual Type TryFindConfigurationType(
      Type contextType,
      IEnumerable<Type> typesToSearch = null)
    {
      return this.TryFindConfigurationType(contextType.Assembly(), contextType, typesToSearch);
    }

    public virtual Type TryFindConfigurationType(
      Assembly assemblyHint,
      Type contextTypeHint,
      IEnumerable<Type> typesToSearch = null)
    {
      if (contextTypeHint != (Type) null)
      {
        Type c = contextTypeHint.GetCustomAttributes<DbConfigurationTypeAttribute>(true).Select<DbConfigurationTypeAttribute, Type>((Func<DbConfigurationTypeAttribute, Type>) (a => a.ConfigurationType)).FirstOrDefault<Type>();
        if (c != (Type) null)
        {
          if (!typeof (DbConfiguration).IsAssignableFrom(c))
            throw new InvalidOperationException(Strings.CreateInstance_BadDbConfigurationType((object) c.ToString(), (object) typeof (DbConfiguration).ToString()));
          return c;
        }
      }
      List<Type> list = (typesToSearch ?? assemblyHint.GetAccessibleTypes()).Where<Type>((Func<Type, bool>) (t =>
      {
        if (t.IsSubclassOf(typeof (DbConfiguration)) && !t.IsAbstract())
          return !t.IsGenericType();
        return false;
      })).ToList<Type>();
      if (list.Count > 1)
        throw new InvalidOperationException(Strings.MultipleConfigsInAssembly((object) list.First<Type>().Assembly(), (object) typeof (DbConfiguration).Name));
      return list.FirstOrDefault<Type>();
    }

    public virtual Type TryFindContextType(
      Assembly assemblyHint,
      Type contextTypeHint,
      IEnumerable<Type> typesToSearch = null)
    {
      if (contextTypeHint != (Type) null)
        return contextTypeHint;
      List<Type> list = (typesToSearch ?? assemblyHint.GetAccessibleTypes()).Where<Type>((Func<Type, bool>) (t =>
      {
        if (t.IsSubclassOf(typeof (DbContext)) && !t.IsAbstract() && !t.IsGenericType())
          return t.GetCustomAttributes<DbConfigurationTypeAttribute>(true).Any<DbConfigurationTypeAttribute>();
        return false;
      })).ToList<Type>();
      if (list.Count != 1)
        return (Type) null;
      return list[0];
    }
  }
}
