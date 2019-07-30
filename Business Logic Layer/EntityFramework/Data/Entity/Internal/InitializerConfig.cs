// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.InitializerConfig
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Internal.ConfigFile;
using System.Data.Entity.Resources;
using System.Linq;

namespace System.Data.Entity.Internal
{
  internal class InitializerConfig
  {
    private const string ConfigKeyKey = "DatabaseInitializerForType";
    private const string DisabledSpecialValue = "Disabled";
    private readonly EntityFrameworkSection _entityFrameworkSettings;
    private readonly KeyValueConfigurationCollection _appSettings;

    public InitializerConfig()
    {
    }

    public InitializerConfig(
      EntityFrameworkSection entityFrameworkSettings,
      KeyValueConfigurationCollection appSettings)
    {
      this._entityFrameworkSettings = entityFrameworkSettings;
      this._appSettings = appSettings;
    }

    private static object TryGetInitializer(
      Type requiredContextType,
      string contextTypeName,
      string initializerTypeName,
      bool isDisabled,
      Func<object[]> initializerArgs,
      Func<object, object, string> exceptionMessage)
    {
      try
      {
        if (Type.GetType(contextTypeName, true) == requiredContextType)
        {
          if (!isDisabled)
            return Activator.CreateInstance(Type.GetType(initializerTypeName, true), initializerArgs());
          return Activator.CreateInstance(typeof (NullDatabaseInitializer<>).MakeGenericType(requiredContextType));
        }
      }
      catch (Exception ex)
      {
        string str = isDisabled ? "Disabled" : initializerTypeName;
        throw new InvalidOperationException(exceptionMessage((object) str, (object) contextTypeName), ex);
      }
      return (object) null;
    }

    public virtual object TryGetInitializer(Type contextType)
    {
      return this.TryGetInitializerFromEntityFrameworkSection(contextType) ?? this.TryGetInitializerFromLegacyConfig(contextType);
    }

    private object TryGetInitializerFromEntityFrameworkSection(Type contextType)
    {
      return this._entityFrameworkSettings.Contexts.OfType<ContextElement>().Where<ContextElement>((Func<ContextElement, bool>) (e =>
      {
        if (!e.IsDatabaseInitializationDisabled)
          return !string.IsNullOrWhiteSpace(e.DatabaseInitializer.InitializerTypeName);
        return true;
      })).Select<ContextElement, object>((Func<ContextElement, object>) (e => InitializerConfig.TryGetInitializer(contextType, e.ContextTypeName, e.DatabaseInitializer.InitializerTypeName ?? string.Empty, e.IsDatabaseInitializationDisabled, (Func<object[]>) (() => e.DatabaseInitializer.Parameters.GetTypedParameterValues()), new Func<object, object, string>(Strings.Database_InitializeFromConfigFailed)))).FirstOrDefault<object>((Func<object, bool>) (i => i != null));
    }

    private object TryGetInitializerFromLegacyConfig(Type contextType)
    {
      foreach (string index in ((IEnumerable<string>) this._appSettings.AllKeys).Where<string>((Func<string, bool>) (k => k.StartsWith("DatabaseInitializerForType", StringComparison.OrdinalIgnoreCase))))
      {
        string contextTypeName = index.Remove(0, "DatabaseInitializerForType".Length).Trim();
        string initializerTypeName = (this._appSettings[index].Value ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(contextTypeName))
          throw new InvalidOperationException(Strings.Database_BadLegacyInitializerEntry((object) index, (object) initializerTypeName));
        object initializer = InitializerConfig.TryGetInitializer(contextType, contextTypeName, initializerTypeName, initializerTypeName.Length == 0 || initializerTypeName.Equals("Disabled", StringComparison.OrdinalIgnoreCase), (Func<object[]>) (() => new object[0]), new Func<object, object, string>(Strings.Database_InitializeFromLegacyConfigFailed));
        if (initializer != null)
          return initializer;
      }
      return (object) null;
    }
  }
}
