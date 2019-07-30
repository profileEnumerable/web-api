// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DependencyResolution.DbConfigurationLoader
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Internal;
using System.Data.Entity.Resources;

namespace System.Data.Entity.Infrastructure.DependencyResolution
{
  internal class DbConfigurationLoader
  {
    public virtual Type TryLoadFromConfig(AppConfig config)
    {
      string configurationTypeName = config.ConfigurationTypeName;
      if (string.IsNullOrWhiteSpace(configurationTypeName))
        return (Type) null;
      Type type;
      try
      {
        type = Type.GetType(configurationTypeName, true);
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException(Strings.DbConfigurationTypeNotFound((object) configurationTypeName), ex);
      }
      if (!typeof (DbConfiguration).IsAssignableFrom(type))
        throw new InvalidOperationException(Strings.CreateInstance_BadDbConfigurationType((object) type.ToString(), (object) typeof (DbConfiguration).ToString()));
      return type;
    }

    public virtual bool AppConfigContainsDbConfigurationType(AppConfig config)
    {
      return !string.IsNullOrWhiteSpace(config.ConfigurationTypeName);
    }
  }
}
