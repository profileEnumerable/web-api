// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.PropertyConvention
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  internal class PropertyConvention : PropertyConventionBase
  {
    private readonly Action<ConventionPrimitivePropertyConfiguration> _propertyConfigurationAction;

    public PropertyConvention(
      IEnumerable<Func<PropertyInfo, bool>> predicates,
      Action<ConventionPrimitivePropertyConfiguration> propertyConfigurationAction)
      : base(predicates)
    {
      this._propertyConfigurationAction = propertyConfigurationAction;
    }

    internal Action<ConventionPrimitivePropertyConfiguration> PropertyConfigurationAction
    {
      get
      {
        return this._propertyConfigurationAction;
      }
    }

    protected override void ApplyCore(
      PropertyInfo memberInfo,
      Func<System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration> configuration,
      System.Data.Entity.ModelConfiguration.Configuration.ModelConfiguration modelConfiguration)
    {
      this._propertyConfigurationAction(new ConventionPrimitivePropertyConfiguration(memberInfo, configuration));
    }
  }
}
