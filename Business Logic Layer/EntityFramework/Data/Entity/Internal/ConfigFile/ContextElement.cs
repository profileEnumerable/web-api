// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.ConfigFile.ContextElement
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Internal.ConfigFile
{
  internal class ContextElement : ConfigurationElement
  {
    private const string TypeKey = "type";
    private const string DisableDatabaseInitializationKey = "disableDatabaseInitialization";
    private const string DatabaseInitializerKey = "databaseInitializer";

    [ConfigurationProperty("type", IsRequired = true)]
    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    public virtual string ContextTypeName
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

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    [ConfigurationProperty("disableDatabaseInitialization", DefaultValue = false)]
    public virtual bool IsDatabaseInitializationDisabled
    {
      get
      {
        return (bool) this["disableDatabaseInitialization"];
      }
      set
      {
        this["disableDatabaseInitialization"] = (object) value;
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    [ConfigurationProperty("databaseInitializer")]
    public virtual DatabaseInitializerElement DatabaseInitializer
    {
      get
      {
        return (DatabaseInitializerElement) this["databaseInitializer"];
      }
      set
      {
        this["databaseInitializer"] = (object) value;
      }
    }
  }
}
