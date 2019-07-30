// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.EntityClient.EntityProviderFactory
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.EntityClient.Internal;
using System.Diagnostics.CodeAnalysis;
using System.Security;
using System.Security.Permissions;

namespace System.Data.Entity.Core.EntityClient
{
  /// <summary>
  /// Class representing a provider factory for the entity client provider
  /// </summary>
  [SuppressMessage("Microsoft.Usage", "CA2302", Justification = "We don't expect serviceType to be an Embedded Interop Types.")]
  public sealed class EntityProviderFactory : DbProviderFactory, IServiceProvider
  {
    /// <summary>
    /// A singleton object for the entity client provider factory object.
    /// This remains a public field (not property) because DbProviderFactory expects a field.
    /// </summary>
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EntityProviderFactory implements the singleton pattern and it's stateless.  This is needed in order to work with DbProviderFactories.")]
    public static readonly EntityProviderFactory Instance = new EntityProviderFactory();

    private EntityProviderFactory()
    {
    }

    /// <summary>
    /// Returns a new instance of the provider's class that implements the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityCommand" />
    /// class.
    /// </summary>
    /// <returns>
    /// A new instance of <see cref="T:System.Data.Entity.Core.EntityClient.EntityCommand" />.
    /// </returns>
    public override DbCommand CreateCommand()
    {
      return (DbCommand) new EntityCommand();
    }

    /// <summary>
    /// Throws a <see cref="T:System.NotSupportedException" />. This method is currently not supported.
    /// </summary>
    /// <returns>This method is currently not supported.</returns>
    public override DbCommandBuilder CreateCommandBuilder()
    {
      throw new NotSupportedException();
    }

    /// <summary>
    /// Returns a new instance of the provider's class that implements the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnection" />
    /// class.
    /// </summary>
    /// <returns>
    /// A new instance of <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnection" />.
    /// </returns>
    public override DbConnection CreateConnection()
    {
      return (DbConnection) new EntityConnection();
    }

    /// <summary>
    /// Returns a new instance of the provider's class that implements the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder" />
    /// class.
    /// </summary>
    /// <returns>
    /// A new instance of <see cref="T:System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder" />.
    /// </returns>
    public override DbConnectionStringBuilder CreateConnectionStringBuilder()
    {
      return (DbConnectionStringBuilder) new EntityConnectionStringBuilder();
    }

    /// <summary>
    /// Throws a <see cref="T:System.NotSupportedException" />. This method is currently not supported.
    /// </summary>
    /// <returns>This method is currently not supported.</returns>
    public override DbDataAdapter CreateDataAdapter()
    {
      throw new NotSupportedException();
    }

    /// <summary>
    /// Returns a new instance of the provider's class that implements the
    /// <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" />
    /// class.
    /// </summary>
    /// <returns>
    /// A new instance of <see cref="T:System.Data.Entity.Core.EntityClient.EntityParameter" />.
    /// </returns>
    public override DbParameter CreateParameter()
    {
      return (DbParameter) new EntityParameter();
    }

    /// <summary>
    /// Throws a <see cref="T:System.NotSupportedException" />. This method is currently not supported.
    /// </summary>
    /// <param name="state">This method is currently not supported.</param>
    /// <returns>This method is currently not supported.</returns>
    public override CodeAccessPermission CreatePermission(PermissionState state)
    {
      throw new NotSupportedException();
    }

    object IServiceProvider.GetService(Type serviceType)
    {
      if (!(serviceType == typeof (DbProviderServices)))
        return (object) null;
      return (object) EntityProviderServices.Instance;
    }
  }
}
