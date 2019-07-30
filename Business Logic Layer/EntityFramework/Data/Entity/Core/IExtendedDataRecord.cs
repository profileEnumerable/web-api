﻿// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.IExtendedDataRecord
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core
{
  /// <summary>
  /// DataRecord interface supporting structured types and rich metadata information.
  /// </summary>
  public interface IExtendedDataRecord : IDataRecord
  {
    /// <summary>
    /// Gets <see cref="T:System.Data.Entity.Core.Common.DataRecordInfo" /> for this
    /// <see cref="T:System.Data.Entity.Core.IExtendedDataRecord" />
    /// .
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Common.DataRecordInfo" /> object.
    /// </returns>
    DataRecordInfo DataRecordInfo { get; }

    /// <summary>
    /// Gets a <see cref="T:System.Data.Common.DbDataRecord" /> object with the specified index.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Common.DbDataRecord" /> object.
    /// </returns>
    /// <param name="i">The index of the row.</param>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "i")]
    DbDataRecord GetDataRecord(int i);

    /// <summary>
    /// Returns nested readers as <see cref="T:System.Data.Common.DbDataReader" /> objects.
    /// </summary>
    /// <returns>
    /// Nested readers as <see cref="T:System.Data.Common.DbDataReader" /> objects.
    /// </returns>
    /// <param name="i">The ordinal of the column.</param>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "i")]
    DbDataReader GetDataReader(int i);
  }
}
