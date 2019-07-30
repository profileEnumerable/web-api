// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DependencyResolution.DefaultExecutionStrategyResolver
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Infrastructure.DependencyResolution
{
  internal class DefaultExecutionStrategyResolver : IDbDependencyResolver
  {
    public object GetService(Type type, object key)
    {
      if (!(type == typeof (Func<IDbExecutionStrategy>)))
        return (object) null;
      Check.NotNull<object>(key, nameof (key));
      if (!(key is ExecutionStrategyKey))
        throw new ArgumentException(Strings.DbDependencyResolver_InvalidKey((object) typeof (ExecutionStrategyKey).Name, (object) "Func<IExecutionStrategy>"));
      return (object) (Func<IDbExecutionStrategy>) (() => (IDbExecutionStrategy) new DefaultExecutionStrategy());
    }

    public IEnumerable<object> GetServices(Type type, object key)
    {
      return this.GetServiceAsServices(type, key);
    }
  }
}
