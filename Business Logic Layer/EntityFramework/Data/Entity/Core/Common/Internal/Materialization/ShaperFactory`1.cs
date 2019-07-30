// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Internal.Materialization.ShaperFactory`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Common.Internal.Materialization
{
  internal class ShaperFactory<T> : ShaperFactory
  {
    private readonly int _stateCount;
    private readonly CoordinatorFactory<T> _rootCoordinatorFactory;
    [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Used in the debug build")]
    private readonly MergeOption _mergeOption;

    internal ShaperFactory(
      int stateCount,
      CoordinatorFactory<T> rootCoordinatorFactory,
      Type[] columnTypes,
      bool[] nullableColumns,
      MergeOption mergeOption)
    {
      this._stateCount = stateCount;
      this._rootCoordinatorFactory = rootCoordinatorFactory;
      this.ColumnTypes = columnTypes;
      this.NullableColumns = nullableColumns;
      this._mergeOption = mergeOption;
    }

    public Type[] ColumnTypes { get; private set; }

    public bool[] NullableColumns { get; private set; }

    internal Shaper<T> Create(
      DbDataReader reader,
      ObjectContext context,
      MetadataWorkspace workspace,
      MergeOption mergeOption,
      bool readerOwned,
      bool streaming)
    {
      return new Shaper<T>(reader, context, workspace, mergeOption, this._stateCount, this._rootCoordinatorFactory, readerOwned, streaming);
    }
  }
}
