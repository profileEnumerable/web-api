// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.ObjectContextTypeCache
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Concurrent;
using System.Data.Entity.Core.Objects;

namespace System.Data.Entity.Internal
{
  internal static class ObjectContextTypeCache
  {
    private static readonly ConcurrentDictionary<Type, Type> _typeCache = new ConcurrentDictionary<Type, Type>();

    public static Type GetObjectType(Type type)
    {
      return ObjectContextTypeCache._typeCache.GetOrAdd(type, new Func<Type, Type>(ObjectContext.GetObjectType));
    }
  }
}
