// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.NextResultGenerator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data.Entity.Core.Common.Internal.Materialization;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.Objects
{
  internal class NextResultGenerator
  {
    private readonly EntityCommand _entityCommand;
    private readonly ReadOnlyCollection<EntitySet> _entitySets;
    private readonly ObjectContext _context;
    private readonly EdmType[] _edmTypes;
    private readonly int _resultSetIndex;
    private readonly bool _streaming;
    private readonly MergeOption _mergeOption;

    internal NextResultGenerator(
      ObjectContext context,
      EntityCommand entityCommand,
      EdmType[] edmTypes,
      ReadOnlyCollection<EntitySet> entitySets,
      MergeOption mergeOption,
      bool streaming,
      int resultSetIndex)
    {
      this._context = context;
      this._entityCommand = entityCommand;
      this._entitySets = entitySets;
      this._edmTypes = edmTypes;
      this._resultSetIndex = resultSetIndex;
      this._streaming = streaming;
      this._mergeOption = mergeOption;
    }

    internal ObjectResult<TElement> GetNextResult<TElement>(DbDataReader storeReader)
    {
      bool flag;
      try
      {
        flag = storeReader.NextResult();
      }
      catch (Exception ex)
      {
        if (ex.IsCatchableExceptionType())
          throw new EntityCommandExecutionException(Strings.EntityClient_StoreReaderFailed, ex);
        throw;
      }
      if (!flag)
        return (ObjectResult<TElement>) null;
      MetadataHelper.CheckFunctionImportReturnType<TElement>(this._edmTypes[this._resultSetIndex], this._context.MetadataWorkspace);
      return this._context.MaterializedDataRecord<TElement>(this._entityCommand, storeReader, this._resultSetIndex, this._entitySets, this._edmTypes, (ShaperFactory<TElement>) null, this._mergeOption, this._streaming);
    }
  }
}
