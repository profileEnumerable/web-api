// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Utilities.MigrationsConfigurationFinder
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Migrations.Infrastructure;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace System.Data.Entity.Migrations.Utilities
{
  internal class MigrationsConfigurationFinder
  {
    private readonly TypeFinder _typeFinder;

    public MigrationsConfigurationFinder()
    {
    }

    public MigrationsConfigurationFinder(TypeFinder typeFinder)
    {
      this._typeFinder = typeFinder;
    }

    public virtual DbMigrationsConfiguration FindMigrationsConfiguration(
      Type contextType,
      string configurationTypeName,
      Func<string, Exception> noType = null,
      Func<string, IEnumerable<Type>, Exception> multipleTypes = null,
      Func<string, string, Exception> noTypeWithName = null,
      Func<string, string, Exception> multipleTypesWithName = null)
    {
      TypeFinder typeFinder = this._typeFinder;
      Type baseType;
      if (!(contextType == (Type) null))
        baseType = typeof (DbMigrationsConfiguration<>).MakeGenericType(contextType);
      else
        baseType = typeof (DbMigrationsConfiguration);
      string typeName = configurationTypeName;
      Func<IEnumerable<Type>, IEnumerable<Type>> filter = (Func<IEnumerable<Type>, IEnumerable<Type>>) (types => (IEnumerable<Type>) types.Where<Type>((Func<Type, bool>) (t =>
      {
        if (t.GetPublicConstructor() != (ConstructorInfo) null && !t.IsAbstract())
          return !t.IsGenericType();
        return false;
      })).ToList<Type>());
      Func<string, Exception> noType1 = noType;
      Func<string, IEnumerable<Type>, Exception> multipleTypes1 = multipleTypes;
      Func<string, string, Exception> noTypeWithName1 = noTypeWithName;
      Func<string, string, Exception> multipleTypesWithName1 = multipleTypesWithName;
      Type type = typeFinder.FindType(baseType, typeName, filter, noType1, multipleTypes1, noTypeWithName1, multipleTypesWithName1);
      try
      {
        return type == (Type) null ? (DbMigrationsConfiguration) null : type.CreateInstance<DbMigrationsConfiguration>(new Func<string, string, string>(Strings.CreateInstance_BadMigrationsConfigurationType), (Func<string, Exception>) (s => (Exception) new MigrationsException(s)));
      }
      catch (TargetInvocationException ex)
      {
        ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
        throw ex.InnerException;
      }
    }
  }
}
