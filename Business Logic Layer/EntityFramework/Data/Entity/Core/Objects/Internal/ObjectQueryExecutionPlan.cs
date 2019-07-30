// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.Internal.ObjectQueryExecutionPlan
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.Internal.Materialization;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.EntityClient.Internal;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects.ELinq;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity.Core.Objects.Internal
{
  internal class ObjectQueryExecutionPlan
  {
    internal readonly DbCommandDefinition CommandDefinition;
    internal readonly bool Streaming;
    internal readonly ShaperFactory ResultShaperFactory;
    internal readonly TypeUsage ResultType;
    internal readonly MergeOption MergeOption;
    internal readonly IEnumerable<Tuple<ObjectParameter, QueryParameterExpression>> CompiledQueryParameters;
    private readonly EntitySet _singleEntitySet;

    public ObjectQueryExecutionPlan(
      DbCommandDefinition commandDefinition,
      ShaperFactory resultShaperFactory,
      TypeUsage resultType,
      MergeOption mergeOption,
      bool streaming,
      EntitySet singleEntitySet,
      IEnumerable<Tuple<ObjectParameter, QueryParameterExpression>> compiledQueryParameters)
    {
      this.CommandDefinition = commandDefinition;
      this.ResultShaperFactory = resultShaperFactory;
      this.ResultType = resultType;
      this.MergeOption = mergeOption;
      this.Streaming = streaming;
      this._singleEntitySet = singleEntitySet;
      this.CompiledQueryParameters = compiledQueryParameters;
    }

    internal string ToTraceString()
    {
      EntityCommandDefinition commandDefinition = this.CommandDefinition as EntityCommandDefinition;
      if (commandDefinition == null)
        return string.Empty;
      return commandDefinition.ToTraceString();
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Buffer disposed by the returned ObjectResult")]
    internal virtual ObjectResult<TResultType> Execute<TResultType>(
      ObjectContext context,
      ObjectParameterCollection parameterValues)
    {
      DbDataReader reader = (DbDataReader) null;
      BufferedDataReader bufferedDataReader = (BufferedDataReader) null;
      try
      {
        using (EntityCommand entityCommand = this.PrepareEntityCommand(context, parameterValues))
          reader = entityCommand.GetCommandDefinition().ExecuteStoreCommands(entityCommand, this.Streaming ? CommandBehavior.Default : CommandBehavior.SequentialAccess);
        ShaperFactory<TResultType> resultShaperFactory = (ShaperFactory<TResultType>) this.ResultShaperFactory;
        Shaper<TResultType> shaper;
        if (this.Streaming)
        {
          shaper = resultShaperFactory.Create(reader, context, context.MetadataWorkspace, this.MergeOption, true, this.Streaming);
        }
        else
        {
          StoreItemCollection itemCollection = (StoreItemCollection) context.MetadataWorkspace.GetItemCollection(DataSpace.SSpace);
          DbProviderServices service = DbConfiguration.DependencyResolver.GetService<DbProviderServices>((object) itemCollection.ProviderInvariantName);
          bufferedDataReader = new BufferedDataReader(reader);
          bufferedDataReader.Initialize(itemCollection.ProviderManifestToken, service, resultShaperFactory.ColumnTypes, resultShaperFactory.NullableColumns);
          shaper = resultShaperFactory.Create((DbDataReader) bufferedDataReader, context, context.MetadataWorkspace, this.MergeOption, true, this.Streaming);
        }
        TypeUsage resultItemType = this.ResultType.EdmType.BuiltInTypeKind != BuiltInTypeKind.CollectionType ? this.ResultType : ((CollectionType) this.ResultType.EdmType).TypeUsage;
        return new ObjectResult<TResultType>(shaper, this._singleEntitySet, resultItemType);
      }
      catch (Exception ex)
      {
        if (this.Streaming && reader != null)
          reader.Dispose();
        if (!this.Streaming && bufferedDataReader != null)
          bufferedDataReader.Dispose();
        throw;
      }
    }

    internal virtual async Task<ObjectResult<TResultType>> ExecuteAsync<TResultType>(
      ObjectContext context,
      ObjectParameterCollection parameterValues,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      DbDataReader storeReader = (DbDataReader) null;
      BufferedDataReader bufferedReader = (BufferedDataReader) null;
      ObjectResult<TResultType> objectResult;
      try
      {
        using (EntityCommand entityCommand = this.PrepareEntityCommand(context, parameterValues))
          storeReader = await entityCommand.GetCommandDefinition().ExecuteStoreCommandsAsync(entityCommand, this.Streaming ? CommandBehavior.Default : CommandBehavior.SequentialAccess, cancellationToken).WithCurrentCulture<DbDataReader>();
        ShaperFactory<TResultType> shaperFactory = (ShaperFactory<TResultType>) this.ResultShaperFactory;
        Shaper<TResultType> shaper;
        if (this.Streaming)
        {
          shaper = shaperFactory.Create(storeReader, context, context.MetadataWorkspace, this.MergeOption, true, this.Streaming);
        }
        else
        {
          StoreItemCollection storeItemCollection = (StoreItemCollection) context.MetadataWorkspace.GetItemCollection(DataSpace.SSpace);
          DbProviderServices providerServices = DbConfiguration.DependencyResolver.GetService<DbProviderServices>((object) storeItemCollection.ProviderInvariantName);
          bufferedReader = new BufferedDataReader(storeReader);
          await bufferedReader.InitializeAsync(storeItemCollection.ProviderManifestToken, providerServices, shaperFactory.ColumnTypes, shaperFactory.NullableColumns, cancellationToken).WithCurrentCulture();
          shaper = shaperFactory.Create((DbDataReader) bufferedReader, context, context.MetadataWorkspace, this.MergeOption, true, this.Streaming);
        }
        TypeUsage resultItemEdmType = this.ResultType.EdmType.BuiltInTypeKind != BuiltInTypeKind.CollectionType ? this.ResultType : ((CollectionType) this.ResultType.EdmType).TypeUsage;
        objectResult = new ObjectResult<TResultType>(shaper, this._singleEntitySet, resultItemEdmType);
      }
      catch (Exception ex)
      {
        if (this.Streaming && storeReader != null)
          storeReader.Dispose();
        if (!this.Streaming && bufferedReader != null)
          bufferedReader.Dispose();
        throw;
      }
      return objectResult;
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed by caller")]
    private EntityCommand PrepareEntityCommand(
      ObjectContext context,
      ObjectParameterCollection parameterValues)
    {
      EntityCommandDefinition commandDefinition = (EntityCommandDefinition) this.CommandDefinition;
      EntityConnection connection = (EntityConnection) context.Connection;
      EntityCommand entityCommand = new EntityCommand(connection, commandDefinition, context.InterceptionContext, (EntityCommand.EntityDataReaderFactory) null);
      if (context.CommandTimeout.HasValue)
        entityCommand.CommandTimeout = context.CommandTimeout.Value;
      if (parameterValues != null)
      {
        foreach (ObjectParameter parameterValue in parameterValues)
        {
          int index = entityCommand.Parameters.IndexOf(parameterValue.Name);
          if (index != -1)
            entityCommand.Parameters[index].Value = parameterValue.Value ?? (object) DBNull.Value;
        }
      }
      if (connection.CurrentTransaction != null)
        entityCommand.Transaction = connection.CurrentTransaction;
      return entityCommand;
    }
  }
}
