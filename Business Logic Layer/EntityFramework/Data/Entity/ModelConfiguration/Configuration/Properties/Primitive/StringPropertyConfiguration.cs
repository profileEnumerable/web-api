// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.StringPropertyConfiguration
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Linq.Expressions;

namespace System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive
{
  internal class StringPropertyConfiguration : LengthPropertyConfiguration
  {
    public bool? IsUnicode { get; set; }

    public StringPropertyConfiguration()
    {
    }

    private StringPropertyConfiguration(StringPropertyConfiguration source)
      : base((LengthPropertyConfiguration) source)
    {
      this.IsUnicode = source.IsUnicode;
    }

    internal override PrimitivePropertyConfiguration Clone()
    {
      return (PrimitivePropertyConfiguration) new StringPropertyConfiguration(this);
    }

    protected override void ConfigureProperty(EdmProperty property)
    {
      base.ConfigureProperty(property);
      if (!this.IsUnicode.HasValue)
        return;
      property.IsUnicode = this.IsUnicode;
    }

    internal override void Configure(EdmProperty column, FacetDescription facetDescription)
    {
      base.Configure(column, facetDescription);
      switch (facetDescription.FacetName)
      {
        case "Unicode":
          EdmProperty edmProperty = column;
          bool? nullable;
          if (!facetDescription.IsConstant)
          {
            bool? isUnicode = this.IsUnicode;
            nullable = isUnicode.HasValue ? new bool?(isUnicode.GetValueOrDefault()) : column.IsUnicode;
          }
          else
            nullable = new bool?();
          edmProperty.IsUnicode = nullable;
          break;
      }
    }

    internal override void CopyFrom(PrimitivePropertyConfiguration other)
    {
      base.CopyFrom(other);
      StringPropertyConfiguration propertyConfiguration = other as StringPropertyConfiguration;
      if (propertyConfiguration == null)
        return;
      this.IsUnicode = propertyConfiguration.IsUnicode;
    }

    internal override void FillFrom(PrimitivePropertyConfiguration other, bool inCSpace)
    {
      base.FillFrom(other, inCSpace);
      StringPropertyConfiguration propertyConfiguration = other as StringPropertyConfiguration;
      if (propertyConfiguration == null || this.IsUnicode.HasValue)
        return;
      this.IsUnicode = propertyConfiguration.IsUnicode;
    }

    internal override void MakeCompatibleWith(PrimitivePropertyConfiguration other, bool inCSpace)
    {
      base.MakeCompatibleWith(other, inCSpace);
      StringPropertyConfiguration propertyConfiguration = other as StringPropertyConfiguration;
      if (propertyConfiguration == null || !propertyConfiguration.IsUnicode.HasValue)
        return;
      this.IsUnicode = new bool?();
    }

    internal override bool IsCompatible(
      PrimitivePropertyConfiguration other,
      bool inCSpace,
      out string errorMessage)
    {
      StringPropertyConfiguration other1 = other as StringPropertyConfiguration;
      bool flag1 = base.IsCompatible(other, inCSpace, out errorMessage);
      int num;
      if (other1 != null)
        num = this.IsCompatible<bool, StringPropertyConfiguration>((Expression<Func<StringPropertyConfiguration, bool?>>) (c => c.IsUnicode), other1, ref errorMessage) ? 1 : 0;
      else
        num = 1;
      bool flag2 = num != 0;
      if (flag1)
        return flag2;
      return false;
    }
  }
}
