// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.ConfigFile.DefaultConnectionFactoryElement
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Internal.ConfigFile
{
  [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
  internal class DefaultConnectionFactoryElement : ConfigurationElement
  {
    private const string TypeKey = "type";
    private const string ParametersKey = "parameters";

    [ConfigurationProperty("type", IsRequired = true)]
    public string FactoryTypeName
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
    public ParameterCollection Parameters
    {
      get
      {
        return (ParameterCollection) this["parameters"];
      }
    }

    public Type GetFactoryType()
    {
      return Type.GetType(this.FactoryTypeName, true);
    }
  }
}
