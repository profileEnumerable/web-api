// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.ConfigFile.EntityFrameworkSection
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Internal.ConfigFile
{
  internal class EntityFrameworkSection : ConfigurationSection
  {
    private const string DefaultConnectionFactoryKey = "defaultConnectionFactory";
    private const string ContextsKey = "contexts";
    private const string ProviderKey = "providers";
    private const string ConfigurationTypeKey = "codeConfigurationType";
    private const string InterceptorsKey = "interceptors";
    private const string QueryCacheKey = "queryCache";

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    [ConfigurationProperty("defaultConnectionFactory")]
    public virtual DefaultConnectionFactoryElement DefaultConnectionFactory
    {
      get
      {
        return (DefaultConnectionFactoryElement) this["defaultConnectionFactory"];
      }
      set
      {
        this["defaultConnectionFactory"] = (object) value;
      }
    }

    [ConfigurationProperty("codeConfigurationType")]
    public virtual string ConfigurationTypeName
    {
      get
      {
        return (string) this["codeConfigurationType"];
      }
      set
      {
        this["codeConfigurationType"] = (object) value;
      }
    }

    [ConfigurationProperty("providers")]
    public virtual ProviderCollection Providers
    {
      get
      {
        return (ProviderCollection) this["providers"];
      }
    }

    [ConfigurationProperty("contexts")]
    public virtual ContextCollection Contexts
    {
      get
      {
        return (ContextCollection) this["contexts"];
      }
    }

    [ConfigurationProperty("interceptors")]
    public virtual InterceptorsCollection Interceptors
    {
      get
      {
        return (InterceptorsCollection) this["interceptors"];
      }
    }

    [ConfigurationProperty("queryCache")]
    public virtual QueryCacheElement QueryCache
    {
      get
      {
        return (QueryCacheElement) this["queryCache"];
      }
      set
      {
        this["queryCache"] = (object) value;
      }
    }
  }
}
