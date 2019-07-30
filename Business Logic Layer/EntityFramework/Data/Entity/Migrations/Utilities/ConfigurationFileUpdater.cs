// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Utilities.ConfigurationFileUpdater
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace System.Data.Entity.Migrations.Utilities
{
  internal class ConfigurationFileUpdater
  {
    private static readonly XNamespace _asm = (XNamespace) "urn:schemas-microsoft-com:asm.v1";
    private static readonly XElement _dependentAssemblyElement;

    [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
    static ConfigurationFileUpdater()
    {
      AssemblyName name = typeof (ConfigurationFileUpdater).Assembly().GetName();
      ConfigurationFileUpdater._dependentAssemblyElement = new XElement(ConfigurationFileUpdater._asm + "dependentAssembly", new object[2]
      {
        (object) new XElement(ConfigurationFileUpdater._asm + "assemblyIdentity", new object[3]
        {
          (object) new XAttribute((XName) "name", (object) "EntityFramework"),
          (object) new XAttribute((XName) "culture", (object) "neutral"),
          (object) new XAttribute((XName) "publicKeyToken", (object) "b77a5c561934e089")
        }),
        (object) new XElement(ConfigurationFileUpdater._asm + "codeBase", new object[2]
        {
          (object) new XAttribute((XName) "version", (object) name.Version.ToString()),
          (object) new XAttribute((XName) "href", (object) name.CodeBase)
        })
      });
    }

    public virtual string Update(string configurationFile)
    {
      bool flag = !string.IsNullOrWhiteSpace(configurationFile) && File.Exists(configurationFile);
      XDocument container = flag ? XDocument.Load(configurationFile) : new XDocument();
      container.GetOrAddElement((XName) "configuration").GetOrAddElement((XName) "runtime").GetOrAddElement(ConfigurationFileUpdater._asm + "assemblyBinding").Add((object) ConfigurationFileUpdater._dependentAssemblyElement);
      string str = Path.GetTempFileName();
      if (flag)
      {
        File.Delete(str);
        str = Path.Combine(Path.GetDirectoryName(configurationFile), Path.GetFileName(str));
      }
      container.Save(str);
      return str;
    }
  }
}
