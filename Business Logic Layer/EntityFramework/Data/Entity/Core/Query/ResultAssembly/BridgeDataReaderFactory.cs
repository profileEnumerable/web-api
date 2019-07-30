// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.ResultAssembly.BridgeDataReaderFactory
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Common.Internal.Materialization;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.Internal;
using System.Data.Entity.Core.Query.InternalTrees;
using System.Linq;

namespace System.Data.Entity.Core.Query.ResultAssembly
{
  internal class BridgeDataReaderFactory
  {
    private readonly Translator _translator;

    public BridgeDataReaderFactory(Translator translator = null)
    {
      this._translator = translator ?? new Translator();
    }

    public virtual DbDataReader Create(
      DbDataReader storeDataReader,
      ColumnMap columnMap,
      MetadataWorkspace workspace,
      IEnumerable<ColumnMap> nextResultColumnMaps)
    {
      KeyValuePair<Shaper<RecordState>, CoordinatorFactory<RecordState>> shaperInfo = this.CreateShaperInfo(storeDataReader, columnMap, workspace);
      return (DbDataReader) new BridgeDataReader(shaperInfo.Key, shaperInfo.Value, 0, this.GetNextResultShaperInfo(storeDataReader, workspace, nextResultColumnMaps).GetEnumerator());
    }

    private KeyValuePair<Shaper<RecordState>, CoordinatorFactory<RecordState>> CreateShaperInfo(
      DbDataReader storeDataReader,
      ColumnMap columnMap,
      MetadataWorkspace workspace)
    {
      Shaper<RecordState> key = this._translator.TranslateColumnMap<RecordState>(columnMap, workspace, (SpanIndex) null, MergeOption.NoTracking, true, true).Create(storeDataReader, (ObjectContext) null, workspace, MergeOption.NoTracking, true, true);
      return new KeyValuePair<Shaper<RecordState>, CoordinatorFactory<RecordState>>(key, key.RootCoordinator.TypedCoordinatorFactory);
    }

    private IEnumerable<KeyValuePair<Shaper<RecordState>, CoordinatorFactory<RecordState>>> GetNextResultShaperInfo(
      DbDataReader storeDataReader,
      MetadataWorkspace workspace,
      IEnumerable<ColumnMap> nextResultColumnMaps)
    {
      return nextResultColumnMaps.Select<ColumnMap, KeyValuePair<Shaper<RecordState>, CoordinatorFactory<RecordState>>>((Func<ColumnMap, KeyValuePair<Shaper<RecordState>, CoordinatorFactory<RecordState>>>) (nextResultColumnMap => this.CreateShaperInfo(storeDataReader, nextResultColumnMap, workspace)));
    }
  }
}
