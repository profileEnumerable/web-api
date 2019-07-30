// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.DecimalPropertyConfiguration
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Linq.Expressions;

namespace System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive
{
  internal class DecimalPropertyConfiguration : PrimitivePropertyConfiguration
  {
    public byte? Precision { get; set; }

    public byte? Scale { get; set; }

    public DecimalPropertyConfiguration()
    {
    }

    private DecimalPropertyConfiguration(DecimalPropertyConfiguration source)
      : base((PrimitivePropertyConfiguration) source)
    {
      this.Precision = source.Precision;
      this.Scale = source.Scale;
    }

    internal override PrimitivePropertyConfiguration Clone()
    {
      return (PrimitivePropertyConfiguration) new DecimalPropertyConfiguration(this);
    }

    protected override void ConfigureProperty(EdmProperty property)
    {
      base.ConfigureProperty(property);
      byte? precision = this.Precision;
      if ((precision.HasValue ? new int?((int) precision.GetValueOrDefault()) : new int?()).HasValue)
        property.Precision = this.Precision;
      byte? scale = this.Scale;
      if (!(scale.HasValue ? new int?((int) scale.GetValueOrDefault()) : new int?()).HasValue)
        return;
      property.Scale = this.Scale;
    }

    internal override void Configure(EdmProperty column, FacetDescription facetDescription)
    {
      base.Configure(column, facetDescription);
      switch (facetDescription.FacetName)
      {
        case "Precision":
          EdmProperty edmProperty1 = column;
          byte? nullable1;
          if (!facetDescription.IsConstant)
          {
            byte? precision = this.Precision;
            nullable1 = precision.HasValue ? new byte?(precision.GetValueOrDefault()) : column.Precision;
          }
          else
            nullable1 = new byte?();
          edmProperty1.Precision = nullable1;
          break;
        case "Scale":
          EdmProperty edmProperty2 = column;
          byte? nullable2;
          if (!facetDescription.IsConstant)
          {
            byte? scale = this.Scale;
            nullable2 = scale.HasValue ? new byte?(scale.GetValueOrDefault()) : column.Scale;
          }
          else
            nullable2 = new byte?();
          edmProperty2.Scale = nullable2;
          break;
      }
    }

    internal override void CopyFrom(PrimitivePropertyConfiguration other)
    {
      base.CopyFrom(other);
      DecimalPropertyConfiguration propertyConfiguration = other as DecimalPropertyConfiguration;
      if (propertyConfiguration == null)
        return;
      this.Precision = propertyConfiguration.Precision;
      this.Scale = propertyConfiguration.Scale;
    }

    internal override void FillFrom(PrimitivePropertyConfiguration other, bool inCSpace)
    {
      base.FillFrom(other, inCSpace);
      DecimalPropertyConfiguration propertyConfiguration = other as DecimalPropertyConfiguration;
      if (propertyConfiguration == null)
        return;
      byte? precision = this.Precision;
      if (!(precision.HasValue ? new int?((int) precision.GetValueOrDefault()) : new int?()).HasValue)
        this.Precision = propertyConfiguration.Precision;
      byte? scale = this.Scale;
      if ((scale.HasValue ? new int?((int) scale.GetValueOrDefault()) : new int?()).HasValue)
        return;
      this.Scale = propertyConfiguration.Scale;
    }

    internal override void MakeCompatibleWith(PrimitivePropertyConfiguration other, bool inCSpace)
    {
      base.MakeCompatibleWith(other, inCSpace);
      DecimalPropertyConfiguration propertyConfiguration = other as DecimalPropertyConfiguration;
      if (propertyConfiguration == null)
        return;
      byte? precision = propertyConfiguration.Precision;
      if ((precision.HasValue ? new int?((int) precision.GetValueOrDefault()) : new int?()).HasValue)
        this.Precision = new byte?();
      byte? scale = propertyConfiguration.Scale;
      if (!(scale.HasValue ? new int?((int) scale.GetValueOrDefault()) : new int?()).HasValue)
        return;
      this.Scale = new byte?();
    }

    internal override bool IsCompatible(
      PrimitivePropertyConfiguration other,
      bool inCSpace,
      out string errorMessage)
    {
      DecimalPropertyConfiguration other1 = other as DecimalPropertyConfiguration;
      bool flag1 = base.IsCompatible(other, inCSpace, out errorMessage);
      int num1;
      if (other1 != null)
        num1 = this.IsCompatible<byte, DecimalPropertyConfiguration>((Expression<Func<DecimalPropertyConfiguration, byte?>>) (c => c.Precision), other1, ref errorMessage) ? 1 : 0;
      else
        num1 = 1;
      bool flag2 = num1 != 0;
      int num2;
      if (other1 != null)
        num2 = this.IsCompatible<byte, DecimalPropertyConfiguration>((Expression<Func<DecimalPropertyConfiguration, byte?>>) (c => c.Scale), other1, ref errorMessage) ? 1 : 0;
      else
        num2 = 1;
      bool flag3 = num2 != 0;
      if (flag1 && flag2)
        return flag3;
      return false;
    }
  }
}
