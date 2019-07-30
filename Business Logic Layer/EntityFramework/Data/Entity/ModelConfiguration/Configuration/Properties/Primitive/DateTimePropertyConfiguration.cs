// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.DateTimePropertyConfiguration
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Linq.Expressions;

namespace System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive
{
  internal class DateTimePropertyConfiguration : PrimitivePropertyConfiguration
  {
    public byte? Precision { get; set; }

    public DateTimePropertyConfiguration()
    {
    }

    private DateTimePropertyConfiguration(DateTimePropertyConfiguration source)
      : base((PrimitivePropertyConfiguration) source)
    {
      this.Precision = source.Precision;
    }

    internal override PrimitivePropertyConfiguration Clone()
    {
      return (PrimitivePropertyConfiguration) new DateTimePropertyConfiguration(this);
    }

    protected override void ConfigureProperty(EdmProperty property)
    {
      base.ConfigureProperty(property);
      byte? precision = this.Precision;
      if (!(precision.HasValue ? new int?((int) precision.GetValueOrDefault()) : new int?()).HasValue)
        return;
      property.Precision = this.Precision;
    }

    internal override void Configure(EdmProperty column, FacetDescription facetDescription)
    {
      base.Configure(column, facetDescription);
      switch (facetDescription.FacetName)
      {
        case "Precision":
          EdmProperty edmProperty = column;
          byte? nullable;
          if (!facetDescription.IsConstant)
          {
            byte? precision = this.Precision;
            nullable = precision.HasValue ? new byte?(precision.GetValueOrDefault()) : column.Precision;
          }
          else
            nullable = new byte?();
          edmProperty.Precision = nullable;
          break;
      }
    }

    internal override void CopyFrom(PrimitivePropertyConfiguration other)
    {
      base.CopyFrom(other);
      DateTimePropertyConfiguration propertyConfiguration = other as DateTimePropertyConfiguration;
      if (propertyConfiguration == null)
        return;
      this.Precision = propertyConfiguration.Precision;
    }

    internal override void FillFrom(PrimitivePropertyConfiguration other, bool inCSpace)
    {
      base.FillFrom(other, inCSpace);
      DateTimePropertyConfiguration propertyConfiguration = other as DateTimePropertyConfiguration;
      if (propertyConfiguration == null)
        return;
      byte? precision = this.Precision;
      if ((precision.HasValue ? new int?((int) precision.GetValueOrDefault()) : new int?()).HasValue)
        return;
      this.Precision = propertyConfiguration.Precision;
    }

    internal override void MakeCompatibleWith(PrimitivePropertyConfiguration other, bool inCSpace)
    {
      base.MakeCompatibleWith(other, inCSpace);
      DateTimePropertyConfiguration propertyConfiguration = other as DateTimePropertyConfiguration;
      if (propertyConfiguration == null)
        return;
      byte? precision = propertyConfiguration.Precision;
      if (!(precision.HasValue ? new int?((int) precision.GetValueOrDefault()) : new int?()).HasValue)
        return;
      this.Precision = new byte?();
    }

    internal override bool IsCompatible(
      PrimitivePropertyConfiguration other,
      bool inCSpace,
      out string errorMessage)
    {
      DateTimePropertyConfiguration other1 = other as DateTimePropertyConfiguration;
      bool flag1 = base.IsCompatible(other, inCSpace, out errorMessage);
      int num;
      if (other1 != null)
        num = this.IsCompatible<byte, DateTimePropertyConfiguration>((Expression<Func<DateTimePropertyConfiguration, byte?>>) (c => c.Precision), other1, ref errorMessage) ? 1 : 0;
      else
        num = 1;
      bool flag2 = num != 0;
      if (flag1)
        return flag2;
      return false;
    }
  }
}
