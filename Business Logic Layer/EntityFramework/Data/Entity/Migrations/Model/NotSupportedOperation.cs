// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Model.NotSupportedOperation
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Migrations.Model
{
  /// <summary>
  /// Represents a migration operation that can not be performed, possibly because it is not supported by the targeted database provider.
  /// </summary>
  public class NotSupportedOperation : MigrationOperation
  {
    internal static readonly NotSupportedOperation Instance = new NotSupportedOperation();

    private NotSupportedOperation()
      : base((object) null)
    {
    }

    /// <summary>
    /// Gets a value indicating if this operation may result in data loss. Always returns false.
    /// </summary>
    public override bool IsDestructiveChange
    {
      get
      {
        return false;
      }
    }
  }
}
