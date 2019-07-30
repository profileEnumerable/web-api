// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.CommandHelper
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Data.Entity.Spatial;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity.Core.Common.Utils
{
  internal static class CommandHelper
  {
    internal static void ConsumeReader(DbDataReader reader)
    {
      if (reader == null || reader.IsClosed)
        return;
      do
        ;
      while (reader.NextResult());
    }

    internal static async Task ConsumeReaderAsync(
      DbDataReader reader,
      CancellationToken cancellationToken)
    {
      if (reader == null || reader.IsClosed)
        return;
      cancellationToken.ThrowIfCancellationRequested();
      while (true)
      {
        if (await reader.NextResultAsync(cancellationToken).WithCurrentCulture<bool>())
          cancellationToken.ThrowIfCancellationRequested();
        else
          break;
      }
    }

    internal static void ParseFunctionImportCommandText(
      string commandText,
      string defaultContainerName,
      out string containerName,
      out string functionImportName)
    {
      string[] strArray = commandText.Split('.');
      containerName = (string) null;
      functionImportName = (string) null;
      if (2 == strArray.Length)
      {
        containerName = strArray[0].Trim();
        functionImportName = strArray[1].Trim();
      }
      else if (1 == strArray.Length && defaultContainerName != null)
      {
        containerName = defaultContainerName;
        functionImportName = strArray[0].Trim();
      }
      if (string.IsNullOrEmpty(containerName) || string.IsNullOrEmpty(functionImportName))
        throw new InvalidOperationException(Strings.EntityClient_InvalidStoredProcedureCommandText);
    }

    internal static void SetStoreProviderCommandState(
      EntityCommand entityCommand,
      EntityTransaction entityTransaction,
      DbCommand storeProviderCommand)
    {
      storeProviderCommand.CommandTimeout = entityCommand.CommandTimeout;
      storeProviderCommand.Connection = entityCommand.Connection.StoreConnection;
      storeProviderCommand.Transaction = entityTransaction?.StoreTransaction;
      storeProviderCommand.UpdatedRowSource = entityCommand.UpdatedRowSource;
    }

    internal static void SetEntityParameterValues(
      EntityCommand entityCommand,
      DbCommand storeProviderCommand,
      EntityConnection connection)
    {
      foreach (DbParameter parameter1 in storeProviderCommand.Parameters)
      {
        if ((parameter1.Direction & ParameterDirection.Output) != (ParameterDirection) 0)
        {
          int index = entityCommand.Parameters.IndexOf(parameter1.ParameterName);
          if (0 <= index)
          {
            EntityParameter parameter2 = entityCommand.Parameters[index];
            object fromProviderValue = parameter1.Value;
            TypeUsage typeUsage = parameter2.GetTypeUsage();
            if (Helper.IsSpatialType(typeUsage))
              fromProviderValue = CommandHelper.GetSpatialValueFromProviderValue(fromProviderValue, (PrimitiveType) typeUsage.EdmType, connection);
            parameter2.Value = fromProviderValue;
          }
        }
      }
    }

    private static object GetSpatialValueFromProviderValue(
      object spatialValue,
      PrimitiveType parameterType,
      EntityConnection connection)
    {
      DbSpatialServices spatialServices = DbProviderServices.GetSpatialServices(DbConfiguration.DependencyResolver, connection);
      if (Helper.IsGeographicType(parameterType))
        return (object) spatialServices.GeographyFromProviderValue(spatialValue);
      return (object) spatialServices.GeometryFromProviderValue(spatialValue);
    }

    internal static EdmFunction FindFunctionImport(
      MetadataWorkspace workspace,
      string containerName,
      string functionImportName)
    {
      EntityContainer entityContainer;
      if (!workspace.TryGetEntityContainer(containerName, DataSpace.CSpace, out entityContainer))
        throw new InvalidOperationException(Strings.EntityClient_UnableToFindFunctionImportContainer((object) containerName));
      EdmFunction edmFunction = (EdmFunction) null;
      foreach (EdmFunction functionImport in entityContainer.FunctionImports)
      {
        if (functionImport.Name == functionImportName)
        {
          edmFunction = functionImport;
          break;
        }
      }
      if (edmFunction == null)
        throw new InvalidOperationException(Strings.EntityClient_UnableToFindFunctionImport((object) containerName, (object) functionImportName));
      if (edmFunction.IsComposableAttribute)
        throw new InvalidOperationException(Strings.EntityClient_FunctionImportMustBeNonComposable((object) (containerName + "." + functionImportName)));
      return edmFunction;
    }
  }
}
