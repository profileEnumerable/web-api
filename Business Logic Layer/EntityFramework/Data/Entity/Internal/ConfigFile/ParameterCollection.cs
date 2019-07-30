// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.ConfigFile.ParameterCollection
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.Internal.ConfigFile
{
  [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
  internal class ParameterCollection : ConfigurationElementCollection
  {
    private const string ParameterKey = "parameter";
    private int _nextKey;

    protected override ConfigurationElement CreateNewElement()
    {
      ParameterElement parameterElement = new ParameterElement(this._nextKey);
      ++this._nextKey;
      return (ConfigurationElement) parameterElement;
    }

    protected override object GetElementKey(ConfigurationElement element)
    {
      return (object) ((ParameterElement) element).Key;
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
        return "parameter";
      }
    }

    public virtual object[] GetTypedParameterValues()
    {
      return this.Cast<ParameterElement>().Select<ParameterElement, object>((Func<ParameterElement, object>) (e => e.GetTypedParameterValue())).ToArray<object>();
    }

    internal ParameterElement NewElement()
    {
      ConfigurationElement newElement = this.CreateNewElement();
      this.BaseAdd(newElement);
      return (ParameterElement) newElement;
    }
  }
}
