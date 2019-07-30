// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.ConfigFile.ProviderElement
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Configuration;

namespace System.Data.Entity.Internal.ConfigFile
{
  internal class ProviderElement : ConfigurationElement
  {
    private const string InvariantNameKey = "invariantName";
    private const string TypeKey = "type";

    [ConfigurationProperty("invariantName", IsRequired = true)]
    public string InvariantName
    {
      get
      {
        return (string) this["invariantName"];
      }
      set
      {
        this["invariantName"] = (object) value;
      }
    }

    [ConfigurationProperty("type", IsRequired = true)]
    public string ProviderTypeName
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
  }
}
