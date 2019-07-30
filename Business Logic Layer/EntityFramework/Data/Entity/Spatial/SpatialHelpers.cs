// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Spatial.SpatialHelpers
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Common;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity.Spatial
{
  internal static class SpatialHelpers
  {
    internal static object GetSpatialValue(
      MetadataWorkspace workspace,
      DbDataReader reader,
      TypeUsage columnType,
      int columnOrdinal)
    {
      DbSpatialDataReader spatialDataReader = SpatialHelpers.CreateSpatialDataReader(workspace, reader);
      if (Helper.IsGeographicType((PrimitiveType) columnType.EdmType))
        return (object) spatialDataReader.GetGeography(columnOrdinal);
      return (object) spatialDataReader.GetGeometry(columnOrdinal);
    }

    internal static async Task<object> GetSpatialValueAsync(
      MetadataWorkspace workspace,
      DbDataReader reader,
      TypeUsage columnType,
      int columnOrdinal,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      DbSpatialDataReader spatialReader = SpatialHelpers.CreateSpatialDataReader(workspace, reader);
      if (Helper.IsGeographicType((PrimitiveType) columnType.EdmType))
        return (object) await spatialReader.GetGeographyAsync(columnOrdinal, cancellationToken).WithCurrentCulture<DbGeography>();
      return (object) await spatialReader.GetGeometryAsync(columnOrdinal, cancellationToken).WithCurrentCulture<DbGeometry>();
    }

    internal static DbSpatialDataReader CreateSpatialDataReader(
      MetadataWorkspace workspace,
      DbDataReader reader)
    {
      StoreItemCollection itemCollection = (StoreItemCollection) workspace.GetItemCollection(DataSpace.SSpace);
      DbSpatialDataReader spatialDataReader = itemCollection.ProviderFactory.GetProviderServices().GetSpatialDataReader(reader, itemCollection.ProviderManifestToken);
      if (spatialDataReader == null)
        throw new ProviderIncompatibleException(Strings.ProviderDidNotReturnSpatialServices);
      return spatialDataReader;
    }
  }
}
