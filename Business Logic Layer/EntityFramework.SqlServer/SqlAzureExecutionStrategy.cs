// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.SqlAzureExecutionStrategy
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Data.Entity.Infrastructure;

namespace System.Data.Entity.SqlServer
{
  /// <summary>
  /// An <see cref="T:System.Data.Entity.Infrastructure.IDbExecutionStrategy" /> that retries actions that throw exceptions caused by SQL Azure transient failures.
  /// </summary>
  /// <remarks>
  /// This execution strategy will retry the operation on <see cref="T:System.TimeoutException" /> and <see cref="T:System.Data.SqlClient.SqlException" />
  /// if the <see cref="P:System.Data.SqlClient.SqlException.Errors" /> contains any of the following error numbers:
  /// 40613, 40501, 40197, 10929, 10928, 10060, 10054, 10053, 233, 64 and 20
  /// </remarks>
  public class SqlAzureExecutionStrategy : DbExecutionStrategy
  {
    /// <summary>
    /// Creates a new instance of <see cref="T:System.Data.Entity.SqlServer.SqlAzureExecutionStrategy" />.
    /// </summary>
    /// <remarks>
    /// The default retry limit is 5, which means that the total amount of time spent between retries is 26 seconds plus the random factor.
    /// </remarks>
    public SqlAzureExecutionStrategy()
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="T:System.Data.Entity.SqlServer.SqlAzureExecutionStrategy" /> with the specified limits for
    /// number of retries and the delay between retries.
    /// </summary>
    /// <param name="maxRetryCount"> The maximum number of retry attempts. </param>
    /// <param name="maxDelay"> The maximum delay in milliseconds between retries. </param>
    public SqlAzureExecutionStrategy(int maxRetryCount, TimeSpan maxDelay)
      : base(maxRetryCount, maxDelay)
    {
    }

    /// <inheritdoc />
    protected override bool ShouldRetryOn(Exception exception)
    {
      return SqlAzureRetriableExceptionDetector.ShouldRetryOn(exception);
    }
  }
}
