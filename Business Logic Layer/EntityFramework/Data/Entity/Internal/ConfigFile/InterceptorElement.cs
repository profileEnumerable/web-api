// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.ConfigFile.InterceptorElement
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Configuration;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.Resources;

namespace System.Data.Entity.Internal.ConfigFile
{
  internal class InterceptorElement : ConfigurationElement
  {
    private const string TypeKey = "type";
    private const string ParametersKey = "parameters";

    public InterceptorElement(int key)
    {
      this.Key = key;
    }

    internal int Key { get; private set; }

    [ConfigurationProperty("type", IsRequired = true)]
    public virtual string TypeName
    {
      get
      {
        return (string) this["type"];
      }
      set
      {
        this["type"] = (object) value;
      }
    }

    [ConfigurationProperty("parameters")]
    public virtual ParameterCollection Parameters
    {
      get
      {
        return (ParameterCollection) this["parameters"];
      }
    }

    public virtual IDbInterceptor CreateInterceptor()
    {
      object instance;
      try
      {
        instance = Activator.CreateInstance(Type.GetType(this.TypeName, true), this.Parameters.GetTypedParameterValues());
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException(Strings.InterceptorTypeNotFound((object) this.TypeName), ex);
      }
      IDbInterceptor dbInterceptor = instance as IDbInterceptor;
      if (dbInterceptor == null)
        throw new InvalidOperationException(Strings.InterceptorTypeNotInterceptor((object) this.TypeName));
      return dbInterceptor;
    }
  }
}
