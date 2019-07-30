// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.EdmMetadataContext
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Common;
using System.Data.Entity.Infrastructure;

namespace System.Data.Entity.Internal
{
  internal class EdmMetadataContext : DbContext
  {
    public const string TableName = "EdmMetadata";

    static EdmMetadataContext()
    {
      Database.SetInitializer<EdmMetadataContext>((IDatabaseInitializer<EdmMetadataContext>) null);
    }

    public EdmMetadataContext(DbConnection existingConnection)
      : base(existingConnection, false)
    {
    }

    public virtual IDbSet<EdmMetadata> Metadata { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      EdmMetadataContext.ConfigureEdmMetadata(modelBuilder.ModelConfiguration);
    }

    public static void ConfigureEdmMetadata(System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration modelConfiguration)
    {
      modelConfiguration.Entity(typeof (EdmMetadata)).ToTable("EdmMetadata");
    }
  }
}
