// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.ConfigurationTypesFinder
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Configuration.Types;

namespace System.Data.Entity.ModelConfiguration.Configuration
{
  internal class ConfigurationTypesFinder
  {
    private readonly ConfigurationTypeActivator _activator;
    private readonly ConfigurationTypeFilter _filter;

    public ConfigurationTypesFinder()
      : this(new ConfigurationTypeActivator(), new ConfigurationTypeFilter())
    {
    }

    public ConfigurationTypesFinder(
      ConfigurationTypeActivator activator,
      ConfigurationTypeFilter filter)
    {
      this._activator = activator;
      this._filter = filter;
    }

    public virtual void AddConfigurationTypesToModel(
      IEnumerable<Type> types,
      System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration modelConfiguration)
    {
      foreach (Type type in types)
      {
        if (this._filter.IsEntityTypeConfiguration(type))
          modelConfiguration.Add(this._activator.Activate<EntityTypeConfiguration>(type));
        else if (this._filter.IsComplexTypeConfiguration(type))
          modelConfiguration.Add(this._activator.Activate<ComplexTypeConfiguration>(type));
      }
    }
  }
}
