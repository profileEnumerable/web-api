// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.BinaryPropertyConfiguration
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq.Expressions;

namespace System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive
{
  internal class BinaryPropertyConfiguration : LengthPropertyConfiguration
  {
    public bool? IsRowVersion { get; set; }

    public BinaryPropertyConfiguration()
    {
    }

    private BinaryPropertyConfiguration(BinaryPropertyConfiguration source)
      : base((LengthPropertyConfiguration) source)
    {
      this.IsRowVersion = source.IsRowVersion;
    }

    internal override PrimitivePropertyConfiguration Clone()
    {
      return (PrimitivePropertyConfiguration) new BinaryPropertyConfiguration(this);
    }

    protected override void ConfigureProperty(EdmProperty property)
    {
      if (this.IsRowVersion.HasValue && this.IsRowVersion.Value)
      {
        ConcurrencyMode? concurrencyMode = this.ConcurrencyMode;
        this.ConcurrencyMode = new ConcurrencyMode?(concurrencyMode.HasValue ? concurrencyMode.GetValueOrDefault() : ConcurrencyMode.Fixed);
        DatabaseGeneratedOption? databaseGeneratedOption = this.DatabaseGeneratedOption;
        this.DatabaseGeneratedOption = new DatabaseGeneratedOption?(databaseGeneratedOption.HasValue ? databaseGeneratedOption.GetValueOrDefault() : DatabaseGeneratedOption.Computed);
        bool? isNullable = this.IsNullable;
        this.IsNullable = new bool?(isNullable.HasValue && isNullable.GetValueOrDefault());
        this.MaxLength = new int?(this.MaxLength ?? 8);
      }
      base.ConfigureProperty(property);
    }

    protected override void ConfigureColumn(
      EdmProperty column,
      EntityType table,
      DbProviderManifest providerManifest)
    {
      if (this.IsRowVersion.HasValue && this.IsRowVersion.Value)
        this.ColumnType = this.ColumnType ?? "rowversion";
      base.ConfigureColumn(column, table, providerManifest);
      if (!this.IsRowVersion.HasValue || !this.IsRowVersion.Value)
        return;
      column.MaxLength = new int?();
    }

    internal override void CopyFrom(PrimitivePropertyConfiguration other)
    {
      base.CopyFrom(other);
      BinaryPropertyConfiguration propertyConfiguration = other as BinaryPropertyConfiguration;
      if (propertyConfiguration == null)
        return;
      this.IsRowVersion = propertyConfiguration.IsRowVersion;
    }

    internal override void FillFrom(PrimitivePropertyConfiguration other, bool inCSpace)
    {
      base.FillFrom(other, inCSpace);
      BinaryPropertyConfiguration propertyConfiguration = other as BinaryPropertyConfiguration;
      if (propertyConfiguration == null || this.IsRowVersion.HasValue)
        return;
      this.IsRowVersion = propertyConfiguration.IsRowVersion;
    }

    internal override void MakeCompatibleWith(PrimitivePropertyConfiguration other, bool inCSpace)
    {
      base.MakeCompatibleWith(other, inCSpace);
      BinaryPropertyConfiguration propertyConfiguration = other as BinaryPropertyConfiguration;
      if (propertyConfiguration == null || !propertyConfiguration.IsRowVersion.HasValue)
        return;
      this.IsRowVersion = new bool?();
    }

    internal override bool IsCompatible(
      PrimitivePropertyConfiguration other,
      bool inCSpace,
      out string errorMessage)
    {
      BinaryPropertyConfiguration other1 = other as BinaryPropertyConfiguration;
      bool flag1 = base.IsCompatible(other, inCSpace, out errorMessage);
      int num;
      if (other1 != null)
        num = this.IsCompatible<bool, BinaryPropertyConfiguration>((Expression<Func<BinaryPropertyConfiguration, bool?>>) (c => c.IsRowVersion), other1, ref errorMessage) ? 1 : 0;
      else
        num = 1;
      bool flag2 = num != 0;
      if (flag1)
        return flag2;
      return false;
    }
  }
}
