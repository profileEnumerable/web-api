// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.ConfigFile.InterceptorsCollection
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;

namespace System.Data.Entity.Internal.ConfigFile
{
  internal class InterceptorsCollection : ConfigurationElementCollection
  {
    private const string ElementKey = "interceptor";
    private int _nextKey;

    protected override ConfigurationElement CreateNewElement()
    {
      return (ConfigurationElement) new InterceptorElement(this._nextKey++);
    }

    protected override object GetElementKey(ConfigurationElement element)
    {
      return (object) ((InterceptorElement) element).Key;
    }

    public override ConfigurationElementCollectionType CollectionType
    {
      get
      {
        return ConfigurationElementCollectionType.BasicMap;
      }
    }

    protected override string ElementName
    {
      get
      {
        return "interceptor";
      }
    }

    public void AddElement(InterceptorElement element)
    {
      this.BaseAdd((ConfigurationElement) element);
    }

    public virtual IEnumerable<IDbInterceptor> Interceptors
    {
      get
      {
        return (IEnumerable<IDbInterceptor>) this.OfType<InterceptorElement>().Select<InterceptorElement, IDbInterceptor>((Func<InterceptorElement, IDbInterceptor>) (e => e.CreateInterceptor())).ToList<IDbInterceptor>();
      }
    }
  }
}
