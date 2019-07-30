// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.LengthPropertyConfiguration
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Utilities;
using System.Linq.Expressions;

namespace System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive
{
  internal abstract class LengthPropertyConfiguration : PrimitivePropertyConfiguration
  {
    public bool? IsFixedLength { get; set; }

    public int? MaxLength { get; set; }

    public bool? IsMaxLength { get; set; }

    protected LengthPropertyConfiguration()
    {
    }

    protected LengthPropertyConfiguration(LengthPropertyConfiguration source)
      : base((PrimitivePropertyConfiguration) source)
    {
      Check.NotNull<LengthPropertyConfiguration>(source, nameof (source));
      this.IsFixedLength = source.IsFixedLength;
      this.MaxLength = source.MaxLength;
      this.IsMaxLength = source.IsMaxLength;
    }

    protected override void ConfigureProperty(EdmProperty property)
    {
      base.ConfigureProperty(property);
      if (this.IsFixedLength.HasValue)
        property.IsFixedLength = this.IsFixedLength;
      if (this.MaxLength.HasValue)
        property.MaxLength = this.MaxLength;
      if (!this.IsMaxLength.HasValue)
        return;
      property.IsMaxLength = this.IsMaxLength.Value;
    }

    internal override void Configure(EdmProperty column, FacetDescription facetDescription)
    {
      base.Configure(column, facetDescription);
      switch (facetDescription.FacetName)
      {
        case "FixedLength":
          EdmProperty edmProperty1 = column;
          bool? nullable1;
          if (!facetDescription.IsConstant)
          {
            bool? isFixedLength = this.IsFixedLength;
            nullable1 = isFixedLength.HasValue ? new bool?(isFixedLength.GetValueOrDefault()) : column.IsFixedLength;
          }
          else
            nullable1 = new bool?();
          edmProperty1.IsFixedLength = nullable1;
          break;
        case "MaxLength":
          EdmProperty edmProperty2 = column;
          int? nullable2;
          if (!facetDescription.IsConstant)
          {
            int? maxLength = this.MaxLength;
            nullable2 = maxLength.HasValue ? new int?(maxLength.GetValueOrDefault()) : column.MaxLength;
          }
          else
            nullable2 = new int?();
          edmProperty2.MaxLength = nullable2;
          EdmProperty edmProperty3 = column;
          int num;
          if (!facetDescription.IsConstant)
          {
            bool? isMaxLength = this.IsMaxLength;
            num = isMaxLength.HasValue ? (isMaxLength.GetValueOrDefault() ? 1 : 0) : (column.IsMaxLength ? 1 : 0);
          }
          else
            num = 0;
          edmProperty3.IsMaxLength = num != 0;
          break;
      }
    }

    internal override void CopyFrom(PrimitivePropertyConfiguration other)
    {
      base.CopyFrom(other);
      LengthPropertyConfiguration propertyConfiguration = other as LengthPropertyConfiguration;
      if (propertyConfiguration == null)
        return;
      this.IsFixedLength = propertyConfiguration.IsFixedLength;
      this.MaxLength = propertyConfiguration.MaxLength;
      this.IsMaxLength = propertyConfiguration.IsMaxLength;
    }

    internal override void FillFrom(PrimitivePropertyConfiguration other, bool inCSpace)
    {
      base.FillFrom(other, inCSpace);
      LengthPropertyConfiguration propertyConfiguration = other as LengthPropertyConfiguration;
      if (propertyConfiguration == null)
        return;
      if (!this.IsFixedLength.HasValue)
        this.IsFixedLength = propertyConfiguration.IsFixedLength;
      if (!this.MaxLength.HasValue)
        this.MaxLength = propertyConfiguration.MaxLength;
      if (this.IsMaxLength.HasValue)
        return;
      this.IsMaxLength = propertyConfiguration.IsMaxLength;
    }

    internal override void MakeCompatibleWith(PrimitivePropertyConfiguration other, bool inCSpace)
    {
      base.MakeCompatibleWith(other, inCSpace);
      LengthPropertyConfiguration propertyConfiguration = other as LengthPropertyConfiguration;
      if (propertyConfiguration == null)
        return;
      if (propertyConfiguration.IsFixedLength.HasValue)
        this.IsFixedLength = new bool?();
      if (propertyConfiguration.MaxLength.HasValue)
        this.MaxLength = new int?();
      if (!propertyConfiguration.IsMaxLength.HasValue)
        return;
      this.IsMaxLength = new bool?();
    }

    internal override bool IsCompatible(
      PrimitivePropertyConfiguration other,
      bool inCSpace,
      out string errorMessage)
    {
      LengthPropertyConfiguration other1 = other as LengthPropertyConfiguration;
      bool flag1 = base.IsCompatible(other, inCSpace, out errorMessage);
      int num1;
      if (other1 != null)
        num1 = this.IsCompatible<bool, LengthPropertyConfiguration>((Expression<Func<LengthPropertyConfiguration, bool?>>) (c => c.IsFixedLength), other1, ref errorMessage) ? 1 : 0;
      else
        num1 = 1;
      bool flag2 = num1 != 0;
      int num2;
      if (other1 != null)
        num2 = this.IsCompatible<bool, LengthPropertyConfiguration>((Expression<Func<LengthPropertyConfiguration, bool?>>) (c => c.IsMaxLength), other1, ref errorMessage) ? 1 : 0;
      else
        num2 = 1;
      bool flag3 = num2 != 0;
      int num3;
      if (other1 != null)
        num3 = this.IsCompatible<int, LengthPropertyConfiguration>((Expression<Func<LengthPropertyConfiguration, int?>>) (c => c.MaxLength), other1, ref errorMessage) ? 1 : 0;
      else
        num3 = 1;
      bool flag4 = num3 != 0;
      if (flag1 && flag2 && flag3)
        return flag4;
      return false;
    }
  }
}
