// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.ConfigFile.QueryCacheElement
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Internal.ConfigFile
{
  [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
  internal class QueryCacheElement : ConfigurationElement
  {
    private const string SizeKey = "size";
    private const string CleaningIntervalInSecondsKey = "cleaningIntervalInSeconds";

    [ConfigurationProperty("size")]
    [IntegerValidator(MaxValue = 2147483647, MinValue = 0)]
    public int Size
    {
      get
      {
        return (int) this["size"];
      }
      set
      {
        this["size"] = (object) value;
      }
    }

    [IntegerValidator(MaxValue = 2147483647, MinValue = 0)]
    [ConfigurationProperty("cleaningIntervalInSeconds")]
    public int CleaningIntervalInSeconds
    {
      get
      {
        return (int) this["cleaningIntervalInSeconds"];
      }
      set
      {
        this["cleaningIntervalInSeconds"] = (object) value;
      }
    }
  }
}
