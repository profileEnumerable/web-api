﻿// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Spatial.DbSpatialDataReader
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity.Spatial
{
  /// <summary>
  /// A provider-independent service API for geospatial (Geometry/Geography) type support.
  /// </summary>
  public abstract class DbSpatialDataReader
  {
    /// <summary>
    /// When implemented in derived types, reads an instance of <see cref="T:System.Data.Entity.Spatial.DbGeography" /> from the column at the specified column ordinal.
    /// </summary>
    /// <returns>The instance of DbGeography at the specified column value</returns>
    /// <param name="ordinal">The ordinal of the column that contains the geography value</param>
    public abstract DbGeography GetGeography(int ordinal);

    /// <summary>
    /// Asynchronously reads an instance of <see cref="T:System.Data.Entity.Spatial.DbGeography" /> from the column at the specified column ordinal.
    /// </summary>
    /// <remarks>
    /// Providers should override with an appropriate implementation.
    /// The default implementation invokes the synchronous <see cref="M:System.Data.Entity.Spatial.DbSpatialDataReader.GetGeography(System.Int32)" /> method and returns
    /// a completed task, blocking the calling thread.
    /// </remarks>
    /// <param name="ordinal"> The ordinal of the column that contains the geography value. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the instance of <see cref="T:System.Data.Entity.Spatial.DbGeography" /> at the specified column value.
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception provided in the returned task.")]
    public virtual Task<DbGeography> GetGeographyAsync(
      int ordinal,
      CancellationToken cancellationToken)
    {
      if (cancellationToken.IsCancellationRequested)
        return TaskHelper.FromCancellation<DbGeography>();
      try
      {
        return Task.FromResult<DbGeography>(this.GetGeography(ordinal));
      }
      catch (Exception ex)
      {
        return TaskHelper.FromException<DbGeography>(ex);
      }
    }

    /// <summary>
    /// When implemented in derived types, reads an instance of <see cref="T:System.Data.Entity.Spatial.DbGeometry" /> from the column at the specified column ordinal.
    /// </summary>
    /// <returns>The instance of DbGeometry at the specified column value</returns>
    /// <param name="ordinal">The ordinal of the data record column that contains the provider-specific geometry data</param>
    public abstract DbGeometry GetGeometry(int ordinal);

    /// <summary>
    /// Asynchronously reads an instance of <see cref="T:System.Data.Entity.Spatial.DbGeometry" /> from the column at the specified column ordinal.
    /// </summary>
    /// <remarks>
    /// Providers should override with an appropriate implementation.
    /// The default implementation invokes the synchronous <see cref="M:System.Data.Entity.Spatial.DbSpatialDataReader.GetGeometry(System.Int32)" /> method and returns
    /// a completed task, blocking the calling thread.
    /// </remarks>
    /// <param name="ordinal"> The ordinal of the data record column that contains the provider-specific geometry data. </param>
    /// <param name="cancellationToken">
    /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the instance of <see cref="T:System.Data.Entity.Spatial.DbGeometry" /> at the specified column value.
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception provided in the returned task.")]
    public virtual Task<DbGeometry> GetGeometryAsync(
      int ordinal,
      CancellationToken cancellationToken)
    {
      if (cancellationToken.IsCancellationRequested)
        return TaskHelper.FromCancellation<DbGeometry>();
      try
      {
        return Task.FromResult<DbGeometry>(this.GetGeometry(ordinal));
      }
      catch (Exception ex)
      {
        return TaskHelper.FromException<DbGeometry>(ex);
      }
    }

    /// <summary>
    /// Returns whether the column at the specified column ordinal is of geography type
    /// </summary>
    /// <param name="ordinal">The column ordinal.</param>
    /// <returns>
    /// <c>true</c> if the column at the specified column ordinal is of geography type;
    /// <c>false</c> otherwise.
    /// </returns>
    public abstract bool IsGeographyColumn(int ordinal);

    /// <summary>
    /// Returns whether the column at the specified column ordinal is of geometry type
    /// </summary>
    /// <param name="ordinal">The column ordinal.</param>
    /// <returns>
    /// <c>true</c> if the column at the specified column ordinal is of geometry type;
    /// <c>false</c> otherwise.
    /// </returns>
    public abstract bool IsGeometryColumn(int ordinal);
  }
}
