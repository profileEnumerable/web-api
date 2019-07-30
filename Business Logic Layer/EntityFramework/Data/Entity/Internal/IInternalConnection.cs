// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.IInternalConnection
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Common;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;

namespace System.Data.Entity.Internal
{
  internal interface IInternalConnection : IDisposable
  {
    DbConnection Connection { get; }

    string ConnectionKey { get; }

    bool ConnectionHasModel { get; }

    DbConnectionStringOrigin ConnectionStringOrigin { get; }

    AppConfig AppConfig { get; set; }

    string ProviderName { get; set; }

    string ConnectionStringName { get; }

    string OriginalConnectionString { get; }

    ObjectContext CreateObjectContextFromConnectionModel();
  }
}
