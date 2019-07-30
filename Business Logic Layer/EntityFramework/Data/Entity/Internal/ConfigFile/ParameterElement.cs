// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.ConfigFile.ParameterElement
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace System.Data.Entity.Internal.ConfigFile
{
  internal class ParameterElement : ConfigurationElement
  {
    private const string ValueKey = "value";
    private const string TypeKey = "type";

    public ParameterElement(int key)
    {
      this.Key = key;
    }

    internal int Key { get; private set; }

    [ConfigurationProperty("value", IsRequired = true)]
    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    public string ValueString
    {
      get
      {
        return (string) this["value"];
      }
      set
      {
        this[nameof (value)] = (object) value;
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    [ConfigurationProperty("type", DefaultValue = "System.String")]
    public string TypeName
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

    public object GetTypedParameterValue()
    {
      return Convert.ChangeType((object) this.ValueString, Type.GetType(this.TypeName, true), (IFormatProvider) CultureInfo.InvariantCulture);
    }
  }
}
