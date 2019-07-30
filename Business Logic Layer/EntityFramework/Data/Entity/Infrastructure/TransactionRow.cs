// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.TransactionRow
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Infrastructure
{
  /// <summary>Rrepresents a transaction</summary>
  public class TransactionRow
  {
    /// <summary>A unique id assigned to a transaction object.</summary>
    public Guid Id { get; set; }

    /// <summary>The local time when the transaction was started.</summary>
    public DateTime CreationTime { get; set; }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
      TransactionRow transactionRow = obj as TransactionRow;
      if (transactionRow != null)
        return this.Id == transactionRow.Id;
      return false;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
      return this.Id.GetHashCode();
    }
  }
}
