// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.EntityClient.Internal.IEntityAdapter
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity.Core.EntityClient.Internal
{
  internal interface IEntityAdapter
  {
    DbConnection Connection { get; set; }

    bool AcceptChangesDuringUpdate { get; set; }

    int? CommandTimeout { get; set; }

    int Update();

    Task<int> UpdateAsync(CancellationToken cancellationToken);
  }
}
