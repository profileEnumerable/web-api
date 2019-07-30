// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Infrastructure.MigrationAssembly
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Migrations.Utilities;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.Migrations.Infrastructure
{
  internal class MigrationAssembly
  {
    private readonly IList<IMigrationMetadata> _migrations;

    public static string CreateMigrationId(string migrationName)
    {
      return UtcNowGenerator.UtcNowAsMigrationIdTimestamp() + "_" + migrationName;
    }

    public static string CreateBootstrapMigrationId()
    {
      return new string('0', 15) + "_" + Strings.BootstrapMigration;
    }

    protected MigrationAssembly()
    {
    }

    public MigrationAssembly(Assembly migrationsAssembly, string migrationsNamespace)
    {
      this._migrations = (IList<IMigrationMetadata>) migrationsAssembly.GetAccessibleTypes().Where<Type>((Func<Type, bool>) (t =>
      {
        if (t.IsSubclassOf(typeof (DbMigration)) && typeof (IMigrationMetadata).IsAssignableFrom(t) && (t.GetPublicConstructor() != (ConstructorInfo) null && !t.IsAbstract()) && !t.IsGenericType())
          return t.Namespace == migrationsNamespace;
        return false;
      })).Select<Type, IMigrationMetadata>((Func<Type, IMigrationMetadata>) (t => (IMigrationMetadata) Activator.CreateInstance(t))).Where<IMigrationMetadata>((Func<IMigrationMetadata, bool>) (mm =>
      {
        if (!string.IsNullOrWhiteSpace(mm.Id))
          return mm.Id.IsValidMigrationId();
        return false;
      })).OrderBy<IMigrationMetadata, string>((Func<IMigrationMetadata, string>) (mm => mm.Id)).ToList<IMigrationMetadata>();
    }

    public virtual IEnumerable<string> MigrationIds
    {
      get
      {
        return (IEnumerable<string>) this._migrations.Select<IMigrationMetadata, string>((Func<IMigrationMetadata, string>) (t => t.Id)).ToList<string>();
      }
    }

    public virtual string UniquifyName(string migrationName)
    {
      return this._migrations.Select<IMigrationMetadata, string>((Func<IMigrationMetadata, string>) (m => m.GetType().Name)).Uniquify(migrationName);
    }

    public virtual DbMigration GetMigration(string migrationId)
    {
      DbMigration dbMigration = (DbMigration) this._migrations.SingleOrDefault<IMigrationMetadata>((Func<IMigrationMetadata, bool>) (m => m.Id.StartsWith(migrationId, StringComparison.Ordinal)));
      dbMigration?.Reset();
      return dbMigration;
    }
  }
}
