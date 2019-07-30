// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Builders.TableBuilder`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.ModelConfiguration.Utilities;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Data.Entity.Migrations.Builders
{
  /// <summary>
  /// Helper class that is used to further configure a table being created from a CreateTable call on
  /// <see cref="T:System.Data.Entity.Migrations.DbMigration" />
  /// .
  /// 
  /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
  /// (such as the end user of an application). If input is accepted from such sources it should be validated
  /// before being passed to these APIs to protect against SQL injection attacks etc.
  /// </summary>
  /// <typeparam name="TColumns">The type that represents the table's columns.</typeparam>
  public class TableBuilder<TColumns>
  {
    private readonly CreateTableOperation _createTableOperation;
    private readonly DbMigration _migration;

    /// <summary>Initializes a new instance of the TableBuilder class.</summary>
    /// <param name="createTableOperation"> The table creation operation to be further configured. </param>
    /// <param name="migration"> The migration the table is created in. </param>
    public TableBuilder(CreateTableOperation createTableOperation, DbMigration migration)
    {
      Check.NotNull<CreateTableOperation>(createTableOperation, nameof (createTableOperation));
      this._createTableOperation = createTableOperation;
      this._migration = migration;
    }

    /// <summary>
    /// Specifies a primary key for the table.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="keyExpression"> A lambda expression representing the property to be used as the primary key. C#: t =&gt; t.Id VB.Net: Function(t) t.Id If the primary key is made up of multiple properties then specify an anonymous type including the properties. C#: t =&gt; new { t.Id1, t.Id2 } VB.Net: Function(t) New With { t.Id1, t.Id2 } </param>
    /// <param name="name"> The name of the primary key. If null is supplied, a default name will be generated. </param>
    /// <param name="clustered"> A value indicating whether or not this is a clustered primary key. </param>
    /// <param name="anonymousArguments"> Additional arguments that may be processed by providers. Use anonymous type syntax to specify arguments e.g. 'new { SampleArgument = "MyValue" }'. </param>
    /// <returns> Itself, so that multiple calls can be chained. </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    public TableBuilder<TColumns> PrimaryKey(
      Expression<Func<TColumns, object>> keyExpression,
      string name = null,
      bool clustered = true,
      object anonymousArguments = null)
    {
      Check.NotNull<Expression<Func<TColumns, object>>>(keyExpression, nameof (keyExpression));
      AddPrimaryKeyOperation primaryKeyOperation = new AddPrimaryKeyOperation(anonymousArguments);
      primaryKeyOperation.Name = name;
      primaryKeyOperation.IsClustered = clustered;
      AddPrimaryKeyOperation addPrimaryKeyOperation = primaryKeyOperation;
      keyExpression.GetSimplePropertyAccessList().Select<PropertyPath, ColumnModel>((Func<PropertyPath, ColumnModel>) (p => this._createTableOperation.Columns.Single<ColumnModel>((Func<ColumnModel, bool>) (c => c.ApiPropertyInfo == p.Single<PropertyInfo>())))).Each<ColumnModel>((Action<ColumnModel>) (c => addPrimaryKeyOperation.Columns.Add(c.Name)));
      this._createTableOperation.PrimaryKey = addPrimaryKeyOperation;
      return this;
    }

    /// <summary>
    /// Specifies an index to be created on the table.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="indexExpression"> A lambda expression representing the property to be indexed. C#: t =&gt; t.PropertyOne VB.Net: Function(t) t.PropertyOne If multiple properties are to be indexed then specify an anonymous type including the properties. C#: t =&gt; new { t.PropertyOne, t.PropertyTwo } VB.Net: Function(t) New With { t.PropertyOne, t.PropertyTwo } </param>
    /// <param name="name"> The name of the index. </param>
    /// <param name="unique"> A value indicating whether or not this is a unique index. </param>
    /// <param name="clustered"> A value indicating whether or not this is a clustered index. </param>
    /// <param name="anonymousArguments"> Additional arguments that may be processed by providers. Use anonymous type syntax to specify arguments e.g. 'new { SampleArgument = "MyValue" }'. </param>
    /// <returns> Itself, so that multiple calls can be chained. </returns>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    public TableBuilder<TColumns> Index(
      Expression<Func<TColumns, object>> indexExpression,
      string name = null,
      bool unique = false,
      bool clustered = false,
      object anonymousArguments = null)
    {
      Check.NotNull<Expression<Func<TColumns, object>>>(indexExpression, nameof (indexExpression));
      CreateIndexOperation createIndexOperation1 = new CreateIndexOperation(anonymousArguments);
      createIndexOperation1.Name = name;
      createIndexOperation1.Table = this._createTableOperation.Name;
      createIndexOperation1.IsUnique = unique;
      createIndexOperation1.IsClustered = clustered;
      CreateIndexOperation createIndexOperation = createIndexOperation1;
      indexExpression.GetSimplePropertyAccessList().Select<PropertyPath, ColumnModel>((Func<PropertyPath, ColumnModel>) (p => this._createTableOperation.Columns.Single<ColumnModel>((Func<ColumnModel, bool>) (c => c.ApiPropertyInfo == p.Single<PropertyInfo>())))).Each<ColumnModel>((Action<ColumnModel>) (c => createIndexOperation.Columns.Add(c.Name)));
      this._migration.AddOperation((MigrationOperation) createIndexOperation);
      return this;
    }

    /// <summary>
    /// Specifies a foreign key constraint to be created on the table.
    /// 
    /// Entity Framework Migrations APIs are not designed to accept input provided by untrusted sources
    /// (such as the end user of an application). If input is accepted from such sources it should be validated
    /// before being passed to these APIs to protect against SQL injection attacks etc.
    /// </summary>
    /// <param name="principalTable"> Name of the table that the foreign key constraint targets. </param>
    /// <param name="dependentKeyExpression"> A lambda expression representing the properties of the foreign key. C#: t =&gt; t.PropertyOne VB.Net: Function(t) t.PropertyOne If multiple properties make up the foreign key then specify an anonymous type including the properties. C#: t =&gt; new { t.PropertyOne, t.PropertyTwo } VB.Net: Function(t) New With { t.PropertyOne, t.PropertyTwo } </param>
    /// <param name="cascadeDelete"> A value indicating whether or not cascade delete should be configured on the foreign key constraint. </param>
    /// <param name="name"> The name of this foreign key constraint. If no name is supplied, a default name will be calculated. </param>
    /// <param name="anonymousArguments"> Additional arguments that may be processed by providers. Use anonymous type syntax to specify arguments e.g. 'new { SampleArgument = "MyValue" }'. </param>
    /// <returns> Itself, so that multiple calls can be chained. </returns>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public TableBuilder<TColumns> ForeignKey(
      string principalTable,
      Expression<Func<TColumns, object>> dependentKeyExpression,
      bool cascadeDelete = false,
      string name = null,
      object anonymousArguments = null)
    {
      Check.NotEmpty(principalTable, nameof (principalTable));
      Check.NotNull<Expression<Func<TColumns, object>>>(dependentKeyExpression, nameof (dependentKeyExpression));
      AddForeignKeyOperation foreignKeyOperation = new AddForeignKeyOperation(anonymousArguments);
      foreignKeyOperation.Name = name;
      foreignKeyOperation.PrincipalTable = principalTable;
      foreignKeyOperation.DependentTable = this._createTableOperation.Name;
      foreignKeyOperation.CascadeDelete = cascadeDelete;
      AddForeignKeyOperation addForeignKeyOperation = foreignKeyOperation;
      dependentKeyExpression.GetSimplePropertyAccessList().Select<PropertyPath, ColumnModel>((Func<PropertyPath, ColumnModel>) (p => this._createTableOperation.Columns.Single<ColumnModel>((Func<ColumnModel, bool>) (c => c.ApiPropertyInfo == p.Single<PropertyInfo>())))).Each<ColumnModel>((Action<ColumnModel>) (c => addForeignKeyOperation.DependentColumns.Add(c.Name)));
      this._migration.AddOperation((MigrationOperation) addForeignKeyOperation);
      return this;
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override string ToString()
    {
      return base.ToString();
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj)
    {
      return base.Equals(obj);
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    /// <summary>
    /// Gets the <see cref="T:System.Type" /> of the current instance.
    /// </summary>
    /// <returns>The exact runtime type of the current instance.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public new Type GetType()
    {
      return base.GetType();
    }

    /// <summary>
    /// Creates a shallow copy of the current <see cref="T:System.Object" />.
    /// </summary>
    /// <returns>A shallow copy of the current <see cref="T:System.Object" />.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected new object MemberwiseClone()
    {
      return base.MemberwiseClone();
    }
  }
}
