// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.ConfigFile.ProviderCollection
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Configuration;
using System.Data.Entity.Resources;

namespace System.Data.Entity.Internal.ConfigFile
{
  internal class ProviderCollection : ConfigurationElementCollection
  {
    private const string ProviderKey = "provider";

    protected override ConfigurationElement CreateNewElement()
    {
      return (ConfigurationElement) new ProviderElement();
    }

    protected override object GetElementKey(ConfigurationElement element)
    {
      return (object) ((ProviderElement) element).InvariantName;
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
        return "provider";
      }
    }

    protected override void BaseAdd(ConfigurationElement element)
    {
      if (this.ValidateProviderElement(element))
        return;
      base.BaseAdd(element);
    }

    protected override void BaseAdd(int index, ConfigurationElement element)
    {
      if (this.ValidateProviderElement(element))
        return;
      base.BaseAdd(index, element);
    }

    private bool ValidateProviderElement(ConfigurationElement element)
    {
      object elementKey = this.GetElementKey(element);
      ProviderElement providerElement = (ProviderElement) this.BaseGet(elementKey);
      if (providerElement != null && providerElement.ProviderTypeName != ((ProviderElement) element).ProviderTypeName)
        throw new InvalidOperationException(Strings.ProviderInvariantRepeatedInConfig(elementKey));
      return providerElement != null;
    }

    public ProviderElement AddProvider(string invariantName, string providerTypeName)
    {
      ProviderElement newElement = (ProviderElement) this.CreateNewElement();
      base.BaseAdd((ConfigurationElement) newElement);
      newElement.InvariantName = invariantName;
      newElement.ProviderTypeName = providerTypeName;
      return newElement;
    }
  }
}
