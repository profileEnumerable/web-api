// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Model.HistoryOperation
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.Migrations.Model
{
  /// <summary>
  /// Operation representing DML changes to the migrations history table.
  /// The migrations history table is used to store a log of the migrations that have been applied to the database.
  /// 
  /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
  /// (such as the end user of an application). If input is accepted from such sources it should be validated
  /// before being passed to these APIs to protect against SQL injection attacks etc.
  /// </summary>
  public class HistoryOperation : MigrationOperation
  {
    private readonly IList<DbModificationCommandTree> _commandTrees;

    /// <summary>
    /// Initializes a new instance of the HistoryOperation class.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="commandTrees"> A sequence of command trees representing the operations being applied to the history table. </param>
    /// <param name="anonymousArguments"> Use anonymous type syntax to specify arguments e.g. 'new { SampleArgument = "MyValue" }'. </param>
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    public HistoryOperation(
      IList<DbModificationCommandTree> commandTrees,
      object anonymousArguments = null)
      : base(anonymousArguments)
    {
      Check.NotNull<IList<DbModificationCommandTree>>(commandTrees, nameof (commandTrees));
      if (!commandTrees.Any<DbModificationCommandTree>())
        throw new ArgumentException(Strings.CollectionEmpty((object) nameof (commandTrees), (object) nameof (HistoryOperation)));
      this._commandTrees = commandTrees;
    }

    /// <summary>
    /// A sequence of commands representing the operations being applied to the history table.
    /// </summary>
    public IList<DbModificationCommandTree> CommandTrees
    {
      get
      {
        return this._commandTrees;
      }
    }

    /// <inheritdoc />
    public override bool IsDestructiveChange
    {
      get
      {
        return false;
      }
    }
  }
}
