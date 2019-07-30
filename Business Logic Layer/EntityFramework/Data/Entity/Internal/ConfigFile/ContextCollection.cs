// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.ConfigFile.ContextCollection
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Configuration;
using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Internal.ConfigFile
{
  [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
  internal class ContextCollection : ConfigurationElementCollection
  {
    private const string ContextKey = "context";

    protected override ConfigurationElement CreateNewElement()
    {
      return (ConfigurationElement) new ContextElement();
    }

    protected override object GetElementKey(ConfigurationElement element)
    {
      return (object) ((ContextElement) element).ContextTypeName;
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
        return "context";
      }
    }

    protected override void BaseAdd(ConfigurationElement element)
    {
      object elementKey = this.GetElementKey(element);
      if (this.BaseGet(elementKey) != null)
        throw Error.ContextConfiguredMultipleTimes(elementKey);
      base.BaseAdd(element);
    }

    protected override void BaseAdd(int index, ConfigurationElement element)
    {
      object elementKey = this.GetElementKey(element);
      if (this.BaseGet(elementKey) != null)
        throw Error.ContextConfiguredMultipleTimes(elementKey);
      base.BaseAdd(index, element);
    }
  }
}
